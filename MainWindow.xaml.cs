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
using System.Reflection;
using System.Diagnostics;
using MortysDLP_dotNet_.Properties;
using System.Runtime.Intrinsics.Arm;
using static System.Collections.Specialized.BitVector32;

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

            if (Properties.Settings.Default.CHECKED_ZEITSPANNE.Equals("1")) {
                cb_Zeitspanne.IsChecked = true;
                tb_zeitspanne_von.Text = Properties.Settings.Default.ZEITSPANNE_VON;
                tb_zeitspanne_bis.Text = Properties.Settings.Default.ZEITSPANNE_BIS;
            }
            else { 
                cb_Zeitspanne.IsChecked = false;
                tb_zeitspanne_von.Text = "";
                tb_zeitspanne_bis.Text = "";
            }

            if (Properties.Settings.Default.CHECKED_ERSTESEKUNDEN.Equals("1"))
            {
                cb_ErsteSekunden.IsChecked = true;
                tb_ErsteSekunden_Sekunden.Text = Properties.Settings.Default.ERSTESEKUNDEN_SEKUNDEN;
            }
            else
            {
                cb_ErsteSekunden.IsChecked = false;
                tb_ErsteSekunden_Sekunden.Text = "";

            }

            if (Properties.Settings.Default.CHECKED_VIDEOFORMAT.Equals("1"))
            {
                cb_Videoformat.IsChecked = true;
            }
            else
            {
                cb_Videoformat.IsChecked = false;
            }

            if (Properties.Settings.Default.DOWNLOADPATH.Equals("") || Properties.Settings.Default.DOWNLOADPATH.Equals("0"))
            {
                tb_downloadpath.Text = defaultdownloadsPath;
            }

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
            // Lade Zustaende aus Form
            string tempdownloadpath = tb_downloadpath.Text;
            string cb_ErsteSekundenChecked = "0";
            string cb_VideoformatChecked = "0";
            string cb_ZeitspanneChecked = "0";
            string tb_Von_Text = "";
            string tb_Bis_Text = "";
            string tb_ErsteSekunden_Text = "";
            if (cb_Zeitspanne.IsChecked == true)
            {
                cb_ZeitspanneChecked = "1";
                tb_Von_Text = tb_zeitspanne_von.Text;
                tb_Bis_Text = tb_zeitspanne_bis.Text;
            }

            if (cb_ErsteSekunden.IsChecked == true) 
            { 
                cb_ErsteSekundenChecked = "1"; 
                tb_ErsteSekunden_Text = tb_ErsteSekunden_Sekunden.Text;
            }
            if (cb_Videoformat.IsChecked == true)   cb_VideoformatChecked = "1";

            var Result = System.Windows.MessageBox.Show("Sollen die Einstellungen für den nächsten Programmstart gespeichert werden?", "Einstellunge abspeichern?", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (Result == System.Windows.MessageBoxResult.Yes)
            {
                Properties.Settings.Default.CHECKED_ZEITSPANNE = cb_ZeitspanneChecked;
                Properties.Settings.Default.ZEITSPANNE_VON = tb_Von_Text;
                Properties.Settings.Default.ZEITSPANNE_BIS = tb_Bis_Text;
                Properties.Settings.Default.CHECKED_ERSTESEKUNDEN = cb_ErsteSekundenChecked;
                Properties.Settings.Default.ERSTESEKUNDEN_SEKUNDEN = tb_ErsteSekunden_Text;
                Properties.Settings.Default.CHECKED_VIDEOFORMAT = cb_VideoformatChecked;
                Properties.Settings.Default.DOWNLOADPATH = tempdownloadpath;
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
                Properties.Settings.Default.CHECKED_ZEITSPANNE = "0";
                Properties.Settings.Default.ZEITSPANNE_VON = "0";
                Properties.Settings.Default.ZEITSPANNE_BIS = "0";
                Properties.Settings.Default.CHECKED_ERSTESEKUNDEN = "0";
                Properties.Settings.Default.ERSTESEKUNDEN_SEKUNDEN = "0";
                Properties.Settings.Default.CHECKED_VIDEOFORMAT = "0";
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
        private string BaueYTDLPArgumente(string ffmpegpath)
        {
            // DOWNLOAD FIRST 15 SEC
            //yt-dlp -f "bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b" -o "%(title)s.%(ext)s" --downloader ffmpeg --downloader-args "ffmpeg:-t 10"  ""
            // yt-dlp -f "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best" "https://youtu.be/pvm6avc8Zwo?si=MHQrnVOevYhbO1zW" -S vcodec:h264



            string ba_Args = "-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\"";

            

            //URL
            ba_Args += " \"" + tb_URL.Text + "\"";

            

            if (cb_Videoformat.IsChecked == true)
            {
                ba_Args += " -S vcodec: h264 --no-mtime ";
            }

            if (cb_Zeitspanne.IsChecked == true)
            {
                // --download-sections "*00:32:53.2-00:32:55.5"
                ba_Args += " -o \"" + tb_downloadpath.Text + "\\z_%(title)s.%(ext)s\"";
                ba_Args += " --download-sections \"*" + tb_zeitspanne_von.Text + "-" + tb_zeitspanne_bis.Text + "\"";
            }
            else if(cb_ErsteSekunden.IsChecked == true)
            {
                ba_Args += " -o \"" + tb_downloadpath.Text + "\\" + tb_ErsteSekunden_Sekunden.Text + "_%(title)s.%(ext)s\"";
                ba_Args += " --downloader \""+ ffmpegpath+ "\" --downloader-args \"ffmpeg:-t " + tb_ErsteSekunden_Sekunden.Text + "\"";
            }
            else
            {
                ba_Args += " -o \"" + tb_downloadpath.Text + "\\%(title)s.%(ext)s\"";
            }

            // Skip Cert
            ba_Args += " --no-check-certificates ";

            return ba_Args;

        }
        private void DownloadStarten_Click(object sender, RoutedEventArgs e)
        {
            // TODO PFADE EINBINDEN von yt-dlp.exe und ffmpeg.exe
            // Selbststaendiges Auslesen der pfade!
            var path = "";
            var ffmpegpath = "";

            string args = BaueYTDLPArgumente(ffmpegpath);


            System.Diagnostics.Process process1 = new System.Diagnostics.Process();
            process1.StartInfo.FileName = path;
            process1.StartInfo.Arguments = args;
            process1.Start();
           //process1.WaitForExit();
            //process1.Close();
        }

        
    }
}