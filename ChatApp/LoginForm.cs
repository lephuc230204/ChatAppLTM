using System;
using System.Diagnostics;  
using System.Windows.Forms;  
using Npgsql; 

namespace ChatApp
{
    public partial class LoginForm : Form
    {
        // Chuỗi kết nối tới cơ sở dữ liệu PostgreSQL
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=lam1782004;Database=chatapp";

        // Hàm khởi tạo cho form đăng nhập
        public LoginForm()
        {
            InitializeComponent();  // Khởi tạo các thành phần giao diện người dùng
        }

        // Sự kiện khi nút "Đăng nhập" được nhấn
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các ô nhập liệu
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Kiểm tra thông tin đăng nhập
            if (ValidateLogin(username, password))
            {
                // Nếu thông tin hợp lệ, lưu lại tên người dùng và cổng riêng
                User.UserName = username;
                User.PrivatePort = GenerateRandomPort();  // Tạo một cổng ngẫu nhiên cho người dùng

                // Ghi lại thông tin đăng nhập vào log để theo dõi
                Trace.WriteLine("User with username: " + User.UserName);
                Trace.WriteLine("User with private port: " + User.PrivatePort);

                // Chuyển đến form chọn phòng chat
                SelectChatRoom selectChatRoom = new SelectChatRoom();
                this.Hide();  // Ẩn form đăng nhập hiện tại
                selectChatRoom.ShowDialog();  // Hiển thị form chọn phòng chat
            }
            else
            {
                // Nếu thông tin không hợp lệ, hiển thị thông báo lỗi
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm kiểm tra thông tin đăng nhập
        private bool ValidateLogin(string username, string password)
        {
            // Kết nối tới cơ sở dữ liệu PostgreSQL
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();  // Mở kết nối

                // Thực hiện câu lệnh SQL để đếm số lượng người dùng có username và password khớp với thông tin nhập vào
                using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username AND password = @password", connection))
                {
                    // Thêm tham số cho câu lệnh SQL
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password); // Mật khẩu nên được mã hóa trong thực tế

                    // Thực thi câu lệnh và lấy kết quả
                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    // Nếu có ít nhất 1 người dùng khớp với thông tin, trả về true (thông tin hợp lệ)
                    return userCount > 0;
                }
            }
        }

        // Hàm tạo một số cổng ngẫu nhiên
        private int GenerateRandomPort()
        {
            Random random = new Random();  // Tạo đối tượng random
            return random.Next(1000, 9999);  // Trả về một số ngẫu nhiên trong khoảng từ 1000 đến 9999
        }

        // Sự kiện khi nút "Đăng ký" được nhấn
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Mở form đăng ký người dùng mới
            RegisterForm registerForm = new RegisterForm();
            this.Hide();  // Ẩn form đăng nhập hiện tại
            registerForm.ShowDialog();  // Hiển thị form đăng ký
            this.Show();  // Hiển thị lại form đăng nhập sau khi form đăng ký đóng
        }
    }
}
