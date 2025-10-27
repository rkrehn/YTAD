using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YTPD.Properties
{

    public partial class frmArtist : Form
    {
        private string url;
        private string albumData = "";
        public frmArtist(string artisturl)
        {
            InitializeComponent();
            url = artisturl;
        }

        private void frmArtist_Load(object sender, EventArgs e)
        {
            lblArtist.Text = "Loading albums from artist...";
            GetArtistInfo(url);
        }
        private async void GetArtistInfo(string url)
        {
            if (url.Length == 0) return;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0");

            // Explicitly handle encoding
            var response = await client.GetAsync(url);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var html = Encoding.UTF8.GetString(bytes);

            var parser = new YouTubeMusicParser();
            var albums = parser.ExtractAlbumData(html);

            foreach (var album in albums)
            {
                lblArtist.Text = album.ArtistName;
                dataDisco.Rows.Add(1, album.AlbumName, album.PlaylistUrl);
            }

            dataDisco.Sort(dataDisco.Columns[1], ListSortDirection.Ascending);  
        }

        private void btn_Process_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataDisco.Rows)
            {
                if (row.Cells[0].Value == null) continue;

                if (row.Cells[0].Value.ToString() == "1")
                {
                    albumData += row.Cells[2].Value.ToString() + ";";
                }
            }

            // save album data
            Properties.Settings.Default.AlbumData = albumData.Substring(0, albumData.Length - 1);
            Properties.Settings.Default.Save();

            this.Visible = false;
            this.Close();
        }

        private void frmArtist_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(albumData.Length == 0)
            {
                Properties.Settings.Default.AlbumData = "END";
                Properties.Settings.Default.Save();
            }
        }
    }
    public class AlbumInfo
    {
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public int SongCount { get; set; }
        public string PlaylistUrl { get; set; }
    }
    public class YouTubeMusicParser
    {
        public List<AlbumInfo> ExtractAlbumData(string html)
        {
            var albums = new List<AlbumInfo>();

            // First, extract the artist name from the page
            string artistName = ExtractArtistNameFromHtml(html);

            // Try multiple regex patterns to catch different YouTube formatting variations
            var patterns = new[]
            {
            @"initialData\.push\([^)]+,\s*data:\s*['""]([^'""]+)['""]",          // Original flexible version
            @"initialData\.push\({[^}]+},\s*data:\s*['""]([^'""]+)['""]",        // Your original but with flexible quotes/spacing
            @"data:\s*['""]([^'""]*\\x[^'""]+)['""]",                            // Look for any data field with hex escapes
            @"path:\s*['""][^'""]*['""],\s*params:[^,]*,\s*data:\s*['""]([^'""]+)['""]" // More specific YouTube structure
        };

            var allMatches = new List<Match>();

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(html, pattern);
                foreach (Match match in matches)
                {
                    allMatches.Add(match);
                }
            }

            Console.WriteLine($"Found {allMatches.Count} potential JSON matches");

            if (allMatches.Count == 0)
            {
                // Diagnostic: Show what we're actually finding
                var diagnosticPattern = @"initialData\.push\([^)]+\)";
                var diagnosticMatches = Regex.Matches(html, diagnosticPattern);
                Console.WriteLine($"Found {diagnosticMatches.Count} initialData.push calls total");

                if (diagnosticMatches.Count > 0)
                {
                    Console.WriteLine("First few calls look like:");
                    for (int i = 0; i < Math.Min(3, diagnosticMatches.Count); i++)
                    {
                        var sample = diagnosticMatches[i].Value;
                        if (sample.Length > 200) sample = sample.Substring(0, 200) + "...";
                        Console.WriteLine($"  {sample}");
                    }
                }
            }

            foreach (var match in allMatches)
            {
                try
                {
                    var jsonData = DecodeYouTubeJson(match.Groups[1].Value);
                    if (jsonData != null)
                    {
                        var extractedAlbums = ExtractAlbumsFromJson(jsonData, artistName);
                        albums.AddRange(extractedAlbums);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON block: {ex.Message}");
                }
            }

            return albums.DistinctBy(a => a.AlbumName).ToList();
        }

        private string ExtractArtistNameFromHtml(string html)
        {
            // Try multiple extraction methods in order of reliability

            // Method 1: From meta tags
            var metaTitleMatch = Regex.Match(html, @"<meta property=""og:title"" content=""([^""]+)""");
            if (metaTitleMatch.Success)
            {
                return  metaTitleMatch.Groups[1].Value;
            }

            // Method 2: From page title
            var titleMatch = Regex.Match(html, @"<title>([^<]+)</title>");
            if (titleMatch.Success)
            {
                return titleMatch.Groups[1].Value;
            }

            // Method 3: From canonical URL (extract channel name)
            var canonicalMatch = Regex.Match(html, @"<link rel=""canonical"" href=""[^""]*channel/([^""]+)""");
            if (canonicalMatch.Success)
            {
                return "Unknown Artist"; // Channel ID doesn't give us the name
            }

            return "Unknown Artist";
        }

        private JObject DecodeYouTubeJson(string escapedJson)
        {
            try
            {
                // YouTube uses multiple levels of escaping
                var step1 = System.Text.RegularExpressions.Regex.Unescape(escapedJson);

                // Handle hex escapes
                var hexPattern = @"\\x([0-9a-fA-F]{2})";
                var step2 = Regex.Replace(step1, hexPattern, match =>
                {
                    var hex = match.Groups[1].Value;
                    var charCode = Convert.ToInt32(hex, 16);
                    return ((char)charCode).ToString();
                });

                return JObject.Parse(step2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to decode JSON: {ex.Message}");
                return null;
            }
        }

        private List<AlbumInfo> ExtractAlbumsFromJson(JObject jsonData, string artistName)
        {
            var albums = new List<AlbumInfo>();

            try
            {
                // Navigate through the various possible content structures
                var tabs = jsonData["contents"]?["singleColumnBrowseResultsRenderer"]?["tabs"];
                if (tabs == null) return albums;

                foreach (var tab in tabs)
                {
                    var sections = tab["tabRenderer"]?["content"]?["sectionListRenderer"]?["contents"];
                    if (sections == null) continue;

                    foreach (var section in sections)
                    {
                        // Check for album carousels
                        albums.AddRange(ExtractFromCarousel(section, artistName));

                        // Check for album shelves
                        albums.AddRange(ExtractFromShelf(section, artistName));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting albums from JSON: {ex.Message}");
            }

            return albums;
        }

        private List<AlbumInfo> ExtractFromCarousel(JToken section, string artistName)
        {
            var albums = new List<AlbumInfo>();

            var carousel = section["musicCarouselShelfRenderer"];
            if (carousel == null) return albums;

            // Check if this is actually an albums section
            var headerText = carousel["header"]?["musicCarouselShelfBasicHeaderRenderer"]?["title"]?["runs"]?[0]?["text"]?.ToString()?.ToLower();
            if (headerText != null && (headerText.Contains("album") || headerText.Contains("release")))
            {
                var contents = carousel["contents"];
                if (contents != null)
                {
                    foreach (var item in contents)
                    {
                        var albumInfo = ExtractAlbumFromTwoRowItem(item["musicTwoRowItemRenderer"], artistName);
                        if (albumInfo != null)
                        {
                            albums.Add(albumInfo);
                        }
                    }
                }
            }

            return albums;
        }

        private List<AlbumInfo> ExtractFromShelf(JToken section, string artistName)
        {
            var albums = new List<AlbumInfo>();

            var shelf = section["musicShelfRenderer"];
            if (shelf == null) return albums;

            var contents = shelf["contents"];
            if (contents == null) return albums;

            foreach (var item in contents)
            {
                var songRenderer = item["musicResponsiveListItemRenderer"];
                if (songRenderer != null)
                {
                    var albumFromSong = ExtractAlbumFromSongItem(songRenderer, artistName);
                    if (albumFromSong != null)
                    {
                        albums.Add(albumFromSong);
                    }
                }
            }

            return albums;
        }

        private AlbumInfo ExtractAlbumFromTwoRowItem(JToken itemRenderer, string artistName)
        {
            if (itemRenderer == null) return null;

            try
            {
                var title = itemRenderer["title"]?["runs"]?[0]?["text"]?.ToString();
                var navigationEndpoint = itemRenderer["navigationEndpoint"]?["browseEndpoint"];
                var browseId = navigationEndpoint?["browseId"]?.ToString();

                // Fix for the issue with the "params" keyword
                var parameters = navigationEndpoint?["params"]?.ToString();

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(browseId))
                {
                    var playlistId = ExtractPlaylistIdFromParams(parameters);

                    return new AlbumInfo
                    {
                        ArtistName = artistName,
                        AlbumName = title,
                        SongCount = 0, // Requires separate API call
                        PlaylistUrl = !string.IsNullOrEmpty(playlistId)
                            ? $"https://music.youtube.com/playlist?list={playlistId}"
                            : $"https://music.youtube.com/browse/{browseId}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting from two-row item: {ex.Message}");
            }

            return null;
        }

        private AlbumInfo ExtractAlbumFromSongItem(JToken songRenderer, string fallbackArtistName)
        {
            if (songRenderer == null) return null;

            try
            {
                var flexColumns = songRenderer["flexColumns"];
                if (flexColumns == null || !flexColumns.Any()) return null;

                string extractedArtist = fallbackArtistName;
                string albumName = null;
                string albumBrowseId = null;

                // YouTube Music can have different column arrangements
                foreach (var column in flexColumns)
                {
                    var columnRenderer = column["musicResponsiveListItemFlexColumnRenderer"];
                    if (columnRenderer == null) continue;

                    var runs = columnRenderer["text"]?["runs"];
                    if (runs == null || !runs.Any()) continue;

                    var firstRun = runs[0];
                    var text = firstRun["text"]?.ToString();
                    var endpoint = firstRun["navigationEndpoint"];

                    if (string.IsNullOrEmpty(text)) continue;

                    // Determine what this column represents based on the navigation endpoint
                    var browseEndpoint = endpoint?["browseEndpoint"];
                    if (browseEndpoint != null)
                    {
                        var pageType = browseEndpoint["browseEndpointContextSupportedConfigs"]?["browseEndpointContextMusicConfig"]?["pageType"]?.ToString();

                        if (pageType == "MUSIC_PAGE_TYPE_ARTIST")
                        {
                            extractedArtist = text;
                        }
                        else if (pageType == "MUSIC_PAGE_TYPE_ALBUM")
                        {
                            albumName = text;
                            albumBrowseId = browseEndpoint["browseId"]?.ToString();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(albumName))
                {
                    return new AlbumInfo
                    {
                        ArtistName = extractedArtist,
                        AlbumName = albumName,
                        SongCount = 0,
                        PlaylistUrl = !string.IsNullOrEmpty(albumBrowseId)
                            ? $"https://music.youtube.com/browse/{albumBrowseId}"
                            : null
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting from song item: {ex.Message}");
            }

            return null;
        }

        private string ExtractPlaylistIdFromParams(string encodedParams)
        {
            if (string.IsNullOrEmpty(encodedParams)) return null;

            try
            {
                var decoded = Convert.FromBase64String(encodedParams);
                var decodedString = System.Text.Encoding.UTF8.GetString(decoded);

                // Look for YouTube Music playlist patterns
                var patterns = new[]
                {
                @"OLAK5uy_[A-Za-z0-9_-]+",
                @"PL[A-Za-z0-9_-]+",
                @"RD[A-Za-z0-9_-]+",
                @"VL[A-Za-z0-9_-]+"
            };

                foreach (var pattern in patterns)
                {
                    var match = Regex.Match(decodedString, pattern);
                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding playlist params: {ex.Message}");
            }

            return null;
        }

        private async void GetArtistInfo(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0");

            // Explicitly handle encoding
            var response = await client.GetAsync("https://music.youtube.com/channel/UCHuvxMBXfZ09Be3q6Be0MOQ");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var html = Encoding.UTF8.GetString(bytes);

            html = html.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }

        private void frmArtist_Load(object sender, EventArgs e)
        {

        }
    }

}
