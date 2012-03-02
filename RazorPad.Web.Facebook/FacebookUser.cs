using System.Runtime.Serialization;

namespace RazorPad.Web.Facebook
{
    [DataContract]
    public class FacebookUser
    {
        [DataMember(Name = "email")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public uint UserId { get; set; }
    }
}