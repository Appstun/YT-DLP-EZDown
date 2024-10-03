using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace YT_DLP_EZDown
{
    internal class Program
    {
        public static string[] formatArray = { "MP4", "MKV", "MP3", "WAV" };

        public static string mainDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        public static string outDir = @$"{mainDir}\output";
        public static string downDir = @$"{mainDir}\packagedownload";
        public static string ytDlpPath = @$"{downDir}\yt-dlp.exe";
        public static string ffmpegZipGit = "ffmpeg-master-latest-win64-gpl.zip";
        public static string ffmpegUnzipName = ffmpegZipGit.Replace(".zip", "");
        public static string ffmpegZipPath = @$"{downDir}\{ffmpegZipGit}";
        public static string ffmpegPath = @$"{downDir}\ffmpeg.exe";
        public static string outDirFfmpegPath = @$"{outDir}\ffmpeg.exe";

        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("   YT-DLP EZDown   ");
            Console.ResetColor();
            GUI.writeColoredLine("§8(https://github.com/Appstun/YT-DLP-EZDown)");
            Console.WriteLine("");
            Console.WriteLine("");

            FileManager.checkDirectory();
            FileManager.checkDownloader();

            mainFunc();
        }

        static void mainFunc()
        {
            List<GUI.Interface.Option.SelectOption> list = new();
            for (int i = 0; i < formatArray.Length; i++)
                list.Add(new GUI.Interface.Option.SelectOption(formatArray[i], i));

            var gui = new GUI.Interface(new GUI.Interface.ScreenType.OptionsScreen("", "Wähle ein Format aus", [.. list]),
                new GUI.Interface.ScreenType.TextinputScreen("", "Download-URL", false, "§fFüge nun die URL ein, von dem, was du herunterladen möchtest."));
            gui.showGUI();

            int resultFormat = (int)((GUI.Interface.Option.SelectOption)gui.outputResults[0].outputResult!).value!;
            string resultUrl = (string)(gui.outputResults[1].outputResult ?? "");
            resultUrl = resultUrl.Split(" ")[0].Split("&")[0];

            if (!Functions.isURL(resultUrl))
            {
                new GUI.Interface(new GUI.Interface.ScreenType.InfoScreen("", "§cUngültige URL",
                    $"Die angegebene URL ist ungültig.\n§7Haha, jetzt musst du wieder von vorne anfangen.\n§8({resultUrl})", ConsoleKey.Enter)).showGUI();
                mainFunc();
                return;
            }

            File.Move(ffmpegPath, outDirFfmpegPath);

            string option = "--recode-video";
            if (resultFormat >= 2) option = "--extract-audio --audio-quality best --audio-format";
            string ytDlp_path = @$"""{ytDlpPath}""";
            string ffmpeg_path = @$"ffmpeg.exe";
            string cmd = $"{ytDlp_path} --ffmpeg-location {ffmpeg_path} {option} {formatArray[resultFormat].ToLower()} {resultUrl}";

            Console.Clear();
            GUI.writeColoredLine($"§aDer Download beginnt! §3Format: {formatArray[resultFormat]} | URL: {resultUrl}");
            GUI.writeColoredLine("§8Befehl: " + cmd);
            GUI.writeColoredLine("§9Outputpfad: " + outDir);
            GUI.writeColoredLine("§e--- yt-dlp führt nun den Download aus! ---");
            Console.WriteLine("");
            Console.ResetColor();

            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    WorkingDirectory = outDir
                }
            };
            process.Start();
            process.WaitForExit();

            File.Move(outDirFfmpegPath, ffmpegPath);

            Console.WriteLine("");
            GUI.writeColoredLine("§eProgramm wurde beendet! ");
            GUI.writeColoredLine("§7Drücke eine beliebige Taste zum Beenden und um den Ordner zu öffnen...");

            Console.ReadKey();
            Process.Start("explorer.exe", outDir);

        }
    }
}