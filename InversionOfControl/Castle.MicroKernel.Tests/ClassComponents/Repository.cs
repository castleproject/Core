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

namespace Castle.MicroKernel.Tests.ClassComponents
{
	public interface IRepository { }

	public class DecoratedRepository : IRepository
	{
		public DecoratedRepository() { }
	}

	public class Repository1 : IRepository
	{
		private IRepository _inner;

		public IRepository InnerRepository
		{
			get { return _inner; }
		}

		public Repository1(IRepository inner)
		{
			_inner = inner;
		}
	}

	public class Repository2 : IRepository
	{
		private IRepository _inner;

		public IRepository InnerRepository
		{
			get { return _inner; }
		}

		public Repository2(IRepository inner)
		{
			_inner = inner;
		}
	}

	public class Repository3 : IRepository
	{
		private IRepository _inner;

		public IRepository InnerRepository
		{
			get { return _inner; }
		}

		public Repository3(IRepository inner)
		{
			_inner = inner;
		}
	}
}
