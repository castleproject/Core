// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal
{
	/// <summary>
	/// Model for importing classes so HQL queries can use them more easily.
	/// </summary>
	public class ImportModel : IModelNode
	{
		private readonly ImportAttribute att;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportModel"/> class.
		/// </summary>
		/// <param name="att">The att.</param>
		public ImportModel(ImportAttribute att)
		{
			this.att = att;
		}

		/// <summary>
		/// Gets the import attribute
		/// </summary>
		/// <value>The import att.</value>
		public ImportAttribute ImportAtt
		{
			get { return att; }
		}

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitImport(this);
		}
	}
}