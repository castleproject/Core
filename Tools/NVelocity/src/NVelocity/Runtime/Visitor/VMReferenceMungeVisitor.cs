// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.Runtime.Visitor
{
	using System;
	using System.Collections;
	using NVelocity.Runtime.Parser.Node;

	/// <summary>
	/// This class is a visitor used by the VM proxy to change the
	/// literal representation of a reference in a VM.  The reason is
	/// to preserve the 'render literal if null' behavior w/o making
	/// the VMProxy stuff more complicated than it is already.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: VMReferenceMungeVisitor.cs,v 1.3 2003/10/27 13:54:11 corts Exp $</version>
	public class VMReferenceMungeVisitor : BaseVisitor
	{
		/// <summary>
		/// Map containing VM arg to instance-use reference
		/// Passed in with CTOR
		/// </summary>
		private Hashtable argumentMap = null;

		/// <summary>
		/// CTOR - takes a map of args to reference
		/// </summary>
		public VMReferenceMungeVisitor(Hashtable map)
		{
			argumentMap = map;
		}

		/// <summary>
		/// Visitor method - if the literal is right, will
		/// set the literal in the ASTReference node
		/// </summary>
		/// <param name="node">ASTReference to work on</param>
		/// <param name="data">Object to pass down from caller</param>
		public override Object Visit(ASTReference node, Object data)
		{
			// see if there is an override value for this
			// reference
			String overrideVal = (String) argumentMap[node.Literal.Substring(1)];

			// if so, set in the node
			if (overrideVal != null)
				node.SetLiteral(overrideVal);

			// feed the children...
			data = node.ChildrenAccept(this, data);

			return data;
		}
	}
}