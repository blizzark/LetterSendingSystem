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
            ComboNames.ItemsSource = GetCountries();
        }
        private List<string> GetCountries()
        {
            List<string> countries = new List<string>();
            countries.Add("Россия");
            countries.Add("Украина");
            countries.Add("Беларусь");
            countries.Add("Казахстан");
            countries.Add("Армения");
            countries.Add("Грузия");
            // и так далее...
            return countries;
        }
        private List<string> GetFilteredCountries(string searchText)
        {
            List<string> filteredCountries = new List<string>();
            foreach (string country in GetCountries())
            {
                if (country.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    filteredCountries.Add(country);
                }
            }
            return filteredCountries;
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ComboNames.Text;
            ComboNames.ItemsSource = GetFilteredCountries(searchText);
        }
    }
}
