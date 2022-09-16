using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Dlubal.WS.Common.Dialogs
{
    public abstract class CustomTreeItem<T> : DependencyObject
    {
        // Dependency Properties
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnCheckedChanged)));
        public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register(nameof(IsThreeState), typeof(bool), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ParentItemProperty = DependencyProperty.Register(nameof(Parent), typeof(CustomTreeItem<T>), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<CustomTreeItem<T>>), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(CustomTreeItem<T>), new FrameworkPropertyMetadata(false));

        public T Content { get; set; } = default(T);

        private static bool IsCheckedChangedEventEnabled { get; set; } = true;

        private static void OnCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var currentItem = obj as CustomTreeItem<T>;

            if (currentItem == null)
            {
                return;
            }

            if (IsCheckedChangedEventEnabled && (currentItem.Items != null))
            {
                foreach (var childItem in currentItem.Items)
                {
                    childItem.IsChecked = (bool?)args.NewValue;
                }
            }

            IsCheckedChangedEventEnabled = false;

            if ((currentItem.Parent != null) && (currentItem.Parent.Items?.Count >= 1))
            {
                bool? state = currentItem.Parent.Items[0].IsChecked;

                for (int index = 1; index < currentItem.Parent.Items.Count; ++index)
                {
                    if (state != currentItem.Parent.Items[index].IsChecked)
                    {
                        state = null;
                        break;
                    }
                }

                currentItem.Parent.IsThreeState = !state.HasValue;
                currentItem.Parent.IsChecked = state;
            }

            IsCheckedChangedEventEnabled = true;
        }

        public bool? IsChecked
        {
            get
            {
                return (bool?)GetValue(IsCheckedProperty);
            }

            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

        public bool IsThreeState
        {
            get
            {
                return (bool)GetValue(IsThreeStateProperty);
            }

            set
            {
                SetValue(IsThreeStateProperty, value);

                if (!value)
                {
                    IsChecked = false;
                }
            }
        }

        public string Name
        {
            get
            {
                return (string)GetValue(NameProperty);
            }

            set
            {
                SetValue(NameProperty, value);
            }
        }

        public CustomTreeItem<T> Parent
        {
            get
            {
                return (CustomTreeItem<T>)GetValue(ParentItemProperty);
            }

            set
            {
                SetValue(ParentItemProperty, value);
            }
        }

        public ObservableCollection<CustomTreeItem<T>> Items
        {
            get
            {
                return (ObservableCollection<CustomTreeItem<T>>)GetValue(ItemsProperty);
            }

            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        protected CustomTreeItem(string name)
        {
            Name = name;
        }

        internal virtual void GetContentOfCheckedItems(List<T> itemContentList)
        {
            throw new NotImplementedException("You should implement method GetContentOfCheckedItems()!");
        }

        public virtual void GetCheckedItems(List<CustomTreeItem<T>> itemList)
        {
            throw new NotImplementedException("You should implement method GetCheckedItems()!");
        }
    }
}