using TMPro;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversItem : MonoBehaviour
    {
        [SerializeField] private UIMark markIcon;
        [SerializeField] private TMP_Text text;

        public void Init(IslandSeedData islandData, string islandName)
        {
            markIcon.Init(islandData);
            text.text = islandName;
        }
    }
}
