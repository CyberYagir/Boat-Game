using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class ResourcesCounter : MonoBehaviour
    {
        [SerializeField] private EResourceTypes resourceType;
        [SerializeField] private RectTransform mask;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float fullSize = 256;

        public EResourceTypes ResourceTypes => resourceType;

        public void UpdateCounter(int count, int maxCount)
        {
            mask.ChangeSizeDeltaX(count / (float) maxCount * fullSize);
            text.text = count + "/" + maxCount;
        }
    }
}
