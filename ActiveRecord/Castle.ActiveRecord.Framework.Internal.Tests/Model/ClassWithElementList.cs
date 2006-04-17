using System;
using System.Collections;
using System.Text;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
    [ActiveRecord]
    public class ClassWithElementList
    {
        int id;
        IList elements;

        [PrimaryKey]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [HasMany(typeof(string), "ClassId", "Elements", Element = "Name")]
        public IList Elements
        {
            get { return elements; }
            set { elements = value; }
        }
    }
}
