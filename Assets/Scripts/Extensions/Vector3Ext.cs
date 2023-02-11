using UnityEngine;

namespace Extensions {
    public static class Vector3Ext {
        public static Vector3 WithY(this Vector3 vector, float value) {
            return new Vector3(vector.x, value, vector.z);
        }

        public static Vector2 XZProjection(this Vector3 vector) {
            return new Vector2(vector.x, vector.z);
        }
    }
}