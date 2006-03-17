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

using Directive = NVelocity.Runtime.Directive.Directive;
using DirectiveType = NVelocity.Runtime.Directive.DirectiveType;
using IInternalContextAdapter = NVelocity.Context.IInternalContextAdapter;
using INode = NVelocity.Runtime.Parser.Node.INode;
using IRuntimeServices = NVelocity.Runtime.IRuntimeServices;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.Collections;
	using System.IO;

	public class SubSectionDirective : Directive
	{
		private readonly string name;
		private NVelocityViewContextAdapter contextAdapter;
		private INode savedNode;

		public SubSectionDirective(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// Return the name of this directive
		/// </summary>
		public override String Name
		{
			get { return name; }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Get the directive type BLOCK/LINE
		/// </summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}

		public override bool AcceptParams
		{
			get { return false; }
		}

		/// <summary>
		/// How this directive is to be initialized.
		/// </summary>
		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			savedNode = node;
		}

		/// <summary>
		/// How this directive is to be rendered
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			foreach(DictionaryEntry entry in contextAdapter.ContextVars)
			{
				context.Put(entry.Key.ToString(), entry.Value);
			}

			for(int i=0; i < savedNode.ChildrenCount; i++)
			{
				INode childNode = savedNode.GetChild(i);
				childNode.Render(context, writer);
			}

			return true;
		}

		internal NVelocityViewContextAdapter ContextAdapter
		{
			set { contextAdapter = value; }
		}
	}
}
