using System;
using System.Collections.Generic;
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
            
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User? user = ConnectDB.GetUser(loginTextBox.Text, passwordBox.Password).Result;
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
