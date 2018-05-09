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
    public partial class BuysForm : Form
    {
        public BuysForm(User u = null)
        {
            InitializeComponent();
            Buy[] bs = null;
            if (user == null) { bs = BookServiceClient.instance.Service.listBaysAll(); }
            else { bs = BookServiceClient.instance.Service.listBays(u); }
            dataGridView1.Rows.Clear();
            bool first = true;
            foreach(Buy b in bs)
            {
                if(first) { first = false; } else { dataGridView1.Rows.Add(); }
                int ni = dataGridView1.Rows.Add();
                DataGridViewRow r = dataGridView1.Rows[ni];
                r.Cells["date"].Value = b.date;
                r.Cells["price"].Value = b.price;
                r.Cells["user"].Value = b.user.name;
                foreach(Buy.BuyItem bi in b.books)
                {
                    ni = dataGridView1.Rows.Add();
                    r = dataGridView1.Rows[ni];
                    r.Cells["name"].Value = bi.book.name;
                    r.Cells["count"].Value = bi.count;
                    r.Cells["genre"].Value = bi.book.genre.name;
                    string authors = "";
                    foreach (Author a in bi.book.authors) { authors += (authors.Length == 0 ? "" : "; ") + a.name; }
                    r.Cells["authors"].Value = authors;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
