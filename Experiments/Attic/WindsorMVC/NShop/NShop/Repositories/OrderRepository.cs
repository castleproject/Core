using NShop.Services;

namespace NShop.Repositories
{
    public class OrderRepository : NHibernateRepository<Order>
    {
        private readonly IOrdersDispatcher dispatcher;

        public OrderRepository(ISessionProvider seesionProvider, IOrdersDispatcher dispatcher)
            : base(seesionProvider)
        {
            this.dispatcher = dispatcher;
        }

        public IOrdersDispatcher OrderDispatcher
        {
            get
            {
                return dispatcher;
            }
        }

        public override void Save(Order item)
        {
            if (item.IsComplete)
                dispatcher.Dispatch(item);
            base.Save(item);
        }
    }
}