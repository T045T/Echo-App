using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Echo.Helpers
{
    public class ListBoxScrollToSelectionBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var myList = AssociatedObject as ListBox;
            myList.SelectionChanged += new SelectionChangedEventHandler(myList_SelectionChanged);
        }

        void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var myList = sender as ListBox;
            if (e.AddedItems.Count > 0)
            {
                myList.ScrollIntoView(e.AddedItems[0]);
            }
        }
    }
}
