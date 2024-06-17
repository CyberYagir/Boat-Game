using System;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIDiscoversTooltip : MonoBehaviour
    {
        [SerializeField] private UIDiscoversItem item;
        [SerializeField] private GameObject content;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private TooltipDataObject tooltipItemData;
        [SerializeField] private UITooltip toolTip;
        private bool isOpened = false;
        
        public void Init(SaveDataObject saveData)
        {
            toolTip.Init(tooltipItemData);
            int discoversCount = 0;
            item.gameObject.SetActive(true);
            for (int i = 0; i < saveData.Map.Islands.Count; i++)
            {
                if (!string.IsNullOrEmpty(saveData.Map.Islands[i].IslandName))
                {
                    Instantiate(item, item.transform.parent)
                        .Init(IslandSeedData.Generate(saveData.Map.Islands[i].IslandPos), saveData.Map.Islands[i].IslandName);
                    discoversCount++;
                }
            }
            item.gameObject.SetActive(false);

            Toggle();
            
            if (discoversCount == 0) gameObject.SetActive(false);
        }

        public void Toggle()
        {
            isOpened = !isOpened;

            if (isOpened)
            {
                arrow.DORotate(Vector3.forward * -90, 0.2f).SetLink(gameObject);
            }
            else
            {
                arrow.DORotate(Vector3.zero, 0.2f).SetLink(gameObject);
            }

            content.gameObject.SetActive(isOpened);
        }
        
    }
}
