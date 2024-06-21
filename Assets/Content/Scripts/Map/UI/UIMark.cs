using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Map.UI
{
    public class UIMark : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image background;
        [SerializeField] private Image middle;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text nameText;


        [SerializeField] private List<Color> backgroundColor, middleColor;

        public void Init(IslandSeedData islandData, string islandName)
        {
            levelText.text = islandData.Level.ToString();

            background.color = backgroundColor[islandData.Level - 1];
            middle.color = middleColor[islandData.Level - 1];
            nameText.text = islandName;
        }
    }
}
