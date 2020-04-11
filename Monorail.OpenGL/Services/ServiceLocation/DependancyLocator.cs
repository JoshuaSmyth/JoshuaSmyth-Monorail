namespace Monorail.Framework.Services.ServiceLocation
{
    public class DependancyLocator
    {
        public static IServiceMapper ServiceMapper = new ServiceMapper();

        public static T Resolve<T>(T item) where T : IRequireServices
        {
            item.ResolveDependancies(ServiceMapper);
            return item;
        }

        public static T GetDependancy<T>() where T : class
        {
            return ServiceMapper.GetDependancy<T>();
        }

        public static void AddDepenancy<T>(T obj)
        {
            ServiceMapper.AddDepenancy(obj);
        }
        
    }
}
