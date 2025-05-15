using System.Windows.Media.Animation;
using System.Windows;

namespace QuanLyNhaSach.Views.CustomAnimation
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get { return (GridLength)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public GridLength To
        {
            get { return (GridLength)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public override Type TargetPropertyType => typeof(GridLength);

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            // Use From value directly when provided
            double fromVal = From.Value;

            // If From is not specified (or auto), use the default value
            if (fromVal == 0 && From.GridUnitType == GridUnitType.Auto)
                fromVal = ((GridLength)defaultOriginValue).Value;

            double toVal = To.Value;
            if (toVal == 0 && To.GridUnitType == GridUnitType.Auto)
                toVal = ((GridLength)defaultDestinationValue).Value;

            if (animationClock.CurrentProgress == null)
                return new GridLength(fromVal);

            double progress = animationClock.CurrentProgress.Value;
            return new GridLength((1 - progress) * fromVal + progress * toVal, GridUnitType.Pixel);
        }
    }
}
