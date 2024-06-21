using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.Map.UI;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapUIService : MonoBehaviour
    {
        [SerializeField] private UIMarksContainer marksContainer;
        [SerializeField] private UIIslandWindow islandWindow;
        [SerializeField] private UIDiscoversTooltip discoversTooltip;
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private UIMoveIslandTimer moveIslandTimer;
        
        private ScenesService scenesService;
        private MapMoverService mapMoverService;

        [Inject]
        private void Construct(
            MapIslandCollector mapIslandCollector,
            ScenesService scenesService,
            MapSelectionService mapSelectionService,
            SaveDataObject saveDataObject,
            MapMoverService mapMoverService,
            GameDataObject gamedata
        )
        {
            this.mapMoverService = mapMoverService;
            this.scenesService = scenesService;
            marksContainer.Init(mapIslandCollector, saveDataObject);
            discoversTooltip.Init(saveDataObject, messageBoxManager, this, gamedata, CrossSceneContext.GetCharactersService(), CrossSceneContext.GetResourcesService());
            moveIslandTimer.Init();
            
            scenesService.OnChangeActiveScene += OnChangeScene;
            scenesService.OnLoadOtherScene += ScenesServiceOnOnLoadOtherScene;
            scenesService.OnUnLoadOtherScene += ScenesServiceOnOnLoadOtherScene;
            OnChangeScene(scenesService.GetActiveScene());
            
            islandWindow.Init(mapSelectionService);
        }

        private void ScenesServiceOnOnLoadOtherScene(ESceneName obj)
        {
            scenesService.OnChangeActiveScene -= OnChangeScene;
            scenesService.OnLoadOtherScene -= ScenesServiceOnOnLoadOtherScene;
            scenesService.OnUnLoadOtherScene -= ScenesServiceOnOnLoadOtherScene;
        }


        private void OnChangeScene(ESceneName state)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(state == ESceneName.Map);
            }
        }

        private void LateUpdate()
        {
            marksContainer.UpdateMarks();
        }

        public void GoToIsland(int islandIslandSeed)
        {
            moveIslandTimer.Show(mapMoverService.GetTimeDistance(islandIslandSeed), delegate
            {
                mapMoverService.GoToIsland(islandIslandSeed);
                CrossSceneContext.GetSaveService().SaveWorld();
            });
        }
    }
}
