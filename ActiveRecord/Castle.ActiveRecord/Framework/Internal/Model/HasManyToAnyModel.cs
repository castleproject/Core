using System;
using System.Collections;
using System.Reflection;
using Castle.ActiveRecord;

namespace Castle.ActiveRecord.Framework.Internal
{

	public class HasManyToAnyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly HasManyToAnyAttribute hasManyToAnyAtt;

		public HasManyToAnyModel(PropertyInfo prop, HasManyToAnyAttribute hasManyToAnyAtt)
		{
			this.prop = prop;
			this.hasManyToAnyAtt = hasManyToAnyAtt;
		}

		public PropertyInfo Property
		{
			get { return prop; }
		}

		public HasManyToAnyAttribute HasManyToAnyAtt
		{
			get { return hasManyToAnyAtt; }
		}

		public Config Configuration
		{
			get { return  new Config(this); }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitHasManyToAny(this);
		}

		#endregion

		/// <summary>
		/// I need this class to pass special configuration for the many-to-any
		/// </summary>
		public class Config : IModelNode
		{
			HasManyToAnyModel parent;

			public HasManyToAnyModel Parent
			{
				get { return parent; }
				set { parent = value; }
			}

			internal Config(HasManyToAnyModel parent)
			{
				this.parent = parent;
			}


			#region IVisitable Members

			public void Accept(IVisitor visitor)
			{
				visitor.VisitHasManyToAnyConfig(this);
			}

			#endregion
		}
}
}
