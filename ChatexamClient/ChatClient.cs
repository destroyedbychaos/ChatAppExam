using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text.Json;

namespace ChatexamClient
{
    internal class ChatClient
    {
        private Socket clientSocket;
        private StreamReader reader;
        private StreamWriter writer;
        private Form1 form;
        private Thread receiveThread;
        private const int BUFFER_SIZE = 1024;
        private string fileDirectory = "C:\\Users\\hp\\source\\repos\\ChatAppExam";
        private string username;

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

            receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.IsBackground = true;
            receiveThread.Start();

        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    UpdateChatFromReader();
                }
            }
            catch(Exception ex)
            {
                form.Invoke(new Action(() => { form.UpdateChat("Connection failed: " + ex.Message);}));
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    string message = reader.ReadLine();
                    if (message != null)
                    {
                        if (message.Contains("FILE:"))
                        {
                            ReceiveFile(message, reader);
                        }
                        if (message.StartsWith("[") && message.EndsWith("]"))
                        {
                            List<string> contacts = JsonSerializer.Deserialize<List<string>>(message);
                            form.UpdateContacts(contacts);
                        }
                        else
                        {
                            form.Invoke(new Action(() => form.UpdateChat("Server: " + message)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                form.Invoke(new Action(() => form.UpdateChat("Connection lost: " + ex.Message)));
            }
        }

        private void UpdateChatFromReader()
        {
            string message = reader.ReadLine();
            if (message != null)
            {
                form.Invoke(new Action(() => form.UpdateChat("Server: " + message)));
            }
        }

        public void SendMessage(string message)
        {
            writer.WriteLine(message);
            form.Invoke(new Action(() => form.UpdateChat("Me: " + message)));
        }

        public void SendFile(string filePath, string fileName, long fileSize)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    writer.WriteLine($"FILE: {fileName}:{fileSize}");

                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        clientSocket.Send(buffer, 0, bytesRead, SocketFlags.None);
                    }
                }
            }
            catch (Exception ex)
            {
               writer.WriteLine($"Error sending file {fileName}: {ex.Message}");
            }
        }

        public void ReceiveFile(string message, StreamReader reader)
        {
            try
            {
                string[] parts = message.Split(':');
                if (parts.Length < 3)
                {
                    throw new ArgumentException("Invalid message format for file receiving.");
                }

                string fileName = parts[1].Trim();
                int fileSize;
                if (!int.TryParse(parts[2].Trim(), out fileSize))
                {
                    throw new ArgumentException("Invalid file size format.");
                }

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

                form.Invoke(new Action(() =>
                {
                    try
                    {
                        if (form.IsDisposed || !form.IsHandleCreated) return;

                        form.UpdateChat($"File received: {fileName}");

                        DialogResult result = MessageBox.Show(form, $"File received: {fileName}\n\nDo you want to open this file?", "File Received", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = savePath,
                                UseShellExecute = true
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        form.Invoke(() => ($"Error showing MessageBox: {ex.Message}"));
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving file: {ex.Message}");
            }
        }

        public void Close()
        {
            receiveThread?.Interrupt();
            writer.Close();
            reader.Close();
            clientSocket.Close();
        }
    }
}
