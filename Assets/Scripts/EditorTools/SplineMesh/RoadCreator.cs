using DefaultNamespace.World.View;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.EditorTools.SplineMesh {
    public class RoadCreator : SplineMeshCreator {
        public float roadWidth = 12;
        public float wallWidth = 2;
        public float wallHeight = 10;

        public Material roadMaterial;
        public Material wallMaterial;
        
        [Button]
        public void GenerateRoadWithWalls() {
            float roadHalfWidth = roadWidth/2;
            var road = GenerateChildMesh("Road", -roadHalfWidth, roadHalfWidth, -2, 0, true);
            road.tag = "ground";
            road.RemakeComponent<MeshCollider>();
            road.GetOrAddComponent<MeshRenderer>().material = roadMaterial;
            var leftWall = GenerateChildMesh("LeftWall", -wallWidth-roadHalfWidth, -roadHalfWidth, -2, wallHeight, true);
            leftWall.tag = "Wall";
            leftWall.RemakeComponent<MeshCollider>();
            leftWall.GetOrAddComponent<MeshRenderer>().material = wallMaterial;;
            var rightWall = GenerateChildMesh("RightWall", roadHalfWidth, wallWidth+roadHalfWidth, -2, wallHeight, true);
            rightWall.tag = "Wall";
            rightWall.RemakeComponent<MeshCollider>();
            rightWall.GetOrAddComponent<MeshRenderer>().material = wallMaterial;;

            var tileView = gameObject.GetOrAddComponent<TileView>();
            tileView.SetSpline(spline);
            tileView.Density = Density;
            
            DestroyImmediate(this);
        }
    }
}