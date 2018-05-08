using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BooksClient
{
    public partial class SelectModeForm : Form
    {
        public enum mode
        {
            none,
            editor,
            buyer
        }

        private mode m_Mode = mode.none;

        public mode AppMode
        {
            get { return m_Mode; }
        }

        public SelectModeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Mode = mode.editor;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_Mode = mode.buyer;
            Close();
        }
    }
}
