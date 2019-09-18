using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        Form2 child;
        public Form1()
        {
            InitializeComponent();
        }

        private void server_Click(object sender, EventArgs e)
        {
            child=new Form2(ip.Text, port.Text);
            child.ShowDialog();
        }
    }
}
