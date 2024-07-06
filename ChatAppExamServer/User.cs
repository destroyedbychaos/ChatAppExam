using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppExamServer
{
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Dictionary<string, User> Contacts { get; set; }
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            Contacts = new Dictionary<string, User>();
        }
    }
}
