using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        // Enhanced regex patterns to handle different YouTube Music data structures
        var patterns = new[]
        {
            // Pattern 1: Standard format with navigationEndpoint (made more lenient)
            @"""text"":""([^""]{1,50})"",""navigationEndpoint"":\{[^}]*""watchEndpoint"":\{""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 2: Hex-decoded format (made more lenient)
            @"""text"":\{""runs"":\[\{""text"":""([^""]{1,50})"",""navigationEndpoint"":\{[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 3: Direct text followed by videoId
            @"""text"":""([^""]{1,50})""[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 4: More flexible pattern for complex structures (updated)
            @"""text"":""([A-Za-z0-9][^""]{0,49})"".*?""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 5: Handle accessibility data structure  
            @"""accessibilityPlayData"":\{""accessibilityData"":\{""label"":""Play ([^""]+) - ([^""]+)""[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",

            // Pattern 6: Additional pattern for runs structure with different ordering
            @"""runs"":\[\{""text"":""([^""]{1,50})""[^}]*\}[^}]*""watchEndpoint"":\{""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 7: Pattern specifically for numeric tracks and special characters
            @"""text"":""(\d+|[A-Za-z0-9'&\s\-\(\)\.!?\[\]]{1,50})""[^}]*""navigationEndpoint""[^}]*""videoId"":""([A-Za-z0-9_-]{11})""",
            
            // Pattern 8: Fallback pattern for playNavigationEndpoint structure
            @"""playNavigationEndpoint"":\{[^}]*""videoId"":""([A-Za-z0-9_-]{11})""[^}]*\}[^}]*""text"":\{""runs"":\[\{""text"":""([^""]{1,50})""",
        
            // Pattern 9: flexColumns structure - must have navigationEndpoint immediately after text
            @"""flexColumns"":\[\{""musicResponsiveListItemFlexColumnRenderer"":\{""text"":\{""runs"":\[\{""text"":""([^""]+)"",""navigationEndpoint"":\{""clickTrackingParams""[^}]+""watchEndpoint"":\{""videoId"":""([A-Za-z0-9_-]{11})""",
        };

        var processedVideoIds = new HashSet<string>(); // Track videoIds we've seen
        var excludeWords = new[] { "plays", "minutes", "seconds", "Sign in", "Save", "Play", "Add",
                  "Share", "Go to", "Start", "Remove", "Improve", "Make", "Like",
                  "Dislike", "Not a fan", "Action menu", "songs", "Album", "Artist",
                  "View song", "thousand", "million", "Home", "Explore", "Library",
                  "will play", "added to", "Song will", "Song added", "Track moved" };

        // Dictionary to store song data temporarily for duration matching
        var songDataMap = new Dictionary<string, Song>();

        // Try ALL patterns instead of stopping after the first successful one
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
                else if (pattern.Contains("playNavigationEndpoint") && match.Groups.Count > 2)
                {
                    // Pattern 8 has videoId first, then song name
                    videoId = match.Groups[1].Value;
                    songName = match.Groups[2].Value.Trim();
                }
                else
                {
                    songName = match.Groups[1].Value.Trim();
                    videoId = match.Groups[2].Value;
                }

                // Fix any encoded characters in the song name
                songName = DecodeText(songName);

                // Clean up leading/trailing special characters (pipes, spaces, etc.)
                songName = songName.Trim().TrimStart('|', ' ', '-', '/', '\\').TrimEnd('|', ' ', '-', '/', '\\').Trim();

                // Try to extract the track index from HTML for this videoId
                var trackIndex = ExtractTrackIndex(decodedContent, videoId);

                // If we've already processed this videoId
                if (processedVideoIds.Contains(videoId))
                {
                    // Check if we should UPDATE the existing song with better data
                    if (songDataMap.ContainsKey(videoId) && IsValidSongName(songName, excludeWords))
                    {
                        var existingSong = songDataMap[videoId];

                        // Prefer longer, more complete song names
                        if (songName.Length > existingSong.Name.Length)
                        {
                            existingSong.Name = songName;
                            if (!string.IsNullOrEmpty(artistFromPattern))
                            {
                                existingSong.Artist = artistFromPattern;
                            }
                            WriteError($"UPDATED: VideoID {videoId} with better name: '{songName}'");
                        }

                        // Update track number if we found one and didn't have one before
                        if (trackIndex.HasValue && existingSong.Number == 0)
                        {
                            existingSong.Number = trackIndex.Value;
                        }
                    }
                    continue; // Skip adding duplicate
                }

                // Validate the song name
                if (IsValidSongName(songName, excludeWords))
                {
                    var song = new Song
                    {
                        Number = trackIndex ?? 0, // Use extracted track number or 0
                        Name = songName,
                        VideoId = videoId,
                        Album = albumInfo?.Album,
                        Artist = artistFromPattern ?? albumInfo?.Artist
                    };

                    // Validate the song has required data before adding
                    if (IsValidSong(song))
                    {
                        songs.Add(song);
                        songDataMap[videoId] = song;
                        processedVideoIds.Add(videoId);
                    }
                    else
                    {
                        WriteError($"INVALID SONG DATA: '{songName}' (VideoID: {videoId}) - Failed validation");
                    }
                }
                else
                {
                    // DEBUG: Log rejected songs
                    WriteError($"REJECTED: '{songName}' (VideoID: {videoId})");
                }
            }
        }

        // Sort songs by their appearance order in the original content
        songs = SortSongsByOriginalOrder(songs, decodedContent);

        // Extract durations for the songs we found
        ExtractDurations(decodedContent, songDataMap);

        // Assign correct track numbers AFTER sorting
        // Only assign sequential numbers if the song doesn't already have a track number from the HTML
        int sequentialNumber = 1;
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].Number == 0) // If we didn't extract a track number from HTML
            {
                songs[i].Number = sequentialNumber;
            }
            else
            {
                // Use the extracted track number
                sequentialNumber = songs[i].Number;
            }
            sequentialNumber++; // Increment for next song
        }

        // Sort by track number to ensure proper order
        songs = songs.OrderBy(s => s.Number).ToList();

        return songs.Take(20).ToList(); // Reasonable limit for an album
    }

    // Helper method to sort songs by their appearance order in the original content
    private static List<Song> SortSongsByOriginalOrder(List<Song> songs, string content)
    {
        var songsWithPositions = new List<(Song song, int position)>();

        foreach (var song in songs)
        {
            // Find the first occurrence of this video ID in the content
            int position = content.IndexOf(song.VideoId, StringComparison.OrdinalIgnoreCase);
            if (position == -1)
            {
                // Fallback: search for song name in quotes
                position = content.IndexOf($"\"{song.Name}\"", StringComparison.OrdinalIgnoreCase);
            }
            songsWithPositions.Add((song, position == -1 ? int.MaxValue : position));
        }

        // Sort by position in the original content
        return songsWithPositions
            .OrderBy(x => x.position)
            .Select(x => x.song)
            .ToList();
    }

    public static string DecodeText(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // First decode HTML entities
        string decoded = System.Web.HttpUtility.HtmlDecode(input);

        // Then decode Unicode escape sequences
        decoded = System.Text.RegularExpressions.Regex.Unescape(decoded);

        return decoded;
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
                Album = DecodeText(match.Groups[1].Value.Trim()),
                Artist = DecodeText(match.Groups[2].Value.Trim())
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
                    Album = DecodeText(parts[0].Trim()),
                    Artist = DecodeText(parts[1].Trim())
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
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (name.Length < 1)
            return false;

        // Special case: Allow pure numeric track names (like "111") if they're reasonable length
        // BUT exclude 1-2 digit numbers which are almost certainly track numbers, not song names
        if (Regex.IsMatch(name, @"^\d+$"))
        {
            // Only allow numeric names that are 3+ digits (like "111", "2112", etc.)
            if (name.Length >= 3 && name.Length <= 5)
                return true;
            else
                return false;
        }

        // For non-numeric names, require at least 2 characters
        if (name.Length < 2)
            return false;

        // Must start with a letter or number
        if (!char.IsLetterOrDigit(name[0]))
            return false;

        // Check for exact matches or very close matches with UI text
        foreach (var word in excludeWords)
        {
            // Exact match - definitely UI text
            if (name.Equals(word, StringComparison.OrdinalIgnoreCase))
                return false;

            // For short names containing common UI words, be suspicious
            if (name.Length < 20 && name.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                // Allow if it's clearly part of a longer title (has other substantial content)
                if (name.Length < word.Length + 5)
                    return false;
            }
        }

        // Check for specific UI patterns (be precise, not broad)
        if (name.Contains("K plays") || // "20K plays", "39K plays"
            name.Contains("M plays") || // "1.2M plays"
            name.Contains("thousand plays") ||
            name.Contains("million plays") ||
            name.Contains("Song will play next") ||
            name.Contains("Song added to queue") ||
            name.Contains("Save to playlist") ||
            name.Contains("•") || // UI separators
            name.Length > 150 || // Reasonable limit
            name.Equals("Pause", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Action menu", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Play ", StringComparison.OrdinalIgnoreCase) && name.Length < 15 ||
            name.StartsWith("Add ", StringComparison.OrdinalIgnoreCase) && name.Length < 15 ||
            name.Contains(" - ") && name.Split(" - ").Length > 2 || // Multiple dashes
            Regex.IsMatch(name, @"^\d+:\d+$") || // Pure time format like "4:03"
            Regex.IsMatch(name, @"^\d+\s+(minutes?|seconds?)$")) // "2 minutes", "45 seconds"
            return false;

        return true;
    }

    private static int? ExtractTrackIndex(string decodedContent, string videoId)
    {
        // Pattern to find the index near a specific videoId
        // Looking for: "videoId":"8F2nfvH9glI"......"index":{"runs":[{"text":"6"}]}
        var indexPattern = $@"""videoId"":""{Regex.Escape(videoId)}"".*?""index"":\{{""runs"":\[\{{""text"":""(\d+)""";
        var match = Regex.Match(decodedContent, indexPattern, RegexOptions.Singleline);

        if (match.Success && int.TryParse(match.Groups[1].Value, out int trackNum))
        {
            return trackNum;
        }

        return null;
    }

    private static bool IsValidSong(Song song)
    {
        // Must have a video ID
        if (string.IsNullOrWhiteSpace(song.VideoId) || song.VideoId.Length != 11)
        {
            WriteError($"IsValidSong FAILED: Invalid VideoID '{song.VideoId}'");
            return false;
        }

        // Must have a song name
        if (string.IsNullOrWhiteSpace(song.Name))
        {
            WriteError($"IsValidSong FAILED: Empty song name for VideoID '{song.VideoId}'");
            return false;
        }

        // Song name must be at least 2 characters (except for special numeric cases handled elsewhere)
        if (song.Name.Length < 2)
        {
            WriteError($"IsValidSong FAILED: Song name too short '{song.Name}' for VideoID '{song.VideoId}'");
            return false;
        }

        // Don't require artist/album - they're nice to have but not critical
        // Artist and Album are often populated from the page title, not per-song data

        return true;
    }

    private static void WriteError(string msg)
    {
        string strFile = Application.StartupPath + "\\error.log";
        if (!System.IO.File.Exists(strFile)) System.IO.File.Create(strFile).Close();

        using (var sr = new StreamWriter(strFile, true, Encoding.UTF8))
        {
            sr.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg);
        }
    }
}