using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NShop
{
    public class OrderLine
    {
        Guid guid = Guid.NewGuid();
        private EntityRef<Order> order;
        private decimal cost;
        private int amount;
        private EntityRef<Product> product;

        public OrderLine()
        {
            product = new EntityRef<Product>();
            order = new EntityRef<Order>(
                delegate(Order o) { o.OrderLines.Add(this); },
                delegate(Order o) { o.OrderLines.Remove(this); }
                );
        }

        public OrderLine(Order order, Product product, int amount) : this()
        {
            this.order.Value = order;
            this.product.Value = product;
            this.cost = product.Cost;
            this.amount = amount;
        }

        public Guid Id
        {
            get
            {
                return guid;
            }
            set
            {
                guid = value;
            }
        }

        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }

        public Order Order
        {
            get
            {
                return order.Value;
            }
            set
            {
                order.Value = value;
            }
        }

        public Product Product
        {
            get
            {
                return product.Value;
            }
            set
            {
                product.Value = value;
            }
        }
    }
}
