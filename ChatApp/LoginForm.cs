using System;
using System.Diagnostics;
using System.Windows.Forms;
using Npgsql;

namespace ChatApp
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=thanhmaso1;Database=chatapp";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (ValidateLogin(username, password))
            {
                User.UserName = username;
                User.PrivatePort = GenerateRandomPort();

                Trace.WriteLine("User with username: " + User.UserName);
                Trace.WriteLine("User with private port: " + User.PrivatePort);

                SelectChatRoom selectChatRoom = new SelectChatRoom();
                this.Hide();
                selectChatRoom.ShowDialog();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username AND password = @password", connection))
                {
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password); // Mật khẩu nên được mã hóa trong thực tế

                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    return userCount > 0;
                }
            }
        }

        private int GenerateRandomPort()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            this.Hide();
            registerForm.ShowDialog();
            this.Show();
        }
    }
}
