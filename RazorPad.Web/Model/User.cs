using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RazorPad.Web
{
    public class User
    {
        public long Id { get; private set; }

        public ICollection<Credential> Credentials { get; set; }

        public DateTime DateCreated { get; set; }

        public string EmailAddress { get; set; }

        public string Username { get; set; }


        public User()
        {
            Credentials = new Collection<Credential>();
        }
    }
}