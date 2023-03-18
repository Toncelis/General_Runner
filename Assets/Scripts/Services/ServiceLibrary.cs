using System.Collections.Generic;
using UnityEngine;

namespace Services {
    public class ServiceLibrary : MonoBehaviour {
        private static ServiceLibrary Instance;
        private Dictionary<string, Service> _services;

        public static T GetService<T>() where T : Service, new() {
            string type = typeof(T).ToString();
            if (!Instance._services.ContainsKey(type)) {
                Instance.SetupService<T>(type);
            }
            return (T)Instance._services[type];
        }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            }

            Instance = this;
        }

        private void Start() {
            SetupServices();
        }

        private void SetupServices() {
            _services = new();
            GetService<PositionService>();
        }

        private void SetupService<T>(string type) where T : Service, new() {
            var service = new T();
            _services.Add(type, service);
            service.SetupService();
        }

        private void OnDestroy() {
            if (this == Instance) {
                foreach (var service in _services.Values) {
                    service.CloseService();
                }

                _services.Clear();
            }
        }
    }
}