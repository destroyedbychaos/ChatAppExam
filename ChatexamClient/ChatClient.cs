using System.Net.Sockets;
using System.Net;

namespace ChatexamClient
{
    internal class ChatClient
    {
        private Socket clientSocket;
        private StreamReader reader;
        private StreamWriter writer;
        private Form1 form;
        public ChatClient(string ip, int port, Form1 form)
        {
            this.form = form;

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
            form.UpdateChat(reader.ReadLine());

            form.UpdateChat(reader.ReadLine());

            while (true)
            {
                string response = reader.ReadLine();
                form.UpdateChat("Me: " + response);
            }
        }

        public void SendMessage(string message)
        {
            writer.WriteLine(message);
        }

        public void Close()
        {
            writer.Close();
            reader.Close();
            clientSocket.Close();
        }
    }
}
