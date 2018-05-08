using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceContract.Entity;

namespace BooksClient
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();
            updateBooksList();
        }

        private void updateBooksList()
        {
            Book[] books = BookServiceClient.instance.Service.listBooks();
            dataGridView1.Rows.Clear();
            foreach (Book b in books)
            {
                int ni = dataGridView1.Rows.Add();
                DataGridViewRow r = dataGridView1.Rows[ni];
                r.Tag = b;
                r.Cells["name"].Value = b.name;
                r.Cells["count"].Value = b.count;
                r.Cells["price"].Value = b.price;
                r.Cells["genre"].Value = b.genre.name;
                string authors = "";
                foreach (Author a in b.authors) { authors += (authors.Length == 0 ? "" : "; ") + a.name; }
                r.Cells["authors"].Value = authors;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EditBook dlg = new EditBook(null);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            updateBooksList();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Book b = (Book)dataGridView1.SelectedRows[0].Tag;
                EditBook dlg = new EditBook(b);
                dlg.Owner = this;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    updateBooksList();
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Book b = (Book)dataGridView1.SelectedRows[0].Tag;
                BookServiceClient.instance.Service.deleteBook(b);
                updateBooksList();
            }
        }
    }
}
