using UnityEngine;

namespace Extensions {
    public static class Vector4Ext {
        public static Vector4 WithX(this Vector4 vector, float x) {
            return new Vector4(x, vector.y, vector.z, vector.w);
        }
        public static Vector4 WithY(this Vector4 vector, float y) {
            return new Vector4(vector.x, y, vector.z, vector.w);
        }
        public static Vector4 WithZ(this Vector4 vector, float z) {
            return new Vector4(vector.x, vector.y, z, vector.w);
        }
        public static Vector4 WithW(this Vector4 vector, float w) {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }
    }
}