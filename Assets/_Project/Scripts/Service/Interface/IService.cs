using System;
using UnityEngine;

namespace Zeff.Core.Service
{
    public enum Status
    {
        Success,
        Fail
    }

    public interface IService
    {
        public Status Status { get; set; }

        void Initialize()
        {
        }
    }

    public class Service<T> : IService
    {
        public static T Instance;

        public Status Status { get; set; }

        public virtual void Initialize()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = (T)(object)this;
                    Status = Status.Success;
                }
                else
                {
                    Debug.LogError($"[Service] {typeof(T)} is already initialized");
                    Status = Status.Fail;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Service] {typeof(T)} Initialize: {e}");
                Status = Status.Fail;
            }
        }
    }
}