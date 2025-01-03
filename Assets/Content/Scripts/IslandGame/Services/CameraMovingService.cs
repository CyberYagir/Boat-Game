using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
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
        private SelectionService selectionService;

        [Inject]
        private void Construct(CharacterService characterService, SelectionService selectionService)
        {
            this.selectionService = selectionService;
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

        private Vector2 lastMouseVel;
        private bool isMoving;
        private void LateUpdate()
        {
            if (zoomWait) return;
            // if ((selectionService.LastUIBlockedTransform == null || selectionService.LastUIBlockedTransform.GetComponentInParent<UIActionManager>() == null)) return;
            
            if (InputService.IsRMBUp)
            {
                isMoving = false;
            }
            
            if (InputService.IsRMBDown && !isMoving)
            {
                if (!selectionService.IsUIBlocked)
                {
                    isMoving = true;
                }
            }
            
            if (CameraMove()) return;
            CameraZoom();
        }

        private void CameraZoom()
        {
            zoom += InputService.MouseWheel * TimeService.UnscaledDelta * cameraZoomSpeed;
            zoom = Mathf.Clamp01(zoom);
            cameraSpeed = Mathf.Lerp(minCameraSpeed, maxCameraSpeed, zoom);

            var nextPosXYZ = cameraPosition + offcet;
            var lerpPosXZ = Vector3.Lerp(cameraTransform.position, nextPosXYZ, TimeService.UnscaledDelta * cameraSpeed);
            var yLerp = Mathf.Lerp(cameraTransform.position.y, nextPosXYZ.y, TimeService.UnscaledDelta * 10f);
            
            
            cameraTransform.position = new Vector3(lerpPosXZ.x, yLerp, lerpPosXZ.z);

            var localZoomPos = Vector3.Lerp(minZoomPoint.localPosition, maxZoomPoint.localPosition, zoom);

            zoomTransform.localPosition = Vector3.Lerp(zoomTransform.localPosition, localZoomPos, TimeService.UnscaledDelta * 5f);
        }

        private bool CameraMove()
        {
            if ((InputService.IsRMBPressed || lastMouseVel.magnitude != 0))
            {
                if (!InputService.IsRMBPressed)
                {
                    lastMouseVel = Vector2.Lerp(lastMouseVel, Vector2.zero, 10 * Time.deltaTime);
                }
                else
                {
                    var dir = -InputService.MouseAxis;
                    lastMouseVel = dir;
                }

                if (lastMouseVel.magnitude > 20) return true;

                MoveCamera(lastMouseVel);
            }
            else if (InputService.MoveAxis.magnitude != 0 && isMoving)
            {
                MoveCamera(InputService.MoveAxis);
            }

            return false;
        }

        private void MoveCamera(Vector2 dir)
        {
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
