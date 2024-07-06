namespace ChatexamClient
{
    public partial class Form1 : Form
    {
        private ChatClient client;
        public Form1()
        {
            InitComponent();
            this.Load += new EventHandler(Form1_Load);
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
                if(client == null)
                {
                    MessageBox.Show("No client initialized.");
                }
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    string message = textBox1.Text.Trim();
                    client.SendMessage(message);
                    textBox1.Clear();
                    textBox1.Focus();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
