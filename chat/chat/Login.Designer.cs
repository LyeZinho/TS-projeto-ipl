namespace chat
{
    partial class Login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_UsernameImput = new TextBox();
            lb_UsernameImput = new Label();
            btn_ConfirmUsername = new Button();
            SuspendLayout();
            // 
            // tb_UsernameImput
            // 
            tb_UsernameImput.Location = new Point(47, 69);
            tb_UsernameImput.Name = "tb_UsernameImput";
            tb_UsernameImput.Size = new Size(268, 23);
            tb_UsernameImput.TabIndex = 0;
            // 
            // lb_UsernameImput
            // 
            lb_UsernameImput.AutoSize = true;
            lb_UsernameImput.Location = new Point(47, 51);
            lb_UsernameImput.Name = "lb_UsernameImput";
            lb_UsernameImput.Size = new Size(98, 15);
            lb_UsernameImput.TabIndex = 1;
            lb_UsernameImput.Text = "Nome de usuario";
            // 
            // btn_ConfirmUsername
            // 
            btn_ConfirmUsername.Location = new Point(321, 68);
            btn_ConfirmUsername.Name = "btn_ConfirmUsername";
            btn_ConfirmUsername.Size = new Size(75, 23);
            btn_ConfirmUsername.TabIndex = 2;
            btn_ConfirmUsername.Text = "Confirmar";
            btn_ConfirmUsername.UseVisualStyleBackColor = true;
            btn_ConfirmUsername.Click += btn_ConfirmUsername_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(441, 153);
            Controls.Add(btn_ConfirmUsername);
            Controls.Add(lb_UsernameImput);
            Controls.Add(tb_UsernameImput);
            Name = "Login";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_UsernameImput;
        private Label lb_UsernameImput;
        private Button btn_ConfirmUsername;
    }
}
