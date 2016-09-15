// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections.Generic;
	using Castle.Core;

	public sealed class VirtualSite<TNode, TMember> :
		IVirtualSite<TNode>,
		IEquatable<VirtualSite<TNode, TMember>>
	{
		private readonly IVirtualTarget<TNode, TMember> target;
		private readonly TMember member;

		public VirtualSite(IVirtualTarget<TNode, TMember> target, TMember member)
		{
			this.target = target;
			this.member = member;
		}

		public IVirtualTarget<TNode, TMember> Target
		{
			get { return target; }
		}

		public TMember Member
		{
			get { return member; }
		}

		public void OnRealizing(TNode node)
		{
			target.OnRealizing(node, member);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as VirtualSite<TNode, TMember>);
		}

		public bool Equals(VirtualSite<TNode, TMember> other)
		{
			return other != null
				&& TargetComparer.Equals(target, other.target)
				&& MemberComparer.Equals(member, other.member);
		}

		public override int GetHashCode()
		{
			return 0x72F10A3D
				+ 37 * TargetComparer.GetHashCode(target)
				+ 37 * MemberComparer.GetHashCode(member);
		}

		private static readonly IEqualityComparer<IVirtualTarget<TNode, TMember>>
			TargetComparer = ReferenceEqualityComparer<IVirtualTarget<TNode, TMember>>.Instance;

		private static readonly IEqualityComparer<TMember>
			MemberComparer = EqualityComparer<TMember>.Default;
	}
}
