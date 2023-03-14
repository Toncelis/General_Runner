﻿using UnityEngine;

namespace Extensions {
    public static class GameObjectExt {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            if (component == null) {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        
        public static T RemakeComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            if (component != null) {
                Object.DestroyImmediate(component);
            }
            component = gameObject.AddComponent<T>();
            return component;
        }
    }
}