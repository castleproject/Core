
namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for EasyEvent.
	/// </summary>
	public class EasyEvent : IEasyMember
	{
		private EventBuilder m_builder;
		private AbstractEasyType m_maintype;
		private EasyMethod m_addOnMethod;
    private EasyMethod m_removeOnMethod;
    private string m_name;

		public EasyEvent( AbstractEasyType maintype, String name, Type eventHandlerType )
		{
      m_name = name;
			m_maintype = maintype;
			m_builder = maintype.TypeBuilder.DefineEvent(
				name, EventAttributes.None, eventHandlerType);
		}

		public String Name
		{
			get { return m_name; }
		}

		public EasyMethod CreateAddOnMethod(MethodAttributes atts, params Type[] parameters)
		{
      if (m_addOnMethod == null)
      {
        m_addOnMethod = 
          new EasyMethod(m_maintype, "add_" + Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters) );
      }

      return m_addOnMethod;
		}

    public EasyMethod CreateRemoveOnMethod(MethodAttributes atts, params Type[] parameters)
    {
      if (m_removeOnMethod == null)
      {
        m_removeOnMethod = 
          new EasyMethod(m_maintype, "remove_" + Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters) );
      }

      return m_removeOnMethod;
    }

		#region IEasyMember Members

		public void Generate()
		{
			if (m_addOnMethod != null)
			{
				m_addOnMethod.Generate();
				m_builder.SetAddOnMethod(m_addOnMethod.MethodBuilder);
			}
			
			if (m_removeOnMethod != null)
			{
				m_removeOnMethod.Generate();
				m_builder.SetRemoveOnMethod(m_removeOnMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (m_addOnMethod != null)
			{
				m_addOnMethod.EnsureValidCodeBlock();
			}
			
			if (m_removeOnMethod != null)
			{
				m_removeOnMethod.EnsureValidCodeBlock();
			}
		}

		public MethodBase Member
		{
			get
			{
				return null;
			}
		}

		public Type ReturnType
		{
			get
			{
        throw new Exception("TBD");
			}
		}

		#endregion
	}
}

