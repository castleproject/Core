using System;
using System.Collections.Generic;
using System.Text;

namespace NShop
{
    public class Product
    {
        Guid guid = Guid.NewGuid();
        private string name;
        private decimal cost;
        private string description;

        public Product()
        {

        }

        public Product(string name, string description, decimal cost)
        {
            this.name = name;
            this.description = description;
            this.cost = cost;
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

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
    }
}
