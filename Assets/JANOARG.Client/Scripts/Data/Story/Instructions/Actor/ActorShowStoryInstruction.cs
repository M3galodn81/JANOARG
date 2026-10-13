using System;
using UnityEngine;
using JANOARG.Client.Behaviors.Storyteller;
using JANOARG.Client.Data.Files;
using System.Collections;

namespace JANOARG.Client.Data.Story.Instructions
{
    [Serializable]
    public class ActorShowStoryInstruction : StoryInstruction
    {
        public string Alias;
        public string SpriteId;
        public ActorPosition Destination;

        [StoryTag("ActorShow")]
        public ActorShowStoryInstruction(string id , string spriteId, string destination)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
               throw new ArgumentNullException("Actor Show Instruction: Alias is null or empty."); 
            }
            if (string.IsNullOrWhiteSpace(spriteId))
            {
               throw new ArgumentNullException("Actor Show Instruction: SpriteId is null or empty."); 
            }
            
            Alias = id;
            SpriteId = spriteId;
            if (!Enum.TryParse(destination, true, out Destination))
            {
                throw new ArgumentException($"Invalid actor position: {destination}");
            }
        }

        public override IEnumerator OnActorAction(Storyteller teller)
        {
            yield return teller.ActorStage.ShowActor(Alias,SpriteId,Destination);
        }
    }
}