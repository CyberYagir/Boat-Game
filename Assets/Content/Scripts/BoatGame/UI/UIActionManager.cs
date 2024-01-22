using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class UIActionManager : MonoBehaviour
    {
        [System.Serializable]
        public class ActionButton
        {
            [SerializeField] private Transform transform;
            [SerializeField] private Image icon;
            [SerializeField] private Button button;
            [SerializeField] private Image cancelState;

            public Image Icon => icon;

            public Transform Transform => transform;

            public Button Button => button;

            public Image CancelState => cancelState;
        }

        [SerializeField] private GameObject holder;
        [SerializeField] private List<ActionButton> buttons;

        private List<Vector3> localButtonsPoses = new List<Vector3>();

        private SelectionService selectionService;

        private Vector3 localPositionClick;
        private Transform clickedItem;
        private ISelectable selectable;

        public void Init(SelectionService selectionService)
        {
            holder.SetActive(false);
            this.selectionService = selectionService;
            selectionService.OnChangeSelectObject += OnChangeSelectObject;
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;

            for (int i = 0; i < buttons.Count; i++)
            {
                localButtonsPoses.Add(buttons[i].Transform.localPosition);
            }
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            holder.gameObject.SetActive(false);
        }

        private void OnChangeSelectObject(ISelectable selectable)
        {
            holder.gameObject.SetActive(selectable != null);
            if (selectable == null) return;
            if (selectable.PlayerActions.Count == 0) return;
            
            this.selectable = selectable;

            UpdateButtons(true);

            holder.transform.position = GetWorldToScreenPoint();

            clickedItem = selectable.Transform;
            localPositionClick = clickedItem.InverseTransformPoint(selectionService.LastWorldClick);
        }

        public void UpdateButtons(bool animated)
        {
            if (holder.active && selectable != null)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Transform.gameObject.SetActive(selectable.PlayerActions.Count > i);
                    if (animated)
                    {
                        buttons[i].Transform.localPosition = Vector3.zero;
                        buttons[i].Transform.localScale = Vector3.zero;
                        buttons[i].Transform.DOLocalMove(localButtonsPoses[i], 0.2f);
                        buttons[i].Transform.DOScale(Vector3.one, 0.2f);
                    }

                    if (selectable.PlayerActions.Count > i)
                    {
                        if (!selectable.PlayerActions[i].IsSelectedCharacterOnThisAction())
                        {
                            buttons[i].CancelState.gameObject.SetActive(false);
                            buttons[i].Icon.gameObject.SetActive(true);


                            buttons[i].Button.onClick.RemoveAllListeners();
                            buttons[i].Button.onClick.AddListener(selectable.PlayerActions[i].Action);
                            buttons[i].Button.onClick.AddListener(delegate { holder.gameObject.SetActive(false); });

                            buttons[i].Icon.sprite = selectable.PlayerActions[i].Icon;
                            var canShow = selectable.PlayerActions[i].IsCanShow();
                            buttons[i].Icon.SetAlpha(canShow ? 1f : 0.35f);
                            buttons[i].Button.enabled = canShow;
                        }
                        else
                        {
                            buttons[i].CancelState.gameObject.SetActive(true);
                            buttons[i].Icon.gameObject.SetActive(false);

                            buttons[i].Button.onClick.RemoveAllListeners();
                            buttons[i].Button.onClick.AddListener(selectable.PlayerActions[i].BreakAction);
                            buttons[i].Button.onClick.AddListener(delegate { holder.gameObject.SetActive(false); });


                            buttons[i].CancelState.SetAlpha(selectable.PlayerActions[i].IsCanCancel() ? 1f : 0.35f);

                            buttons[i].Button.enabled = true;
                        }
                    }
                }
            }
        }

        private Vector3 GetWorldToScreenPoint()
        {
            return selectionService.Camera.WorldToScreenPoint(selectionService.LastWorldClick);
        }

        public void Update()
        {
            if (clickedItem != null)
            {
                holder.transform.position = selectionService.Camera.WorldToScreenPoint(clickedItem.TransformPoint(localPositionClick));
            }

            if (holder.active && selectable != null)
            {
                if (selectionService.SelectedCharacter != null && selectionService.SelectedCharacter.CurrentState != EStateType.Idle)
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        if (selectable.PlayerActions.Count > i)
                        {
                            if (selectable.PlayerActions[i].IsSelectedCharacterOnThisAction())
                            {
                                buttons[i].CancelState.SetAlpha(selectable.PlayerActions[i].IsCanCancel() ? 1f : 0.35f);
                            }
                        }
                    }
                }
            }
        }
    }
}