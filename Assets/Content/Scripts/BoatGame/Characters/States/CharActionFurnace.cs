using System;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionFurnace : CharActionBase
    {
        public Action OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            
            OnOpenWindow?.Invoke();
            
            EndState();
        }
    }
}
