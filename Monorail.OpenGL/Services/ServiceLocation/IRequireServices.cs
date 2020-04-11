namespace Monorail.Framework.Services.ServiceLocation
{
    public interface IRequireServices {
        void ResolveDependancies(IServiceMapper serviceMapper);
    }
}
