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

namespace Castle.MicroKernel.Tests.ClassComponents
{
	/// <summary>
	/// Summary description for IMapper.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMapper<T>
	{
	}

	/// <summary>
	/// Summary description for CommonImpl1Mapper.
	/// </summary>
	public class CommonImpl1Mapper : IMapper<CommonImpl1>
	{
	}

	/// <summary>
	/// Summary description for CommonImpl2Mapper.
	/// </summary>
	public class CommonImpl2Mapper : IMapper<CommonImpl2>
	{
	}
}