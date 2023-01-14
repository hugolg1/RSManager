using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSManager.Base.Behaviours
{
    internal static class TextBoxBehaviour
    {
        #region Title
        public static string GetTitle(DependencyObject dp)
        {
            return dp.GetValue(TitleProperty) as string;
        }

        public static void SetTitle(DependencyObject dp, string value)
        {
            dp.SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(TextBoxBehaviour), new PropertyMetadata(null));
        #endregion
    }
}
