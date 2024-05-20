using System;
using System.Collections.Generic;
using UnityEngine;
using static Content.Scripts.BoatGame.Services.RaftBuildService.RaftItem;

namespace Content.Scripts.BoatGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create RaftsPriorityObject", fileName = "RaftsPriorityObject", order = 0)]
    public class RaftsPriorityObject : ScriptableObject
    {
        [System.Serializable]
        public class Priority
        {
            [SerializeField] private ERaftType raftType;
            [SerializeField] private int priority;

            public Priority(ERaftType raftType)
            {
                this.raftType = raftType;
            }

            public int PriorityIndex => priority;

            public ERaftType RaftType => raftType;
        }

        [SerializeField] private List<Priority> priorities;

        private Dictionary<ERaftType, int> prioritiesMap = new Dictionary<ERaftType, int>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            var names = Enum.GetNames(typeof(ERaftType));

            for (int i = 0; i < names.Length; i++)
            {
                if (priorities.Find(x => x.RaftType.ToString() == names[i]) == null)
                {
                    priorities.Add(new Priority(Enum.Parse<ERaftType>(names[i])));
                }
            }
        }
#endif

        public void Init()
        {
            prioritiesMap.Clear();
            for (int i = 0; i < priorities.Count; i++)
            {
                prioritiesMap.Add(priorities[i].RaftType, priorities[i].PriorityIndex);
            }
        }

        public int GetIndex(ERaftType raftType)
        {
            if (prioritiesMap.ContainsKey(raftType))
            {
                return prioritiesMap[raftType];
            }

            return 0;
        }
    }
}
