using LetterSendingSystem.Connect;
using LetterSendingSystem.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace LetterSendingSystem
{
    /// <summary>
    /// Логика взаимодействия для MailForm.xaml
    /// </summary>
    public partial class MailForm : Window
    {
        private ObservableCollection<Letter>? UserLetters { get; set; }
        private ObservableCollection<Letter>? UserHistory { get; set; }
        private User userSender { get; set; }

        private int pageListBoxUserLetters = 0;
        private int pageListBoxUserHistory = 0;
        public MailForm(User user)
        {
            this.userSender = user;
            InitializeComponent();
           
            Title = $"Здравствуйте, {user.FirstName}!";
            LoadListBoxUserLetters();
            LoadListBoxUserHistory();

        }






        private void LoadListBoxUserLetters()
        {
            try
            {
                var letters = LetterRepository.GetListUserLetters(userSender.Id, pageListBoxUserLetters).Result;
                if (letters is null)
                {
                    UserLetters = new ObservableCollection<Letter>();
                    listBoxUserLetters.ItemsSource = UserLetters;
                    return;
                }
                UserLetters = new ObservableCollection<Letter>(letters);
                listBoxUserLetters.ItemsSource = UserLetters;
            }
            catch (Exception ex)
            {
                App.ErrorMessegeBox(ex.Message);
            }
        }


        private void LoadListBoxUserHistory()
        {
            try
            {
                var letters = LetterRepository.GetListUserHistory(userSender.Id, pageListBoxUserLetters).Result;
                if (letters is null)
                {
                    UserHistory = new ObservableCollection<Letter>();
                    listBoxUserHistory.ItemsSource = UserHistory;
                    return;
                }
                UserHistory = new ObservableCollection<Letter>(letters);
                listBoxUserHistory.ItemsSource = UserHistory;
            }
            catch (Exception ex)
            {
                App.ErrorMessegeBox(ex.Message);
            }
        }
        private List<User>? GetFilteredCountries(string searchText)
        {


            List<User>? Countries = null;

            try
            {
                Countries = UserRepository.GetListUser(searchText).Result;
            }
            catch (Exception ex)
            {
                App.ErrorMessegeBox(ex.Message);
            }

            if (Countries == null)
                return Countries;

            return Countries;
        }
        private bool selectedComboBoxNameRecipient = false;
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if ((User)ComboNameRecipient.SelectedItem is User && selectedComboBoxNameRecipient)
            {
                selectedComboBoxNameRecipient = false;
                return;
            }

           

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
                App.ErrorMessegeBox("Выберите получателя письма!");
                return;
            }
            if ((User)ComboNameRecipient.SelectedItem is User userRecipient)
            {
                Letter letter = new Letter() { Sender = userSender.Id, Recipient = userRecipient.Id, Titel = titelTextBox.Text, Text = bodyTextBox.Text, Date = DateTime.Now };
                try
                {
                    await LetterRepository.SendLetter(letter);
                    
                    MessageBox.Show("Письмо успешно отправлено!", "Отправлено", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearTextBox();
                    UserHistory?.Add(letter);
                    tabControl.SelectedItem = incomingTab;
                }
                catch (Exception ex)
                {
                    App.ErrorMessegeBox(ex.Message);
                }

            }
            else
            {
                App.ErrorMessegeBox("Выберите получателя письма!");
                return;
            }

        }

        private void listBoxUserLetters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxUserLetters.SelectedItem is Letter letter)
            {
                const int removeSelection = -1;
                viewingNameRecipientBox.Text = letter.EmailSender;
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
                viewingNameRecipientBox.Text = letter.EmailSender;
                viewingTitelTextBox.Text = letter.Titel;
                viewingBodyTextBox.Text = letter.Text;
                dateLabel.Content = $"Дата: {letter.Date}";
                listBoxUserHistory.SelectedIndex = removeSelection;

                tabControl.SelectedItem = viewingLetterTab;

            }
        }

        private void ExitToAuth_Click(object sender, RoutedEventArgs e)
        {
            Authorization win = new Authorization();
            this.Hide();
            win.ShowDialog();
            this.Close();
        }

        private void ComboNameRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedComboBoxNameRecipient = true;
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (e.Delta > 0)
            {
                scrollViewer.LineUp();
            }
            else
            {
                scrollViewer.LineDown();
            }
            // Остановить обработку события прокрутки родительским элементом.
            e.Handled = true;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Проверяем, достигли ли мы конца ListView при прокрутке
            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
            {
              
            }
        }
    }
}
