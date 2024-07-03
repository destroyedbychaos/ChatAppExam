namespace ChatAppExam
{
    class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public Dictionary<string, User> Contacts { get; set; }
        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
            Contacts = new Dictionary<string, User>();
        }
    }
}
