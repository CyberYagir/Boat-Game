using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TrapItem : MonoBehaviour
    {
        [SerializeField] private GameObject[] colliders;

        public void Init(int i)
        {
            
            if (i <= 2)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    colliders[j].SetActive(false);
                }
            }

            if (i <= 0.5f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
