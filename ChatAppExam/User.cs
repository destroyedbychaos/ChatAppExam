namespace ChatAppExam
{
    internal class User
    {
        private string username { get; set; }
        private string password { get; set; }
        private string email { get; set; }
        private Dictionary<string, User> Contacts { get; set; }
        User(string username, string password, string email)
        {
            this.username = username;
            this.password = password;
            this.email = email;
            Contacts = new Dictionary<string, User>();
        }
    }
}
