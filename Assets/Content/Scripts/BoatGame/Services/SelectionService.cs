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

    public class OverUIChecker
    {
        private bool isUIBlocked;
        private GameObject lastBlocked;
        
        private List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>(10);
        private List<RaycastResult> raycastResults = new List<RaycastResult>(10);

        public OverUIChecker(GameObject raycastersHolder)
        {
            if (raycastersHolder != null)
            {
                raycasters = raycastersHolder.GetComponentsInChildren<GraphicRaycaster>(true).ToList();
            }
        }


        public GameObject LastBlocked => lastBlocked;

        public bool IsUIBlocked => isUIBlocked;

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
                    lastBlocked = raycastResults[0].gameObject;
                    isUIBlocked = true;
                    return true;
                }
            }
            isUIBlocked = false;
            return false;
        }
    }
    public class SelectionService : MonoBehaviour, ISelectionService
    {

        [SerializeField] private new Camera camera;
        [SerializeField] private UIService uiService;
        
        
        [SerializeField, ReadOnly] private PlayerCharacter selectedCharacter;
        private ISelectable selectedObject;

        
        private Vector3 lastWorldClick;
        private OverUIChecker overUIChecker;
        
        private GameStateService gameStateService;
        private ScenesService scenesService;

        public event Action<ISelectable> OnChangeSelectObject;
        public event Action OnDoubleClick;
        public event Action<PlayerCharacter> OnChangeSelectCharacter;
        public event Action<RaftTapToBuild> OnTapOnBuildingRaft;

        public Camera Camera => camera;

        public Vector3 LastWorldClick => lastWorldClick;
        public PlayerCharacter SelectedCharacter => selectedCharacter;

        public ISelectable SelectedObject => selectedObject;

        public bool IsUIBlocked => overUIChecker.IsUIBlocked;
        public GameObject LastUIBlockedTransform => overUIChecker.LastBlocked;
        

        [Inject]
        private void Construct(GameStateService gameStateService, ScenesService scenesService)
        {
            this.scenesService = scenesService;
            this.gameStateService = gameStateService;
            
            overUIChecker = new OverUIChecker(uiService.transform.parent.gameObject);

            scenesService.OnChangeActiveScene += ScenesServiceOnOnChangeActiveScene;

            lastClickTime = Time.time;
        }

        private void ScenesServiceOnOnChangeActiveScene(ESceneName obj)
        {
            ClearSelectedObject();
        }

        private void OnDisable()
        {
            scenesService.OnChangeActiveScene -= ScenesServiceOnOnChangeActiveScene;  
        }


        private float lastClickTime;
        private Vector3 worldClickDouble;
        private Vector3 worldClickFirst;
        private bool isDoubleClick;
        public void Update()
        {
            if (InputService.IsLMBDown)
            {
                if (scenesService.GetActiveScene() == ESceneName.Map) return;
                
                if (IsUIBlocked) return;

                if (Time.time - lastClickTime < 0.35f && !isDoubleClick)
                {
                    var hit = GetClickHit(out var isHitted);
                    if (isHitted)
                    {
                        worldClickDouble = hit.point;
                        isDoubleClick = true;
                    }
                }
                else
                {
                    var hit = GetClickHit(out var isHitted);
                    if (isHitted)
                    {
                        worldClickFirst = hit.point;
                    }
                }

                lastClickTime = Time.time;
                
                switch (gameStateService.GameState)
                {
                    case GameStateService.EGameState.Normal:
                        NormalStateSelectionLogic();
                        break;
                    case GameStateService.EGameState.Building or GameStateService.EGameState.Removing:
                        BuildingStateSelectionLogic();
                        break;
                }
            }
        }

        public void LateUpdate()
        {
            if (isDoubleClick)
            {
                if (worldClickDouble.ToDistance(worldClickFirst) < 1)
                {
                    OnDoubleClick?.Invoke();
                }

                isDoubleClick = false;
            }
        }

        private void FixedUpdate()
        {
            overUIChecker.CheckUILogic();
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
            var hit = GetClickHit(out var isHit);

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
                        if (selectable.Transfered != null)
                        {
                            selectedObject = selectable.Transfered;
                        }
                        else
                        {
                            selectedObject = selectable;
                        }

                        OnChangeSelectObject?.Invoke(selectedObject);
                    }
                }
            }
        }

        private RaycastHit GetClickHit(out bool isHit)
        {
            var hit = Camera.MouseRaycast(out isHit, Input.mousePosition, Mathf.Infinity, LayerMask.GetMask("Default", "Raft", "Builds", "Water", "Trees", "Player", "Drop", "Terrain"));
            return hit;
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

        public Vector3 GetUnderMousePosition(out bool isNotEmpty)
        {
            var hit = GetClickHit(out bool isHit);
            isNotEmpty = isHit;
            return hit.point;
        }
    }
}
