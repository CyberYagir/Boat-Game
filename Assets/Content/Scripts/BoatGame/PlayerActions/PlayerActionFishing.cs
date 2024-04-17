using Content.Scripts.BoatGame.Characters.States;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionFishing : PlayerAction
    {
        public override void BreakAction()
        {
            if (IsCanCancel())
            {
                base.BreakAction();
            }
        }
    }
}