using System;
using System.Collections;
using System.Reflection;
using Castle.ActiveRecord;

namespace Castle.ActiveRecord.Framework.Internal
{
	
	public class AnyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly AnyAttribute anyAtt;
		private IList metaValues;

		public AnyModel(PropertyInfo prop, AnyAttribute anyAtt)
		{
			this.prop = prop;
			this.anyAtt = anyAtt;
			metaValues = new ArrayList();
		}

		public PropertyInfo Property
		{
			get { return prop; }
		}

		public AnyAttribute AnyAtt
		{
			get { return anyAtt; }
		}

		public IList MetaValues
		{
			get { return metaValues; }
			set { metaValues = value; }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitAny(this);
		}

		#endregion

	}
}
