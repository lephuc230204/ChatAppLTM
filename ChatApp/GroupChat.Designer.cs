namespace ChatApp
{
    partial class GroupChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.listTextMessages = new System.Windows.Forms.RichTextBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 487);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(391, 63);
            this.txtMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(409, 520);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(102, 33);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // listTextMessages
            // 
            this.listTextMessages.Location = new System.Drawing.Point(14, 15);
            this.listTextMessages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listTextMessages.Name = "listTextMessages";
            this.listTextMessages.Size = new System.Drawing.Size(505, 464);
            this.listTextMessages.TabIndex = 3;
            this.listTextMessages.Text = "";
            this.listTextMessages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listTextMessages_MouseDown);
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(409, 483);
            this.btnFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(102, 35);
            this.btnFile.TabIndex = 4;
            this.btnFile.Text = "sendFile";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // GroupChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 566);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.listTextMessages);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GroupChat";
            this.Text = "GroupChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GroupChat_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RichTextBox listTextMessages;
        private System.Windows.Forms.Button btnFile;
    }
}