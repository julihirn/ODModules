using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public class ValueLabel : UserControl {
        public ValueLabel() {
            NorthGrowthColor = Color.LightYellow;
            SouthGrowthColor = Color.LimeGreen;
            NorthStateColor = Color.LightYellow;
            SouthStateColor = Color.Yellow;
            NorthFallColor = Color.Pink;
            SouthFallColor = Color.OrangeRed;
            SeparatorColor = Color.Black;
            LabelFont = Font;
            ValueFont = new Font(Font.FontFamily, 14);
            ValueColor = Color.Black;
            LabelColor = Color.Black;
            LabelText = "";
            DoubleBuffered = true;
            Values = new float[Steps];
        }
        #region Properties
        private Color _NorthGrowthColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color NorthGrowthColor {
            get {
                return _NorthGrowthColor;
            }
            set {
                _NorthGrowthColor = value; Invalidate();
            }
        }
        private Color _SouthGrowthColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color SouthGrowthColor {
            get {
                return _SouthGrowthColor;
            }
            set {
                _SouthGrowthColor = value; Invalidate();
            }
        }
        private Color _NorthStateColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color NorthStateColor {
            get {
                return _NorthStateColor;
            }
            set {
                _NorthStateColor = value; Invalidate();
            }
        }
        private Color _SouthStateColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color SouthStateColor {
            get {
                return _SouthStateColor;
            }
            set {
                _SouthStateColor = value; Invalidate();
            }
        }
        private Color _NorthFallColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color NorthFallColor {
            get {
                return _NorthFallColor;
            }
            set {
                _NorthFallColor = value; Invalidate();
            }
        }
        private Color _SouthFallColor;
        [System.ComponentModel.Category("Status Appearance")]
        public Color SouthFallColor {
            get {
                return _SouthFallColor;
            }
            set {
                _SouthFallColor = value; Invalidate();
            }
        }
        private Color _SeparatorColor;
        [System.ComponentModel.Category("Appearance")]
        public Color SeparatorColor {
            get {
                return _SeparatorColor;
            }
            set {
                _SeparatorColor = value; Invalidate();
            }
        }
        private Font? _ValueFont;
        [System.ComponentModel.Category("Appearance")]
        public Font? ValueFont {
            get {
                return _ValueFont;
            }
            set {
                _ValueFont = value; Invalidate();
            }
        }
        private Color _ValueColor;
        [System.ComponentModel.Category("Appearance")]
        public Color ValueColor {
            get {
                return _ValueColor;
            }
            set {
                _ValueColor = value; Invalidate();
            }
        }
        private int Steps = 60;
        private float AverageValue = 0.00f;
        private float DeltaValue = 0.00f;
        private float[] Values;
        private float _Value = 0.00f;
        private float Max = 0.00f;
        private float Min = 0.00f;
        private float Range = 0.00f;
        [System.ComponentModel.Category("Control")]
        public float Value {
            get {
                return _Value;
            }
            set {
                _Value = value;
                float Sum_Y = 0.0f;
                float Sum_X = 0.0f;
                float Sum_X2 = 0.0f;
                float Sum_XY = 0.0f;
                float n = (float)Steps;

                float CurrentMin = Values[0];
                float CurrentMax = 0.00f;
                for (int i = 0; i < Steps; i++) {
                    if (i < Steps - 1) {
                        Values[i] = Values[i + 1];
                    }
                    else {
                        Values[i] = value;
                    }
                    Sum_Y += Values[i];
                    Sum_X += (float)i;
                    Sum_X2 += (float)Math.Pow((double)i, (double)2);
                    Sum_XY += Values[i] * (float)i;
                    if (Values[i] <= CurrentMin) { CurrentMin = value; }
                    if (Values[i] >= CurrentMax) { CurrentMax = value; }
                }

                AverageValue = Sum_Y / 60.0f;
                DeltaValue = ((n * Sum_XY) - (Sum_X * Sum_Y)) / ((n * Sum_X2) - (float)Math.Pow(Sum_X, (double)2));
                Max = CurrentMax;
                Min = CurrentMin;
                Debug.Print(Min.ToString() + " - " + Max.ToString());
                float TempRange = Max - Min;

                if (TempRange == 0) { Range = Max; }
                else { Range = TempRange; }
                Invalidate();
            }
        }
        private Color _LabelColor;
        [System.ComponentModel.Category("Appearance")]
        public Color LabelColor {
            get {
                return _LabelColor;
            }
            set {
                _LabelColor = value;
                Invalidate();
            }
        }
        private string _ValueFormat = "0";
        [System.ComponentModel.Category("Appearance")]
        public string ValueFormat {
            get {
                return _ValueFormat;
            }
            set {
                _ValueFormat = value; Invalidate();
            }
        }
        private string _ValueUnit = "";
        [System.ComponentModel.Category("Appearance")]
        public string ValueUnit {
            get {
                return _ValueUnit;
            }
            set {
                _ValueUnit = value; Invalidate();
            }
        }
        private Font? _LabelFont;
        [System.ComponentModel.Category("Appearance")]
        public Font? LabelFont {
            get {
                return _LabelFont;
            }
            set {
                _LabelFont = value; Invalidate();
            }
        }
        private string _LabelText = "";
        [System.ComponentModel.Category("Appearance")]
        public string LabelText {
            get {
                return _LabelText;
            }
            set {
                _LabelText = value; Invalidate();
            }
        }
        #endregion
        #region Drawing
        protected override void OnPaint(PaintEventArgs e) {
            int ValHeight = 9;
            if (ValueFont != null) {
                ValHeight = (int)e.Graphics.MeasureString("W", ValueFont).Height;
            }
            int ValueCentre = (int)((Height - ValHeight) / 2.0f);
            TrendArrowDirection Direction = TrendArrowDirection.ArrowNoChange;
            if (DeltaValue > 0.0f) {
                Direction = TrendArrowDirection.ArrowUp;
            }
            else {
                Direction = TrendArrowDirection.ArrowDown;
            }
            Rectangle BodyBounds = new Rectangle(0, 0, Width, Height);
            //using (LinearGradientBrush AverageBrush = new LinearGradientBrush(BodyBounds, Color.Transparent, ForeColor, 0.00f)) {
            //    float LinePosition = (float)Height - (((AverageValue - Min)/Range) * (float)Height);
            //    e.Graphics.FillRectangle(AverageBrush, new Rectangle((int)(Width * 0.4f), (int)LinePosition, (int)(Width - (Width * 0.4f)), 3));
            //}
            if (_LabelText.Length > 0) {
                if (_LabelFont != null) {
                    if (_ValueFont != null) {
                        int LblHeight = (int)e.Graphics.MeasureString("W", _LabelFont).Height - 5;
                        int LabelCentre = (int)((Height - (LblHeight + ValHeight)) / 2.0f);
                        Point TextPosition = new Point(ValHeight + (int)(1.4f * (float)ValHeight), LabelCentre);
                        using (SolidBrush LblBrush = new SolidBrush(_LabelColor)) {
                            e.Graphics.DrawString(_LabelText, _LabelFont, LblBrush, TextPosition);
                        }
                        TextPosition.Y += LblHeight + 2;
                        using (SolidBrush ValBrush = new SolidBrush(_ValueColor)) {
                            e.Graphics.DrawString(_Value.ToString(_ValueFormat) + _ValueUnit, _ValueFont, ValBrush, TextPosition);
                        }
                    }
                }
            }
            else {
                Point TextPosition = new Point(ValHeight + (int)(1.4f * (float)ValHeight), ValueCentre);
                if (_ValueFont != null) {
                    using (SolidBrush ValBrush = new SolidBrush(_ValueColor)) {
                        e.Graphics.DrawString(_Value.ToString(_ValueFormat) + _ValueUnit, _ValueFont, ValBrush, TextPosition);
                    }
                }
            }
            DrawArrow(e, Direction, new Point(ValHeight, ValueCentre), new Size((int)(1.2f * (float)ValHeight), ValHeight));
        }
        private void DrawArrow(PaintEventArgs e, TrendArrowDirection Direction, Point Offset, Size ArrowSize) {
            Color ColNorth = _NorthStateColor;
            Color ColSouth = _SouthStateColor;
            if (Direction == TrendArrowDirection.ArrowUp) {
                ColNorth = _NorthGrowthColor;
                ColSouth = _SouthGrowthColor;
            }
            else if (Direction == TrendArrowDirection.ArrowDown) {
                ColNorth = _NorthFallColor;
                ColSouth = _SouthFallColor;
            }
            using (LinearGradientBrush LinBrush = new LinearGradientBrush(new Rectangle(Offset, ArrowSize), ColNorth, ColSouth, 90)) {
                Point U_P1 = new Point(0, 0);
                Point U_P2 = new Point(0, 0);
                Point U_P3 = new Point(0, 0);
                Point U_P4 = new Point(0, 0);
                if (Direction == TrendArrowDirection.ArrowUp) {
                    U_P1 = new Point((int)(((double)ArrowSize.Width / 2.0f) + Offset.X), Offset.Y);
                    U_P2 = new Point((int)(((4.0f / 5.0f) * ArrowSize.Width) + Offset.X), ArrowSize.Height + Offset.Y);
                    U_P3 = new Point((int)(((double)ArrowSize.Width / 2.0f) + Offset.X), (int)((5.0f / 7.0f) * ArrowSize.Height) + Offset.Y);
                    U_P4 = new Point((int)((1.0f / 5.0f) * ArrowSize.Width) + Offset.X, ArrowSize.Height + Offset.Y);
                }
                else if (Direction == TrendArrowDirection.ArrowDown) {
                    U_P1 = new Point((int)(ArrowSize.Width / 2.0f) + Offset.X, ArrowSize.Height + Offset.Y);
                    U_P2 = new Point((int)((4.0f / 5.0f) * ArrowSize.Width) + Offset.X, Offset.Y);
                    U_P3 = new Point((int)(ArrowSize.Width / 2.0f) + Offset.X, (int)((1.0f / 7.0f) * ArrowSize.Height) + Offset.Y);
                    U_P4 = new Point((int)((1.0f / 5.0f) * ArrowSize.Width) + Offset.X, Offset.Y);
                }
                else {
                    U_P1 = new Point((int)((1.0f / 5.0f) * Size.Width) + Offset.X, (int)((3.0f / 7.0f) * Size.Height) + Offset.Y);
                    U_P2 = new Point((int)((4.0f / 5.0f) * Size.Width) + Offset.X, (int)((3.0f / 7.0f) * Size.Height) + Offset.Y);
                    U_P4 = new Point((int)((1.0f / 5.0f) * Size.Width) + Offset.X, (int)((4.0f / 7.0f) * Size.Height) + Offset.Y);
                    U_P3 = new Point((int)((4.0f / 5.0f) * Size.Width) + Offset.X, (int)((4.0f / 7.0f) * Size.Height) + Offset.Y);
                }
                Point[] CurvePoints = new[] { U_P1, U_P2, U_P3, U_P4 };
                e.Graphics.FillPolygon(LinBrush, CurvePoints);
                using (SolidBrush BorderBrush = new SolidBrush(ColSouth)) {
                    using (Pen BorderPen = new Pen(BorderBrush)) {
                        BorderPen.Alignment = PenAlignment.Inset;
                        e.Graphics.DrawPolygon(BorderPen, CurvePoints);
                    }
                }
            }

        }
        #endregion
        #region Events
        protected override void OnResize(EventArgs e) {
            Invalidate();
        }
        private void ValueLabel_Load(object? sender, EventArgs e) {

        }
        #endregion 
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // ValueLabel
            // 
            this.Name = "ValueLabel";
            this.Load += new System.EventHandler(this.ValueLabel_Load);
            this.ResumeLayout(false);

        }
    }
    public enum TrendArrowDirection {
        ArrowUp = 0x01,
        ArrowNoChange = 0x02,
        ArrowDown = 0x03
    }
}
