using UnityEngine;

namespace Extensions {
    public static class Vector2Ext {
        public static Vector2 Rotate(this Vector2 vector2, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = vector2.x;
            float ty = vector2.y;
            vector2.x = (cos * tx) - (sin * ty);
            vector2.y = (sin * tx) + (cos * ty);
            return vector2;
        }
        
        public static Vector3 ToXZ(this Vector2 vector2) {
            return new Vector3(vector2.x, 0, vector2.y);
        }

        public static Vector3 ToXZByDirection(this Vector2 vector2, Vector2 direction) {
            Vector2 vectorByDirection = vector2.y * direction + vector2.x * direction.Rotate(-90);
            return vectorByDirection.ToXZ();
        }
    }
}