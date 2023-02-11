#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
 
namespace DefaultNamespace.EditorTools 
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class SerializeMesh : MonoBehaviour
    {
        [HideInInspector] [SerializeField] Vector2[] uv;
        [HideInInspector] [SerializeField] Vector3[] verticies;
        [HideInInspector] [SerializeField] int[] triangles;
        [HideInInspector] [SerializeField] bool serialized = false;
        // Use this for initialization
 
        void Awake()
        {
            if (serialized)
            {
                GetComponent<MeshFilter>().mesh = Rebuild();
                Debug.Log($"mesh rebuilt. Mesh name : {GetComponent<MeshFilter>().mesh.name}");
            }
        }
 
        void Start()
        {
            if (serialized) return;
 
            Serialize();
        }
 
        public void Serialize()
        {
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.name = "mesh" + gameObject.name;
 
            uv = mesh.uv;
            verticies = mesh.vertices;
            triangles = mesh.triangles;
 
            serialized = true;
        }
 
        public Mesh Rebuild()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = verticies;
            mesh.triangles = triangles;
            mesh.uv = uv;
           
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.name = "mesh_" + gameObject.name;
 
            return mesh;
        }
    }
 
#if UNITY_EDITOR
    [CustomEditor(typeof(SerializeMesh))]
    class SerializeMeshEditor : Editor
    {
        SerializeMesh obj;
 
        void OnSceneGUI()
        {
            obj = (SerializeMesh)target;
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
 
            if (GUILayout.Button("Rebuild"))
            {
                if (obj)
                {
                    obj.gameObject.GetComponent<MeshFilter>().mesh = obj.Rebuild();
                }
            }
 
            if (GUILayout.Button("Serialize"))
            {
                if (obj)
                {
                   obj.Serialize();
                }
            }
        }
    }
#endif
}