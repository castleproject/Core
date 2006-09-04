using NHibernate;

namespace NShop.Services
{
    public interface ISessionProvider
    {
        ISession Session { get; }
    }
}