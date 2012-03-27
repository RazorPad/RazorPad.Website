using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using RazorPad.Web.Dynamic;

namespace RazorPad.Web.Website.Models
{
    public class SnippetViewModel
    {
        public string Key { get; set; }
        
        public string View { get; set; }
        
        public string Model { get; set; }
        
        public string Owner { get; set; }
        
        public string CreatedBy { get; set; }

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

        public string Title
        {
            get
            {
                return string.IsNullOrEmpty(_title) ? Key : _title;
            }
            set { _title = value; }
        }
        private string _title;

        public string Notes { get; set; }

        public DateTime? DateCreated { get; set; }

        public SnippetViewModel()
        {
            Model = string.Empty;
            Owner = string.Empty;
            Title = string.Empty;
            View = string.Empty;
        }

        public SnippetViewModel(Snippet snippet)
        {
            if(snippet == null)
                return;

            CreatedBy = snippet.CreatedBy;
            DateCreated = snippet.DateCreated;
            Key = snippet.Key;
            Model = snippet.Model;
            Owner = snippet.Owner;
            Title = snippet.Title;
            View = snippet.View;
            Notes = snippet.Notes;
        }
    }
}