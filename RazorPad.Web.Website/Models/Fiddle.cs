using System;
using System.Collections.Generic;
using RazorPad.DynamicModel;
using System.Web.Script.Serialization;
namespace RazorPad.Website.Models
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