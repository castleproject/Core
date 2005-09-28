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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;


	public class FieldModel : PropertyModel
	{
		private readonly FieldInfo field;
		private readonly FieldAttribute att;

		public FieldModel(FieldInfo field, FieldAttribute att)
		{
			this.field = field;
			this.att = att;
		}

		public override string PropertyName
		{
			get { return field.Name; }
		}

		public override Type PropertyType
		{
			get { return field.FieldType; }
		}

		public override PropertyAttribute PropertyAtt
		{
			get { return att; }
		}

		public FieldAttribute FieldAtt
		{
			get { return att; }
		}
	}
}
