// ${Copyrigth}

namespace Castle.CastleOnRails.Framework
{
	using System;

	[Serializable]
	public class HelperAttribute : Attribute
	{
		private readonly Type _helperType;

		public HelperAttribute(Type helperType)
		{
			_helperType = helperType;
		}

		public Type HelperType
		{
			get
			{
				return _helperType;
			}
		}
	}
}
