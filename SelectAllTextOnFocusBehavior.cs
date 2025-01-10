using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace MortysDLP_dotNet_ { 
    public class SelectAllTextOnFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += OnGotFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectAll();
        }
    }
}