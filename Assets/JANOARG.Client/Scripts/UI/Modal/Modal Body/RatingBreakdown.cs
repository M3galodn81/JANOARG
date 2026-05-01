using System.Collections.Generic;
using JANOARG.Client.Behaviors.Common;
using JANOARG.Client.Behaviors.Panels.Profile;
using JANOARG.Client.Data.Playlist;
using JANOARG.Client.Data.Storage;
using JANOARG.Shared.Data.ChartInfo;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Playables;

namespace JANOARG.Client.UI
{
    public class RatingBreakdownModalBody : MonoBehaviour
    {
        public ScrollRect ScrollRect;
        public List<RatingBreakdownEntry> RatingBreakdownEntries;

        public List<ScoreStoreEntry> ScoreStoreEntries;

        //This Playlist will be the main/root playlist so we can use the PlayableSong's metachart and cover
        public Playlist MainPlaylist;
        public Dictionary<string, PlayableSong> SongDict;

        public IEnumerator BuildSongList()
        {
            SongDict = new Dictionary<string, PlayableSong>();

            if (MainPlaylist == null)
            {
                Debug.LogWarning("MainPlaylist is null.");
                yield break;
            }

            HashSet<Playlist> visited = new HashSet<Playlist>();

            yield return StartCoroutine(
                CollectSongsRecursive(MainPlaylist, SongDict, visited)
            );
        }

        public IEnumerator CollectSongsRecursive(
            Playlist playlist,
            Dictionary<string, PlayableSong> dict,
            HashSet<Playlist> visited)
        {
            if (playlist == null || visited.Contains(playlist))
                yield break;

            visited.Add(playlist);

            // 1. Load songs
            if (playlist.Songs != null)
            {
                foreach (var song in playlist.Songs)
                {
                    if (song == null) continue;

                    string path = $"Songs/{song.ID}/{song.ID}";
                    ResourceRequest req = Resources.LoadAsync<ExternalPlayableSong>(path);

                    yield return req;

                    if (req.asset == null)
                    {
                        Debug.LogWarning("Couldn't load Playable Song at " + path);
                        continue;
                    }

                    PlayableSong playable = ((ExternalPlayableSong)req.asset).Data;

                    if (!dict.ContainsKey(song.ID))
                    {
                        dict.Add(song.ID, playable);
                    }
                }
            }

            // 2. Traverse sub-playlists
            if (playlist.Playlists != null)
            {
                foreach (var sub in playlist.Playlists)
                {
                    if (sub?.Playlist == null) continue;

                    yield return StartCoroutine(
                        CollectSongsRecursive(sub.Playlist, dict, visited)
                    );
                }
            }
        }

        public IEnumerator GetCoverImage(PlayableSong song, string id, System.Action<Texture2D> onDone)
        {
            string imagePath = song.Cover.Layers[0].Target;
            string path = $"Songs/{id}/{imagePath}";

            if (Path.HasExtension(path))
                path = Path.ChangeExtension(path, "").TrimEnd('.');

            ResourceRequest req = Resources.LoadAsync<Texture2D>(path);
            yield return req;

            if (req.asset == null)
            {
                Debug.LogWarning("Couldn't load texture at " + path);
                onDone?.Invoke(null);
                yield break;
            }

            onDone?.Invoke((Texture2D)req.asset);
        }
        public IEnumerator GetIconImage(string id, System.Action<Texture2D> onDone)
        {
            string path = $"Songs/{id}/icon";

            ResourceRequest req = Resources.LoadAsync<Texture2D>(path);
            yield return req;

            if (req.asset == null)
            {
                Debug.LogWarning("Couldn't load texture at " + path);
                onDone?.Invoke(null);
                yield break;
            }

            onDone?.Invoke((Texture2D)req.asset);
        }


        private IEnumerator Start()
        {
            ScrollRect.verticalNormalizedPosition = 1f;

            // Get all score entrys from Score Store
            ScoreStoreEntries = StorageManager.sMain.Scores.GetBestEntries();

            // Safety checks
            if (ScoreStoreEntries == null || RatingBreakdownEntries == null)
            {
                Debug.LogError("ScoreStoreEntries or RatingBreakdownEntries is null.");
                yield return null;
            }

            if (MainPlaylist == null)
            {
                Debug.LogError("MainPlaylist is null. Set it on RatingBreakdownModalBody.");
                yield return null;
            }

            // Use the smaller count to avoid out-of-range errors
            int count = Mathf.Min(ScoreStoreEntries.Count, RatingBreakdownEntries.Count);

            // Get all songs that are in the score entries

            // Append a playable song to songlist if the key is match
            yield return StartCoroutine(BuildSongList());
         
            Dictionary<string, PlayableSong> songLookup = new Dictionary<string, PlayableSong>();

            // Looping every rating breakdown entries to set their data
            for (int i = 0; i < count; i++)
            {
                if (RatingBreakdownEntries[i] == null)
                    continue;

                if (ScoreStoreEntries[i] == null)
                    continue;

                var entry = ScoreStoreEntries[i];

                RatingBreakdownEntries[i].SetData(entry);

                if (SongDict != null && SongDict.TryGetValue(entry.SongID, out var song))
                {
                    RatingBreakdownEntries[i].SongName.text = Truncate(song.SongName,30);
                    RatingBreakdownEntries[i].SongArtist.text = Truncate(song.SongArtist,30);
                    RatingBreakdownEntries[i].ChartConstant.text = song.Charts.Find(x => x.Target == entry.ChartID).DifficultyLevel.ToString();
                    
                    Texture2D iconTex = null;

                    yield return StartCoroutine(
                        GetIconImage(entry.SongID, (tex) =>
                        {
                            iconTex = tex;
                        })
                    );

                    if (iconTex != null)
                    {
                        RatingBreakdownEntries[i].Icon.texture = iconTex;
                    } 

                    Texture2D coverTex = null;

                    yield return StartCoroutine(
                        GetCoverImage(song, entry.SongID, (tex) =>
                        {
                            coverTex = tex;
                        })
                    );

                    if (coverTex != null)
                    {
                        RatingBreakdownEntries[i].BackgroundCover.texture = coverTex;
                    }
                }
                else
                {
                    Debug.LogWarning($"Song not found: {entry.SongID}");
                }
            }
        }
        

        string Truncate(string text, int maxLength)
        {
            return text.Length > maxLength
                ? text.Substring(0, maxLength) + "..."
                : text;
        }
    }
}