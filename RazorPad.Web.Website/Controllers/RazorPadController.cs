using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Raven.Abstractions.Data;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;
using RazorPad.Web.Dynamic;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models.RazorPad;
using RazorPad.Web.RavenDb;

namespace RazorPad.Web.Website.Controllers
{
    [ValidateInput(false)]
    public class RazorPadController : Controller
    {
        public ActionResult Index(string id)
        {
            Fiddle fiddle = null;
            if (!string.IsNullOrEmpty(id))
            {
                var session = DataDocumentStore.OpenSession();
                fiddle = session.Load<Fiddle>(id);
                session.Dispose();
            }
            return View("MainUI", fiddle ?? new Fiddle());
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
            var session = DataDocumentStore.OpenSession();
            

            var fiddleId = request.FiddleId;
            if (string.IsNullOrEmpty(fiddleId))
            {
                var fiddle = new Fiddle
                {
                    View = request.Template,
                    InputModel = request.Model,
                    Language = request.Language.ToString(),
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = ""//ToDo: Set Created By
                };

                session.Store(fiddle);
                session.SaveChanges();
                fiddleId = session.Advanced.GetDocumentId(fiddle);
            }
            else
            {
                session.Advanced.DatabaseCommands.Patch(
                    fiddleId,
                    new[]
                        {
                            new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "View",
                                    Value = request.Template
                                },
                            new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "InputModel",
                                    Value = request.Model
                                }
                        });
                session.SaveChanges();
            }
            session.Dispose();
    
            return Json(fiddleId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new TemplateMessage { Kind = TemplateMessageKind.Error, Text = filterContext.Exception.ToString() };
            filterContext.Result = Json(new ParseResult { Success = false, Messages = new[] { error } }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

    }
}
