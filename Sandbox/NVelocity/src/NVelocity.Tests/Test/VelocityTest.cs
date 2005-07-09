using System;
using NUnit.Framework;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text;

//using Spring2.Core.Types;

//using VelocityNET.App;
//using VelocityNET.Context;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;

namespace NVelocity.Test {

    /// <summary>
    /// Test Velocity processing
    /// </summary>
    [TestFixture]
    public class VelocityTest {

	[Test]
	public void Test_Evaluate() {
	    VelocityContext c = new VelocityContext();
	    c.Put("key", "value");
	    c.Put("firstName", "Cort");
	    c.Put("lastName", "Schaefer");
	    Hashtable h = new Hashtable();
	    h.Add("foo", "bar");
	    c.Put("hashtable", h);

	    AddressData address = new AddressData();
	    address.Address1 ="9339 Grand Teton Drive";
	    address.Address2 = "Office in the back";
	    c.Put("address", address);

	    ContactData contact = new ContactData();
	    contact.Name = "Cort";
	    contact.Address = address;
	    c.Put("contact", contact);

	    // test simple objects (no nesting)
	    StringWriter sw = new StringWriter();
	    Boolean ok = Velocity.Evaluate(c, sw, "", "$firstName is my first name, my last name is $lastName");
	    Assertion.Assert("Evalutation returned failure", ok);
	    String s = sw.ToString();
	    Assertion.Assert("test simple objects (no nesting)", s.Equals("Cort is my first name, my last name is Schaefer"));

	    // test nested object
	    sw = new StringWriter();
	    String template = "These are the individual properties:\naddr1=9339 Grand Teton Drive\naddr2=Office in the back";
	    ok = Velocity.Evaluate(c, sw, "", template)
	    ;
	    Assertion.Assert("Evalutation returned failure", ok);
	    s = sw.ToString();
	    Assertion.Assert("test nested object", !s.Equals(String.Empty));

	    // test hashtable
	    //	    sw = new StringWriter();
	    //	    template = "Hashtable lookup: foo=$hashtable.foo";
	    //	    ok = Velocity.Evaluate(c, sw, "", template);
	    //	    Assert("Evalutation returned failure", ok);
	    //	    s = sw.ToString();
	    //	    Assert("Evaluation did not evaluate right", s.Equals("Hashtable lookup: foo=bar"));

	    // test nested properties
	    //    	    sw = new StringWriter();
	    //	    template = "These are the nested properties:\naddr1=$contact.Address.Address1\naddr2=$contact.Address.Address2";
	    //	    ok = Velocity.Evaluate(c, sw, "", template);
	    //	    Assert("Evalutation returned failure", ok);
	    //	    s = sw.ToString();
	    //	    Assert("test nested properties", s.Equals("These are the nested properties:\naddr1=9339 Grand Teton Drive\naddr2=Office in the back"));

	    // test key not found in context
	    sw = new StringWriter();
	    template = "$!NOT_IN_CONTEXT";
	    ok = Velocity.Evaluate(c, sw, "", template)
	    ;
	    Assertion.Assert("Evalutation returned failure", ok);
	    s = sw.ToString();
	    Assertion.Assert("test key not found in context", s.Equals(String.Empty));

	    // test nested properties where property not found
	    //	    sw = new StringWriter();
	    //	    template = "These are the non-existent nested properties:\naddr1=$contact.Address.Address1.Foo\naddr2=$contact.Bar.Address.Address2";
	    //	    ok = Velocity.Evaluate(c, sw, "", template);
	    //	    Assert("Evalutation returned failure", ok);
	    //	    s = sw.ToString();
	    //	    Assert("test nested properties where property not found", s.Equals("These are the non-existent nested properties:\naddr1=\naddr2="));
	}


	// inner classes to support tests --------------------------

	public class ContactData {
	    private String name = String.Empty;
	    private AddressData address = new AddressData();

	    public String Name {
		get { return name; }
		set { name = value; }
	    }

	    public AddressData Address {
		get { return address; }
		set { address = value; }
	    }
	}

	public class AddressData {
	    private String address1 = String.Empty;
	    private String address2 = String.Empty;

	    public String Address1 {
		get { return this.address1; }
		set { this.address1 = value; }
	    }

	    public String Address2 {
		get { return this.address2; }
		set { this.address2 = value; }
	    }
	}


    }
}
