using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RazorPad.Compilation;
using RazorPad.Web.Services;

namespace RazorPad.Web
{
    public class Snippet
    {
        public long Id { get; private set; }

        public string CloneOf { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime DateCreated { get; private set; }

        [Required]
        public string Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_key))
                    _key = new UniqueKeyGenerator().Generate();

                return _key;
            }
            private set { _key = value; }
        }
        private string _key;

        public TemplateLanguage Language { get; set; }

        [StringLength(25000, ErrorMessage = "You can't save this model -- it's way too big!  Try something under 25k characters.")]
        public string Model { get; set; }

        [StringLength(1000, ErrorMessage = "Your note is too long - try to think of something under 1k characters")]
        public string Notes { get; set; }

        [StringLength(500, ErrorMessage = "Your title is too long - try to think of something under 500 characters")]
        public string Title { get; set; }

        [Required]
        [StringLength(50000, ErrorMessage = "You can't save this view -- it's way too big!  Try something under 50k characters.")]
        public string View { get; set; }


        public Snippet()
        {
            DateCreated = DateTime.UtcNow;
        }
    }

    public static class SnippetRepositoryExtensions
    {
        
        public static Snippet FindSnippet(this IRepository repository, string key)
        {
            return repository.SingleOrDefault<Snippet>(x => x.Key == key);
        }

        public static IEnumerable<Snippet> FindSnippetsByUsername(this IRepository repository, string username)
        {
            return repository.Query<Snippet>().Where(x => x.CreatedBy == username);
        }

    }
}