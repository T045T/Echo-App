// Keyboard helper from pauliom.wordpress.com, @pauliom
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace WPKeyboardHelper
{
    /// <summary>
    /// Helper to provide missing tab-to-next-control functionality
    /// </summary>
    public class KeyboardHelper
    {
        private List<Control> tabbedControls;
        private Control lastTabbedControl = null;
        private UIElement layoutRoot;
        private Control page;

        /// <summary>
        /// Constructor to support a phone page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="layoutRoot"></param>
        public KeyboardHelper(UIElement layoutRoot)
        {            
            this.layoutRoot = layoutRoot;                                   
        }


        private Control Page
        {
            get
            {
                if (this.page == null)
                {
                    //this.page = Application.Current.RootVisual as Control;
                    this.page = GetPage(this.layoutRoot);
                    RefeshTabbedControls(layoutRoot);
                }
                return this.page;
            }
        }

         public PhoneApplicationPage GetPage(DependencyObject obj)
         {
             DependencyObject parent = VisualTreeHelper.GetParent(obj);
             while(parent!=null)
             {
                 PhoneApplicationPage page = parent as PhoneApplicationPage;
                 if (page !=null)
                 {
                     return page;           
                 }
                 parent = VisualTreeHelper.GetParent(parent);      
             }
             return null;
         }

        public void HideKeyboard()
        {
            this.Page.Focus();
        }

        /// <summary>
        /// Refresh the tabbed controls collection, helpful if you dynamically alter the controls
        /// </summary>
        /// <param name="layoutRoot"></param>
        public void RefeshTabbedControls(UIElement layoutRoot)
        {
            this.tabbedControls = GetChildsRecursive(layoutRoot).OfType<Control>().Where(c => c.IsTabStop && c.TabIndex != int.MaxValue).OrderBy(c => c.TabIndex).ToList();
            if (this.tabbedControls != null && this.tabbedControls.Count > 0)
            {
                this.lastTabbedControl = this.tabbedControls[this.tabbedControls.Count - 1];
            }
        }

        // code from 'tucod'
        IEnumerable<DependencyObject> GetChildsRecursive(DependencyObject root)
        {
            List<DependencyObject> elts = new List<DependencyObject>();
            elts.Add(root);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                elts.AddRange(GetChildsRecursive(VisualTreeHelper.GetChild(root, i)));

            return elts;
        }

        /// <summary>
        /// Process a return key from the client controls key-up event
        /// </summary>
        /// <param name="sender"></param>
        internal void HandleReturnKey()
        {
            Control controlWithFocus = FocusManager.GetFocusedElement() as Control;
            if (controlWithFocus == null)
            {
                return;
            }
            Control page = this.Page;
            Control thisControlInTabbedList = tabbedControls.FirstOrDefault(c => c == controlWithFocus);
            if (thisControlInTabbedList != null)
            {
                // what's the next control?                                                                        
                SetFocusOnNextControl(thisControlInTabbedList);
            }

        }

        private void SetFocusOnNextControl(Control thisControlInTabbedList)
        {
            if (lastTabbedControl == thisControlInTabbedList)
            {
                // we've come to the end so remove the keyboard
                bool hasSet = this.page.Focus();
                if (!hasSet)
                {
                    System.Diagnostics.Debug.WriteLine("Page could not accept focus");
                }
            }
            else
            {
                Control nextControl = tabbedControls.FirstOrDefault(c => c.TabIndex > thisControlInTabbedList.TabIndex);
                bool wasFocusSet = false;
                if (nextControl != null)
                {
                    wasFocusSet = nextControl.Focus();
                }

                if (!wasFocusSet)
                {
                    SetFocusOnNextControl(nextControl);
                }
                
            }
        }
    }
}
