using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Services
{
    public class CameraMovingService : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform zoomTransform;
        [SerializeField] private float cameraSpeed, minCameraSpeed,  maxCameraSpeed, cameraZoomSpeed;
        [SerializeField] private Vector3 offcet;

        [SerializeField] private Transform minZoomPoint, maxZoomPoint;

        [SerializeField] private LayerMask mask;

        private float zoom = 0.2f;
        private Vector3 cameraPosition;

        private bool zoomWait;
        
        [Inject]
        private void Construct(CharacterService characterService)
        {
            characterService.OnCharactersFocus += OnFocusCharacter; 
        }

        private void OnFocusCharacter(PlayerCharacter obj)
        {
            if (zoomWait) return;
            if (cameraPosition.ToDistance(obj.transform.position) < 5) return;
            
            zoomWait = true;

            cameraPosition = obj.transform.position;
            cameraTransform.transform.DOMove(obj.transform.position + offcet, 1f)
                .onComplete += () => zoomWait = false;
        }

        private void LateUpdate()
        {
            if (zoomWait) return;
            if (InputService.IsRMBPressed)
            {
                var dir = -InputService.MouseAxis;

                if(dir.magnitude > 20 ) return;
                
                cameraPosition += new Vector3(dir.x, 0, dir.y) * TimeService.UnscaledDelta * cameraSpeed;
                


                if (Physics.Raycast(cameraPosition + Vector3.up * 300, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
                {
                    var y = hit.point.y;
                    if (y < 0)
                    {
                        y = 0;
                    }

                    cameraPosition.y = y;
                }
            }

            zoom += InputService.MouseWheel * TimeService.UnscaledDelta * cameraZoomSpeed;
            zoom = Mathf.Clamp01(zoom);
            cameraSpeed = Mathf.Lerp(minCameraSpeed, maxCameraSpeed, zoom);
            
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraPosition + offcet, TimeService.UnscaledDelta * cameraSpeed);

            var localZoomPos = Vector3.Lerp(minZoomPoint.localPosition, maxZoomPoint.localPosition, zoom);
            
            zoomTransform.localPosition = Vector3.Lerp(zoomTransform.localPosition, localZoomPos, TimeService.UnscaledDelta * 5f);
        }


        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            Gizmos.DrawSphere(cameraPosition, 0.5f);
            Gizmos.DrawLine(minZoomPoint.position, maxZoomPoint.position);
        }

        public void SetStartPosition(Vector3 pointPosition)
        {
            cameraPosition = pointPosition;

            cameraTransform.position = pointPosition + offcet;
        }
    }
}