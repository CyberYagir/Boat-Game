using System;
using System.Collections.Generic;
using UnityEngine;
using static Content.Scripts.BoatGame.PlayerCharacter.AppearanceManager;

namespace Content.Scripts.BoatGame.Equipment
{
    public class EquipmentWorker : MonoBehaviour
    {
        [SerializeField] private EBones targetBone;
        protected PlayerCharacter.AppearanceManager appearanceManager;

        public EBones TargetBone => targetBone;


        public virtual void Init(PlayerCharacter.AppearanceManager appearanceManager)
        {
            this.appearanceManager = appearanceManager;
        }

        public virtual void OnDestroy()
        {
            
        }
    }
}
