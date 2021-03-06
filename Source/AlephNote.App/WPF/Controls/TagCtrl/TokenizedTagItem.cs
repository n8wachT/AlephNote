﻿using AlephNote.WPF.Util;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AlephNote.WPF.Controls
{
	[TemplatePart(Name = "PART_InputBox", Type = typeof(AutoCompleteBox))]
	[TemplatePart(Name = "PART_DeleteTagButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_TagButton", Type = typeof(Button))]
	public class TokenizedTagItem : Control
	{
		public static readonly DependencyProperty TextProperty = 
			DependencyProperty.Register(
				"Text", 
				typeof(string), 
				typeof(TokenizedTagItem), 
				new PropertyMetadata(null));
		
		public string Text 
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}
		
		private static readonly DependencyPropertyKey IsEditingPropertyKey = 
			DependencyProperty.RegisterReadOnly(
				"IsEditing", 
				typeof(bool), 
				typeof(TokenizedTagItem), 
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;
		
		public bool IsEditing
		{ 
			get => (bool)GetValue(IsEditingProperty);
			internal set => SetValue(IsEditingPropertyKey, value);
		}

		private readonly TokenizedTagControl _parent;

		static TokenizedTagItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenizedTagItem), new FrameworkPropertyMetadata(typeof(TokenizedTagItem)));
		}

		public TokenizedTagItem(string text, TokenizedTagControl parent)
		{
			_parent = parent;
			this.Text = text;
		}

		public override void OnApplyTemplate()
		{
			AutoCompleteBox inputBox = this.GetTemplateChild("PART_InputBox") as AutoCompleteBox;
			if (inputBox != null)
			{
				inputBox.LostKeyboardFocus += InputBox_LostFocus;
				inputBox.Loaded += InputBox_Loaded;
			}

			Button btn = this.GetTemplateChild("PART_TagButton") as Button;
			if (btn != null)
			{
				btn.Loaded += (s, e) =>
				{
					Button b = s as Button;
					if (b.Template.FindName("PART_DeleteTagButton", b) is Button btnDelete)
					{
						btnDelete.Click -= BtnDelete_Click;
						btnDelete.Click += BtnDelete_Click;
					}
				};

				btn.Click += (s, e) =>
				{
					_parent.RaiseTagClick(this);
					if (_parent.IsSelectable && !_parent.IsReadonly) _parent.SelectedItem = this;
				};

				btn.MouseDoubleClick += (s, e) =>
				{
					_parent.RaiseTagDoubleClick(this);
					if (_parent.IsSelectable && !_parent.IsReadonly) _parent.SelectedItem = this;
				};
			}

			base.OnApplyTemplate();
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			var item = WPFHelper.GetParentOfType<TokenizedTagItem>(sender as FrameworkElement);
			if (item != null) _parent.RemoveTag(item);
			e.Handled = true;
		}

		private void InputBox_Loaded(object sender, RoutedEventArgs e)
		{
			AutoCompleteBox acb = sender as AutoCompleteBox;
			
			if (acb != null)
			{
				var tb = acb.Template.FindName("Text", acb) as TextBox;

				if (tb != null)
					tb.Focus();

				acb.PreviewKeyDown += (s, e1) =>
				{
					if (_parent.IsReadonly)
					{
						_parent.Focus();
						return;
					}

					switch (e1.Key)
					{
						case (Key.Enter):
						{
							if (!string.IsNullOrWhiteSpace(this.Text))
							{
								_parent.OnApplyTemplate(this);
								_parent.SelectedItem = _parent.InitializeNewTag();
							}
							else
							{
								_parent.Focus();
							}
						}
						break;

						case (Key.Escape):
						{
							_parent.Focus();
						}
						break;

						case (Key.Back):
						{
							if (string.IsNullOrWhiteSpace(this.Text))
							{
								InputBox_LostFocus(this, new RoutedEventArgs());
								var previousTagIndex = ((IList)_parent.ItemsSource).Count - 1;
								if (previousTagIndex < 0) break;
								
								var previousTag = (((IList)_parent.ItemsSource)[previousTagIndex] as TokenizedTagItem);
								previousTag.Focus();
								previousTag.IsEditing = true;
							}
						}
						break;
					}
				};
			}
		}
		
		private void InputBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.Text))
			{
				if (!((AutoCompleteBox) sender).IsDropDownOpen) this.IsEditing = false;
			}
			else
			{
				_parent.RemoveTag(this, true);
			}

			_parent.IsEditing = false;
		}
	}
}
