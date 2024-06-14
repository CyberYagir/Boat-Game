using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionFurnace : CharActionMoveTo
    {
        public event Action OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            

            var furnace = SelectionService.SelectedObject.Transform.GetComponent<Furnace>();
            if (furnace == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(furnace.transform.position, Machine.transform.position) < 3)
            {
                OnOpenWindow?.Invoke();
                EndState();
            }
            else
            {
                MoveToPoint(furnace.transform.position);
            }
        }
    }
}
