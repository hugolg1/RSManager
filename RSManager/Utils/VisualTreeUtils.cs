using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace RSManager.Utils
{
    internal static class VisualTreeUtils
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if(child == null)
            {
                return null;
            }

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
            {
                return null;
            }

            if (parent is T)
            {
                return parent as T;
            }

            return FindParent<T>(parent);
        }

        public static T FindChildOfType<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var result = (child as T) ?? FindChildOfType<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

    }
}
