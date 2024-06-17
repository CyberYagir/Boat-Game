using System;
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
        
        private ScenesService scenesService;

        [Inject]
        private void Construct(MapIslandCollector mapIslandCollector, ScenesService scenesService, MapSelectionService mapSelectionService, SaveDataObject saveDataObject)
        {
            this.scenesService = scenesService;
            marksContainer.Init(mapIslandCollector);
            discoversTooltip.Init(saveDataObject);
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
    }
}
