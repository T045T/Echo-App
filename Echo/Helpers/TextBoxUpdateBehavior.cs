using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Echo.Helpers
{
    public class TextBoxUpdateBehavior : Behavior<TextBox>
    {
        public TextBoxUpdateBehavior() { }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs args)
        {
            var binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }
    }
    public class PasswordBoxUpdateBehavior : Behavior<PasswordBox>
    {
        public PasswordBoxUpdateBehavior() { }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs args)
        {
            var binding = (sender as PasswordBox).GetBindingExpression(PasswordBox.PasswordProperty);
            binding.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObjectOnTextChanged;
        }
    }
}

