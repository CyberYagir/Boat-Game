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

        public override int PriorityAdd()
        {
            var points = SelectionService.SelectedCharacter.NeedManager.Hunger;

            if (points < 30)
            {
                return 3;
            }

            if (points < 40)
            {
                return 2;
            }

            if (points < 70)
            {
                return 1;
            }

            return 0;
        }
    }
}
