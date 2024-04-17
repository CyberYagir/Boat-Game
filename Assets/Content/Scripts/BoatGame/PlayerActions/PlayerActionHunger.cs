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
            if (base.IsCanShow())
            {
                return SelectionService.SelectedCharacter.AIMoveManager.FindResource(type) != null;
            }

            return false;
        }
    }
}
