using System;
using System.Windows.Forms;  // Thư viện để tạo giao diện người dùng Windows Forms
using Npgsql;  // Thư viện để kết nối và làm việc với PostgreSQL

namespace ChatApp
{
    public partial class RegisterForm : Form
    {
        // Chuỗi kết nối tới cơ sở dữ liệu PostgreSQL
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=lam1782004;Database=chatapp";

        // Hàm khởi tạo của form đăng ký
        public RegisterForm()
        {
            InitializeComponent();  // Khởi tạo các thành phần giao diện
        }

        // Sự kiện khi nút "Đăng ký" được nhấn
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các trường nhập liệu
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Kiểm tra nếu có trường nào bị bỏ trống
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Hiển thị thông báo lỗi
                return;
            }

            // Kiểm tra nếu mật khẩu và mật khẩu xác nhận không khớp
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Hiển thị thông báo lỗi
                return;
            }

            // Gọi hàm RegisterUser để đăng ký người dùng mới
            if (RegisterUser(username, password))
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);  // Hiển thị thông báo thành công
                this.Close();  // Đóng form sau khi đăng ký thành công
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Hiển thị thông báo lỗi
            }
        }

        // Sự kiện khi nút "Đăng nhập" được nhấn
        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Close();  // Đóng form đăng ký

            // Mở form đăng nhập
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        // Hàm đăng ký người dùng mới vào cơ sở dữ liệu
        private bool RegisterUser(string username, string password)
        {
            try
            {
                // Mở kết nối tới cơ sở dữ liệu PostgreSQL
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();  // Mở kết nối

                    // Tạo lệnh SQL để thêm người dùng mới vào bảng users
                    using (var command = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", connection))
                    {
                        // Gán giá trị cho các tham số trong câu lệnh SQL
                        command.Parameters.AddWithValue("username", username);
                        command.Parameters.AddWithValue("password", password); // Trong ứng dụng thực tế, mật khẩu cần được băm

                        // Thực thi lệnh SQL và trả về số dòng bị ảnh hưởng
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;  // Nếu có dòng bị ảnh hưởng, trả về true
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có ngoại lệ xảy ra
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;  // Trả về false nếu có lỗi xảy ra
            }
        }
    }
}
