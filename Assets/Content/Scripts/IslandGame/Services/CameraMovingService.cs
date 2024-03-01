using System;
using Content.Scripts.BoatGame.Services;
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
        
        private void LateUpdate()
        {
            if (InputService.IsRMBPressed)
            {
                var dir = -InputService.MouseAxis;

                if(dir.magnitude > 20 ) return;
                
                cameraPosition += new Vector3(dir.x, 0, dir.y) * Time.unscaledDeltaTime * cameraSpeed;
                


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

            zoom += InputService.MouseWheel * Time.unscaledDeltaTime * cameraZoomSpeed;
            zoom = Mathf.Clamp01(zoom);
            cameraSpeed = Mathf.Lerp(minCameraSpeed, maxCameraSpeed, zoom);
            
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraPosition + offcet, Time.unscaledDeltaTime * cameraSpeed);

            zoomTransform.localPosition = Vector3.Lerp(minZoomPoint.localPosition, maxZoomPoint.localPosition, zoom);
        }


        private void OnDrawGizmos()
        {
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
