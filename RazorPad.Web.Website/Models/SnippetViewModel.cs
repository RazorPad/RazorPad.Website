using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using RazorPad.Core;
using RazorPad.Web.Dynamic;

namespace RazorPad.Web.Website.Models
{
    public class SnippetViewModel
    {
        public string CreatedBy { get; set; }
        
        public DateTime? DateCreated { get; set; }
        
        public string DisplayName
        {
            get
            {
                return string.IsNullOrWhiteSpace(Title) ? Key : Title;
            }
        }

        public string DisplayDate
        {
            get
            {
                if(DateCreated == null)
                    return string.Empty;
                
                return DateCreated.Value.ToString("G");
            }
        }

        public bool IsNew
        {
            get
            {
                return string.IsNullOrWhiteSpace(Key);
            }
        }

        public string Key { get; set; }

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
        
        public string Notes { get; set; }
        
        public string NotesSummary
        {
            get
            {
                return (Notes ?? string.Empty).TruncateAtWord(30);
            }
        }
        
        public string Title { get; set; }

        public string View { get; set; }


        public SnippetViewModel()
        {
            Model = string.Empty;
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
            Title = snippet.Title;
            View = snippet.View;
            Notes = snippet.Notes;
        }
    }
}