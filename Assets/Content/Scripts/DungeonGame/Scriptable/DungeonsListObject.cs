using System.Collections.Generic;
using Content.Scripts.DungeonGame.Services;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create DungeonsListObject", fileName = "DungeonsListObject", order = 0)]
    public class DungeonsListObject : ScriptableObject
    {
        [SerializeField] private List<DungeonService.ConfigsHolder> configs = new List<DungeonService.ConfigsHolder>();
        [SerializeField, ShowIf("@isDebug")] private List<DungeonService.ConfigsHolder> debugConfigs = new List<DungeonService.ConfigsHolder>();

        [SerializeField] private bool isDebug;

        public List<DungeonService.ConfigsHolder> GetConfigs()
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                return debugConfigs;
            }
#endif
            
            return configs;
        }
    }
}
