namespace ChatExamClientConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChatClient client = new ChatClient("127.0.0.1", 50000);
            client.Start();

            client.Close();
        }
    }
}