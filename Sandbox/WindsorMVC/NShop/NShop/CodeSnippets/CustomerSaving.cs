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
    public class CustomerSaving
    {
        public void SaveCustomer()
        {
            Customer customer = null;

            //create a session provide, provide the current database session
            ISessionProvider sessionProvider = new SessionProvider();
            //a generic repository that handles saving the entity to 
            // the database
            NHibernateRepository<Customer> customerRepository = 
                new NHibernateRepository<Customer>(sessionProvider);
            //save the customer to the database
            customerRepository.Save(customer);
            
        }
    }
}
