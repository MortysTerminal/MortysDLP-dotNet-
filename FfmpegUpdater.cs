using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using MortysDLP_dotNet_;
using MortysDLP_dotNet_.Properties;

public class FfmpegUpdater
{
    private static readonly string FfmpegReleasesUrl   = Settings.Default.FFMPEG_RELEASE_URL;
    private static readonly string FfmpegJsonPath      = Settings.Default.FFMPEG_JSON_PATH;
    private static readonly string FfmpegPath          = Settings.Default.FFMPEG_PATH;
    private static readonly string FfprobePath         = Settings.Default.FFPROBE_PATH;
    private static readonly string TempDownloadPath    = Settings.Default.TEMP_FFMPEG_DOWNLOAD_PATH;

    /// <summary>
    /// Überprüft und aktualisiert ffmpeg.exe und ffprobe.exe, falls erforderlich.
    /// </summary>
    /// <param name="owner">Das Hauptfenster für den Dialog.</param>
    public static async Task CheckAndUpdateAsync(Window owner)
    {
        try
        {
            var ffmpegInfo = LoadFfmpegInfo();

            // Überprüfen, ob ein Update erforderlich ist
            if (IsUpdateRequired(ffmpegInfo.LastChecked))
            {
                var (latestVersion, downloadUrl) = await GetLatestFfmpegInfoAsync();

                if (ffmpegInfo.Version != latestVersion)
                {
                    string message = $"Eine neue Version von FFMPEG ist verfügbar: {latestVersion}\n" +
                                     "Möchten Sie die neue Version herunterladen und installieren?";

                    var dialog = new UpdateDialog(message) { Owner = owner };
                    bool? result = dialog.ShowDialog();

                    if (result == true)
                    {
                        // Download und Entpacken der neuen Version
                        await DownloadFileAsync(downloadUrl, TempDownloadPath);
                        ExtractFfmpegFiles(TempDownloadPath);

                        // JSON aktualisieren
                        SaveFfmpegInfo(new FfmpegInfo
                        {
                            Version = latestVersion,
                            LastChecked = DateTime.UtcNow
                        });

                        MessageBox.Show("Die neue Version von FFMPEG wurde erfolgreich heruntergeladen und installiert.", "Erfolg");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler bei der Aktualisierung von FFMPEG: {ex.Message}", "Fehler");
        }
    }

    /// <summary>
    /// Lädt die Informationen zur aktuellen FFMPEG-Version aus der JSON-Datei.
    /// </summary>
    private static FfmpegInfo LoadFfmpegInfo()
    {
        if (!File.Exists(FfmpegJsonPath))
        {
            return new FfmpegInfo { Version = "0.0.0", LastChecked = DateTime.MinValue };
        }

        string json = File.ReadAllText(FfmpegJsonPath);
        return JsonSerializer.Deserialize<FfmpegInfo>(json) ?? new FfmpegInfo { Version = "0.0.0", LastChecked = DateTime.MinValue };
    }

    /// <summary>
    /// Speichert die aktuelle FFMPEG-Version in der JSON-Datei.
    /// </summary>
    private static void SaveFfmpegInfo(FfmpegInfo info)
    {
        string json = JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
        string directory = Path.GetDirectoryName(FfmpegJsonPath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(FfmpegJsonPath, json);
    }

    /// <summary>
    /// Prüft, ob seit dem letzten Update mehr als 14 Tage vergangen sind.
    /// </summary>
    private static bool IsUpdateRequired(DateTime lastChecked)
    {
        return (DateTime.UtcNow - lastChecked).TotalDays > 14;
    }

    /// <summary>
    /// Holt die neueste Version von GitHub und die zugehörige Download-URL.
    /// </summary>
    private static async Task<(string version, string downloadUrl)> GetLatestFfmpegInfoAsync()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(FfmpegReleasesUrl);

        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
        htmlDoc.LoadHtml(response);

        // Versionsnummer extrahieren (aus dem Title-Tag)
        var versionNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
        if (versionNode == null) throw new Exception("Versionsinformation konnte nicht extrahiert werden.");

        // "Release ffmpeg 2024.12.23 · GyanD/codexffmpeg · GitHub"
        string[] titleParts = versionNode.InnerText.Split(' ');
        string version = titleParts[2].Trim();

        // Dynamischen Download-Link erstellen
        string downloadUrl = $"https://github.com/GyanD/codexffmpeg/releases/download/{version}/ffmpeg-{version}-essentials_build.zip";

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

    /// <summary>
    /// Entpackt die heruntergeladene FFMPEG-ZIP-Datei und kopiert die benötigten Dateien.
    /// </summary>
    private static void ExtractFfmpegFiles(string zipPath)
    {
        string tempExtractPath = Path.Combine(Path.GetTempPath(), "ffmpeg_extract");
        if (Directory.Exists(tempExtractPath))
        {
            Directory.Delete(tempExtractPath, true);
        }

        // ZIP-Datei entpacken
        ZipFile.ExtractToDirectory(zipPath, tempExtractPath);

        // Rekursiv nach den gewünschten Dateien suchen
        string ffmpegSourcePath = Directory.GetFiles(tempExtractPath, "ffmpeg.exe", SearchOption.AllDirectories).FirstOrDefault();
        string ffprobeSourcePath = Directory.GetFiles(tempExtractPath, "ffprobe.exe", SearchOption.AllDirectories).FirstOrDefault();

        if (string.IsNullOrEmpty(ffmpegSourcePath) || string.IsNullOrEmpty(ffprobeSourcePath))
        {
            throw new Exception("Die benötigten Dateien 'ffmpeg.exe' oder 'ffprobe.exe' wurden im entpackten Archiv nicht gefunden.");
        }

        // Zielpfad sicherstellen
        string ffmpegTargetDir = Path.GetDirectoryName(FfmpegPath);
        if (!Directory.Exists(ffmpegTargetDir))
        {
            Directory.CreateDirectory(ffmpegTargetDir);
        }

        // Dateien kopieren
        File.Copy(ffmpegSourcePath, FfmpegPath, true);
        File.Copy(ffprobeSourcePath, FfprobePath, true);

        // Temporäres Verzeichnis und ZIP löschen
        Directory.Delete(tempExtractPath, true);
        File.Delete(zipPath);
    }


    public class FfmpegInfo
    {
        public string Version { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
