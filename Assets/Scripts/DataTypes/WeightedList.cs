using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataTypes {
    [Serializable]
    public class WeightedList<T> {
        [SerializeField, LabelText("Weighted list")]
        private List<WeightedItem<T>> ItemsList = new();

        public int count => ItemsList?.Count ?? 0;

        private void Add(T obj, float weight) {
            ItemsList.Add(new (obj, weight));
        }

        private void Add(WeightedItem<T> weightedItem) {
            ItemsList.Add(weightedItem);
        }

        public T GetRandomObject() {
            if (count == 0) {
                return default;
            }

            float totalWeight = ItemsList.Sum(weightedItem => weightedItem.weight);

            float randomisedWeight = Random.Range(0, totalWeight);

            for (int i = 0; i < count; i++) {
                randomisedWeight -= ItemsList[i].weight;
                if (randomisedWeight <= 0) {
                    return ItemsList[i].item;
                }
            }

            return ItemsList.Last().item;
        }

        public bool GetRandomObjectWithCondition(Predicate<T> condition, out T obj) {
            WeightedList<T> listUnderConditions = new();
            
            for (int i = 0; i < count; i++) {
                if (condition(ItemsList[i].item)) {
                    listUnderConditions.Add(ItemsList[i]);
                }
            }

            if (listUnderConditions.count == 0) {
                obj = default;
                return false;
            }
            
            obj = listUnderConditions.GetRandomObject();
            return true;
        }
    }

    [Serializable]
    public class WeightedItem<T> {
        [HorizontalGroup]
        [SerializeField,HideLabel]
        private T Item;
        [HorizontalGroup]
        [SerializeField, HideLabel, LabelWidth(20)]
        private float Weight = 1;

        public T item => Item;
        public float weight => Weight;

        public WeightedItem(T obj, float w) {
            Item = obj;
            Weight = w;
        }
    }
}