using System;

namespace Match_3
{
    public interface IBuildItem 
    {
        void Initialize();
        void SetData(IBuildData data, Action onBuySuccess = null, Action onBuyFail = null);
    }

    public interface IBuildData
    {
    }
}
