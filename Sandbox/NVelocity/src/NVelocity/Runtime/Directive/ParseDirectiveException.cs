namespace NVelocity.Runtime.Directive
{
	using System;
	using System.Collections;

	/// <summary> Exception for #parse() problems
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ParseDirectiveException.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	[Serializable]
	public class ParseDirectiveException : Exception
	{
		private void InitBlock()
		{
			filenameStack = new Stack();
		}

		public override String Message
		{
			get
			{
				String returnStr = "#parse() exception : depth = " + depthCount + " -> " + msg;

				returnStr += " File stack : ";

				try
				{
					while (!(filenameStack.Count == 0))
					{
						returnStr += (String) filenameStack.Pop();
						returnStr += " -> ";
					}
				}
				catch (Exception e)
				{
				}

				return returnStr;
			}

		}

		//UPGRADE_NOTE: The initialization of  'filenameStack' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		private Stack filenameStack;
		private String msg = "";
		private int depthCount = 0;

		/// <summary> Constructor
		/// </summary>
		internal ParseDirectiveException(String m, int i)
		{
			InitBlock();
			msg = m;
			depthCount = i;
		}

		/// <summary> Get a message.
		/// </summary>
		/// <summary> Add a file to the filename stack
		/// </summary>
		public void addFile(String s)
		{
			Object temp_object;
			temp_object = s;
			Object generatedAux = temp_object;
			filenameStack.Push(temp_object);
		}
	}
}