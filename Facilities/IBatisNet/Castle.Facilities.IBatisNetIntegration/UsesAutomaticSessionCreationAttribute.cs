#region License

/// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --

#endregion

namespace Castle.Facilities.IBatisNetIntegration
{
	using System;

	/// <summary>
	/// Declares that a component wants to use an IBatis's Data Mapper session.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class UsesAutomaticSessionCreationAttribute : Attribute
	{
		private String _sqlMapId;

		public UsesAutomaticSessionCreationAttribute() {}

		public UsesAutomaticSessionCreationAttribute(String sqlMapId)
		{
			_sqlMapId = sqlMapId;
		}

		public String SqlMapId
		{
			get { return _sqlMapId; }
		}
	}
}