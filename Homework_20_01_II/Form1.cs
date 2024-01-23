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
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;

            Connect();
        }

        public async Task Connect()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            if (client != null && client.Connected)
            {
                client.Close();
            }

            client = new TcpClient();

            try
            {
                await client.ConnectAsync(ip, port);
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Соединение с сервером установлено.");
                listBox1.Items.Add("[Подсказка]: Для продолжения вам необходимо авторизироваться.");
                listBox1.Items.Add("[Подсказка]: Нажмите 'Войти' или 'Зарегистрироваться', не забудьте ввести логин и пароль.");

                textBox2.Enabled = true;
                textBox3.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;

                await Task.Run(() => Read_Message());

            }
            catch(Exception ex)
            {
                textBox1.Enabled = false;
                button1.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;

                listBox1.Items.Add($"[Ошибка]: {ex.Message}");
            }
        }

        public async Task Send_Message()
        {
            try
            {
                string message = $"{textBox2.Text}: {textBox1.Text}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);
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

                    if (listBox1.InvokeRequired)
                    {
                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
                        }));
                    }
                    else
                    {
                        listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: {response}");
                    }


                    if (response == "Логин занят другим пользователем.")
                    {

                        textBox2.Invoke(new Action(() =>
                        {
                            textBox2.Enabled = false; 
                        }));

                        textBox3.Invoke(new Action(() =>
                        {
                            textBox3.Enabled = false; 
                        }));

                        button3.Invoke(new Action(() =>
                        {
                            button3.Enabled = false;
                        }));

                        button4.Invoke(new Action(() =>
                        {
                            button4.Enabled = false;
                        }));

                        if (client != null && client.Client != null && client.Connected)
                        {
                            client.Close();
                        }

                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Соединение с сервером разорвано.");
                            listBox1.Items.Add("[Подсказка]: Для повторного соединения с сервером перезапустите клиент.");
                        }));

                    }
                    else if (response == "Неверный логин или пароль.")
                    {
                        textBox2.Invoke(new Action(() =>
                        {
                            textBox2.Enabled = false;
                        }));

                        textBox3.Invoke(new Action(() =>
                        {
                            textBox3.Enabled = false;
                        }));

                        button3.Invoke(new Action(() =>
                        {
                            button3.Enabled = false;
                        }));

                        button4.Invoke(new Action(() =>
                        {
                            button4.Enabled = false;
                        }));

                        if (client != null && client.Client != null && client.Connected)
                        {
                            client.Close();
                        }

                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Соединение с сервером разорвано.");
                            listBox1.Items.Add("[Подсказка]: Для повторного соединения с сервером перезапустите клиент.");
                        }));
                    }
                    else if (response == "Добро пожаловать.")
                    {
                        textBox1.Invoke(new Action(() =>
                        {
                            textBox1.Enabled = true;
                        }));

                        button1.Invoke(new Action(() =>
                        {
                            button1.Enabled = true;
                        }));

                        textBox2.Invoke(new Action(() =>
                        {
                            textBox2.Enabled = false;
                        }));

                        textBox3.Invoke(new Action(() =>
                        {
                            textBox3.Enabled = false;
                        }));

                        button3.Invoke(new Action(() =>
                        {
                            button3.Enabled = false;
                        }));

                        button4.Invoke(new Action(() =>
                        {
                            button4.Enabled = false;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke(new Action(() =>
                    {
                        listBox1.Items.Add($"[Ошибка]: {ex.Message}");
                    }));
                }
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
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Выполняется авторизация");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[Ошибка]: {ex.Message}");
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
                listBox1.Items.Add($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}]: Выполняется авторизация");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"[Ошибка]: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Send_Message();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Sign_Up();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Sign_In();
        }
    }
}
