using System;

using System.Text;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord(DiscriminatorValue="b")]
	class DiscriminatorGrandchild : ClassDiscriminatorA
	{
	}
}
