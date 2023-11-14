using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LetterSendingSystem.Entities;
using LetterSendingSystem.JsonItems;
using System.Text.Json;
using System.Text.Json.Serialization;
using LetterSendingSystem.Connect;
using LetterSendingSystem.Helper;

namespace LetterSendingSystem
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
            LoadJson();
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "appsettings.json"))
            {
                string json = r.ReadToEnd();
                ServerData serverData = JsonSerializer.Deserialize<ServerData>(json)!;   
                Request.hostName = serverData.ServerUrl;
            }
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = MD5.GetHash(passwordBox.Password);



            try
            {
                User? user = UserRepository.Auth(new RestClient() {Login = login, Password = password }).Result;
                if (user != null)
                {
                    MailForm win = new MailForm(user);
                    this.Hide();
                    win.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm win = new RegistrationForm();
            this.Hide();
            win.ShowDialog();
            this.Close();
        }
    }
}
