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
    public partial class EditBook : Form
    {
        private Book m_Book;

        private class GenreItem
        {
            public readonly Genre genre;
            public GenreItem(Genre g) { genre = g; }
            public override string ToString() { return genre.name; }
        }

        private class AuthorItem
        {
            public readonly Author author;
            public AuthorItem(Author a) { author = a; }
            public override string ToString() { return author.name; }
        }

        public EditBook(Book book)
        {
            InitializeComponent();
            m_Book = book;

            Genre[] list = BookServiceClient.instance.Service.listGenres();
            foreach (Genre g in list)
            {
                comboBox1.Items.Add(new GenreItem(g));
                if (m_Book != null)
                {
                    if(m_Book.genre.id == g.id)
                    {
                        comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    }
                }
            }

            Author[] a_list = BookServiceClient.instance.Service.listAuthors();
            foreach (Author a in a_list)
            {
                listBox1.Items.Add(new AuthorItem(a));
                if (m_Book != null)
                {
                    foreach(Author sa in m_Book.authors)
                    {
                        if(sa.id == a.id)
                        {
                            listBox1.SelectedItems.Add(listBox1.Items[listBox1.Items.Count - 1]);
                        }
                    }
                }
            }

            if (m_Book != null)
            {
                textBox1.Text = m_Book.name;
                textBox2.Text = m_Book.count.ToString();
                textBox3.Text = m_Book.price.ToString();
                button2.Text = "Сохранить";
            }
            else
            {
                button2.Text = "Создать";
            }
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

            int count = 0;
            bool sr = int.TryParse(textBox2.Text, out count);
            if(!sr || (count <= 0))
            {
                textBox2.Focus();
                return;
            }

            float price = 0;
            sr = float.TryParse(textBox3.Text, out price);
            if (!sr || (price <= 0))
            {
                textBox3.Focus();
                return;
            }

            if(listBox1.SelectedItems.Count == 0)
            {
                listBox1.Focus();
                return;
            }

            if(comboBox1.SelectedIndex < 0)
            {
                comboBox1.Focus();
                return;
            }

            List<Author> a_list = new List<Author>();
            foreach(object a in listBox1.SelectedItems)
            {
                a_list.Add(((AuthorItem)a).author);
            }

            
            if (m_Book == null)
            {
                BookServiceClient.instance.Service.addNewBook(textBox1.Text, count, price, a_list.ToArray(), ((GenreItem)comboBox1.SelectedItem).genre);
                Close();
            }
            else
            {
                m_Book.name = textBox1.Text;
                m_Book.count = count;
                m_Book.price = price;
                m_Book.authors = a_list.ToArray();
                m_Book.genre = ((GenreItem)comboBox1.SelectedItem).genre;
                BookServiceClient.instance.Service.saveBook(m_Book);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddEntityForm dlg = new AddEntityForm();
            dlg.Owner = this;
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                Author na = BookServiceClient.instance.Service.addNewAuthor(dlg.NewValue);
                bool exist = false;
                foreach(object a in listBox1.Items)
                {
                    AuthorItem ai = (AuthorItem)a;
                    if(ai.author.id == na.id)
                    {
                        exist = true;
                        break;
                    }
                }
                if(!exist)
                {
                    listBox1.Items.Add(new AuthorItem(na));
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddEntityForm dlg = new AddEntityForm();
            dlg.Owner = this;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Genre ng = BookServiceClient.instance.Service.addNewGenre(dlg.NewValue);
                bool exist = false;
                foreach (object g in comboBox1.Items)
                {
                    GenreItem gi = (GenreItem)g;
                    if (gi.genre.id == ng.id)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    comboBox1.Items.Add(new GenreItem(ng));
                }
            }
        }
    }
}
