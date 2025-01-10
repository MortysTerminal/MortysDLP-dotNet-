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
using System.IO;
using System.Text.Json;
using System.Net;
using System.Net.Http;

namespace MortysDLP_dotNet_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string defaultdownloadsPath = KnownFolders.Downloads.Path;
        //string downloadspath = "";
        private string YtDlpPath = Properties.Settings.Default.YT_DLP_PATH;
        private string FfmpegPath = Properties.Settings.Default.FFMPEG_PATH;
        private string FfprobePath = Properties.Settings.Default.FFPROBE_PATH;
        //private readonly string YtDlpUrl = Properties.Settings.Default.YT_DLP_URL;
        //private readonly string FfmpegUrl = Properties.Settings.Default.FFMPEG_URL; // Beispiel-URL; muss ggf. angepasst werden



        public MainWindow()
        {
            InitializeComponent();

            DatenLaden();
            ZeitspanneAnpassen();
            ErsteSekundenAnpassen();
            VideoSchnittFormatAnpassen();
            AudioOnlyAnpassen();

            AudioOnlyVideoSchnittWorkaround();
            // Event registrieren
            Loaded += MainWindow_Loaded;
        }
        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/MortysTerminal/MortysDLP-dotNet-"; // Ihre GitHub-Seite
            try
            {
                // Startet den Standardbrowser und öffnet die URL
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Erforderlich, um den Standardbrowser zu verwenden
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Fehler beim Öffnen der URL: {ex.Message}", "Fehler", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tb_URL_GotFocus(object sender, RoutedEventArgs e)
        {
            tb_URL.SelectAll();
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    textBox.SelectAll();
                    textBox.Focus(); // Erneut den Fokus sicherstellen
                });
            }
        }
        private string GetSelectedAudioFormat()
        {
            if (AudioFormatComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
                //System.Windows.MessageBox.Show($"Ausgewähltes Audioformat: {selectedFormat}");
            }
            else { return "mp3"; }
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            

            // ErsterStart wird ausgeführt, sobald das MainWindow vollständig geladen ist
            try
            {
                await YtDlpUpdater.CheckAndUpdateAsync(this);
                await FfmpegUpdater.CheckAndUpdateAsync(this);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Fehler bei der Aktualisierung von yt-dlp: {ex.Message}", "Fehler");
            }
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
        private void cbAudioOnlyCheck(object sender, RoutedEventArgs e)
        {
            AudioOnlyAnpassen();
        }
        private void cbVideoFormatCheck(object sender, RoutedEventArgs e)
        {
            VideoSchnittFormatAnpassen();
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

            if (String.IsNullOrEmpty(Properties.Settings.Default.DOWNLOADPATH))
            {
                tb_downloadpath.Text = defaultdownloadsPath;
            }
            else
            {
                tb_downloadpath.Text = Properties.Settings.Default.DOWNLOADPATH;
            }

            if (Properties.Settings.Default.CHECKED_AUDIO_ONLY.Equals("1"))
            {
                cb_AudioOnly.IsChecked = true;
                SelectAudioFormat(Properties.Settings.Default.SELECTED_AUDIO_FORMAT);
            }

            // Downloadpfad einfuegen in TextBox
            //tb_downloadpath.Text = downloadspath;
        }
        private void SelectAudioFormat(string savedFormat)
        {
            // Angenommener gespeicherter Wert
            //string savedFormat = Properties.Settings.Default.AudioFormat ?? "mp3";

            // Suche nach dem gespeicherten Eintrag und setze ihn als ausgewählt
            // Standardwert setzen, falls das gespeicherte Format nicht gefunden wird
            bool formatFound = false;

            foreach (ComboBoxItem item in AudioFormatComboBox.Items)
            {
                if (item.Content.ToString() == savedFormat)
                {
                    AudioFormatComboBox.SelectedItem = item;
                    formatFound = true;
                    break;
                }
            }

            // Wenn das Format nicht gefunden wurde, "mp3" auswählen
            if (!formatFound)
            {
                foreach (ComboBoxItem item in AudioFormatComboBox.Items)
                {
                    if (item.Content.ToString() == "mp3")
                    {
                        AudioFormatComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
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
            string cb_AudioOnlyChecked = "0";
            string tb_AudioOnly_Text = "";
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

            if (cb_AudioOnly.IsChecked == true)
            {
                cb_AudioOnlyChecked = "1";
                tb_AudioOnly_Text = GetSelectedAudioFormat();
            }

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
                Properties.Settings.Default.CHECKED_AUDIO_ONLY = cb_AudioOnlyChecked;
                Properties.Settings.Default.SELECTED_AUDIO_FORMAT = tb_AudioOnly_Text;
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
                Properties.Settings.Default.CHECKED_VIDEOFORMAT = "1";
                Properties.Settings.Default.DOWNLOADPATH = "";
                Properties.Settings.Default.CHECKED_AUDIO_ONLY = "0";
                Properties.Settings.Default.SELECTED_AUDIO_FORMAT = "mp3";
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
        private void AudioOnlyAnpassen()
        {
            if (cb_AudioOnly.IsChecked == true)
            {
                txt_AudioOnly_info.Foreground = Brushes.Black;
                txt_AudioOnly_info.IsEnabled = true;
                AudioFormatComboBox.IsReadOnly = false;
                AudioFormatComboBox.IsEnabled = true;

                cb_Videoformat.IsEnabled = false;
            }
            else
            {
                txt_AudioOnly_info.Foreground = Brushes.Silver;
                txt_AudioOnly_info.IsEnabled = false;
                AudioFormatComboBox.IsReadOnly = true;
                AudioFormatComboBox.IsEnabled = false;

                cb_Videoformat.IsEnabled = true;
            }
        }
        private void VideoSchnittFormatAnpassen()
        {
            if (cb_Videoformat.IsChecked == true)
            {
                txt_Videoformat_info.Foreground = Brushes.Black;
                txt_Videoformat_info.IsEnabled = true;

                cb_AudioOnly.IsEnabled = false;
            }
            else
            {
                txt_Videoformat_info.Foreground = Brushes.Silver;
                txt_Videoformat_info.IsEnabled = false;

                cb_AudioOnly.IsEnabled = true;
            }
        }
        private void AudioOnlyVideoSchnittWorkaround()
        {
            if (cb_AudioOnly.IsChecked == true && cb_Videoformat.IsChecked == true)
            {
                cb_AudioOnly.IsChecked = false;
                cb_Videoformat.IsChecked = false;
            }
        }
        private string BaueYTDLPArgumente()
        {
            string selectedAudioFormatComboBox = GetSelectedAudioFormat();

            // DOWNLOAD FIRST 15 SEC
            //yt-dlp -f "bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b" -o "%(title)s.%(ext)s" --downloader ffmpeg --downloader-args "ffmpeg:-t 10"  ""
            // yt-dlp -f "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best" "https://youtu.be/pvm6avc8Zwo?si=MHQrnVOevYhbO1zW" -S vcodec:h264

            string ba_Args = "-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\"";

            //URL
            ba_Args += " \"" + tb_URL.Text + "\"";
            

            if (cb_Zeitspanne.IsChecked == true)
            {
                // --download-sections "*00:32:53.2-00:32:55.5"
                ba_Args += " -o \"" + tb_downloadpath.Text + "\\z_%(title)s.%(ext)s\"";
                ba_Args += " --download-sections \"*" + tb_zeitspanne_von.Text + "-" + tb_zeitspanne_bis.Text + "\"";
            }
            if(cb_ErsteSekunden.IsChecked == true)
            {
                ba_Args += " -o \"" + tb_downloadpath.Text + "\\" + tb_ErsteSekunden_Sekunden.Text + "_%(title)s.%(ext)s\"";
                ba_Args += " --downloader \""+ FfmpegPath + "\" --downloader-args \"ffmpeg:-t " + tb_ErsteSekunden_Sekunden.Text + "\"";
            }
            // TODO MP3 ONLY
            // arg " -x --audio-format mp3 --audio-quality 0"
            if (cb_AudioOnly.IsChecked == true){
                ba_Args += $" -x --audio-format {selectedAudioFormatComboBox} --audio-quality 0";
            }
            else
            {
                if (cb_Videoformat.IsChecked == true)
                {
                    //ba_Args += " -S vcodec: h264";
                    ba_Args += " -S vcodec:h264";
                }
            }
                
            // Downloadpfad hinzufuegen
            ba_Args += " -o \"" + tb_downloadpath.Text + "\\%(title)s.%(ext)s\"";
            


            // Skip Cert
            ba_Args += " --no-check-certificates";

            // Heutige Zeit uebernehmen
            ba_Args += " --no-mtime";

            

            return ba_Args;

        }
        private void DownloadStarten_Click(object sender, RoutedEventArgs e)
        {
            // TODO PFADE EINBINDEN von yt-dlp.exe und ffmpeg.exe
            // Selbststaendiges Auslesen der pfade!

            string args = BaueYTDLPArgumente();

            
            RunYtDlpAsync(args);

            //System.Diagnostics.Process process1 = new System.Diagnostics.Process();
            //process1.StartInfo.FileName = YtDlpPath;
            //process1.StartInfo.Arguments = args;
            //process1.Start();
            //process1.WaitForExit();
            //process1.Close();
        }
        private void RunYtDlpAsync(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = YtDlpPath, // Pfad zur yt-dlp.exe
                    Arguments = arguments, // CLI-Argumente
                    RedirectStandardOutput = true, // Standardausgabe umleiten
                    RedirectStandardError = true, // Fehlerausgabe umleiten
                    UseShellExecute = false, // Shell nicht verwenden
                    CreateNoWindow = true // Kein separates Fenster öffnen
                }
            };

            // Ereignis für die Datenempfang der Standardausgabe
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    AppendOutput(e.Data);
                }
            };

            // Ereignis für die Datenempfang der Fehlerausgabe
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    AppendOutput($"[ERROR] {e.Data}");
                }
            };

            try
            {
                process.Start(); // Prozess starten

                // Asynchrone Ausgabe lesen
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExitAsync(); // Auf Prozessende warten
                AppendOutput($"Prozess beendet mit Code: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                AppendOutput($"[EXCEPTION] {ex.Message}");
            }
        }
        private void AppendOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText($"{text}{Environment.NewLine}");
                OutputTextBox.ScrollToEnd(); // Automatisch nach unten scrollen
            });
        }


    }
}