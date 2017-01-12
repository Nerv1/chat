using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;

namespace SocketTcpServer
{ 
    
    class Program
    {
        public static string serverName = "127.0.0.1"; // Адрес сервера (для локальной базы пишите "localhost")
        public static string userName = "root"; // Имя пользователя
        public static string dbName = "socket_chat"; //Имя базы данных
        public static string port1 = "3306"; // Порт для подключения
        public static string password = ""; // Пароль для подключения

        public static string connStr = "server=" + serverName +
            ";user=" + userName +
            ";database=" + dbName +
            ";port=" + port1 +
            ";password=" + password + ";"; 



        public static int countClients = 0;

        public static int[] ArrayClients = new int[1000];
        public static string[] ArrayNameClients = new string[1000];

        static List<Socket> Connections = new List<Socket>();

        static int port = 8005; // порт для приема входящих запросов
         

        static void func(object numb1)
        {
            MySqlConnection connection = new MySqlConnection(connStr); // підключення
            connection.Open(); // відкриваємо зв'язок з БД


            string sql2 = ""; //  запрос
            string sql3 = "";

            

            int countMessag = 0;
            numb1 = (int)numb1 - 1;
            for (;;)
            {
                try
                {

                    // отримуємо  повідомлення
                    StringBuilder builder = new StringBuilder();
                    int bytes1 = 0; // кількість получених байтів
                    byte[] data1 = new byte[256]; // буфер для получених данних

                    do
                    {
                        bytes1 = Connections[(int)numb1].Receive(data1);// чекаємо на повідомлення
                        builder.Append(Encoding.Unicode.GetString(data1, 0, bytes1));
                    }
                    while (Connections[(int)numb1].Available > 0);

                    countMessag++;

                    if (bytes1 == 0) // коли користувач відключився
                    {
                        Console.WriteLine("Користувач " + ArrayNameClients[(int)numb1] + " вiдключився");

                        sql3 = "INSERT INTO `date` (`name`, `message`, `time`) VALUES ( 'System', 'Користувач " + ArrayNameClients[(int)numb1] + " вiдключився','" + DateTime.Now.ToShortTimeString() + "')";


                        MySqlCommand sqlCom2 = new MySqlCommand(sql3, connection);
                        sqlCom2.ExecuteNonQuery();//відправка запиту

                        for (int i = 0; i < countClients; i++)
                        {
                            // відправляємо повідомлення усім хто підключений
                            if (ArrayClients[i] != 0)
                            {
                                data1 = Encoding.Unicode.GetBytes("[" + DateTime.Now.ToShortTimeString() + "] System: Користувач " + ArrayNameClients[(int)numb1] + " вiдключився");
                                Connections[i].Send(data1);
                            }
                        }
                        ArrayClients[(int)numb1] = 0;
                        return;
                    }
                    // коли перше повідомлення є ім'я користувача
                    string[] stringArray = builder.ToString().Split(new Char[] { ':' });
                    if (countMessag==1 && stringArray[0] == "my names is ")
                    {
                        ArrayNameClients[(int)numb1] = stringArray[1];
                        Console.WriteLine("Пiдключився користувач: " + ArrayNameClients[(int)numb1]);

                        sql2 = "INSERT INTO `date` (`name`, `message`, `time`) VALUES ( 'System', 'Пiдключився користувач: " + ArrayNameClients[(int)numb1] + "','" + DateTime.Now.ToShortTimeString() + "')";

                        for (int i = 0; i < countClients; i++)
                        {
                            // відправляємо повідомлення усім хто підключений
                            if (ArrayClients[i] != 0)
                            {
                                data1 = Encoding.Unicode.GetBytes("[" + DateTime.Now.ToShortTimeString() + "] System: Пiдключився користувач: " + ArrayNameClients[(int)numb1]);
                                Connections[i].Send(data1);
                            }
                        }
                    }
                    else
                    {
                        if (countMessag == 1)// коли перше повідомлення не ім'я користувача, його ім'я буде - Unknown
                        {
                            Console.WriteLine("Пiдключився користувач: " + ArrayNameClients[(int)numb1]);

                            sql3 = "INSERT INTO `date` (`name`, `message`, `time`) VALUES ( 'System', 'Пiдключився користувач: " + ArrayNameClients[(int)numb1] + "','" + DateTime.Now.ToShortTimeString() + "')";
                            MySqlCommand sqlCom1 = new MySqlCommand(sql3, connection);
                            sqlCom1.ExecuteNonQuery();//відправка запиту

                            for (int i = 0; i < countClients; i++)
                            {
                                // відправляємо повідомлення усім хто підключений
                                if (ArrayClients[i] != 0)
                                {
                                    data1 = Encoding.Unicode.GetBytes("[" + DateTime.Now.ToShortTimeString() + "] System: Пiдключився користувач: " + ArrayNameClients[(int)numb1]);
                                    Connections[i].Send(data1);
                                }
                            }
                        }

                        // запис повідомлення в БД
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                        sql2 = "INSERT INTO `date` (`name`, `message`, `time`) VALUES ( '" + ArrayNameClients[(int)numb1] + "', '" + builder.ToString() + "','" + DateTime.Now.ToShortTimeString() + "')";

                        for (int i = 0; i < countClients; i++)
                        {
                            // відправляємо повідомлення усім хто підключений
                            if (ArrayClients[i] != 0)
                            {
                                string message = "["+DateTime.Now.ToShortTimeString() + "] " + ArrayNameClients[(int)numb1] + ": " + builder.ToString();
                                data1 = Encoding.Unicode.GetBytes(message);
                                Connections[i].Send(data1);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Користувач " + ArrayNameClients[(int)numb1] + " вiдключився");
                    sql2 = "INSERT INTO `date` (`name`, `message`, `time`) VALUES ( 'System', 'Користувач " + ArrayNameClients[(int)numb1] + " вiдключився','" + DateTime.Now.ToShortTimeString() + "')";

                    MySqlCommand sqlCom3 = new MySqlCommand(sql2, connection);
                    sqlCom3.ExecuteNonQuery();//відправка запиту

                    ArrayClients[(int)numb1] = 0;
                    byte[] data2 = new byte[256]; // буфер для получених данних
                    for (int i = 0; i < countClients; i++)
                    {
                        // відправляємо повідомлення усім хто підключений
                        if (ArrayClients[i] != 0)
                        {
                            data2 = Encoding.Unicode.GetBytes("[" + DateTime.Now.ToShortTimeString() + "] System: Користувач " + ArrayNameClients[(int)numb1] + " вiдключився");
                            Connections[i].Send(data2);
                        }
                    }
                    return;
                }
                MySqlCommand sqlCom = new MySqlCommand(sql2, connection);
                sqlCom.ExecuteNonQuery();//відправка запиту
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Сервер";
            byte[] data3 = new byte[256]; // буфер для получених данних
            int numb = 0;
            // отримуємо адрес для запуску сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // створюємо сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // зв'язуємо сокет з локальною точкою, по якій будемо отримувати дані
                listenSocket.Bind(ipPoint);

                // починаємо слухати
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущений. Очiкування пiдключення...");

                while (true)
                {
                    Console.WriteLine("Кiлькiсть пiдключень: " + countClients);
                    // чекаємо на підключення клієнта
                    Socket handler = listenSocket.Accept();
                    
                    // підключилися
                    
                    Connections.Add(handler);
                    ArrayClients[numb] = 1;
                    ArrayNameClients[numb] = "Unknown";

                    countClients++;
                    numb++;

                    string chatText3 = "";
                    string[] chatText2 = new string[1000];

                   
                    ParameterizedThreadStart mydelegate = new ParameterizedThreadStart(func);
                    Thread myThread = new Thread(mydelegate);
                    myThread.Start(numb);

                    MySqlConnection connection = new MySqlConnection(connStr); // підключення
                    connection.Open(); // відкриваємо зв'язок з БД


                    string sql = "SELECT time, name, message FROM date"; //  запрос

                    MySqlCommand sqlCom = new MySqlCommand(sql, connection);
                    sqlCom.ExecuteNonQuery();//відправка запиту

                    // отримання відповіді
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    var myData = dt.Select();

                    //Console.WriteLine(myData.Length); // кількість рядків
                    if (myData.Length > 500)
                    {
                        int count = myData.Length - 500;

                        for (int i = count; i < myData.Length; i++)
                        {
                            for (int j = 0; j < myData[i].ItemArray.Length; j++)
                            {
                                if (j == 0)
                                {
                                    chatText3 += "[" + myData[i].ItemArray[j] + "] ";
                                }
                                if (j == 1)
                                {
                                    chatText3 += myData[i].ItemArray[j] + ": ";
                                }
                                if (j == 2)
                                {
                                    chatText3 += myData[i].ItemArray[j].ToString();
                                }
                            }
                            chatText3 += "\r\n";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myData.Length; i++)
                        {
                            for (int j = 0; j < myData[i].ItemArray.Length; j++)
                            {
                                if (j == 0)
                                {
                                    chatText3 += "[" + myData[i].ItemArray[j] + "] ";
                                }
                                if (j == 1)
                                {
                                    chatText3 += myData[i].ItemArray[j] + ": ";
                                }
                                if (j == 2)
                                {
                                    chatText3 += myData[i].ItemArray[j].ToString();
                                }
                            }
                            chatText3 += "\r\n";
                        }
                    }

                    connection.Close();

                    data3 = Encoding.Unicode.GetBytes(chatText3);
                    handler.Send(data3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}