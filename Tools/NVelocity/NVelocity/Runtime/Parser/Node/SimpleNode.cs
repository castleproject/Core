namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using System.Text;
	using Context;
	using Exception;

	public class SimpleNode : INode
	{
		protected internal IRuntimeServices runtimeServices = null;

		protected internal INode parent;
		protected internal INode[] children;
		protected internal int id;
		protected internal Parser parser;

		protected internal int info; // added
		public bool state;
		protected internal bool invalid = false;

		/* Added */
		protected internal Token first, last;

		public SimpleNode(int i)
		{
			id = i;
		}

		public SimpleNode(Parser p, int i) : this(i)
		{
			parser = p;
		}

		public Token FirstToken
		{
			get { return first; }

			set { first = value; }
		}

		public Token LastToken
		{
			get { return last; }
		}

		public int Type
		{
			get { return id; }
		}

		public int Info
		{
			get { return info; }
			set { info = value; }
		}

		public int Line
		{
			get { return first.BeginLine; }
		}

		public int Column
		{
			get { return first.BeginColumn; }
		}

		public void Open()
		{
			first = parser.GetToken(1); // added
		}

		public void Close()
		{
			last = parser.GetToken(0); // added
		}

		public INode Parent
		{
			set { parent = value; }
			get { return parent; }
		}

		public void AddChild(INode n, int i)
		{
			if (children == null)
			{
				children = new INode[i + 1];
			}
			else if (i >= children.Length)
			{
				INode[] c = new INode[i + 1];
				Array.Copy(children, 0, c, 0, children.Length);
				children = c;
			}
			children[i] = n;
		}

		public INode GetChild(int i)
		{
			return children[i];
		}

		public int ChildrenCount
		{
			get
			{
				if (children == null)
				{
					return 0;
				}
				else
				{
					return children.Length;
				}
			}
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public virtual Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public Object ChildrenAccept(IParserVisitor visitor, Object data)
		{
			if (children != null)
			{
				for(int i = 0; i < children.Length; ++i)
				{
					children[i].Accept(visitor, data);
				}
			}
			return data;
		}

		/* You can override these two methods in subclasses of SimpleNode to
	    customize the way the node appears when the tree is dumped.  If
	    your output uses more than one line you should override
	    toString(String), otherwise overriding toString() is probably all
	    you need to do. */

		//    public String toString()
		// {
		//    return ParserTreeConstants.jjtNodeName[id];
		// }
		public String ToString(String prefix)
		{
			return prefix + ToString();
		}

		/* Override this method if you want to customize how the node dumps
		out its children. */

		public void Dump(String prefix)
		{
			Console.Out.WriteLine(ToString(prefix));
			if (children != null)
			{
				for(int i = 0; i < children.Length; ++i)
				{
					SimpleNode n = (SimpleNode) children[i];
					if (n != null)
					{
						n.Dump(string.Format("{0} ", prefix));
					}
				}
			}
		}

		// All additional methods

		public virtual string Literal
		{
			get
			{
				Token t = first;
				StringBuilder sb = new StringBuilder(t.Image);

				while(t != last)
				{
					t = t.Next;
					sb.Append(t.Image);
				}

				return sb.ToString();
			}
		}

		public virtual Object Init(IInternalContextAdapter context, Object data)
		{
			/*
	    * hold onto the RuntimeServices
	    */

			runtimeServices = (IRuntimeServices) data;

			int i, k = ChildrenCount;

			for(i = 0; i < k; i++)
			{
				try
				{
					GetChild(i).Init(context, data);
				}
				catch(ReferenceException re)
				{
					runtimeServices.Error(re);
				}
			}

			return data;
		}

		public virtual bool Evaluate(IInternalContextAdapter context)
		{
			return false;
		}

		public virtual Object Value(IInternalContextAdapter context)
		{
			return null;
		}

		public virtual bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			int i, k = ChildrenCount;

			for(i = 0; i < k; i++)
			{
				GetChild(i).Render(context, writer);
			}

			return true;
		}

		public virtual Object Execute(Object o, IInternalContextAdapter context)
		{
			return null;
		}

		public bool IsInvalid
		{
			get { return invalid; }
			set { invalid = true; }
		}
	}
}