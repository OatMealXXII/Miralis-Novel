using System;
using System.Collections.Generic;
using UnityEngine;

namespace VSNL.Core
{
    /// <summary>
    /// A simple service locator registry.
    /// </summary>
    public class ServiceLocator
    {
        private readonly Dictionary<Type, IGameService> _services = new Dictionary<Type, IGameService>();

        public void Register<T>(T service) where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is already registered. Overwriting.");
            }
            _services[type] = service;
        }

        public T Get<T>() where T : class, IGameService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            
            Debug.LogError($"Service {type.Name} not found!");
            return null;
        }

        public IEnumerable<IGameService> GetAllServices()
        {
            return _services.Values;
        }

        public void Unregister<T>() where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }
    }
}
