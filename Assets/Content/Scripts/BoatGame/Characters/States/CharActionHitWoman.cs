namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionHitWoman : CharActionAttack
    {

        public override void AttackEnemy()
        {
            base.AttackEnemy();
            
            EndState();
        }
    }
}