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


namespace Homework_20_01_II
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private DateTime connectionTime;
        public Form1()
        {
            InitializeComponent();
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        public async Task Connect()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            client = new TcpClient();

            try
            {
                await client.ConnectAsync(ip, port);
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Вы подключились к серверу.");
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Авторизируйтесь для продолжения.");
                Task.Run(() => Read_Message());

                button5.Enabled = false;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                textBox1.Enabled = true;
                button1.Enabled = true;

            }
            catch(Exception ex)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {ex.Message}");
            }
        }

        public async Task Send_Message()
        {
            try
            {
                string message = $"{textBox2.Text}: {textBox1.Text}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);

                data = new byte[256];
                int bytesRead = await client.GetStream().ReadAsync(data, 0, data.Length);
                string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {ex.Message}");
            }
        }

        public async Task Read_Message()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[256];
                    int bytesRead = await client.GetStream().ReadAsync(data, 0, data.Length);
                    string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                    listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {ex.Message}");
            }
        }

        public async Task Sign_Up()
        {
            try
            {
                string login = textBox2.Text;
                string password = textBox3.Text;
                string command = "регистрация";

                string credentials = $"{login} , {password} , {command}";

                byte[] data = Encoding.UTF8.GetBytes(credentials);
                await client.GetStream().WriteAsync(data, 0, data.Length);

                byte[] data1 = new byte[256];
                int bytesRead = await client.GetStream().ReadAsync(data1, 0, data1.Length);
                string response = Encoding.UTF8.GetString(data1, 0, bytesRead);
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {ex.Message}");
            }
        }

        public async Task Sign_In()
        {
            try
            {
                string login = textBox2.Text;
                string password = textBox3.Text;
                string command = "вход";

                string credentials = $"{login} , {password} , {command}";

                byte[] data = Encoding.UTF8.GetBytes(credentials);
                await client.GetStream().WriteAsync(data, 0, data.Length);

                byte[] data1 = new byte[256];
                int bytesRead = await client.GetStream().ReadAsync(data1, 0, data1.Length);
                string response = Encoding.UTF8.GetString(data1, 0, bytesRead);
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Send_Message();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;

            if (client != null && client.Connected)
            {
                client.Close();
            }
            listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Вы отключились от сервера.");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Sign_Up();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Sign_In();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Connect();
        }
    }
}
