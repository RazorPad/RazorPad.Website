using System;
using System.ComponentModel.DataAnnotations;
using RazorPad.Web.Services;

namespace RazorPad.Web
{
    public class Fiddle
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


        public Fiddle()
        {
            DateCreated = DateTime.UtcNow;
        }
    }

    public static class FiddleRepositoryExtensions
    {
        
        public static Fiddle FindFiddle(this IRepository repository, string key)
        {
            return repository.SingleOrDefault<Fiddle>(x => x.Key == key);
        }

    }
}