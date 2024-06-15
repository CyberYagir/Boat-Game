using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
            [SerializeField, PreviewField] private Sprite icon;

            public Sprite Icon => icon;

            public T Key => key;
        }

        [SerializeField] private List<IconsKeys<EStateType>> stateTypes;
        [SerializeField] private Sprite errorIcon;
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
            return errorIcon;
        }
    }
}
