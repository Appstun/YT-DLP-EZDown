
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

string[] formatArray = { "MP4", "MKV", "MP3", "WAV" };

string mainDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
string outDir = @$"{mainDir}\output";
string downDir = @$"{mainDir}\packagedownload";
string ytDlpPath = @$"{downDir}\yt-dlp.exe";
string ffmpegZipGit = "ffmpeg-master-latest-win64-gpl.zip";
string ffmpegUnzipName = ffmpegZipGit.Replace(".zip", "");
string ffmpegZipPath = @$"{downDir}\{ffmpegZipGit}";
string ffmpegPath = @$"{downDir}\ffmpeg.exe";
string outDirFfmpegPath = @$"{outDir}\ffmpeg.exe";

/*Console.WriteLine(mainDir);
Console.WriteLine(outDir);
Console.WriteLine(downDir);
Console.WriteLine(ytDlpPath);
Console.WriteLine(ffmpegZipGit);
Console.WriteLine(ffmpegUnzipName);
Console.WriteLine(ffmpegZipPath);
Console.WriteLine(ffmpegPath);
Console.WriteLine(outDirFfmpegPath);*/

checkDirectory();
checkDownloader();

void checkDownloader()
{
    sendHeader("Paket-Downloader");

    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Checke yt-dlp.exe .....");
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    if (!File.Exists(ytDlpPath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("yt-dlp.exe nicht gefunden!");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(" Downloade...");
        Console.ResetColor();

        DownloadLatestReleaseExe("https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest", mainDir);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Download für yt-dlp.exe abgeschlossen!");

        Console.ResetColor();
    }
    else Console.WriteLine("yt-dlp.exe gefunden! Kein Download erforderlich.");
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Checke ffmpeg.exe .....");
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    if (!File.Exists(ffmpegPath))
    {
        if (!File.Exists(outDirFfmpegPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ffmpeg.exe nicht gefunden!");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(" Downloade...");
            Console.ResetColor();
            DownloadLatestReleaseExe("https://api.github.com/repos/BtbN/FFmpeg-Builds/releases/latest", mainDir);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Zip-Datei wird entpackt ...");
            ZipFile.ExtractToDirectory(ffmpegZipPath, downDir);

            Console.WriteLine("Verschiebe ffmpeg.exe ...");
            File.Move(@$"{downDir}\{ffmpegUnzipName}\bin\ffmpeg.exe", ffmpegPath);

            Console.WriteLine("Lösche unötige Dateien ...");
            File.Delete(ffmpegZipPath);
            Directory.Delete(@$"{downDir}\{ffmpegUnzipName}", true);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Download für ffmpeg.exe abgeschlossen!");

            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("ffmpeg.exe gefunden! Aber nicht da wo die Datei sein sollte. Wird verschoben ...");
            File.Move(outDirFfmpegPath, ffmpegPath);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Verschieben der ffmpeg.exe abgeschlossen!");

            Console.ResetColor();
        }
    }
    else Console.WriteLine("ffmpeg.exe gefunden! Kein Download erforderlich.");

    Console.WriteLine("");
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Es wird 3 Sekunden abgewartet...");
    Thread.Sleep(3000);

    Console.Clear();
    mainFunc();
}

void mainFunc()
{
    sendHeader("yt-dlp-Optionen");

    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.WriteLine("Verwendbare Formate:");
    for (int i = 0; i < formatArray.Length; i++)
    {
        Console.Write("     ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(i + 1);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" = ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(formatArray[i]);
        Console.WriteLine("");
    }
    Console.ResetColor();
    Console.WriteLine("");
    Console.Write("Wähle ein Format, indem du eine Zahl eingibts: ");

    Console.ForegroundColor = ConsoleColor.Yellow;
    string formatInputStr = Console.ReadLine();
    Console.ResetColor();
    Console.WriteLine("");

    int formatInput = 0;
    if (checkIsNumber(formatInputStr))
    {
        formatInput = int.Parse(formatInputStr) - 1;
        if (formatInput >= 0 && formatInput < formatArray.Length)
        {
            string formatSel = formatArray[formatInput];

            Console.Write("Nun die URL, wovon gedownloadet werden soll: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string urlInputStr = Console.ReadLine();
            Console.ResetColor();

            if (isURL(urlInputStr))
            {
                File.Move(ffmpegPath, outDirFfmpegPath);

                string option = "--recode-video";
                if (formatInput >= 2) option = "--extract-audio --audio-quality best --audio-format";
                string ytDlp_path = @$"""{ytDlpPath}""";
                string ffmpeg_path = @$"ffmpeg.exe";
                string cmd = @$"{ytDlp_path} --ffmpeg-location {ffmpeg_path} {option} {formatSel.ToLower()} {urlInputStr}";


                Console.Clear();

                sendHeader("Download der Mediendatei");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Der Download beginnt! ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Format: " + formatSel + " | URL: " + urlInputStr);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Befehl: " + cmd);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Outputpfad: " + outDir);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- yt-dlp führt nun den Download aus! ---");
                Console.WriteLine("");
                Console.ResetColor();

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {cmd}";
                process.StartInfo.WorkingDirectory = outDir;
                process.Start();

                process.WaitForExit();
                File.Move(outDirFfmpegPath, ffmpegPath);

                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Programm wurde beendet! ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Drücke eine beliebige Taste zum Beenden und um den Ordner zu öffnen...");

                Console.ReadKey();
                Process.Start("explorer.exe", outDir);
            }
            else sendError($"{urlInputStr} ist keine gültige URL!");
        }
        else sendError("Wähle eine Zahl im Bereich von 1 bis " + formatArray.Length + "!");
    }
    else sendError($"{formatInputStr} ist keine gültige Ganzzahl!");
}


Boolean checkIsNumber(string input)
{
    try
    {
        int.Parse(input);
        return true;
    }
    catch
    { return false; }
}

void sendInfo(string info, ConsoleColor infoColor, string message, ConsoleColor messageColor = ConsoleColor.White)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("[");
    Console.ForegroundColor = infoColor;
    Console.Write(info.ToUpper());
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("] ");
    Console.ResetColor();
    if (messageColor != ConsoleColor.White) Console.ForegroundColor = messageColor;
    Console.Write(message);
}

void sendError(string message)
{
    Console.WriteLine("");
    Console.WriteLine("");

    sendInfo("error", ConsoleColor.Red, message);

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("");
    Console.WriteLine("Drücke nun Enter, um von vorne zu beginnen!");
    Console.ResetColor();
    Console.ReadLine();
    Console.Clear();
    mainFunc();
}

Boolean isURL(string input)
{
    return Regex.IsMatch(input, @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$");
}

void checkDirectory()
{
    if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
    if (!Directory.Exists(downDir)) Directory.CreateDirectory(downDir);
}
void DownloadLatestReleaseExe(string downl, string saveDirectory)
{
    string releaseJson = DownloadString(downl);
    ReleaseInfo releaseData = JsonSerializer.Deserialize<ReleaseInfo>(releaseJson);

    string downloadUrl = null;
    string fileName = null;

    foreach (var asset in releaseData.assets)
    {
        if (asset.name.EndsWith(".exe") || asset.name.ToLower() == ffmpegZipGit.ToLower())
        {
            downloadUrl = asset.browser_download_url;
            fileName = asset.name;
            break;
        }
    }

    if (downloadUrl == null) sendError("yt-dlp.exe könnte nicht von Github gedownloadet werden!");

    DownloadFile(downloadUrl, fileName.ToLower() == ffmpegZipGit.ToLower() ? ffmpegZipPath : ytDlpPath);
}

static string DownloadString(string url)
{
    Console.WriteLine($"Hole Informationen von {url} ...");
    using (var client = new WebClient())
    {
        client.Headers.Add("User-Agent", "Mozilla/5.0");
        return client.DownloadString(url);
    }
}

static void DownloadFile(string url, string savePath)
{
    Console.WriteLine($"Downloade {url} ...");
    using (var client = new WebClient())
    {
        client.Headers.Add("User-Agent", "Mozilla/5.0");
        client.DownloadFile(url, savePath);
    }
}

static void sendHeader(string info)
{
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("YT-DLP-EZDown");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(" (https://github.com/Appstun/YT-DLP-EZDown)");
    Console.WriteLine("");

    Console.BackgroundColor = ConsoleColor.DarkYellow;
    Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine($"    {info}    ");

    Console.WriteLine("");
    Console.WriteLine("");

    Console.ResetColor();
}

class ReleaseInfo
{
    public Asset[] assets { get; set; }
}

class Asset
{
    public string browser_download_url { get; set; }
    public string name { get; set; }
}