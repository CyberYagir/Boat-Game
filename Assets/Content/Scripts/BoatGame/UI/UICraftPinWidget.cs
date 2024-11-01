using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftPinWidget : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private UICraftPinItem item;
        [SerializeField] private Image icon;
        private GameDataObject gameDataObject;
        private List<UICraftPinItem> items = new List<UICraftPinItem>();
        private IResourcesService resourcesService;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;


        public void Init(SaveDataObject saveDataObject, GameDataObject gameDataObject, IResourcesService resourcesService, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.saveDataObject = saveDataObject;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            saveDataObject.Global.CraftPin.OnCraftPinChanged.AddListener(UpdateCraftPin);
            UpdateCraftPin(saveDataObject.Global.CraftPin.CraftID);
            
            scenesService.OnChangeActiveScene += ScenesServiceOnOnChangeActiveScene;
        }

        private void ScenesServiceOnOnChangeActiveScene(ESceneName obj)
        {
            if (obj == ESceneName.Map)
            {
                Close();
            }
            else
            {
                UpdateCraftPin(saveDataObject.Global.CraftPin.CraftID);
            }
        }

        private void OnDestroy()
        {
            scenesService.OnChangeActiveScene -= ScenesServiceOnOnChangeActiveScene;
        }

        private void UpdateCraftPin(string craftItem)
        {
            if (!string.IsNullOrEmpty(craftItem))
            {
                var craft = gameDataObject.Crafts.Find(x => x.Uid == craftItem);
                if (craft != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        Destroy(items[i].gameObject);
                    }
                    items.Clear();
                    item.gameObject.SetActive(true);
                    for (var i = 0; i < craft.Ingredients.Count; i++)
                    {
                        var id = i;
                        Instantiate(item, item.transform.parent)
                            .With(x => x.Init(craft.Ingredients[id].Count, resourcesService, craft.Ingredients[id].ResourceName))
                            .With(x => items.Add(x));
                    }
                    item.gameObject.SetActive(false);
                    icon.sprite = craft.Icon;
                    canvas.enabled = true;
                    return;
                }
            }
            
            Close();
        }

        public void UnpinCraft()
        {
            saveDataObject.Global.CraftPin.ChangeCraftID(null);
            Close();
        }

        public void Close()
        {
            canvas.enabled = false;
        }
    }
}
