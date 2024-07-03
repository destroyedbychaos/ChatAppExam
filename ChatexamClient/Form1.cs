namespace ChatexamClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                richTextBox1.AppendText("Me: " + textBox1.Text + "\n");
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        private void InitializeComponent()
        {

        }
    }
}
