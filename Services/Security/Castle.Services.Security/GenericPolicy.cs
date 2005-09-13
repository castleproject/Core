using System;

namespace Castle.Services.Security
{
	/// <summary>
	/// Summary description for GenericPolicy.
	/// </summary>
	public class GenericPolicy : IPolicy
	{
		public GenericPolicy()
		{
			//
			// TODO: Add constructor logic here
			//
        }
        #region IPolicy Members

        public bool Evaluate()
        {
            // TODO:  Add GenericPolicy.Evaluate implementation
            return false;
        }

        #endregion
    }
}
