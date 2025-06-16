namespace chatapp
{
    partial class Chat
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
            btn_SendMessage = new Button();
            textBox1 = new TextBox();
            rtb_MessageBoxList = new RichTextBox();
            lb_FriendList = new ListBox();
            btn_FriendAdd = new Button();
            SuspendLayout();
            // 
            // btn_SendMessage
            // 
            btn_SendMessage.Location = new Point(645, 406);
            btn_SendMessage.Name = "btn_SendMessage";
            btn_SendMessage.Size = new Size(75, 23);
            btn_SendMessage.TabIndex = 0;
            btn_SendMessage.Text = "Send";
            btn_SendMessage.UseVisualStyleBackColor = true;
            btn_SendMessage.Click += btn_SendMessage_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(217, 406);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(422, 23);
            textBox1.TabIndex = 1;
            // 
            // rtb_MessageBoxList
            // 
            rtb_MessageBoxList.Location = new Point(217, 18);
            rtb_MessageBoxList.Name = "rtb_MessageBoxList";
            rtb_MessageBoxList.Size = new Size(503, 382);
            rtb_MessageBoxList.TabIndex = 2;
            rtb_MessageBoxList.Text = "";
            // 
            // lb_FriendList
            // 
            lb_FriendList.FormattingEnabled = true;
            lb_FriendList.ItemHeight = 15;
            lb_FriendList.Location = new Point(14, 19);
            lb_FriendList.Name = "lb_FriendList";
            lb_FriendList.Size = new Size(172, 379);
            lb_FriendList.TabIndex = 3;
            lb_FriendList.SelectedIndexChanged += lb_FriendList_SelectedIndexChanged;
            // 
            // btn_FriendAdd
            // 
            btn_FriendAdd.Location = new Point(43, 405);
            btn_FriendAdd.Name = "btn_FriendAdd";
            btn_FriendAdd.Size = new Size(114, 23);
            btn_FriendAdd.TabIndex = 4;
            btn_FriendAdd.Text = "Adcionar Amigo";
            btn_FriendAdd.UseVisualStyleBackColor = true;
            btn_FriendAdd.Click += btn_FriendAdd_Click;
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(741, 450);
            Controls.Add(btn_FriendAdd);
            Controls.Add(lb_FriendList);
            Controls.Add(rtb_MessageBoxList);
            Controls.Add(textBox1);
            Controls.Add(btn_SendMessage);
            Name = "Chat";
            Text = "Chat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_SendMessage;
        private TextBox textBox1;
        private RichTextBox rtb_MessageBoxList;
        private ListBox lb_FriendList;
        private Button btn_FriendAdd;
    }
}