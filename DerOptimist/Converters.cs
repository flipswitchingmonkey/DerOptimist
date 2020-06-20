using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DerOptimist
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan)value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds((double)value);
        }
    }

    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan)value).ToString(@"d\.hh\:mm\:ss\.ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.Zero;
        }
    }

    public class DurationToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Duration d = (Duration)value;
            if (d == Duration.Automatic || d == Duration.Forever || !d.HasTimeSpan) return 0;
            return d.TimeSpan.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Duration(TimeSpan.FromSeconds((double)value));
        }
    }

    public class DurationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Duration d = (Duration)value;
            if (d == Duration.Automatic || d == Duration.Forever || !d.HasTimeSpan) return "Auto";
            return d.TimeSpan.ToString(@"d\.hh\:mm\:ss\.ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Duration(TimeSpan.Zero);
        }
    }

    public class TimeSpanToFramesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return -1;
            //return (int)Math.Floor(((TimeSpan)values[0]).TotalSeconds * ((TimeSpan)values[1]).TotalSeconds);
            var frames = (int)Math.Round(((TimeSpan)values[0]).TotalSeconds * ((double)values[1]));
            if (values.Length == 3)
            {
                //Duration d = (Duration)values[2];
                if (values[2] == null) return frames;
                TimeSpan ts = (TimeSpan)values[2];
                //if (d == Duration.Automatic || d == Duration.Forever || !d.HasTimeSpan) return frames;
                var duration = (int)Math.Round(ts.TotalSeconds * (double)values[1]);
                return $"{frames}/{duration}";
            }
            else
                return frames;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { TimeSpan.Zero, 0};
        }
    }

    //public class DurationToFramesConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (values.Length < 2) return -1;
    //        Duration d = (Duration)values[0];
    //        //TimeSpan ts = (TimeSpan)values[1];
    //        if (d == Duration.Automatic || d == Duration.Forever || !d.HasTimeSpan) return 0;
    //        //return (int)Math.Floor(d.TimeSpan.TotalSeconds * ts.TotalSeconds);
    //        return (int)Math.Round(d.TimeSpan.TotalSeconds * (double)values[1]);
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        return new object[] { new Duration(TimeSpan.Zero), 0 };
    //    }
    //}

    public class TimeSpanWithOffsetToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null) return "";
            return (((TimeSpan)values[0]).Add((TimeSpan)values[1])).ToString(@"d\.hh\:mm\:ss\.ff");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { new Duration(TimeSpan.Zero), 0 };
        }
    }

    public class QueueStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            RenderQueueItemStatus status = (RenderQueueItemStatus)value;
            switch(value)
            {
                case RenderQueueItemStatus.None:
                    return "None";
                case RenderQueueItemStatus.Queued:
                    return "Queued";
                case RenderQueueItemStatus.Encoding:
                    return "Rendering";
                case RenderQueueItemStatus.Finished:
                    return "Finished";
                case RenderQueueItemStatus.Waiting:
                    return "Waiting";
                case RenderQueueItemStatus.Stopped:
                    return "Stopped";
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return RenderQueueItemStatus.None;
        }
    }

    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (double)value == 0.0) return "--";
            if ((double)value > 99.9) return "100.0%";
            return $"{(double)value:00.}%";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null) return -1.0;
            return double.Parse((string)value);
        }
    }
}
