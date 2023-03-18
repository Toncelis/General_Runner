using System;
using System.Collections;
using DataTypes;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.World.View {
    public class CollectableView : MonoBehaviour {
        [Title("Settings")]
        [SerializeField] public CollectableData Data;
        [SerializeField] private float ShrinkSpeed;
        
        [PropertySpace(4), FoldoutGroup("Internal dependencies")]
        [SerializeField] private Collider Collider;
        [FoldoutGroup("Internal dependencies")]
        [SerializeField] public GameObject UnlockedObject;
        [FoldoutGroup("Internal dependencies")]
        [SerializeField] public GameObject LockedObject;

        private bool _locked;

        private CollectableTypesEnum type => Data.type;

        private CollectablesService collectablesService => ServiceLibrary.GetService<CollectablesService>();

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                collectablesService.PickUpCollectable(type);
                StartCoroutine(DestructionRoutine());
            }
        }

        private void Start() {
            _locked = collectablesService.IsCollectableLocked(type);

            UpdateActiveObject();
            Data.lockedSignal.RegisterResponse(OnLockedSignal);
            Data.unlockedSignal.RegisterResponse(OnUnlockedSignal);
        }

        private void UpdateActiveObject() {
            UnlockedObject.SetActive(!_locked);
            LockedObject.SetActive(_locked);
        }

        private void OnLockedSignal(CollectableTypesEnum lockedType) {
            if (type == lockedType && !_locked) {
                _locked = true;
                UpdateActiveObject();
            }
        }

        private void OnUnlockedSignal(CollectableTypesEnum unlockedType) {
            if (type == unlockedType && _locked) {
                _locked = false;
                UpdateActiveObject();
            }
        }

        private IEnumerator DestructionRoutine() {
            Collider.enabled = false;
            while (transform.localScale.x > 0) {
                transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnDestroy() {
            Data.lockedSignal.UnregisterResponse(OnLockedSignal);
            Data.unlockedSignal.UnregisterResponse(OnUnlockedSignal);
        }
    }
}