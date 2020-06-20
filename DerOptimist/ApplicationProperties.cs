using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DerOptimist
{
    public partial class MainWindow
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // Just so we can call it via lambda which is nicer
        private void RaisePropertyChanged<T>(Expression<Func<T>> propertyNameExpression)
        {
            RaisePropertyChanged(((MemberExpression)propertyNameExpression.Body).Member.Name);
        }

        public string[] SupportedPixFmts
        {
            get { return (string[])GetValue(SupportedPixFmtsProperty); }
            set { SetValue(SupportedPixFmtsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SupportedPixFmts.
        public static readonly DependencyProperty SupportedPixFmtsProperty =
            DependencyProperty.Register("SupportedPixFmts", typeof(string[]), typeof(MainWindow), new PropertyMetadata(default(string[])));

        ///
        /// SLIDER MEDIA A
        ///

        public double RangeSliderPlayA_RangeLowerValue
        {
            get { return (double)GetValue(RangeSliderPlayA_RangeLowerValueProperty); }
            set { SetValue(RangeSliderPlayA_RangeLowerValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeLowerValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeSliderPlayA_RangeLowerValueProperty =
            DependencyProperty.Register("RangeSliderPlayA_RangeLowerValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double RangeSliderPlayA_RangeUpperValue
        {
            get { return (double)GetValue(RangeSliderPlayA_RangeUpperValueProperty); }
            set { SetValue(RangeSliderPlayA_RangeUpperValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeSliderPlayA_RangeUpperValueProperty =
            DependencyProperty.Register("RangeSliderPlayA_RangeUpperValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public string LabelMediaAInPoint_Content
        {
            get { return (string)GetValue(LabelMediaAInPoint_ContentProperty); }
            set { SetValue(LabelMediaAInPoint_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMediaAInPoint_ContentProperty =
            DependencyProperty.Register("LabelMediaAInPoint_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

               
        public string LabelMediaBLeft_ContentValue
        {
            get { return (string)GetValue(LabelMediaBLeft_ContentProperty); }
            set { SetValue(LabelMediaBLeft_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMediaBLeft_ContentProperty =
            DependencyProperty.Register("LabelMediaBLeft_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public string LabelMediaBRight_ContentValue
        {
            get { return (string)GetValue(LabelMediaBRight_ContentProperty); }
            set { SetValue(LabelMediaBRight_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMediaBRight_ContentProperty =
            DependencyProperty.Register("LabelMediaBRight_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public string LabelMediaBInPoint_ContentValue
        {
            get { return (string)GetValue(LabelMediaBInPoint_ContentProperty); }
            set { SetValue(LabelMediaBInPoint_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMediaBInPoint_ContentProperty =
            DependencyProperty.Register("LabelMediaBInPoint_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));


        public string LabelMediaBOutPoint_ContentValue
        {
            get { return (string)GetValue(LabelMediaBOutPoint_ContentProperty); }
            set { SetValue(LabelMediaBOutPoint_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMediaBOutPoint_ContentProperty =
            DependencyProperty.Register("LabelMediaBOutPoint_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        ///
        /// SLIDER QUALITY
        ///

        public double SliderQuality_MinValue
        {
            get { return (double)GetValue(SliderQuality_MinValueProperty); }
            set { SetValue(SliderQuality_MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQuality_MinValueProperty =
            DependencyProperty.Register("SliderQuality_MinValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double SliderQuality_MaxValue
        {
            get { return (double)GetValue(SliderQuality_MaxValueProperty); }
            set { SetValue(SliderQuality_MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQuality_MaxValueProperty =
            DependencyProperty.Register("SliderQuality_MaxValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double SliderQuality_QualityValue
        {
            get { return (double)GetValue(SliderQuality_QualityValueProperty); }
            set { SetValue(SliderQuality_QualityValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQuality_QualityValueProperty =
            DependencyProperty.Register("SliderQuality_QualityValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double SliderQuality_StepValue
        {
            get { return (double)GetValue(SliderQuality_StepValueProperty); }
            set { SetValue(SliderQuality_StepValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SliderQualityAudio_StepValue.
        public static readonly DependencyProperty SliderQuality_StepValueProperty =
            DependencyProperty.Register("SliderQuality_StepValue", typeof(double), typeof(MainWindow), new PropertyMetadata(1.0));


        public string LabelQuality_Content
        {
            get { return (string)GetValue(LabelQuality_ContentProperty); }
            set { SetValue(LabelQuality_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelQuality_Content.
        public static readonly DependencyProperty LabelQuality_ContentProperty =
            DependencyProperty.Register("LabelQuality_Content", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        ///
        /// SLIDER QUALITY AUDIO
        ///

        public double SliderQualityAudio_MinValue
        {
            get { return (double)GetValue(SliderQualityAudio_MinValueProperty); }
            set { SetValue(SliderQualityAudio_MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQualityAudio_MinValueProperty =
            DependencyProperty.Register("SliderQualityAudio_MinValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double SliderQualityAudio_MaxValue
        {
            get { return (double)GetValue(SliderQualityAudio_MaxValueProperty); }
            set { SetValue(SliderQualityAudio_MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQualityAudio_MaxValueProperty =
            DependencyProperty.Register("SliderQualityAudio_MaxValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double SliderQualityAudio_QualityValue
        {
            get { return (double)GetValue(SliderQualityAudio_QualityValueProperty); }
            set { SetValue(SliderQualityAudio_QualityValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSliderPlayA_RangeUpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderQualityAudio_QualityValueProperty =
            DependencyProperty.Register("SliderQualityAudio_QualityValue", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));


        public double SliderQualityAudio_StepValue
        {
            get { return (double)GetValue(SliderQualityAudio_StepValueProperty); }
            set { SetValue(SliderQualityAudio_StepValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SliderQualityAudio_StepValue.
        public static readonly DependencyProperty SliderQualityAudio_StepValueProperty =
            DependencyProperty.Register("SliderQualityAudio_StepValue", typeof(double), typeof(MainWindow), new PropertyMetadata(1.0));


        public string LabelQualityAudio_Content
        {
            get { return (string)GetValue(LabelQualityAudio_ContentProperty); }
            set { SetValue(LabelQualityAudio_ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelQualityAudio_Content.
        public static readonly DependencyProperty LabelQualityAudio_ContentProperty =
            DependencyProperty.Register("LabelQualityAudio_Content", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        ///
        /// MAIN WINDOW
        /// 

        public bool ToggleResizeVideo_Value
        {
            get { return (bool)GetValue(ToggleResizeVideo_ValueProperty); }
            set { SetValue(ToggleResizeVideo_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleResizeVideo_Value.
        public static readonly DependencyProperty ToggleResizeVideo_ValueProperty =
            DependencyProperty.Register("ToggleResizeVideo_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        
        public bool ToggleUseRangeForFinal_Value
        {
            get { return (bool)GetValue(ToggleUseRangeForFinal_ValueProperty); }
            set { SetValue(ToggleUseRangeForFinal_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleResizeVideo_Value.
        public static readonly DependencyProperty ToggleUseRangeForFinal_ValueProperty =
            DependencyProperty.Register("ToggleUseRangeForFinal_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));


        public bool ToggleForceAspectRatio_Value
        {
            get { return (bool)GetValue(ToggleForceAspectRatio_ValueProperty); }
            set { SetValue(ToggleForceAspectRatio_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleForceAspectRatio_Value.
        public static readonly DependencyProperty ToggleForceAspectRatio_ValueProperty =
            DependencyProperty.Register("ToggleForceAspectRatio_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool ToggleCheckBurnInTimeCode_Value
        {
            get { return (bool)GetValue(ToggleCheckBurnInTimeCode_ValueProperty); }
            set { SetValue(ToggleCheckBurnInTimeCode_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleCheckBurnInTimeCode_Value.
        public static readonly DependencyProperty ToggleCheckBurnInTimeCode_ValueProperty =
            DependencyProperty.Register("ToggleCheckBurnInTimeCode_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

        public bool ToggleManualRangeMode_Value
        {
            get { return (bool)GetValue(ToggleManualRangeMode_ValueProperty); }
            set { SetValue(ToggleManualRangeMode_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleManualRangeMode_Value.
        public static readonly DependencyProperty ToggleManualRangeMode_ValueProperty =
            DependencyProperty.Register("ToggleManualRangeMode_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

        public bool ToggleLoopedPlayback_Value
        {
            get { return (bool)GetValue(ToggleLoopedPlayback_ValueProperty); }
            set { SetValue(ToggleLoopedPlayback_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleLoopedPlayback_Value.
        public static readonly DependencyProperty ToggleLoopedPlayback_ValueProperty =
            DependencyProperty.Register("ToggleLoopedPlayback_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public bool ToggleLinkedPlayers_Value
        {
            get { return (bool)GetValue(ToggleLinkedPlayers_ValueProperty); }
            set { SetValue(ToggleLinkedPlayers_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleLinkedPlayers_Value.
        public static readonly DependencyProperty ToggleLinkedPlayers_ValueProperty =
            DependencyProperty.Register("ToggleLinkedPlayers_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));


        //public bool ToggleFitVideoToWindowAuto_Value
        //{
        //    get { return (bool)GetValue(ToggleFitVideoToWindowAutoProperty); }
        //    set { SetValue(ToggleFitVideoToWindowAutoProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for fitVideoToWindowAuto.
        //public static readonly DependencyProperty ToggleFitVideoToWindowAutoProperty =
        //    DependencyProperty.Register("ToggleFitVideoToWindowAuto_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));


        public bool ToggleCheckApplyGamma_Value
        {
            get { return (bool)GetValue(ToogleCheckApplyGamma_ValueProperty); }
            set { SetValue(ToogleCheckApplyGamma_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToogleCheckApplyGamma_Value.
        public static readonly DependencyProperty ToogleCheckApplyGamma_ValueProperty =
            DependencyProperty.Register("ToogleCheckApplyGamma_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool ToggleCheckApplyLUT_Value
        {
            get { return (bool)GetValue(ToogleCheckApplyLUT_ValueProperty); }
            set { SetValue(ToogleCheckApplyLUT_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToogleCheckApplyLUT_Value.
        public static readonly DependencyProperty ToogleCheckApplyLUT_ValueProperty =
            DependencyProperty.Register("ToogleCheckApplyLUT_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool ToggleCheckSetTimeCode_Value
        {
            get { return (bool)GetValue(ToggleCheckSetTimeCode_ValueProperty); }
            set { SetValue(ToggleCheckSetTimeCode_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleCheckSetTimeCode_Value.
        public static readonly DependencyProperty ToggleCheckSetTimeCode_ValueProperty =
            DependencyProperty.Register("ToggleCheckSetTimeCode_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool ToggleAutoPreview_Value
        {
            get { return (bool)GetValue(ToggleAutoPreview_ValueProperty); }
            set { SetValue(ToggleAutoPreview_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleAutoPreview_Value.
        public static readonly DependencyProperty ToggleAutoPreview_ValueProperty =
            DependencyProperty.Register("ToggleAutoPreview_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));


        public bool ToggleReplaceAudio_Value
        {
            get { return (bool)GetValue(ToggleReplaceAudio_ValueProperty); }
            set { SetValue(ToggleReplaceAudio_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleReplaceAudio_Value.
        public static readonly DependencyProperty ToggleReplaceAudio_ValueProperty =
            DependencyProperty.Register("ToggleReplaceAudio_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));


        public bool ToggleShowLog_Value
        {
            get { return (bool)GetValue(ToggleShowLog_ValueProperty); }
            set { SetValue(ToggleShowLog_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToggleShowLog_Value.
        public static readonly DependencyProperty ToggleShowLog_ValueProperty =
            DependencyProperty.Register("ToggleShowLog_Value", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));



        public string InputFramerate_Value
        {
            get { return (string)GetValue(InputFramerate_ValueProperty); }
            set { SetValue(InputFramerate_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputFramerate_Value.
        public static readonly DependencyProperty InputFramerate_ValueProperty =
            DependencyProperty.Register("InputFramerate_Value", typeof(string), typeof(MainWindow), new PropertyMetadata(Properties.Settings.Default.defaultFrameRate));


        public string TheLog
        {
            get { return (string)GetValue(TheLogProperty); }
            set { SetValue(TheLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TheLog.
        public static readonly DependencyProperty TheLogProperty =
            DependencyProperty.Register("TheLog", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));


        public string LabelLeftStatus_ContentValue
        {
            get { return (string)GetValue(LabelLeftStatus_ContentValueProperty); }
            set { SetValue(LabelLeftStatus_ContentValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelLeftStatus_ContentValue.
        public static readonly DependencyProperty LabelLeftStatus_ContentValueProperty =
            DependencyProperty.Register("LabelLeftStatus_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));


        public string LabelRightStatus_ContentValue
        {
            get { return (string)GetValue(LabelRightStatus_ContentValueProperty); }
            set { SetValue(LabelRightStatus_ContentValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelRightStatus_ContentValue.
        public static readonly DependencyProperty LabelRightStatus_ContentValueProperty =
            DependencyProperty.Register("LabelRightStatus_ContentValue", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));


        public string LabelMediaACurrent_Value
        {
            get { return (string)GetValue(LabelMediaACurrent_ValueProperty); }
            set { SetValue(LabelMediaACurrent_ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelMediaACurrent_Value.
        public static readonly DependencyProperty LabelMediaACurrent_ValueProperty =
            DependencyProperty.Register("LabelMediaACurrent_Value", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));


        public double MediaPositionA
        {
            get { return (double)GetValue(MediaPositionAProperty); }
            set { SetValue(MediaPositionAProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaPositionA.
        public static readonly DependencyProperty MediaPositionAProperty =
            DependencyProperty.Register("MediaPositionA", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));


        public int MediaAPositionAsFrames
        {
            get { return (int)GetValue(MediaAPositionAsFramesProperty); }
            set { SetValue(MediaAPositionAsFramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaAPositionAsFrames.
        public static readonly DependencyProperty MediaAPositionAsFramesProperty =
            DependencyProperty.Register("MediaAPositionAsFrames", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int MediaBPositionAsFrames
        {
            get { return (int)GetValue(MediaBPositionAsFramesProperty); }
            set { SetValue(MediaBPositionAsFramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaAPositionAsFrames.
        public static readonly DependencyProperty MediaBPositionAsFramesProperty =
            DependencyProperty.Register("MediaBPositionAsFrames", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int MediaADurationAsFrames
        {
            get { return (int)GetValue(MediaADurationAsFramesProperty); }
            set { SetValue(MediaADurationAsFramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaADurationAsFrames.
        public static readonly DependencyProperty MediaADurationAsFramesProperty =
            DependencyProperty.Register("MediaADurationAsFrames", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int MediaBDurationAsFrames
        {
            get { return (int)GetValue(MediaBDurationAsFramesProperty); }
            set { SetValue(MediaBDurationAsFramesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaADurationAsFrames.
        public static readonly DependencyProperty MediaBDurationAsFramesProperty =
            DependencyProperty.Register("MediaBDurationAsFrames", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        public int MaxConcurrentEncoding
        {
            get {
                try
                {
                    return (int)GetValue(MaxConcurrentEncodingProperty);
                }
                catch
                {
                    return 0;
                }
                }
            set { SetValue(MaxConcurrentEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaADurationAsFrames.
        public static readonly DependencyProperty MaxConcurrentEncodingProperty =
            DependencyProperty.Register("MaxConcurrentEncoding", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));
    }
}
