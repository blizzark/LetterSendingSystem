using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для MailForm.xaml
    /// </summary>
    public partial class MailForm : Window
    {
        private User user { get; set; }

        public MailForm(User user)
        {
            this.user = user;
            InitializeComponent();
            Title = $"Здравствуйте, {user.FirstName}!";
        }
        
        private List<string> GetFilteredCountries(string searchText)
        {
            List<string> filteredCountries = new List<string>();
            List<User>? Countries = ConnectDB.GetListUser(searchText).Result;

            if (Countries == null)
                return filteredCountries;

            foreach (User usersCountries in Countries!)
            {
                
                filteredCountries.Add(usersCountries.FirstName + " " + usersCountries.SecondName + " " + usersCountries.Email);
                
            }
            return filteredCountries;
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ComboNameRecipient.Text;
            ComboNameRecipient.ItemsSource = GetFilteredCountries(searchText);

            var tb = (TextBox)e.OriginalSource;
            if (tb.SelectionStart != 0)
            {
                ComboNameRecipient.SelectedItem = null; // Если набирается текст сбросить выбраный элемент
            }
            if (tb.SelectionStart == 0 && ComboNameRecipient.SelectedItem == null)
            {
                ComboNameRecipient.IsDropDownOpen = false; // Если сбросили текст и элемент не выбран, сбросить фокус выпадающего списка
            }
            ComboNameRecipient.IsDropDownOpen = true;

            if (ComboNameRecipient.SelectedItem == null)
            {
                // Если элемент не выбран менять фильтр
                CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(ComboNameRecipient.ItemsSource);
                cv.Filter = s => ((string)s).IndexOf(ComboNameRecipient.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
        }

        private void ErrorMessegeBox(string mes)
        {
            MessageBox.Show(mes, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void ClearTextBox()
        {
            ComboNameRecipient.Text = string.Empty;
            titelTextBox.Text = string.Empty;
            bodyTextBox.Text = string.Empty;
        }

        private async void ButtonSendLetter_Click(object sender, RoutedEventArgs e)
        {
            if (ComboNameRecipient.Text == string.Empty)
            {
                ErrorMessegeBox("Выберите получателя письма!");
                return;
            }
            
            Letter letter = new Letter() { Sender = user.Id, Recipient = user.Id, Titel = titelTextBox.Text, Text = bodyTextBox.Text, Date = DateTime.Now};
            try
            {
                await ConnectDB.PostLetter(letter);
                ClearTextBox();
                MessageBox.Show("Письмо успешно отправлено!", "Отправлено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                ErrorMessegeBox(ex.Message);
            }


        }
    }
}
