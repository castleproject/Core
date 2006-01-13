// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

using DirectiveConstants_Fields = NVelocity.Runtime.Directive.DirectiveConstants_Fields;

using INode = NVelocity.Runtime.Parser.Node.INode;
using Template = NVelocity.Template;
using Resource = NVelocity.Runtime.Resource.Resource;
using Directive = NVelocity.Runtime.Directive.Directive;
using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
using RuntimeConstants_Fields = NVelocity.Runtime.RuntimeConstants_Fields;
using RuntimeServices = NVelocity.Runtime.RuntimeServices;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
using ParserTreeConstants = NVelocity.Runtime.Parser.ParserTreeConstants;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;

	using Castle.MonoRail.Framework.Internal;

	#if NOT_COMPLETED
	public class HtmlDirective : Directive
	{
		public HtmlDirective(IViewComponentFactory viewComponentFactory) : base(viewComponentFactory)
		{
		}

		public override String Name
		{
			get { return "h"; }
			set {  }
		}

		public override int Type
		{
			get { return DirectiveConstants_Fields.LINE; }
		}
		
		public void render(NVelocity.Context.InternalContextAdapter ctx, System.IO.TextWriter w, NVelocity.Runtime.Parser.Node.INode node)
		{
			throw new NotImplementedException();
		}
	}
	#endif
}
