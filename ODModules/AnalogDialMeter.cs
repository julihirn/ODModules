using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ODModules {
    public class AnalogDialMeter : Control {
        #region Control Methods
        public AnalogDialMeter() {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
           // SetStyle(ControlStyles.Opaque, true);
            DoubleBuffered = true;
            AnimationTimer = new System.Windows.Forms.Timer();
            AnimationTimer.Interval = 1;
            AnimationTimer.Enabled = false;
            AnimationTimer.Tick += AnimationTimer_Tick;
            lowBar = new Range(0, 50);
            mediumBar = new Range(50, 70);
            highBar = new Range(70, 100);
        }
        #endregion
        #region Properties 
        [System.ComponentModel.Browsable(false)]
        protected bool InDesignMode {
            get {
                return (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            }
        }
        Color lowBarColor = Color.Green;
        System.Windows.Forms.Timer AnimationTimer;
        [System.ComponentModel.Category("Color Scale")]
        public Color LowBarColor {
            get {
                return lowBarColor;
            }
            set {
                lowBarColor = value;
                Invalidate();
            }
        }
        Color mediumBarColor = Color.Yellow;
        [System.ComponentModel.Category("Color Scale")]
        public Color MediumBarColor {
            get {
                return mediumBarColor;
            }
            set {
                mediumBarColor = value;
                Invalidate();
            }
        }
        Color highBarColor = Color.Red;
        [System.ComponentModel.Category("Color Scale")]
        public Color HighBarColor {
            get {
                return highBarColor;
            }
            set {
                highBarColor = value;
                Invalidate();
            }
        }
        void AdjustRanges() {
            if (lowBar.Stop != mediumBar.Start) {
                MediumBar = new Range(lowBar.Stop, mediumBar.Stop);
            }
            if (mediumBar.Stop != highBar.Start) {
                HighBar = new Range(mediumBar.Stop, highBar.Stop);
            }
        }
        Range lowBar;
        [System.ComponentModel.Category("Color Scale")]
        public Range LowBar {
            get {
                return lowBar;
            }
            set {
                if (RangeDifference(value) < 0) {
                    lowBar = new Range(value.Stop, value.Start);
                }
                else {
                    lowBar = value;
                }
                if (lowBar.Stop != mediumBar.Start) {
                    MediumBar = new Range(lowBar.Stop, mediumBar.Stop);
                }
                //if (mediumBar.Stop != highBar.Start) {
                //    HighBar = new Range(mediumBar.Stop, highBar.Stop);
                //}
                Invalidate();
            }
        }
        Range mediumBar;
        [System.ComponentModel.Category("Color Scale")]
        public Range MediumBar {
            get {
                return mediumBar;
            }
            set {
                if (RangeDifference(value) < 0) {
                    mediumBar = new Range(value.Stop, value.Start);
                }
                else {
                    mediumBar = value;
                }
                if (lowBar.Stop != mediumBar.Start) {
                    LowBar = new Range(lowBar.Start, mediumBar.Start);
                }
                if (mediumBar.Stop != highBar.Start) {
                    HighBar = new Range(mediumBar.Stop, highBar.Stop);
                }
                Invalidate();
            }
        }
        Range highBar;
        [System.ComponentModel.Category("Color Scale")]
        public Range HighBar {
            get {
                return highBar;
            }
            set {
                if (RangeDifference(value) < 0) {
                    highBar = new Range(value.Stop, value.Start);
                }
                else {
                    highBar = value;
                }
                if (mediumBar.Stop != highBar.Start) {
                    MediumBar = new Range(mediumBar.Start, highBar.Start);
                }
                Invalidate();
            }
        }
        decimal minimumValue = 0;
        [System.ComponentModel.Category("Control")]
        public decimal MinimumValue {
            get {
                return minimumValue;
            }
            set {
                if (value >= maximumValue) { minimumValue = maximumValue; }
                else { minimumValue = value; }
                Invalidate();
            }
        }
        decimal maximumValue = 0;
        [System.ComponentModel.Category("Control")]
        public decimal MaximumValue {
            get {
                return maximumValue;
            }
            set {
                if (value <= minimumValue) { maximumValue = minimumValue; }
                else { maximumValue = value; }
                Invalidate();
            }
        }
        decimal needleSize = (decimal)2;
        [System.ComponentModel.Category("Appearance")]
        public decimal NeedleSize {
            get {
                return needleSize;
            }
            set {
                if (value < (decimal)0.3) {
                    needleSize = (decimal)0.3f;
                }
                else if (value > 5) {
                    needleSize = 5;
                }
                else {
                    needleSize = value;
                }

                Invalidate();
            }
        }
        decimal currentValue = 0;
        decimal displayValue = 0;
        decimal NeedleValue = 0;
        decimal ValueDifferenceStep = 0;
        int AnimationFrames = 1000;
        [System.ComponentModel.Category("Control")]
        public decimal DisplayValue {
            get {
                return currentValue;
            }
        }
        [System.ComponentModel.Category("Control")]
        public decimal Value {
            get {
                return displayValue;
            }
            set {
                displayValue = value;
                if (value < minimumValue) {
                    currentValue = minimumValue;
                }
                else if (value > maximumValue) {
                    currentValue = maximumValue;
                }
                else {
                    currentValue = value;
                }
                if (smoothNeedleMovements == true) {
                    if (InDesignMode == false) {
                        ValueDifferenceStep = (currentValue - NeedleValue);// / (decimal)AnimationFrames;
                        AnimationTimer.Enabled = true;
                    }
                    else {
                        NeedleValue = value;
                    }
                }
                Invalidate();
            }
        }
        Color needleColor = Color.DarkGray;
        [System.ComponentModel.Category("Appearance")]
        public Color NeedleColor {
            get {
                return needleColor;
            }
            set {
                needleColor = value;
                Invalidate();
            }
        }
        Color needleInnerColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color NeedleCenterColor {
            get {
                return needleInnerColor;
            }
            set {
                needleInnerColor = value;
                Invalidate();
            }
        }
        Color axialColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color AxialColor {
            get {
                return axialColor;
            }
            set {
                axialColor = value;
                Invalidate();
            }
        }
        Color meterBackColor = Color.Gainsboro;
        [System.ComponentModel.Category("Appearance")]
        public Color MeterBackColor {
            get {
                return meterBackColor;
            }
            set {
                meterBackColor = value;
                Invalidate();
            }
        }
        Color meterBorderColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color MeterBorderColor {
            get {
                return meterBorderColor;
            }
            set {
                meterBorderColor = value;
                Invalidate();
            }
        }
        string valueUnit = "";
        [System.ComponentModel.Category("Appearance")]
        public string ValueUnit {
            get {
                return valueUnit;
            }
            set {
                valueUnit = value;
                Invalidate();
            }
        }
        Color tickColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color TickColor {
            get {
                return tickColor;
            }
            set {
                tickColor = value;
                Invalidate();
            }
        }
        Color valueForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ValueForeColor {
            get {
                return valueForeColor;
            }
            set {
                valueForeColor = value;
                Invalidate();
            }
        }
        Color meterBorderShadowColor = Color.FromArgb(40, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color MeterBorderShadowColor {
            get {
                return meterBorderShadowColor;
            }
            set {
                meterBorderShadowColor = value;
                Invalidate();
            }
        }
        bool borderShadow = true;
        [System.ComponentModel.Category("Appearance")]
        public bool MeterBorderShadow {
            get {
                return borderShadow;
            }
            set {
                borderShadow = value;
                Invalidate();
            }
        }
        bool showLabel = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowLabel {
            get {
                return showLabel;
            }
            set {
                showLabel = value;
                Invalidate();
            }
        }
        bool showColorScale = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowColorScale {
            get {
                return showColorScale;
            }
            set {
                showColorScale = value;
                Invalidate();
            }
        }
        bool showTicks = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowTicks {
            get {
                return showTicks;
            }
            set {
                showTicks = value;
                Invalidate();
            }
        }
        bool showValue = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowValue {
            get {
                return showValue;
            }
            set {
                showValue = value;
                Invalidate();
            }
        }
        bool smoothNeedleMovements = false;
        [System.ComponentModel.Category("Behavior")]
        public bool SmoothNeedleMovements {
            get {
                return smoothNeedleMovements;
            }
            set {
                smoothNeedleMovements = value;
                if (value == false) {
                    AnimationTimer.Enabled = false;
                }
                Invalidate();
            }
        }
        #endregion
        #region Drawing 
        int UnitTextSize = 10;
        Point Centre;
        double Radius = 10;

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (Font UnitFont = new Font(Font.FontFamily, 11.0f)) {
                UnitTextSize = (int)e.Graphics.MeasureString("W", UnitFont).Height;
            }
            Centre = new Point((int)((double)Width / 2.0f), (int)((double)Height / 2.0f));
            Radius = GetBoundingRadius(Size, UnitTextSize);
            try {
                DrawBody(e);
                DrawText(e);
                DrawZones(e);
                DrawTicks(e);
                DrawNeedle(e);
            }
            catch {

            }
        }
        private void DrawBody(PaintEventArgs e) {
            using (SolidBrush MeterBackBrush = new SolidBrush(meterBorderColor)) {
                e.Graphics.FillEllipse(MeterBackBrush, CentreCircle(Radius));
            }
            Rectangle Circle = CentreCircle(0.9f * Radius);
            using (SolidBrush MeterBorderBrush = new SolidBrush(meterBackColor)) {
                e.Graphics.FillEllipse(MeterBorderBrush, Circle);
            }
            if (borderShadow == true) {
                using (GraphicsPath BorderShowPath = new GraphicsPath()) {
                    BorderShowPath.AddEllipse(CentreCircle(0.9f * Radius));
                    using (PathGradientBrush BorderShadowBrush = new PathGradientBrush(BorderShowPath)) {
                        BorderShadowBrush.SurroundColors = new Color[] { meterBorderShadowColor };
                        BorderShadowBrush.CenterColor = Color.Transparent;
                        e.Graphics.FillEllipse(BorderShadowBrush, Circle);
                    }
                }
            }
        }
        private void DrawNeedle(PaintEventArgs e) {
            double NeedleAnglePosition = GetNeedlePosition();
            double WidthOffset = (double)UnitTextSize * (double)needleSize;
            Point NeedlePnt1 = ToRectangular(NeedleAnglePosition - WidthOffset, (int)(Radius * 0.1f), Centre);
            Point NeedlePnt2 = ToRectangular(NeedleAnglePosition, (int)(Radius * 0.8f), Centre);
            Point NeedlePnt3 = ToRectangular(NeedleAnglePosition + WidthOffset, (int)(Radius * 0.1f), Centre);
            using (LinearGradientBrush NeedleBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), needleInnerColor, needleColor, (float)NeedleAnglePosition)) {
                Point[] NeedlePoints = { NeedlePnt1, NeedlePnt2, NeedlePnt3 };
                e.Graphics.FillPolygon(NeedleBrush, NeedlePoints);
                using (SolidBrush NeedleBorderBrush = new SolidBrush(needleInnerColor)) {
                    using (Pen NeedleBorderPen = new Pen(NeedleBrush)) {
                        e.Graphics.DrawPolygon(NeedleBorderPen, NeedlePoints);
                    }
                }
            }
            using (SolidBrush AxialBrush = new SolidBrush(Color.FromArgb(axialColor.R, axialColor.G, axialColor.B))) {
                double ExpandFactor = WidthOffset * 0.001f;
                e.Graphics.FillEllipse(AxialBrush, CentreCircle((0.12f + ExpandFactor) * Radius));
            }
        }
        private void DrawZones(PaintEventArgs e) {
            double ArcRadius = 0.7f * Radius;
            float AngleStart = 135.0f;
            float AngleSpan = 270.0f;
            if (showColorScale == true) {
                Range MinZone = LimitZone(lowBar);
                Range MidZone = LimitZone(mediumBar);
                Range MaxZone = LimitZone(highBar);
                decimal ZoneSize = RangeDifference(MinZone) + RangeDifference(MidZone) + RangeDifference(MaxZone);
                decimal BarStart = (decimal)AngleStart + ((decimal)AngleSpan * (MinZone.Start + Math.Abs(minimumValue)) / (Math.Abs(minimumValue) + Math.Abs(maximumValue)));
                decimal BarSize = Math.Abs(minimumValue) + Math.Abs(maximumValue);
                decimal CurrentStartPoint = BarStart;
                if (RangeDifference(MinZone) > 0) {
                    using (SolidBrush LowZoneBrush = new SolidBrush(lowBarColor)) {
                        using (Pen LowZonePen = new Pen(LowZoneBrush, 3)) {
                            decimal Length = (RangeDifference(MinZone) / BarSize) * (decimal)AngleSpan;
                            e.Graphics.DrawArc(LowZonePen, CentreCircle(ArcRadius), (float)CurrentStartPoint, (float)Length);
                            CurrentStartPoint += Length;
                        }
                    }
                }
                if (RangeDifference(MidZone) > 0) {
                    using (SolidBrush LowZoneBrush = new SolidBrush(mediumBarColor)) {
                        using (Pen LowZonePen = new Pen(LowZoneBrush, 3)) {
                            decimal Length = (RangeDifference(MidZone) / BarSize) * (decimal)AngleSpan;
                            e.Graphics.DrawArc(LowZonePen, CentreCircle(ArcRadius), (float)CurrentStartPoint, (float)Length);
                            CurrentStartPoint += Length;
                        }
                    }
                }
                if (RangeDifference(MaxZone) > 0) {
                    using (SolidBrush LowZoneBrush = new SolidBrush(highBarColor)) {
                        using (Pen LowZonePen = new Pen(LowZoneBrush, 3)) {
                            decimal Length = (RangeDifference(MaxZone) / BarSize) * (decimal)AngleSpan;
                            e.Graphics.DrawArc(LowZonePen, CentreCircle(ArcRadius), (float)CurrentStartPoint, (float)Length);
                        }
                    }
                }
            }
        }
        private void DrawTicks(PaintEventArgs e) {
            int InnerTickRadius = (int)(Radius * 0.7f);
            int OutterSmallTickRadius = (int)(Radius * 0.65f);
            int OutterLargeTickRadius = (int)(Radius * 0.6f);
            int LabelRadius = (int)(Radius * 0.55f);
            double AngleStart = 135.0f;
            double AngleSpan = 270.0f;
            using (SolidBrush TickBrush = new SolidBrush(tickColor)) {
                if (showTicks == true) {
                    using (Pen TickPen = new Pen(TickBrush)) {
                        for (int i = 0; i < 101; i++) {
                            if ((i % 10) == 0) {
                                double Angle = (AngleStart + (AngleSpan * ((double)i / 100.0f)));
                                e.Graphics.DrawLine(TickPen, ToRectangular(Angle, InnerTickRadius, Centre), ToRectangular(Angle, OutterLargeTickRadius, Centre));
                            }
                            else if ((i % 5) == 0) {
                                double Angle = (AngleStart + (AngleSpan * ((double)i / 100.0f)));
                                e.Graphics.DrawLine(TickPen, ToRectangular(Angle, InnerTickRadius, Centre), ToRectangular(Angle, OutterSmallTickRadius, Centre));
                            }
                            if ((i % 20) == 0) {
                                double Angle = (AngleStart + (AngleSpan * ((double)i / 100.0f)));
                                decimal Range = Math.Abs(maximumValue - minimumValue);
                                decimal TickValue = (Range * ((decimal)i / 100.0m)) + minimumValue;
                                string ValueString = FormatNumber(TickValue);
                                using (Font UnitFont = new Font(Font.FontFamily, 11.0f)) {
                                    int UnitTickWidth = (int)e.Graphics.MeasureString(ValueString, UnitFont).Width;
                                    using (Font TickFont = GetAdjustedFont(e, ValueString, UnitFont, UnitTickWidth, 100, 8, true)) {
                                        int UnitTickHeight = (int)e.Graphics.MeasureString(ValueString, TickFont).Height;
                                        Point TextPosition = ToRectangular(Angle, LabelRadius, Centre);
                                        //TextPosition.X -= (int)((float)UnitTickWidth / 2.0f);
                                        //TextPosition.Y -= (int)((float)UnitTickHeight / 2.0f);
                                        using (StringFormat LabelFormat = new StringFormat()) {
                                            LabelFormat.Alignment = StringAlignment.Center;
                                            e.Graphics.DrawString(ValueString, TickFont, TickBrush, TextPosition, LabelFormat);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private string FormatNumber(decimal Input, bool ValueDisplay = false) {
            decimal Temp = 0;
            string Output = "";
            bool RequiresPadding = false;
            if (Input >= 1000000000000) {
                Temp = Input / 1000000000000;
                Output = Temp.ToString("0.0") + " T"; RequiresPadding = true;
            }
            else if (Input >= 1000000000) {
                Temp = Input / 1000000000;
                Output = Temp.ToString("0.0") + " G"; RequiresPadding = true;
            }
            else if (Input >= 1000000) {
                Temp = Input / 1000000;
                Output = Temp.ToString("0.0") + " M"; RequiresPadding = true;
            }
            else if (Input >= 1000) {
                Temp = Input / 1000;
                Output = Temp.ToString("0.0") + " k"; RequiresPadding = true;
            }
            else if (Input >= 100) {
                Output = Input.ToString("0.0");
            }
            else{
                Output = Input.ToString("0.00");
            }
            if (ValueDisplay == true) {
                if ((valueUnit.Length > 0)&&(RequiresPadding == true)) { Output += " "; }
            }
            return Output;
        }
        private void DrawText(PaintEventArgs e) {
            // Rectangle Centre = CentreCircle(0);
            double BoxYOffset = Radius * 0.22f;
            int LabelOffset = 0;
            if (showLabel == true) {
                using (SolidBrush TextBrush = new SolidBrush(ForeColor)) {

                    double BoxWidth = Radius * 0.95f;
                    double BoxHeight = Radius * 0.1f;
                    Rectangle BoundingBox = new Rectangle(Centre.X - (int)(BoxWidth / 2.0f), Centre.Y + (int)(BoxYOffset), (int)BoxWidth, (int)BoxHeight);
                    using (Font LabelFont = GetAdjustedFont(e, Text, Font, BoundingBox.Width, 100, 2, true)) {
                        LabelOffset = (int)e.Graphics.MeasureString("W", LabelFont).Height;
                        BoundingBox.Height = LabelOffset;
                        using (StringFormat LabelFormat = new StringFormat()) {
                            LabelFormat.Alignment = StringAlignment.Center;
                            LabelFormat.Trimming = StringTrimming.None;
                            e.Graphics.DrawString(Text, LabelFont, TextBrush, BoundingBox, LabelFormat);
                        }
                        BoxYOffset += LabelOffset;
                    }

                    //using (Pen Pn = new Pen(TextBrush)) {

                    //    e.Graphics.DrawRectangle(Pn, BoundingBox);
                    //}
                }
            }
            if (showValue == true) {
                using (SolidBrush TextBrush = new SolidBrush(valueForeColor)) {
                    double BoxWidth = Radius * 0.95f;
                    double BoxHeight = Radius * 0.1f;
                    string ValueString = "";// Value.ToString("F2");
                    if (smoothNeedleMovements == true) {
                        ValueString = FormatNumber(NeedleValue);//.ToString("F2");
                    }
                    else {
                        ValueString = FormatNumber(Value);
                    }
                    if (valueUnit.Length > 0) {
                        ValueString += valueUnit;
                    }
                    Rectangle BoundingBox = new Rectangle(Centre.X - (int)(BoxWidth / 2.0f), Centre.Y + (int)(BoxYOffset), (int)BoxWidth, (int)BoxHeight);
                    using (Font LabelFont = GetAdjustedFont(e, ValueString, Font, BoundingBox.Width, 100, 2, true)) {
                        LabelOffset = (int)e.Graphics.MeasureString("W", LabelFont).Height;
                        BoundingBox.Height = LabelOffset;
                        using (StringFormat LabelFormat = new StringFormat()) {
                            LabelFormat.Alignment = StringAlignment.Center;
                            LabelFormat.Trimming = StringTrimming.None;

                            e.Graphics.DrawString(ValueString, LabelFont, TextBrush, BoundingBox, LabelFormat);
                        }
                    }
                }
            }
        }
        #endregion
        #region Drawing Support 
        public Font GetAdjustedFont(PaintEventArgs e, string graphicString, Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail) {
            Font testFont = this.Font;
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--) {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);
                string Adjusted = graphicString;
                if (Adjusted.Length < 11) {
                    Adjusted = PadStart(graphicString, 11 - Adjusted.Length);
                }
                SizeF adjustedSizeNew = e.Graphics.MeasureString(Adjusted, testFont);
                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width)) {
                    return testFont;
                }
            }
            if (smallestOnFail) {
                return testFont;
            }
            else {
                return originalFont;
            }
        }
        string PadStart(string Input, int StartPad) {
            string Output = "";
            if (StartPad <= 0) {
                return Input;
            }
            else {
                for (int i = 0; i < StartPad; i++) {
                    Output += "W";
                }
                Output += Input;
                return Output;
            }
        }
        Range LimitZone(Range Input) {
            Range Output = new Range(0, 0);
            if (Input.Start < minimumValue) {
                Output.Start = minimumValue;
            }
            else if (Input.Start > maximumValue) {
                Output.Start = maximumValue;
            }
            else {
                Output.Start = Input.Start;
            }
            if (Input.Stop < minimumValue) {
                Output.Stop = minimumValue;
            }
            else if (Input.Stop > maximumValue) {
                Output.Stop = maximumValue;
            }
            else {
                Output.Stop = Input.Stop;
            }
            return Output;
        }
        decimal RangeDifference(Range Input) {
            return Input.Stop - Input.Start;
        }
        Point ToRectangular(double Angle, int Radius, Point Offset) {
            double x = (double)(Radius * Math.Cos(Angle * Math.PI / 180.0f)) + (double)Offset.X;
            double y = (double)(Radius * Math.Sin(Angle * Math.PI / 180.0f)) + (double)Offset.Y;
            return new Point((int)x, (int)y);
        }
        Rectangle CentreCircle(Double CircleRadius) {
            return new Rectangle(Centre.X - (int)CircleRadius, Centre.Y - (int)CircleRadius, (int)CircleRadius * 2, (int)CircleRadius * 2);
        }
        double GetBoundingRadius(Size BoxSize, int BorderPadding) {
            if (BoxSize.Width > BoxSize.Height) {
                return ((double)BoxSize.Height - (double)BorderPadding) / 2.0f;
            }
            else {
                return ((double)BoxSize.Width - (double)BorderPadding) / 2.0f;
            }
        }
        double GetNeedlePosition() {
            double AngleStart = 135.0f;
            double AngleSpan = 270.0f;
            double Distance = 1;
            if (maximumValue > minimumValue) {
                Distance = (double)Math.Abs(maximumValue - minimumValue);
            }
            else if ( minimumValue > maximumValue) {
                Distance = (double)Math.Abs(minimumValue - maximumValue);
            }
            double NeedleValueOut = 0;
            if (smoothNeedleMovements == false) {
                //return AngleStart + (AngleSpan * ((double)currentValue + Math.Abs((double)minimumValue)) / (double)(Math.Abs(minimumValue) + Math.Abs(maximumValue)));
                NeedleValueOut = AngleStart + (AngleSpan * ((double)currentValue - (double)minimumValue) / Distance);
            }
            else {
                NeedleValueOut = AngleStart + (AngleSpan * ((double)NeedleValue - (double)minimumValue) / Distance);
            }
            if (NeedleValueOut < AngleStart) { return AngleStart; }
            else if(NeedleValueOut > AngleStart + AngleSpan) { return AngleStart + AngleSpan; }
            else { return NeedleValueOut; }

        }
        #endregion
        #region Events
        int Tick = 0;
        private void AnimationTimer_Tick(object? sender, EventArgs e) {
            if (InDesignMode == false) {
                if ((currentValue <= NeedleValue + (decimal)0.01) && (currentValue >= NeedleValue - (decimal)0.01)) {
                    NeedleValue = currentValue;
                    ValueDifferenceStep = 0;
                    AnimationTimer.Enabled = false;
                    Tick = 0;
                }
                else {
                    ValueDifferenceStep = (currentValue - NeedleValue);
                    NeedleValue += ValueDifferenceStep * (decimal)Math.Abs(Math.Sin(Math.PI * ((double)Tick / (double)AnimationFrames)));
                    Tick++;
                }
                Invalidate();
            }

        }
        protected override void OnResize(EventArgs e) {
            Invalidate();
            base.OnResize(e);
        }
        #endregion
        #region Methods
        public void PegNeedleAtMaximum() {
            Value = maximumValue;
        }
        public void PegNeedleAtMinimum() {
            Value = minimumValue;
        }
        #endregion
    }
    #region Classes and Structures
    [TypeConverter(typeof(ValueTypeTypeConverter))]
    public struct Range {
        private decimal start;
        private decimal stop;
        public decimal Start { get => start; set => start = value; }
        public decimal Stop { get => stop; set => stop = value; }
        public Range(decimal start, decimal stop) {
            this.start = start;
            this.stop = stop;
        }
    }
    public class ValueTypeTypeConverter : System.ComponentModel.ExpandableObjectConverter {
        public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext? context) {
            return true;
        }
        public override object? CreateInstance(System.ComponentModel.ITypeDescriptorContext? context, System.Collections.IDictionary? propertyValues) {
            if (propertyValues == null)
                throw new ArgumentNullException("propertyValues");
            if(context == null) { return null; }
            object? boxed = Activator.CreateInstance(context.PropertyDescriptor.PropertyType);
            foreach (System.Collections.DictionaryEntry entry in propertyValues) {
                if (entry.Key != null) {
                    string keyname = entry.Key.ToString() ?? "";
                    System.Reflection.PropertyInfo? pi = context.PropertyDescriptor.PropertyType.GetProperty(keyname);
                    if ((pi != null) && (pi.CanWrite)) {
                        pi.SetValue(boxed, Convert.ChangeType(entry.Value, pi.PropertyType), null);
                    }
                }
            }
            return boxed;
        }
    }
    #endregion
}