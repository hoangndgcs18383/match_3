using System.Collections.Generic;
using UnityEngine;

namespace Zeff.Core.Service
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        private Dictionary<string, IService> _servicesLocators;

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }

                return _instance;
            }
        }

        public void RegisterService(string key, IService service)
        {
            if (_servicesLocators == null)
            {
                _servicesLocators = new Dictionary<string, IService>();
            }

            if (!_servicesLocators.ContainsKey(key))
            {
                _servicesLocators.Add(key, service);
            }
        }

        public  void LoadService()
        {
            foreach (var service in _servicesLocators)
            {
                service.Value.Initialize();
                
                if (service.Value.Status == Status.Success)
                {
                    Debug.Log("[Service] " + service.Key + " is loaded");
                }
                else
                {
                    break;
                }
            }
        }

        public T GetService<T>(string key) where T : IService
        {
            if (_servicesLocators.ContainsKey(key))
            {
                return (T) _servicesLocators[key];
            }

            return default(T);
        }
    }
}