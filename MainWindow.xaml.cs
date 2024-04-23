using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using Syroot.Windows.IO;

namespace MortysDLP_dotNet_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string defaultdownloadsPath = KnownFolders.Downloads.Path;
        string downloadspath = "";

        public MainWindow()
        {
            InitializeComponent();
            ErsterStart();
            DatenLaden();
            ZeitspanneAnpassen();
            ErsteSekundenAnpassen();
        }

        private void topBar_Info_Click(object sender, RoutedEventArgs e)
        {
        }
        private void cbZeitspanneCheck(object sender, RoutedEventArgs e)
        {
            ZeitspanneAnpassen();
        }
        private void cbErsteSekundenCheck(object sender, RoutedEventArgs e)
        {
            ErsteSekundenAnpassen();
        }
        private void ErsterStart()
        {
            if ((Properties.Settings.Default.DOWNLOADPATH).Equals(""))
            {
                this.downloadspath = defaultdownloadsPath;
            }
            else
            {
                this.downloadspath = Properties.Settings.Default.DOWNLOADPATH;
            }
        }
        private void DatenLaden()
        {
            // Downloadpfad einfuegen in TextBox
            tb_downloadpath.Text = downloadspath;
        }
        private void DownloadPfadDurchsuchen(object sender, RoutedEventArgs e)
        {
            // Configure open folder dialog box
            Microsoft.Win32.OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Wähle das Verzeichnis in welchem die Dateien gespeichert werden sollen";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                tb_downloadpath.Text = dialog.FolderName;
                //string folderNameOnly = dialog.SafeFolderName;
            }
        }
        private void EinstellungenSpeichern(object sender, RoutedEventArgs e)
        {
            var Result = System.Windows.MessageBox.Show("Sollen die Einstellungen für den nächsten Programmstart gespeichert werden?", "Einstellunge abspeichern?", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (Result == System.Windows.MessageBoxResult.Yes)
            {
                Properties.Settings.Default.DOWNLOADPATH = tb_downloadpath.Text;
                Properties.Settings.Default.Save();
                System.Windows.MessageBox.Show("Einstellungen gespeichert");
            }
            else if (Result == System.Windows.MessageBoxResult.No)
            {
                Environment.Exit(0);
            }
        }
        private void TopBar_EinstellungenReset(object sender, RoutedEventArgs e)
        {
            var Result = System.Windows.MessageBox.Show("Sollen die Einstellungen auf den Standard zurückgesetzt werden?", "Einstellungen zurücksetzen?", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (Result == System.Windows.MessageBoxResult.Yes)
            {
                Properties.Settings.Default.DOWNLOADPATH = "";
                Properties.Settings.Default.Save();
                System.Windows.MessageBox.Show("Einstellungen zurückgesetzt. Software muss neu gestartet werden.");
                Environment.Exit(0);
            }
            else if (Result == System.Windows.MessageBoxResult.No)
            {
            }
        }
        private void ErsteSekundenAnpassen()
        {
            //if(cb_Zeitspanne.IsChecked == true)
            //{
            //    cb_ErsteSekunden.IsEnabled = false;
            //}

            if (cb_ErsteSekunden.IsChecked == true)
            {
                txt_ErsteSekunden_info1.Foreground = Brushes.Black;
                txt_ErsteSekunden_info2.Foreground = Brushes.Black;
                tb_ErsteSekunden_Sekunden.IsEnabled = true;
                tb_ErsteSekunden_Sekunden.IsReadOnly = false;
                cb_Zeitspanne.IsEnabled = false;
            }
            else
            {
                txt_ErsteSekunden_info1.Foreground = Brushes.Silver;
                txt_ErsteSekunden_info2.Foreground = Brushes.Silver;
                tb_ErsteSekunden_Sekunden.IsEnabled = false;
                tb_ErsteSekunden_Sekunden.IsReadOnly = true;
                cb_Zeitspanne.IsEnabled = true;
            }

        }
        private void ZeitspanneAnpassen()
        {
            if(cb_Zeitspanne.IsChecked == true)
            {
                txt_zeitspanne_von.Foreground = Brushes.Black;
                txt_zeitspanne_info.Foreground = Brushes.Black;
                txt_zeitspanne_bindestrich.Foreground = Brushes.Black;
                tb_zeitspanne_von.IsReadOnly = false;
                tb_zeitspanne_von.IsEnabled = true;
                tb_zeitspanne_bis.IsReadOnly = false;
                tb_zeitspanne_bis.IsEnabled = true;
                cb_ErsteSekunden.IsEnabled = false;

            }
            else {
                txt_zeitspanne_von.Foreground = Brushes.Silver;
                txt_zeitspanne_info.Foreground = Brushes.Silver;
                txt_zeitspanne_bindestrich.Foreground = Brushes.Silver;
                tb_zeitspanne_von.IsReadOnly = true;
                tb_zeitspanne_von.IsEnabled = false;
                tb_zeitspanne_bis.IsReadOnly = true;
                tb_zeitspanne_bis.IsEnabled = false;
                cb_ErsteSekunden.IsEnabled = true;


            }
        }
    }
}