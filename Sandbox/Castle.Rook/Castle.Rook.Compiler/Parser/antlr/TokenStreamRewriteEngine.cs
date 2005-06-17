namespace antlr
{
	/* ANTLR Translator Generator
	 * Project led by Terence Parr at http://www.jGuru.com
	 * Software rights: http://www.antlr.org/license.html
	 */

	//
	// ANTLR C# Code Generator by Micheal Jordan
	//                            Kunle Odutola       : kunle UNDERSCORE odutola AT hotmail DOT com
	//                            Anthony Oguntimehin
	//

	using System;
	using IList 		= System.Collections.IList;
	using IDictionary 	= System.Collections.IDictionary;
	using ArrayList 	= System.Collections.ArrayList;
	using Hashtable		= System.Collections.Hashtable;
	using IComparer		= System.Collections.IComparer;
	using StringBuilder = System.Text.StringBuilder;
	using BitSet 		= antlr.collections.impl.BitSet;

	/// <summary>
	/// This token stream tracks the *entire* token stream coming from
	/// a lexer, but does not pass on the whitespace (or whatever else
	/// you want to discard) to the parser.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class can then be asked for the ith token in the input stream.
	/// Useful for dumping out the input stream exactly after doing some
	/// augmentation or other manipulations.  Tokens are index from 0..n-1
	/// </para>
	/// <para>
	/// You can insert stuff, replace, and delete chunks.  Note that the
	/// operations are done lazily--only if you convert the buffer to a
	/// string.  This is very efficient because you are not moving data around
	/// all the time.  As the buffer of tokens is converted to strings, the
	/// toString() method(s) check to see if there is an operation at the
	/// current index.  If so, the operation is done and then normal string
	/// rendering continues on the buffer.  This is like having multiple Turing
	/// machine instruction streams (programs) operating on a single input tape. :)
	/// </para>
	/// <para>
	/// Since the operations are done lazily at toString-time, operations do not
	/// screw up the token index values.  That is, an insert operation at token
	/// index i does not change the index values for tokens i+1..n-1.
	/// </para>
	/// <para>
	/// Because operations never actually alter the buffer, you may always get
	/// the original token stream back without undoing anything.  Since
	/// the instructions are queued up, you can easily simulate transactions and
	/// roll back any changes if there is an error just by removing instructions.
	/// For example,
	/// </para>
	/// <example>For example:
	/// <code>
	/// TokenStreamRewriteEngine rewriteEngine = new TokenStreamRewriteEngine(lexer);
	/// JavaRecognizer           parser        = new JavaRecognizer(rewriteEngine);
	/// ...
	/// rewriteEngine.insertAfter("pass1", t, "foobar");}
	/// rewriteEngine.insertAfter("pass2", u, "start");}
	/// System.Console.Out.WriteLine(rewriteEngine.ToString("pass1"));
	/// System.Console.Out.WriteLine(rewriteEngine.ToString("pass2"));
	/// </code>
	/// </example>
	/// <para>
	/// You can also have multiple "instruction streams" and get multiple
	/// rewrites from a single pass over the input.  Just name the instruction
	/// streams and use that name again when printing the buffer.  This could be
	/// useful for generating a C file and also its header file--all from the
	/// same buffer.
	/// </para>
	/// <para>
	/// If you don't use named rewrite streams, a "default" stream is used.
	/// </para>
	/// <para>
	/// Terence Parr, parrt@cs.usfca.edu
	/// University of San Francisco
	/// February 2004
	/// </para>
	/// </remarks>
	public class TokenStreamRewriteEngine : TokenStream
	{
		public const int MIN_TOKEN_INDEX = 0;

		protected class RewriteOperation 
		{
			protected internal int 		index;
			protected internal string 	text;
			
			protected RewriteOperation(int index, string text) 
			{
				this.index = index;
				this.text  = text;
			}
			
			/// <summary>
			/// Execute the rewrite operation by possibly adding to the buffer.
			/// </summary>
			/// <param name="buf">rewrite buffer</param>
			/// <returns>The index of the next token to operate on.</returns>
			public virtual int execute(StringBuilder buf) 
			{
				return index;
			}
		}

		protected class InsertBeforeOp : RewriteOperation 
		{
			public InsertBeforeOp(int index, string text) : base(index, text)
			{
			}
			
			public override int execute(StringBuilder buf) 
			{
				buf.Append(text);
				return index;
			}
		}

		protected class ReplaceOp : RewriteOperation 
		{
			protected int lastIndex;
			
			public ReplaceOp(int from, int to, string text) : base(from, text)
			{
				lastIndex = to;
			}
			
			public override int execute(StringBuilder buf) 
			{
				if ( text != null ) 
				{
					buf.Append(text);
				}
				return lastIndex+1;
			}
		}

		protected class DeleteOp : ReplaceOp 
		{
			public DeleteOp(int from, int to) : base(from, to, null)
			{
			}
		}

		public const string 	DEFAULT_PROGRAM_NAME = "default";
		public const int 		PROGRAM_INIT_SIZE 	= 100;

		/// <summary>
		/// Track the incoming list of tokens
		/// </summary>
		protected IList tokens;

		/// <summary>
		/// You may have multiple, named streams of rewrite operations.
		/// I'm calling these things "programs."
		/// Maps string (name) -> rewrite (List)
		/// </summary>
		protected IDictionary programs = null;

		/// <summary>
		/// Map string (program name) -> Integer index
		/// </summary>
		protected IDictionary lastRewriteTokenIndexes = null;

		/// <summary>
		/// track index of tokens
		/// </summary>
		protected int index = MIN_TOKEN_INDEX;

		/// <summary>
		/// Who do we suck tokens from?
		/// </summary>
		protected TokenStream stream;

		/// <summary>
		/// Which (whitespace) token(s) to throw out
		/// </summary>
		protected BitSet discardMask = new BitSet();

		public TokenStreamRewriteEngine(TokenStream upstream) : this(upstream, 1000)
		{
		}

		public TokenStreamRewriteEngine(TokenStream upstream, int initialSize) 
		{
			stream   = upstream;
			tokens   = new ArrayList(initialSize);
			programs = new Hashtable();
			programs[DEFAULT_PROGRAM_NAME]	= new ArrayList(PROGRAM_INIT_SIZE);
			lastRewriteTokenIndexes			= new Hashtable();
		}

		public IToken nextToken() 				// throws TokenStreamException 
		{
			TokenWithIndex t;

			// suck tokens until end of stream or we find a non-discarded token
			do 
			{
				t = (TokenWithIndex) stream.nextToken();
				if ( t != null ) 
				{
					t.setIndex(index);  // what is t's index in list?
					if ( t.Type != Token.EOF_TYPE ) 
					{
						tokens.Add(t);  // track all tokens except EOF
					}
					index++;			// move to next position
				}
			} while ( (t != null) && (discardMask.member(t.Type)) );

			return t;
		}

		public void rollback(int instructionIndex) 
		{
			rollback(DEFAULT_PROGRAM_NAME, instructionIndex);
		}

		/// <summary>
		/// Rollback the instruction stream for a program so that
		/// the indicated instruction (via instructionIndex) is no
		/// longer in the stream.
		/// </summary>
		/// <remarks>
		/// UNTESTED!
		/// </remarks>
		/// <param name="programName"></param>
		/// <param name="instructionIndex"></param>
		public void rollback(string programName, int instructionIndex) 
		{
			ArrayList il = (ArrayList) programs[programName];
			if ( il != null ) 
			{
				programs[programName] = il.GetRange(MIN_TOKEN_INDEX, (instructionIndex - MIN_TOKEN_INDEX));
			}
		}

		public void deleteProgram() 
		{
			deleteProgram(DEFAULT_PROGRAM_NAME);
		}

		/// <summary>
		/// Reset the program so that no instructions exist
		/// </summary>
		/// <param name="programName"></param>
		public void deleteProgram(string programName) 
		{
			rollback(programName, MIN_TOKEN_INDEX);
		}

		/// <summary>
		/// If op.index > lastRewriteTokenIndexes, just add to the end.
		/// Otherwise, do linear
		/// </summary>
		/// <param name="op"></param>
		protected void addToSortedRewriteList(RewriteOperation op) 
		{
			addToSortedRewriteList(DEFAULT_PROGRAM_NAME, op);
		}

		protected void addToSortedRewriteList(string programName, RewriteOperation op) 
		{
			ArrayList rewrites = (ArrayList) getProgram(programName);
			// if at or beyond last op's index, just append
			if ( op.index >= getLastRewriteTokenIndex(programName) ) 
			{
				rewrites.Add(op); // append to list of operations
				// record the index of this operation for next time through
				setLastRewriteTokenIndex(programName, op.index);
				return;
			}
			// not after the last one, so must insert to ordered list
			int pos = rewrites.BinarySearch(op, RewriteOperationComparer.Default);
			if (pos < 0) 
			{
				rewrites.Insert(-pos-1, op);
			}
		}

		public void insertAfter(IToken t, string text) 
		{
			insertAfter(DEFAULT_PROGRAM_NAME, t, text);
		}

		public void insertAfter(int index, string text) 
		{
			insertAfter(DEFAULT_PROGRAM_NAME, index, text);
		}

		public void insertAfter(string programName, IToken t, string text) 
		{
			insertAfter(programName,((TokenWithIndex) t).getIndex(), text); 
		}

		public void insertAfter(string programName, int index, string text) 
		{
			// to insert after, just insert before next index (even if past end)
			insertBefore(programName, index+1, text); 
		}

		public void insertBefore(IToken t, string text) 
		{
			insertBefore(DEFAULT_PROGRAM_NAME, t, text);
		}

		public void insertBefore(int index, string text) 
		{
			insertBefore(DEFAULT_PROGRAM_NAME, index, text);
		}

		public void insertBefore(string programName, IToken t, string text) 
		{
			insertBefore(programName, ((TokenWithIndex) t).getIndex(), text);
		}

		public void insertBefore(string programName, int index, string text) 
		{
			addToSortedRewriteList(programName, new InsertBeforeOp(index, text));
		}

		public void replace(int index, string text) 
		{
			replace(DEFAULT_PROGRAM_NAME, index, index, text);
		}

		public void replace(int from, int to, string text) 
		{
			replace(DEFAULT_PROGRAM_NAME, from, to, text);
		}

		public void replace(IToken indexT, string text) 
		{
			replace(DEFAULT_PROGRAM_NAME, indexT, indexT, text);
		}

		public void replace(IToken from, IToken to, string text) 
		{
			replace(DEFAULT_PROGRAM_NAME, from, to, text);
		}

		public void replace(string programName, int from, int to, string text) 
		{
			addToSortedRewriteList(new ReplaceOp(from, to, text));
		}

		public void replace(string programName, IToken from, IToken to, string text) 
		{
			replace(programName,
				((TokenWithIndex) from).getIndex(),
				((TokenWithIndex) to).getIndex(),
				text);
		}

		public void delete(int index) 
		{
			delete(DEFAULT_PROGRAM_NAME, index, index);
		}

		public void delete(int from, int to) 
		{
			delete(DEFAULT_PROGRAM_NAME, from, to);
		}

		public void delete(IToken indexT) 
		{
			delete(DEFAULT_PROGRAM_NAME, indexT, indexT);
		}

		public void delete(IToken from, IToken to) 
		{
			delete(DEFAULT_PROGRAM_NAME, from, to);
		}

		public void delete(string programName, int from, int to) 
		{
			replace(programName, from, to, null);
		}

		public void delete(string programName, IToken from, IToken to) 
		{
			replace(programName, from, to, null);
		}

		public void discard(int ttype) 
		{
			discardMask.add(ttype);
		}

		public TokenWithIndex getToken(int i) 
		{
			return (TokenWithIndex) tokens[i];
		}

		public int getTokenStreamSize() 
		{
			return tokens.Count;
		}

		public string ToOriginalString() 
		{
			return ToOriginalString(MIN_TOKEN_INDEX, getTokenStreamSize()-1);
		}

		public string ToOriginalString(int start, int end) 
		{
			StringBuilder buf = new StringBuilder();
			for (int i = start; (i >= MIN_TOKEN_INDEX) && (i <= end) && (i < tokens.Count); i++) 
			{
				buf.Append(getToken(i).getText());
			}
			return buf.ToString();
		}

		public override string ToString() 
		{
			return ToString(MIN_TOKEN_INDEX, getTokenStreamSize());
		}

		public string ToString(string programName) 
		{
			return ToString(programName, MIN_TOKEN_INDEX, getTokenStreamSize());
		}

		public string ToString(int start, int end) 
		{
			return ToString(DEFAULT_PROGRAM_NAME, start, end);
		}

		public string ToString(string programName, int start, int end) 
		{
			IList rewrites = (IList) programs[programName];
			if (rewrites == null) 
			{
				return null; // invalid program
			}
			StringBuilder buf = new StringBuilder();

			// Index of first rewrite we have not done
			int rewriteOpIndex = 0;

			int tokenCursor = start;
			while ( (tokenCursor >= MIN_TOKEN_INDEX) &&
				(tokenCursor <= end) &&
				(tokenCursor < tokens.Count) )
			{
				if (rewriteOpIndex < rewrites.Count) 
				{
					RewriteOperation op = (RewriteOperation) rewrites[rewriteOpIndex];
					while ( (tokenCursor == op.index) && (rewriteOpIndex < rewrites.Count) ) 
					{
						/*
						Console.Out.WriteLine("execute op "+rewriteOpIndex+
											" (type "+op.GetType().FullName+")"
											+" at index "+op.index);
						*/
						tokenCursor = op.execute(buf);
						rewriteOpIndex++;
						if (rewriteOpIndex < rewrites.Count) 
						{
							op = (RewriteOperation) rewrites[rewriteOpIndex];
						}
					}
				}
				if ( tokenCursor < end ) 
				{
					buf.Append(getToken(tokenCursor).getText());
					tokenCursor++;
				}
			}
			// now see if there are operations (append) beyond last token index
			for (int opi = rewriteOpIndex; opi < rewrites.Count; opi++) 
			{
				RewriteOperation op = (RewriteOperation) rewrites[opi];
				op.execute(buf); // must be insertions if after last token
			}

			return buf.ToString();
		}

		public string ToDebugString() 
		{
			return ToDebugString(MIN_TOKEN_INDEX, getTokenStreamSize());
		}

		public string ToDebugString(int start, int end) 
		{
			StringBuilder buf = new StringBuilder();
			for (int i = start; (i >= MIN_TOKEN_INDEX) && (i <= end) && (i < tokens.Count); i++) 
			{
				buf.Append(getToken(i));
			}
			return buf.ToString();
		}

		public int getLastRewriteTokenIndex() 
		{
			return getLastRewriteTokenIndex(DEFAULT_PROGRAM_NAME);
		}

		protected int getLastRewriteTokenIndex(string programName) 
		{
			object i = lastRewriteTokenIndexes[programName];
			if (i == null)
			{
				return -1;
			}
			return (int) i;
		}

		protected void setLastRewriteTokenIndex(string programName, int i) 
		{
			lastRewriteTokenIndexes[programName] = (object) i;
		}

		protected IList getProgram(string name) 
		{
			IList il = (IList) programs[name];
			if ( il == null ) 
			{
				il = initializeProgram(name);
			}
			return il;
		}

		private IList initializeProgram(string name) 
		{
			IList il = new ArrayList(PROGRAM_INIT_SIZE);
			programs[name] = il;
			return il;
		}

		public class RewriteOperationComparer : IComparer
		{
			public static readonly RewriteOperationComparer Default = new RewriteOperationComparer();
		
			public virtual int Compare(object o1, object o2)
			{
				RewriteOperation rop1 = (RewriteOperation) o1;
				RewriteOperation rop2 = (RewriteOperation) o2;

				if (rop1.index < rop2.index) return -1;
				if (rop1.index > rop2.index) return 1;
				return 0;
			}
		}
	}
}
