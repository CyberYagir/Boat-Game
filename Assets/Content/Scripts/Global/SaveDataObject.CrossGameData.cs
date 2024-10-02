using System;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class CrossGameData
        {
            [SerializeField] private int soulsCount;

            public int SoulsCount => soulsCount;

            [System.NonSerialized] public UnityEvent OnSoulsChanged = new UnityEvent();
            

            public void AddSoul()
            {
                soulsCount++;
                OnSoulsChanged.Invoke();
            }

            public CrossGameData Clone()
            {
                return (CrossGameData) MemberwiseClone();
            }

            public void RemoveSouls(int containerSoulsCount)
            {
                soulsCount -= containerSoulsCount;
                OnSoulsChanged.Invoke();
            }
        }
    }
}