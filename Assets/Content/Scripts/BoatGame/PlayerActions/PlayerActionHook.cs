using Content.Scripts.BoatGame.Characters.States;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionHook : PlayerAction
    {
        public override bool IsCanCancel()
        {
            if (IsSelectedCharacterOnThisAction())
            {
                var action = SelectionService.SelectedCharacter.GetCharacterAction<CharActionHooking>();
                return action.State is CharActionHooking.States.HookSpawned or CharActionHooking.States.HookBack;
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