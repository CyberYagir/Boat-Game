using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionHunger : PlayerAction
    {
        [SerializeField] protected EResourceTypes type;
        public override bool IsCanShow()
        {
            // var count = SelectionService.SelectedCharacter.GetCharacterAction<CharActionHunger>().Count;
            return SelectionService.SelectedCharacter.AIMoveManager.FindResource(type) != null;
        }

        public override bool IsCanCancel()
        {
            if (IsSelectedCharacterOnThisAction())
            {
                var action = SelectionService.SelectedCharacter.GetCharacterAction<CharActionHunger>();
                return action.CurrentState is CharActionHunger.EHungerState.MoveToStorage;
            }

            return true;
        }
    }
}
