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
        public event Action<NativeController> OnFirstOpenWindow;

        public override void StartState()
        {
            base.StartState();


            

            if (selectedShaman == null)
            {
                selectedShaman = SelectionService.SelectedObject.Transform.GetComponent<NativeController>();
            }
            
            
            

            if (selectedShaman == null)
            {
                EndState();
                return;
            }

#if  UNITY_EDITOR
            
            OnOpenWindow?.Invoke(selectedShaman.VillageData.VillageID, selectedShaman.VillageData.IslandData.Level);
#endif
            
            
            MoveTo();
        }

        private void MoveTo()
        {
            if (Vector3.Distance(selectedShaman.transform.position, Machine.transform.position) < 3)
            {
                OpenWindow();
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
            OpenWindow();

            base.OnMoveEnded();
        }

        private void OpenWindow()
        {
            if (Machine.SaveData.Tutorials.VillageDialogTutorial)
            {
                OnOpenWindow?.Invoke(selectedShaman.VillageData.VillageID, selectedShaman.VillageData.IslandData.Level);
            }
            else
            {
                OnFirstOpenWindow?.Invoke(selectedShaman);
            }
        }

        public override void EndState()
        {
            base.EndState();

            if (selectedShaman)
            {
                selectedShaman.ChangeStateTo(EMobsState.Idle);
                selectedShaman = null;
            }
        }

        public void ApplyShaman(NativeController villageShaman)
        {
            selectedShaman = villageShaman;
            MoveTo();
        }
    }
}