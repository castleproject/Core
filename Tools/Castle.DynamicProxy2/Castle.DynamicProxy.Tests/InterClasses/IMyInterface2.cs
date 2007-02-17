using System;

namespace Castle.DynamicProxy.Tests.InterClasses
{
	/// <summary>
	/// Summary description for IMyInterface.
	/// </summary>
	public interface IMyInterface2
	{
		String Name
		{
			get;
			set;
		}

		bool Started
		{
			get;
			set;
		}

		int Calc(int x, int y);

		int Calc(int x, int y, int z, Single k);
	}

	/// <summary>
	/// Summary description for MyInterfaceImpl.
	/// </summary>
	[Serializable]
	[MyAttribute("MyInterfaceImpl")]
	public class MyInterfaceImpl : IMyInterface2
	{
		private String _name;
		private bool _started;

		public MyInterfaceImpl()
		{
		}

		public virtual String Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public virtual bool Started
		{
			get
			{
				return _started;
			}
			set
			{
				_started = value;
			}
		}

		[MyAttribute("Calc1")]
		public virtual int Calc(int x, int y)
		{
			return x + y;
		}

		[MyAttribute("Calc2")]
		public virtual int Calc(int x, int y, int z, Single k)
		{
			return x + y + z + (int)k;
		}
	}

	public class MyInterfaceImplX : MyInterfaceImpl
	{
		
	}

	
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Method)]
	public class MyAttribute : Attribute
	{
		private string _name;

		public MyAttribute(String name)
		{
			_name = name;
		}

		public string name
		{
			get { return _name; }
		}
	}

}