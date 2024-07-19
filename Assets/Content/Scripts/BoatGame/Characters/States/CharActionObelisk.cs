using System;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionObelisk : CharActionMoveTo
    {
        public event Action OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            

            var obelisk = SelectionService.SelectedObject.Transform.GetComponent<LoreObelisk>();
            if (obelisk == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(obelisk.transform.position, Machine.transform.position) < 3)
            {
                OnOpenWindow?.Invoke();
                EndState();
            }
            else
            {
                MoveToPoint(obelisk.transform.position);
            }
        }

        protected override void OnMoveEnded()
        {
            OnOpenWindow?.Invoke();
            base.OnMoveEnded();
        }
    }
}
