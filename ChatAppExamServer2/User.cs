using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChatAppExamServer
{
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Dictionary<string, User> Contacts { get; set; }

        [JsonIgnore]
        public StreamWriter Writer { get; set; }
        public User(string username, string password, StreamWriter writer)
        {
            this.Username = username;
            this.Password = password;
            Contacts = new Dictionary<string, User>();
            Writer = writer;
        }

        public override string ToString()
        {
            return $"{Username} has {Contacts.Count} contacts";
        }
    }
}
