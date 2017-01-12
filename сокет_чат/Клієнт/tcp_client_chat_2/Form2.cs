using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace tcp_client_chat_2
{
    public partial class Form2 : Form
    {
        static private Socket Client;
        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private int port = 8005;
        private Thread th;
        public string name;
        public Form2()
        {
            InitializeComponent();
            try
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.Connect(ip, port);

                th = new Thread(delegate()
                {
                    recvMessage();
                });
                th.Start();
            }
            catch (Exception ex)
            {
                DialogResult dialog = MessageBox.Show("Неможливо підключитися до сервера, програма буде закрита", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            
        }
        void sendMessage(string message)
        {
            if (message != " " && message != "")
            {
                byte[] buffer = new byte[1024];
                //buffer = Encoding.UTF8.GetBytes(message);
                buffer = Encoding.Unicode.GetBytes(message);//масивом байт
                Client.Send(buffer);
            }
        }
        void recvMessage()
        {
            sendMessage("my names is :" + name);
            byte[] buffer1 = new byte[1028];
            for (int i = 0; i < buffer1.Length; i++)
            {
                buffer1[i] = 0;
            }
            for (;;)
            {
                try
                {
                    Client.Receive(buffer1);// получаємо повідомлення
                    string mess = Encoding.Unicode.GetString(buffer1);// переробляємо з масиву байт в строку
                    for (int i = 0; i < buffer1.Length; i++)//чистимо буфер
                    {
                        buffer1[i] = 0;
                    }

                    this.Invoke((MethodInvoker)delegate()   // функція для отримання доступу елементів інтерфейсу
                    {
                        this.textBox1.AppendText(mess); //вивід на екран повідомлення
                        this.textBox1.AppendText("\r\n");
                    }
                    );
                }

                catch (Exception ex)
                {

                    DialogResult dialog = MessageBox.Show("Втрата зв'язку з сервером, програма буде закрита", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
             
            sendMessage(textBox2.Text);
            //textBox1.AppendText(name + ": " + textBox2.Text + "\r\n");
            textBox2.Text = "";
            
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);  
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendMessage(textBox2.Text);
                //textBox1.AppendText(name + ": " + textBox2.Text + "\r\n");
                textBox2.Text = "";
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendMessage(textBox2.Text);
                //textBox1.AppendText(name + ": " + textBox2.Text + "\r\n");
                textBox2.Text = "";
            }
        }
    }
}
