namespace ChatexamClient
{
    public partial class Form1 : Form
    {
        private ChatClient client;
        public Form1()
        {
            InitComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ChatClient("127.0.0.1", 50000, this);
            client.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                string message = textBox1.Text.Trim();
                client.SendMessage(message);
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        public void UpdateChat(string message)
        {
            //не могла розібратись чому не працює, це взяла з інтернету; все ще не працює але...
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() => UpdateChat(message));
                return;
            }

            richTextBox1.AppendText(message + Environment.NewLine);
        }
    }
}
