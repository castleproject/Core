using System;
public class SupportClass {
    public class Tokenizer {
	private System.Collections.ArrayList elements;
	private string source;
	//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
	private string delimiters = " \t\n\r";

	public Tokenizer(string source) {
	    this.elements = new System.Collections.ArrayList();
	    this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
	    this.RemoveEmptyStrings();
	    this.source = source;
	}

	public Tokenizer(string source, string delimiters) {
	    this.elements = new System.Collections.ArrayList();
	    this.delimiters = delimiters;
	    this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
	    this.RemoveEmptyStrings();
	    this.source = source;
	}

	public int Count
	{
	    get {
		return (this.elements.Count);
	    }
	}

	public bool HasMoreTokens() {
	    return (this.elements.Count > 0);
	}

	public string NextToken() {
	    string result;
	    if (source == "")
		throw new System.Exception();
	    else {
		this.elements = new System.Collections.ArrayList();
		this.elements.AddRange(this.source.Split(delimiters.ToCharArray()));
		RemoveEmptyStrings();
		result = (string) this.elements[0];
		this.elements.RemoveAt(0);
		this.source = this.source.Replace(result,"");
		this.source = this.source.TrimStart(this.delimiters.ToCharArray());
		return result;
	    }
	}

	public string NextToken(string delimiters) {
	    this.delimiters = delimiters;
	    return NextToken();
	}

	private void RemoveEmptyStrings() {
	    //VJ++ does not treat empty strings as tokens
	    for (int index=0; index < this.elements.Count; index++)
		if ((string)this.elements[index]== "") {
		    this.elements.RemoveAt(index);
		    index--;
		}
	}
    }

    /*******************************/
    public static System.Object PutElement(System.Collections.Hashtable hashTable, System.Object key, System.Object newValue) {
	System.Object element = hashTable[key];
	hashTable[key] = newValue;
	return element;
    }

    /*******************************/
    public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream) {
	stream.Write(throwable.StackTrace);
	stream.Flush();
    }

}
