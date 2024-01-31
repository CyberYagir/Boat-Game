using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Map
{
    public class MapPathObject : ScriptableObject
    {
        public class Point
        {
            private byte r;

            private Vector3Int pos;

            public Point(byte r, Vector3Int pos)
            {
                this.r = r;
                this.pos = pos;
            }

            public Vector3Int Pos => pos;

            public int R => r;
        }
        
        [SerializeField] private Texture2D texturePath;
        [SerializeField] private List<Vector3> pathPoints;

        public Texture2D TexturePath => texturePath;
        public List<Vector3> PathPoints => pathPoints;


        [Button]
        public void CalculatePath()
        {
            List<Point> list = new List<Point>();

            
            for (int x = 0; x < texturePath.width; x++)
            {
                for (int y = 0; y < texturePath.height; y++)
                {
                    var color = (Color32)texturePath.GetPixel(x, y);
                    if (color.a != 0)
                    {
                        if (color.r > 0 && color.g == 0 && color.b == 0)
                        {
                            list.Add(new Point(color.r, new Vector3Int(x, 0, y)));
                        }
                    }
                }
            }

            list = list.OrderBy(x => x.R).Reverse().ToList();


            pathPoints.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                pathPoints.Add(list[i].Pos);
            }
        }
    }
}
