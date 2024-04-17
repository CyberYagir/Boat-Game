using Content.Scripts.BoatGame.Characters.States;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionHook : PlayerAction
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