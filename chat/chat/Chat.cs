using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class Chat : Form
    {
        string username;
        public Chat(string username)
        {
            this.username = username;
            InitializeComponent();
        }


        private void btn_Send_Click(object sender, EventArgs e)
        {

        }
    }


}
