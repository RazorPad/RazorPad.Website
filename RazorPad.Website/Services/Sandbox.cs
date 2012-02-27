using System;
using RazorPad.Compilation;
using RazorPad.Compilation.Hosts;

namespace RazorPad.Website.Services
{
    public class Sandbox : MarshalByRefObject
    {
        public static string Execute(TemplateLanguage language, string template, dynamic model = null)
        {
            var templateParams = TemplateCompilationParameters.CreateFromLanguage(language);

            var compiler = new TemplateCompiler(templateParams);

            // TODO: Run this in a sandbox
            return compiler.Execute(template, model, new RazorPadMvcEngineHost(templateParams.Language));
        }
    }
}