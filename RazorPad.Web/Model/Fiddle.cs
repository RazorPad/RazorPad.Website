using System;
using System.Web.Script.Serialization;
using RazorPad.Web.Dynamic;

namespace RazorPad.Web
{
    public class Fiddle
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public string View { get; set; }
        public string InputModel { get; set; }
        public dynamic DynamicInputModel
        {
            get
            {
                if (string.IsNullOrEmpty(InputModel))
                    return null;

                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

                return jss.Deserialize(InputModel, typeof(object));
            }
        }
        public string Language { get; set; }
        public string Version { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
    }
}