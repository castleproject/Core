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

namespace Castle.ActiveRecord
{
	using System;

	public enum CompositionType
	{
		OneToOne,
		Component,
		Subclass,
		JoinedSubclass
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class ComposedOfAttribute : Attribute
	{
		private CompositionType _composition = CompositionType.Subclass;
		private Type _proxy;
		private String _discriminator;
		private bool _dynamicInsert;
		private bool _dynamicUpdate;
		private String _key;
		private String _cascade;
		private bool _constrained;
		private String _outerJoin;

		public ComposedOfAttribute()
		{
		}

		public CompositionType Composition
		{
			get { return _composition; }
			set { _composition = value; }
		}

		public Type Proxy
		{
			get { return _proxy; }
			set { _proxy = value; }
		}

		public String Discriminator
		{
			get { return _discriminator; }
			set { _discriminator = value; }
		}

		public bool DynamicInsert
		{
			get { return _dynamicInsert; }
			set { _dynamicInsert = value; }
		}

		public bool DynamicUpdate
		{
			get { return _dynamicUpdate; }
			set { _dynamicUpdate = value; }
		}

		public String Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public String Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public bool Constrained
		{
			get { return _constrained; }
			set { _constrained = value; }
		}

		public String OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}
	}
}