using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class Song
{
    public int Number { get; set; }
    public string Name { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public string VideoId { get; set; }
    public string Duration { get; set; } // Format: "4:03"
    public string Url => $"https://music.youtube.com/watch?v={VideoId}";
}

public class AlbumInfo
{
    public string Album { get; set; }
    public string Artist { get; set; }
}

public class YouTubeMusicExtractor
{
    public static List<Song> ExtractSongs(string htmlContent)
    {
        var songs = new List<Song>();

        // First decode any hex-encoded content
        string decodedContent = DecodeHexContent(htmlContent);

        // Extract album and artist information from page title
        var albumInfo = ExtractAlbumInfo(htmlContent);

        // Multiple regex patterns to handle different YouTube Music data structures
        var patterns = new[]
        {
            // Pattern 1: Standard format with navigationEndpoint
            @"""text"":""([^""]{2,50})"",""navigationEndpoint"":\{[^}]*""watchEndpoint"":\{""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 2: Hex-decoded format from your samples
            @"""text"":\{""runs"":\[\{""text"":""([^""]{2,50})"",""navigationEndpoint"":\{[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 3: Direct text followed by videoId (for hex-decoded content)
            @"""text"":""([^""]{2,50})""[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 4: More flexible pattern for complex structures
            @"""text"":""([A-Za-z][^""]{1,49})"".*?""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 5: Handle the specific structure from your sample with accessibility data
            @"""accessibilityPlayData"":\{""accessibilityData"":\{""label"":""Play ([^""-]+) - ([^""]+)""[^}]*""videoId"":""([A-Za-z0-9_-]{11})"""
        };

        var processedVideoIds = new HashSet<string>();
        var excludeWords = new[] { "plays", "minutes", "seconds", "Sign in", "Save", "Play", "Add",
                                  "Share", "Go to", "Start", "Remove", "Improve", "Make", "Like",
                                  "Dislike", "Not a fan", "Action menu", "songs", "Album", "Artist",
                                  "View song", "thousand", "million" };

        // Dictionary to store song data temporarily for duration matching
        var songDataMap = new Dictionary<string, Song>();

        // Try each pattern
        foreach (var pattern in patterns)
        {
            var matches = Regex.Matches(decodedContent, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string songName, videoId, artistFromPattern = null;

                // Handle different pattern capture groups
                if (pattern.Contains("accessibilityPlayData") && match.Groups.Count > 3)
                {
                    // Pattern 5 captures both song and artist
                    songName = match.Groups[1].Value.Trim();
                    artistFromPattern = match.Groups[2].Value.Trim();
                    videoId = match.Groups[3].Value;
                }
                else
                {
                    songName = match.Groups[1].Value.Trim();
                    videoId = match.Groups[2].Value;
                }

                // Skip if we've already processed this video ID
                if (processedVideoIds.Contains(videoId))
                    continue;

                // Validate the song name
                if (IsValidSongName(songName, excludeWords))
                {
                    var song = new Song
                    {
                        Number = songs.Count + 1,
                        Name = songName,
                        VideoId = videoId,
                        Album = albumInfo?.Album,
                        Artist = artistFromPattern ?? albumInfo?.Artist // Prefer pattern-extracted artist
                    };

                    songs.Add(song);
                    songDataMap[videoId] = song;
                    processedVideoIds.Add(videoId);
                }
            }

            // If we found songs with this pattern, we can stop trying other patterns
            if (songs.Count > 0) break;
        }

        // Extract durations for the songs we found
        ExtractDurations(decodedContent, songDataMap);

        return songs.Take(20).ToList(); // Reasonable limit for an album
    }

    private static AlbumInfo ExtractAlbumInfo(string htmlContent)
    {
        // Pattern to match: <title>De Profundis - Album by VADER</title>
        var titlePattern = @"<title>([^-]+) - Album by ([^<]+)</title>";
        var match = Regex.Match(htmlContent, titlePattern);

        if (match.Success)
        {
            return new AlbumInfo
            {
                Album = match.Groups[1].Value.Trim(),
                Artist = match.Groups[2].Value.Trim()
            };
        }

        // Fallback pattern for other title formats
        var alternatePattern = @"<title>([^|]+) \| ([^<]+)</title>";
        match = Regex.Match(htmlContent, alternatePattern);

        if (match.Success)
        {
            var parts = match.Groups[1].Value.Split('-');
            if (parts.Length >= 2)
            {
                return new AlbumInfo
                {
                    Album = parts[0].Trim(),
                    Artist = parts[1].Trim()
                };
            }
        }

        return null;
    }

    private static void ExtractDurations(string decodedContent, Dictionary<string, Song> songDataMap)
    {
        // Pattern to match duration in fixedColumns sections
        // Looking for: "text":{"runs":[{"text":"4:03"}]} in fixedColumns after a song
        var durationPattern = @"""fixedColumns"":\[[^]]*""text"":\{""runs"":\[\{""text"":""(\d+:\d{2})""";
        var matches = Regex.Matches(decodedContent, durationPattern);

        // Also try a more specific pattern that looks for the duration near videoId
        var contextualDurationPattern = @"""videoId"":""([A-Za-z0-9_-]{11})""[^}]*}[^}]*}[^}]*""fixedColumns"":\[[^]]*""text"":\{""runs"":\[\{""text"":""(\d+:\d{2})""";
        var contextualMatches = Regex.Matches(decodedContent, contextualDurationPattern);

        // Process contextual matches first (more reliable)
        foreach (Match match in contextualMatches)
        {
            var videoId = match.Groups[1].Value;
            var duration = match.Groups[2].Value;

            if (songDataMap.ContainsKey(videoId))
            {
                songDataMap[videoId].Duration = duration;
            }
        }

        // For songs without duration from contextual matching, try to match by position
        // This is a bit hacky but sometimes necessary with YouTube's inconsistent structure
        if (matches.Count > 0 && songDataMap.Values.Any(s => string.IsNullOrEmpty(s.Duration)))
        {
            var songsWithoutDuration = songDataMap.Values.Where(s => string.IsNullOrEmpty(s.Duration)).ToList();

            for (int i = 0; i < Math.Min(matches.Count, songsWithoutDuration.Count); i++)
            {
                var duration = matches[i].Groups[1].Value;
                // Simple validation - should be in format M:SS or MM:SS
                if (Regex.IsMatch(duration, @"^\d{1,2}:\d{2}$"))
                {
                    songsWithoutDuration[i].Duration = duration;
                }
            }
        }

        // Final fallback: look for any duration pattern near song names
        foreach (var song in songDataMap.Values.Where(s => string.IsNullOrEmpty(s.Duration)))
        {
            var songContextPattern = $@"""{Regex.Escape(song.Name)}""[^}}]*}}[^}}]*""text"":\{{""runs"":\[\{{""text"":""(\d+:\d{{2}})""";
            var contextMatch = Regex.Match(decodedContent, songContextPattern);

            if (contextMatch.Success)
            {
                song.Duration = contextMatch.Groups[1].Value;
            }
        }
    }

    private static string DecodeHexContent(string content)
    {
        // Decode common hex sequences found in YouTube Music data
        return content.Replace("\\x22", "\"")
                     .Replace("\\x5b", "[")
                     .Replace("\\x5d", "]")
                     .Replace("\\x7b", "{")
                     .Replace("\\x7d", "}")
                     .Replace("\\x3d", "=")
                     .Replace("\\x26", "&")
                     .Replace("\\x2c", ",")
                     .Replace("\\x3a", ":")
                     .Replace("\\x5c", "\\")
                     .Replace("\\x2f", "/")
                     .Replace("\\x27", "'")
                     .Replace("\\x28", "(")
                     .Replace("\\x29", ")")
                     .Replace("\\x2d", "-")
                     .Replace("\\x2e", ".");
    }

    private static bool IsValidSongName(string name, string[] excludeWords)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            return false;

        // Must start with a letter or number (some songs start with numbers)
        if (!char.IsLetterOrDigit(name[0]))
            return false;

        // Check if it contains any excluded words
        foreach (var word in excludeWords)
        {
            if (name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
        }

        // Additional checks - typical song names don't contain these patterns
        if (name.Contains("K ") || name.Contains(" K") || // Like "8.9K plays"
            name.All(char.IsDigit) || // Pure numbers
            name.Contains("•") || // UI separators
            name.Length > 150 || // Increased limit for longer song titles
            name.Contains("Pause ") || // UI text
            name.Contains("Action ") || // UI text
            name.Contains(" - ") && name.Split(" - ").Length > 2 || // Multiple dashes (likely UI text)
            Regex.IsMatch(name, @"^\d+:\d+$")) // Pure time formats
            return false;

        return true;
    }
}