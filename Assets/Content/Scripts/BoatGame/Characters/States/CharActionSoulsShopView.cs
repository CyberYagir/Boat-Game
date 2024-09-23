using System;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionSoulsShopView : CharActionMoveTo
    {
        public event Action OnOpenWindow;

        public override void StartState()
        {
            base.StartState();
            

            var targetFloatingShop = SelectionService.SelectedObject.Transform.GetComponentInParent<FloatingShop>();
            if (targetFloatingShop == null)
            {
                EndState();
                return;
            }

            OnOpenWindow?.Invoke();
            EndState();
        }
    }
}