using System;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionViewVillage : CharActionMoveTo
    {
        private NativeController selectedShaman;
        public event Action<string, int> OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            

            selectedShaman = SelectionService.SelectedObject.Transform.GetComponent<NativeController>();
            if (selectedShaman == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(selectedShaman.transform.position, Machine.transform.position) < 3)
            {
                OnOpenWindow?.Invoke(selectedShaman.VillageData.VillageID, selectedShaman.VillageData.IslandData.Level);
                EndState();
            }
            else
            {
                selectedShaman.ChangeStateTo(EMobsState.Stop);
                MoveToPoint(selectedShaman.transform.position);
            }
        }

        protected override void OnMoveEnded()
        {
            OnOpenWindow?.Invoke(selectedShaman.VillageData.VillageID, selectedShaman.VillageData.IslandData.Level);
            base.OnMoveEnded();
        }

        public override void EndState()
        {
            base.EndState();

            if (selectedShaman)
            {
                selectedShaman.ChangeStateTo(EMobsState.Idle);
            }
        }
    }
}