using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BooksClient
{
    public partial class AddEntityForm : Form
    {
        string m_NewValue = string.Empty;

        public string NewValue
        {
            get { return m_NewValue; }
        }
        public AddEntityForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length == 0)
            {
                textBox1.Focus();
                return;
            }

            m_NewValue = textBox1.Text;

            DialogResult = DialogResult.OK;
        }
    }
}
