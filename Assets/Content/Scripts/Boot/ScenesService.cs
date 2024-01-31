using System;
using System.Collections.Generic;
using Content.Scripts.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Scripts.Boot
{
    public class ScenesService : MonoBehaviour
    {
        [SerializeField] private Fader fader;

        private List<string> overlayLoadedScenes = new List<string>();

        public void ChangeScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public AsyncOperation ChangeSceneAsync(string name)
        {
            return SceneManager.LoadSceneAsync(name);
        }

        public void FadeScene(string name)
        {
            fader.Fade(delegate
            {
                ChangeScene(name);
            });
        }

        public AsyncOperation AddScene(string name)
        {
            if (!overlayLoadedScenes.Contains(name))
            {
                overlayLoadedScenes.Add(name);
                return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            }
            return null;
        }

        public void UnloadScene(string name)
        {
            if (overlayLoadedScenes.Contains(name))
            {
                SceneManager.UnloadSceneAsync(name);
                overlayLoadedScenes.Remove(name);
            }
        }


        public void Fade(Action action)
        {
            fader.Fade(action);
        }
    }
}
