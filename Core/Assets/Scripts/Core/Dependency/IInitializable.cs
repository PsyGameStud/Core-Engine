namespace Core.Dependency
{
    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IPostInitializable
    {
        public void PostInitialize();
    }
}