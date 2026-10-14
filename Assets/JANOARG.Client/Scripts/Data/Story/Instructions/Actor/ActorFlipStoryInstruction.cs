using System;
using UnityEngine;
using JANOARG.Client.Behaviors.Storyteller;
using JANOARG.Client.Data.Files;
using System.Collections;

namespace JANOARG.Client.Data.Story.Instructions
{
    [Serializable]
    public class ActorFlipStoryInstruction : StoryInstruction
    {
        public string Alias;
        public ActorPosition Destination;

        [StoryTag("ActorFlip")]
        public ActorFlipStoryInstruction(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
               throw new ArgumentNullException("Actor Show Instruction: Alias is null or empty."); 
            }
            Alias = id;
        }

        public override IEnumerator OnActorAction(Storyteller teller)
        {
            yield return teller.ActorStage.Flip(Alias);
        }
    }
}