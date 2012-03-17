using System.Collections.Generic;
using System.Linq;

namespace RazorPad.Web.Website.Models
{
    public class UserFiddlesViewModel
    {
        public string Username { get; set; }

        public IEnumerable<FiddleViewModel> Fiddles { get; set; }


        public UserFiddlesViewModel()
        {
            Fiddles = Enumerable.Empty<FiddleViewModel>();
        }
    }
}