namespace Content.Scripts.BoatGame.PlayerActions
{
    public class PlayerActionViewInfo : PlayerAction
    {
        public override int PriorityAdd()
        {
            return SelectionService.SelectedCharacter.Character.SkillData.Scores > 0 ? 1 : 0;
        }
    }
}
