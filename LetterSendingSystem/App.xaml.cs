using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
    }

}
