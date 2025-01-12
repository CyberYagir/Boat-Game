using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.DungeonGame.UI
{
    public class UIGetScrollWindow : AnimatedWindow
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Image image;
        [SerializeField, TextArea] private string desc;
        private GameDataObject gameDataObject;
        private GameStateService gameStateService;


        public void Init(GameDataObject gameDataObject, IResourcesService dungeonResourcesService)
        {
            this.gameStateService = gameStateService;
            this.gameDataObject = gameDataObject;
            nameText.text = gameDataObject.ConfigData.LoreItem.ItemName;
            image.sprite = gameDataObject.ConfigData.LoreItem.ItemIcon;
            dungeonResourcesService.OnAddItemToRaft += CheckScrollInRaft;
        }

        private void CheckScrollInRaft(ItemObject obj)
        {
            if (obj == gameDataObject.ConfigData.LoreItem)
            {
                ShowWindow();
            }
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            
            nameText.transform.localScale = Vector3.zero;
            image.transform.localScale = Vector3.zero;

            nameText.DOScale(1, 0.4f);
            image.rectTransform.DOScale(1, 0.4f);
            
            
            StartCoroutine(TextLoop());
        }

        IEnumerator TextLoop()
        {
            descText.text = "";

            yield return new WaitForSeconds(0.5f);
            
            for (int i = 0; i < desc.Length; i++)
            {
                descText.text += desc[i];
                yield return new WaitForSeconds(2f / desc.Length);
            }
        }
    }
}
