namespace antlr
{
	using System;
	using AST		= antlr.collections.AST;

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

	/// <summary>
	/// A creator of AST node instances.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class and it's sub-classes exists primarily as an optimization
	/// of the reflection-based mechanism(s) previously used exclusively to 
	/// create instances of AST node objects.
	/// </para>
	/// <para>
	/// Parsers and TreeParsers already use the ASTFactory class in ANTLR whenever
	/// they need to create an AST node objeect. What this class does is to support
	/// performant extensibility of the basic ASTFactory. The ASTFactory can now be
	/// extnded as run-time to support more new AST node types without using needing
	/// to use reflection.
	/// </para>
	/// </remarks>
	public abstract class ASTNodeCreator
	{
		/// <summary>
		/// Returns the fully qualified name of the AST type that this
		/// class creates.
		/// </summary>
		public abstract string ASTNodeTypeName
		{
			get;
		}

		/// <summary>
		/// Constructs an <see cref="AST"/> instance.
		/// </summary>
		public abstract AST Create();
	}
}