using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using MortysDLP_dotNet_;
using MortysDLP_dotNet_.Properties;

public static class YtDlpUpdater
{
    private static readonly string YtDlpReleasesUrl = Settings.Default.YT_DLP_RELEASE_URL;
    //private static readonly string YtDlpFilePath = Settings.Default.YT_DLP_PATH;

    /// <summary>
    /// Überprüft, ob eine neue Version von yt-dlp verfügbar ist, und bietet ein Update an.
    /// </summary>
    /// <param name="owner">Das Hauptfenster für den Dialog.</param>
    public static async Task CheckAndUpdateAsync(Window owner)
    {
        try
        {
            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "yt-dlp", "yt-dlp.exe");

            // Prüfen, ob die Datei existiert
            if (!File.Exists(ytDlpPath))
            {
                string message = "Die Datei 'yt-dlp.exe' wurde nicht gefunden.\n" +
                                 "Die Software benötigt diese Datei, um ordnungsgemäß zu funktionieren.\n" +
                                 "Möchten Sie die Datei jetzt herunterladen?";

                var dialog = new UpdateDialog(message) { Owner = owner };
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    // Neueste Version und Download-URL abrufen
                    var (latestVersion, downloadUrl) = await GetLatestYtDlpInfoAsync();

                    // Datei herunterladen
                    await DownloadFileAsync(downloadUrl, ytDlpPath);
                    MessageBox.Show($"Die Datei 'yt-dlp.exe' wurde erfolgreich heruntergeladen (Version: {latestVersion}).", "Erfolg");
                }
                else
                {
                    throw new FileNotFoundException("Die Datei 'yt-dlp.exe' wird benötigt. Die Software kann nicht gestartet werden.");
                }
            }
            else
            {
                // Lokale Version abrufen
                string localVersion = GetLocalYtDlpVersion();

                // Neueste Version von GitHub abrufen
                var (latestVersion, downloadUrl) = await GetLatestYtDlpInfoAsync();

                // Prüfen, ob ein Update erforderlich ist
                if (string.Compare(localVersion, latestVersion) < 0) // Lokale Version ist älter
                {
                    string message = $"Eine neue Version von yt-dlp ist verfügbar: {latestVersion}\n" +
                                     $"Ihre aktuelle Version: {localVersion}\n" +
                                     "Möchten Sie die neue Version herunterladen und installieren?";

                    var dialog = new UpdateDialog(message) { Owner = owner };
                    bool? result = dialog.ShowDialog();

                    if (result == true)
                    {
                        await DownloadFileAsync(downloadUrl, ytDlpPath);
                        MessageBox.Show("Die neue Version wurde erfolgreich heruntergeladen und installiert.", "Erfolg");
                    }
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show(ex.Message, "Fehler");
            Application.Current.Shutdown(); // Anwendung beenden, wenn die Datei benötigt wird
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler bei der Aktualisierung von yt-dlp: {ex.Message}", "Fehler");
        }
    }


    /// <summary>
    /// Holt die lokale Version von yt-dlp.exe.
    /// </summary>
    private static string GetLocalYtDlpVersion()
    {
        // Absoluter Pfad zur yt-dlp.exe
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "yt-dlp", "yt-dlp.exe");

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Die Datei '{fullPath}' wurde nicht gefunden.");
        }

        var versionInfo = FileVersionInfo.GetVersionInfo(fullPath);
        return versionInfo.FileVersion ?? "0.0.0";
    }


    /// <summary>
    /// Holt die neueste Version von GitHub und die zugehörige Download-URL.
    /// </summary>
    private static async Task<(string version, string downloadUrl)> GetLatestYtDlpInfoAsync()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(YtDlpReleasesUrl);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

        // Versionsnummer extrahieren (aus dem Title-Tag)
        var versionNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
        if (versionNode == null) throw new Exception("Versionsinformation konnte nicht extrahiert werden.");

        // "Release yt-dlp 2024.12.23 · yt-dlp/yt-dlp · GitHub" -> Extrahiere "2024.12.23"
        string[] titleParts = versionNode.InnerText.Split(' ');
        string version = titleParts[2].Trim(); // Dritter Teil ist das Datum

        // Download-URL basierend auf der extrahierten Version zusammenbauen
        string downloadUrl = $"https://github.com/yt-dlp/yt-dlp/releases/download/{version}/yt-dlp.exe";

        return (version, downloadUrl);
    }

    /// <summary>
    /// Lädt die Datei von der angegebenen URL herunter.
    /// </summary>
    private static async Task DownloadFileAsync(string url, string outputPath)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string directory = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fileStream);
    }
}
