using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JANOARG.Client.Behaviors.Common;
using JANOARG.Client.Data.Storage;
using JANOARG.Client.Behaviors.SongSelect;
using JANOARG.Shared.Data.ChartInfo;
using System.IO;

using JANOARG.Client.Utils;
using JANOARG.Client.Behaviors.SongSelect.List;

namespace JANOARG.Client.Behaviors.Panels.Profile
{
    public class RatingBreakdownEntry : MonoBehaviour
    {
        public TMP_Text Rating;
        public TMP_Text BestScore;
        public TMP_Text SongName;
        public TMP_Text SongArtist;
        public TMP_Text ChartConstant;

        public Image Icon;
        public Image BackgroundCover;

        public GameObject FullStreakIndicator;
        public GameObject AllFlawlessIndicator;

        // private List<SongSelectItemUI> _SongSelectItems;
        // private Color _Color;

        public void SetData(ScoreStoreEntry entry)
        {
            Rating.text = entry.Rating.ToString("F2");
            BestScore.text = Helper.PadScore(entry.Score.ToString()) + "<size=50%><b>ppm";
            // SongName.text = entry.SongName;
            // SongArtist.text = entry.SongArtist;
            // ChartConstant.text = entry.ChartConstant;
            if (entry.BadCount == 0)
            {
                FullStreakIndicator.SetActive(true);
            }
            else if (entry.PerfectCount == entry.MaxCombo)
            {
                AllFlawlessIndicator.SetActive(true);
            }
        }

        
    }
}