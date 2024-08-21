using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class SelectChatRoom : Form
    {
        // Hàm khởi tạo của form chọn phòng chat
        public SelectChatRoom()
        {
            InitializeComponent();  // Khởi tạo các thành phần giao diện
        }

        // Sự kiện khi nút "Private Chat" được nhấn
        private void btnPrChat_Click(object sender, EventArgs e)
        {
            // Tạo một phiên trò chuyện riêng tư (Private Chat)
            PrivateChat privateChat = new PrivateChat();
            this.Hide();  // Ẩn form chọn phòng chat
            privateChat.ShowDialog();  // Hiển thị form trò chuyện riêng tư dưới dạng hộp thoại
        }

        // Sự kiện khi nút "Group Chat" được nhấn
        private void btnGrChat_Click(object sender, EventArgs e)
        {
            // Tạo một phiên trò chuyện nhóm (Group Chat)
            GroupChat groupChat = new GroupChat();
            this.Hide();  // Ẩn form chọn phòng chat
            groupChat.ShowDialog();  // Hiển thị form trò chuyện nhóm dưới dạng hộp thoại
        }
    }
}
