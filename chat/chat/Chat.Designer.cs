namespace chat
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
            lb_Friends = new ListBox();
            rtb_Messages = new RichTextBox();
            tb_MessageInput = new TextBox();
            btn_Send = new Button();
            btn_AddFriend = new Button();
            SuspendLayout();
            // 
            // lb_Friends
            // 
            lb_Friends.FormattingEnabled = true;
            lb_Friends.ItemHeight = 15;
            lb_Friends.Location = new Point(14, 14);
            lb_Friends.Name = "lb_Friends";
            lb_Friends.Size = new Size(160, 364);
            lb_Friends.TabIndex = 0;
            // 
            // rtb_Messages
            // 
            rtb_Messages.Location = new Point(191, 14);
            rtb_Messages.Name = "rtb_Messages";
            rtb_Messages.Size = new Size(513, 368);
            rtb_Messages.TabIndex = 1;
            rtb_Messages.Text = "";
            // 
            // tb_MessageInput
            // 
            tb_MessageInput.Location = new Point(191, 401);
            tb_MessageInput.Name = "tb_MessageInput";
            tb_MessageInput.Size = new Size(440, 23);
            tb_MessageInput.TabIndex = 2;
            // 
            // btn_Send
            // 
            btn_Send.Location = new Point(637, 401);
            btn_Send.Name = "btn_Send";
            btn_Send.Size = new Size(67, 23);
            btn_Send.TabIndex = 3;
            btn_Send.Text = "Enviar";
            btn_Send.UseVisualStyleBackColor = true;
            btn_Send.Click += btn_Send_Click;
            // 
            // btn_AddFriend
            // 
            btn_AddFriend.Location = new Point(14, 384);
            btn_AddFriend.Name = "btn_AddFriend";
            btn_AddFriend.Size = new Size(75, 23);
            btn_AddFriend.TabIndex = 4;
            btn_AddFriend.Text = "Adcionar";
            btn_AddFriend.UseVisualStyleBackColor = true;
            btn_AddFriend.Click += btn_AddFriend_Click;
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(716, 450);
            Controls.Add(btn_AddFriend);
            Controls.Add(btn_Send);
            Controls.Add(tb_MessageInput);
            Controls.Add(rtb_Messages);
            Controls.Add(lb_Friends);
            Name = "Chat";
            Text = "Chat";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lb_Friends;
        private RichTextBox rtb_Messages;
        private TextBox tb_MessageInput;
        private Button btn_Send;
        private Button btn_AddFriend;
    }
}