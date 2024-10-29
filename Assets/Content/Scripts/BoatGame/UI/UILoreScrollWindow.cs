using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UILoreScrollWindow : AnimatedWindow
    {
        [SerializeField] private RectTransform scrollAnimatedRect;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float minScrollScale = 150;
        [SerializeField] private float maxScrollScale = 800;
        private IResourcesService resourcesService;
        private SaveDataObject saveDataObject;
        private GameDataObject gameDataObject;

        public void Init(SelectionService selectionService, SaveDataObject saveDataObject, GameDataObject gameDataObject, IResourcesService resourcesService)
        {
            this.gameDataObject = gameDataObject;
            this.saveDataObject = saveDataObject;
            this.resourcesService = resourcesService;
            var lineID = saveDataObject.Map.GetPlotBySeed(saveDataObject.GetTargetIsland().IslandSeed);

            var lines = gameDataObject.PlotLines.LinesToList();
            if (lineID != -1)
            {
                text.text = lines[Mathf.Clamp(lineID, 0, lines.Count-1)];
            }
            else
            {
                text.text = lines.GetRandomItem();
            }

            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionObelisk>();
            furnaceAction.OnOpenWindow -= ShowWindow;
            furnaceAction.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }
        
        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            scrollAnimatedRect.DOKill();
            scrollAnimatedRect.ChangeSizeDeltaY(minScrollScale);

            scrollAnimatedRect.DOSizeDelta(new Vector2(scrollAnimatedRect.sizeDelta.x, maxScrollScale), 3f).SetDelay(0.5f);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            var plot = saveDataObject.Map.GetPlotDataBySeed(saveDataObject.GetTargetIsland().IslandSeed);
            if (plot != null)
            {
                if (!plot.Collected)
                {
                    plot.SetCollected();
                    resourcesService.AddItemsToAnyRafts(new RaftStorage.StorageItem(gameDataObject.ConfigData.LoreItem, 1));
                    saveDataObject.SaveFile();
                }
            }
        }
    }
}
