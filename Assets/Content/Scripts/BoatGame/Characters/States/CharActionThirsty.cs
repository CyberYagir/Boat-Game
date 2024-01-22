namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionThirsty : CharActionHunger
    {
        public override void Animation() => Machine.AnimationManager.TriggerDrinkAnimation();
    }
}
