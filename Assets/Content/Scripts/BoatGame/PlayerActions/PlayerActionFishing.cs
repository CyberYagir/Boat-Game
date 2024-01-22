using Content.Scripts.BoatGame.Characters.States;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionFishing : PlayerAction
    {
        public override bool IsCanCancel()
        {
            if (IsSelectedCharacterOnThisAction())
            {
                var action = SelectionService.SelectedCharacter.GetCharacterAction<CharActionFishing>();
                return action.CurrentState != CharActionFishing.EState.RopeBack && action.CurrentState != CharActionFishing.EState.FishToStorage;
            }

            return true;
        }

        public override void BreakAction()
        {
            if (IsCanCancel())
            {
                base.BreakAction();
            }
        }
    }
}