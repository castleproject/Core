// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.InterClasses
{
	using System;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class AlwaysThrowsServiceImpl : IService
	{
		public int Sum(int b1, int b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public byte Sum(byte b1, byte b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public long Sum(long b1, long b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public short Sum(short b1, short b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public float Sum(float b1, float b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public double Sum(double b1, double b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ushort Sum(ushort b1, ushort b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public uint Sum(uint b1, uint b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ulong Sum(ulong b1, ulong b2)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Valid
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
}