using System;
using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    [RequireComponent(typeof(RaftBuild))]
    public class RaftBuildVisuals : MonoBehaviour
    {
        [SerializeField] private List<GameObject> parts;


        private void Awake()
        {
            var data = GetComponent<RaftBuild>();
            data.OnChangeProgress += OnOnChangeProgress;
            
            OnOnChangeProgress(data.Progress);
        }

        private void OnOnChangeProgress(float progress)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                var value = (i / (float) parts.Count) * 100;
                parts[i].gameObject.SetActive(value <= progress);
            }
        }
    }
}
