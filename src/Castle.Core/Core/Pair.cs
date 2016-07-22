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

namespace Castle.Core
{
	using System;

	/// <summary>
	/// General purpose class to represent a standard pair of values. 
	/// </summary>
	/// <typeparam name="TFirst">Type of the first value</typeparam>
	/// <typeparam name="TSecond">Type of the second value</typeparam>
	public class Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>
	{
		private readonly TFirst first;
		private readonly TSecond second;

		/// <summary>
		/// Constructs a pair with its values
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		public Pair(TFirst first, TSecond second)
		{
			this.first = first;
			this.second = second;
		}

		public TFirst First
		{
			get { return first; }
		}

		public TSecond Second
		{
			get { return second; }
		}

		public override string ToString()
		{
			return first + " " + second;
		}

		public bool Equals(Pair<TFirst, TSecond> other)
		{
			if (other == null)
			{
				return false;
			}
			return Equals(first, other.first) && Equals(second, other.second);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as Pair<TFirst, TSecond>);
		}

		public override int GetHashCode()
		{
			return first.GetHashCode() + 29 * second.GetHashCode();
		}
	}
}
