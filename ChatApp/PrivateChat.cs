using System;
using System.Diagnostics;  // Cung cấp các lớp để theo dõi và ghi lại thông tin runtime
using System.IO;  // Thư viện để xử lý file và stream
using System.Net;  // Thư viện cho các lớp liên quan đến mạng
using System.Net.Sockets;  // Thư viện cho việc giao tiếp qua các socket
using System.Threading.Tasks;  // Thư viện hỗ trợ lập trình bất đồng bộ
using System.Windows.Forms;  // Thư viện tạo ứng dụng Windows Forms
using System.Collections.Generic;  // Thư viện hỗ trợ các cấu trúc dữ liệu như Dictionary, List

namespace ChatApp
{
    public partial class PrivateChat : Form
    {
        private TcpClient client;  // Biến đại diện cho client kết nối
        public StreamReader STR;  // Biến đọc dữ liệu từ stream
        public StreamWriter STW;  // Biến ghi dữ liệu vào stream
        public string receive;  // Biến lưu trữ dữ liệu nhận được
        public string TextToSend;  // Biến lưu trữ dữ liệu chuẩn bị gửi đi
        public bool isConnected = false;  // Biến kiểm tra trạng thái kết nối
        private OpenFileDialog openFileDialog;  // Biến mở hộp thoại chọn tệp

        // Hàm khởi tạo cho form chat riêng
        public PrivateChat()
        {
            InitializeComponent();  // Khởi tạo các thành phần giao diện
            yPort.Text = User.PrivatePort.ToString();  // Hiển thị cổng riêng của người dùng
            openFileDialog = new OpenFileDialog();  // Khởi tạo đối tượng hộp thoại mở file
        }

