using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Diagnostics;
using System.Security.Policy;
using Handlers;
namespace ODModules {
    public partial class NumericTextbox : UserControl {
        public delegate void ValueChangedHandler(object sender, ValueChangedEventArgs e);
        [Category("Value")]
        public event ValueChangedHandler? ValueChanged;

        public delegate void PrefixClickedHandler(object sender, PrefixClickedEventArgs e);
        [Category("Value")]
        public event PrefixClickedHandler? PrefixClicked;

        public delegate void UnitClickedHandler(object sender, UnitClickedEventArgs e);
        [Category("Value")]
        public event UnitClickedHandler? UnitClicked;
        public delegate void UnitChangedHandler(object sender);
        [Category("Value")]
        public event UnitChangedHandler? UnitChanged;
        public delegate void PrefixChangedHandler(object sender);
        [Category("Value")]
        public event PrefixChangedHandler? PrefixChanged;
        public NumericTextbox() {
            InitializeComponent();
            this.borderColor = Color.DimGray;
            this.hovercolor = Color.LightGray;
            this.buttondowncolor = Color.Gray;
            this.selectedborderColor = Color.Beige;
            // KeyPreview = true;
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.FixedHeight, true);
            this.TabStop = true;
            this.labelFont = Font;
            this.labelForeColor = Color.Gray;

        }
        private object? secondaryTag = null;
        [Category("Control")]
        public object? SecondaryTag { get => secondaryTag; set => secondaryTag = value; }
        private bool allowClipboard = true;
        [Category("Control")]
        public bool AllowClipboard { get => allowClipboard; set => allowClipboard = value; }
        private bool allowTyping = true;
        [Category("Control")]
        public bool AllowTyping { get => allowTyping; set => allowTyping = value; }
        private bool allowNumberEntry = true;
        [Category("Control")]
        public bool AllowNumberEntry { get => allowNumberEntry; set => allowNumberEntry = value; }
        private bool allowMouseWheel = true;
        [Category("Control")]
        public bool AllowMouseWheel { get => allowMouseWheel; set => allowMouseWheel = value; }
        private bool allowDragValueChange = true;
        [Category("Control")]
        public bool AllowDragValueChange { get => allowDragValueChange; set => allowDragValueChange = value; }
        private bool showLabel = true;
        [Category("Show/Hide")]
        public bool ShowLabel {
            get { return showLabel; }
            set {
                showLabel = value;
                Invalidate();
            }
        }
        private Font labelFont;
        [Category("Appearance")]
        public Font LabelFont {
            get { return labelFont; }
            set {
                labelFont = value;
                Invalidate();
            }
        }
        private Color labelForeColor;
        [Category("Appearance")]
        public Color LabelForeColor {
            get { return labelForeColor; }
            set {
                labelForeColor = value;
                Invalidate();
            }
        }
        private string labelText = "";
        [Category("Appearance")]
        public string LabelText {
            get { return labelText; }
            set {
                labelText = value;
                Invalidate();
            }
        }
        private Color borderColor;
        [Category("Appearance")]
        public Color BorderColor {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }
        private Color selectedborderColor;
        [Category("Appearance")]
        public Color SelectedBorderColor {
            get { return selectedborderColor; }
            set { selectedborderColor = value; Invalidate(); }
        }
        private Color selectedBackColor;
        [Category("Appearance")]
        public Color SelectedBackColor {
            get { return selectedBackColor; }
            set { selectedBackColor = value; Invalidate(); }
        }
        private Color hovercolor;
        [Category("Appearance")]
        public Color ButtonHoverColor {
            get { return hovercolor; }
            set { hovercolor = value; Invalidate(); }
        }
        private Color buttondowncolor;
        [Category("Appearance")]
        public Color ButtonDownColor {
            get { return buttondowncolor; }
            set { buttondowncolor = value; Invalidate(); }
        }
        private bool autoSizeToText;
        [Category("Layout")]
        public bool AutoSizeToText {
            get { return autoSizeToText; }
            set { autoSizeToText = value; Invalidate(); }
        }
        private string ValueString = "0";
        [Category("Number")]
        public object Value {
            get { return ValueString; }
            set {
                // this.value = value;
                if (value == null) { }
                else {
                    if (rangeLimited == true) {
                        string HoldString = value.ToString() ?? "0";
                        CorrectValueToRange(HoldString, out ValueString);
                    }
                    else {
                        ValueString = value.ToString() ?? "0";
                    }
                }
                UserEntered = false;
                DecimalTrim();
                Invalidate();
            }
        }
        private NumberBase numberbase = NumberBase.Base10;
        [Category("Number")]
        public NumberBase Base {
            get { return numberbase; }
            set {
                numberbase = value;
                if (value != NumberBase.Base10) {
                    allowFractions = false;
                    allowNegatives = false;
                    numericalFormat = NumberFormat.Decimal;
                    hasUnit = false;

                }
                ValueString = "0";
                Invalidate();
            }
        }
        private bool allowFractions = true;
        [Category("Number")]
        public bool AllowFractionals {
            get { return allowFractions; }
            set {
                if (numberbase == NumberBase.Base10) {
                    allowFractions = value;
                }
                else {
                    allowFractions = false;
                }
                if (allowFractions == false) {
                    RemoveFractionals();
                }
                Invalidate();
            }
        }
        private bool allowNegatives = true;
        [Category("Number")]
        public bool AllowNegatives {
            get { return allowNegatives; }
            set {
                if (numberbase == NumberBase.Base10) {
                    allowNegatives = value;
                }
                else {
                    allowNegatives = false;
                }
                if (allowNegatives == false) {
                    RemoveNegatives();
                }
                Invalidate();
            }
        }
        bool UserEntered = false;
        //public double CurrentValue { get => currentValue; }
        [Description("Changes whether the number is a united quantity"), Category("Number")]
        public bool HasUnit {
            get {
                return hasUnit;
            }
            set {
                if (numberbase == NumberBase.Base10) {
                    hasUnit = value;
                }
                else { hasUnit = false; }
                if (hasUnit == false) { Prefix = MetricPrefix.None; }
                Invalidate();
            }
        }
        [Description("Is the unit metric"), Category("Number")]
        public bool IsMetric {
            get { return isMetric; }
            set {
                isMetric = value;
                if (value == false) { Prefix = MetricPrefix.None; }
                UnitChanged?.Invoke(this);
                Invalidate();
            }

        }
        [Description("Metric Prefix"), Category("Number")]
        public MetricPrefix Prefix {
            get {
                return prefix;
            }
            set {
                if (isMetric == true) {
                    prefix = value;
                }
                else { prefix = MetricPrefix.None; }
                Invalidate();
                PrefixChanged?.Invoke(this);
            }
        }
        private string unit = "";
        [Category("Number")]
        public string Unit { get { return unit; } set { unit = value; Invalidate(); } }

        private string secondaryUnit = "";
        [Category("Secondary Unit")]
        public string SecondaryUnit { get { return secondaryUnit; } set { secondaryUnit = value; Invalidate(); } }
        private MetricPrefix secondaryPrefix = MetricPrefix.None;
        [Category("Secondary Unit")]
        public MetricPrefix SecondaryPrefix {
            get {
                return secondaryPrefix;
            }
            set {
                if (isSecondaryMetric == true) {
                    secondaryPrefix = value;
                }
                else { secondaryPrefix = MetricPrefix.None; }
                Invalidate();
                PrefixChanged?.Invoke(this);
            }
        }
        bool isSecondaryMetric = false;
        [Category("Secondary Unit")]
        public bool IsSecondaryMetric {
            get { return isSecondaryMetric; }
            set {
                isSecondaryMetric = value;
                if (value == false) { secondaryPrefix = MetricPrefix.None; }
                UnitChanged?.Invoke(this);
                Invalidate();
            }

        }

        private SecondaryUnitDisplayType secondaryUnitDisplay = SecondaryUnitDisplayType.NoSecondaryUnit;
        [Category("Secondary Unit")]
        public SecondaryUnitDisplayType SecondaryUnitDisplay {
            get {
                return secondaryUnitDisplay;
            }
            set {
                secondaryUnitDisplay = value;
                Invalidate();
            }
        }
        NumericalString minimum = new NumericalString();
        [Category("Number Range")]
        public NumericalString Minimum {
            get { return minimum; }
            set {
                if (maximum < value) {
                    minimum.Value = maximum.Value;
                }
                else {
                    minimum = value;
                }
                Invalidate();
            }
        }
        NumericalString maximum = new NumericalString(100);
        [Category("Number Range")]
        public NumericalString Maximum {
            get { return maximum; }
            set {
                if (minimum > value) {
                    maximum.Value = minimum.Value;
                }
                else {
                    maximum = value;
                }
                Invalidate();
            }
        }
        bool rangeLimited = false;
        [Category("Number Range")]
        public bool RangeLimited {
            get { return rangeLimited; }
            set {
                rangeLimited = value;
                Invalidate();
            }
        }

