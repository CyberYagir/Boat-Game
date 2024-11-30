using System;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionEnterDungeon : CharActionMoveTo
    {
        private Crypt selectedCrypt;
        public event Action<Crypt> OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            

            selectedCrypt = SelectionService.SelectedObject.Transform.GetComponent<Crypt>();
            if (selectedCrypt == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(selectedCrypt.transform.position, Machine.transform.position) < 3)
            {
                OnOpenWindow?.Invoke(selectedCrypt);
                EndState();
            }
            else
            {
                MoveToPoint(selectedCrypt.transform.position);
            }
        }

        protected override void OnMoveEnded()
        {
            OnOpenWindow?.Invoke(selectedCrypt);
            base.OnMoveEnded();
        }

        public override void EndState()
        {
            base.EndState();
        }
    }
}