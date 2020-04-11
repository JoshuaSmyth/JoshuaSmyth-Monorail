namespace Monorail.Framework.Services.ServiceLocation
{
    public interface IServiceMapper
    {
        void AddDepenancy<T>(T obj);

        T GetDependancy<T>() where T : class;

        void Clear();

        void ResolveDependancies();
    }
}
