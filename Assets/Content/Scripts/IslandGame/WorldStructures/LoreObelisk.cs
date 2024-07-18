using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class LoreObelisk : MonoBehaviour
    {
        [SerializeField] private List<Transform> points = new List<Transform>();

        public List<Transform> Points => points;
    }
}
