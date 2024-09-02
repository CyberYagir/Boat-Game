using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class NoneTile : MonoBehaviour
    {
        [SerializeField] private PropsSelector[] props;


        public void Init(System.Random rnd)
        {
            foreach (var propsSelector in props)
            {
                propsSelector.Init(rnd);
            }
        }
    }
}
