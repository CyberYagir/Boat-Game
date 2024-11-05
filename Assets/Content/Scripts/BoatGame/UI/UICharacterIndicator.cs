using System.Collections.Generic;
using PixelPlay.OffScreenIndicator;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIScreenIndicatorBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform rectTransform;
        [SerializeField] private List<Image> backgrounds;
        protected PlayerCharacter character;
        protected Camera camera;

        public void Init(PlayerCharacter character, Camera camera, Color color)
        {
            this.camera = camera;
            this.character = character;

            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgrounds[i].color = color;
            }
        }

        public virtual void UpdateItem()
        {
            
        }
    }

    public class UICharacterIndicator : UIScreenIndicatorBase
    {
        [SerializeField] private RawImage image;


        public override void UpdateItem()
        {
            if (character == null || character.NeedManager.IsDead)
            {
                gameObject.SetActive(false);
                
                return;
            }
            image.texture = character.CharacterPreview;

            var screenPosition = OffScreenIndicatorCore.GetScreenPosition(camera, character.transform.position);

            bool isVisible = OffScreenIndicatorCore.IsTargetVisible(screenPosition);

            if (isVisible)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                float angle = 0;
                var screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
                var screenBounds = screenCentre * 0.9f;
                OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle, screenCentre, screenBounds);
                
                rectTransform.transform.eulerAngles = new Vector3(0,0, angle * Mathf.Rad2Deg);

                rectTransform.transform.position = screenPosition;
                
                image.transform.eulerAngles = Vector3.zero;
            }
        }
    }
}
