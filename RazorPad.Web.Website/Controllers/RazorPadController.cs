using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using System.Web.Razor;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Website.Controllers
{
    [ValidateInput(false)]
    public class RazorPadController : Controller
    {
        private const string AnonymousUsername = "Anonymous";
        private readonly IRepository _repository;

        public RazorPadController(IRepository repository)
        {
            _repository = repository;
        }


        public ActionResult Index(string id)
        {
            var snippet = _repository.FindSnippet(id);

            return MainUI(snippet);
        }

        public ActionResult Parse([Bind(Prefix = "")]ParseRequest request)
        {
            var result = new ParseResult();

            using (var writer = new StringWriter())
            {
                var generatorResults = new TemplateCompiler().GenerateCode(request.Template, writer);
                result.SetGeneratorResults(generatorResults);
                result.GeneratedCode = writer.ToString();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Execute([Bind(Prefix = "")]ExecuteRequest request)
        {
            var result = new ExecuteResult();

            var template = request.Template;
            var templateParams = TemplateCompilationParameters.CreateFromLanguage(request.Language);
            var compiler = new TemplateCompiler(templateParams);

            dynamic inputModel = null;

            GeneratorResults generatorResults;
            using (var writer = new StringWriter())
            {
                generatorResults = compiler.GenerateCode(template, writer, new RazorPadMvcEngineHost(request.RazorLanguage));
                result.SetGeneratorResults(generatorResults);
                result.GeneratedCode = writer.ToString();
            }

            if (generatorResults != null && generatorResults.Success)
            {
                CompilerResults compilerResults = compiler.Compile(generatorResults);

                result.SetCompilerResults(compilerResults);

                if (!compilerResults.Errors.HasErrors)
                {
                    result.TemplateOutput = Sandbox.Execute(request.Language, template, inputModel);
                }

                result.Success = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Clone([Bind(Prefix = "")]SaveRequest request)
        {
            return Save(request, clone: true);
        }

        public ActionResult Save([Bind(Prefix = "")]SaveRequest request, bool clone = false)
        {
            Snippet snippet = null;

            var snippetExists = !string.IsNullOrWhiteSpace(request.SnippetId);

            if (snippetExists)
            {
                snippet = _repository.FindSnippet(request.SnippetId);
                snippetExists = (snippet != null);
            }

            var username = User.Identity.IsAuthenticated ? User.Identity.Name : AnonymousUsername;

            // See if we are cloning or not
            if (snippetExists)
            {
                bool userOwnsSnippet;

                if (User.Identity.IsAuthenticated)
                {
                    userOwnsSnippet = snippet.CreatedBy.Equals(username, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    // Anonymous users can't own each other's snippets 
                    // (and we haven't implemented a way to track our own anonymous snippets)
                    // TODO: Update this when Anonymous users track their own snippets
                    userOwnsSnippet = false;
                }

                clone = clone || !userOwnsSnippet;
            }
            
            var shouldCreateNewSnippet = !snippetExists || clone;

            if (shouldCreateNewSnippet)
            {
                if(clone)
                {
                    request.CloneOf = request.SnippetId;
                }

                snippet = new Snippet
                              {
                                  CloneOf = request.CloneOf,
                                  CreatedBy = username,
                                  Language = request.Language,
                                  Model = request.Model,
                                  Notes = request.Notes,
                                  Title = request.Title,
                                  View = request.Template,
                              };

                _repository.Save(snippet);
            }
            else
            {
                snippet.Model = request.Model;
                snippet.Notes = request.Notes;
                snippet.Title = request.Title;
                snippet.View = request.Template;
            }

            return MainUI(snippet);
        }

        protected ActionResult MainUI(Snippet snippet)
        {
            var viewModel = new SnippetViewModel(snippet);

            if (Request.IsAjaxRequest())
                return Json(viewModel);

            return View("MainUI", viewModel);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
