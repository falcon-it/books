using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceContract.Entity;

namespace BooksClient
{
    public partial class BasketForm : Form
    {
        private void updateBooksList()
        {
            BasketInfo bi = BookServiceClient.instance.Service.getUserBasket(BookServiceClient.instance.user);
            dataGridView1.Rows.Clear();
            foreach (BasketItem b in bi.items)
            {
                int ni = dataGridView1.Rows.Add();
                DataGridViewRow r = dataGridView1.Rows[ni];
                r.Tag = b;
                r.Cells["name"].Value = b.book.name;
                r.Cells["count"].Value = b.count;
                r.Cells["price"].Value = b.book.price;
                r.Cells["price_all"].Value = b.book.price * b.count;
                r.Cells["genre"].Value = b.book.genre.name;
                string authors = "";
                foreach (Author a in b.book.authors) { authors += (authors.Length == 0 ? "" : "; ") + a.name; }
                r.Cells["authors"].Value = authors;
            }
            label1.Text = bi.calcInfo.ToString();
        }
        public BasketForm()
        {
            InitializeComponent();
            updateBooksList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                BasketItem b = (BasketItem)dataGridView1.SelectedRows[0].Tag;
                BookServiceClient.instance.Service.deleteBookFromBasket(BookServiceClient.instance.user, b);
                updateBooksList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BookServiceClient.instance.Service.buyBasket(BookServiceClient.instance.user);
            updateBooksList();
        }
    }
}
