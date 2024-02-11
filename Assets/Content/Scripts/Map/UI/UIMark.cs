using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Map.UI
{
    public class UIMark : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image middle;
        [SerializeField] private TMP_Text levelText;


        [SerializeField] private List<Color> backgroundColor, middleColor;

        public void Init(MapIsland.IslandData islandData)
        {
            levelText.text = islandData.Level.ToString();

            background.color = backgroundColor[islandData.Level - 1];
            middle.color = middleColor[islandData.Level - 1];
        }
    }
}
