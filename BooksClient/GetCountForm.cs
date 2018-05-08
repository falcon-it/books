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
    public partial class GetCountForm : Form
    {
        private int max;
        public int bookCount;

        public GetCountForm(int max)
        {
            InitializeComponent();
            this.max = max;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = 0;
            if(!int.TryParse(textBox1.Text, out count))
            {
                textBox1.Focus();
                return;
            }

            if((count <= 0) || (count > max))
            {
                textBox1.Focus();
                return;
            }

            bookCount = count;
            DialogResult = DialogResult.OK;
        }
    }
}
