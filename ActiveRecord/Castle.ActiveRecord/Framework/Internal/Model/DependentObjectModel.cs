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

namespace Castle.ActiveRecord.Framework.Internal
{
    using System.Reflection;

    ///<summary>
    /// This model is used to represent a dependent object value type (&lt;composite-element/&gt; - in NHibernate talk).
    ///</summary>
    public class DependentObjectModel : IVisitable
    {
    	private readonly HasManyAttribute hasManyAtt;
        private readonly ActiveRecordModel dependentObjectModel;


        ///<summary>
        /// Initializes a new instance of the <see cref="DependentObjectModel"/> class.
        ///</summary>
        /// <param name="propInfo">The prop info.</param>
        /// <param name="hasManyAtt">The nested att.</param>
        /// <param name="dependentObjectModel">The nested model.</param>
        public DependentObjectModel(PropertyInfo propInfo, HasManyAttribute hasManyAtt, ActiveRecordModel dependentObjectModel)
        {
        	this.dependentObjectModel = dependentObjectModel;
            this.hasManyAtt = hasManyAtt;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ActiveRecordModel Model
        {
            get { return dependentObjectModel; }
        }
        /// <summary>
        /// Gets the has many attribute
        /// </summary>
        /// <value>The has many att.</value>
        public HasManyAttribute HasManyAtt
        {
            get { return hasManyAtt; }
        }

        #region IVisitable Members

        /// <summary>
        /// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.VisitDependentObject(this);
        }

        #endregion
    }
}
