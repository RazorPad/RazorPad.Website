using System.CodeDom.Compiler;
using System.Collections.Generic;
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
        private readonly IRepository _repository;

        public RazorPadController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index(string id)
        {
            var snippet = _repository.FindSnippet(id);

            var viewModel = new SnippetViewModel(snippet);

            if (Request.IsAjaxRequest())
                return Json(viewModel);

            return View("MainUI", viewModel);
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

        public ActionResult BrowserView(string template)
        {
            //This is required otherwise Chrom does not executes the script thinking it as cross-site attack
            Response.Headers.Add("X-XSS-Protection", "0");
            return View("BrowserView", "", template);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
