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
		private static readonly string[] sectionNames; 

		static Foreach()
		{
			sectionNames = ForeachSectionEnum.GetNames(typeof(ForeachSectionEnum));
			Array.Sort(sectionNames);
			for(int i=0;i < sectionNames.Length; i++)
			{
				sectionNames[i] = sectionNames[i].ToLower();
			}
		}

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
		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			counterName = rsvc.GetString(RuntimeConstants.COUNTER_NAME);
			counterInitialValue = rsvc.GetInt(RuntimeConstants.COUNTER_INITIAL_VALUE);

			// this is really the only thing we can do here as everything
			// else is context sensitive
			elementKey = node.GetChild(0).FirstToken.Image.Substring(1);
		}

		/// <summary>
		/// returns an Iterator to the collection in the #foreach()
		/// </summary>
		/// <param name="context"> current context </param>
		/// <param name="node">  AST node </param>
		/// <returns>Iterator to do the dataset </returns>
		private IEnumerator GetIterator(IInternalContextAdapter context, INode node)
		{
			// get our list object, and punt if it's null.
			Object listObject = node.GetChild(2).Value(context);
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
					rsvc.Warn("Warning! The reference " + node.GetChild(2).FirstToken.Image + " is an Enumeration in the #foreach() loop at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName + ". Because it's not resetable," + " if used in more than once, this may lead to" + " unexpected results.");
					return (IEnumerator) listObject;

				case EnumType.Array:
					return ((Array) listObject).GetEnumerator();

				case EnumType.Dictionary:
					return ((IDictionary) listObject).GetEnumerator();

				default:
					/*  we have no clue what this is  */
					rsvc.Warn("Could not determine type of enumerator (" + listObject.GetType().Name + ") in " + "#foreach loop for " + node.GetChild(2).FirstToken.Image + " at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName);

					return null;
			}
		}

		/// <summary>
		/// renders the #foreach() block
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			// do our introspection to see what our collection is
			IEnumerator enumerator = GetIterator(context, node);
			INode bodyNode = node.GetChild(3);

			INode[][] sections = PrepareSections(bodyNode);
			bool isFancyLoop = (sections != null);

			if( enumerator == null && !isFancyLoop )
			{
				return true;
			}

			int counter = counterInitialValue;

			// save the element key if there is one,
			// and the loop counter
			Object o = context.Get(elementKey);
			Object ctr = context.Get(counterName);

			if( enumerator != null && enumerator.MoveNext() )
			{		
				do
				{
					object current = enumerator.Current;

					context.Put(counterName, counter);

					context.Put(elementKey, current);

					if( !isFancyLoop )
					{
						bodyNode.Render(context, writer);
					}
					else
					{
						if( counter == counterInitialValue )
						{
							ProcessSection( ForeachSectionEnum.BeforeAll, sections, context, writer );
						}
						else
						{
							ProcessSection( ForeachSectionEnum.Between, sections, context, writer );
						}

						ProcessSection( ForeachSectionEnum.Before, sections, context, writer );

						// since 1st item is zero we invert odd/even
						if( (counter - counterInitialValue) % 2 == 0)
						{
							ProcessSection( ForeachSectionEnum.Odd, sections, context, writer );
						}
						else
						{
							ProcessSection( ForeachSectionEnum.Even, sections, context, writer );
						}						

						ProcessSection( ForeachSectionEnum.Each, sections, context, writer );

						ProcessSection( ForeachSectionEnum.After, sections, context, writer );
					}

					counter++;
				}			
				while( enumerator.MoveNext() );
			}

			if( isFancyLoop )
			{
				if( counter > counterInitialValue )
				{
					ProcessSection( ForeachSectionEnum.AfterAll, sections, context, writer );
				}	
				else
				{
					ProcessSection( ForeachSectionEnum.NoData, sections, context, writer );
				}				
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

		private void ProcessSection( ForeachSectionEnum sectionEnumType, INode[][] sections, IInternalContextAdapter context, TextWriter writer )
		{
			int sectionIndex = (int) sectionEnumType;

			if( sections[sectionIndex] == null ) return;

			foreach(INode node in sections[sectionIndex])
			{
				node.Render(context, writer);
			}
		}

		private INode[][] PrepareSections( INode node )
		{
			bool isFancyLoop = false;
			int curSection = (int) ForeachSectionEnum.Each;
			ArrayList[] sections = new ArrayList[sectionNames.Length];
			int nodeCount = node.ChildrenCount;

			for( int i = 0; i < nodeCount; i++ )
			{
				INode childNode = node.GetChild(i);
				ASTDirective directive = childNode as ASTDirective;

				if( directive != null && Array.BinarySearch(sectionNames, directive.DirectiveName) > -1 )
				{
					isFancyLoop = true;
					curSection = (int) ForeachSectionEnum.Parse(typeof(ForeachSectionEnum), directive.DirectiveName, true );
				}
				else
				{
					if(sections[curSection] == null)
					{
						sections[curSection] = new ArrayList();
					}
					sections[curSection].Add(childNode);
				}
			}

			if( !isFancyLoop )
			{
				return null;
			}
			else
			{
				INode[][] result = new INode[sections.Length][];
			
				for( int i=0; i < sections.Length; i++ )
				{
					if( sections[i] != null )
						result[i] = sections[i].ToArray(typeof(INode)) as INode[];
				}

				return result;				
			}
		}
	}

	public enum ForeachSectionEnum
	{
		Each      = 0,
		Between   = 1,
		Odd		  = 2,
		Even	  = 3,
		NoData	  = 4,
		BeforeAll = 5,
		AfterAll  = 6,
		Before    = 7,
		After     = 8
	}

	public interface IForeachSection
	{
		ForeachSectionEnum Section { get; }		
	}

	public abstract class AbstractForeachSection : Directive, IForeachSection
	{
		public override String Name
		{
			get { return Section.ToString().ToLower(); }
			set { throw new NotImplementedException(); }
		}

		public override bool AcceptParams
		{
			get { return false; }
		}

		public override DirectiveType Type
		{
			get { return DirectiveType.LINE; }
		}

		public override bool Render( IInternalContextAdapter context, TextWriter writer, INode node )
		{
			return true;
		}

		public abstract ForeachSectionEnum Section
		{
			get;
		}		
	}

	public class ForeachEachSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.Each; }
		}
	}

	public class ForeachBetweenSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.Between; }
		}
	}

	public class ForeachOddSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.Odd; }
		}
	}

	public class ForeachEvenSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.Even; }
		}
	}

	public class ForeachNoDataSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.NoData; }
		}
	}

	public class ForeachBeforeSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.Before; }
		}
	}

	public class ForeachAfterSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.After; }
		}
	}

	public class ForeachBeforeAllSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.BeforeAll; }
		}
	}

	public class ForeachAfterAllSection : AbstractForeachSection
	{	
		public override ForeachSectionEnum Section
		{
			get { return ForeachSectionEnum.AfterAll; }
		}
	}

}
