using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures.Productor
{
    public class ConveerItem : MonoBehaviour
    {
        [SerializeField] private List<GameObject> level = new List<GameObject>();
        private int l = 0;

        public void UpgradeLevel()
        {
            if (l == level.Count - 1) return;
            
            l++;

            for (int i = 0; i < level.Count; i++)
            {
                level[i].gameObject.SetActive(i == l);
            }
        }

        public void Activate()
        {
            gameObject.AddComponent<Rigidbody>()
                .With(y => y.AddForce(Random.insideUnitSphere, ForceMode.Impulse))
                .With(y => y.AddTorque(Random.insideUnitSphere, ForceMode.Impulse));

            GetComponentInChildren<Collider>(true).enabled = true;
        }
    }
}
