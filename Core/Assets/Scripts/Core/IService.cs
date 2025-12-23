using Core.Dependency;

namespace Core
{
    public interface IService : IDependency
    {
        void Initialize();
        void Destroy();
    }
    
    public interface IService<T> : IService
    {
        T Config { get; set; }
    }
}
