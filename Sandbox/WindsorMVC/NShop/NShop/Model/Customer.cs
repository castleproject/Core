using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NShop
{
    public class Customer
    {
        Guid guid = Guid.NewGuid();
        private string name;
        private string email;
        private EntitySet<Order> orders;

        public Customer()
        {
            orders = new EntitySet<Order>(
                delegate(Order o) { o.Customer = this; },
                delegate(Order o) { o.Customer = null; }
                );
        }

        public Customer(string name, string email) : this()
        {
            this.name = name;
            this.email = email;
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

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        public ICollection<Order> Orders
        {
            get
            {
                return orders;
            }
        }
    }
}
