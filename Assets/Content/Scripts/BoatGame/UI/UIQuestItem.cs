using System;
using AssetKits.ParticleImage;
using Content.Scripts.QuestsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIQuestItem : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text header;
        [SerializeField] private UITooltip tooltip;
        [SerializeField] private Image icon;
        [SerializeField] private UIBar bar;
        [SerializeField] private Button onCompleteButton;
        [SerializeField] private ParticleImage particleImage;
        
        private bool animated;
        private QuestBase questBase;
        private UIQuestsOverlay.QuestDrawer drawer;

        public bool Animated => animated;


        public void Init(QuestBase questBase, UIQuestsOverlay.QuestDrawer drawer)
        {
            this.drawer = drawer;
            this.questBase = questBase;
            questBase.OnQuestChanged += UpdateItem;
            header.text = questBase.QuestData.QuestName;
            tooltip.Init(questBase.QuestData.Description);

            icon.sprite = questBase.QuestData.Icon;
            bar.Init(string.Empty, questBase.Value, questBase.QuestData.MaxValue);
            UpdateItem(questBase);

            animated = true;
            rectTransform.anchoredPosition = new Vector2(-445, 0);
            rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.InCubic).onComplete += delegate
            {
                animated = false;
            };
        }

        private void UpdateItem(QuestBase data)
        {
            bar.UpdateBar(Mathf.Clamp(data.Value, 0, data.QuestData.MaxValue));

            if (data.IsCompleted)
            {
                bar.gameObject.SetActive(false);
                onCompleteButton.gameObject.SetActive(true);
            }
            else
            {
                bar.gameObject.SetActive(true);
                onCompleteButton.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (questBase != null)
            {
                questBase.OnQuestChanged -= UpdateItem;
            }
        }


        public void CompleteButton()
        {
            animated = true;
            particleImage.Play();
            particleImage.transform.SetParent(drawer.Holder);
            onCompleteButton.gameObject.SetActive(false);
            rectTransform.DOPunchScale(Vector3.one * 0.25f, 0.5f).onComplete += delegate
            {
                rectTransform.DOAnchorPosX(-445, 0.5f).SetEase(Ease.OutQuad).onComplete += delegate
                {
                    animated = false;
                    gameObject.SetActive(false);
                };
            };
            
            questBase.ClaimAndDispose();
        }
    }
}
