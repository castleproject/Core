namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Context;
	using Exception;
	using NVelocity.App.Events;
	using NVelocity.Exception;

	/// <summary>
	/// Reference types
	/// </summary>
	internal enum ReferenceType
	{
		Normal = 1,
		Formal = 2,
		Quiet = 3,
		Runt = 4,
	}

	/// <summary>
	/// This class is responsible for handling the references in
	/// VTL ($foo).
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a></author>
	/// <author> <a href="mailto:kjohnson@transparent.com">Kent Johnson</a></author>
	/// <version> $Id: ASTReference.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTReference : SimpleNode
	{
		private ReferenceType referenceType;
		private String nullString;
		private String rootString;
		private bool escaped = false;
		private bool computableReference = true;
		private String escPrefix = string.Empty;
		private String morePrefix = string.Empty;
		private String identifier = string.Empty;

		private String literal = null;

		private Stack referenceStack;

		private int numChildren = 0;

		public ASTReference(int id) : base(id)
		{
		}

		public ASTReference(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Returns the 'root string', the reference key
		/// </summary>
		public String RootString
		{
			get { return rootString; }
		}


		public void SetLiteral(String value)
		{
			if (literal == null)
			{
				literal = value;
			}
		}

		public override String Literal
		{
			get
			{
				if (literal != null)
				{
					return literal;
				}

				return base.Literal;
			}
		}

		/// <summary>Accept the visitor.</summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override Object Init(IInternalContextAdapter context, Object data)
		{
			// init our children
			base.Init(context, data);

			// the only thing we can do in init() is getRoot()
			// as that is template based, not context based,
			// so it's thread- and context-safe
			rootString = Root;

			numChildren = ChildrenCount;

			// and if appropriate...
			if (numChildren > 0)
			{
				identifier = GetChild(numChildren - 1).FirstToken.Image;
			}

			return data;
		}

		/// <summary>
		/// gets an Object that 'is' the value of the reference
		/// </summary>
		public override Object Execute(Object o, IInternalContextAdapter context)
		{
			if (referenceType == ReferenceType.Runt)
				return null;

			// get the root object from the context
			Object result = GetVariableValue(context, rootString);

			if (context.EventCartridge != null)
			{
				referenceStack = new Stack();
				referenceStack.Push(result);
			}

			if (result == null)
			{
				return null;
			}

			// Iteratively work 'down' (it's flat...) the reference
			// to get the value, but check to make sure that
			// every result along the path is valid. For example:
			//
			// $hashtable.Customer.Name
			//
			// The $hashtable may be valid, but there is no key
			// 'Customer' in the hashtable so we want to stop
			// when we find a null value and return the null
			// so the error gets logged.
			try
			{
				for(int i = 0; i < numChildren; i++)
				{
					result = GetChild(i).Execute(result, context);

					if (referenceStack != null)
					{
						referenceStack.Push(result);
					}

					if (result == null)
					{
						return null;
					}
				}

				return result;
			}
			catch(MethodInvocationException methodInvocationException)
			{
				// someone tossed their cookies
				runtimeServices.Error(
					string.Format("Method {0} threw exception for reference ${1} in template {2} at  [{3},{4}]",
					              methodInvocationException.MethodName, rootString, context.CurrentTemplateName, Line, Column));

				methodInvocationException.ReferenceName = rootString;
				throw;
			}
		}

		/// <summary>
		/// gets the value of the reference and outputs it to the
		/// writer.
		/// </summary>
		/// <param name="context"> context of data to use in getting value </param>
		/// <param name="writer">  writer to render to </param>
		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			if (referenceType == ReferenceType.Runt)
			{
				writer.Write(rootString);
				return true;
			}

			Object value = Execute(null, context);

			// if this reference is escaped (\$foo) then we want to do one of two things :
			// 1) if this is a reference in the context, then we want to print $foo
			// 2) if not, then \$foo  (its considered shmoo, not VTL)
			if (escaped)
			{
				StringBuilder b = new StringBuilder();
				b.Append(escPrefix);
				if (value == null)
				{
					b.Append("\\");
				}
				b.Append(nullString);

				writer.Write(b);

				return true;
			}

			// the normal processing

			// if we have an event cartridge, get a new value object
			EventCartridge eventCartridge = context.EventCartridge;

			if (eventCartridge != null && referenceStack != null)
			{
				value = eventCartridge.ReferenceInsert(referenceStack, nullString, value);
			}

			// if value is null...
			if (value == null)
			{
				// write prefix twice, because it's shmoo, so the \ don't escape each other...
				StringBuilder b = new StringBuilder();

				b.Append(escPrefix);
				b.Append(escPrefix);
				b.Append(morePrefix);
				b.Append(nullString);

				writer.Write(b);

				if (referenceType != ReferenceType.Quiet &&
				    runtimeServices.GetBoolean(RuntimeConstants.RUNTIME_LOG_REFERENCE_LOG_INVALID, true))
				{
					runtimeServices.Warn(
						new ReferenceException(string.Format("reference : template = {0}", context.CurrentTemplateName), this));
				}

				return true;
			}
			else
			{
				// non-null processing
				StringBuilder b = new StringBuilder();
				b.Append(escPrefix);
				b.Append(morePrefix);
				b.Append(value);

				writer.Write(b);

				return true;
			}
		}

		/// <summary>   
		/// Computes boolean value of this reference
		/// Returns the actual value of reference return type
		/// boolean, and 'true' if value is not null
		/// </summary>
		/// <param name="context">context to compute value with</param>
		public override bool Evaluate(IInternalContextAdapter context)
		{
			Object value = Execute(null, context);

			if (value == null)
			{
				return false;
			}
			else if (value is Boolean)
			{
				return (bool) value;
			}
			else
			{
				return true;
			}
		}

		public override Object Value(IInternalContextAdapter context)
		{
			return (computableReference ? Execute(null, context) : null);
		}

		/// <summary>
		/// Sets the value of a complex reference (something like $foo.bar)
		/// Currently used by ASTSetReference()
		/// </summary>
		/// <seealso cref=" ASTSetDirective"/>
		/// <param name="context">context object containing this reference</param>
		/// <param name="value">Object to set as value</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool SetValue(IInternalContextAdapter context, Object value)
		{
			// The rootOfIntrospection is the object we will
			// retrieve from the Context. This is the base
			// object we will apply reflection to.
			Object result = GetVariableValue(context, rootString);

			if (result == null)
			{
				runtimeServices.Error(
					new ReferenceException(string.Format("reference set : template = {0}", context.CurrentTemplateName), this));
				return false;
			}

			// How many child nodes do we have?
			for(int i = 0; i < numChildren - 1; i++)
			{
				result = GetChild(i).Execute(result, context);

				if (result == null)
				{
					runtimeServices.Error(
						new ReferenceException(string.Format("reference set : template = {0}", context.CurrentTemplateName), this));
					return false;
				}
			}

			// We support two ways of setting the value in a #set($ref.foo = $value ) :
			// 1) ref.setFoo( value )
			// 2) ref,put("foo", value ) to parallel the get() map introspection
			try
			{
				IDuck duck = result as IDuck;

				if (duck == null)
				{
					// first, we introspect for the set<identifier> setter method
					Type c = result.GetType();
					PropertyInfo p;

					try
					{
						p = runtimeServices.Introspector.GetProperty(c, identifier);

						if (p == null)
						{
							throw new MethodAccessException();
						}
					}
					catch(MethodAccessException)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(identifier);

						if (Char.IsLower(sb[0]))
						{
							sb[0] = Char.ToUpper(sb[0]);
						}
						else
						{
							sb[0] = Char.ToLower(sb[0]);
						}

						p = runtimeServices.Introspector.GetProperty(c, sb.ToString());

						if (p == null)
						{
							throw;
						}
					}

					// and if we get here, getMethod() didn't chuck an exception...
					Object[] args = new Object[] {};
					p.SetValue(result, value, args);
				}
				else
				{
					duck.SetInvoke(identifier, value);
				}
			}
			catch(MethodAccessException)
			{
				// right now, we only support the IDictionary interface
				if (result is IDictionary)
				{
					try
					{
						IDictionary d = (IDictionary) result;
						d[identifier] = value;
					}
					catch(Exception ex)
					{
						runtimeServices.Error(
							string.Format("ASTReference Map.put : exception : {0} template = {1} [{2},{3}]", ex, context.CurrentTemplateName,
							              Line, Column));
						return false;
					}
				}
				else
				{
					runtimeServices.Error(
						string.Format("ASTReference : cannot find {0} as settable property or key to Map in template = {1} [{2},{3}]",
						              identifier, context.CurrentTemplateName, Line, Column));
					return false;
				}
			}
			catch(TargetInvocationException targetInvocationException)
			{
				// this is possible 
				throw new MethodInvocationException(
					string.Format("ASTReference : Invocation of method '{0}' in  {1} threw exception {2}", identifier, result.GetType(),
					              targetInvocationException.GetBaseException().GetType()), targetInvocationException, identifier);
			}
			catch(Exception e)
			{
				// maybe a security exception?
				runtimeServices.Error(
					string.Format("ASTReference setValue() : exception : {0} template = {1} [{2},{3}]", e, context.CurrentTemplateName,
					              Line, Column));
				return false;
			}

			return true;
		}

		public Object GetVariableValue(IContext context, String variable)
		{
			return context.Get(variable);
		}

		private String Root
		{
			get
			{
				Token t = FirstToken;

				// we have a special case where something like 
				// $(\\)*!, where the user want's to see something
				// like $!blargh in the output, but the ! prevents it from showing.
				// I think that at this point, this isn't a reference.

				// so, see if we have "\\!"

				int slashBang = t.Image.IndexOf("\\!");

				if (slashBang != -1)
				{
					// lets do all the work here.  I would argue that if this occurs, it's 
					// not a reference at all, so preceeding \ characters in front of the $
					// are just schmoo.  So we just do the escape processing trick (even | odd)
					// and move on.  This kind of breaks the rule pattern of $ and # but '!' really
					// tosses a wrench into things.

					// count the escapes : even # -> not escaped, odd -> escaped
					int i = 0;
					int len = t.Image.Length;

					i = t.Image.IndexOf('$');

					if (i == -1)
					{
						// yikes!
						runtimeServices.Error("ASTReference.getRoot() : internal error : no $ found for slashbang.");
						computableReference = false;
						nullString = t.Image;
						return nullString;
					}

					while(i < len && t.Image[i] != '\\')
					{
						i++;
					}

					// ok, i is the first \ char
					int start = i;
					int count = 0;

					while(i < len && t.Image[i++] == '\\')
					{
						count++;
					}

					// now construct the output string.  We really don't care about leading 
					// slashes as this is not a reference.  It's quasi-schmoo
					nullString = t.Image.Substring(0, (start) - (0)); // prefix up to the first
					nullString += t.Image.Substring(start, (start + count - 1) - (start)); // get the slashes
					nullString += t.Image.Substring(start + count); // and the rest, including the

					// this isn't a valid reference, so lets short circuit the value and set calcs
					computableReference = false;

					return nullString;
				}

				// we need to see if this reference is escaped.  if so
				// we will clean off the leading \'s and let the 
				// regular behavior determine if we should output this
				// as \$foo or $foo later on in render(). Laziness..
				escaped = false;

				if (t.Image.StartsWith(@"\"))
				{
					// count the escapes : even # -> not escaped, odd -> escaped
					int i = 0;
					int len = t.Image.Length;

					while(i < len && t.Image[i] == '\\')
					{
						i++;
					}

					if ((i % 2) != 0)
					{
						escaped = true;
					}

					if (i > 0)
					{
						escPrefix = t.Image.Substring(0, (i / 2) - (0));
					}

					t.Image = t.Image.Substring(i);
				}

				// Look for preceeding stuff like '#' and '$'
				// and snip it off, except for the
				// last $
				int loc1 = t.Image.LastIndexOf('$');

				// if we have extra stuff, loc > 0
				// ex. '#$foo' so attach that to 
				// the prefix.
				if (loc1 > 0)
				{
					morePrefix = morePrefix + t.Image.Substring(0, (loc1) - (0));
					t.Image = t.Image.Substring(loc1);
				}

				// Now it should be clean. Get the literal in case this reference 
				// isn't backed by the context at runtime, and then figure out what
				// we are working with.

				nullString = Literal;

				if (t.Image.StartsWith("$!"))
				{
					referenceType = ReferenceType.Quiet;

					// only if we aren't escaped do we want to null the output
					if (!escaped)
					{
						nullString = string.Empty;
					}

					if (t.Image.StartsWith("$!{"))
					{
						// ex : $!{provider.Title} 
						return t.Next.Image;
					}
					else
					{
						// ex : $!provider.Title
						return t.Image.Substring(2);
					}
				}
				else if (t.Image.Equals("${"))
				{
					// ex : ${provider.Title}
					referenceType = ReferenceType.Formal;
					return t.Next.Image;
				}
				else if (t.Image.StartsWith("$"))
				{
					// just nip off the '$' so we have 
					// the root
					referenceType = ReferenceType.Normal;
					return t.Image.Substring(1);
				}
				else
				{
					// this is a 'RUNT', which can happen in certain circumstances where
					// the parser is fooled into believing that an IDENTIFIER is a real 
					// reference.  Another 'dreaded' MORE hack :). 
					referenceType = ReferenceType.Runt;
					return t.Image;
				}
			}
		}
	}
}