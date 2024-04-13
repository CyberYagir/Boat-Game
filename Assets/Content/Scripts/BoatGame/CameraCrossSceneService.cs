using System;
using System.Collections.Generic;
using Content.Scripts.Boot;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class CameraCrossSceneService : MonoBehaviour
    {
        [SerializeField] private List<Camera> camera;

        private Dictionary<Camera, bool> cameraStates = new();

        private bool currentState = true;
        private ScenesService scenesService;


        [Inject]
        private void Construct(ScenesService scenesService)
        {
            this.scenesService = scenesService;
            scenesService.OnChangeActiveScene -= ScenesServiceOnOnChangeActiveScene;
            scenesService.OnChangeActiveScene += ScenesServiceOnOnChangeActiveScene;
            ScenesServiceOnOnChangeActiveScene(scenesService.GetActiveScene());
        }

        private void OnDisable()
        {
            scenesService.OnChangeActiveScene -= ScenesServiceOnOnChangeActiveScene;
        }

        private void ScenesServiceOnOnChangeActiveScene(ESceneName newScene)
        {
            var state = newScene.ToString() == gameObject.scene.name;
            
            if (state != currentState)
            {
                if (state == false)
                {
                    cameraStates.Clear();
                    for (int i = 0; i < camera.Count; i++)
                    {
                        cameraStates.Add(camera[i], camera[i].enabled);
                        camera[i].enabled = false;
                    }
                }
                else
                {
                    foreach (var st in cameraStates)
                    {
                        st.Key.enabled = st.Value;
                    }
                }
            }

            currentState = state;
        }
    }
}
