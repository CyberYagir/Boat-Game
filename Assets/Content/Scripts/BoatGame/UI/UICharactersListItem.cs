using System;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public partial class UICharactersListItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,  IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ActionsDataSO actionsData;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image currentActionIcon, needIcon;
        [SerializeField] private RectTransform background;
        [SerializeField] private UIBar healthBar;
        [SerializeField] private float unactiveX, activeX;
        [SerializeField] private RawImage rawImage;

        private bool isOver;
        private RenderTexture renderTexture;
        
        private PlayerCharacter targetCharacter;
        private EStateType lastState;
        private SelectionService selectionService;
        private TickService tickService;
        private CharacterService characterService;
        private Camera camera;
        private PlayerCharacter character;

        public PlayerCharacter TargetCharacter => targetCharacter;

        public bool IsOver => isOver;

        public void Init(PlayerCharacter character, TickService tickService, SelectionService selectionService, CharacterService characterService, Camera camera)
        {
            this.character = character;
            this.camera = camera;
            this.characterService = characterService;
            this.tickService = tickService;
            this.selectionService = selectionService;

            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(128, 128, 8, GraphicsFormat.R8G8B8A8_UNorm);
                rawImage.texture = renderTexture;
                character.SaveCharacterPreviewRenderTexture(renderTexture);
            }
            
            healthBar.Init("Health", character.NeedManager.Health, 100f);
            
            tickService.OnTick -= TickServiceOnOnTick;
            tickService.OnTick += TickServiceOnOnTick;

            selectionService.OnChangeSelectCharacter -= AnimateButtonAnimated;
            selectionService.OnChangeSelectCharacter += AnimateButtonAnimated;

            targetCharacter = character;

            text.text = targetCharacter.Character.Name;
            
            AnimateButton(selectionService.SelectedCharacter);
            
            
            currentActionIcon.sprite = actionsData.GetActionIcon(targetCharacter.CurrentState);
            lastState = targetCharacter.CurrentState;
            
        }

        private void OnDisable()
        {
            if (selectionService == null || tickService == null) return;
            
            selectionService.OnChangeSelectCharacter -= AnimateButton;
            tickService.OnTick -= TickServiceOnOnTick;
        }


        private void TickServiceOnOnTick(float delta)
        {
            if (targetCharacter == null) return;

            if (targetCharacter.CurrentState != lastState)
            {
                currentActionIcon.sprite = actionsData.GetActionIcon(targetCharacter.CurrentState);
                lastState = targetCharacter.CurrentState;
            }

            var sprite = targetCharacter.NeedManager.GetCurrentIcons(out bool isActive);
            needIcon.gameObject.SetActive(isActive);
            if (isActive)
            {
                needIcon.sprite = sprite;
            }

            var bone = targetCharacter.AppearanceDataManager.GetBone(PlayerCharacter.AppearanceManager.EBones.Spine2);

            camera.transform.position = bone.transform.position;
            camera.transform.rotation = bone.transform.rotation;
            camera.transform.Translate(Vector3.forward * 0.8f + Vector3.up/2f, Space.Self);
            camera.transform.Rotate(Vector3.up * 180, Space.Self);

            camera.targetTexture = renderTexture;
            camera.Render();


            healthBar.UpdateBar(TargetCharacter.NeedManager.Health);
        }

        private void AnimateButton(PlayerCharacter character)
        {
            background.anchoredPosition = new Vector2(
                background.anchoredPosition.x,
                character == targetCharacter ? activeX : unactiveX
            );
        }

        private void AnimateButtonAnimated(PlayerCharacter character)
        {
            background.DOAnchorPosY(character == targetCharacter ? activeX : unactiveX, 0.2f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOver = false;
        }
    }
}
