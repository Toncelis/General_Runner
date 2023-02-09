using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataTypes {
    [Serializable]
    public class WeightedList <T> {
        [SerializeField]
        private List<T> objects;
        [SerializeField]
        private List<float> weights;

        public int Count => objects?.Count ?? 0;

        public void Validate(float standardWeight = 1) {
            objects ??= new List<T>();
            weights ??= new List<float>();
            int objCount = objects.Count;
            int weightsCount = weights.Count;

            for (int i = 0; i < weightsCount - objCount; i++) {
                weights.Add(standardWeight);
            }

            for (int i = 0; i < objCount - weightsCount; i++) {
                objects.RemoveAt(objects.Count-1);
            }
        }
        
        public void Add(T obj, float weight) {
            Validate();
            objects.Add(obj);
            weights.Add(weight);
        }

        public T GetRandomObject() {
            Validate();
            if (objects == null || objects.Count == 0) {
                return default;
            }
            
            float totalWeight = 0;
            foreach (var w in weights) {
                totalWeight += w;
            }

            float randomisedWeight = Random.Range(0, totalWeight);

            for (int i = 0; i < objects.Count; i++) {
                randomisedWeight -= weights[i];
                if (randomisedWeight <= 0) {
                    return objects[i];
                }
            }

            return objects.Last();
        }

        public T GetRandomObjectWithCondition(Predicate<T> condition) {
            WeightedList<T> listUnderConditions = new();
            Validate();
            for (int i = 0; i < Count; i++) {
                if (condition(objects[i])) {
                    listUnderConditions.Add(objects[i], weights[i]);
                }
            }

            return listUnderConditions.GetRandomObject();
        }
    }
}