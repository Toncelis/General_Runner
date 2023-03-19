using System.Collections.Generic;
using UnityEngine;

namespace Services {
    public class ServiceLibrary {
        private static ServiceLibrary Instance;
        private readonly Dictionary<string, Service> _services;

        public static T GetService<T>() where T : Service, new() {
            string type = typeof(T).ToString();
            if (!Instance._services.ContainsKey(type)) {
                Debug.LogError($"trying to access {type} service before it's initialisation");
                return null;
            }
            return (T)Instance._services[type];
        }

        public static void SetupService<TService>(ISourceOfServiceDependencies sourceOfDependencies) where TService : Service, new() {
            string type = typeof(TService).ToString();
            if (Instance._services.ContainsKey(type)) {
                Debug.LogWarning($"recreating {type}");
                Instance._services[type].CloseService();
                Instance._services.Remove(type);
            }
            var service = new TService();
            service.SetupService(sourceOfDependencies);
            Instance._services.Add(type, service);
        }
        
        public static ServiceLibrary InitLibrary() {
            if (Instance != null) {
                return Instance;
            }
            return new ServiceLibrary();
        }

        private ServiceLibrary() {
            Instance = this;
            _services = new();
        }

        public void ClearLibrary() {
            if (this == Instance) {
                foreach (var service in _services.Values) {
                    service.CloseService();
                }

                _services.Clear();
                Instance = null;
            }
        }
    }
}