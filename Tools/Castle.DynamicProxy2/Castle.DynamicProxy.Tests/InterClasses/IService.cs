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

namespace Castle.DynamicProxy.Tests.InterClasses
{
	using System;

	public interface IService
	{
		int Sum(int b1, int b2);

//		byte Sum(byte b1, byte b2);
//
//		long Sum(long b1, long b2);
//
//		short Sum(short b1, short b2);
//
//		float Sum(float b1, float b2);
//
//		double Sum(double b1, double b2);
//
//		UInt16 Sum(UInt16 b1, UInt16 b2);
//
//		UInt32 Sum(UInt32 b1, UInt32 b2);
//
//		UInt64 Sum(UInt64 b1, UInt64 b2);

		bool Valid { get; }
	}
}
