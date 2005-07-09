namespace NVelocity.Util.Introspection {
    using System;
    /*
	* Copyright 2002-2004 The Apache Software Foundation.
	*
	* Licensed under the Apache License, Version 2.0 (the "License")
	* you may not use this file except in compliance with the License.
	* You may obtain a copy of the License at
	*
	*     http://www.apache.org/licenses/LICENSE-2.0
	*
	* Unless required by applicable law or agreed to in writing, software
	* distributed under the License is distributed on an "AS IS" BASIS,
	* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	* See the License for the specific language governing permissions and
	* limitations under the License.
	*/
	
    /// <summary>  Little class to carry in info such as template name, line and column
    /// for information error reporting from the uberspector implementations
    /// *
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: Info.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
    /// 
    /// </version>
    public class Info {
	virtual public System.String TemplateName {
	    get {
		return templateName;
	    }
			
	}
	virtual public int Line {
	    get {
		return line;
	    }
			
	}
	virtual public int Column {
	    get {
		return column;
	    }
			
	}
	private int line;
	private int column;
	private System.String templateName;
		
	/// <param name="source">Usually a template name.
	/// </param>
	/// <param name="line">The line number from <code>source</code>.
	/// </param>
	/// <param name="column">The column number from <code>source</code>.
	/// 
	/// </param>
	public Info(System.String source, int line, int column) {
	    this.templateName = source;
	    this.line = line;
	    this.column = column;
	}
		
	/// <summary> Force callers to set the location information.
	/// </summary>
	private Info() {
	}
		
		
		
		
	/// <summary> Formats a textual representation of this object as <code>SOURCE
	/// [line X, column Y]</code>.
	/// </summary>
	public override System.String ToString() {
	    return TemplateName + " [line " + Line + ", column " + Column + ']';
	}
    }
}