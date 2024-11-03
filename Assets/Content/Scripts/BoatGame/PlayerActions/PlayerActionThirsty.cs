using Content.Scripts.BoatGame.Characters.States;

namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionThirsty : PlayerActionHunger
    {
        public override int PriorityAdd()
        {
            var points = SelectionService.SelectedCharacter.NeedManager.Thirsty;

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
