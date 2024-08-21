using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChatApp
{
    public partial class PrivateChat : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string receive;
        public string TextToSend;
        public bool isConnected = false;
        private OpenFileDialog openFileDialog;

        public PrivateChat()
        {
            InitializeComponent();
            yPort.Text = User.PrivatePort.ToString();
            openFileDialog = new OpenFileDialog();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessage.Text))
            {
                string message = User.UserName + ": " + ThayTheBangIcon(txtMessage.Text);
                SendMessage(message);
            }
            txtMessage.Text = "";
        }

        private string ThayTheBangIcon(string text)
        {
            foreach (var icon in Icons.IconMap)
            {
                text = text.Replace(icon.Key, icon.Value);
            }
            return text;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    NetworkStream networkStream = client.GetStream();
                    BinaryReader reader = new BinaryReader(networkStream);

                    string dataType = reader.ReadString();

                    if (dataType == "message")
                    {
                        string message = reader.ReadString();
                        this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                        {
                            listTextMessages.AppendText(message + "\n");
                        }));
                    }
                    else if (dataType == "file")
                    {
                        int fileLength = reader.ReadInt32();
                        byte[] fileBytes = reader.ReadBytes(fileLength);

                        // Tạo tên tệp tạm thời với phần mở rộng tệp
                        string tempFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + Path.GetExtension("received_file");
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), tempFileName);

                        File.WriteAllBytes(filePath, fileBytes);
                        string message = User.UserName + " đã gửi tệp: " + Path.GetFileName(filePath);
                        this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                        {
                            listTextMessages.AppendText(message + "\n");
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error receiving data: " + ex.Message);
                    client.Close();
                }
            }
        }


        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SendMessage(TextToSend);
        }

        private void SendMessage(string message)
        {
            if (client.Connected)
            {
                try
                {
                    NetworkStream networkStream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(networkStream);

                    if (message.Contains("đã gửi tệp:"))
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "received_file");
                        if (File.Exists(filePath))
                        {
                            byte[] fileBytes = File.ReadAllBytes(filePath);
                            writer.Write("file");
                            writer.Write(fileBytes.Length);
                            writer.Write(fileBytes);
                        }
                    }
                    else
                    {
                        writer.Write("message");
                        writer.Write(message);
                    }

                    this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                    {
                        listTextMessages.AppendText(message + "\n");
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending message: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Send failed!");
            }
        }

        void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(yPort.Text));
            listener.Start();
            Task.Run(() =>
            {
                try
                {
                    client = listener.AcceptTcpClient();
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        listTextMessages.AppendText("Client connected\n");
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            });
            Trace.WriteLine("Server started with port " + User.PrivatePort);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected == false)
            {
                ConnectClient();
            }
            else
            {
                btnConnect.Text = "Disconnect";
                Disconnect();
            }
        }

        void ConnectClient()
        {
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(clPort.Text));

            try
            {
                client.Connect(IP_End);
                TextToSend = User.UserName + " joined the chat";
                SendMessage(TextToSend);
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            listTextMessages.AppendText("Connected to server\n");
        }

        void Disconnect()
        {
            TextToSend = User.UserName + " left the chat";
            SendMessage(TextToSend);
            client.Close();
            STR.Close();
            STW.Close();
            listTextMessages.Clear();
        }

        private void PrivateChat_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
            SelectChatRoom selectChatRoom = new SelectChatRoom();
            selectChatRoom.Show();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartServer();
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                SendFile(filePath);
            }
        }

        private void SendFile(string filePath)
        {
            if (client.Connected)
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    NetworkStream networkStream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(networkStream);

                    writer.Write("file");
                    writer.Write(fileBytes.Length);
                    writer.Write(fileBytes);

                    TextToSend = User.UserName + " đã gửi tệp: " + Path.GetFileName(filePath);
                    SendMessage(TextToSend);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending file: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Cannot send file. Connection is closed.");
            }
        }

    }

    public static class Icons
    {
        public static Dictionary<string, string> IconMap = new Dictionary<string, string>
        {
            { ":)", "😊" },
            { ":(", "😞" },
            { ":D", "😄" },
            { ";)", "😉" }
        };
    }
}
