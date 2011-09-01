//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml.XPath;

//namespace Castle.Components.DictionaryAdapter.Xml
//{
//    public abstract class XmlNodeCollection : IConfigurable<object>
//    {
//        private readonly XmlTypedNode parentNode;
//        private readonly XmlGetterContext context;

//        protected XmlNodeCollection(XmlTypedNode parentNode, XmlGetterContext context)
//        {
//            this.parentNode = parentNode;
//            this.context = context;
//        }

//        public XmlIterator SelectItems()
//        {
//            throw new NotImplementedException();
//        }

//        protected void Populate()
//        {
//            context.Accessor.GetCollectionItems(parentNode.Node, context.ParentObject, this);
//        }

//        public void Configure(object item)
//        {
//            AddExisting(item);
//        }

//        protected abstract void AddExisting(object item);

//        protected object Deserialize(XmlCollectionItem item)
//        {
//            return item.Object
//                ?? (item.Object = context.Accessor.Serializer.GetValue(item.TypedNode,
//                    new XmlGetterContext(context.Accessor, context.ParentObject, context.VirtualEnabled)));
//        }

//        protected void Serialize(XmlCollectionItem item, object value)
//        {
//            throw new NotImplementedException();
//        }

//        protected XmlTypedNode Add(object value)
//        {
//            throw new NotImplementedException();
//        }

//        protected XmlTypedNode InsertBefore(object value, XmlTypedNode beforeNode)
//        {
//            throw new NotImplementedException();
//        }

//        protected void Remove(XmlTypedNode node)
//        {
//            throw new NotImplementedException();
//        }

//        protected void Clear()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
