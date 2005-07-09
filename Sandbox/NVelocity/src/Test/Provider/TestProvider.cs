using System;
using System.Collections;

namespace NVelocity.Test.Provider {

    /// <summary>
    /// This class is used by the testbed. Instances of the class
    /// are fed into the context that is set before the AST
    /// is traversed and dynamic content generated.
    /// </summary>
    /// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a><author>
    public class TestProvider {

	public static System.String PUB_STAT_STRING = "Public Static String";

	internal System.String title = "lunatic";
	internal bool state;
	internal System.Object ob = null;
	internal int stateint = 0;

	public virtual System.String Name {
	    get {
		return "jason";
	    }
	}

	public virtual System.Collections.Stack Stack {
	    get {
		System.Collections.Stack stack = new System.Collections.Stack();
		System.Object temp_object;
		temp_object = "stack element 1";
		System.Object generatedAux = temp_object;
		stack.Push(temp_object);
		System.Object temp_object2;
		temp_object2 = "stack element 2";
		System.Object generatedAux2 = temp_object2;
		stack.Push(temp_object2);
		System.Object temp_object3;
		temp_object3 = "stack element 3";
		System.Object generatedAux3 = temp_object3;
		stack.Push(temp_object3);
		return stack;
	    }
	}

	public virtual IList EmptyList {
	    get {
		IList list = new ArrayList();
		return list;
	    }
	}

	public virtual IList List {
	    get {
		IList list = new ArrayList();
		list.Add("list element 1");
		list.Add("list element 2");
		list.Add("list element 3");

		return list;
	    }
	}

	public virtual System.Collections.Hashtable Search {
	    get {
		System.Collections.Hashtable h = new System.Collections.Hashtable();
		h.Add("Text", "this is some text");
		h.Add("EscText", "this is escaped text");
		h.Add("Title", "this is the title");
		h.Add("Index", "this is the index");
		h.Add("URL", "http://periapt.com");

		ArrayList al = new ArrayList();
		al.Add(h);

		h.Add("RelatedLinks", al);

		return h;
	    }
	}

	public virtual System.Collections.Hashtable Hashtable {
	    get {
		System.Collections.Hashtable h = new System.Collections.Hashtable();
		SupportClass.PutElement(h, "key0", "value0");
		SupportClass.PutElement(h, "key1", "value1");
		SupportClass.PutElement(h, "key2", "value2");

		return h;
	    }
	}

	public virtual ArrayList RelSearches {
	    get {
		ArrayList al = new ArrayList();
		al.Add(Search);

		return al;
	    }
	}

	public virtual System.String Title {
	    get {
		return title;
	    }

	    set {
		this.title = value;
	    }
	}

	public virtual System.Object[] Menu {
	    get {
		//ArrayList al = new ArrayList();
		System.Object[] menu = new System.Object[3];
		for (int i = 0; i < 3; i++) {
		    System.Collections.Hashtable item = new System.Collections.Hashtable();
		    SupportClass.PutElement(item, "id", "item" + System.Convert.ToString(i + 1));
		    SupportClass.PutElement(item, "name", "name" + System.Convert.ToString(i + 1));
		    SupportClass.PutElement(item, "label", "label" + System.Convert.ToString(i + 1));
		    //al.Add(item);
		    menu[i] = item;
		}

		//return al;
		return menu;
	    }
	}

	public ArrayList getCustomers() {
	    return Customers;
	}


	public virtual ArrayList Customers {
	    get {
		ArrayList list = new ArrayList();

		list.Add("ArrayList element 1");
		list.Add("ArrayList element 2");
		list.Add("ArrayList element 3");
		list.Add("ArrayList element 4");

		return list;
	    }
	}

	public virtual ArrayList Customers2 {
	    get {
		ArrayList list = new ArrayList();

		list.Add(new TestProvider());
		list.Add(new TestProvider());
		list.Add(new TestProvider());
		list.Add(new TestProvider());

		return list;
	    }
	}

	public virtual System.Collections.ArrayList Vector {
	    get {
		System.Collections.ArrayList list = new System.Collections.ArrayList();

		list.Add("vector element 1");
		list.Add("vector element 2");

		return list;
	    }
	}

