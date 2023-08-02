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

namespace ODModules {
    public partial class LabelButton : UserControl {
        public delegate void ButtonClickedHandler(object sender, Point MenuPoint);
        [Category("Value")]
        public event ButtonClickedHandler? ButtonClicked;
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
        private string labelText = "";
        [Category("Appearance")]
        public string LabelText {
            get { return labelText; }
            set {
                labelText = value;
                Invalidate();
            }
        }
        public LabelButton() {
            InitializeComponent();
            this.buttondowncolor = Color.Gray;
            this.hovercolor = Color.Beige;
            DoubleBuffered = true;
        }

        private void LabelButton_Load(object sender, EventArgs e) {

        }
        Point CursorLocation = new Point(0, 0);
        bool IsMouseDown = false;
        Rectangle TextBlock = new Rectangle(-1, -1, 0, 0);
        Rectangle ArrowBlock = new Rectangle(-1, -1, 0, 0);
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (LabelText.Length > 0) {
                SizeF TextSize = e.Graphics.MeasureString(LabelText, Font);
                Size UnitSize = new Size((int)e.Graphics.MeasureString("W", Font).Width, (int)e.Graphics.MeasureString("W", Font).Height);
                Point CentrePoint = new Point(0, (int)(Height - TextSize.Height) / 2);
                TextBlock = new Rectangle(CentrePoint, new Size((int)TextSize.Width, (int)TextSize.Height));
                ArrowBlock = new Rectangle(new Point(CentrePoint.X + TextBlock.Width, CentrePoint.Y), UnitSize);
                Color TextColor = GetColorMouseAction(TextBlock);
                Color ArrowColor = GetColorMouseAction(ArrowBlock);
                TextColor = ArrowColor == ForeColor ? TextColor : ArrowColor;
                using (SolidBrush TextBrush = new SolidBrush(TextColor)) {
                    e.Graphics.DrawString(LabelText, Font, TextBrush, TextBlock.Location);
                }
                using (SolidBrush BorderBrush = new SolidBrush(TextColor)) {
                    using (Pen BorderPen = new Pen(BorderBrush)) {
                        int Height1over5 = (int)((float)ArrowBlock.Height * (2.0f / 6.0f));
                        int Height4over5 = (int)((float)ArrowBlock.Height * (4.0f / 6.0f));
                        int ThirdWidth = (int)(ArrowBlock.Width / 4.5f);
                        e.Graphics.DrawLines(BorderPen, new Point[]{
                            new Point(ArrowBlock.Left+ThirdWidth, ArrowBlock.Top + Height1over5),
                            new Point(ArrowBlock.Left + ArrowBlock.Width/2, ArrowBlock.Top + Height4over5),
                            new Point(ArrowBlock.Right-ThirdWidth, ArrowBlock.Top+ Height1over5)});
                    }
                }
            }
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

        protected override void OnMouseClick(MouseEventArgs e) {
            if ((GetBoundState(TextBlock) == ButtonState.MouseDown) || (GetBoundState(ArrowBlock) == ButtonState.MouseDown)) {
                ButtonClicked?.Invoke(this, new Point(Location.X,Location.Y + TextBlock.Y + TextBlock.Height));
            }
            base.OnMouseClick(e);
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            CursorLocation = new Point(-1, -1);
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            CursorLocation = e.Location;
            Invalidate();
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            IsMouseDown = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            IsMouseDown = true;
            Invalidate();
            base.OnMouseDown(e);
        }
        private enum ButtonState {
            Normal = 0x00,
            MouseOver = 0x01,
            MouseDown = 0x02
        }
    }
}
