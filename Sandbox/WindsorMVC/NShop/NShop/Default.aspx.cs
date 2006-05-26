using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NHibernate;
using NShop.Impl;
using NShop.Views;

namespace NShop
{
    public partial class Default : System.Web.UI.Page, IView
    {
        private IDictionary<string, object> items = new Dictionary<string, object>();

        protected void Page_Load(object sender, EventArgs e)
        {
            BindingList<Customer> bindingList = new BindingList<Customer>((IList<Customer>)items["Customers"]);
            this.GridView1.DataSource = bindingList;
            this.GridView1.DataBind();
        }

        public IDictionary<string, object> Items
        {
            get { return items; }
            set { items = value; }
        }
    }
}
