using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIMarksContainer : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private List<RectTransform> marksPool;

        private Dictionary<RectTransform, bool> isAnimated = new(10);

        private MapIslandCollector mapIslandCollector;

        public void Init(MapIslandCollector mapIslandCollector)
        {
            this.mapIslandCollector = mapIslandCollector;

            for (int i = 0; i < marksPool.Count; i++)
            {
                marksPool[i].gameObject.SetActive(false);
            }
        }

        public void UpdateMarks()
        {
            int islandID = 0;
            for (int i = 0; i < marksPool.Count; i++)
            {
                var target = marksPool[i];
                
                if (islandID < mapIslandCollector.IslandsInRadius.Count)
                {
                    if (!target.gameObject.active)
                    {
                        if (!isAnimated.ContainsKey(marksPool[i]))
                        {
                            isAnimated.Add(target, true);
                            
                            target.gameObject.SetActive(true);
                            target.localScale = Vector3.zero;
                            
                            target.DOScale(Vector3.one, 0.2f)
                                .SetLink(gameObject).
                                SetUpdate(UpdateType.Late)
                                .onComplete += () => { isAnimated.Remove(target); };
                        }
                    }
                    target.GetComponent<UIMark>().Init(mapIslandCollector.IslandsInRadius[islandID].GeneratedData);
                    islandID++;
                }
                else
                {
                    if (!isAnimated.ContainsKey(marksPool[i]))
                    {
                        if (marksPool[i].gameObject.active)
                        {
                            isAnimated.Add(target, true);
                            target.DOScale(Vector3.zero, 0.2f)
                                .SetLink(gameObject)
                                .SetUpdate(UpdateType.Late)
                                .onComplete += (() =>
                            {
                                target.gameObject.SetActive(false);
                                isAnimated.Remove(target);
                            });
                            marksPool.Add(target);
                            marksPool.RemoveAt(i);
                        }
                    }
                }
            }

            for (var i = 0; i < mapIslandCollector.IslandsInRadius.Count; i++)
            {
                if (i < marksPool.Count)
                {
                    var rectTransform = marksPool[i];
                    rectTransform.transform.position = camera.WorldToScreenPoint(mapIslandCollector.IslandsInRadius[i].transform.position);
                }
            }
        }
    }
}
