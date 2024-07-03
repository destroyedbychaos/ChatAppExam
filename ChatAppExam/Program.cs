namespace ChatAppExam
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ChatApp chat = new ChatApp("127.0.0.1", 50000);

            chat.StartChat();
        }
    }
}
