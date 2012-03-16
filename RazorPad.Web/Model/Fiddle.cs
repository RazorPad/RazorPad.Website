using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using RazorPad.Web.Dynamic;

namespace RazorPad.Web
{
    public class Fiddle
    {
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Notes { get; set; }

        [StringLength(50000, ErrorMessage = "You can't save this view -- it's way too big!  Try something under 50k characters.")]
        public string View { get; set; }

        [StringLength(25000, ErrorMessage = "You can't save this model -- it's way too big!  Try something under 25k characters.")]
        public string InputModel { get; set; }

        public dynamic DynamicInputModel
        {
            get
            {
                if (string.IsNullOrEmpty(InputModel))
                    return null;

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

                return serializer.Deserialize(InputModel, typeof(object));
            }
        }

        public string Language { get; set; }

        public string Version { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }
    }
}