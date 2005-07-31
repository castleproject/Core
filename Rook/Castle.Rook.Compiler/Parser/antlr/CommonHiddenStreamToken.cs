using System;

namespace antlr
{
	/*ANTLR Translator Generator
	* Project led by Terence Parr at http://www.jGuru.com
	* Software rights: http://www.antlr.org/license.html
	*
	* $Id:$
	*/

	//
	// ANTLR C# Code Generator by Micheal Jordan
	//                            Kunle Odutola       : kunle UNDERSCORE odutola AT hotmail DOT com
	//                            Anthony Oguntimehin
	//
	// With many thanks to Eric V. Smith from the ANTLR list.
	//
	
	public class CommonHiddenStreamToken : CommonToken, IHiddenStreamToken
	{
		new public static readonly CommonHiddenStreamToken.CommonHiddenStreamTokenCreator Creator = new CommonHiddenStreamTokenCreator();

		protected internal IHiddenStreamToken hiddenBefore;
		protected internal IHiddenStreamToken hiddenAfter;
		
		public CommonHiddenStreamToken() : base()
		{
		}
		
		public CommonHiddenStreamToken(int t, string txt) : base(t, txt)
		{
		}
		
		public CommonHiddenStreamToken(string s) : base(s)
		{
		}
		
		public virtual IHiddenStreamToken getHiddenAfter()
		{
			return hiddenAfter;
		}
		
		public virtual IHiddenStreamToken getHiddenBefore()
		{
			return hiddenBefore;
		}
		
		public virtual void  setHiddenAfter(IHiddenStreamToken t)
		{
			hiddenAfter = t;
		}
		
		public virtual void  setHiddenBefore(IHiddenStreamToken t)
		{
			hiddenBefore = t;
		}

		public class CommonHiddenStreamTokenCreator : TokenCreator
		{
			public CommonHiddenStreamTokenCreator() {}

			/// <summary>
			/// Returns the fully qualified name of the Token type that this
			/// class creates.
			/// </summary>
			public override string TokenTypeName
			{
				get 
				{ 
					return typeof(antlr.CommonHiddenStreamToken).FullName;; 
				}
			}

			/// <summary>
			/// Constructs a <see cref="Token"/> instance.
			/// </summary>
			public override IToken Create()
			{
				return new CommonHiddenStreamToken();
			}
		}
	}
}