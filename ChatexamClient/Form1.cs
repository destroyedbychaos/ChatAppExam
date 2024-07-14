namespace ChatexamClient
{
    public partial class Form1 : Form
    {
        private ChatClient client;
        private OpenFileDialog openFileDialog;
        private string selectedContact;
        public Form1()
        {
            InitComponent();
            this.Load += new EventHandler(Form1_Load);

            openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "All Files|*.*";
            openFileDialog.CheckFileExists = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.client = new ChatClient("127.0.0.1", 50000, this);
                MessageBox.Show("Client initialized successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing client: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (client == null)
                {
                    MessageBox.Show("No client initialized.");
                    return;
                }
                if (selectedContact != null)
                {
                    string message = richTextBox1.Text;
                    client.SendMessage($"ROUTE:{selectedContact}:{message}");
                }
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    string message = textBox1.Text.Trim();
                    client.SendMessage(message);
                    textBox1.Clear();
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (client == null)
                {
                    MessageBox.Show("No client initialized.");
                    return;
                }
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(filePath);
                    long fileSize = new FileInfo(filePath).Length;

                    client.SendFile(filePath, fileName, fileSize);

                    richTextBox1.AppendText($"You sent a file: {fileName}\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending file: {ex.Message}");
            }
        }

        public void UpdateChat(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() => UpdateChat(message));
            }
            else
            {
                richTextBox1.AppendText(message + Environment.NewLine);
            }

        }

        public void UpdateContacts(List<string> contacts)
        {
            try
            {
                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke(new Action(() =>
                    {
                        listBox1.Items.Clear();
                        foreach (var contact in contacts)
                        {
                            listBox1.Items.Add(contact);
                        }
                    }));
                }
                else
                {
                    listBox1.Items.Clear();
                    foreach (var contact in contacts)
                    {
                        listBox1.Items.Add(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateChat($"Exception in UpdateContacts: {ex.Message}");
            }
        }

        public void ClearRichTextBox()
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => ClearRichTextBox()));
            }
            else
            {
                richTextBox1.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem != null)
            {
                selectedContact = listBox1.SelectedItem.ToString();
                client.SelectContact(selectedContact);
            }

        }
    }
}
