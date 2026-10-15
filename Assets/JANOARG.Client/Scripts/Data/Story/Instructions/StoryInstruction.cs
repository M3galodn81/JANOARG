using System;
using System.Collections;
using JANOARG.Client.Behaviors.Storyteller;

namespace JANOARG.Client.Data.Story.Instructions
{
    [Serializable]
    public abstract class StoryInstruction
    {
        public virtual void OnTextBuild(Storyteller teller) {}

        public virtual IEnumerator OnSceneSet(Storyteller teller) => null;
        public virtual IEnumerator OnBeforeTextReveal(Storyteller teller) => null;
        public virtual IEnumerator OnTextReveal(Storyteller teller) => null;
        public virtual IEnumerator OnAfterTextReveal(Storyteller teller) => null;
    }
}