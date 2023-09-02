using UnityEngine;

namespace Zeff.Core.Parser
{
    public interface IBaseParser
    {
        void Initialize();
        void LoadData(string data);
    }

    public abstract class ZBaseParser : IBaseParser
    {
        public string FileName { get; set; }

        private bool _isInitialized;

        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        public virtual void LoadData(string data)
        {
        }
    }
}