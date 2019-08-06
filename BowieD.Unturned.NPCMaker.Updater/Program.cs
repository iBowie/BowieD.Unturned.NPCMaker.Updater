using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace BowieD.Unturned.NPCMaker.Updater
{
    class Program
    {
        public static UpdateManifest? GetUpdateManifest()
        {
            try
            {
                string content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "update.manifest");
                return JsonConvert.DeserializeObject<UpdateManifest>(content);
            }
            catch { return null; }
        }
        static void Main(string[] args)
        {
            if (args.Length < 1 || !File.Exists(args[0]))
            {
                WriteLine($"ILLEGAL ARGUMENTS. CLOSING...", ConsoleColor.Red);
                Thread.Sleep(3000);
                return;
            }
            for (int k = 0; k < 10; k++)
            {
                WriteLine($"Try #{k}", ConsoleColor.White);
                try
                {
                    WriteLine($"Getting manifest...", ConsoleColor.White);
                    var manifest = GetUpdateManifest();
                    if (manifest == null)
                        WriteLine($"FAILED: Could not get anything.", ConsoleColor.Red);
                    else if (manifest?.tag_name == null)
                        WriteLine($"FAILED: Could not get version info.", ConsoleColor.Red);
                    else if (manifest?.assets[0].browser_download_url == null)
                        WriteLine($"FAILED: Could not get download URL.", ConsoleColor.Red);
                    else
                    {
                        WriteLine($"SUCCESS: Version info and download URL obtained.", ConsoleColor.Green);
                        if (File.Exists(args[0]))
                        {
                            WriteLine($"Deleting old version...", ConsoleColor.White);
                            File.Delete(args[0]);
                            WriteLine($"Deleted.", ConsoleColor.White);
                        }
                        WriteLine($"Downloading new version...", ConsoleColor.White);
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(manifest?.assets[0].browser_download_url, args[0]);
                        }
                        WriteLine($"Downloaded!", ConsoleColor.White);
                        WriteLine($"Launching in 3...", ConsoleColor.White);
                        Thread.Sleep(1000);
                        WriteLine($"Launching in 2...", ConsoleColor.White);
                        Thread.Sleep(1000);
                        WriteLine($"Launching in 1...", ConsoleColor.White);
                        Thread.Sleep(1000);
                        System.Diagnostics.Process.Start(args[0]);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    WriteLine($"FAILED: Exception: {ex.Message}", ConsoleColor.Red);
                }
                WriteLine("Trying again in 1 second...", ConsoleColor.White);
                Thread.Sleep(1000);
            }
            WriteLine($"Update failed. Try again later.", ConsoleColor.Red);
            WriteLine($"Press any key to close.", ConsoleColor.White);
            Console.ReadKey(true);
        }
        static void WriteLine(object text, ConsoleColor color)
        {
            var oldClr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldClr;
        }
    }
    public struct UpdateManifest
    {
        public string name;
        public string tag_name;
        public string body;
        public asset[] assets;
        public class asset
        {
            public string browser_download_url;
        }
    }
}