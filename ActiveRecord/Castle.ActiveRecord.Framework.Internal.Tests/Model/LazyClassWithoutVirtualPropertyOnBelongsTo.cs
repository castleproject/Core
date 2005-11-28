using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
    [ActiveRecord(Lazy=true)]
    public class LazyClassWithoutVirtualPropertyOnBelongsTo
    {
        int id;
        ClassWithAnyAttribute clazz;

        [PrimaryKey(PrimaryKeyType.Native)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [BelongsTo(typeof(ClassWithAnyAttribute))]
        public ClassWithAnyAttribute Clazz
        {
            get { return clazz; }
            set { clazz = value; }
        }
    }
}
