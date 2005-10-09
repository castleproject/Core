using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord(DiscriminatorValue="b")]
	class DiscriminatorGrandchild : ClassDiscriminatorA
	{
	}
}
