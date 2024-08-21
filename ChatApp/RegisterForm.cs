using System;
using System.Windows.Forms;
using Npgsql;

namespace ChatApp
{
    public partial class RegisterForm : Form
    {
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=lam1782004;Database=chatapp";

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (RegisterUser(username, password))
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Close the form after successful registration
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the RegisterForm

            // Show LoginForm
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private bool RegisterUser(string username, string password)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", connection))
                    {
                        command.Parameters.AddWithValue("username", username);
                        command.Parameters.AddWithValue("password", password); // Password should be hashed in a real application

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