        // Sự kiện khi nút "Gửi" được nhấn
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessage.Text))
            {
                string message = User.UserName + ": " + ThayTheBangIcon(txtMessage.Text);  // Thay thế ký tự bằng icon
                SendMessage(message);  // Gửi tin nhắn
            }
            txtMessage.Text = "";  // Xóa nội dung sau khi gửi
        }

        // Hàm thay thế ký tự bằng icon
        private string ThayTheBangIcon(string text)
        {
            foreach (var icon in Icons.IconMap)
            {
                text = text.Replace(icon.Key, icon.Value);  // Thay thế từng ký tự đặc biệt bằng biểu tượng
            }
            return text;
        }

        // Xử lý công việc nhận dữ liệu trong background
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    NetworkStream networkStream = client.GetStream();  // Lấy stream từ client
                    BinaryReader reader = new BinaryReader(networkStream);  // Tạo đối tượng đọc dữ liệu từ stream

                    string dataType = reader.ReadString();  // Đọc loại dữ liệu

                    if (dataType == "message")
                    {
                        string message = reader.ReadString();  // Đọc tin nhắn
                        this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                        {
                            listTextMessages.AppendText(message + "\n");  // Hiển thị tin nhắn lên giao diện
                        }));
                    }
                    else if (dataType == "file")
                    {
                        int fileLength = reader.ReadInt32();  // Đọc kích thước tệp
                        byte[] fileBytes = reader.ReadBytes(fileLength);  // Đọc nội dung tệp

                        string tempFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + Path.GetExtension("received_file");
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), tempFileName);  // Lưu tệp vào desktop

                        File.WriteAllBytes(filePath, fileBytes);  // Ghi tệp vào máy
                        string message = User.UserName + " đã nhận tệp: " + Path.GetFileName(filePath);  // Tạo thông báo nhận tệp
                        this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                        {
                            listTextMessages.AppendText(message + "\n");  // Hiển thị thông báo lên giao diện
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error receiving data: " + ex.Message);  // Thông báo lỗi nếu xảy ra
                    client.Close();  // Đóng kết nối
                }
            }
        }

        // Xử lý công việc gửi dữ liệu trong background
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SendMessage(TextToSend);  // Gửi tin nhắn
        }

        // Hàm gửi tin nhắn hoặc tệp
        private void SendMessage(string message)
        {
            if (client.Connected)
            {
                try
                {
                    NetworkStream networkStream = client.GetStream();  // Lấy stream từ client
                    BinaryWriter writer = new BinaryWriter(networkStream);  // Tạo đối tượng ghi dữ liệu vào stream

                    if (message.Contains("đã gửi tệp:"))
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "received_file");
                        if (File.Exists(filePath))
                        {
                            byte[] fileBytes = File.ReadAllBytes(filePath);  // Đọc nội dung tệp
                            writer.Write("file");
                            writer.Write(fileBytes.Length);
                            writer.Write(fileBytes);  // Gửi tệp
                        }
                    }
                    else
                    {
                        writer.Write("message");
                        writer.Write(message);  // Gửi tin nhắn
                    }

                    this.listTextMessages.Invoke(new MethodInvoker(delegate ()
                    {
                        listTextMessages.AppendText(message + "\n");  // Hiển thị tin nhắn lên giao diện
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending message: " + ex.Message);  // Thông báo lỗi nếu xảy ra
                }
            }
            else
            {
                MessageBox.Show("Send failed!");  // Thông báo nếu không thể gửi
            }
        }

        // Hàm bắt đầu server
        void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(yPort.Text));  // Tạo đối tượng lắng nghe kết nối tại cổng chỉ định
            listener.Start();  // Bắt đầu lắng nghe
            Task.Run(() =>
            {
                try
                {
                    client = listener.AcceptTcpClient();  // Chấp nhận kết nối từ client
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();  // Bắt đầu worker để nhận dữ liệu
                    backgroundWorker2.WorkerSupportsCancellation = true;  // Cho phép hủy worker gửi dữ liệu
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        listTextMessages.AppendText("Client connected\n");  // Hiển thị thông báo kết nối thành công
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());  // Thông báo lỗi nếu xảy ra
                }
            });
            Trace.WriteLine("Server started with port " + User.PrivatePort);  // Ghi log khi server bắt đầu
        }

        // Sự kiện khi nút "Kết nối" được nhấn
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected == false)
            {
                ConnectClient();  // Kết nối tới server nếu chưa kết nối
            }
            else
            {
                btnConnect.Text = "Disconnect";
                Disconnect();  // Ngắt kết nối nếu đã kết nối
            }
        }

        // Hàm kết nối client tới server
        void ConnectClient()
        {
            client = new TcpClient();  // Tạo đối tượng client
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(clPort.Text));  // Định nghĩa điểm cuối IP

            try
            {
                client.Connect(IP_End);  // Kết nối tới server
                TextToSend = User.UserName + " joined the chat";  // Tạo tin nhắn thông báo gia nhập
                SendMessage(TextToSend);  // Gửi tin nhắn
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();  // Bắt đầu worker nhận dữ liệu
                backgroundWorker2.WorkerSupportsCancellation = true;  // Cho phép hủy worker gửi dữ liệu
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());  // Thông báo lỗi nếu xảy ra
            }
            listTextMessages.AppendText("Connected to server\n");  // Hiển thị thông báo kết nối thành công
        }

        // Hàm ngắt kết nối
        void Disconnect()
        {
            TextToSend = User.UserName + " left the chat";  // Tạo tin nhắn thông báo rời khỏi chat
            SendMessage(TextToSend);  // Gửi tin nhắn
            client.Close();  // Đóng kết nối client
            STR.Close();  // Đóng stream đọc
            STW.Close();  // Đóng stream ghi
            listTextMessages.Clear();  // Xóa nội dung hiển thị
        }

        // Sự kiện khi form bị đóng
        private void PrivateChat_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();  // Ngắt kết nối
            SelectChatRoom selectChatRoom = new SelectChatRoom();  // Mở form chọn phòng chat
            selectChatRoom.Show();  // Hiển thị form chọn phòng chat
        }

        // Sự kiện khi nút "Bắt đầu" được nhấn
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartServer();  // Bắt đầu server
        }

        // Sự kiện khi nút "Gửi tệp" được nhấn
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;  // Lấy đường dẫn tệp được chọn
                SendFile(filePath);  // Gửi tệp
            }
        }

        // Hàm gửi tệp
        private void SendFile(string filePath)
        {
            if (client.Connected)
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);  // Đọc nội dung tệp
                    NetworkStream networkStream = client.GetStream();  // Lấy stream từ client
                    BinaryWriter writer = new BinaryWriter(networkStream);  // Tạo đối tượng ghi dữ liệu vào stream

                    writer.Write("file");
                    writer.Write(fileBytes.Length);
                    writer.Write(fileBytes);  // Gửi tệp

                    TextToSend = User.UserName + " đã gửi tệp: " + Path.GetFileName(filePath);  // Tạo tin nhắn thông báo gửi tệp
                    SendMessage(TextToSend);  // Gửi tin nhắn
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending file: " + ex.Message);  // Thông báo lỗi nếu xảy ra
                }
            }
            else
            {
                MessageBox.Show("Cannot send file. Connection is closed.");  // Thông báo nếu không thể gửi tệp
            }
        }

    }

    // Lớp chứa các icon thay thế cho ký tự đặc biệt
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
