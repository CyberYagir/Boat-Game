using System;
using Content.Scripts.Boot;
using Content.Scripts.Map.UI;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapUIService : MonoBehaviour
    {
        [SerializeField] private UIMarksContainer marksContainer;
        [SerializeField] private UIIslandWindow islandWindow;
        
        [Inject]
        private void Construct(MapIslandCollector mapIslandCollector, ScenesService scenesService, MapSelectionService mapSelectionService)
        {
            marksContainer.Init(mapIslandCollector);
            scenesService.OnChangeActiveScene += OnChangeScene;
            OnChangeScene(scenesService.GetActiveScene());

            islandWindow.Init(mapSelectionService);
        }

        private void OnChangeScene(ESceneName state)
        {
            gameObject.SetActive(state == ESceneName.Map);
        }

        private void LateUpdate()
        {
            marksContainer.UpdateMarks();
        }
    }
}
