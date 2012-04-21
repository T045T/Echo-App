using System.Windows;
using System.Windows.Controls;

namespace Echo.Helpers
{
    /// <summary>
    /// Mediator that forwards Offset property changes on to a ScrollViewer
    /// instance to enable the animation of Horizontal/VerticalOffset.
    /// </summary>
    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        /// <summary>
        /// ScrollViewer instance to forward Offset changes on to.
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }
        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register(
                "ScrollViewer",
                typeof(ScrollViewer),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(OnScrollViewerChanged));
        private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = (ScrollViewer)(e.NewValue);
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(mediator.VerticalOffset);
            }
        }

        /// <summary>
        /// VerticalOffset property to forward to the ScrollViewer.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(OnVerticalOffsetChanged));
        public static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            if (null != mediator.ScrollViewer)
            {
                mediator.ScrollViewer.ScrollToVerticalOffset((double)(e.NewValue));
            }
        }

        /// <summary>
        /// Multiplier for ScrollableHeight property to forward to the ScrollViewer.
        /// </summary>
        /// <remarks>
        /// 0.0 means "scrolled to top"; 1.0 means "scrolled to bottom".
        /// </remarks>
        public double ScrollableHeightMultiplier
        {
            get { return (double)GetValue(ScrollableHeightMultiplierProperty); }
            set { SetValue(ScrollableHeightMultiplierProperty, value); }
        }
        public static readonly DependencyProperty ScrollableHeightMultiplierProperty =
            DependencyProperty.Register(
                "ScrollableHeightMultiplier",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(OnScrollableHeightMultiplierChanged));
        public static void OnScrollableHeightMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = mediator.ScrollViewer;
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset((double)(e.NewValue) * scrollViewer.ScrollableHeight);
            }
        }
    }


    public static class ScrollViewerAnimationHelper
    {
        #region MyHorizontalOffset
        public static readonly DependencyProperty MyHorizontalOffsetProperty =
         DependencyProperty.RegisterAttached("MyHorizontalOffset",
         typeof(double),
         typeof(ScrollViewerAnimationHelper),
         new PropertyMetadata(MyHorizontalOffsetPropertyChanged));

        // Called when Property is retrieved
        public static double GetMyHorizontalOffset(DependencyObject obj)
        {
            return (double) obj.GetValue(MyHorizontalOffsetProperty);
        }

        // Called when Property is set
        public static void SetMyHorizontalOffset(
          DependencyObject obj,
          double value)
        {
            obj.SetValue(MyHorizontalOffsetProperty, value);
        }

        // Called when property is changed
        private static void MyHorizontalOffsetPropertyChanged(
         object sender,
         DependencyPropertyChangedEventArgs args)
        {
            var attachedObject = sender as ScrollViewer;
            if (attachedObject == null) return;
            attachedObject.ScrollToHorizontalOffset((double)args.NewValue);
        }
        #endregion

        #region MyVerticalOffset
        public static readonly DependencyProperty MyVerticalOffsetProperty =
         DependencyProperty.RegisterAttached("MyVerticalOffset",
         typeof(double),
         typeof(ScrollViewerAnimationHelper),
         new PropertyMetadata(MyVerticalOffsetPropertyChanged));

        // Called when Property is retrieved
        public static double GetMyVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(MyVerticalOffsetProperty);
        }

        // Called when Property is set
        public static void SetMyVerticalOffset(
          DependencyObject obj,
          double value)
        {
            obj.SetValue(MyVerticalOffsetProperty, value);
        }

        // Called when property is changed
        private static void MyVerticalOffsetPropertyChanged(
         object sender,
         DependencyPropertyChangedEventArgs args)
        {
            var attachedObject = sender as ScrollViewer;
            if (attachedObject == null) return;
            attachedObject.ScrollToVerticalOffset((double)args.NewValue);
        }
        #endregion
    }
}
