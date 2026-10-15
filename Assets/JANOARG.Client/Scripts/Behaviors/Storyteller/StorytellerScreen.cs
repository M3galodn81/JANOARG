using System.Collections;
using JANOARG.Client.Behaviors.Common;
using JANOARG.Client.Data.Story;
using UnityEngine;
using UnityEngine.UIElements;

namespace JANOARG.Client.Behaviors.Storyteller
{
    public class StorytellerScreen : MonoBehaviour
    {
        public static StorytellerScreen sMain;

        public static StoryScript sScriptToPlay;
        public bool IsInit;

        public void Awake()
        {
            sMain = this;
            CommonScene.Load();
        }

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(StoryRoutine());
        }

        
        public IEnumerator StoryRoutine()
        {
            if (sScriptToPlay)
            {
                IsInit = true;
                Storyteller.sMain.CurrentScript = sScriptToPlay;
            } else
            {
                yield break;   
            }
        }
    }
}
