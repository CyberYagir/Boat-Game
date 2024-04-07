using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Content.Scripts.BoatGame.UI
{
    public partial class UICharactersListItem
    {
        int clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            clicked++;
 
            if (clicked == 1)
                clicktime = Time.time;
 
            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                // Double click detected
                clicked = 0;
                clicktime = 0;
                DoubleClick();
                return;
            }
            else if (clicked > 2 || Time.time - clicktime > 1)
                clicked = 0;
            
            
            OneClick();
        }

        private void DoubleClick()
        {
            characterService.FocusTo(targetCharacter);
        }

        private void OneClick()
        {
            selectionService.ChangeCharacter(targetCharacter);
            background.localScale = Vector3.one;
            background.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}