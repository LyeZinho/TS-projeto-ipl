namespace chatapp
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
            tb_Connect = new TextBox();
            btn_Login = new Button();
            textBox1 = new TextBox();
            btn_Register = new Button();
            label1 = new Label();
            lb_Password = new Label();
            SuspendLayout();
            // 
            // tb_Connect
            // 
            tb_Connect.Location = new Point(44, 128);
            tb_Connect.Name = "tb_Connect";
            tb_Connect.Size = new Size(255, 23);
            tb_Connect.TabIndex = 0;
            // 
            // btn_Login
            // 
            btn_Login.Location = new Point(201, 174);
            btn_Login.Name = "btn_Login";
            btn_Login.Size = new Size(98, 33);
            btn_Login.TabIndex = 1;
            btn_Login.Text = "Entrar";
            btn_Login.UseVisualStyleBackColor = true;
            btn_Login.Click += btn_Login_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(44, 60);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(255, 23);
            textBox1.TabIndex = 2;
            // 
            // btn_Register
            // 
            btn_Register.Location = new Point(44, 174);
            btn_Register.Name = "btn_Register";
            btn_Register.Size = new Size(97, 33);
            btn_Register.TabIndex = 3;
            btn_Register.Text = "Registrar";
            btn_Register.UseVisualStyleBackColor = true;
            btn_Register.Click += btn_Register_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(44, 38);
            label1.Name = "label1";
            label1.Size = new Size(71, 19);
            label1.TabIndex = 4;
            label1.Text = "Username";
            // 
            // lb_Password
            // 
            lb_Password.AutoSize = true;
            lb_Password.Font = new Font("Segoe UI", 10F);
            lb_Password.Location = new Point(44, 106);
            lb_Password.Name = "lb_Password";
            lb_Password.Size = new Size(67, 19);
            lb_Password.TabIndex = 5;
            lb_Password.Text = "Password";
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(399, 237);
            Controls.Add(lb_Password);
            Controls.Add(label1);
            Controls.Add(btn_Register);
            Controls.Add(textBox1);
            Controls.Add(btn_Login);
            Controls.Add(tb_Connect);
            Name = "Login";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_Connect;
        private Button btn_Login;
        private TextBox textBox1;
        private Button btn_Register;
        private Label label1;
        private Label lb_Password;
    }
}
