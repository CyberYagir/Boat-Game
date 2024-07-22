using System;
using System.Collections.Generic;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.Scriptable
{
    public class PotionLogicBaseSO : ScriptableObject
    {
        public enum EPotionType
        {
            Moment,
            Time
        }

        [SerializeField, ReadOnly] private string uid;
        [SerializeField] private EPotionType potionType;
        [SerializeField, ShowIf("@potionType == EPotionType.Time")] private int seconds;
        [SerializeField, ShowIf("@potionType == EPotionType.Time")] private float multiply;
        [SerializeField, ShowIf("@potionType == EPotionType.Time")] private bool stackable;
        
        
        protected ItemObject sender;
        protected PlayerCharacter playerCharacter;


        public ItemObject Sender => sender;

        public float Multiply => multiply;

        public int Seconds => seconds;

        public EPotionType PotionType => potionType;

        public string Uid => uid;

        public bool Stackable => stackable;

        public virtual void StartEffect(PlayerCharacter playerCharacter, ItemObject item)
        {
            this.playerCharacter = playerCharacter;
            sender = item;
        }

        [Button]
        public void GenerateID()
        {
            uid = Guid.NewGuid().ToString();
        }

        public void RemoveSecond()
        {
            seconds--;
        }

        public virtual void AddEffectBonus()
        {
        }

        public virtual void StopEffect()
        {
            
        }

        public void SetSeconds(int effectRemainingSeconds)
        {
            seconds = effectRemainingSeconds;
        }
    }
}
