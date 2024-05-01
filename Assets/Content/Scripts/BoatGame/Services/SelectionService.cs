using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Boot;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class SelectionService : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private UIService uiService;
        
        
        [SerializeField, ReadOnly] private PlayerCharacter selectedCharacter;
        [SerializeField] private ISelectable selectedObject;

        
        private Vector3 lastWorldClick;
        private bool isUIBlocked;
        private List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>(10);
        private List<RaycastResult> raycastResults = new List<RaycastResult>(10);
        
        private GameStateService gameStateService;
        private ScenesService scenesService;

        public Action<ISelectable> OnChangeSelectObject;
        public Action<PlayerCharacter> OnChangeSelectCharacter;
        public Action<RaftTapToBuild> OnTapOnBuildingRaft;
        

        
        
        public Camera Camera => camera;

        public Vector3 LastWorldClick => lastWorldClick;
        public PlayerCharacter SelectedCharacter => selectedCharacter;

        public ISelectable SelectedObject => selectedObject;

        public bool IsUIBlocked => isUIBlocked;

        [Inject]
        private void Construct(GameStateService gameStateService, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.gameStateService = gameStateService;
            raycasters = uiService.transform.parent.GetComponentsInChildren<GraphicRaycaster>(true).ToList();
            scenesService.OnChangeActiveScene += ScenesServiceOnOnChangeActiveScene;  
        }

        private void ScenesServiceOnOnChangeActiveScene(ESceneName obj)
        {
            ClearSelectedObject();
        }


        public void Update()
        {
            if (InputService.IsLMBDown)
            {
                if (scenesService.GetActiveScene() == ESceneName.Map) return;
                
                if (isUIBlocked) return;
                
                switch (gameStateService.GameState)
                {
                    case GameStateService.EGameState.Normal:
                        NormalStateSelectionLogic();
                        break;
                    case GameStateService.EGameState.Building:
                        BuildingStateSelectionLogic();
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            CheckUILogic();
        }

        private void BuildingStateSelectionLogic()
        {
            var hit = Camera.MouseRaycast(out bool isHit, Input.mousePosition, Mathf.Infinity, LayerMask.GetMask("Builds"));

            if (isHit)
            {
                var build = hit.transform.GetComponent<RaftTapToBuild>();
                if (build)
                {
                    OnTapOnBuildingRaft?.Invoke(build);
                }
            }
        }
        private void NormalStateSelectionLogic()
        {
            var hit = Camera.MouseRaycast(out bool isHit, Input.mousePosition, Mathf.Infinity, LayerMask.GetMask("Default", "Raft", "Builds", "Water", "Trees", "Player", "Drop", "Terrain"));

            if (isHit)
            {
                lastWorldClick = hit.point;

                var character = hit.transform.GetComponent<ICharacter>();
                if (character != null)
                {
                    if ((PlayerCharacter) character != selectedCharacter)
                    {
                        ChangeCharacter(character);
                    }
                    else
                    {
                        character = null;
                    }
                }

                if (character == null)
                {
                    var selectable = hit.transform.GetComponent<ISelectable>();
                    if (selectable != null)
                    {
                        print(hit.transform.name);
                        selectedObject = selectable;
                        OnChangeSelectObject?.Invoke(selectable);
                    }
                }
            }
        }

        public bool CheckUILogic()
        {
            raycastResults.Clear();
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            for (int i = 0; i < raycasters.Count; i++)
            {
                if (raycasters[i] == null) continue;
                raycasters[i].Raycast(pointer, raycastResults);

                if (raycastResults.Count > 0)
                {
                    isUIBlocked = true;
                    return true;
                }
            }
            isUIBlocked = false;
            return false;
        }


        public void ChangeCharacter(ICharacter character)
        {
            selectedCharacter?.Select(false);
            if (character != null)
            {
                character.Select(true);
                selectedCharacter = character as PlayerCharacter;
            }
            OnChangeSelectCharacter?.Invoke(selectedCharacter);
        }

        public void ClearSelectedObject()
        {
            if (selectedObject != null)
            {
                selectedObject = null;
                OnChangeSelectObject?.Invoke(null);
            }
        }
    }
}
