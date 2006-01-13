namespace NVelocity.Runtime.Directive
{
	using System;
	using System.Collections;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// Foreach directive used for moving through arrays,
	/// or objects that provide an Iterator.
	/// </summary>
	public class Foreach : Directive
	{
		private enum EnumType
		{
			Unknown = -1,
			Array = 1,
			Iterator = 2,
			Dictionary = 3,
			Collection = 4,
			Enumeration = 5,
			Enumerable = 6,
		}

		/// <summary>
		/// The name of the variable to use when placing
		/// the counter value into the context. Right
		/// now the default is $velocityCount.
		/// </summary>
		private String counterName;

		/// <summary>
		/// What value to start the loop counter at.
		/// </summary>
		private int counterInitialValue;

		/// <summary>
		/// The reference name used to access each
		/// of the elements in the list object. It
		/// is the $item in the following:
		///
		/// #foreach ($item in $list)
		///
		/// This can be used class wide because
		/// it is immutable.
		/// </summary>
		private String elementKey;

		public Foreach()
		{
		}

		/// <summary>
		/// Return name of this directive.
		/// </summary>
		public override String Name
		{
			get { return "foreach"; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Return type of this directive.
		/// </summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}

		/// <summary>  
		/// simple init - init the tree and get the elementKey from
		/// the AST
		/// </summary>
		public override void Init(RuntimeServices rs, InternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			counterName = rsvc.GetString(RuntimeConstants.COUNTER_NAME);
			counterInitialValue = rsvc.GetInt(RuntimeConstants.COUNTER_INITIAL_VALUE);

			// this is really the only thing we can do here as everything
			// else is context sensitive
			elementKey = node.jjtGetChild(0).FirstToken.Image.Substring(1);
		}

		/// <summary>
		/// returns an Iterator to the collection in the #foreach()
		/// </summary>
		/// <param name="context"> current context </param>
		/// <param name="node">  AST node </param>
		/// <returns>Iterator to do the dataset </returns>
		private IEnumerator GetIterator(InternalContextAdapter context, INode node)
		{
			// get our list object, and punt if it's null.
			Object listObject = node.jjtGetChild(2).Value(context);
			if (listObject == null)
				return null;

			// See if we already know what type this is. 
			// Use the introspection cache
			EnumType type = EnumType.Unknown;

			IntrospectionCacheData icd = context.ICacheGet(this);
			Type c = listObject.GetType();

			// if we have an entry in the cache, and the Class we have
			// cached is the same as the Class of the data object
			// then we are ok

			if (icd != null && icd.ContextData == c)
			{
				// dig the type out of the cata object
				type = ((EnumType) icd.Thingy);
			}

			// If we still don't know what this is, 
	    // figure out what type of object the list
	    // element is, and get the iterator for it
			if (type == EnumType.Unknown)
			{
				if (listObject.GetType().IsArray)
					type = EnumType.Array;

				// NOTE: IDictionary needs to come before ICollection as it support ICollection
				else if (listObject is IDictionary)
					type = EnumType.Dictionary;
				else if (listObject is ICollection)
					type = EnumType.Collection;
				else if (listObject is IEnumerable)
					type = EnumType.Enumerable;
				else if (listObject is IEnumerator)
					type = EnumType.Enumeration;

				// if we did figure it out, cache it
				if (type != EnumType.Unknown)
				{
					icd = new IntrospectionCacheData(c, type);
					context.ICachePut(this, icd);
				}
			}

			// now based on the type from either cache or examination...
			switch (type)
			{
				case EnumType.Collection:
					return ((ICollection) listObject).GetEnumerator();

				case EnumType.Enumerable:
					return ((IEnumerable) listObject).GetEnumerator();

				case EnumType.Enumeration:
					rsvc.Warn("Warning! The reference " + node.jjtGetChild(2).FirstToken.Image + " is an Enumeration in the #foreach() loop at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName + ". Because it's not resetable," + " if used in more than once, this may lead to" + " unexpected results.");
					return (IEnumerator) listObject;

				case EnumType.Array:
					return ((Array) listObject).GetEnumerator();

				case EnumType.Dictionary:
					return ((IDictionary) listObject).GetEnumerator();

				default:
					/*  we have no clue what this is  */
					rsvc.Warn("Could not determine type of enumerator (" + listObject.GetType().Name + ") in " + "#foreach loop for " + node.jjtGetChild(2).FirstToken.Image + " at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName);

					return null;
			}
		}

		/// <summary>
		/// renders the #foreach() block
		/// </summary>
		public override bool Render(InternalContextAdapter context, TextWriter writer, INode node)
		{
			// do our introspection to see what our collection is
			IEnumerator i = GetIterator(context, node);

			if (i == null)
				return false;

			int counter = counterInitialValue;

			// save the element key if there is one,
	    // and the loop counter
			Object o = context.Get(elementKey);
			Object ctr = context.Get(counterName);

			while (i.MoveNext())
			{
				context.Put(counterName, counter);
				context.Put(elementKey, i.Current);
				node.jjtGetChild(3).Render(context, writer);
				counter++;
			}

			// restores the loop counter (if we were nested)
	    // if we have one, else just removes
			if (ctr != null)
				context.Put(counterName, ctr);
			else
				context.Remove(counterName);


			// restores element key if exists
	    // otherwise just removes
			if (o != null)
				context.Put(elementKey, o);
			else
				context.Remove(elementKey);

			return true;
		}
	}
}