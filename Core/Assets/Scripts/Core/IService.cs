namespace Core
{
    public interface IService
    {
        void Initialize();
        void Destroy();
    }
    
    public interface IService<T> : IService
    {
        T Config { get; set; }
    }
}
