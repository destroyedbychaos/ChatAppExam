using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatAppExamServer
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
                RegisterLoginGetResponse(reader, writer, clientSocket, response);
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
        public void RegisterLoginGetResponse(StreamReader reader, StreamWriter writer, Socket clientSocket, string response)
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

        public void HandleClient(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("~~~ Welcome To The Chat ~~~ type \"register\" to register or \"login\" to log in");

            string response = reader.ReadLine();

            try
            {
                RegisterLoginGetResponse(reader, writer, clientSocket, response);
            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.Message);
            }
            finally
            {
                reader.Close();
                writer.Close();
                clientSocket.Close();
            }
        }

        public void RegisterUser(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("Enter a username:");
            string username = reader.ReadLine();

            bool Valid = false;

            while (!Valid)
            {
                if (users.ContainsKey(username))
                {
                    writer.WriteLine("This username is already taken. Try again.");
                    username = reader.ReadLine();
                }
                Valid = true;
            }

            writer.WriteLine("Enter a password with 8 numbers, 1 uppercase, and 1 lowercase letter.");
            string password = reader.ReadLine();

            var numberRegex = new Regex(@"(\D*\d\D*){8,}");
            var upperRegex = new Regex(@"[A-Z]");
            var lowerRegex = new Regex(@"[a-z]");
            Valid = false;

            while (!Valid)
            {
                if (numberRegex.IsMatch(password) && upperRegex.IsMatch(password) && lowerRegex.IsMatch(password))
                {
                    writer.WriteLine("Success! Press enter to return to the menu");
                    User user = new User(username, password);
                    users.Add(username, user);
                    Valid = true;
                }
                else if (!numberRegex.IsMatch(password) && !upperRegex.IsMatch(password) && !lowerRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 8 numbers, 1 uppercase, and 1 lowercase letter. Try Again.");
                }
                else if (!upperRegex.IsMatch(password) && !lowerRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 1 uppercase and 1 lowercase letters. Try again.");
                }
                else if (!numberRegex.IsMatch(password) && !upperRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 8 numbers and 1 uppercase letter. Try again.");
                }
                else if (!numberRegex.IsMatch(password) && !lowerRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 8 numbers and 1 lowercase letter. Try again.");
                }
                else if (!numberRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 8 numbers. Try again.");
                }
                else if (!upperRegex.IsMatch(password))
                {
                    writer.WriteLine("The passowrd must contain at least 1 uppercase letter. Try again.");
                }
                else if (!lowerRegex.IsMatch(password))
                {
                    writer.WriteLine("The password must contain at least 1 lowercase letter. Try again.");
                }

                password = reader.ReadLine();
            }

            HandleClient(reader, writer, clientSocket);
        }

        public void LoginUser(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("Enter your username or enter \"stoplogin\" to exit to the menu:");
            string username = reader.ReadLine();
            bool valid = false;

            while (!valid)
            {
                if (username == "stoplogin")
                {
                    writer.WriteLine("Returning to the main menu...");
                    RegisterLoginGetResponse(reader, writer, clientSocket, string.Empty);
                    return;
                }
                else if (!users.ContainsKey(username))
                {
                    writer.WriteLine("Username not found. Try again or enter \"stoplogin\" to exit to the menu:");
                    username = reader.ReadLine();
                }
                else
                {
                    valid = true;
                }
            }

            writer.WriteLine("Enter your password:");
            string password = reader.ReadLine();
            User user = users[username];
            valid = false;

            while (!valid)
            {
                if (user.Password != password)
                {
                    writer.WriteLine("Wrong password. Enter 1 to try again or 2 to register:");
                    string input = reader.ReadLine();
                    int choice;

                    if (int.TryParse(input, out choice))
                    {
                        if (choice == 1)
                        {
                            writer.WriteLine("Enter your password:");
                            password = reader.ReadLine();
                        }
                        else if (choice == 2)
                        {
                            RegisterUser(reader, writer, clientSocket);
                            return;
                        }
                        else
                        {
                            writer.WriteLine("Invalid choice. Please enter 1 or 2:");
                        }
                    }
                    else
                    {
                        writer.WriteLine("Invalid input. Please enter a number (1 or 2):");
                    }
                }
                else
                {
                    valid = true;
                }
            }

            writer.WriteLine("Login was successful!");
            onlineUsers[username] = user;
            UserSession(reader, writer, clientSocket, user);
        }

        public void UserSession(StreamReader reader, StreamWriter writer, Socket clientSocket, User user)
        {
            writer.WriteLine("user session started");
        }
    }
}