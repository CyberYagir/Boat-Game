using UnityEditor;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class CollisionSO : ScriptableObject
    {
        [SerializeField] private Vector3 center;
        [SerializeField] private float radius, height;


        public void SetData(CapsuleCollider capsuleCollider)
        {
            center = capsuleCollider.center;
            radius = capsuleCollider.radius;
            height = capsuleCollider.height;
        }
        
        public void LoadCollider(CapsuleCollider capsuleCollider, float scaleFactor)
        {
            capsuleCollider.center = center * scaleFactor;
            capsuleCollider.radius = radius * scaleFactor;
            capsuleCollider.height = height * scaleFactor;
        }
        
        
#if UNITY_EDITOR

        [MenuItem("CONTEXT/CapsuleCollider/Convert to SO")]
        public static void CreateSO(MenuCommand command)
        {
            var capsule = (command.context as CapsuleCollider);
            var treeData = capsule.GetComponent<TreeData>();
            
            var so = ScriptableObject.CreateInstance<CollisionSO>()
                .With(x => x.SetData(capsule));
            
            AssetDatabase.CreateAsset(so, "Assets/Content/Resources/IslandTrees/Collision_" + capsule.transform.name + ".asset");

            treeData.SetCollisionAsset(so);

            DestroyImmediate(capsule);
            EditorUtility.SetDirty(treeData.gameObject);
            
            AssetDatabase.SaveAssets();
        }


#endif
        
    }
}
