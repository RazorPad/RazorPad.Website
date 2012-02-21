using System;
namespace RazorPad.Website.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
    }
}