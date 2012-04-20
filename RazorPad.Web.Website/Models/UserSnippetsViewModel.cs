using System.Collections.Generic;
using System.Linq;

namespace RazorPad.Web.Website.Models
{
    public class UserSnippetsViewModel
    {
        public string Username { get; set; }
        
        public IEnumerable<SnippetViewModel> Snippets { get; set; }


        public UserSnippetsViewModel()
        {
        }

        public UserSnippetsViewModel(string username, IEnumerable<Snippet> snippets)
            : this(username, (snippets ?? Enumerable.Empty<Snippet>()).Select(x => new SnippetViewModel(x)))
        {
        }

        public UserSnippetsViewModel(string username, IEnumerable<SnippetViewModel> snippets)
        {
            Username = username;
            Snippets = snippets ?? Enumerable.Empty<SnippetViewModel>();
        }
    }
}