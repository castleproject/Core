namespace Commons.Collections
{
	using System;
	using System.Collections;

	public class StringTokenizer
	{
		private ArrayList elements;
		private string source;
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
		private string delimiters = " \t\n\r";

		public StringTokenizer(string source)
		{
			elements = new ArrayList();
			elements.AddRange(source.Split(delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public StringTokenizer(string source, string delimiters)
		{
			elements = new ArrayList();
			this.delimiters = delimiters;
			elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public int Count
		{
			get { return (elements.Count); }
		}

		public virtual bool HasMoreTokens()
		{
			return (elements.Count > 0);
		}

		public virtual string NextToken()
		{
			string result;
			if (source == "")
			{
				throw new Exception();
			}
			else
			{
				elements = new ArrayList();
				elements.AddRange(source.Split(delimiters.ToCharArray()));
				RemoveEmptyStrings();
				result = (string) elements[0];
				elements.RemoveAt(0);
				source = source.Replace(result, "");
				source = source.TrimStart(delimiters.ToCharArray());
				return result;
			}
		}

		public string NextToken(string delimiters)
		{
			this.delimiters = delimiters;
			return NextToken();
		}

		private void RemoveEmptyStrings()
		{
			//VJ++ does not treat empty strings as tokens
			for(int index = 0; index < elements.Count; index++)
				if ((string) elements[index] == "")
				{
					elements.RemoveAt(index);
					index--;
				}
		}
	}
}