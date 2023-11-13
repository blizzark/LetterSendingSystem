using LetterSendingSystem.Connect;
using LetterSendingSystem.Entities;
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

        private void returnToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            Authorization win = new Authorization();
            this.Hide();
            win.ShowDialog();
            this.Close();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            User user = new User() {FirstName = firstNameBox.Text, SecondName = secondNameBox.Text,
                Email = eMailBox.Text, Password = passwordBox.Password, ConfirmPassword = confirmPasswordBox.Password };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(user);

            if (!Validator.TryValidateObject(user, context, results, true))
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in results)
                {
                    errorMessage.Append(error.ErrorMessage + '\n');
                }
                App.ErrorMessegeBox(errorMessage.ToString());
            }
            else
            {
                try
                {
                    user.Password = LetterSendingSystem.MD5.GetHash(user.Password);
                    user = UserRepository.CreateUser(user).Result!;

                    MessageBox.Show("Пользователь создан!", "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
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
