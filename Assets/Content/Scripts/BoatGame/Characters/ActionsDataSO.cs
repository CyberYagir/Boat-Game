using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Characters
{
    [CreateAssetMenu(menuName = "Create ActionsDataSO", fileName = "ActionsDataSO", order = 0)]
    public class ActionsDataSO : ScriptableObject
    {
        [System.Serializable]
        public class IconsKeys<T>
        {
            [SerializeField] private T key;
            [SerializeField] private Sprite icon;

            public Sprite Icon => icon;

            public T Key => key;
        }

        [SerializeField] private List<IconsKeys<EStateType>> stateTypes;

        private Dictionary<EStateType, Sprite> stateIconsMap;

        public void Init()
        {
            stateIconsMap = stateTypes.ToDictionary(x => x.Key, x => x.Icon);
        }
        public Sprite GetActionIcon(EStateType type)
        {
            if (stateIconsMap.ContainsKey(type))
            {
                return stateIconsMap[type];
            }
            return null;
        }
    }
}