        bool formatOutput = true;
        [Category("Number")]
        public bool FormatOutput {
            get { return formatOutput; }
            set {
                formatOutput = value;
                Invalidate();
            }

        }
        NumberFormat numericalFormat = NumberFormat.Decimal;
        [Category("Number")]
        public NumberFormat NumericalFormat {
            get { return numericalFormat; }
            set {
                if (numberbase == NumberBase.Base10) {
                    numericalFormat = value;
                }
                else {
                    numericalFormat = NumberFormat.Decimal;
                }
                Invalidate();
            }
        }
        int numericalLeftRadixDigitsMaximum = 7;
        [Category("Number")]
        public int NumericalLeftRadixDigitsMaximum {
            get { return numericalLeftRadixDigitsMaximum; }
            set {
                if (value <= 4) {
                    numericalLeftRadixDigitsMaximum = 4;
                }
                else {
                    numericalLeftRadixDigitsMaximum = value;
                }
                Invalidate();
            }
        }
        int numericalRightRadixDigitsMaximum = 4;
        [Category("Number")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int NumericalRightRadixDigitsMaximum {
            get { return numericalRightRadixDigitsMaximum; }
            set {
                if (value <= 4) {
                    numericalRightRadixDigitsMaximum = 4;
                }
                else {
                    numericalRightRadixDigitsMaximum = value;
                }
                Invalidate();
            }
        }
        uint maxmiumDecimalPlaces = 18;
        [Category("Number")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public uint MaxmiumDecimalPlaces {
            get { return maxmiumDecimalPlaces; }
            set {
                maxmiumDecimalPlaces = value;
                Invalidate();
            }

        }
        [Category("Number")]
        private MetricPrefix prefix = MetricPrefix.None;


        private bool hasUnit = true;
        private bool isMetric = true;
        private double currentValue = 0;
        //private double value = 0.0f;
        #region Control Drawing
        Rectangle Bounding = new Rectangle(0, 0, 0, 0);
        int EndPoint = 0;
        protected override void OnPaint(PaintEventArgs pe) {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Bounding = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            Color BackCol = BackColor;
            if (this.Focused == true) {
                using (SolidBrush br = new SolidBrush(selectedBackColor)) {
                    pe.Graphics.FillRectangle(br, this.Bounding);
                    BackCol = selectedBackColor;
                }
            }
            if (autoSizeToText == true) {
                int h = (int)(pe.Graphics.MeasureString("W", Font).Height * 1.5f) + Padding.Bottom + Padding.Top;
                this.Height = h;
                this.MinimumSize = new Size(this.MinimumSize.Width, h);
            }
            else {
                this.MinimumSize = new Size(this.MinimumSize.Width, 0);
            }
            //base.OnPaint(pe);
            //DrawFixtures(pe);
            //TextBoxRenderer.DrawTextBox(pe.Graphics, Bounding, "Hello", this.Font, System.Windows.Forms.VisualStyles.TextBoxState.Normal);

            DrawUnit(pe);
            RenderValueText(pe, BackCol);
            if (showLabel == true) {
                DrawLabel(pe);
            }
            if (this.Focused == true) {
                DrawBorder(pe, Bounding, SelectedBorderColor);
            }
            else {
                DrawBorder(pe, Bounding, BorderColor);
            }

            //Bounding.Inflate(-1, -1);
            //DrawBorder(pe, Bounding, RenderHandler.DeterministicDarkenColor(BorderColor, BackColor, 100), 1);


        }
        protected int MeasureDisplayStringWidth(Graphics graphics, string text, Font font) {
            if (text == "")
                return 0;

            StringFormat format = new StringFormat(StringFormat.GenericDefault);
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);
            format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Right) - 3;
        }
        Rectangle PrefixRectangle = new Rectangle(-1, -1, 0, 0);
        Rectangle UnitRectangle = new Rectangle(-1, -1, 0, 0);

