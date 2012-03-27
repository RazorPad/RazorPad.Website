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
            Snippets = Enumerable.Empty<SnippetViewModel>();
        }
    }
}