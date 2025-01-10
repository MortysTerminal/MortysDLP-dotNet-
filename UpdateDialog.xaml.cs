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

namespace MortysDLP_dotNet_
{
    /// <summary>
    /// Interaktionslogik für UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        public UpdateDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OnYesButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Setzt den Rückgabewert auf true
            Close();
        }

        private void OnNoButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Setzt den Rückgabewert auf false
            Close();
        }
    }
}
