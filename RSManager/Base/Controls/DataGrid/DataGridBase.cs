using RSManager.Base.Behaviours;
using RSManager.Converters;
using RSManager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RSManager.Base.Controls
{
    public class DataGridBase : DataGrid
    {

        #region Dependency Properties

        public IList SelectedDataItems
        {
            get { return (IList)GetValue(SelectedDataItemsProperty); }
            set { SetValue(SelectedDataItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDataItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDataItemsProperty =
            DependencyProperty.Register("SelectedDataItems", typeof(IList), typeof(DataGridBase), new PropertyMetadata(null));

        #endregion

        private ContextMenu headerMenu;

        public DataGridBase()
        {

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitializedHeaderMenu();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (ItemsSource == null)
            {
                return;
            }

            Type collectionType = base.ItemsSource.GetType();
            Type type = null;

            if (collectionType == typeof(CollectionView))
            {
                type = (base.ItemsSource as CollectionView).SourceCollection.GetType().GetGenericArguments()[0];
            }
            else if (collectionType == typeof(ListCollectionView))
            {
                type = (base.ItemsSource as ListCollectionView).SourceCollection.GetType().GetGenericArguments()[0];
            }
            else
            {
                type = collectionType.GetGenericArguments()[0];
            }

            var listType = typeof(List<>).MakeGenericType(type);
            var items = (IList)Activator.CreateInstance(listType);
            foreach (var item in base.SelectedItems)
            {
                items.Add(item);
            }
            SelectedDataItems = items;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            var row = ItemsControl.ContainerFromElement(this, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null && SelectedDataItems != null && SelectedDataItems.Count > 0)
            {
                base.SelectedItems.Clear();
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            var row = ItemsControl.ContainerFromElement(this, e.OriginalSource as DependencyObject) as DataGridRow;

            if (row != null)
            {
                //row.Item
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            var dp = e.OriginalSource as DependencyObject;
            DataGridColumnHeader header = VisualTreeUtils.FindParent<DataGridColumnHeader>(dp);

            //if(header == null && dp is FrameworkElement element && element.Name == "DG_ScrollViewer")
            //{
            //    header = VisualTreeUtils.FindChildOfType<DataGridColumnHeader>(element);
            //}

            if (header == null)
            {
                return;
            }

            if (header.ContextMenu != null)
            {
                return;
            }

            header.ContextMenu = headerMenu;
        }

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            base.OnSorting(eventArgs);

            int? idSort = eventArgs.Column.GetValue(DataGridBehaviour.SortIdProperty) as int?;

            if (idSort == 2)
            {
                eventArgs.Column.SortDirection = null;
                eventArgs.Column.SetValue(DataGridBehaviour.SortIdProperty, null);
            }
            else
            {
                eventArgs.Column.SetValue(DataGridBehaviour.SortIdProperty, (idSort ?? 0) + 1);
            }
        }

        private void InitializedHeaderMenu()
        {
            headerMenu = new ContextMenu();
            foreach (var column in Columns)
            {
                MenuItem menuItem = new MenuItem()
                {
                    DataContext = column,
                    IsCheckable = true,
                };

                menuItem.SetBinding(MenuItem.HeaderProperty, new Binding("Header")
                {
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });
                menuItem.SetBinding(MenuItem.IsCheckedProperty, new Binding("Visibility")
                {
                    Converter = new VisibilityToBooleanValueConverter(),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });

                menuItem.Click += MenuItemHeaderMenu_Click;
                headerMenu.Items.Add(menuItem);
            }
        }

        private void MenuItemHeaderMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            var column = menuItem?.DataContext as DataGridColumn;

            if (column == null)
            {
                return;
            }

            //Dont allow uncheked all columns
            if (Columns.Count(x => x.Visibility == Visibility) == 1 && !menuItem.IsChecked)
            {
                menuItem.IsChecked = true;
                return;
            }

            column.Visibility = column.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