        Rectangle PrefixRectangle2 = new Rectangle(-1, -1, 0, 0);
        Rectangle UnitRectangle2 = new Rectangle(-1, -1, 0, 0);
        //private void DrawUnit(PaintEventArgs e) {
        //        TextFormatFlags Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.PreserveGraphicsClipping;
        //        //Size BasisSize = new Size(int.MaxValue, int.MaxValue);
        //        string PrefixStr = PrefixToSymbol(Prefix);
        //        int PrefixSize = MeasureDisplayStringWidth(e.Graphics, PrefixStr, this.Font); //MinimumSizing(
        //        int UnitSize = MeasureDisplayStringWidth(e.Graphics, unit, this.Font);
        //        int PaddingUnit = MeasureDisplayStringWidth(e.Graphics, "w", this.Font);
        //        if (unit.Trim(' ').Length == 0) { UnitSize = 0; }
        //        if (hasUnit == false) { UnitSize = 0; }
        //        if (isMetric == false) {
        //            PrefixSize = 0;
        //            if (UnitSize == 0) { PaddingUnit = PaddingUnit/2; }
        //        }
        //        int ButtonBasisPosition = this.Width - UnitSize - PrefixSize - PaddingUnit;
        //        EndPoint = ButtonBasisPosition;
        //        if (isMetric == true) {
        //            PrefixRectangle = new Rectangle(ButtonBasisPosition, 0, PrefixSize, Height);
        //            TextRenderer.DrawText(e.Graphics, PrefixStr, this.Font, PrefixRectangle, GetColorMouseAction(PrefixRectangle), Flags);
        //        }
        //        if (UnitSize != 0) {
        //            UnitRectangle = new Rectangle(ButtonBasisPosition + PrefixSize, 0, UnitSize, Height);
        //            TextRenderer.DrawText(e.Graphics, unit, this.Font, UnitRectangle, GetColorMouseAction(UnitRectangle), Flags);
        //        }
        //}
        private void DrawUnit(PaintEventArgs e) {
            TextFormatFlags Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.PreserveGraphicsClipping;
            //Size BasisSize = new Size(int.MaxValue, int.MaxValue);
            string PrefixStr = PrefixToSymbol(Prefix);
            string PrefixStr2 = PrefixToSymbol(secondaryPrefix);
            int PrefixSize = MeasureDisplayStringWidth(e.Graphics, PrefixStr, this.Font); //MinimumSizing(
            int UnitSize = MeasureDisplayStringWidth(e.Graphics, unit, this.Font);
            int PaddingUnit = MeasureDisplayStringWidth(e.Graphics, "w", this.Font);
            if (unit.Trim(' ').Length == 0) { UnitSize = 0; }
            if (hasUnit == false) {
                UnitSize = 0;
                EndPoint = this.Width - (PaddingUnit / 2);
                return;
            }
            if (isMetric == false) {
                PrefixSize = 0;
                if (UnitSize == 0) { PaddingUnit = PaddingUnit / 2; }
            }
            int ButtonBasisPosition = this.Width - UnitSize - PrefixSize - PaddingUnit;

            bool SecondUnitShow = false;
            bool SecondPrefixShow = false;

            int UnitSize2 = 0;
            int PrefixSize2 = 0;
            int SymbolSize = 0;
            //if (isMetric == true) {
            if (secondaryUnitDisplay != SecondaryUnitDisplayType.NoSecondaryUnit) {
                UnitSize2 = MeasureDisplayStringWidth(e.Graphics, secondaryUnit, this.Font);
                if (secondaryUnit.Trim(' ').Length == 0) { UnitSize2 = 0; }
                else {
                    ButtonBasisPosition -= UnitSize2;
                    SecondUnitShow = true;
                    if (secondaryUnitDisplay == SecondaryUnitDisplayType.Multiply) { SymbolSize = MeasureDisplayStringWidth(e.Graphics, "·", this.Font); }
                    else if (secondaryUnitDisplay == SecondaryUnitDisplayType.Divide) { SymbolSize = MeasureDisplayStringWidth(e.Graphics, "/", this.Font); }
                    ButtonBasisPosition -= SymbolSize;

                    if (isSecondaryMetric) {
                        PrefixSize2 = MeasureDisplayStringWidth(e.Graphics, PrefixStr2, this.Font);
                        ButtonBasisPosition -= PrefixSize2;
                        SecondPrefixShow = true;
                    }
                }
            }
            //}

            EndPoint = ButtonBasisPosition;
            if (isMetric == true) {
                PrefixRectangle = new Rectangle(ButtonBasisPosition, 0, PrefixSize, Height);
                TextRenderer.DrawText(e.Graphics, PrefixStr, this.Font, PrefixRectangle, GetColorMouseAction(PrefixRectangle), Flags);
            }
            if (UnitSize != 0) {
                UnitRectangle = new Rectangle(ButtonBasisPosition + PrefixSize, 0, UnitSize, Height);
                TextRenderer.DrawText(e.Graphics, unit, this.Font, UnitRectangle, GetColorMouseAction(UnitRectangle), Flags);
            }
            if (SecondPrefixShow == true) {
                PrefixRectangle2 = new Rectangle(ButtonBasisPosition + PrefixSize + UnitSize + SymbolSize, 0, PrefixSize2, Height);
                TextRenderer.DrawText(e.Graphics, PrefixStr2, this.Font, PrefixRectangle2, GetColorMouseAction(PrefixRectangle2), Flags);
            }
            if (SecondUnitShow == true) {
                UnitRectangle2 = new Rectangle(ButtonBasisPosition + PrefixSize + UnitSize + SymbolSize + PrefixSize2, 0, UnitSize2, Height);
                Rectangle SumbolRectangle = new Rectangle(ButtonBasisPosition + PrefixSize + UnitSize, 0, SymbolSize, Height);
                if (secondaryUnitDisplay == SecondaryUnitDisplayType.Multiply) {
                    TextRenderer.DrawText(e.Graphics, "·", this.Font, SumbolRectangle, ForeColor, Flags);
                }
                else if (secondaryUnitDisplay == SecondaryUnitDisplayType.Divide) {
                    TextRenderer.DrawText(e.Graphics, "/", this.Font, SumbolRectangle, ForeColor, Flags);
                }
                TextRenderer.DrawText(e.Graphics, secondaryUnit, this.Font, UnitRectangle2, GetColorMouseAction(UnitRectangle2), Flags);
            }
        }
        private void DrawButtons(PaintEventArgs e) {
            if (hasUnit == true) {
                TextFormatFlags Flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.PreserveGraphicsClipping;
                System.Drawing.Size BasisSize = new Size(int.MaxValue, int.MaxValue);
                //System.Drawing.Size UnitSize = MinimumSizing(TextRenderer.MeasureText(e.Graphics, unit, this.Font, BasisSize, Flags).Width);
                string Symbol = unit;
                if (isMetric == true) {
                    Symbol = PrefixToSymbol(Prefix) + unit;
                }
                int UnitSize = MinimumSizing(MeasureDisplayStringWidth(e.Graphics, Symbol, this.Font), e);
                int ButtonBasisPosition = this.Width - UnitSize;
                EndPoint = ButtonBasisPosition;
                //if (isMetric == true) {
                //    string PrefixSymbol = ConversionHandler.PrefixToSymbol(Prefix);
                //    int PrefixSize = MeasureDisplayStringWidth(e.Graphics, "aa", this.Font);
                //    //System.Drawing.Size PrefixSize = MinimumSizing(TextRenderer.MeasureText(e.Graphics, PrefixSymbol, this.Font, BasisSize, Flags).Width);
                //    Rectangle PrefixRect = new Rectangle(new Point(ButtonBasisPosition - PrefixSize, 0), new Size(PrefixSize, Height));
                //    EndPoint -= PrefixSize;
                //    DrawButton(e, PrefixRect, PrefixSymbol, TextFormatFlags.Right | Flags);
                //}
                DrawButton(e, new Rectangle(ButtonBasisPosition, 0, UnitSize, this.Height), Symbol, TextFormatFlags.Left | Flags);
            }
        }
        private int MinimumSizing(int Width, PaintEventArgs e) {
            int WideWidth = MeasureDisplayStringWidth(e.Graphics, ".WW.", Font);//TextRenderer.MeasureText("-", this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            if (Width < WideWidth) {
                return WideWidth;
            }
            else {
                return Width;
            }
        }
        private void DrawFixtures(PaintEventArgs e) {
            DrawBorder(e, Bounding, borderColor, 1);
        }
        private void DrawLabel(PaintEventArgs e) {
            int h = (int)(e.Graphics.MeasureString("W", labelFont).Width / 2.0f) + Padding.Top;
            using (SolidBrush br = new SolidBrush(labelForeColor)) {
                e.Graphics.DrawString(labelText, labelFont, br, new PointF(h, 0));
            }
        }
        private void DrawBorder(PaintEventArgs e, Rectangle Bound, Color BorderingColour, int PenSizing = 1) {
            if (PenSizing > 0) {
                using (Brush Brs = new SolidBrush(BorderingColour)) {
                    using (Pen Pn = new Pen(Brs, PenSizing)) {
                        if (PenSizing == 1) {
                            Rectangle Rect = new Rectangle(Bound.X, Bound.Y, Bound.Width - 1, Bound.Height - 1);
                            e.Graphics.DrawRectangle(Pn, Rect);
                        }
                        else {
                            Pn.Alignment = PenAlignment.Inset;
                            e.Graphics.DrawRectangle(Pn, Bound);
                        }
                    }
                }
            }
        }
        //private void DrawButton(PaintEventArgs e, )
        private string GetSuperScriptText(int Exponent) {
            string ExponentString = Exponent.ToString();
            string Temp = "";
            for (int i = 0; i < ExponentString.Length; i++) {
                char CurrentChar = ExponentString[i];
                switch (CurrentChar) {
                    case '-':
                        Temp += '\u207B'; break;
                    case '1':
                        Temp += '\u00B9'; break;
                    case '2':
                        Temp += '\u00B2'; break;
                    case '3':
                        Temp += '\u00B3'; break;
                    default:
                        if (Char.IsDigit(CurrentChar)) {
                            int Index = (CurrentChar - 0x30) + 0x2070;
                            Temp += (char)Index;
                        }
                        break;
                }
            }
            return Temp;
        }
        private void RenderValueText(PaintEventArgs e, Color Backcoloring) {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (UserEntered == false) {
                string ValueFormat = DecimalTrim(ValueStringFormatting(ValueString));
                if (Exponent != 0) {
                    ValueFormat += " · 10" + GetSuperScriptText(Exponent);
                    //using (Font ExponentFont = new Font(this.Font.FontFamily, this.Font.Size / 2)) {
                    //   
                    //    int YPosition = (Height / 2) - (int)e.Graphics.MeasureString("100", ExponentFont).Height;
                    //    int XPosition = (int)(e.Graphics.MeasureString(Exponent.ToString(), ExponentFont).Width * 1.05f);
                    //    int XPad = (int)(e.Graphics.MeasureString(Exponent.ToString(), ExponentFont).Width * 0.8f);
                    //    using (SolidBrush ExponentBrush = new SolidBrush(ForeColor)) {
                    //        e.Graphics.DrawString(Exponent.ToString(), ExponentFont, ExponentBrush, new Point(EndPoint - XPosition, YPosition));
                    //    }
                    //    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint - XPad, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                    //}
                    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
                else {
                    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
            }
            else {
                string ValueFormat = ValueStringFormatting(ValueString);
                if (Exponent != 0) {
                    ValueFormat += " · 10" + GetSuperScriptText(Exponent);
                    //using (Font ExponentFont = new Font(this.Font.FontFamily, this.Font.Size / 2)) {
                    //    int YPosition = (Height / 2) - (int)e.Graphics.MeasureString("100", ExponentFont).Height;
                    //    int XPosition = (int)(e.Graphics.MeasureString(Exponent.ToString(), ExponentFont).Width * 1.05f);
                    //    int XPad = (int)(e.Graphics.MeasureString(Exponent.ToString(), ExponentFont).Width * 0.8f);
                    //    using (SolidBrush ExponentBrush = new SolidBrush(ForeColor)) {
                    //        e.Graphics.DrawString(Exponent.ToString(), ExponentFont, ExponentBrush, new Point(EndPoint - XPosition, YPosition));
                    //    }
                    //    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint - XPad, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                    //}
                    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
                else {
                    TextRenderer.DrawText(e.Graphics, ValueFormat, this.Font, new Rectangle(0, 0, EndPoint, Height), ForeColor, Backcoloring, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
            }
        }
        int Exponent = 0;
        string DecimalTrim(string Input) {
            bool HasDecimalPoint = Input.Contains('.');
            if (HasDecimalPoint == false) {
                return Input;
            }
            else {
                string LeftData = Input.Split('.')[0];
                string RightData = Input.Split('.')[1];
                if (maxmiumDecimalPlaces > 0) {
                    string Temp = RightData;
                    if (RightData.Length > maxmiumDecimalPlaces) {
                        Temp = RightData.Substring(0, (int)maxmiumDecimalPlaces);
                    }
                    while (Temp.EndsWith("0") == true) {
                        Temp = Temp.Remove(Temp.Length - 1, 1);
                    }
                    if (Temp.Length == 0) {
                        return LeftData;
                    }
                    return LeftData + "." + Temp;
                }
                else { return LeftData; }
            }
        }
        string ValueStringFormatting(string Input) {
            if (numericalFormat == NumberFormat.Decimal) { Exponent = 0; return Input; }
            else {
                if (Input.Length == 0) { return Input; }
                else {
                    bool HasDecimalPoint = Input.Contains('.');
                    if (HasDecimalPoint == false) {
                        string LeftData = Input.Split('.')[0].Replace("-", "");
                        if (LeftData.Length >= numericalLeftRadixDigitsMaximum) {
                            string Output = "";
                            for (int i = 0; i < LeftData.Length; i++) {
                                Output += LeftData[i];
                                if (i == 0) { Output += "."; }
                            }
                            Exponent = LeftData.Length - 1;
                            if (Output.EndsWith('.')) { Output.Remove(Output.Length - 1, 1); }
                            if (Output.Length == 0) { Output = "0"; }
                            if (Input.StartsWith('-')) { Output = "-" + Output; }
                            return Output;
                        }
                        else { Exponent = 0; return Input; }
                    }
                    else {
                        string LeftData = Input.Split('.')[0].Replace("-", "");
                        string RightData = Input.Split('.')[1];
                        bool IsZero = true;
                        //for (int i = 0; i < LeftData.Length; i++) {
                        //    if (LeftData[i] != '0') { IsZero = false; }
                        //}
                        if (LeftData == "0") { IsZero = true; }
                        else { IsZero = false; }
                        if (IsZero == false) {
                            if (LeftData.Length >= numericalLeftRadixDigitsMaximum) {
                                string Output = "";
                                for (int i = 0; i < LeftData.Length; i++) {
                                    Output += LeftData[i];
                                    if (i == 0) { Output += "."; }
                                }
                                for (int i = 0; i < RightData.Length; i++) {
                                    Output += RightData[i];
                                }
                                Exponent = LeftData.Length - 1;

                                if (Output.EndsWith('.')) { Output = Output.Remove(Output.Length - 1, 1); }
                                if (Output.Length == 0) { Output = "0"; }
                                if (Input.StartsWith('-')) { Output = "-" + Output; }
                                return Output;
                            }
                            else { Exponent = 0; return Input; }
                        }
                        else {
                            int NonZeroCount = 0;
                            for (int i = 0; i < RightData.Length; i++) {
                                if (RightData[i] != '0') { break; }
                                else { NonZeroCount++; }
                            }
                            if (NonZeroCount >= numericalRightRadixDigitsMaximum) {
                                string Output = "";
                                bool FirstNonZero = false;
                                bool FirstPoint = false;
                                int Zero = 0;
                                for (int i = 0; i < RightData.Length; i++) {

                                    if (FirstNonZero == false) {
                                        if (RightData[i] != '0') {
                                            FirstNonZero = true;
                                            FirstPoint = true;
                                            Zero = i + 1;
                                        }
                                    }
                                    if (FirstNonZero == true) { Output += RightData[i]; }
                                    if (FirstPoint == true) { Output += "."; FirstPoint = false; }
                                }
                                Exponent = (-1) * Zero;

                                if (Output.EndsWith('.')) {
                                    Output = Output.Remove(Output.Length - 1, 1);
                                }
                                if (Output.Length == 0) { Output = "0"; }
                                if (Input.StartsWith('-')) { Output = "-" + Output; }
                                return Output;
                            }
                            else { Exponent = 0; return Input; }
                        }
                    }


                }
            }
        }
        string ValueStringFormatted = "";
        private void DecimalTrim() {
            //if (formatOutput == true) {
            if (ValueString.Contains(".")) {
                string Output = "";
                bool StartTrimming = false;
                int DigitsAfter = 0;
                char lastchar = (char)0x00;
                int Occurances = 0;
                for (int i = 0; i < ValueString.Length; i++) {
                    if (StartTrimming == true) {
                        DigitsAfter++;
                        if (ValueString[i] == lastchar) {
                            if (Occurances >= 3) {
                                break;
                            }
                            else {
                                Occurances++;
                            }
                        }
                        else {
                            Occurances = 0;
                        }
                        lastchar = ValueString[i];
                    }
                    if (ValueString[i] == '.') { StartTrimming = true; }
                    Output += ValueString[i];
                }
                ValueStringFormatted = Output;
            }
            else {
                ValueStringFormatted = ValueString;
            }
            //}
        }
        private string TextFormatter() {
            int DigitsBeforePoint = 0;
            int DigitsAfterPoint = 0;
            for (int i = 0; i < ValueString.Length; i++) {
                if (ValueString[i] == '.') { break; }
                // else if (ValueString[i] == '-') { break; }
                else { DigitsBeforePoint++; }
            }
            if (ValueString.Contains(".")) {
                //00.000 = 6, 2b, 3a
                DigitsAfterPoint = ValueString.Length - 1 - DigitsBeforePoint;
            }
            else { DigitsAfterPoint = 0; }

            if (DigitsBeforePoint >= 7) {
                //1000000
                return ValueString;
            }
            else {
                string Output = "";
                bool StartTrimming = false;
                int DigitsAfter = 0;
                for (int i = 0; i < ValueString.Length; i++) {
                    if (StartTrimming == true) {
                        DigitsAfter++;
                        if (DigitsAfter >= 5) { break; }
                    }
                    if (ValueString[i] == '.') { StartTrimming = true; }
                    Output += ValueString[i];
                }
                return Output;
            }
        }
        private void DrawButton(PaintEventArgs e, Rectangle Bound, string Text, TextFormatFlags Format) {
            if (Bound.Contains(CursorLocation)) {
                if (IsMouseDown == false) {
                    using (Brush HoverBrush = new SolidBrush(hovercolor)) {
                        e.Graphics.FillRectangle(HoverBrush, Bound);
                    }
                }
                else {
                    using (Brush HoverBrush = new SolidBrush(buttondowncolor)) {
                        e.Graphics.FillRectangle(HoverBrush, Bound);
                    }
                }
                DrawBorder(e, Bound, borderColor);
            }

            TextRenderer.DrawText(e.Graphics, Text, this.Font, Bound, this.ForeColor, Format);
        }
        private ButtonState GetBoundState(Rectangle Bound) {
            if (Bound.Contains(CursorLocation)) {
                if (IsMouseDown == false) {
                    return ButtonState.MouseOver;
                }
                else {
                    return ButtonState.MouseDown;
                }
            }
            return ButtonState.Normal;
        }
        private Color GetColorMouseAction(Rectangle Bound) {
            switch (GetBoundState(Bound)) {
                case ButtonState.MouseOver:
                    return ButtonHoverColor;
                case ButtonState.MouseDown:
                    return buttondowncolor;
                case ButtonState.Normal:
                    return ForeColor;
            }
            return ForeColor;
        }
        #endregion
        #region Event Handling
        Point CursorLocation = new Point(0, 0);
        bool IsMouseDown = false;
        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            if (hasUnit == true) {
                if (GetBoundState(UnitRectangle) == ButtonState.MouseDown) {
                    Point HitLocation = new Point(UnitRectangle.X, UnitRectangle.Y);
                    UnitClicked?.Invoke(this, new UnitClickedEventArgs(HitLocation, Cursor.Position, ValueString, Prefix, Unit));
                }
                else if (GetBoundState(PrefixRectangle) == ButtonState.MouseDown) {
                    Point HitLocation = new Point(PrefixRectangle.X, PrefixRectangle.Y);
                    PrefixClicked?.Invoke(this, new PrefixClickedEventArgs(HitLocation, Cursor.Position, ValueString, Prefix, Unit));
                }
                else {
                    if (secondaryUnitDisplay != SecondaryUnitDisplayType.NoSecondaryUnit) {
                        if (GetBoundState(UnitRectangle2) == ButtonState.MouseDown) {
                            Point HitLocation = new Point(UnitRectangle2.X, UnitRectangle2.Y);
                            UnitClicked?.Invoke(this, new UnitClickedEventArgs(HitLocation, Cursor.Position, ValueString, Prefix, Unit, true));
                        }
                        else if ((GetBoundState(PrefixRectangle2) == ButtonState.MouseDown) && (isSecondaryMetric == true)) {
                            Point HitLocation = new Point(PrefixRectangle2.X, PrefixRectangle2.Y);
                            PrefixClicked?.Invoke(this, new PrefixClickedEventArgs(HitLocation, Cursor.Position, ValueString, Prefix, Unit, true));
                        }
                    }
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            IsMouseDown = true;
            Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            CursorLocation = new Point(-1, -1);
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseHover(EventArgs e) {
            base.OnMouseHover(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            ChangeStep = 0;
            IsMouseDown = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if (IsMouseDown == true) {
                if (allowDragValueChange == true) {
                    int AbsoluteY = 0;
                    int CurentHeight = Height > 0 ? Height : 1;
                    if (e.Location.Y < Height / 2) {
                        if (e.Location.Y < 0) {
                            AbsoluteY = Math.Abs(e.Location.Y) + (Height / 2);
                        }
                        else {
                            AbsoluteY = (Height / 2) - e.Location.Y;
                        }
                        StepReversed = true;
                    }
                    else if (e.Location.Y >= Height / 2) {
                        AbsoluteY = e.Location.Y - (Height / 2);
                        StepReversed = false;
                    }
                    ChangeStep = (int)Math.Floor((float)AbsoluteY / ((float)CurentHeight));
                    if (ChangeStep > 0) {
                        if (StepReversed == false) {
                            for (int i = 0; i < ChangeStep; i++) {
                                TickBackward();
                            }
                        }
                        else {
                            for (int i = 0; i < ChangeStep; i++) {
                                TickForward();
                            }
                        }
                        Invalidate();
                    }
                }
            }
            CursorLocation = e.Location;
            Invalidate();
            base.OnMouseMove(e);
        }
        protected override void OnLostFocus(EventArgs e) {
            Invalidate();
            base.OnLostFocus(e);
        }
        protected override void OnGotFocus(EventArgs e) {
            Invalidate();
            base.OnGotFocus(e);
        }
        private int ChangeStep = 0;
        private bool StepReversed = false;
        protected override void OnKeyDown(KeyEventArgs e) {
            // e.Handled = true;
            if (ProcessUnit(e) == false) {
                if (allowTyping == true) {
                    ProcessNumerical(e);
                }
            }
            else {
                ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
            }
            Invalidate();
            e.SuppressKeyPress = true;
            base.OnKeyDown(e);
        }
        private void SetPrefix(MetricPrefix CurrentPrefix, bool IsSecondary) {
            if (IsSecondary == true) { SecondaryPrefix = CurrentPrefix; }
            else { Prefix = CurrentPrefix; }
        }
        private bool ProcessUnit(KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.C) {
                if (allowClipboard == false) { return false; }
                Copy(); return false;
            }
            else if (e.Control && e.KeyCode == Keys.D) {
                if (allowClipboard == false) { return false; }
                CopyWithUnit(); return false;
            }
            else if (e.Control && e.KeyCode == Keys.V) {
                if (allowClipboard == false) { return false; }
                PasteNumber(Clipboard.GetText()); return false;
            }
            else {
                if (allowTyping == false) { return false; }
                if (isMetric == false) { return false; }
                if (numberbase != NumberBase.Base10) { return false; }
                bool SecondaryShift = (Control.ModifierKeys & Keys.Alt) == Keys.Alt ? true : false;
                bool CaseShift = Control.IsKeyLocked(Keys.CapsLock);
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
                    if (CaseShift == true) {
                        CaseShift = false;
                    }
                    else {
                        CaseShift = true;
                    }
                }
                switch (e.KeyCode) {
                    case Keys.Q:
                        if (CaseShift) { SetPrefix(MetricPrefix.Quetta, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Quecto, SecondaryShift); }
                        return true;
                    case Keys.R:
                        if (CaseShift) { SetPrefix(MetricPrefix.Ronna, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Ronto, SecondaryShift); }
                        return true;
                    case Keys.A:
                        SetPrefix(MetricPrefix.Atto, SecondaryShift);
                        return true;
                    case Keys.E:
                        SetPrefix(MetricPrefix.Exa, SecondaryShift);
                        return true;
                    case Keys.T:
                        SetPrefix(MetricPrefix.Tera, SecondaryShift);
                        return true;
                    case Keys.G:
                        SetPrefix(MetricPrefix.Giga, SecondaryShift);
                        return true;
                    case Keys.K:
                        SetPrefix(MetricPrefix.Kilo, SecondaryShift);
                        return true;
                    case Keys.H:
                        SetPrefix(MetricPrefix.Hecto, SecondaryShift);
                        return true;
                    case Keys.D:
                        SetPrefix(MetricPrefix.Deci, SecondaryShift);
                        return true;
                    case Keys.C:
                        SetPrefix(MetricPrefix.Centi, SecondaryShift);
                        return true;
                    case Keys.N:
                        SetPrefix(MetricPrefix.Nano, SecondaryShift);
                        return true;
                    case Keys.F:
                        SetPrefix(MetricPrefix.Femto, SecondaryShift);
                        return true;
                    case Keys.U:
                        SetPrefix(MetricPrefix.Micro, SecondaryShift);
                        return true;
                    case Keys.Space:
                        SetPrefix(MetricPrefix.None, SecondaryShift);
                        return true;
                    case Keys.Y:
                        if (CaseShift) { SetPrefix(MetricPrefix.Yotta, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Yocto, SecondaryShift); }
                        return true;
                    case Keys.Z:
                        if (CaseShift) { SetPrefix(MetricPrefix.Zetta, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Zepto, SecondaryShift); }
                        return true;
                    case Keys.M:
                        if (CaseShift) { SetPrefix(MetricPrefix.Mega, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Milli, SecondaryShift); }
                        return true;
                    case Keys.P:
                        if (CaseShift) { SetPrefix(MetricPrefix.Peta, SecondaryShift); }
                        else { SetPrefix(MetricPrefix.Pico, SecondaryShift); }
                        return true;
                    default:
                        return false;
                }
            }
        }
        private void ProcessNumerical(KeyEventArgs e) {
            char DecSep = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (ValueString.Length < 20) {
                if (allowNumberEntry == false) { return; }
                if ((e.KeyCode == Keys.NumPad0) || (e.KeyCode == Keys.D0)) {
                    if (ValueString.Length >= 1) {
                        ValueString += "0"; UserEntered = true;
                    }
                    else { ValueString = "0"; UserEntered = true; }
                }
                else if ((e.KeyCode == Keys.NumPad1) || (e.KeyCode == Keys.D1)) { ValueString += "1"; UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad2) || (e.KeyCode == Keys.D2)) { if (numberbase > NumberBase.Base2) { ValueString += "2"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad3) || (e.KeyCode == Keys.D3)) { if (numberbase > NumberBase.Base2) { ValueString += "3"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad4) || (e.KeyCode == Keys.D4)) { if (numberbase > NumberBase.Base2) { ValueString += "4"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad5) || (e.KeyCode == Keys.D5)) { if (numberbase > NumberBase.Base2) { ValueString += "5"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad6) || (e.KeyCode == Keys.D6)) { if (numberbase > NumberBase.Base2) { ValueString += "6"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad7) || (e.KeyCode == Keys.D7)) { if (numberbase > NumberBase.Base2) { ValueString += "7"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad8) || (e.KeyCode == Keys.D8)) { if (numberbase > NumberBase.Base8) { ValueString += "8"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.NumPad9) || (e.KeyCode == Keys.D9)) { if (numberbase > NumberBase.Base8) { ValueString += "9"; } UserEntered = true; }
                else if ((e.KeyCode == Keys.OemPeriod) || (e.KeyCode == Keys.Decimal)) {
                    if (allowFractions) {
                        ValueString = ValueString.Replace(DecSep.ToString(), "");
                        ValueString += DecSep.ToString();
                    }
                }
                else {
                    if (numberbase == NumberBase.Base16) {
                        if (e.KeyCode == Keys.A) { ValueString += "A"; }
                        else if (e.KeyCode == Keys.B) { ValueString += "B"; }
                        else if (e.KeyCode == Keys.C) { ValueString += "C"; }
                        else if (e.KeyCode == Keys.D) { ValueString += "D"; }
                        else if (e.KeyCode == Keys.E) { ValueString += "E"; }
                        else if (e.KeyCode == Keys.F) { ValueString += "F"; }
                    }
                }
            }
            if (e.KeyCode == Keys.Delete) {
                if (allowNumberEntry == false) { return; }
                ValueString = "0"; UserEntered = true;
            }
            else if ((e.KeyCode == Keys.OemMinus) || (e.KeyCode == Keys.Subtract)) {
                if (allowNegatives) {
                    if (ValueString.Contains("-")) { ValueString = ValueString.Replace("-", ""); }
                    else {
                        if (ContainsNonZero(ValueString)) {
                            ValueString = "-" + ValueString;
                        }
                    }
                }
                UserEntered = true;
            }
            else if (e.KeyCode == Keys.Back) {
                if (ValueString.Length > 0) {
                    ValueString = ValueString.Remove(ValueString.Length - 1, 1);
                    if (ContainsNonZero(ValueString) == false) {
                        ValueString = ValueString.Replace("-", "");
                    }
                    if (ValueString.Length == 0) { ValueString = "0"; }
                }
                else { ValueString = "0"; }
                UserEntered = true;
            }
            if ((ValueString.StartsWith("0")) && (ValueString.Length > 1)) {
                ValueString = ValueString.TrimStart('0');
            }
            if (ValueString.StartsWith(DecSep.ToString())) {
                ValueString = "0" + ValueString;
            }
            else if (ValueString.StartsWith("-" + DecSep.ToString())) {
                ValueString = "-0" + ValueString.Remove(0, 1);
            }
            if (ValueString.Length == 0) { ValueString = "0"; }
            CorrectValueToRange(ValueString, out ValueString);
            DecimalTrim();
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (allowTyping == false) {
                if (keyData == Keys.Control) {
                    return true;
                }
                else if (keyData == Keys.Shift) {
                    return true;
                }
                else if (keyData == Keys.Alt) {
                    return true;
                }
                else {
                    return base.ProcessCmdKey(ref msg, keyData);
                }
            }
            if (keyData == (Keys.Up | Keys.Control)) {
                ChangeUnitPrefix(true);
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Down | Keys.Control)) {
                ChangeUnitPrefix(false);
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Up | Keys.Control | Keys.Alt)) {
                ChangeSecondUnitPrefix(true);
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Down | Keys.Control | Keys.Alt)) {
                ChangeSecondUnitPrefix(false);
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Left | Keys.Control)) {
                ChangeOrder(false);
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Right | Keys.Control)) {
                ChangeOrder(true);
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Up) {
                TickForward();
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Down) {
                TickBackward();
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Control) {
                return true;
            }
            else if (keyData == Keys.Shift) {
                return true;
            }
            else if (keyData == Keys.Alt) {
                return true;
            }
            else {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (allowMouseWheel == true) {
                Focus();
                int abDelta = (int)(((float)Math.Abs(e.Delta)) / 120.0f);
                if (e.Delta > 0) {
                    for (int i = 0; i < abDelta; i++) {
                        ForwardAction();
                    }
                }
                else {
                    for (int i = 0; i < abDelta; i++) {
                        BackwardAction();
                    }
                }
                Invalidate();
            }
        }
        protected override void OnKeyUp(KeyEventArgs e) {

        }
        private enum ButtonState {
            Normal = 0x00,
            MouseOver = 0x01,
            MouseDown = 0x02
        }
        #endregion
        #region Numerical Control
        private void Base16TickChange(bool IsForward) {
            string BufferValueString = ValueString;
            bool IsNeg = IsNegative(BufferValueString);
            BufferValueString = BufferValueString.Replace("-", "");
            StringBuilder sb = new StringBuilder(BufferValueString);
            string Attachment = "";
            bool HasOverflow = false;
            bool IsNegTemp = IsNeg;
            if (IsForward == false) {
                IsNegTemp = !IsNegTemp;
                if (allowNegatives == false) {
                    if ((BufferValueString.Length == 1) && (BufferValueString == "0")) {
                        return;
                    }
                }

            }
            for (int i = BufferValueString.Length - 1; i > -1; i--) {
                HasOverflow = false;
                if ((BufferValueString[i] != '.') && (BufferValueString[i] != ',')) {
                    if (IsNegTemp == true) {
                        char Current = BufferValueString[i];
                        if (Current == '0') {
                            HasOverflow = true;
                            sb[i] = 'F';
                        }
                        else {
                            int bar = int.Parse(BufferValueString[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                            bar -= 1;
                            sb[i] = (bar.ToString("X").ToCharArray())[0];
                        }
                    }
                    else {
                        char Current = BufferValueString[i];
                        if (Current == 'F') {
                            HasOverflow = true;
                            sb[i] = '0';
                            if (i == 0) { Attachment = "1"; }
                        }
                        else {
                            int bar = int.Parse(BufferValueString[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                            bar += 1;
                            sb[i] = (bar.ToString("X").ToCharArray())[0];
                        }
                    }
                    if (HasOverflow == false) { break; }
                }
            }
            PerformCleanup(Attachment + sb.ToString(), IsNeg);
        }
        private void Base10TickChange(bool IsForward, char TickOverChar = '9') {
            if (IsForward == true) {
                if (ValueString.EndsWith(Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).ToString())) {
                    ValueString += "1";
                }
                else {
                    string BufferValueString = ValueString;
                    bool IsNeg = IsNegative(BufferValueString);
                    BufferValueString = BufferValueString.Replace("-", "");
                    StringBuilder sb = new StringBuilder(BufferValueString);
                    string Attachment = "";
                    bool HasOverflow = false;
                    for (int i = BufferValueString.Length - 1; i > -1; i--) {
                        HasOverflow = false;
                        if ((BufferValueString[i] != '.') && (BufferValueString[i] != ',')) {
                            if (IsNeg == true) {
                                char Current = BufferValueString[i];
                                if (Current == '0') {
                                    HasOverflow = true;
                                    sb[i] = TickOverChar;
                                }
                                else {
                                    char SelectedChar = BufferValueString[i];
                                    int bar = SelectedChar - '0';
                                    bar -= 1;
                                    sb[i] = (bar.ToString().ToCharArray())[0];
                                }
                            }
                            else {
                                char Current = BufferValueString[i];
                                if (Current == TickOverChar) {
                                    HasOverflow = true;
                                    sb[i] = '0';
                                    if (i == 0) { Attachment = "1"; }
                                }
                                else {
                                    char SelectedChar = BufferValueString[i];
                                    int bar = SelectedChar - '0';
                                    bar += 1;
                                    sb[i] = (bar.ToString().ToCharArray())[0];
                                }
                            }
                            if (HasOverflow == false) { break; }
                        }
                    }
                    PerformCleanup(Attachment + sb.ToString(), IsNeg);
                }
            }
            else {
                if (ValueString.EndsWith(Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).ToString())) {
                    ValueString += TickOverChar;
                }
                else {
                    string BufferValueString = ValueString;
                    bool IsNeg = IsNegative(BufferValueString);
                    BufferValueString = BufferValueString.Replace("-", "");
                    StringBuilder sb = new StringBuilder(BufferValueString);
                    string Attachment = "";
                    bool HasOverflow = false;
                    bool IgnoreFormat = false;
                    if (IsZero(BufferValueString)) {
                        IsNeg = true;
                        IgnoreFormat = !allowNegatives;
                    }
                    if (IgnoreFormat == false) {
                        for (int i = BufferValueString.Length - 1; i > -1; i--) {
                            if ((BufferValueString[i] != '.') && (BufferValueString[i] != ',')) {
                                HasOverflow = false;
                                if (IsNeg == true) {
                                    char Current = BufferValueString[i];
                                    if (Current == TickOverChar) {
                                        HasOverflow = true;
                                        sb[i] = '0';
                                        if (i == 0) { Attachment = "1"; }
                                    }
                                    else {
                                        char SelectedChar = BufferValueString[i];
                                        int bar = SelectedChar - '0';
                                        bar += 1;
                                        sb[i] = (bar.ToString().ToCharArray())[0];
                                    }
                                }
                                else {
                                    char Current = BufferValueString[i];
                                    if (Current == '0') {
                                        HasOverflow = true;
                                        sb[i] = TickOverChar;
                                    }
                                    else {
                                        char SelectedChar = BufferValueString[i];
                                        int bar = SelectedChar - '0';
                                        bar -= 1;
                                        sb[i] = (bar.ToString().ToCharArray())[0];
                                    }
                                }
                                if (HasOverflow == false) { break; }
                            }
                        }
                        PerformCleanup(Attachment + sb.ToString(), IsNeg);
                    }
                    //ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
                }
            }
        }
        private void TickForward() {
            if (allowNumberEntry == false) { return; }
            if (numberbase == NumberBase.Base10) {
                Base10TickChange(true);
            }
            else if (numberbase == NumberBase.Base2) {
                Base10TickChange(true, '1');
            }
            else if (numberbase == NumberBase.Base8) {
                Base10TickChange(true, '7');
            }
            else if (numberbase == NumberBase.Base16) {
                Base16TickChange(true);
            }
        }
        private void TickBackward() {
            if (allowNumberEntry == false) { return; }
            if (numberbase == NumberBase.Base10) {
                Base10TickChange(false);
            }
            else if (numberbase == NumberBase.Base2) {
                Base10TickChange(false, '1');
            }
            else if (numberbase == NumberBase.Base8) {
                Base10TickChange(false, '7');
            }
            else if (numberbase == NumberBase.Base16) {
                Base16TickChange(false);
            }
        }
        private void ChangeOrder(bool IncreasePointPosition) {
            /*Changes the decimal placement
             */
            string Buffer = ValueString;
            char a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            bool ContainsPoint = Buffer.Contains(a.ToString());
            bool ContainsNeg = Buffer.Contains("-");
            Buffer = Buffer.Replace("-", "");
            if (IncreasePointPosition == true) {
                if (ContainsPoint == true) {
                    int OldIndex = GetDecimalIndex(Buffer);
                    if ((OldIndex > -1) && (OldIndex < Buffer.Length - 1)) {
                        Buffer = Buffer.Replace(a.ToString(), "");
                        OldIndex++;
                        string OutBuffer = "";
                        for (int i = 0; i < Buffer.Length; i++) {
                            if (i == OldIndex) { OutBuffer += a; }
                            OutBuffer += Buffer[i];
                        }
                        PerformCleanup(OutBuffer, ContainsNeg);
                    }
                }
            }
            else {
                int OldIndex = GetDecimalIndex(Buffer);
                if (ContainsPoint == false) {
                    OldIndex = Buffer.Length;
                }
                int MinimumIndex = 0;
                if ((OldIndex > -1) && (OldIndex > MinimumIndex)) {
                    Buffer = Buffer.Replace(a.ToString(), "");
                    OldIndex--;
                    string OutBuffer = "";
                    for (int i = 0; i < Buffer.Length; i++) {
                        if (i == OldIndex) { OutBuffer += a; }
                        OutBuffer += Buffer[i];
                    }
                    PerformCleanup(OutBuffer, ContainsNeg);
                }
            }

        }
        private void PerformCleanup(string Input, bool IsNegative) {
            if (allowFractions == false) {
                if (Input.Contains(".")) {
                    Input = Input.Split(".")[0];
                }
            }
            if (allowNegatives == false) {
                if (Input.Contains("-")) {
                    Input = Input.Replace("-", "");
                }
            }
            Input = Input.TrimStart('0');
            if (Input.Length == 0) { Input = "0"; }
            if ((Input.StartsWith(".")) || (Input.StartsWith(","))) { Input = "0" + Input; }
            if ((IsNegative == true) && (allowNegatives == true)) {
                Input = "-" + Input;
                if (IsZero(Input)) { Input = Input.Replace("-", ""); }
            }
            CorrectValueToRange(Input, out Input);
            ValueString = Input;
            DecimalTrim();
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
        }
        private int GetDecimalIndex(string Input) {
            char DecSep = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            for (int i = 0; i < Input.Length; i++) {
                if (Input[i] == DecSep) {
                    return i;
                }
            }
            return -1;
        }
        private void ChangeUnitPrefix(bool Foward) {
            if (isMetric == true) {
                if (Foward == true) {
                    if (prefix < MetricPrefix.Quetta) {
                        Prefix += 1;
                    }
                }
                else {
                    if (prefix > MetricPrefix.Quecto) {
                        Prefix -= 1;
                    }
                }
            }
            else {
                prefix = MetricPrefix.None;
            }
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
        }
        private void ChangeSecondUnitPrefix(bool Foward) {
            if (isMetric == true) {
                if (Foward == true) {
                    if (secondaryPrefix < MetricPrefix.Quetta) {
                        SecondaryPrefix += 1;
                    }
                }
                else {
                    if (secondaryPrefix > MetricPrefix.Quecto) {
                        SecondaryPrefix -= 1;
                    }
                }
            }
            else {
                secondaryPrefix = MetricPrefix.None;
            }
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
        }
        private void ForwardAction() {
            if ((ModifierKeys.HasFlag(Keys.Control)) && (ModifierKeys.HasFlag(Keys.Alt))) {
                ChangeSecondUnitPrefix(true);
            }
            else if (ModifierKeys.HasFlag(Keys.Control)) {
                ChangeUnitPrefix(true);
            }
            else if (ModifierKeys.HasFlag(Keys.Shift)) {

            }
            else {
                TickForward();
            }
        }
        private void BackwardAction() {
            if ((ModifierKeys.HasFlag(Keys.Control)) && (ModifierKeys.HasFlag(Keys.Alt))) {
                ChangeSecondUnitPrefix(false);
            }
            else if (ModifierKeys.HasFlag(Keys.Control)) {
                ChangeUnitPrefix(false);
            }
            else if (ModifierKeys.HasFlag(Keys.Shift)) {

            }
            else {
                TickBackward();
            }
        }
        private void PasteNumber(string InputString) {
            if (allowNumberEntry == false) { return; }
            try {
                char a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string Input = MorePointsRemoval(InputString, a);
                // bool HasMorePoints = MorePoints(Input); //Test whether the copied string has more decimal points instead of commas
                string NumericalPart = "";
                bool IgnoreNumerical = false;
                bool IgnoreSusMinuses = false;
                string PrefixPart = "";
                int HighestNumber = 9;
                if (numberbase <= NumberBase.Base8) {
                    HighestNumber = (int)numberbase - 1;
                }
                for (int i = 0; i < Input.Length; i++) {
                    if (IgnoreNumerical == false) {
                        if (((int)Input[i] - 0x30 >= 0) && ((int)Input[i] - 0x30 <= HighestNumber)) {
                            NumericalPart += Input[i];
                            IgnoreSusMinuses = true;
                        }
                        else if (Input[i] == '+') {
                            //Allows + fronts, therefore: +1234 is valid
                            if (IgnoreSusMinuses == false) {
                                IgnoreSusMinuses = true;
                            }
                            else {
                                IgnoreNumerical = true;
                                //String afterwards is invalid and not a number!
                            }
                        }
                        else if (Input[i] == '-') {
                            if (IgnoreSusMinuses == false) {
                                NumericalPart += Input[i];
                                IgnoreSusMinuses = true;
                            }
                            else {
                                IgnoreNumerical = true;
                                //String afterwards is invalid and not a number!
                            }
                        }
                        else if (Input[i] == a) {
                            NumericalPart += Input[i];
                        }
                        else {
                            if (numberbase == NumberBase.Base16) {
                                if (((int)Input[i] - 0x41 >= 0) && ((int)Input[i] - 0x41 <= 5)) {
                                    NumericalPart += Input[i];
                                    IgnoreSusMinuses = true;
                                }
                                else if (((int)Input[i] - 0x61 >= 0) && ((int)Input[i] - 0x61 <= 5)) {
                                    NumericalPart += Input[i].ToString().ToUpper();
                                    IgnoreSusMinuses = true;
                                }
                                else {
                                    IgnoreNumerical = true; i = i - 1;
                                }
                            }
                            else {
                                IgnoreNumerical = true; i = i - 1;
                            }
                        }
                    }
                    else {
                        if ((Input[i] == ' ') || (Input[i] == (char)0x09)) { }
                        else {
                            if (IsVaildPrefixChar(Input[i]) == true) {
                                if ((PrefixPart.ToLower() == "d") && (Input[i] == 'a')) {
                                    PrefixPart += Input[i];
                                    break;
                                }
                                else {
                                    PrefixPart += Input[i];
                                    if (PrefixPart.ToLower() != "d") {
                                        break;
                                    }
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
                if (NumericalPart.Trim() != "") {
                    ValueString = NumericalPart;
                }
                if (PrefixPart.Trim() != "") {
                    SymbolToPrefix(PrefixPart);
                }
                DecimalTrim();
                if (allowFractions == false) {
                    RemoveFractionals();
                }
                if (allowNegatives == false) {
                    RemoveNegatives();
                }
                ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
            }
            catch {
                //Invalid Data
            }
        }
        private void RemoveNegatives() {
            if (ValueString.Contains("-")) {
                ValueString = ValueString.Replace("-", "");
            }
        }
        private void RemoveFractionals() {
            if (ValueString.Contains(".")) {
                ValueString = ValueString.Split(".")[0];
            }
        }
        #endregion
        #region Clipboard Actions
        public void Paste() {
            PasteNumber(Clipboard.GetText());
        }
        public void Copy() {
            Clipboard.SetText(ValueString.ToString());
        }
        public void CopyWithUnit() {
            string UnitString = "";
            bool HasEnding = false;
            if (isMetric == true) {
                UnitString += PrefixToSymbol(prefix);
                HasEnding = true;
            }
            if (unit.Trim().Length >= 0) {
                if (hasUnit == true) {
                    UnitString += unit;
                    HasEnding = true;
                }
            }
            if (secondaryUnit.Trim().Length > 0) {
                if (secondaryUnitDisplay != SecondaryUnitDisplayType.NoSecondaryUnit) {
                    if (secondaryUnitDisplay == SecondaryUnitDisplayType.Multiply) { UnitString += "·"; }
                    else if (secondaryUnitDisplay == SecondaryUnitDisplayType.Divide) { UnitString += "/"; }
                    if (isSecondaryMetric == true) { UnitString += PrefixToSymbol(secondaryPrefix); }
                    UnitString += secondaryUnit;
                }
            }
            if (HasEnding == true) {
                UnitString = " " + UnitString;
            }
            Clipboard.SetText(ValueString.ToString() + UnitString);
        }
        public void Clear() {
            ValueString = "0"; UserEntered = true;
            DecimalTrim();
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
        }
        public void PushCharacter(char Input) {
            bool entryState = allowNumberEntry;

            KeyEventArgs? e = null;
            switch (Input) {
                case '0':
                    e = new KeyEventArgs(Keys.NumPad0); break;
                case '1':
                    e = new KeyEventArgs(Keys.NumPad1); break;
                case '2':
                    e = new KeyEventArgs(Keys.NumPad2); break;
                case '3':
                    e = new KeyEventArgs(Keys.NumPad3); break;
                case '4':
                    e = new KeyEventArgs(Keys.NumPad4); break;
                case '5':
                    e = new KeyEventArgs(Keys.NumPad5); break;
                case '6':
                    e = new KeyEventArgs(Keys.NumPad6); break;
                case '7':
                    e = new KeyEventArgs(Keys.NumPad7); break;
                case '8':
                    e = new KeyEventArgs(Keys.NumPad8); break;
                case '9':
                    e = new KeyEventArgs(Keys.NumPad9); break;
                case 'A':
                    e = new KeyEventArgs(Keys.A); break;
                case 'B':
                    e = new KeyEventArgs(Keys.B); break;
                case 'C':
                    e = new KeyEventArgs(Keys.C); break;
                case 'D':
                    e = new KeyEventArgs(Keys.D); break;
                case 'E':
                    e = new KeyEventArgs(Keys.E); break;
                case 'R':
                    e = new KeyEventArgs(Keys.F); break;
                case '.':
                    e = new KeyEventArgs(Keys.OemPeriod); break;
                case '-':
                    e = new KeyEventArgs(Keys.OemMinus); break;
                default:
                    break;
            }
            if (e != null) {
                allowNumberEntry = true;
                UserEntered = true;
                ProcessNumerical(e);
                allowNumberEntry = entryState;
                ValueChanged?.Invoke(this, new ValueChangedEventArgs(ValueString, Prefix));
                Invalidate();
            }
        }
        #endregion
        #region Text Testing
        private string MorePointsRemoval(string Input, char DecimalSeperator) {
            int Commas = 0;
            int Points = 0;
            for (int i = 0; i < Input.Length; i++) {
                if (Input[i] == ',') {
                    Commas++;
                }
                else if (Input[i] == '.') {
                    Points++;
                }
            }
            if (Commas < Points) {
                //Has to be where ',' is the decimal delimiter
                if ((Commas == 0) && (Points == 1)) {
                    string TempStr = Input.Replace(".", DecimalSeperator.ToString());
                    return TempStr;
                }
                else {
                    string TempStr = Input.Replace(".", "");
                    TempStr = TempStr.Replace(",", DecimalSeperator.ToString());
                    return TempStr;
                }
            }
            else if (Commas > Points) {
                //Has to be where ',' is the decimal delimiter
                if ((Commas == 1) && (Points == 0)) {
                    string TempStr = Input.Replace(",", DecimalSeperator.ToString());
                    return TempStr;
                }
                else {
                    string TempStr = Input.Replace(",", "");
                    TempStr = TempStr.Replace(".", DecimalSeperator.ToString());
                    return TempStr;
                }
            }
            else {
                char GroupSep = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
                string TempStr = Input.Replace(GroupSep.ToString(), "");
                // TempStr = TempStr.Replace(".", DecimalSeperator.ToString());
                return TempStr;
            }
        }
        private bool IsNumeric(string value) {
            int dec_cnt = 0;
            string trimmed = value.Trim(' ');
            bool res = false;
            for (int i = 0; i < trimmed.Length; i++) {
                if (((int)trimmed[i] - 0x30 >= 0) && ((int)trimmed[i] - 0x30 <= 9)) {
                    res = true;
                }
                else {
                    if (((trimmed[i] == '-') || (trimmed[i] == '−')) && (i == 0)) {
                        res = true;
                    }
                    else if (trimmed[i] == '.') {
                        dec_cnt++;
                        if (dec_cnt > 1) {
                            res = false;
                            break;
                        }
                    }
                    else {
                        res = false;
                        break;
                    }
                }
            }
            return res;
        }
        bool IsZero(string str) {
            str = str.Replace(".", "");
            str = str.Replace(",", "");
            str = str.Replace("-", "");
            bool Output = true;
            foreach (char c in str) {
                if ((c >= '1') && (c <= '9')) {
                    Output = false;
                }
            }
            return Output;
        }
        private bool IsNegative(string Input) {
            if (Input.StartsWith("-")) {
                return true;
            }
            else {
                return false;
            }
        }
        private bool ContainsNonZero(string Input) {
            if ((Input.Contains("1")) || (Input.Contains("2")) || (Input.Contains("3")) || (Input.Contains("4")) ||
                (Input.Contains("5")) || (Input.Contains("6")) || (Input.Contains("7")) || (Input.Contains("8")) || (Input.Contains("9"))) {
                return true;
            }
            else {
                return false;
            }
        }
        #endregion
        #region Unit Handling
        private string PrefixToSymbol(MetricPrefix prefix) {
            switch (prefix) {
                case MetricPrefix.Quetta: return "Q";       //
                case MetricPrefix.Ronna: return "R";        //
                case MetricPrefix.Yotta: return "Y";//
                case MetricPrefix.Zetta: return "Z";//
                case MetricPrefix.Exa: return "E";
                case MetricPrefix.Peta: return "P";//
                case MetricPrefix.Tera: return "T";
                case MetricPrefix.Giga: return "G";
                case MetricPrefix.Mega: return "M";//
                case MetricPrefix.Kilo: return "k";
                case MetricPrefix.Hecto: return "h";
                case MetricPrefix.Deca: return "da";
                case MetricPrefix.None: return "";
                case MetricPrefix.Deci: return "d";
                case MetricPrefix.Centi: return "c";
                case MetricPrefix.Milli: return "m";//
                case MetricPrefix.Micro: return "μ";
                case MetricPrefix.Nano: return "n";
                case MetricPrefix.Pico: return "p";//
                case MetricPrefix.Femto: return "f";
                case MetricPrefix.Atto: return "a";//
                case MetricPrefix.Zepto: return "z";//
                case MetricPrefix.Yocto: return "y";//
                case MetricPrefix.Ronto: return "r";        //
                case MetricPrefix.Quecto: return "q";       //
                default: return "";
            }
        }
        private void SymbolToPrefix(string Input) {
            if (Input.Length == 2) {
                if (Input.ToLower() == "da") {
                    Prefix = MetricPrefix.Deca;
                }
            }
            else if (Input.Length == 1) {
                switch (Input[0]) {
                    case 'Q': Prefix = MetricPrefix.Ronna; break;         //Proposed
                    case 'R': Prefix = MetricPrefix.Quetta; break;        //Proposed
                    case 'И': Prefix = MetricPrefix.Yotta; break;
                    case 'Y': Prefix = MetricPrefix.Yotta; break;
                    case 'Z': Prefix = MetricPrefix.Zetta; break;
                    case 'З': Prefix = MetricPrefix.Zetta; break;
                    case 'E': Prefix = MetricPrefix.Exa; break;
                    case 'Э': Prefix = MetricPrefix.Exa; break;
                    case 'P': Prefix = MetricPrefix.Peta; break;
                    case 'П': Prefix = MetricPrefix.Peta; break;
                    case 'T': Prefix = MetricPrefix.Tera; break;
                    case 't': Prefix = MetricPrefix.Tera; break;
                    case 'g': Prefix = MetricPrefix.Giga; break;
                    case 'G': Prefix = MetricPrefix.Giga; break;
                    case 'Г': Prefix = MetricPrefix.Giga; break;
                    case 'M': Prefix = MetricPrefix.Mega; break;
                    case 'М': Prefix = MetricPrefix.Mega; break;
                    case 'K': Prefix = MetricPrefix.Kilo; break;
                    case 'k': Prefix = MetricPrefix.Kilo; break;
                    case 'к': Prefix = MetricPrefix.Kilo; break;
                    case 'h': Prefix = MetricPrefix.Hecto; break;
                    case 'H': Prefix = MetricPrefix.Hecto; break;
                    case 'г': Prefix = MetricPrefix.Hecto; break;
                    case 'd': Prefix = MetricPrefix.Deci; break;
                    case 'д': Prefix = MetricPrefix.Deci; break;
                    case 'c': Prefix = MetricPrefix.Centi; break;
                    case 'C': Prefix = MetricPrefix.Centi; break;
                    case 'm': Prefix = MetricPrefix.Milli; break;
                    case 'м': Prefix = MetricPrefix.Milli; break;
                    case 'μ': Prefix = MetricPrefix.Micro; break;
                    case 'µ': Prefix = MetricPrefix.Micro; break;
                    case 'u': Prefix = MetricPrefix.Micro; break;
                    case 'n': Prefix = MetricPrefix.Nano; break;
                    case 'н': Prefix = MetricPrefix.Nano; break;
                    case 'p': Prefix = MetricPrefix.Pico; break;
                    case 'ρ': Prefix = MetricPrefix.Pico; break;
                    case 'п': Prefix = MetricPrefix.Pico; break;
                    case 'F': Prefix = MetricPrefix.Femto; break;
                    case 'f': Prefix = MetricPrefix.Femto; break;
                    case 'ф': Prefix = MetricPrefix.Femto; break;
                    case 'Ф': Prefix = MetricPrefix.Femto; break;
                    case 'a': Prefix = MetricPrefix.Atto; break;
                    case 'A': Prefix = MetricPrefix.Atto; break;
                    case 'z': Prefix = MetricPrefix.Zepto; break;
                    case 'з': Prefix = MetricPrefix.Zepto; break;
                    case 'y': Prefix = MetricPrefix.Yocto; break;
                    case 'r': Prefix = MetricPrefix.Ronto; break;        //
                    case 'q': Prefix = MetricPrefix.Quetta; break;       //
                    default: break;
                }
            }
        }
        private bool IsVaildPrefixChar(char Input) {
            switch (Input) {
                case 'Q': Prefix = MetricPrefix.Ronna; return true;         //
                case 'R': Prefix = MetricPrefix.Quetta; return true;        //
                case 'И': Prefix = MetricPrefix.Yotta; return true;
                case 'Y': Prefix = MetricPrefix.Yotta; return true;
                case 'З': Prefix = MetricPrefix.Zetta; return true;
                case 'Z': Prefix = MetricPrefix.Zetta; return true;
                case 'E': Prefix = MetricPrefix.Exa; return true;
                case 'Э': Prefix = MetricPrefix.Exa; return true;
                case 'P': Prefix = MetricPrefix.Peta; return true;
                case 'П': Prefix = MetricPrefix.Peta; return true;
                case 'T': Prefix = MetricPrefix.Tera; return true;
                case 't': Prefix = MetricPrefix.Tera; return true;
                case 'g': Prefix = MetricPrefix.Giga; return true;
                case 'G': Prefix = MetricPrefix.Giga; return true;
                case 'Г': Prefix = MetricPrefix.Giga; return true;
                case 'M': Prefix = MetricPrefix.Mega; return true;
                case 'М': Prefix = MetricPrefix.Mega; return true;
                case 'K': Prefix = MetricPrefix.Kilo; return true;
                case 'k': Prefix = MetricPrefix.Kilo; return true;
                case 'к': Prefix = MetricPrefix.Kilo; return true;
                case 'h': Prefix = MetricPrefix.Hecto; return true;
                case 'H': Prefix = MetricPrefix.Hecto; return true;
                case 'г': Prefix = MetricPrefix.Hecto; return true;
                case 'd': Prefix = MetricPrefix.Deci; return true;
                case 'д': Prefix = MetricPrefix.Deci; return true;
                case 'c': Prefix = MetricPrefix.Centi; return true;
                case 'C': Prefix = MetricPrefix.Centi; return true;
                case 'm': Prefix = MetricPrefix.Milli; return true;
                case 'м': Prefix = MetricPrefix.Milli; return true;
                case 'μ': Prefix = MetricPrefix.Micro; return true;
                case 'µ': Prefix = MetricPrefix.Micro; return true;
                case 'u': Prefix = MetricPrefix.Micro; return true;
                case 'n': Prefix = MetricPrefix.Nano; return true;
                case 'н': Prefix = MetricPrefix.Nano; return true;
                case 'p': Prefix = MetricPrefix.Pico; return true;
                case 'ρ': Prefix = MetricPrefix.Pico; return true;
                case 'п': Prefix = MetricPrefix.Pico; return true;
                case 'F': Prefix = MetricPrefix.Femto; return true;
                case 'f': Prefix = MetricPrefix.Femto; return true;
                case 'ф': Prefix = MetricPrefix.Femto; return true;
                case 'Ф': Prefix = MetricPrefix.Femto; return true;
                case 'a': Prefix = MetricPrefix.Atto; return true;
                case 'A': Prefix = MetricPrefix.Atto; return true;
                case 'z': Prefix = MetricPrefix.Zepto; return true;
                case 'з': Prefix = MetricPrefix.Zepto; return true;
                case 'и': Prefix = MetricPrefix.Yocto; return true;
                case 'y': Prefix = MetricPrefix.Yocto; return true;
                case 'r': Prefix = MetricPrefix.Ronto; return true;        //
                case 'q': Prefix = MetricPrefix.Quetta; return true;       //
                default: return false; ;
            }
        }
        private ValueRangeType ValueInRange(string Input) {
            if (rangeLimited == true) {
                string TempInput = Input;
                if (numberbase == NumberBase.Base2) {
                    TempInput = MathHandler.BinaryToDecimal(Input, BinaryFormatFlags.Length256Bit | BinaryFormatFlags.Signed).ToString();
                }
                else if (numberbase == NumberBase.Base8) {
                    TempInput = MathHandler.OctalToDecimal(Input, BinaryFormatFlags.Length256Bit | BinaryFormatFlags.Signed).ToString();
                }
                else if (numberbase == NumberBase.Base16) {
                    TempInput = MathHandler.HexadecimalToDecimal(Input, BinaryFormatFlags.Length256Bit | BinaryFormatFlags.Signed).ToString();
                }
                NumericalString TestValue = new NumericalString(TempInput);
                if (TestValue < minimum) {
                    return ValueRangeType.OutOfLower;
                }
                if (TestValue > maximum) {
                    return ValueRangeType.OutOfUpper;
                }
                return ValueRangeType.InRange;
            }
            return ValueRangeType.InRange;
        }
        private void CorrectValueToRange(string Input, out string Output) {
            ValueRangeType State = ValueInRange(Input);
            BinaryFormatFlags FormatFlags = BinaryFormatFlags.Length256Bit | BinaryFormatFlags.Signed;
            switch (State) {
                case ValueRangeType.OutOfLower:
                    if (numberbase == NumberBase.Base2) {
                        Output = MathHandler.DecimalToBinary(minimum.ToString(), FormatFlags);
                    }
                    else if (numberbase == NumberBase.Base8) {
                        Output = MathHandler.DecimalToOctal(minimum.ToString(), FormatFlags);
                    }
                    else if (numberbase == NumberBase.Base16) {
                        Output = MathHandler.DecimalToHexadecimal(minimum.ToString(), FormatFlags);
                    }
                    else { Output = minimum.ToString(); }
                    break;
                case ValueRangeType.InRange:
                    Output = Input; break;
                case ValueRangeType.OutOfUpper:
                    if (numberbase == NumberBase.Base2) {
                        Output = MathHandler.DecimalToBinary(maximum.ToString(), FormatFlags);
                    }
                    else if (numberbase == NumberBase.Base8) {
                        Output = MathHandler.DecimalToOctal(maximum.ToString(), FormatFlags);
                    }
                    else if (numberbase == NumberBase.Base16) {
                        Output = MathHandler.DecimalToHexadecimal(maximum.ToString(), FormatFlags);
                    }
                    else { Output = maximum.ToString(); }
                    break;
                default:
                    Output = Input; break;
            }
        }
        public enum MetricPrefix {
            Quecto = -12,   //
            Ronto = -11,    //
            Yocto = -10,
            Zepto = -9,
            Atto = -8,
            Femto = -7,
            Pico = -6,
            Nano = -5,
            Micro = -4,
            Milli = -3,
            Centi = -2,
            Deci = -1,
            None = 0,
            Deca = 1,
            Hecto = 2,
            Kilo = 3,
            Mega = 4,
            Giga = 5,
            Tera = 6,
            Peta = 7,
            Exa = 8,
            Zetta = 9,
            Yotta = 10,
            Ronna = 11,     //
            Quetta = 12     //
        }
        #endregion
        #region Number Handling
        public enum NumberFormat {
            Decimal = 0x01,
            Scientific = 0x02
        }
        public enum SecondaryUnitDisplayType {
            NoSecondaryUnit = 0x00,
            Multiply = 0x01,
            Divide = 0x02
        }
        public enum NumberBase {
            Base2 = 0x02,
            Base8 = 0x08,
            Base10 = 0x0A,
            Base16 = 0x10
        }
        private enum ValueRangeType {
            OutOfLower = 0x01,
            InRange = 0x02,
            OutOfUpper = 0x03
        }
        #endregion
        #region Reporting
        public bool ValueStartsWithZero() {
            string TempValue = ValueString;
            if (TempValue.StartsWith("-")) {
                TempValue = TempValue.Remove(0, 1);
            }
            if (TempValue.Length > 0) {
                if (TempValue[0] == '0') {
                    return true;
                }
            }
            return false;
        }
        public int ValueDigitsBeforePoint() {
            string TempValue = ValueString;
            if (TempValue.StartsWith("-")) {
                TempValue = TempValue.Remove(0, 1);
            }
            if (TempValue.Length > 0) {
                if (TempValue.Contains(".")) {
                    string DigitsBefore = TempValue.Split('.')[0];
                    return DigitsBefore.Length;
                }
                return TempValue.Length;
            }
            return 0;
        }
        public bool IsZeroed() {
            string TempValue = ValueString;
            if (TempValue.StartsWith("-")) {
                TempValue = TempValue.Remove(0, 1);
            }
            TempValue = TempValue.Replace(".", "");
            TempValue = TempValue.Replace("0", "");
            if (TempValue.Length == 0) { return true; }
            return false;
        }
        public int ZerosDigitsAfterPoint() {
            string TempValue = ValueString;
            if (TempValue.StartsWith("-")) {
                TempValue = TempValue.Remove(0, 1);
            }
            if (TempValue.Length > 0) {
                if (TempValue.Contains(".")) {
                    string DigitsAfter = TempValue.Split('.')[1];
                    int Count = 0;
                    for (int i = 0; i < DigitsAfter.Length; i++) {
                        if (DigitsAfter[i] == '0') {
                            Count++;
                        }
                        else {
                            break;
                        }
                    }
                    return Count;
                }
                return 0;
            }
            return 0;
        }
        #endregion
        private void NumericTextbox_Load(object sender, EventArgs e) {

        }
        protected override void OnDragEnter(DragEventArgs drgevent) {
            if (drgevent.Data != null) {
                if (drgevent.Data.GetDataPresent(DataFormats.Text, false)) {
                    drgevent.Effect = DragDropEffects.Copy;
                }
            }
            else {
                drgevent.Effect = DragDropEffects.None;
            }
        }
        protected override void OnDragDrop(DragEventArgs drgevent) {
            if (drgevent.Data != null) {
                string? Data = drgevent.Data.GetData(DataFormats.Text, true).ToString();
                if (Data != null) {
                    PasteNumber(Data);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e) {
            Invalidate();
            base.OnSizeChanged(e);
        }

        #region AutoSize
        //private Size GetAutoSize() {
        //    //  Do your specific calculation here...
        //    Size size = new Size(Width, 10);
        //
        //    return size;
        //}
        //private void ResizeForAutoSize() {
        //    if (this.AutoSize) {
        //        this.SetBoundsCore(this.Left, this.Top, this.Width, this.Height, BoundsSpecified.Size);
        //    }
        //}
        //public override Size GetPreferredSize(Size proposedSize) {
        //    return GetAutoSize();
        //}
        //protected override void SetBoundsCore(int x, int y, int width, int height,
        //        BoundsSpecified specified) {
        //    //  Only when the size is affected...
        //    if (this.AutoSize && (specified & BoundsSpecified.Size) != 0) {
        //        Size size = GetAutoSize();
        //
        //        width = size.Width;
        //        height = size.Height;
        //    }
        //
        //    base.SetBoundsCore(x, y, width, height, specified);
        //}
        #endregion
    }
    public class UnitClickedEventArgs : EventArgs {
        public string Value;
        public NumericTextbox.MetricPrefix Prefix;
        public string Unit;
        public bool IsSecondary = false;
        private Point location;
        private Point screenLocation;
        public Point ScreenLocation {
            get { return screenLocation; }
        }
        public Point Location {
            get { return location; }
        }

        public UnitClickedEventArgs(Point HitPoint, Point AbsolutePosition, string value, NumericTextbox.MetricPrefix prefix, string unit, bool IsSecondary = false) {
            Value = value;
            Prefix = prefix;
            Unit = unit;
            location = HitPoint;
            screenLocation = AbsolutePosition;
            this.IsSecondary = IsSecondary;
        }
    }
    public class PrefixClickedEventArgs : EventArgs {
        public string Value;
        public NumericTextbox.MetricPrefix Prefix;
        public string Unit;
        public bool IsSecondary = false;
        private Point location;
        public Point Location {
            get { return location; }
        }
        private Point screenLocation;
        public Point ScreenLocation {
            get { return screenLocation; }
        }
        public PrefixClickedEventArgs(Point HitPoint, Point AbsolutePosition, string value, NumericTextbox.MetricPrefix prefix, string unit, bool IsSecondary = false) {
            Value = value;
            Prefix = prefix;
            Unit = unit;
            location = HitPoint;
            screenLocation = AbsolutePosition;
            this.IsSecondary = IsSecondary;
        }
    }
    public class ValueChangedEventArgs : EventArgs {
        public string Value;
        public NumericTextbox.MetricPrefix Prefix;
        public bool IsSecondary = false;
        public ValueChangedEventArgs(string Value, NumericTextbox.MetricPrefix Prefix, bool IsSecondary = false) {
            this.Value = Value;
            this.Prefix = Prefix;
            this.IsSecondary = IsSecondary;
        }
    }
    public static class Extensions {
        public static T Next<T>(this T src) where T : struct {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])System.Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
        public static T Previous<T>(this T src) where T : struct {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])System.Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) - 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
