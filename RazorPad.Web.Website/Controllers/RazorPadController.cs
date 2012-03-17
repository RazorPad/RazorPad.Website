using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;
using RazorPad.Web.Dynamic;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;
using RazorPad.Web.Website.Models.RazorPad;

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
            var fiddle = _repository.FindFiddle(id);
            
            return View("MainUI", new FiddleViewModel(fiddle));
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

            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

            dynamic inputModel = jss.Deserialize(request.Model, typeof(object));
            
            var templ = request.Template;
            var generatorResults = compiler.GenerateCode(templ, writer, new RazorPadMvcEngineHost(request.RazorLanguage));
            result.SetGeneratorResults(generatorResults);
            // TODO: Extract the right stuff
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
            
        public JsonResult Save([Bind(Prefix = "")]SaveRequest request)
        {
            var fiddle = _repository.SingleOrDefault<Fiddle>(f => f.Key == request.FiddleId);

            if (fiddle == null)
            {
                var username = User.Identity.IsAuthenticated ? User.Identity.Name : "Anonymous";

                fiddle = new Fiddle
                {
                    View = request.Template,
                    Model = request.Model,
                    Language = request.Language.ToString(),
                    CreatedBy = username
                };

                _repository.Save(fiddle);
            }
            else
            {
                fiddle.View = request.Template;
                fiddle.Model = request.Model;
            }

            _repository.SaveChanges();

            return Json(fiddle.Key);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
