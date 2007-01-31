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
	/// 
	/// </summary>
	public class SetOperation
	{
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

	public class SetItem
	{
		private readonly string value;
		private readonly object item;
		private readonly string text;
		private readonly bool isSelected;

		public SetItem(object item, String value, String text, bool isSelected)
		{
			this.item = item;
			this.value = value;
			this.text = text;
			this.isSelected = isSelected;
		}

		public object Item
		{
			get { return item; }
		}

		public string Value
		{
			get { return value; }
		}

		public string Text
		{
			get { return text; }
		}

		public bool IsSelected
		{
			get { return isSelected; }
		}
	}

	public abstract class OperationState : IEnumerable, IEnumerator
	{
		protected readonly Type type;
		protected readonly FormHelper.ValueGetter valuePropInfo;
		protected readonly FormHelper.ValueGetter textPropInfo;
		protected readonly String textFormat, valueFormat;
		protected IEnumerator enumerator;

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

		public abstract String TargetSuffix { get; }

		/// <summary>
		/// Formats the text.
		/// </summary>
		/// <param name="value">The value to be formatted.</param>
		/// <param name="format">The format to apply.</param>
		protected void FormatText(ref object value, string format)
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

		protected abstract SetItem CreateItemRepresentation(object current);

		#region IEnumerator implementation

		public bool MoveNext()
		{
			if (enumerator == null)
			{
				return false;
			}
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			if (enumerator != null)
			{
				enumerator.Reset();
			}
		}

		public object Current
		{
			get { return CreateItemRepresentation(enumerator.Current); }
		}

		#endregion

		#region IEnumerable implementation

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
		public static readonly NoIterationState Instance = new NoIterationState();

		private NoIterationState() : base(null, null, false, null, null, null, null)
		{
		}

		public override string TargetSuffix
		{
			get { return null; }
		}

		protected override SetItem CreateItemRepresentation(object current)
		{
			throw new NotImplementedException();
		}
	}

	public class ListDataSourceState : OperationState
	{
		private readonly string customSuffix;

		public ListDataSourceState(Type type, IEnumerable dataSource, String valueProperty,
			String textProperty, String textFormat, String valueFormat, String customSuffix)
			: base(type, dataSource, false, valueProperty, textProperty, textFormat, valueFormat)
		{
			this.customSuffix = customSuffix;
		}

		public override string TargetSuffix
		{
			get { return customSuffix; }
		}

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

	public class SameTypeOperationState : OperationState
	{
		private readonly object initialSelection;
		private readonly bool isInitialSelectionASet;

		public SameTypeOperationState(Type type, object initialSelection, IEnumerable dataSource, 
			bool emptyValueCase,String valueProperty, String textProperty, String textFormat, String valueFormat, bool isInitialSelectionASet)
			: base(type, dataSource, emptyValueCase, valueProperty, textProperty, textFormat, valueFormat)
		{
			this.initialSelection = initialSelection;
			this.isInitialSelectionASet = isInitialSelectionASet;
		}

		public override string TargetSuffix
		{
			get { return valuePropInfo == null ? "" : valuePropInfo.Name; }
		}

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

	public class DifferentTypeOperationState : OperationState
	{
		private readonly object initialSelection;
		private readonly bool isInitialSelectionASet;
		private readonly FormHelper.ValueGetter sourcePropInfo;

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

		public override string TargetSuffix
		{
			get { return sourcePropInfo == null ? "" : sourcePropInfo.Name; }
		}

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
