using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;

namespace Homework_20_01
{
    public class User
    {
        public string login { get; set; }
        public string password { get; set; }
    }

    internal class Program
    {
        private static List<TcpClient> connectedClients = new List<TcpClient>();
        private static List<string> userNames = new List<string>();
        private static TcpListener server;

        

        static async Task Main(string[] args)
        {
            try
            {

                if (!File.Exists("data.json"))
                {
                    Console.WriteLine($"[{DateTime.Now}]: data.json не обнаружен, выполняется создание.");

                    User[] users = new User[]
                    {
                        new User { login = "", password = "" },
                    };

                    string json = JsonConvert.SerializeObject(users);
                    File.WriteAllText("data.json", json);

                    Console.WriteLine($"[{DateTime.Now}]: data.json успешно создан.");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now}]: data.json обнаружен.");
                }

                IPAddress ip = IPAddress.Parse("127.0.0.1");
                int port = 8888;

                server = new TcpListener(ip, port);
                server.Start();

                Console.WriteLine($"[{DateTime.Now}]: Сервер запущен.");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    connectedClients.Add(client);

                    await Task.Run(() => HandleClient(client));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: {ex.Message}");
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            byte[] data = new byte[256];
            int bytes;

            try
            {

                int bytesRead = await client.GetStream().ReadAsync(data, 0, data.Length);
                string credentials = Encoding.UTF8.GetString(data, 0, bytesRead);

                string[] loginPassword = credentials.Split(',');
                if (loginPassword.Length == 3)
                {
                    string login = loginPassword[0].Trim();
                    string password = loginPassword[1].Trim();
                    string command = loginPassword[2].Trim();


                    if (command == "регистрация")
                    {
                        string json = File.ReadAllText("data.json");
                        User[] users = JsonConvert.DeserializeObject<User[]>(json);

                        bool loginExists = false;
                        foreach (User user in users)
                        {
                            if (user.login == login)
                            {
                                loginExists = true;
                                break;
                            }
                        }

                        if (loginExists)
                        {
                            string confirmationMessage = "Логин занят другим пользователем.";
                            byte[] confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
                            await Task.Run(() => client.GetStream().Write(confirmationBytes, 0, confirmationBytes.Length));
                        }
                        else
                        {
                            Array.Resize(ref users, users.Length + 1);
                            users[users.Length - 1] = new User { login = login, password = password };

                            string newJson = JsonConvert.SerializeObject(users);

                            File.WriteAllText("data.json", newJson);

                            string confirmationMessage = "Добро пожаловать.";
                            byte[] confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
                            await Task.Run(() => client.GetStream().Write(confirmationBytes, 0, confirmationBytes.Length));

                            Console.WriteLine($"[{DateTime.Now}]: {login} зарегистрировался на сервере.");
                            userNames.Add(login);

                        }
                    }


                    else if (command == "вход")
                    {
                        string json = File.ReadAllText("data.json");
                        User[] users = JsonConvert.DeserializeObject<User[]>(json);

                        bool credentialsCorrect = false;
                        foreach (User user in users)
                        {
                            if (user.login == login && user.password == password)
                            {
                                credentialsCorrect = true;
                                break;
                            }
                        }

                        if (credentialsCorrect)
                        {
                            string confirmationMessage = "Добро пожаловать.";
                            byte[] confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
                            await Task.Run(() => client.GetStream().Write(confirmationBytes, 0, confirmationBytes.Length));

                            Console.WriteLine($"[{DateTime.Now}]: {login} выполнил вход в аккаунт.");
                            userNames.Add(login);
                        }
                        else
                        {
                            string confirmationMessage = "Неверный логин или пароль.";
                            byte[] confirmationBytes = Encoding.UTF8.GetBytes(confirmationMessage);
                            await Task.Run(() => client.GetStream().Write(confirmationBytes, 0, confirmationBytes.Length));
                        }
                    }
                }

                while ((bytes = await client.GetStream().ReadAsync(data, 0, data.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(data, 0, bytes);
                    Console.WriteLine($"[{DateTime.Now}]: {message}");

                    foreach (var connectedClient in connectedClients)
                    {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        await connectedClient.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: {ex.Message}");
            }
        }
    }
}
