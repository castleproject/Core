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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;

	using Castle.Model.Configuration;


	public abstract class AbstractTypeConverter : ITypeConverter
	{
		private ITypeConverterContext _context;

		public ITypeConverterContext Context
		{
			get { return _context; }
			set { _context = value; }
		}

		public abstract bool CanHandleType(Type type);

		public abstract object PerformConversion(String value, Type targetType);

		public abstract object PerformConversion(IConfiguration configuration, Type targetType);
	}
}