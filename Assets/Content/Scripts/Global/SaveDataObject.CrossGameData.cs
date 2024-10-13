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
            [System.Serializable]
            public class PlayerStatistics
            {
                [SerializeField] private int countBuildRafts;

                public int CountBuildRafts => countBuildRafts;

                public void AddBuildRaft() => countBuildRafts++;
            }
            
            [SerializeField] private int soulsCount;
            [SerializeField] private PlayerStatistics statistics;
            
            public int SoulsCount => soulsCount;

            public PlayerStatistics Statistics => statistics;

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