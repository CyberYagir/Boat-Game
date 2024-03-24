using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class RaftLadderToIsland : MonoBehaviour
    {
        [SerializeField] private TrapItem item;

        public void Init(Vector3 startPoint, Vector3 endPoint)
        {
            var dst = Mathf.CeilToInt(Vector3.Distance(startPoint, endPoint));

            item.gameObject.SetActive(true);
            for (int i = 0; i < dst; i++)
            {
                var z = i;
                Instantiate(item, transform)
                    .With(x => x.transform.localPosition = new Vector3(0, 0, z))
                    .With(x => x.Init(z));

            }

            transform.position = startPoint;
            transform.LookAt(endPoint);            
            item.gameObject.SetActive(false);

        }
    }
}
