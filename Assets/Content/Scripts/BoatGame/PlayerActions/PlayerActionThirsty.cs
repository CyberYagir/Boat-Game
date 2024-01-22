using Content.Scripts.BoatGame.Characters.States;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionThirsty : PlayerActionHunger
    {
        public override bool IsCanShow()
        {
            return SelectionService.SelectedCharacter.NavigationManager.FindResource(type) != null;
        }
    }
}
