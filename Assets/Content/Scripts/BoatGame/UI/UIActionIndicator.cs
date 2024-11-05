using Content.Scripts.BoatGame.Characters;
using PixelPlay.OffScreenIndicator;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIActionIndicator : UIScreenIndicatorBase
    {
        [SerializeField] private Image circle, arrow;
        [SerializeField] private Image image;
        [SerializeField] private ActionsDataSO actionsDataSo;

        public override void UpdateItem()
        {
            if (character == null || character.NeedManager.IsDead)
            {
                gameObject.SetActive(false);
                
                return;
            }

            if (character.CurrentState == EStateType.Idle)
            {
                gameObject.SetActive(false);
                return;
            }
            
            image.sprite = actionsDataSo.GetActionIcon(character.CurrentState);

            var screenPosition = OffScreenIndicatorCore.GetScreenPosition(camera, character.AIMoveManager.NavMeshAgent.ArrivePoint);

            bool isVisible = OffScreenIndicatorCore.IsTargetVisible(screenPosition);
            
            gameObject.SetActive(true);
            
            
            float angle = 0;
            var screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
            var screenBounds = screenCentre * 0.9f;
            
            if (isVisible)
            {

                if (character.AIMoveManager.NavMeshAgent.ArrivePoint.ToDistance(character.transform.position) > 5)
                {
                    arrow.gameObject.SetActive(false);
                    circle.gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                
                arrow.gameObject.SetActive(true);
                circle.gameObject.SetActive(false);
                
                OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle, screenCentre, screenBounds);
                rectTransform.transform.eulerAngles = new Vector3(0,0, angle * Mathf.Rad2Deg);
            }
            
            
           

            rectTransform.transform.position = screenPosition;
                
            image.transform.eulerAngles = Vector3.zero;
        }
    }
}
