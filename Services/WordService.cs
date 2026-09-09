using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WordOverlay.Models;

namespace WordOverlay.Services
{
    public class WordService
    {
        private readonly List<WordEntry> _entries = new();
        private readonly Random _random = new();
        private int _currentIndex = -1;

        public WordService(string jsonPath)
        {
            LoadEntries(jsonPath);
        }

        private void LoadEntries(string jsonPath)
        {
            try
            {
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var loaded = JsonSerializer.Deserialize<List<WordEntry>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loaded != null && loaded.Count > 0)
                    {
                        _entries.AddRange(loaded);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load word list: {ex.Message}");
            }

            // Fallback so the overlay always has something to show.
            _entries.Add(new WordEntry { Word = "No words loaded", Definition = "Check words.json" });
        }

        public WordEntry GetNext()
        {
            if (_entries.Count == 1)
            {
                _currentIndex = 0;
                return _entries[0];
            }

            int next;
            do
            {
                next = _random.Next(_entries.Count);
            } while (next == _currentIndex);

            _currentIndex = next;
            return _entries[_currentIndex];
        }
    }
}
