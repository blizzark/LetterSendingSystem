using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LetterSendingSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises an error alert
        /// </summary>
        /// <param name="mes"></param>
        public static void ErrorMessegeBox(string mes)
        {
            MessageBox.Show(mes, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ValidateObject<T>(T obj)
        {
            if (obj is null)
                return false;

            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(obj);

            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in results)
                {
                    errorMessage.Append(error.ErrorMessage + '\n');
                }
                ErrorMessegeBox(errorMessage.ToString());
                return false;
            }
            return true;
            
        }
    }

}
