using System.Net.Sockets;
using System.Net;

namespace ChatExamClientConsole
{
    internal class ChatClient
    {
        private Socket clientSocket;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private readonly Thread receiveThread;

        public ChatClient(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            var endpoint = new IPEndPoint(ipAddress, port);

            clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endpoint);

            var stream = new NetworkStream(clientSocket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    Console.Write("> ");
                    string input = Console.ReadLine();
                    writer.WriteLine(input);

                    if (input?.ToLower() == "exit")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    string serverResponse = reader.ReadLine();
                    if (serverResponse != null)
                    {
                        Console.WriteLine(serverResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
