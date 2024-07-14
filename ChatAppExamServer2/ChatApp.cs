using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace ChatAppExamServer
{
    internal class ChatApp
    {
        private Socket listenerSocket;
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private Dictionary<string, User> onlineUsers = new Dictionary<string, User>();
        private const int BUFFER_SIZE = 1024;
        private string clientsFile = "C:\\Users\\hp\\source\\repos\\ChatAppExam\\clients.json.txt";
        private string fileDirectory = "C:\\Users\\hp\\source\\repos\\ChatAppExam";

        public ChatApp(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(ipAddress, port);
            listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(endpoint);
            LoadClientsFromFile();
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

        private void SaveClientsToFile()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(clientsFile, jsonString);
                Console.WriteLine("Clients saved to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving clients to file: {ex.Message}");
            }
        }

        private void LoadClientsFromFile()
        {
            try
            {
                string jsonString = File.ReadAllText(clientsFile);
                users = JsonSerializer.Deserialize<Dictionary<string, User>>(jsonString);
                Console.WriteLine("Client list loaded");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Clients file not found. Creating new list.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading clients from file: {ex.Message}");
            }
        }

        public void HandleClient(Socket clientSocket)
        {
            try
            {
                using (NetworkStream stream = new NetworkStream(clientSocket))
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    writer.WriteLine("~~~ Welcome To The Chat ~~~");
                    writer.WriteLine("Type \"register\" to register or \"login\" to log in");

                    RegisterLoginGetResponse(reader, writer, clientSocket);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Client disconnected: {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public void HandleClient(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("~~~ Welcome To The Chat ~~~ type \"register\" to register or \"login\" to log in");;

            try
            {
                RegisterLoginGetResponse(reader, writer, clientSocket);
            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.Message);
            }
        }

        public void RegisterLoginGetResponse(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            string response = reader.ReadLine();

            bool notIn = true;
            while (notIn)
            {

                if (response.ToLower().Contains("register"))
                {
                    RegisterUser(reader, writer, clientSocket);
                    notIn = false;
                }
                else if (response.ToLower().Contains("login"))
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

        public void ReceiveFile(string fileName, int fileSize, StreamReader reader)
        {
            try
            {
                string savePath = Path.Combine(fileDirectory, fileName);
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;
                    int totalBytesRead = 0;

                    while (totalBytesRead < fileSize && (bytesRead = reader.BaseStream.Read(buffer, 0, Math.Min(buffer.Length, fileSize - totalBytesRead))) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                    }
                }
                Console.WriteLine($"File received: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving file {fileName}: {ex.Message}");
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
                    User user = new User(username, password, writer);
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

            SaveClientsToFile();
            HandleClient(reader, writer, clientSocket);
        }

        public void LoginUser(StreamReader reader, StreamWriter writer, Socket clientSocket)
        {
            writer.WriteLine("Enter your username or enter \"stoplogin\" to exit to the menu:");
            string username = reader.ReadLine();
            bool valid = false;


            while (!valid)
            {
                if (username.Contains("stoplogin"))
                {
                    writer.WriteLine("Returning to the main menu...");
                    HandleClient(reader, writer, clientSocket);
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
            user.Writer = writer;
            onlineUsers[username] = user;

            var contacts = user.ContactUsernames;
            string contactsJson = JsonSerializer.Serialize(contacts);
            writer.WriteLine(contactsJson);

            UserSession(reader, writer, clientSocket, user);
        }

        public void UserSession(StreamReader reader, StreamWriter writer, Socket clientSocket, User user)
        {
            writer.WriteLine("user session started, type \"exit\" to log out");

            writer.WriteLine("To send a message, use the format: @username: message");

            string message;
            while ((message = reader.ReadLine()) != null)
            {
                if (message.ToLower().Contains("exit"))
                {
                    writer.WriteLine("Logging out...");
                    onlineUsers.Remove(user.Username);
                    HandleClient(reader, writer, clientSocket);
                    return;
                }
                if (message.StartsWith("FILE:"))
                {
                    string[] parts = message.Split(':');
                    string fileName = parts[1].Trim();
                    int fileSize = int.Parse(parts[2].Trim());

                    ReceiveFile(fileName, fileSize, reader);

                    BroadcastMessage($"File received from {user.Username}: {fileName}");
                }
                if (message.StartsWith("ROUTE:"))
                {
                    string[] parts = message.Split(":");
                    string recepientUsername = parts[1].Trim();
                    message = parts[2].Trim();

                    RouteMessage(user.Username, recepientUsername, message);
                }
                else if (message.StartsWith("@"))
                {
                    int separatorIndex = message.IndexOf(':');
                    if (separatorIndex > 1)
                    {
                        string recipientUsername = message.Substring(1, separatorIndex - 1).Trim();
                        string actualMessage = message.Substring(separatorIndex + 1).Trim();

                        try
                        {
                            User recipient = onlineUsers[recipientUsername];
                            recipient.Writer.WriteLine($"{user.Username}: {actualMessage}");
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine($"User {recipientUsername} is not online.");
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        public void RouteMessage(string senderUsername, string receiverUsername, string message)
        {
            if (onlineUsers.ContainsKey(receiverUsername))
            {
                var receiver = onlineUsers[receiverUsername];
                receiver.Writer.WriteLine($"[{senderUsername}]: {message}");
            }
            else
            {
                Console.WriteLine($"User '{receiverUsername}' is not online.");
            }
        }

        public void BroadcastMessage(string message)
        {
            foreach (var user in onlineUsers.Values)
            {
                user.Writer.WriteLine(message);
            }
        }
    }
}