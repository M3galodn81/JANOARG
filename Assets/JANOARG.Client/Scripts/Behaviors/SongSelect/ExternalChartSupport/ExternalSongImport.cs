using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JANOARG.Client.Data.Playlist;
using UnityEngine.SceneManagement;
using JANOARG.Client.Behaviors.SongSelect.Map.MapItems;

namespace JANOARG.Client.Behaviors.SongSelect
{
    public class ExternalSongImport : MonoBehaviour
    {
        public ExternalPlaylist ExternalPlaylist;
        void Awake()
        {
            UpdateScene();
        }
        
        public void UpdateScene()
        {   
            string sceneName = SongSelectScreen.sMain.Playlist.MapName + " Map";
            GameObject parent = GameObject.Find("External Song Items");

            if (parent == null)
            {
                Debug.LogError("External Song Items not found in scene " + sceneName + "!");
                return;
            }

            // --- NEW: Remove existing children ---
            foreach (Transform child in parent.transform)
            {
                // Use Destroy for normal play, or DestroyImmediate if this runs in-editor
                GameObject.Destroy(child.gameObject);
            }
            // -------------------------------------

            // Reset index to 0 because the parent is now empty
            int index = 0; 

            ExternalPlaylist.ArrayToList(); // Ensure the list is up to date with the array
            foreach (PlaylistSong song in ExternalPlaylist.Songlist)
            {
                Debug.Log("[Playlist Management] Adding song in map: " + song.ID);

                GameObject child = new GameObject($"{song.ID}");
                child.transform.SetParent(parent.transform, false);

                // Incremental positioning
                child.transform.localPosition = new Vector3(index * 5f, 0f, 0f);
                index++;

                // Add + initialize safely
                SongMapItem item = child.AddComponent<SongMapItem>();
                item.Initialize(song);
            }
        }


    }
}
