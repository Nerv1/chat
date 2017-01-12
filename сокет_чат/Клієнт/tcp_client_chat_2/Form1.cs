using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tcp_client_chat_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                this.Hide();
                var form2 = new Form2();
                form2.name = textBox1.Text;
                form2.label1.Text = "Ви увійшли як: " + textBox1.Text;
                form2.Show();
            }
            else
            {
                DialogResult dialog = MessageBox.Show("Введіть свій нік", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    this.Hide();
                    var form2 = new Form2();
                    form2.name = textBox1.Text;
                    form2.label1.Text = "Ви увійшли як: " + textBox1.Text;
                    form2.Show();
                }
                else
                {
                    DialogResult dialog = MessageBox.Show("Введіть свій нік", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }          
    }
}
