using Cysharp.Threading.Tasks;

namespace Core.Dependency
{
    public interface IAsyncInitializable
    {
        public UniTask Initialize();
    }
    
    public interface IAsyncPostInitializable
    {
        public UniTask PostInitialize();
    }
}