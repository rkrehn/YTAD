using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class Song
{
    public int Number { get; set; }
    public string Name { get; set; }
    public string VideoId { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string Duration { get; set; }
    public string Url => $"https://music.youtube.com/watch?v={VideoId}";
}

public class AlbumInfo
{
    public string Album { get; set; }
    public string Artist { get; set; }
}

public static class YouTubeMusicExtractor
{
    private static readonly Regex AlbumPrefixRegex = new Regex(@"^(Album|EP|Single)\s*-\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex TopicSuffixRegex = new Regex(@"\s*-\s*Topic$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static async Task<List<Song>> ExtractSongsAsync(string albumUrl, string ytDlpPath = "yt-dlp")
    {
        var songs = new List<Song>();

        var startInfo = new ProcessStartInfo
        {
            FileName = ytDlpPath,
            Arguments = $"--dump-single-json --flat-playlist \"{albumUrl}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string jsonOutput = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(jsonOutput))
        {
            return songs;
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(jsonOutput);
            JsonElement root = doc.RootElement;

            // 1. Clean Album Name (Strips "Album - ", "EP - ", etc.)
            string rawAlbum = root.TryGetProperty("title", out var titleProp) ? titleProp.GetString() : null;
            string cleanAlbum = CleanAlbumName(rawAlbum);

            // 2. Resolve Fallback Album Artist
            string albumArtist = ExtractArtist(root);

            if (root.TryGetProperty("entries", out JsonElement entries) && entries.ValueKind == JsonValueKind.Array)
            {
                int trackIndex = 1;
                foreach (JsonElement entry in entries.EnumerateArray())
                {
                    string videoId = entry.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    if (string.IsNullOrEmpty(videoId)) continue;

                    string trackTitle = entry.TryGetProperty("title", out var tProp) ? tProp.GetString() : null;

                    // Extract track-level artist, falling back to album artist
                    string trackArtist = ExtractArtist(entry);
                    if (string.IsNullOrEmpty(trackArtist))
                    {
                        trackArtist = albumArtist;
                    }

                    // Duration calculation (seconds -> m:ss)
                    string duration = "Unknown";
                    if (entry.TryGetProperty("duration", out var durProp) && durProp.ValueKind == JsonValueKind.Number)
                    {
                        TimeSpan span = TimeSpan.FromSeconds(durProp.GetDouble());
                        duration = span.ToString(@"m\:ss");
                    }

                    songs.Add(new Song
                    {
                        Number = trackIndex++,
                        Name = trackTitle,
                        VideoId = videoId,
                        Album = cleanAlbum,
                        Artist = trackArtist,
                        Duration = duration
                    });
                }
            }
        }
        catch (JsonException)
        {
            // Handle parsing exceptions gracefully
        }

        return songs;
    }

    private static string CleanAlbumName(string rawAlbum)
    {
        if (string.IsNullOrWhiteSpace(rawAlbum)) return null;

        // Strip leading "Album - ", "EP - ", or "Single - "
        return AlbumPrefixRegex.Replace(rawAlbum, "").Trim();
    }

    private static string ExtractArtist(JsonElement element)
    {
        string artist = null;

        // Check primary artist metadata properties
        if (element.TryGetProperty("artist", out var aProp) && !string.IsNullOrWhiteSpace(aProp.GetString()))
        {
            artist = aProp.GetString();
        }
        else if (element.TryGetProperty("creator", out var cProp) && !string.IsNullOrWhiteSpace(cProp.GetString()))
        {
            artist = cProp.GetString();
        }
        else if (element.TryGetProperty("uploader", out var uProp) && !string.IsNullOrWhiteSpace(uProp.GetString()))
        {
            artist = uProp.GetString();
        }

        if (string.IsNullOrWhiteSpace(artist)) return null;

        // Strip YouTube's auto-generated channel suffix (e.g., "Behemoth - Topic" -> "Behemoth")
        return TopicSuffixRegex.Replace(artist, "").Trim();
    }
}