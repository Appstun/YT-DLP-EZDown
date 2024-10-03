using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YT_DLP_EZDown
{
    internal class FileManager
    {
        class ReleaseInfo
        {
            public Asset[] assets { get; set; }
        }
        class Asset
        {
            public string browser_download_url { get; set; }
            public string name { get; set; }
        }

        public static void checkDirectory()
        {
            if (!Directory.Exists(Program.outDir)) Directory.CreateDirectory(Program.outDir);
            if (!Directory.Exists(Program.downDir)) Directory.CreateDirectory(Program.downDir);
        }
        public static void checkDownloader()
        {
            GUI.writeColoredLine("§eChecke yt-dlp.exe .....");
            if (!File.Exists(Program.ytDlpPath))
            {
                GUI.writeColoredLine("§cyt-dlp.exe nicht gefunden!");
                GUI.writeColoredLine("§e Downloade...");

                DownloadLatestReleaseExe("https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest", Program.mainDir);

                GUI.writeColoredLine("§2Download für yt-dlp.exe abgeschlossen!");
            }
            else GUI.writeColoredLine("§3yt-dlp.exe gefunden! Kein Download erforderlich.");

            Console.WriteLine("");

            GUI.writeColoredLine("§eChecke ffmpeg.exe .....");
            if (!File.Exists(Program.ffmpegPath))
            {
                if (!File.Exists(Program.outDirFfmpegPath))
                {
                    GUI.writeColoredLine("§cffmpeg.exe nicht gefunden!");
                    GUI.writeColoredLine("§e Downloade...");

                    DownloadLatestReleaseExe("https://api.github.com/repos/BtbN/FFmpeg-Builds/releases/latest", Program.mainDir);

                    GUI.writeColoredLine("§e Zip -Datei wird entpackt ...");
                    ZipFile.ExtractToDirectory(Program.ffmpegZipPath, Program.downDir);

                    GUI.writeColoredLine("§e Verschiebe ffmpeg.exe ...");
                    File.Move(@$"{Program.downDir}\{Program.ffmpegUnzipName}\bin\ffmpeg.exe", Program.ffmpegPath);

                    GUI.writeColoredLine("§e Lösche unötige Dateien ...");
                    File.Delete(Program.ffmpegZipPath);
                    Directory.Delete(@$"{Program.downDir}\{Program.ffmpegUnzipName}", true);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    GUI.writeColoredLine("§2Download für ffmpeg.exe abgeschlossen!");
                }
                else
                {
                    GUI.writeColoredLine("§3 ffmpeg.exe gefunden! Aber nicht da wo die Datei sein sollte. Wird verschoben ...");
                    File.Move(Program.outDirFfmpegPath, Program.ffmpegPath);

                    GUI.writeColoredLine("§2Verschieben der ffmpeg.exe abgeschlossen!");
                }
            }
            else GUI.writeColoredLine("§3ffmpeg.exe gefunden! Kein Download erforderlich.");

            Console.WriteLine("");
            GUI.writeColoredLine("§aEs wird 3 Sekunden abgewartet...");
            Thread.Sleep(3000);

            Console.Clear();
        }

        static void DownloadLatestReleaseExe(string downl, string saveDirectory)
        {
            string releaseJson = DownloadString(downl);
            ReleaseInfo releaseData = JsonSerializer.Deserialize<ReleaseInfo>(releaseJson);

            string downloadUrl = null;
            string fileName = null;

            foreach (var asset in releaseData.assets)
            {
                if (asset.name.EndsWith(".exe") || asset.name.ToLower() == Program.ffmpegZipGit.ToLower())
                {
                    downloadUrl = asset.browser_download_url;
                    fileName = asset.name;
                    break;
                }
            }

            //if (downloadUrl == null) sendError("yt-dlp.exe könnte nicht von Github gedownloadet werden!");

            DownloadFile(downloadUrl, fileName.ToLower() == Program.ffmpegZipGit.ToLower() ? Program.ffmpegZipPath : Program.ytDlpPath);
        }

        static string DownloadString(string url)
        {
            GUI.writeColoredLine($"§7 Hole Informationen von '{url}' ...");
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0");
                return client.DownloadString(url);
            }
        }

        static void DownloadFile(string url, string savePath)
        {
            GUI.writeColoredLine($"§7 Downloade '{url}' ...");
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0");
                client.DownloadFile(url, savePath);
            }
        }
    }
}
