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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Model for representing a Composite User type map.
	/// </summary>
	public class CompositeUserTypeModel : IVisitable
	{
		private readonly MemberInfo member;
		private readonly Type memberType;
		private readonly CompositeUserTypeAttribute attribute;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeUserTypeModel"/> class.
		/// </summary>
		/// <param name="member">The member marked with the attribute.</param>
		/// <param name="memberType">The type of member marked with the attribute.</param>
		/// <param name="attribute">The metadata attribute.</param>
		public CompositeUserTypeModel(MemberInfo member, Type memberType, CompositeUserTypeAttribute attribute)
		{
			this.member = member;
			this.memberType = memberType;
			this.attribute = attribute;
		}

		/// <summary>
		/// Gets the member marked with the attribute.
		/// </summary>
		/// <value>The member.</value>
		public MemberInfo Member
		{
			get { return member; }
		}

		/// <summary>
		/// Gets the type of member marked with the attribute.
		/// </summary>
		/// <value>The member.</value>
		public Type MemberType
		{
			get { return memberType; }
		}

		/// <summary>
		/// Gets the attribute instance.
		/// </summary>
		/// <value>The attribute.</value>
		public CompositeUserTypeAttribute Attribute
		{
			get { return attribute; }
		}

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitCompositeUserType(this);
		}
	}
}
