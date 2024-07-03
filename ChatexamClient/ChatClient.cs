using System.Net.Sockets;
using System.Net;

namespace ChatexamClient
{
    internal class ChatClient
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
            writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void Start()
        {
            Console.WriteLine(reader.ReadLine());

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                writer.WriteLine(input);

                string serverResponse = reader.ReadLine();
                Console.WriteLine(serverResponse);
            }
        }

        public void Close()
        {
            writer.Close();
            reader.Close();
            clientSocket.Close();
        }
    }
}
