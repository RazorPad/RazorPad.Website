using System.Web.Razor;
using RazorPad.Compilation;

namespace RazorPad.Web.Website.Models
{
    public class SaveRequest : ParseRequest
    {

        public TemplateLanguage Language { get; set; }
        
        public string Model { get; set; }

        public string Notes { get; set; }

        public RazorCodeLanguage RazorLanguage
        {
            get
            {
                switch (Language)
                {
                    case(TemplateLanguage.VisualBasic):
                        return new VBRazorCodeLanguage();
                    default:
                        return new CSharpRazorCodeLanguage();
                }
            }
            set { }
        }

        public string SnippetId { get; set; }

        public string Title { get; set; }

        public uint Revision { get; set; }
    }

    
}