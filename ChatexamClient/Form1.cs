namespace ChatexamClient
{
    public partial class Form1 : Form
    {
        private ChatClient client;
        private OpenFileDialog openFileDialog;
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
    }
}
