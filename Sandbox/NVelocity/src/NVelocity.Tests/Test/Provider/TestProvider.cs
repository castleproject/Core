namespace NVelocity.Test.Provider
{
	using System;
	using System.Collections;
	using System.Text;

	/// <summary>
	/// This class is used by the testbed. Instances of the class
	/// are fed into the context that is set before the AST
	/// is traversed and dynamic content generated.
	/// </summary>
	/// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a><author>
	public class TestProvider
	{
		public static String PUB_STAT_STRING = "Public Static String";

		internal String title = "lunatic";
		//internal bool state;
		internal Object ob = null;
		internal int stateint = 0;

		public virtual String Name
		{
			get { return "jason"; }
		}

		public virtual Stack Stack
		{
			get
			{
				Stack stack = new Stack();
				Object temp_object;
				temp_object = "stack element 1";
				Object generatedAux = temp_object;
				stack.Push(temp_object);
				Object temp_object2;
				temp_object2 = "stack element 2";
				Object generatedAux2 = temp_object2;
				stack.Push(temp_object2);
				Object temp_object3;
				temp_object3 = "stack element 3";
				Object generatedAux3 = temp_object3;
				stack.Push(temp_object3);
				return stack;
			}
		}

		public virtual IList EmptyList
		{
			get
			{
				IList list = new ArrayList();
				return list;
			}
		}

		public virtual IList List
		{
			get
			{
				IList list = new ArrayList();
				list.Add("list element 1");
				list.Add("list element 2");
				list.Add("list element 3");

				return list;
			}
		}

		public virtual Hashtable Search
		{
			get
			{
				Hashtable h = new Hashtable();
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

		public virtual Hashtable Hashtable
		{
			get
			{
				Hashtable h = new Hashtable();
				SupportClass.PutElement(h, "key0", "value0");
				SupportClass.PutElement(h, "key1", "value1");
				SupportClass.PutElement(h, "key2", "value2");

				return h;
			}
		}

		public virtual ArrayList RelSearches
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add(Search);

				return al;
			}
		}

		public virtual String Title
		{
			get { return title; }

			set { this.title = value; }
		}

		public virtual Object[] Menu
		{
			get
			{
				//ArrayList al = new ArrayList();
				Object[] menu = new Object[3];
				for (int i = 0; i < 3; i++)
				{
					Hashtable item = new Hashtable();
					SupportClass.PutElement(item, "id", "item" + Convert.ToString(i + 1));
					SupportClass.PutElement(item, "name", "name" + Convert.ToString(i + 1));
					SupportClass.PutElement(item, "label", "label" + Convert.ToString(i + 1));
					//al.Add(item);
					menu[i] = item;
				}

				//return al;
				return menu;
			}
		}

		public ArrayList getCustomers()
		{
			return Customers;
		}


		public virtual ArrayList Customers
		{
			get
			{
				ArrayList list = new ArrayList();

				list.Add("ArrayList element 1");
				list.Add("ArrayList element 2");
				list.Add("ArrayList element 3");
				list.Add("ArrayList element 4");

				return list;
			}
		}

		public virtual ArrayList Customers2
		{
			get
			{
				ArrayList list = new ArrayList();

				list.Add(new TestProvider());
				list.Add(new TestProvider());
				list.Add(new TestProvider());
				list.Add(new TestProvider());

				return list;
			}
		}

		public virtual ArrayList Vector
		{
			get
			{
				ArrayList list = new ArrayList();

				list.Add("vector element 1");
				list.Add("vector element 2");

				return list;
			}
		}

		public virtual String[] Array
		{
			get
			{
				String[] strings = new String[2];
				strings[0] = "first element";
				strings[1] = "second element";
				return strings;
			}
		}

		public virtual bool StateTrue
		{
			get { return true; }
		}

		public virtual bool StateFalse
		{
			get { return false; }
		}

		public virtual Person Person
		{
			get { return new Person(); }
		}

		public virtual Child Child
		{
			get { return new Child(); }
		}

		public virtual Boolean State
		{
			set
			{
			}
		}

		public virtual Int32 BangStart
		{
			set
			{
				Console.Out.WriteLine("SetBangStart() : called with val = " + value);
				stateint = value;
			}
		}

		public virtual String Foo
		{
			get
			{
				Console.Out.WriteLine("Hello from getfoo");
				throw new Exception("From getFoo()");
			}
		}

		public virtual String Throw
		{
			get
			{
				Console.Out.WriteLine("Hello from geThrow");
				throw new Exception("From getThrow()");
			}
		}

		public String getTitleMethod()
		{
			return this.title;
		}


		public virtual Object me()
		{
			return this;
		}

		public override String ToString()
		{
			return ("test provider");
		}


		public virtual bool theAPLRules()
		{
			return true;
		}


		public virtual String objectArrayMethod(Object[] o)
		{
			return "result of objectArrayMethod";
		}

		public virtual String concat(Object[] strings)
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < strings.Length; i++)
			{
				result.Append((String) strings[i]).Append(' ');
			}

			return result.ToString();
		}

		public virtual String concat(IList strings)
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < strings.Count; i++)
			{
				result.Append((String) strings[i]).Append(' ');
			}

			return result.ToString();
		}

		public virtual String objConcat(IList objects)
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < objects.Count; i++)
			{
				result.Append(objects[i]).Append(' ');
			}

			return result.ToString();
		}

		public virtual String parse(String a, Object o, String c, String d)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			return a + o.ToString() + c + d;
		}

		public virtual String concat(String a, String b)
		{
			return a + b;
		}

		// These two are for testing subclasses.


		public virtual String showPerson(Person person)
		{
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
		public virtual String chop(String string_Renamed, int i)
		{
			return (string_Renamed.Substring(0, (string_Renamed.Length - i) - (0)));
		}

		public virtual bool allEmpty(Object[] list)
		{
			int size = list.Length;

			for (int i = 0; i < size; i++)
			{
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


		public virtual Int32 bang()
		{
			Console.Out.WriteLine("Bang! : " + stateint);
			Int32 ret = stateint;
			stateint++;
			return ret;
		}

		/// <summary> Test the ability of vel to use a get(key)
		/// method for any object type, not just one
		/// that implements the Map interface.
		/// </summary>
		public virtual String get
			(String key)
		{
			return key;
		}


		public String this[String key]
		{
			get { return key; }
		}


		/// <summary> Test the ability of vel to use a put(key)
		/// method for any object type, not just one
		/// that implements the Map interface.
		/// </summary>
		public virtual String put(String key, Object o)
		{
			ob = o;
			return key;
		}


	}
}