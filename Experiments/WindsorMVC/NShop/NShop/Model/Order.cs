using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NShop
{
    public class Order
    {
        Guid guid = Guid.NewGuid();
        private bool isComplete;
        private string shippingAddress;
        private string billingAddress;
        private ShippingMethod shippingMethod;
        private EntitySet<OrderLine> orderLines;
        private EntityRef<Customer> customer;


        public Order()
        {
            orderLines = new EntitySet<OrderLine>(
                delegate(OrderLine ol) { ol.Order = this; },
                delegate(OrderLine ol) { ol.Order = null; }
                );
            customer = new EntityRef<Customer>(
                delegate(Customer c) { c.Orders.Add(this); },
                delegate(Customer c) { c.Orders.Remove(this); }
                );
        }

        public Order(Customer customer, string shippingAddress, string billingAddress,
            ShippingMethod shippingMethod)
            : this()
        {
            this.customer.Value = customer;
            this.shippingAddress = shippingAddress;
            this.shippingMethod = shippingMethod;
            this.billingAddress = billingAddress;
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

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        public Customer Customer
        {
            get { return customer.Value; }
            set { customer.Value = value; }
        }

        public string ShippingAddress
        {
            get
            {
                return shippingAddress;
            }
            set
            {
                shippingAddress = value;
            }
        }

        public string BillingAddress
        {
            get
            {
                return billingAddress;
            }
            set
            {
                billingAddress = value;
            }
        }

        public ShippingMethod ShippingMethod
        {
            get
            {
                return shippingMethod;
            }
            set
            {
                shippingMethod = value;
            }
        }

        public ICollection<OrderLine> OrderLines
        {
            get { return orderLines; }
        }
    }
}
