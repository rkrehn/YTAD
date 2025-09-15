using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace YTDownload
{
    public class YouTubeVideoInfo
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public int LengthSeconds { get; set; }
        public List<StreamInfo> Streams { get; set; } = new();
    }

    public class StreamInfo
    {
        public int Itag { get; set; }
        public string MimeType { get; set; }
        public string Quality { get; set; }
        public string Url { get; set; }
        public bool HasVideo { get; set; }
        public bool HasAudio { get; set; }
        public long? ContentLength { get; set; }
    }

    public class YouTubeExtractor : IDisposable
    {
        private readonly HttpClient _httpClient;

        public YouTubeExtractor()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public YouTubeVideoInfo ExtractFromHtml(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("HTML content cannot be null or empty.", nameof(htmlContent));
            }

            // Example implementation for extracting video information from HTML content
            var videoInfo = new YouTubeVideoInfo
            {
                VideoId = ExtractVideoId(htmlContent),
                Title = ExtractTitle(htmlContent),
                LengthSeconds = ExtractLengthSeconds(htmlContent),
                Streams = ExtractStreams(htmlContent)
            };

            return videoInfo;
        }

        private string ExtractVideoId(string htmlContent)
        {
            // Placeholder logic for extracting video ID
            var match = Regex.Match(htmlContent, @"\""videoId\"":\""(?<id>[\w-]+)\""");
            return match.Success ? match.Groups["id"].Value : throw new InvalidOperationException("Video ID not found.");
        }

        private string ExtractTitle(string htmlContent)
        {
            // Placeholder logic for extracting title
            var match = Regex.Match(htmlContent, @"\""title\"":\""(?<title>[^\""]+)\""");
            return match.Success ? HttpUtility.HtmlDecode(match.Groups["title"].Value) : "Unknown Title";
        }

        private int ExtractLengthSeconds(string htmlContent)
        {
            // Placeholder logic for extracting video length
            var match = Regex.Match(htmlContent, @"\""lengthSeconds\"":\""(?<length>\d+)\""");
            return match.Success ? int.Parse(match.Groups["length"].Value) : 0;
        }

        private List<StreamInfo> ExtractStreams(string htmlContent)
        {
            // Placeholder logic for extracting streams
            var streams = new List<StreamInfo>();
            var matches = Regex.Matches(htmlContent, @"\""itag\"":(?<itag>\d+),\""url\"":\""(?<url>[^\""]+)\""");

            foreach (Match match in matches)
            {
                streams.Add(new StreamInfo
                {
                    Itag = int.Parse(match.Groups["itag"].Value),
                    Url = HttpUtility.UrlDecode(match.Groups["url"].Value),
                    MimeType = "video/mp4", // Example MIME type
                    Quality = "hd720", // Example quality
                    HasVideo = true,
                    HasAudio = true
                });
            }

            return streams;
        }
    }

    // Example usage
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // WARNING: This is for educational purposes only!
            // Downloading YouTube content may violate their Terms of Service

            var htmlContent = File.ReadAllText("youtube_page.html"); // Your HTML file

            using var extractor = new YouTubeExtractor();

            try
            {
                var videoInfo = extractor.ExtractFromHtml(htmlContent);

                Console.WriteLine($"Video: {videoInfo.Title}");
                Console.WriteLine($"Duration: {videoInfo.LengthSeconds} seconds");
                Console.WriteLine($"Available streams: {videoInfo.Streams.Count}");

                foreach (var stream in videoInfo.Streams)
                {
                    Console.WriteLine($"  Itag: {stream.Itag}, Quality: {stream.Quality}, " +
                                    $"Type: {stream.MimeType}, Video: {stream.HasVideo}, Audio: {stream.HasAudio}");

                    // Only download if URL is available (not cipher-protected)
                    if (!string.IsNullOrEmpty(stream.Url))
                    {
                        var extension = GetFileExtension(stream.MimeType);
                        var fileName = $"{videoInfo.VideoId}_{stream.Itag}.{extension}";

                        // Uncomment to actually download (be careful!)
                        // await extractor.DownloadStreamAsync(stream, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static string GetFileExtension(string mimeType)
        {
            return mimeType.ToLower() switch
            {
                var mime when mime.Contains("mp4") => "mp4",
                var mime when mime.Contains("webm") => "webm",
                var mime when mime.Contains("audio/mp4") => "m4a",
                var mime when mime.Contains("audio/webm") => "webm",
                _ => "bin"
            };
        }
    }
}