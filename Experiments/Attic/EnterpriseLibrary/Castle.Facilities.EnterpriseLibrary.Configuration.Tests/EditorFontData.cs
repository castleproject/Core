//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Configuration Application Block
//===============================================================================
// Copyright © 2004 Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

namespace ConfigurationQuickStart
{
	using System.Text;

	/// <summary>
	/// Summary description for ConfigurationData.
	/// </summary>
	public class EditorFontData
	{		
		private string  name;
		private float   size;
		private int		style;

		public EditorFontData()
		{          
		}

		public string Name 
		{
			get{ return name; }
			set{ name = value; }
		} 
		
		public float Size 
		{
			get{ return size; }
			set{ size = value; }
		} 

		//[XmlElement("FontSize")]
		public int Style 
		{
			get{ return style; }
			set{ style = value; }
		} 

		public override string ToString() 
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Name = {0}; Size = {1}; Style = {2}", name, size.ToString(), style.ToString());

			return sb.ToString();
		}
	}
}
