using LetterSendingSystem.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LetterSendingSystem
{
    /// <summary>
    /// Логика взаимодействия для MailForm.xaml
    /// </summary>
    public partial class MailForm : Window
    {
        private User userSender { get; set; }

        public MailForm(User user)
        {
            this.userSender = user;
            InitializeComponent();
            Title = $"Здравствуйте, {user.FirstName}!";
            UpdateListBoxUserLetters();
            UpdateListBoxUserHistory();
        }

        private void UpdateListBoxUserLetters()
        {
            try
            {
                listBoxUserLetters.ItemsSource = ConnectDB.GetListUserLetters(userSender.Id).Result;
            }
            catch (Exception ex)
            {
                ErrorMessegeBox(ex.Message);
            }
        }
        private void UpdateListBoxUserHistory()
        {
            try
            {
                listBoxUserHistory.ItemsSource = ConnectDB.GetListUserHistory(userSender.Id).Result;
            }
            catch (Exception ex)
            {
                ErrorMessegeBox(ex.Message);
            }
        }
        private List<User>? GetFilteredCountries(string searchText)
        {


            List<User>? Countries = null;

            try
            {
                Countries = ConnectDB.GetListUser(searchText).Result;
            }
            catch (Exception ex)
            {
                ErrorMessegeBox(ex.Message);
            }

            if (Countries == null)
                return Countries;

            return Countries;
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if ((User)ComboNameRecipient.SelectedItem is User)
                return;

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
                //cv.Filter = s => ((s.).IndexOf(ComboNameRecipient.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
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
            if ((User)ComboNameRecipient.SelectedItem is User userRecipient)
            {
                Letter letter = new Letter() { Sender = userSender.Id, Recipient = userRecipient.Id, Titel = titelTextBox.Text, Text = bodyTextBox.Text, Date = DateTime.Now };
                try
                {
                    await ConnectDB.PostLetter(letter);
                    
                    MessageBox.Show("Письмо успешно отправлено!", "Отправлено", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearTextBox();
                    UpdateListBoxUserHistory();
                    tabControl.SelectedItem = incomingTab;
                }
                catch (Exception ex)
                {
                    ErrorMessegeBox(ex.Message);
                }

            }
            else
            {
                ErrorMessegeBox("Выберите получателя письмаfff!");
                return;
            }

        }

        private void listBoxUserLetters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxUserLetters.SelectedItem is Letter letter)
            {
                const int removeSelection = -1;
                viewingNameRecipientBox.Text = ConnectDB.GetInformationOfUser(letter.Sender)?.Result?.Email;
                viewingTitelTextBox.Text = letter.Titel;
                viewingBodyTextBox.Text = letter.Text;
                tabControl.SelectedItem = viewingLetterTab;
                dateLabel.Content = $"Дата: {letter.Date}";
                listBoxUserLetters.SelectedIndex = removeSelection;

            }
        }

        private void listBoxUserHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxUserHistory.SelectedItem is Letter letter)
            {
                const int removeSelection = -1;
                viewingNameRecipientBox.Text = ConnectDB.GetInformationOfUser(letter.Sender)?.Result?.Email;
                viewingTitelTextBox.Text = letter.Titel;
                viewingBodyTextBox.Text = letter.Text;
                dateLabel.Content = $"Дата: {letter.Date}";
                listBoxUserHistory.SelectedIndex = removeSelection;

                tabControl.SelectedItem = viewingLetterTab;

            }
        }
    }
}
