namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionHitWoman : CharActionAttack
    {
        public override void StartState()
        {
            base.StartState();
            
            print("start");
        }

        public override void AttackEnemy()
        {
            base.AttackEnemy();
            
            EndState();
        }
    }
}