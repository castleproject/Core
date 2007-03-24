using System;
using System.Collections.Generic;
using System.Text;

namespace NShop.Services
{
    public interface IOrdersDispatcher
    {
        void Dispatch(Order order);
    }
}
