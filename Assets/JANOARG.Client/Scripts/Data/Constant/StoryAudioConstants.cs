using System;
using System.Collections.Generic;
using UnityEngine;

namespace JANOARG.Client.Data.Constant
{
    // Main Audio Constants List that can be nested like how Playlist works
    // so there will not be a long list of audio
    [CreateAssetMenu(fileName = "Story Audio Constants", menuName = "JANOARG/Story/Story Audio Constants")]
    public class StoryAudioConstants : ScriptableObject
    {
        public Music[] MusicList;
        public SoundEffects[] SFXList;
        public StoryAudioReference[] StoryAudios;
    }

    [Serializable]
    public class Music
    {
        public string ID;
        public AudioClip MusicClip;
    }

    [Serializable]
    public class SoundEffects
    {
        public string ID;
        public AudioClip SoundEffect;
    }

    [Serializable]
    public class StoryAudioReference
    {
        public StoryAudioConstants StoryAudio;
    }


}