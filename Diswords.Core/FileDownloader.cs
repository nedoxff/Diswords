using System;
using System.Net;
using System.Threading;

namespace Diswords.Core
{
    public static class FileDownloader
    {
        public static void Download(string url, string to)
        {
            Console.WriteLine($"Downloading {to} from {url}..");
            var wc = new WebClient();
            var finished = false;

            wc.DownloadProgressChanged += (_, args) =>
                Console.WriteLine(
                    $"{args.BytesReceived}/{args.TotalBytesToReceive} bytes received | {args.ProgressPercentage}%");
            wc.DownloadFileCompleted += (_, _) =>
            {
                Console.WriteLine("Done.");
                finished = true;
            };
            wc.DownloadFileAsync(new Uri(url), to);

            while (!finished) Thread.Sleep(1);
        }
    }
}