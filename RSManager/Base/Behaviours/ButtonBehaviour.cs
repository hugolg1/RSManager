using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSManager.Base.Behaviours
{
    internal static class ButtonBehaviour
    {
        #region Icon Path
        public static string GetIconPath(DependencyObject dp)
        {
            return dp.GetValue(IconPathProperty) as string;
        }

        public static void SetIconPath(DependencyObject dp, string value)
        {
            dp.SetValue(IconPathProperty, value);
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.RegisterAttached("IconPath", typeof(string), typeof(ButtonBehaviour), new PropertyMetadata(null));

        #endregion

    }
}
