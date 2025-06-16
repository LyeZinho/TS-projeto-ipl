namespace chatapp
{
    partial class Register
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
            tb_Username = new TextBox();
            tb_Password = new TextBox();
            tb_RepeatPassword = new TextBox();
            lb_Username = new Label();
            lb_Password = new Label();
            lb_RepeatPassword = new Label();
            btn_Confirm = new Button();
            SuspendLayout();
            // 
            // tb_Username
            // 
            tb_Username.Location = new Point(69, 93);
            tb_Username.Name = "tb_Username";
            tb_Username.Size = new Size(319, 23);
            tb_Username.TabIndex = 0;
            // 
            // tb_Password
            // 
            tb_Password.Location = new Point(69, 166);
            tb_Password.Name = "tb_Password";
            tb_Password.Size = new Size(319, 23);
            tb_Password.TabIndex = 1;
            // 
            // tb_RepeatPassword
            // 
            tb_RepeatPassword.Location = new Point(69, 234);
            tb_RepeatPassword.Name = "tb_RepeatPassword";
            tb_RepeatPassword.Size = new Size(319, 23);
            tb_RepeatPassword.TabIndex = 2;
            // 
            // lb_Username
            // 
            lb_Username.AutoSize = true;
            lb_Username.Font = new Font("Segoe UI", 10F);
            lb_Username.Location = new Point(69, 71);
            lb_Username.Name = "lb_Username";
            lb_Username.Size = new Size(71, 19);
            lb_Username.TabIndex = 3;
            lb_Username.Text = "Username";
            // 
            // lb_Password
            // 
            lb_Password.AutoSize = true;
            lb_Password.Font = new Font("Segoe UI", 10F);
            lb_Password.Location = new Point(69, 144);
            lb_Password.Name = "lb_Password";
            lb_Password.Size = new Size(67, 19);
            lb_Password.TabIndex = 4;
            lb_Password.Text = "Password";
            // 
            // lb_RepeatPassword
            // 
            lb_RepeatPassword.AutoSize = true;
            lb_RepeatPassword.Font = new Font("Segoe UI", 10F);
            lb_RepeatPassword.Location = new Point(69, 212);
            lb_RepeatPassword.Name = "lb_RepeatPassword";
            lb_RepeatPassword.Size = new Size(145, 19);
            lb_RepeatPassword.TabIndex = 5;
            lb_RepeatPassword.Text = "Repita a sua password";
            // 
            // btn_Confirm
            // 
            btn_Confirm.Location = new Point(259, 288);
            btn_Confirm.Name = "btn_Confirm";
            btn_Confirm.Size = new Size(129, 29);
            btn_Confirm.TabIndex = 6;
            btn_Confirm.Text = "Confirmar Registro";
            btn_Confirm.UseVisualStyleBackColor = true;
            btn_Confirm.Click += btn_Confirm_Click;
            // 
            // Register
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(449, 387);
            Controls.Add(btn_Confirm);
            Controls.Add(lb_RepeatPassword);
            Controls.Add(lb_Password);
            Controls.Add(lb_Username);
            Controls.Add(tb_RepeatPassword);
            Controls.Add(tb_Password);
            Controls.Add(tb_Username);
            Name = "Register";
            Text = "Register";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_Username;
        private TextBox tb_Password;
        private TextBox tb_RepeatPassword;
        private Label lb_Username;
        private Label lb_Password;
        private Label lb_RepeatPassword;
        private Button btn_Confirm;
    }
}