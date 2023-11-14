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
        //List of received letters
        private ObservableCollection<Letter> UserLetters { get; set; } = null!;
        //List of sent letters
        private ObservableCollection<Letter> UserHistory { get; set; } = null!;
        //Current authorized user. He's the sender.
        private User userSender { get; set; }

        //Current page for viewing received emails
        private int pageListBoxUserLetters = 0;
        //Current page for viewing sent emails
        private int pageListBoxUserHistory = 0;
        public MailForm(User user)
        {
            this.userSender = user;
            InitializeComponent();

          
            Title = $"Здравствуйте, {user.FirstName}!";
            UserLetters = LoadListBoxUserLetters();
            UserHistory = LoadListBoxUserHistory();
            listBoxUserLetters.ItemsSource = UserLetters;
            listBoxUserHistory.ItemsSource = UserHistory;

        }





        /// <summary>
        /// Loads the first sample of received emails.
        /// </summary>
        /// <returns>A sample if there is one. Empty selection if there is none</returns>
        private ObservableCollection<Letter> LoadListBoxUserLetters()
        {
            List<Letter>? letters = null;
            try
            {
                letters = LetterRepository.GetListUserLetters(userSender.Id, pageListBoxUserLetters++).Result;
            }
            catch (Exception ex)
            {
                App.ErrorMessegeBox(ex.Message);
            }
            if (letters is null)
                return new ObservableCollection<Letter>();
            return new ObservableCollection<Letter>(letters);
        }

        /// <summary>
        /// Loads the first sample of sent emails.
        /// </summary>
        /// <returns>A sample if there is one. Empty selection if there is none</returns>
        private ObservableCollection<Letter> LoadListBoxUserHistory()
        {
            List<Letter>? letters = null;
            try
            {
                letters = LetterRepository.GetListUserHistory(userSender.Id, pageListBoxUserHistory++).Result;
            }
            catch (Exception ex)
            {
                App.ErrorMessegeBox(ex.Message);
            }

            if (letters is null)
                return new ObservableCollection<Letter>();
            return new ObservableCollection<Letter>(letters);

        }

        /// <summary>
        /// Sends a request to search for the occurrence of a line in a full name or email
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
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

        //Variable required for the search to work correctly
        private bool selectedComboBoxNameRecipient = false;
        /// <summary>
        /// User search and selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Clearing the letter form
        /// </summary>
        private void ClearTextBox()
        {
            ComboNameRecipient.Text = string.Empty;
            titelTextBox.Text = string.Empty;
            bodyTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Button to send a form with a letter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonSendLetter_Click(object sender, RoutedEventArgs e)
        {
            if (ComboNameRecipient.Text == string.Empty)
            {
                App.ErrorMessegeBox("Выберите получателя письма!");
                return;
            }
            if ((User)ComboNameRecipient.SelectedItem is User userRecipient)
            {
                Letter letter = new Letter() { Sender = userSender.Id, Recipient = userRecipient.Id, Titel = titelTextBox.Text, Text = bodyTextBox.Text, Date = DateTime.Now, EmailSender = userRecipient.Email};
                try
                {
                    await LetterRepository.SendLetter(letter);
                    
                    MessageBox.Show("Письмо успешно отправлено!", "Отправлено", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearTextBox();
                    UserHistory.Insert(0,letter);
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
        /// <summary>
        /// Event of opening an existing message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxUserLetters_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        /// <summary>
        /// Event of opening an existing message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxUserHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        /// <summary>
        /// Logout event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToAuth_Click(object sender, RoutedEventArgs e)
        {
            Authorization win = new Authorization();
            this.Hide();
            win.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Flag setting event if a user has been selected for sending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboNameRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedComboBoxNameRecipient = true;
        }

        /// <summary>
        /// Scroll event ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
           
            e.Handled = true;
        }

        /// <summary>
        /// Receiving a sample of letters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewerHistory_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Checking to see if we have reached the end of the ListView
            if (scrollViewerHistory.VerticalOffset == scrollViewerHistory.ScrollableHeight)
            {
                try
                {
                    var letters = LetterRepository.GetListUserHistory(userSender.Id, pageListBoxUserHistory++).Result;

                    if (letters is null)
                        return;

                    foreach (var lettersItem in letters)
                    {
                        UserHistory.Add(lettersItem);
                    }

                  
                }
                catch (Exception ex)
                {
                    App.ErrorMessegeBox(ex.Message);
                }
            }
        }
        /// <summary>
        /// Receiving a sample of letters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewerLetters_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Проверяем, достигли ли мы конца ListView при прокрутке
            if (scrollViewerLetters.VerticalOffset == scrollViewerLetters.ScrollableHeight)
            {
                try
                {
                    var letters = LetterRepository.GetListUserLetters(userSender.Id, pageListBoxUserHistory++).Result;

                    if (letters is null)
                        return;

                    foreach (var lettersItem in letters)
                    {
                        UserLetters.Add(lettersItem);
                    }


                }
                catch (Exception ex)
                {
                    App.ErrorMessegeBox(ex.Message);
                }
            }
        }
    }
}
