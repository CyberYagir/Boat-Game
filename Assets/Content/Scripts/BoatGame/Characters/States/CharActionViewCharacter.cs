using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionViewCharacter : CharActionBase
    {
        public event Action OnOpenWindow;


        public override void StartState()
        {
            base.StartState();
            OnOpenWindow?.Invoke();
            EndState();
        }
    }
}
