using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        Form2 child;
        int nChild = 0;

        public Form1()
        {

            InitializeComponent();
        }

        private void server_Click(object sender, EventArgs e)
        {
            child = new Form2(ip.Text,port.Text,id.Text);
            nChild++;
            child.ShowDialog();
        }
    }
}
