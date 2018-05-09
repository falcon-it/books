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
    public partial class BuyerForm : Form
    {
        public BuyerForm()
        {
            InitializeComponent();
            updateBooksList();
            BookServiceClient.instance.updateDelegate = () => { this.Invoke((Action)(delegate () { updateBooksList(); })); };
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
            if (!BookServiceClient.instance.isLogin())
            {
                if (toolStripTextBox1.Text.Length == 0)
                {
                    toolStripTextBox1.Focus();
                    return;
                }

                BookServiceClient.instance.login(toolStripTextBox1.Text);

                toolStripTextBox1.ReadOnly = true;
                toolStripButton1.Text = "Выйти";
                toolStripButton2.Visible = true;
                toolStripButton3.Visible = true;
                toolStripButton5.Visible = true;
            }
            else
            {
                BookServiceClient.instance.logout();

                toolStripTextBox1.Text = "";
                toolStripTextBox1.ReadOnly = false;
                toolStripButton1.Text = "Войти";
                toolStripButton2.Visible = false;
                toolStripButton3.Visible = false;
                toolStripButton5.Visible = false;
            }
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
                GetCountForm dlg = new GetCountForm(b.count);
                dlg.Owner = this;
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    BasketShortInfo bsi = BookServiceClient.instance.Service.addBookToBasket(BookServiceClient.instance.user, b, dlg.bookCount);
                    toolStripButton3.Text = "Корзина (" + bsi.ToString() + ")...";
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            BasketForm dlg = new BasketForm();
            dlg.Owner = this;
            dlg.ShowDialog();
            BasketShortInfo bsi = BookServiceClient.instance.Service.getUserBasketShort(BookServiceClient.instance.user);
            toolStripButton3.Text = "Корзина (" + bsi.ToString() + ")...";
            updateBooksList();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            BuysForm dlg = new BuysForm(BookServiceClient.instance.user);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void BuyerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BookServiceClient.instance.updateDelegate = null;
        }
    }
}
