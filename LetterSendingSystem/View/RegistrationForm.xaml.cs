using LetterSendingSystem.Connect;
using LetterSendingSystem.Entities;
using LetterSendingSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LetterSendingSystem
{
    /// <summary>
    /// Логика взаимодействия для RegistrationForm.xaml
    /// </summary>
    public partial class RegistrationForm : Window
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Return event to the authorization form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returnToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            Authorization win = new Authorization();
            this.Hide();
            win.ShowDialog();
            this.Close();
        }
        /// <summary>
        /// Registration event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            User user = new User() {FirstName = firstNameBox.Text, SecondName = secondNameBox.Text,
                Email = eMailBox.Text, Password = passwordBox.Password, ConfirmPassword = confirmPasswordBox.Password };

            if (App.ValidateObject(user))
            {
                try
                {
                    user.Password = MD5.GetHash(user.Password);
                    user = UserRepository.CreateUser(user).Result!;
                    if (user is null)
                        throw new Exception(MessageConst.CHECK_YOUR_CONNECTION);

                    MessageBox.Show(MessageConst.USER_CREATED, MessageConst.SUCCESSFUL_REGISTRATION, MessageBoxButton.OK, MessageBoxImage.Information);
                    MailForm win = new MailForm(user);
                    this.Hide();
                    win.ShowDialog();
                    this.Close();
                }
                catch(Exception ex)
                {
                    App.ErrorMessegeBox(ex.Message);
                }


                
            }
        }
    }
}
