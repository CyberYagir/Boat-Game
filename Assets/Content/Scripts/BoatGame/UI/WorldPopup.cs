using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class WorldPopup : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float liveTime = 1f;
        [SerializeField] private float upHeight = 3;
        public void InitPopup(ItemObject itemObject, int value)
        {
            text.text = $"+{value}";
            icon.sprite = itemObject.ItemIcon;
            
            icon.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
        }

        public void InitPopup(Sprite sprite)
        {
            icon.sprite = sprite;
         
            icon.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
        }
        
        public void InitPopup(string str)
        {
            text.text = str;
            
            icon.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
        }

        public void Animate(Vector3 pos)
        {
            transform.DOKill();
            
            transform.position = pos;
            transform.DOMoveY(pos.y + upHeight, liveTime).SetUpdate(true).onComplete += delegate
            {
                transform.DOScale(0f, 0.2f).SetUpdate(true).onComplete += delegate
                {
                    gameObject.SetActive(false);
                };
            };
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.2f).SetUpdate(true);
        }
    }
}
