using System;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public partial class UICharactersListItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ActionsDataSO actionsData;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image currentActionIcon, needIcon;
        [SerializeField] private RectTransform background;
        [SerializeField] private float unactiveX, activeX;
        
        
        private PlayerCharacter targetCharacter;
        private EStateType lastState;
        private SelectionService selectionService;
        private TickService tickService;
        private CharacterService characterService;

        public PlayerCharacter TargetCharacter => targetCharacter;

        public void Init(PlayerCharacter character, TickService tickService, SelectionService selectionService, CharacterService characterService)
        {
            this.characterService = characterService;
            this.tickService = tickService;
            this.selectionService = selectionService;
            
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
        }

        private void AnimateButton(PlayerCharacter character)
        {
            background.anchoredPosition = new Vector2(
                character == targetCharacter ? activeX : unactiveX,
                background.anchoredPosition.y
            );
        }

        private void AnimateButtonAnimated(PlayerCharacter character)
        {
            background.DOAnchorPosX(character == targetCharacter ? activeX : unactiveX, 0.2f);
        }
    }
}
