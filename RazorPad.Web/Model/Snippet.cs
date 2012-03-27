using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RazorPad.Web.Services;

namespace RazorPad.Web
{
    public class Snippet
    {
        public string Id { get; private set; }

        [Required]
        public string Key
        {
            get { return _key = _key ?? new UniqueKeyGenerator().Generate(); }
            private set { _key = value; }
        }
        private string _key;

        public string Title { get; set; }
        
        public string Notes { get; set; }

        [Required]
        [StringLength(50000, ErrorMessage = "You can't save this view -- it's way too big!  Try something under 50k characters.")]
        public string View { get; set; }

        [StringLength(25000, ErrorMessage = "You can't save this model -- it's way too big!  Try something under 25k characters.")]
        public string Model { get; set; }

        public string Language { get; set; }

        [Required]
        public DateTime DateCreated { get; private set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public string Owner { get; set; }


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
            return repository.Query<Snippet>().Where(x => x.CreatedBy == username || x.Owner == username);
        }

    }
}