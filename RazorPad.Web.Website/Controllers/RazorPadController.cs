using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Website.Controllers
{
    [ValidateInput(false)]
    public class RazorPadController : Controller
    {
        private readonly IRepository _repository;

        public RazorPadController(IRepository repository)
        {
            _repository = repository;
        }


        public ActionResult Index(string id)
        {
            var snippet = _repository.FindSnippet(id);
            
            return View("MainUI", new SnippetViewModel(snippet));
        }


        public ActionResult Parse([Bind(Prefix = "")]ParseRequest request)
        {
            ParseResult result = new ParseResult();
            var writer = new StringWriter();
            var generatorResults = new TemplateCompiler().GenerateCode(request.Template, writer);
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = writer.ToString();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Execute([Bind(Prefix = "")]ExecuteRequest request)
        {
            ExecuteResult result = new ExecuteResult();

            var templateParams = TemplateCompilationParameters.CreateFromLanguage(request.Language);
            var compiler = new TemplateCompiler(templateParams);

            var writer = new StringWriter();

            dynamic inputModel = null;
            
            var templ = request.Template;
            var generatorResults = compiler.GenerateCode(templ, writer, new RazorPadMvcEngineHost(request.RazorLanguage));
            result.SetGeneratorResults(generatorResults);
            result.GeneratedCode = writer.ToString();

            if (generatorResults.Success)
            {
                CompilerResults compilerResults = compiler.Compile(generatorResults);

                result.SetCompilerResults(compilerResults);

                if (!compilerResults.Errors.HasErrors)
                {
                    result.TemplateOutput = Sandbox.Execute(request.Language, templ, inputModel);
                }

                result.Success = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Clone([Bind(Prefix = "")]SaveRequest request)
        {
            request.CloneOf = request.SnippetId;
            request.SnippetId = null;

            return Save(request);
        }

        public ActionResult Save([Bind(Prefix = "")]SaveRequest request)
        {
            var snippet = _repository.SingleOrDefault<Snippet>(f => f.Key == request.SnippetId);

            if (snippet == null)
            {
                var username = User.Identity.IsAuthenticated ? User.Identity.Name : "Anonymous";

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

            return Json(snippet.Key);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
