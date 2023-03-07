using System.Windows;
using System.Windows.Controls;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
        }
    }

    public class CascadingPanel : Panel
    {
        public double ItemOffset
        {
            get { return (double)GetValue(ItemOffsetProperty); }
            set { SetValue(ItemOffsetProperty, value); }
        }

        public static readonly DependencyProperty ItemOffsetProperty =
          DependencyProperty.Register("ItemOffset", typeof(double), typeof(CascadingPanel));

        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredWidth = 0.0;
            var desiredHeight = 0.0;
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                desiredWidth += child.DesiredSize.Width;
                desiredHeight += child.DesiredSize.Height;
            }
            return new Size(desiredWidth, desiredHeight);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 1; i <= Children.Count; i++)
            {
                if(i == 1)
                {
                    var firstChild = Children[Children.Count - i];
                    firstChild.Arrange(new Rect(0, ItemOffset / 2, firstChild.DesiredSize.Width, firstChild.DesiredSize.Height));
                    continue;
                }

                int horizontalShift = 0;
                if(i % 2 == 0)
                {
                    horizontalShift += (int)ItemOffset;
                }

                var child = Children[Children.Count - i];
                child.Arrange(new Rect(horizontalShift, ItemOffset * (i - 1) + ItemOffset / 2, child.DesiredSize.Width, child.DesiredSize.Height));
            }
            return finalSize;
        }
    }
}
