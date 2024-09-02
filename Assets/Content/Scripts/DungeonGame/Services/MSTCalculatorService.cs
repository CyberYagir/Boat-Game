using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class MSTCalculatorService : MonoBehaviour
    {
        public class Connection
        {
            private Node nodeA;
            private Node nodeB;
            private float distance;
            private bool activeConnection;

            public Connection(Node nodeA, Node nodeB)
            {
                this.nodeA = nodeA;
                this.nodeB = nodeB;
                this.distance = Vector2.Distance(nodeA.Pos, nodeB.Pos);
            }

            public Node NodeB => nodeB;

            public Node NodeA => nodeA;

            public float Distance => distance;

            public bool ActiveConnection => activeConnection;

            public bool IsConnectNodes(Node a, Node b)
            {
                if (!ActiveConnection)
                {
                    return (nodeA == a && nodeB == b) || (nodeA == b && nodeB == a);
                }

                return false;
            }

            public void SetActiveConnection() => activeConnection = true;

            public bool IsHaveNode(Node node, List<Node> visited)
            {
                if (visited.Contains(nodeA) && visited.Contains(nodeB)) return false;
                
                if (!activeConnection)
                {
                    return (nodeA == node && nodeB != node) || (nodeA != node && nodeB == node);
                }

                return false;
            }

            public Node GetReverseNode(List<Node> visited)
            {
                if (!visited.Contains(nodeA))
                {
                    return nodeA;
                }

                return nodeB;
            }
        }
        
        public class Node
        {
            private int id;
            private Vector2Int pos;

            public Node(int id, Vector2Int pos)
            {
                this.id = id;
                this.pos = pos;
            }

            public Vector2Int Pos => pos;

            public int ID => id;
        }

        [SerializeField] private Color[] colors;
        
        private List<Node> nodes = new List<Node>();
        private List<Connection> connections = new List<Connection>();

        public List<Connection> Connections => connections;

        public List<Node> Nodes => nodes;

        public Connection FindConnection(Node a, Node b)
        {
            var connection = Connections.Find(x => x.IsConnectNodes(a, b));
            return connection;
        }

        public Node FindByPos(IPoint point, out Vector2Int pos)
        {
            var targetpos = Vector2Int.RoundToInt(new Vector2((float) point.X, (float) point.Y));
            pos = targetpos;
            return Nodes.Find(x => x.Pos == targetpos);
        }
        
        public Node FindByPos(IPoint point)
        {
            return FindByPos(point, out var n);
        }

        public Node FindByName(int id)
        {
            return Nodes.Find(x=>x.ID == id);
        }


        [Inject]
        private void Construct(TriangulationService triangulationService, DungeonService dungeonService)
        {
            int pointID = 0;
            foreach (var tris in triangulationService.Tris)
            {
                foreach (var point in tris.Points)
                {
                    if (FindByPos(point, out var pos) == null)
                    {
                        Nodes.Add(new Node(pointID, pos));
                        pointID++;
                    }
                }
            }


            foreach (var tris in triangulationService.Tris)
            {
                foreach (var point in tris.Points)
                {
                    var node = FindByPos(point);

                    foreach (var pointSecond in tris.Points)
                    {
                        var neighbourNode = FindByPos(pointSecond);

                        if (node.ID != neighbourNode.ID)
                        {
                            if (FindConnection(node, neighbourNode) == null)
                            {
                                Connections.Add(new Connection(node, neighbourNode));
                            }
                        }
                    }
                }
            }


            List<Node> visited = new List<Node>();
            List<Connection> actualConnections = new List<Connection>();

            var rnd = new System.Random(dungeonService.Seed);
            
            Node targetNode = Nodes.GetRandomItem(rnd);

            visited.Add(targetNode);

            while (visited.Count != Nodes.Count)
            {
                actualConnections.Clear();
                for (int i = 0; i < visited.Count; i++)
                {
                    var connections = this.Connections.FindAll(x => x.IsHaveNode(visited[i], visited));

                    for (int j = 0; j < connections.Count; j++)
                    {
                        if (!actualConnections.Contains(connections[j]))
                        {
                            actualConnections.Add(connections[j]);
                        }
                    }
                }

                var minimalWeight = actualConnections.Min(x => x.Distance);

                var targetConnection = actualConnections.Find(x => x.Distance == minimalWeight);

                targetConnection.SetActiveConnection();
                var nextNode = targetConnection.GetReverseNode(visited);

                visited.Add(nextNode);
            }


            var randomEdges = Mathf.RoundToInt(Connections.Count * 0.2f); //enable 20% of other edges

            var finded = Connections.FindAll(x => !x.ActiveConnection);

            for (int i = 0; i < randomEdges; i++)
            {
                finded.GetRandomItem(rnd).SetActiveConnection();
            }
        }


        private void OnDrawGizmos()
        {
            return;
            foreach (var connection in Connections)
            {
                Gizmos.color = connection.ActiveConnection ? Color.green : Color.red;
                var nodePos = connection.NodeA.Pos;
                var nextPos = connection.NodeB.Pos;

                Gizmos.DrawLine(new Vector3(nodePos.x, 1, nodePos.y), new Vector3(nextPos.x, 1, nextPos.y));
            }
        }

        public List<Connection> GetSelectedEdges()
        {
            return connections.FindAll(x=>x.ActiveConnection);
        }
    }
}
