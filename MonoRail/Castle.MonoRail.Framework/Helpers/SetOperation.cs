// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// The SetOperation exposes an <see cref="IterateOnDataSource"/> that 
	/// extracts information from the attributes and creates a proper configured
	/// Iterator.
	/// <para>
	/// It is shared by a handful of MonoRail operations related to sets. 
	/// </para>
	/// </summary>
	public static class SetOperation
	{
		/// <summary>
		/// Combines a group of well thought rules to create 
		/// an <see cref="OperationState"/> instance.
		/// </summary>
		/// 
		/// <remarks>
		/// The parameters read from the <paramref name="attributes"/> are
		/// 
		/// <list type="bullet">
		/// <item>
		///		<term>value</term>
		///		<description>The property name used to extract the value</description>
		/// </item>
		/// <item>
		///		<term>text</term>
		///		<description>The property name used to extract the display text</description>
		/// </item>
		/// <item>
		///		<term>textformat</term>
		///		<description>A format rule to apply to the text</description>
		/// </item>
		/// <item>
		///		<term>valueformat</term>
		///		<description>A format rule to apply to the value</description>
		/// </item>
		/// <item>
		///		<term>suffix</term>
		///		<description>If the types on both sets are different, 
		///		the suffix specifies a different target property</description>
		/// </item>
		/// <item>
		///		<term>sourceProperty</term>
		///		<description>
		///		If the types on both sets are different, 
		///		the sourceProperty identifies a different source property to extract the value from.
		///		</description>
		/// </item>
		/// 
		/// </list>
		/// 
		/// </remarks>
		/// 
		/// <param name="initialSelection">The initial selection.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public static OperationState IterateOnDataSource(object initialSelection, 
		                                                 IEnumerable dataSource, IDictionary attributes)
		{
			// Extract necessary elements to know which "heuristic" to use

			bool isInitialSelectionASet = IsSet(initialSelection);

			Type initialSelectionType = ExtractType(initialSelection);
			Type dataSourceType = ExtractType(dataSource);

			String customSuffix = CommonUtils.ObtainEntryAndRemove(attributes, "suffix");
			String valueProperty = CommonUtils.ObtainEntryAndRemove(attributes, "value");
			String textProperty = CommonUtils.ObtainEntryAndRemove(attributes, "text");
			String textFormat = CommonUtils.ObtainEntryAndRemove(attributes, "textformat");
			String valueFormat = CommonUtils.ObtainEntryAndRemove(attributes, "valueformat");

			bool emptyValueCase = CheckForEmpyTextValueCase(dataSourceType);

			if (dataSourceType == null)
			{
				// If the dataSourceType could not be obtained 
				// then the datasource is empty or null

				return NoIterationState.Instance;
			}
			else if (initialSelectionType == null)
			{
				if (customSuffix == null && valueProperty != null)
				{
					customSuffix = valueProperty;
				}

				return new ListDataSourceState(dataSourceType, dataSource,
											   valueProperty, textProperty, textFormat, valueFormat, customSuffix);
			}
			else if (initialSelectionType == dataSourceType)
			{
				return new SameTypeOperationState(dataSourceType, 
				                                  initialSelection, dataSource,
												  emptyValueCase, valueProperty, textProperty, textFormat, valueFormat, isInitialSelectionASet);
			}
			else // types are different, most complex scenario
			{
				String sourceProperty = CommonUtils.ObtainEntryAndRemove(attributes, "sourceProperty");

				return new DifferentTypeOperationState(initialSelectionType,
													   dataSourceType, initialSelection, dataSource,
													   sourceProperty, valueProperty, textProperty, textFormat, valueFormat,
													   isInitialSelectionASet);
			}
		}

		private static bool CheckForEmpyTextValueCase(Type datasourceType)
		{
			if (typeof(Enum).IsAssignableFrom(datasourceType))
			{
				return true;
			}
			return false;
		}

		private static Type ExtractType(object source)
		{
			if (source == null)
			{
				return null;
			}
			else if (source is String)
			{
				return typeof(String);
			}
			else if (source is IEnumerable)
			{
				return ExtractType(source as IEnumerable);
			}
			else
			{
				return source.GetType();
			}
		}

		private static Type ExtractType(IEnumerable source)
		{
			if (source == null)
			{
				return null;
			}

			IEnumerator enumerator = source.GetEnumerator();

			if (enumerator.MoveNext())
			{
				return ExtractType(enumerator.Current);
			}

			return null;
		}

		private static bool IsSet(object initialSelection)
		{
			if (initialSelection == null)
			{
				return false;
			}

			return initialSelection.GetType() != typeof(String) && 
			       initialSelection is IEnumerable;
		}
	}

	/// <summary>
	/// Represents a set element
	/// </summary>
	public class SetItem
	{
		private readonly string value;
		private readonly object item;
		private readonly string text;
		private readonly bool isSelected;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetItem"/> class.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="value">The value.</param>
		/// <param name="text">The text.</param>
		/// <param name="isSelected">if set to <c>true</c> [is selected].</param>
		public SetItem(object item, String value, String text, bool isSelected)
		{
			this.item = item;
			this.value = value;
			this.text = text;
			this.isSelected = isSelected;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <value>The item.</value>
		public object Item
		{
			get { return item; }
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get { return value; }
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get { return text; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected
		{
			get { return isSelected; }
		}
	}

	/// <summary>
	/// Base class for set iterators
	/// </summary>
	public abstract class OperationState : IEnumerable, IEnumerator
	{
		/// <summary>
		/// source type
		/// </summary>
		protected readonly Type type;
		/// <summary>
		/// Value getter for value
		/// </summary>
		protected readonly FormHelper.ValueGetter valuePropInfo;
		/// <summary>
		/// Value getter for text
		/// </summary>
		protected readonly FormHelper.ValueGetter textPropInfo;

		/// <summary>
		/// Format rule for text
		/// </summary>
		protected readonly String textFormat;
		/// <summary>
		/// Format rule for value
		/// </summary>
		protected readonly String valueFormat;
		
		/// <summary>
		/// Source enumerator
		/// </summary>
		protected IEnumerator enumerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="OperationState"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="emptyValueCase">if set to <c>true</c> [empty value case].</param>
		/// <param name="valueProperty">The value property.</param>
		/// <param name="textProperty">The text property.</param>
		/// <param name="textFormat">The text format.</param>
		/// <param name="valueFormat">The value format.</param>
		protected OperationState(Type type, IEnumerable dataSource, 
			bool emptyValueCase,String valueProperty, String textProperty, String textFormat, String valueFormat)
		{
			if (dataSource != null)
			{
				enumerator = dataSource.GetEnumerator();
			}

			this.type = type;
			this.textFormat = textFormat;
			this.valueFormat = valueFormat;

			if (valueProperty != null || emptyValueCase) 
			{
				valuePropInfo = FormHelper.ValueGetterAbstractFactory.Create(type, valueProperty); // FormHelper.GetMethod(type, valueProperty);
			}

			if (textProperty != null)
			{
				textPropInfo = FormHelper.ValueGetterAbstractFactory.Create(type, textProperty); 
			}
		}

		/// <summary>
		/// Gets the target suffix.
		/// </summary>
		/// <value>The target suffix.</value>
		public abstract String TargetSuffix { get; }

		/// <summary>
		/// Formats the text.
		/// </summary>
		/// <param name="value">The value to be formatted.</param>
		/// <param name="format">The format to apply.</param>
		protected static void FormatText(ref object value, string format)
		{
			if (format != null && value != null)
			{
				IFormattable formattable = value as IFormattable;

				if (formattable != null)
				{
					value = formattable.ToString(format, null);
				}
			}
		}

		/// <summary>
		/// Creates the item representation.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <returns></returns>
		protected abstract SetItem CreateItemRepresentation(object current);

		#region IEnumerator implementation

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public bool MoveNext()
		{
			if (enumerator == null)
			{
				return false;
			}
			return enumerator.MoveNext();
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public void Reset()
		{
			if (enumerator != null)
			{
				enumerator.Reset();
			}
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <value></value>
		/// <returns>The current element in the collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
		public object Current
		{
			get { return CreateItemRepresentation(enumerator.Current); }
		}

		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return this;
		}

		#endregion
	}

	/// <summary>
	/// Used for empty/null datasources
	/// </summary>
	public class NoIterationState : OperationState
	{
		/// <summary>
		/// Single instance for the iterator.
		/// </summary>
		public static readonly NoIterationState Instance = new NoIterationState();

		private NoIterationState() : base(null, null, false, null, null, null, null)
		{
		}

		/// <summary>
		/// Gets the target suffix.
		/// </summary>
		/// <value>The target suffix.</value>
		public override string TargetSuffix
		{
			get { return null; }
		}

		/// <summary>
		/// Creates the item representation.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <returns></returns>
		protected override SetItem CreateItemRepresentation(object current)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Simple iterator
	/// </summary>
	public class ListDataSourceState : OperationState
	{
		private readonly string customSuffix;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListDataSourceState"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="valueProperty">The value property.</param>
		/// <param name="textProperty">The text property.</param>
		/// <param name="textFormat">The text format.</param>
		/// <param name="valueFormat">The value format.</param>
		/// <param name="customSuffix">The custom suffix.</param>
		public ListDataSourceState(Type type, IEnumerable dataSource, String valueProperty,
			String textProperty, String textFormat, String valueFormat, String customSuffix)
			: base(type, dataSource, false, valueProperty, textProperty, textFormat, valueFormat)
		{
			this.customSuffix = customSuffix;
		}

		/// <summary>
		/// Gets the target suffix.
		/// </summary>
		/// <value>The target suffix.</value>
		public override string TargetSuffix
		{
			get { return customSuffix; }
		}

		/// <summary>
		/// Creates the item representation.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <returns></returns>
		protected override SetItem CreateItemRepresentation(object current)
		{
			object value = current;
			object text = current;

			if (valuePropInfo != null)
			{
				value = valuePropInfo.GetValue(current);
			}

			if (textPropInfo != null)
			{
				text = textPropInfo.GetValue(current);
			}

			FormatText(ref text, textFormat);
			FormatText(ref value, valueFormat);

			return new SetItem(current, value != null ? value.ToString() : String.Empty, text != null ? text.ToString() : String.Empty, false);
		}
	}

	/// <summary>
	/// Iterator for sets type same type
	/// </summary>
	public class SameTypeOperationState : OperationState
	{
		private readonly object initialSelection;
		private readonly bool isInitialSelectionASet;

		/// <summary>
		/// Initializes a new instance of the <see cref="SameTypeOperationState"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="initialSelection">The initial selection.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="emptyValueCase">if set to <c>true</c> [empty value case].</param>
		/// <param name="valueProperty">The value property.</param>
		/// <param name="textProperty">The text property.</param>
		/// <param name="textFormat">The text format.</param>
		/// <param name="valueFormat">The value format.</param>
		/// <param name="isInitialSelectionASet">if set to <c>true</c> [is initial selection A set].</param>
		public SameTypeOperationState(Type type, object initialSelection, IEnumerable dataSource, 
			bool emptyValueCase,String valueProperty, String textProperty, String textFormat, String valueFormat, bool isInitialSelectionASet)
			: base(type, dataSource, emptyValueCase, valueProperty, textProperty, textFormat, valueFormat)
		{
			this.initialSelection = initialSelection;
			this.isInitialSelectionASet = isInitialSelectionASet;
		}

		/// <summary>
		/// Gets the target suffix.
		/// </summary>
		/// <value>The target suffix.</value>
		public override string TargetSuffix
		{
			get { return valuePropInfo == null ? "" : valuePropInfo.Name; }
		}

		/// <summary>
		/// Creates the item representation.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <returns></returns>
		protected override SetItem CreateItemRepresentation(object current)
		{
			object value = current;
			object text = current;

			if (valuePropInfo != null)
			{
				value = valuePropInfo.GetValue(current);
			}

			if (textPropInfo != null)
			{
				text = textPropInfo.GetValue(current);
			}

			FormatText(ref text, textFormat);
			FormatText(ref value, valueFormat);

			bool isSelected = FormHelper.IsPresent(value, initialSelection, valuePropInfo, isInitialSelectionASet);

			return new SetItem(current, value != null ? value.ToString() : String.Empty, text != null ? text.ToString() : String.Empty, isSelected);
		}
	}

	/// <summary>
	/// Iterator for different types on the set
	/// </summary>
	public class DifferentTypeOperationState : OperationState
	{
		private readonly object initialSelection;
		private readonly bool isInitialSelectionASet;
		private readonly FormHelper.ValueGetter sourcePropInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="DifferentTypeOperationState"/> class.
		/// </summary>
		/// <param name="initialSelectionType">Initial type of the selection.</param>
		/// <param name="dataSourceType">Type of the data source.</param>
		/// <param name="initialSelection">The initial selection.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="sourceProperty">The source property.</param>
		/// <param name="valueProperty">The value property.</param>
		/// <param name="textProperty">The text property.</param>
		/// <param name="textFormat">The text format.</param>
		/// <param name="valueFormat">The value format.</param>
		/// <param name="isInitialSelectionASet">if set to <c>true</c> [is initial selection A set].</param>
		public DifferentTypeOperationState(Type initialSelectionType, Type dataSourceType, 
			object initialSelection, IEnumerable dataSource, 
			String sourceProperty, String valueProperty,
			String textProperty, String textFormat, String valueFormat, bool isInitialSelectionASet)
			: base(dataSourceType, dataSource, false, valueProperty, textProperty, textFormat, valueFormat)
		{
			this.initialSelection = initialSelection;
			this.isInitialSelectionASet = isInitialSelectionASet;

			if (sourceProperty != null)
			{
				sourcePropInfo = FormHelper.ValueGetterAbstractFactory.Create(initialSelectionType, sourceProperty); // FormHelper.GetMethod(initialSelectionType, sourceProperty);
			}
			else if (valueProperty != null)
			{
				sourcePropInfo = FormHelper.ValueGetterAbstractFactory.Create(initialSelectionType, valueProperty); // FormHelper.GetMethod(initialSelectionType, valueProperty);
			}
		}

		/// <summary>
		/// Gets the target suffix.
		/// </summary>
		/// <value>The target suffix.</value>
		public override string TargetSuffix
		{
			get { return sourcePropInfo == null ? "" : sourcePropInfo.Name; }
		}

		/// <summary>
		/// Creates the item representation.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <returns></returns>
		protected override SetItem CreateItemRepresentation(object current)
		{
			object value = current;
			object text = current;

			if (valuePropInfo != null)
			{
				value = valuePropInfo.GetValue(current);
			}

			if (textPropInfo != null)
			{
				text = textPropInfo.GetValue(current);
			}

			FormatText(ref text, textFormat);
			FormatText(ref value, valueFormat);

			bool isSelected = FormHelper.IsPresent(value, initialSelection, sourcePropInfo, isInitialSelectionASet);

			return new SetItem(current, value != null ? value.ToString() : String.Empty, text != null ? text.ToString() : String.Empty, isSelected);
		}
	}
}
