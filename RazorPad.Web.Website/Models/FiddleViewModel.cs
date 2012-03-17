using System.Collections.Generic;
using System.Web.Script.Serialization;
using RazorPad.Web.Dynamic;

namespace RazorPad.Web.Website.Models
{
    public class FiddleViewModel
    {
        public string Key { get; set; }
        
        public string View { get; set; }
        
        public string Model { get; set; }

        public IDictionary<string, object> ModelProperties
        {
            get
            {
                if (string.IsNullOrEmpty(Model))
                    return new Dictionary<string, object>();

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

                dynamic model = serializer.Deserialize(Model, typeof(object));

                return model.GetProperties();
            }
        }

        public bool IsNew
        {
            get
            {
                return string.IsNullOrWhiteSpace(Key);
            }
        }

        public FiddleViewModel()
        {
            Model = string.Empty;
            View = string.Empty;
        }
    }
}