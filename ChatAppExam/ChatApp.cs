using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatAppExam
{
    internal class ChatApp
    {
        private Socket listenerSocket;
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private Dictionary<string, User> onlineUsers = new Dictionary<string, User>();
        private const int BUFFER_SIZE = 1024;

        public ChatApp(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(ipAddress, port);
            listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(endpoint);
        }

        public void StartChat()
        {
            listenerSocket.Listen(10);
            Console.WriteLine("chat started");

            while (true)
            {
                Socket clientSocket = listenerSocket.Accept();
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        public void HandleClient(Socket clientSocket)
        {
            NetworkStream stream = new NetworkStream(clientSocket);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) 
            { 
                AutoFlush = true 
            };

            writer.WriteLine("~~~ Welcome To The Chat ~~~");
            writer.WriteLine("type \"register\" to register or \"login\" to log in");

            string response = reader.ReadLine();

            try
            {
                bool notIn = true;
                while (notIn)
                {
                    
                    if (response.ToLower() == "register")
                    {
                        RegisterUser(reader, writer, clientSocket);
                        notIn = false;
                    }
                    else if (response.ToLower() == "login")
                    {
                        LoginUser(reader, writer, clientSocket);
                        notIn = false;
                    }
                    else
                    {
                        writer.WriteLine("Invalid response. Try again: ");
                    }
                    response = reader.ReadLine();
                }

            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.Message);
            }
            finally
            {
                stream.Close();
                clientSocket.Close();
            }
        }

        public void RegisterUser(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("Here will be registering");
        }

        public void LoginUser(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("Here will be logging in");
        }
    }
}