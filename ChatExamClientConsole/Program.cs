using System.Net.Sockets;
using System.Net;

namespace ChatExamClientConsole
{
    class ChatClient
    {
        private Socket clientSocket;
        private StreamReader reader;
        private StreamWriter writer;

        public ChatClient(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(ipAddress, port);

            clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endpoint);

            NetworkStream stream = new NetworkStream(clientSocket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };
        }

        public void Start()
        {
            string serverResponse = reader.ReadLine();
            Console.WriteLine(serverResponse);
            serverResponse = reader.ReadLine();
            Console.WriteLine(serverResponse);

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                writer.WriteLine(input);
                try
                {
                    serverResponse = reader.ReadLine();
                    Console.WriteLine(serverResponse);
                }
                catch(Exception ex)
                {
                    writer.WriteLine(ex.Message);
                }
                
            }
        }

        public void Close()
        {
            writer.Close();
            reader.Close();
            clientSocket.Close();
        }
    }
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