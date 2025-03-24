namespace WebsocketsClient
{
    public partial class Form1 : Form
    {
        private readonly SignalRClient _client;
        private readonly Dictionary<string, List<string>> _chats;
        private string? _selectedChat;
        public Form1()
        {
            InitializeComponent();
            _client = new SignalRClient();
            _chats = new Dictionary<string, List<string>>();
            label2.Text = _client.Id;
            _client.OnReceiveMessage += (message, sourceId) =>
            {
                if (!_chats.ContainsKey(sourceId))
                {
                    _chats[sourceId] = new List<string>();
                    listBox1.Items.Add(sourceId);
                }
                string messageView = $"->: {message}\n";
                _chats[sourceId].Add(messageView);
                if (sourceId == _selectedChat)
                {
                    label3.Text = $"{label3.Text}{messageView}";
                }
            };
        }

        private async void label2_Click(object sender, EventArgs e)
        {
            if (_client.Id != null)
                Clipboard.SetText(_client.Id);
            label2.Text = "Скопировано";
            await Task.Delay(2000);
            label2.Text = _client.Id;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedChat = (string)listBox1.SelectedItem!;
            label3.Text = string.Join("", _chats[_selectedChat]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_chats.ContainsKey(textBox1.Text))
            {
                MessageBox.Show("Чат уже открыт");
                return;
            }
            _chats[textBox1.Text] = new List<string>();
            listBox1.Items.Add(textBox1.Text);
            textBox1.Text = "";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "" || string.IsNullOrWhiteSpace(richTextBox1.Text) || _selectedChat == null)
            {
                return;
            }
            await _client.SendMessageAsync(_selectedChat, richTextBox1.Text);
            string messageView = $"<-: {richTextBox1.Text}\n";
            _chats[_selectedChat].Add(messageView);
            label3.Text = $"{label3.Text}{messageView}";
            richTextBox1.Text = "";
        }
    }
}
