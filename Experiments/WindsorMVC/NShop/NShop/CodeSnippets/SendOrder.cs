using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NShop.Impl;
using NShop.Repositories;
using NShop.Services;

namespace NShop.CodeSnippets
{
    public class SendOrder
    {
        public void SaveOrder()
        {
            string shippingAddress = null, 
                   billingAddress = null;
            Product msdnSubscription = null;
            Customer customer = new Customer();
Order order = new Order(customer, shippingAddress, billingAddress,
            ShippingMethod.UPS);//new order
OrderLine line = new OrderLine(order, msdnSubscription, 1);//new order line
order.OrderLines.Add(line);//add order line to order

//create a session provide, provide the current database session
ISessionProvider sessionProvider = new SessionProvider();
//handles dispatching of the order to other systems
IOrdersDispatcher ordersDispatcher = new OrdersDispatcher();
//order repository contains specialized behavior that execute when
//saving an order
OrderRepository orderRepository = new OrderRepository(
        sessionProvider, ordersDispatcher);
//save the order to the database
orderRepository.Save(order);

        }
    }
}
