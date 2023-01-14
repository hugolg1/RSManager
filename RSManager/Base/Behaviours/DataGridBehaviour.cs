using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSManager.Base.Behaviours
{
    internal static class DataGridBehaviour
    {

        public static int? GetSortId(DependencyObject dp)
        {
            return dp.GetValue(SortIdProperty) as int?;
        }

        public static void SetSortId(DependencyObject dp, int? value)
        {
            dp.SetValue(SortIdProperty, value);
        }

        // Using a DependencyProperty as the backing store for SortIdProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortIdProperty =
            DependencyProperty.RegisterAttached("SortIdProperty", typeof(int?), typeof(DataGridBehaviour), new PropertyMetadata(null));


    }
}