	public virtual System.String[] Array {
	    get {
		System.String[] strings = new System.String[2];
		strings[0] = "first element";
		strings[1] = "second element";
		return strings;
	    }
	}

	public virtual bool StateTrue {
	    get {
		return true;
	    }
	}

	public virtual bool StateFalse {
	    get {
		return false;
	    }
	}

	public virtual Person Person {
	    get {
		return new Person();
	    }
	}

	public virtual Child Child {
	    get {
		return new Child();
	    }
	}

	public virtual System.Boolean State {
	    set {}
	}

	public virtual System.Int32 BangStart {
	    set {
		System.Console.Out.WriteLine("SetBangStart() : called with val = " + value);
		stateint = value;
	    }
	}

	public virtual System.String Foo {
	    get {
		System.Console.Out.WriteLine("Hello from getfoo");
		throw new System.Exception("From getFoo()");
	    }
	}

	public virtual System.String Throw {
	    get {
		System.Console.Out.WriteLine("Hello from geThrow");
		throw new System.Exception("From getThrow()");
	    }
	}

	public String getTitleMethod() {
	    return this.title;
	}














	public virtual System.Object me() {
	    return this;
	}

	public override System.String ToString() {
	    return ("test provider");
	}



	public virtual bool theAPLRules() {
	    return true;
	}



	public virtual System.String objectArrayMethod(System.Object[] o) {
	    return "result of objectArrayMethod";
	}

	public virtual System.String concat(System.Object[] strings) {
	    System.Text.StringBuilder result = new System.Text.StringBuilder();

	    for (int i = 0; i < strings.Length; i++) {
		result.Append((System.String) strings[i]).Append(' ');
	    }

	    return result.ToString();
	}

	public virtual System.String concat(IList strings) {
	    System.Text.StringBuilder result = new System.Text.StringBuilder();

	    for (int i = 0; i < strings.Count; i++) {
		result.Append((System.String) strings[i]).Append(' ');
	    }

	    return result.ToString();
	}

	public virtual System.String objConcat(IList objects) {
	    System.Text.StringBuilder result = new System.Text.StringBuilder();

	    for (int i = 0; i < objects.Count; i++) {
		result.Append(objects[i]).Append(' ');
	    }

	    return result.ToString();
	}

	public virtual System.String parse(System.String a, System.Object o, System.String c, System.String d) {
	    //UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
	    return a + o.ToString() + c + d;
	}

	public virtual System.String concat(System.String a, System.String b) {
	    return a + b;
	}

	// These two are for testing subclasses.



	public virtual System.String showPerson(Person person) {
	    return person.Name;
	}

	/// <summary> Chop i characters off the end of a string.
	/// *
	/// </summary>
	/// <param name="string">String to chop.
	/// </param>
	/// <param name="i">Number of characters to chop.
	/// </param>
	/// <returns>String with processed answer.
	///
	/// </returns>
	public virtual System.String chop(System.String string_Renamed, int i) {
	    return (string_Renamed.Substring(0, (string_Renamed.Length - i) - (0)));
	}

	public virtual bool allEmpty(System.Object[] list) {
	    int size = list.Length;

	    for (int i = 0; i < size; i++) {
		//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
		if (list[i].ToString().Length > 0)
		    return false;
	    }

	    return true;
	}

	/*
	* This can't have the signature

	public void setState(boolean state)

	or dynamically invoking the method
	doesn't work ... you would have to
	put a wrapper around a method for a
	real boolean property that takes a 
	Boolean object if you wanted this to
	work. Not really sure how useful it
	is anyway. Who cares about boolean
	values you can just set a variable.

	*/


	public virtual System.Int32 bang() {
	    System.Console.Out.WriteLine("Bang! : " + stateint);
	    System.Int32 ret = stateint;
	    stateint++;
	    return ret;
	}

	/// <summary> Test the ability of vel to use a get(key)
	/// method for any object type, not just one
	/// that implements the Map interface.
	/// </summary>
	public virtual System.String get
	    (System.String key) {
	    return key;
	}


	public String this[System.String key] {
	    get { return key; }
	}


	/// <summary> Test the ability of vel to use a put(key)
	/// method for any object type, not just one
	/// that implements the Map interface.
	/// </summary>
	public virtual System.String put(System.String key, System.Object o) {
	    ob = o;
	    return key;
	}


    }
}
