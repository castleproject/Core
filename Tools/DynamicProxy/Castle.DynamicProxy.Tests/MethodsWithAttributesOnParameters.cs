using System;

namespace Castle.DynamicProxy.Test
{
	using System.Reflection;
	using NUnit.Framework;

	[TestFixture]
	public class MethodsWithAttributesOnParameters
	{
		[Test]
		[ExpectedException(typeof(ArgumentException),"No default value for argument")]
		public void ParametersAreCopiedToProxiedObject()
		{
			ProxyGenerator pg = new ProxyGenerator();
			ClassWithAttributesOnMethodParameters requiredObj = (ClassWithAttributesOnMethodParameters)pg.CreateClassProxy(typeof(ClassWithAttributesOnMethodParameters), new RequiredParamInterceptor());
			requiredObj.MethodOne(-1);
		}

		[Test]
		public void CanGetParameterAttributeFromProxiedObject()
		{
			ProxyGenerator pg = new ProxyGenerator();
			ClassWithAttributesOnMethodParameters requiredObj = (ClassWithAttributesOnMethodParameters)pg.CreateClassProxy(typeof(ClassWithAttributesOnMethodParameters), new RequiredParamInterceptor());
			requiredObj.MethodTwo(null);
		}
	}

	public class RequiredParamInterceptor :IInterceptor
	{
		public object Intercept(IInvocation invocation, params object[] args)
		{
			ParameterInfo[] parameters = invocation.Method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if(parameters[i].IsDefined(typeof(RequiredAttribute),false))
				{
					RequiredAttribute required = parameters[i].GetCustomAttributes(typeof(RequiredAttribute),false)[0] as RequiredAttribute;
					if( (required.BadValue == null && args[i] == null) ||
						(required.BadValue != null && required.BadValue.Equals(args[i])
						))
					{
						args[i] = required.DefaultValue;
					}
				}
			}
			return invocation.Proceed(args);
		}
	}

	public class ClassWithAttributesOnMethodParameters
	{
		public virtual void MethodOne([Required(BadValue=-1)]int val)
		{
			Assert.IsFalse(val==-1);
		}

		public virtual void MethodTwo([Required("")]string name)
		{
			Assert.IsNotNull(name);
		}
	}

	public class RequiredAttribute : Attribute
	{
		object defaultValue;
		public object BadValue;
		private bool hasDefault=false;
	
		public RequiredAttribute(){}

		public RequiredAttribute(object defaultValue)
		{
			hasDefault = true;
			this.defaultValue = defaultValue;
		}


		public object DefaultValue
		{
			get
			{
				if(!hasDefault)
					throw new ArgumentException("No default value for argument");
				return defaultValue;
			}
		}

	}
}
