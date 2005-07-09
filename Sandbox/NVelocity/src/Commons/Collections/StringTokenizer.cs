using System;
using System.Collections;

namespace Commons.Collections {

    public class StringTokenizer {
	private System.Collections.ArrayList elements;
	private string source;
	//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
	private string delimiters = " \t\n\r";

	public StringTokenizer(string source) {
	    this.elements = new ArrayList();
	    this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
	    this.RemoveEmptyStrings();
	    this.source = source;
	}

	public StringTokenizer(string source, string delimiters) {
	    this.elements = new ArrayList();
	    this.delimiters = delimiters;
	    this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
	    this.RemoveEmptyStrings();
	    this.source = source;
	}

	public int Count {
	    get {
		return (this.elements.Count);
	    }
	}

	public bool HasMoreTokens() {
	    return (this.elements.Count > 0);
	}

	public string NextToken() {
	    string result;
	    if (source == "") {
		throw new System.Exception();
	    } else {
		this.elements = new ArrayList();
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
}
