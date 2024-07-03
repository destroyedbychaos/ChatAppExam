namespace ChatexamClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new ChatexamClient.Form1());
            ChatClient client = new ChatClient("127.0.0.1", 50000);
            client.Start();

            client.Close();
        }
    }
}