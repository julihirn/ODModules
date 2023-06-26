using Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public class LED : UserControl {
        System.Windows.Forms.Timer AnimationTimer;
        public LED() {
            DoubleBuffered = true;
            AnimationTimer = new System.Windows.Forms.Timer();
            AnimationTimer.Interval = 1;
            AnimationTimer.Enabled = false;
            AnimationTimer.Tick += AnimationTimer_Tick;
        }


        #region Properties
        protected bool InDesignMode {
            get {
                return (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            }
        }
        private Color ledColor = Color.Red;
        [System.ComponentModel.Category("Appearance")]
        public Color LEDColor {
            get {
                return ledColor;
            }
            set {
                ledColor = value;
                Invalidate();
            }
        }
        float currentValue = 0;
        float displayValue = 0;
        float LampValue = 0;
        float ValueDifferenceStep = 0;
        int AnimationFrames = 1000;
        [System.ComponentModel.Category("Appearance")]
        public float Brightness {
            get {
                return displayValue;
            }
            set {
                displayValue = value;
                if (value < 0) {
                    currentValue = 0;
                }
                else if (value > 100) {
                    currentValue = 100;
                }
                else {
                    currentValue = value;
                }
                if (smoothLampChanges == true) {
                    if (InDesignMode == false) {
                        ValueDifferenceStep = (currentValue - LampValue);// / (decimal)AnimationFrames;
                        AnimationTimer.Enabled = true;
                    }
                    else {
                        LampValue = value;
                    }
                }
                Invalidate();
            }
        }
        bool smoothLampChanges = false;
        [System.ComponentModel.Category("Behavior")]
        public bool SmoothLampChanges {
            get {
                return smoothLampChanges;
            }
            set {
                smoothLampChanges = value;
                if (value == false) {
                    AnimationTimer.Enabled = false;
                }
                Invalidate();
            }
        }
        bool showText = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowText {
            get {
                return showText;
            }
            set {
                showText = value;
                Invalidate();
            }
        }
        float ledScale = 1.0f;
        [System.ComponentModel.Category("Appearance")]
        public float LEDScale {
            get {
                return ledScale;
            }
            set {
                ledScale = value;
                Invalidate();
            }
        }
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text {
            get { return base.Text; }
            set {
                base.Text = value;
                Invalidate();
            }
        }

        #endregion
        #region Drawing 
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int LEDSize = (int)((DetermineSmallestSide() - 5) * 0.5f);
            Point LEDLocation = new Point((int)((Width - LEDSize) / 2.0f), (int)((Height - LEDSize) / 2.0f));
            float LEDBrightness = 0;
            if (smoothLampChanges == false) { LEDBrightness = currentValue; }
            else { LEDBrightness = LampValue; }
            if (showText) {
                int TextHeight = (int)e.Graphics.MeasureString("W", Font).Height;
                int LEDHeight = (int)(e.Graphics.MeasureString("W", Font).Height * ledScale);
                int Centre = (int)(((float)Height - (float)TextHeight) / 2.0f);
                int LEDCentre = (int)(((float)Height - (float)LEDHeight) / 2.0f);
                DrawLED(e, new Point(10, LEDCentre), LEDHeight, LEDColor, LEDBrightness);
                using (SolidBrush TextBrush = new SolidBrush(ForeColor)) {
                    if (Text != null) {
                        if (Text.Length > 0) {
                            e.Graphics.DrawString(Text, Font, TextBrush, new Point(15 + LEDHeight, Centre));
                        }
                    }
                }

            }
            else {
                DrawLED(e, LEDLocation, LEDSize, LEDColor, LEDBrightness);
            }

        }
        private void DrawLED(PaintEventArgs e, Point Position, int SquareSize, Color DiodeColor, float Brightness) {
            if (Brightness > 100) { Brightness = 100; }
            else if (Brightness < 0) { Brightness = 0; }

            float AdjustedBrightness = 1.0f - (Brightness / 130.0f);
            int BorderSize = (int)((float)SquareSize * 0.02f);
            int GlowSize = (int)((float)SquareSize * 0.75f);
            Rectangle LEDRectangle = new Rectangle(Position.X, Position.Y, SquareSize, SquareSize);
            Rectangle LEDGlowRectangle = EnlargeRectangle(LEDRectangle, new Size(GlowSize, GlowSize));
            Color LED_GlowColor1 = Color.Transparent;
            Color LED_GlowColor2 = Color.FromArgb((int)(((float)Brightness / 100.0f) * 255.0f), DiodeColor.R, DiodeColor.G, DiodeColor.B);
            DrawRadialGradient(e, LEDGlowRectangle, LED_GlowColor1, LED_GlowColor2);
            Color LED_BaseColor1 = RenderHandler.BlackDarkenColor(DiodeColor, (int)(240.0f * AdjustedBrightness));
            Color LED_BaseColor2 = RenderHandler.BlackDarkenColor(DiodeColor, (int)(100.0f * AdjustedBrightness));
            DrawRadialGradient(e, LEDRectangle, LED_BaseColor1, LED_BaseColor2);
            Color LED_BorderColor1 = RenderHandler.BlackDarkenColor(DiodeColor, (int)(175.0f * (0.5f * AdjustedBrightness)));
            Color LED_BorderColor2 = RenderHandler.BlackDarkenColor(DiodeColor, (int)(200.0f * (0.5f * AdjustedBrightness)));
            Color LED_LightSpecular = Color.FromArgb(128, 255, 255, 255);
            DrawSpecularMark(e, LEDRectangle, new Polar(225, SquareSize * 0.26f), new Size((int)(SquareSize * 0.18f), (int)(SquareSize * 0.18f)), Color.White);
            DrawSpecularMark(e, LEDRectangle, new Polar(190, SquareSize * 0.26f), new Size((int)(SquareSize * 0.1f), (int)(SquareSize * 0.1f)), LED_LightSpecular);
            DrawSpecularMark(e, LEDRectangle, new Polar(45, SquareSize * 0.26f), new Size((int)(SquareSize * 0.1f), (int)(SquareSize * 0.1f)), LED_LightSpecular);
            using (LinearGradientBrush BorderBrush = new LinearGradientBrush(LEDRectangle, LED_BorderColor1, LED_BorderColor2, 90)) {
                using (Pen BorderPen = new Pen(BorderBrush, BorderSize)) {
                    BorderPen.Alignment = PenAlignment.Outset;
                    e.Graphics.DrawEllipse(BorderPen, LEDRectangle);
                }
            }
        }
        private void DrawSpecularMark(PaintEventArgs e, Rectangle LED, Polar Position, Size SpecularSize, Color SpecularColor) {
            Point Centre = new Point(LED.X + (int)((float)LED.Width / 2.0f), LED.Y + (int)((float)LED.Height / 2.0f));
            Point SpecularCentre = ToRectangular(Position.Angle, (int)Position.Radius, Centre);
            Size HalfSize = new Size((int)((float)SpecularSize.Width / 2.0f), (int)((float)SpecularSize.Height / 2.0f));
            Rectangle SpecularRectangle = new Rectangle(SpecularCentre.X - HalfSize.Width, SpecularCentre.Y - HalfSize.Height, SpecularSize.Width, SpecularSize.Height);
            DrawRadialGradient(e, SpecularRectangle, Color.Transparent, SpecularColor);

        }
        private void DrawRadialGradient(PaintEventArgs e, Rectangle Position, Color InnerColor, Color OutterColor) {
            try {
                using (GraphicsPath Path = new GraphicsPath()) {
                    Path.AddEllipse(Position);
                    using (PathGradientBrush PathBrush = new PathGradientBrush(Path)) {
                        PathBrush.SurroundColors = new Color[] { InnerColor };
                        PathBrush.CenterColor = OutterColor;
                        e.Graphics.FillRectangle(PathBrush, Position);
                    }
                }
            }
            catch { }
        }

        #endregion
        #region Drawing Support
        private int DetermineSmallestSide() {
            if (Width >= Height) { return Height; }
            else { return Width; }
        }
        private Rectangle EnlargeRectangle(Rectangle Input, Size EnlargeBy) {
            float xSizeChange = (float)EnlargeBy.Width / 2.0f;
            float ySizeChange = (float)EnlargeBy.Height / 2.0f;
            return new Rectangle(Input.X - (int)xSizeChange, Input.Y - (int)ySizeChange, Input.Width + (int)(xSizeChange * 2), Input.Height + (int)(ySizeChange * 2));
        }
        private Rectangle ShrinkRectangle(Rectangle Input, Size ShrinkBy) {
            float xSizeChange = (float)ShrinkBy.Width / 2.0f;
            float ySizeChange = (float)ShrinkBy.Height / 2.0f;
            return new Rectangle(Input.X + (int)xSizeChange, Input.Y + (int)ySizeChange, Input.Width - (int)(xSizeChange * 2), Input.Height - (int)(ySizeChange * 2));
        }
        private Size ShrinkSize(Size Input, SizeF ShrinkBy) {
            return new Size((int)((float)Input.Width * ShrinkBy.Width), (int)((float)Input.Height * ShrinkBy.Height));
        }
        Point ToRectangular(double Angle, int Radius, Point Offset) {
            double x = (double)(Radius * Math.Cos(Angle * Math.PI / 180.0f)) + (double)Offset.X;
            double y = (double)(Radius * Math.Sin(Angle * Math.PI / 180.0f)) + (double)Offset.Y;
            return new Point((int)x, (int)y);
        }
        #endregion 
        int Tick = 0;
        private void AnimationTimer_Tick(object? sender, EventArgs e) {
            if (InDesignMode == false) {
                if ((currentValue <= LampValue + 0.01f) && (currentValue >= LampValue - 0.01f)) {
                    LampValue = currentValue;
                    ValueDifferenceStep = 0;
                    AnimationTimer.Enabled = false;
                    Tick = 0;
                }
                else {
                    ValueDifferenceStep = (currentValue - LampValue);
                    LampValue += ValueDifferenceStep * (float)Math.Abs(Math.Sin(Math.PI * ((double)Tick / (double)AnimationFrames)));
                    Tick++;
                }
                Invalidate();
            }
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // LED
            // 
            this.Name = "LED";
            this.Load += new System.EventHandler(this.LED_Load);
            this.ResumeLayout(false);

        }

        private void LED_Load(object? sender, EventArgs e) {

        }
    }
    public struct Polar {
        float angle;
        float radius;
        public Polar(float angle, float radius) {
            this.angle = angle;
            this.radius = radius;
        }
        public float Angle { get => angle; }
        public float Radius { get => radius; }

    }
}
