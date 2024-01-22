using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class HatsHolder : MonoBehaviour
    {
        [SerializeField] private List<GameObject> list;


        public int HatsCount => list.Count;

        public void ShowHat(int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(i == id);
            }
        }

        public void Disable()
        {
            ShowHat(-1);
        }
    }
}
