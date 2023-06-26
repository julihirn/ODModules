
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ODModules {
    public class Button : UserControl {
        [Category("Button Actions")]
        public event ButtonClickedHandler? ButtonClicked;
        public delegate void ButtonClickedHandler(object sender);
        [Category("Button Actions")]
        public event ButtonCheckedChangedHandler? ButtonCheckedChanged;
        public delegate void ButtonCheckedChangedHandler(object sender);
        ButtonStyle style = ButtonStyle.Square;
        [System.ComponentModel.Category("Appearance")]
        public ButtonStyle Style {
            get { return style; }
            set {
                style = value;
                Invalidate();
            }
        }

        ButtonType type = ButtonType.Button;
        [System.ComponentModel.Category("Control")]
        public ButtonType Type {
            get {
                return type;
            }
            set {
                type = value;
                Invalidate();
            }
        }
        string radioButtonGroup = "";
        [System.ComponentModel.Category("Control")]
        public string RadioButtonGroup {
            get {
                return radioButtonGroup;
            }
            set {
                radioButtonGroup = value;
                Invalidate();
            }
        }
        int groupMaximumChecked = 2;
        [System.ComponentModel.Category("Control")]
        public int GroupMaximumChecked {
            get {
                return groupMaximumChecked;
            }
            set {
                groupMaximumChecked = value;
                ResetAllothers();
                Invalidate();
            }
        }
        bool allowGroupUnchecking = false;
        [System.ComponentModel.Category("Control")]
        public bool AllowGroupUnchecking {
            get {
                return allowGroupUnchecking;
            }
            set {
                allowGroupUnchecking = value;
                Invalidate();
            }
        }
        bool ischecked = false;
        DateTime lastChecked = DateTime.UtcNow;
        [System.ComponentModel.Category("Control")]
        DateTime LastChecked {
            get { return lastChecked; }
        }

        [System.ComponentModel.Category("Control")]
        public bool Checked {
            get {
                return ischecked;
            }
            set {
                ischecked = value;
                if (value == true) {
                    lastChecked = DateTime.UtcNow;
                }
                ResetAllothers();
                ButtonCheckedChanged?.Invoke(this);
                Invalidate();
            }
        }
        private void ResetAllothers() {
            if (type == ButtonType.RadioButton) {
                if (ischecked == true) {
                    try {
                        if (this.Parent != null) {
                            foreach (ODModules.Button Btn in this.Parent.Controls) {
                                if (Btn != this) {
                                    if (Btn.Type == ButtonType.RadioButton) {
                                        if (Btn.RadioButtonGroup == this.RadioButtonGroup) { Btn.Checked = false; }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            else if (type == ButtonType.CheckBoxLimited) {
                if (ischecked == true) {
                    try {
                        if (this.Parent != null) {
                            List<ODModules.Button> Btns = new List<ODModules.Button>();
                            int CheckedCount = 0;
                            foreach (ODModules.Button Btn in this.Parent.Controls) {
                                if (Btn != this) {
                                    if (Btn.Type == ButtonType.CheckBoxLimited) {
                                        if (Btn.RadioButtonGroup == this.RadioButtonGroup) {
                                            if (Btn.Checked == true) {
                                                Btns.Add(Btn);
                                                CheckedCount++;
                                            }
                                        }
                                    }
                                }
                            }
                            Btns.Sort((x, y) => x.LastChecked.Ticks.CompareTo(y.LastChecked.Ticks));
                            if (CheckedCount >= groupMaximumChecked) {
                                int Diff = CheckedCount + 1 - groupMaximumChecked;
                                for (int i = 0; i < Diff; i++) {
                                    Btns[i].Checked = false;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        int borderRadius = 5;
        [System.ComponentModel.Category("Appearance")]
        public int BorderRadius {
            get {
                return borderRadius;
            }
            set {
                if (value < 2) {
                    borderRadius = 2;
                }
                else if (value > 10) {
                    borderRadius = 10;
                }
                else {
                    borderRadius = value;
                }
                Invalidate();
            }
        }
        Color borderColorNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorNorth {
            get {
                return borderColorNorth;
            }
            set {
                borderColorNorth = value;
                Invalidate();
            }
        }
        Color borderColorShadow = Color.FromArgb(120, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorShadow {
            get {
                return borderColorShadow;
            }
            set {
                borderColorShadow = value;
                Invalidate();
            }
        }
        Color borderColorSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorSouth {
            get {
                return borderColorSouth;
            }
            set {
                borderColorSouth = value;
                Invalidate();
            }
        }
        Color borderColorHoverNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorHoverNorth {
            get {
                return borderColorHoverNorth;
            }
            set {
                borderColorHoverNorth = value;
                Invalidate();
            }
        }
        Color borderColorHoverSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorHoverSouth {
            get {
                return borderColorHoverSouth;
            }
            set {
                borderColorHoverSouth = value;
                Invalidate();
            }
        }
        Color borderColorDownNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorDownNorth {
            get {
                return borderColorDownNorth;
            }
            set {
                borderColorDownNorth = value;
                Invalidate();
            }
        }
        Color borderColorDownSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorDownSouth {
            get {
                return borderColorDownSouth;
            }
            set {
                borderColorDownSouth = value;
                Invalidate();
            }
        }
        Color borderColorCheckedNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorCheckedNorth {
            get {
                return borderColorCheckedNorth;
            }
            set {
                borderColorCheckedNorth = value;
                Invalidate();
            }
        }
        Color borderColorCheckedSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorCheckedSouth {
            get {
                return borderColorCheckedSouth;
            }
            set {
                borderColorCheckedSouth = value;
                Invalidate();
            }
        }
        Color backColorNorth = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorNorth {
            get {
                return backColorNorth;
            }
            set {
                backColorNorth = value;
                Invalidate();
            }
        }
        Color backColorSouth = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorSouth {
            get {
                return backColorSouth;
            }
            set {
                backColorSouth = value;
                Invalidate();
            }
        }
        Color backColorHoverNorth = Color.SkyBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorHoverNorth {
            get {
                return backColorHoverNorth;
            }
            set {
                backColorHoverNorth = value;
                Invalidate();
            }
        }
        Color backColorHoverSouth = Color.SkyBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorHoverSouth {
            get {
                return backColorHoverSouth;
            }
            set {
                backColorHoverSouth = value;
                Invalidate();
            }
        }
        Color backColorDownNorth = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorDownNorth {
            get {
                return backColorDownNorth;
            }
            set {
                backColorDownNorth = value;
                Invalidate();
            }
        }
        Color backColorDownSouth = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorDownSouth {
            get {
                return backColorDownSouth;
            }
            set {
                backColorDownSouth = value;
                Invalidate();
            }
        }
        Color backColorCheckedNorth = Color.Orange;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorCheckedNorth {
            get {
                return backColorCheckedNorth;
            }
            set {
                backColorCheckedNorth = value;
                Invalidate();
            }
        }
        Color backColorCheckedSouth = Color.Orange;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorCheckedSouth {
            get {
                return backColorCheckedSouth;
            }
            set {
                backColorCheckedSouth = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        public override string Text {
            get { return base.Text; }
            set {
                base.Text = value;
                Invalidate();
            }
        }
        string secondaryText = "";
        [System.ComponentModel.Category("Appearance")]
        public string SecondaryText {
            get {
                return secondaryText;
            }
            set {
                secondaryText = value;
                Invalidate();
            }
        }
        Font? secondaryFont = null;
        [System.ComponentModel.Category("Appearance")]
        public Font? SecondaryFont {
            get {
                return secondaryFont;
            }
            set {
                secondaryFont = value;
                Invalidate();
            }
        }
        ButtonTextHorizontal textHorizontalAlignment = ButtonTextHorizontal.Center;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextHorizontal TextHorizontalAlignment {
            get { return textHorizontalAlignment; }
            set {
                textHorizontalAlignment = value;
                Invalidate();
            }
        }
        ButtonTextVertical textVerticalAlignment = ButtonTextVertical.Middle;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextVertical TextVerticalAlignment {
            get { return textVerticalAlignment; }
            set {
                textVerticalAlignment = value;
                Invalidate();
            }
        }
        Rectangle ButtonFill = Rectangle.Empty;
        Rectangle ButtonBounds = Rectangle.Empty;
        Color BackNorth = Color.Black;
        Color BackSouth = Color.Black;

        Color BorderNorth = Color.Black;
        Color BorderSouth = Color.Black;
        protected override void OnPaint(PaintEventArgs e) {
            try {
                ButtonFill = new Rectangle(0, 0, Width, Height);
                ButtonBounds = new Rectangle(0, 0, Width, Height);
                GetBackColors(out BackNorth, out BackSouth);
                GetBorderColors(out BorderNorth, out BorderSouth);
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                DrawButton(e);
                if (secondaryText.Trim().Length == 0) {
                    DrawText(e, ButtonBounds, BackNorth, BackSouth);
                }
                else {
                    if (secondaryFont != null) {
                        DrawText(e, ButtonBounds, BackNorth, BackSouth, true);
                    }
                    else {
                        DrawText(e, ButtonBounds, BackNorth, BackSouth);
                    }
                }
            }
            catch { }
        }
        private void DrawButton(PaintEventArgs e) {
            switch (style) {
                case ButtonStyle.Square:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    DrawStraightBorder(e, ButtonBounds, BorderNorth, BorderSouth);
                    break;
                case ButtonStyle.Round:
                    ButtonBounds = new Rectangle(2, 2, Width - 4, Height - 4);
                    using (GraphicsPath GrPath = RoundedRectangle(ButtonBounds, borderRadius)) {
                        DrawRoundedFill(e, ButtonFill, GrPath, BackNorth, BackSouth);
                        DrawRoundedBorder(e, ButtonFill, GrPath, BorderNorth, BorderSouth);
                    }
                    break;
                case ButtonStyle.SquareNoBorder:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    break;
                case ButtonStyle.RoundNoBorder:
                    ButtonBounds = new Rectangle(2, 2, Width - 4, Height - 4);
                    using (GraphicsPath GrPath = RoundedRectangle(ButtonBounds, borderRadius)) {
                        DrawRoundedFill(e, ButtonFill, GrPath, BackNorth, BackSouth);
                    }
                    break;
                case ButtonStyle.ShadowSquare:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    DrawShadowBorder(e, ButtonBounds);
                    break;
            }
        }
        private void DrawText(PaintEventArgs e, Rectangle ObjectBounds, Color North, Color South, bool UseSecondary = false) {
            if (Text.Length == 0) {
                return;
            }
            Rectangle TextRectangle = ButtonBounds;
            int TextPadding = (int)e.Graphics.MeasureString("W", Font).Width / 2;
            using (StringFormat TextFormat = new StringFormat()) {
                TextFormat.Trimming = StringTrimming.Character;
                switch (textHorizontalAlignment) {
                    case ButtonTextHorizontal.Left:
                        TextRectangle = new Rectangle(ButtonBounds.X + TextPadding, ButtonBounds.Y, ButtonBounds.Width - TextPadding, ButtonBounds.Height);
                        TextFormat.Alignment = StringAlignment.Near; break;
                    case ButtonTextHorizontal.Center:
                        TextFormat.Alignment = StringAlignment.Center; break;
                    case ButtonTextHorizontal.Right:
                        TextRectangle = new Rectangle(ButtonBounds.X, ButtonBounds.Y, ButtonBounds.Width - TextPadding, ButtonBounds.Height);
                        TextFormat.Alignment = StringAlignment.Far; break;
                }
                switch (textVerticalAlignment) {
                    case ButtonTextVertical.Top:
                        TextFormat.LineAlignment = StringAlignment.Near; break;
                    case ButtonTextVertical.Middle:
                        TextFormat.LineAlignment = StringAlignment.Center; break;
                    case ButtonTextVertical.Bottom:
                        TextFormat.LineAlignment = StringAlignment.Far; break;
                }
                if (style == ButtonStyle.Text) {
                    bool AreSame = ColorsSame(North, South);
                    if (AreSame) {
                        using (SolidBrush Br = new SolidBrush(North)) {
                            e.Graphics.DrawString(Text, Font, Br, TextRectangle, TextFormat);
                        }
                    }
                    else {
                        using (LinearGradientBrush Br = new LinearGradientBrush(ObjectBounds, North, South, LinearGradientMode.Vertical)) {
                            e.Graphics.DrawString(Text, Font, Br, TextRectangle, TextFormat);
                        }
                    }
                }
                else {
                    if (UseSecondary == false) {
                        using (SolidBrush TextBrush = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Text, Font, TextBrush, TextRectangle, TextFormat);
                        }
                    }
                    else {
                        SizeF PrimaryText = e.Graphics.MeasureString(Text, Font);
                        SizeF SecondaryText = e.Graphics.MeasureString(secondaryText, secondaryFont);
                        float MidY = ObjectBounds.Y + (ObjectBounds.Height - (PrimaryText.Height + SecondaryText.Height)) / 2.0f;
                        PointF Starting = new PointF(ObjectBounds.X, MidY);

                        TextFormat.LineAlignment = StringAlignment.Center;
                        switch (textVerticalAlignment) {
                            case ButtonTextVertical.Top:

                                break;
                            case ButtonTextVertical.Middle:
                                TextFormat.LineAlignment = StringAlignment.Center;
                                break;
                            case ButtonTextVertical.Bottom:
                                TextFormat.LineAlignment = StringAlignment.Far;
                                break;
                        }
                    }
                }
            }
        }
        public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius) {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0) {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        private void GetBackColors(out Color NorthColor, out Color SouthColor) {
            NorthColor = backColorNorth; SouthColor = backColorSouth;
            if (Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
                        if (type != ButtonType.Button) {
                            if (ischecked == true) {
                                if (IsMouseDown != true) {
                                    NorthColor = AddCalors(backColorNorth, backColorCheckedNorth);
                                    SouthColor = AddCalors(backColorSouth, backColorCheckedSouth);
                                }
                                else {
                                    NorthColor = AddCalors(backColorNorth, backColorDownNorth);
                                    SouthColor = AddCalors(backColorSouth, backColorDownSouth);
                                }
                            }
                        }
                        break;
                    case MouseStates.Hover:
                        NorthColor = AddCalors(backColorNorth, backColorHoverNorth);
                        SouthColor = AddCalors(backColorSouth, backColorHoverSouth); break;
                    case MouseStates.Down:
                        NorthColor = AddCalors(backColorNorth, backColorDownNorth);
                        SouthColor = AddCalors(backColorSouth, backColorDownSouth); break;
                    default:
                        break;
                }
            }
            else {
                NorthColor = WhiteLightenColor(backColorNorth, 64);
                SouthColor = WhiteLightenColor(backColorSouth, 64);
            }
        }
        private void GetBorderColors(out Color NorthColor, out Color SouthColor) {
            NorthColor = borderColorNorth; SouthColor = borderColorSouth;
            if (Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
                        if (type != ButtonType.Button) {
                            if (ischecked == true) {
                                if (IsMouseDown != true) {
                                    NorthColor = AddCalors(borderColorNorth, borderColorCheckedNorth);
                                    SouthColor = AddCalors(borderColorSouth, borderColorCheckedSouth);
                                }
                                else {
                                    NorthColor = AddCalors(borderColorNorth, borderColorDownNorth);
                                    SouthColor = AddCalors(borderColorSouth, borderColorDownSouth);
                                }
                            }
                        }
                        break;
                    case MouseStates.Hover:
                        NorthColor = AddCalors(borderColorNorth, borderColorHoverNorth);
                        SouthColor = AddCalors(borderColorSouth, borderColorHoverSouth); break;
                    case MouseStates.Down:
                        NorthColor = AddCalors(borderColorNorth, borderColorDownNorth);
                        SouthColor = AddCalors(borderColorSouth, borderColorDownSouth); break;
                    default:
                        break;
                }
            }
            else {
                NorthColor = WhiteLightenColor(borderColorNorth, 64);
                SouthColor = WhiteLightenColor(borderColorSouth, 64);
            }
        }
        //public static Color DeterministicDarkenColorInverted(Color Input, Color BackColor, int Alpha) {
        //    if (IsDark(BackColor) == false) {
        //        decimal AlphaReduce = Convert.ToDecimal((255 - Alpha) / (double)255);
        //        int AR = Convert.ToInt32(Math.Floor(Input.R * AlphaReduce));
        //        int AG = Convert.ToInt32(Math.Floor(Input.G * AlphaReduce));
        //        int AB = Convert.ToInt32(Math.Floor(Input.B * AlphaReduce));
        //        return Color.FromArgb(AR, AG, AB);
        //    }
        //    else {
        //        int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
        //        int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
        //        int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
        //        return Color.FromArgb(AR, AG, AB);
        //    }
        //}
        private Color WhiteLightenColor(Color Input, int Alpha) {
            int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
            int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
            int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
            return Color.FromArgb(AR, AG, AB);
        }
        private Color AddCalors(Color A, Color B) {
            if ((A.A == 255) && (B.A == 255)) { return B; }
            else if ((A.A == 0) && (B.A == 0)) { return A; }
            else if ((A.A == 0) && (B.A > 0)) { return B; }
            else if ((A.A > 0) && (B.A == 0)) { return A; }
            else {
                float AAlpha = (float)A.A / 255.0f;
                float BAlpha = (float)B.A / 255.0f;
                float ar = AAlpha + BAlpha - (AAlpha * BAlpha);
                float asr = BAlpha / ar;
                float a1 = 1 - asr;
                float a2 = asr * (1 - AAlpha);
                float ab = asr * AAlpha;
                byte r = (byte)(A.R * a1 + B.R * a2 + B.R * ab);
                byte g = (byte)(A.G * a1 + B.G * a2 + B.G * ab);
                byte b = (byte)(A.B * a1 + B.B * a2 + B.B * ab);
                return Color.FromArgb((byte)(ar * 255), r, g, b);
            }
        }
        private void DrawStraightFill(PaintEventArgs e, Rectangle ObjectBounds, Color North, Color South) {
            bool AreSame = ColorsSame(North, South);
            if (AreSame) {
                using (SolidBrush Br = new SolidBrush(North)) {
                    e.Graphics.FillRectangle(Br, ObjectBounds);
                }
            }
            else {
                using (LinearGradientBrush Br = new LinearGradientBrush(ObjectBounds, North, South, LinearGradientMode.Vertical)) {
                    e.Graphics.FillRectangle(Br, ObjectBounds);
                }
            }
        }
        private void DrawRoundedFill(PaintEventArgs e, Rectangle ObjectBounds, GraphicsPath Path, Color North, Color South) {
            bool AreSame = ColorsSame(North, South);
            if (AreSame) {
                using (SolidBrush Br = new SolidBrush(North)) {
                    e.Graphics.FillPath(Br, Path);
                }
            }
            else {
                using (LinearGradientBrush Br = new LinearGradientBrush(ObjectBounds, North, South, LinearGradientMode.Vertical)) {
                    e.Graphics.FillPath(Br, Path);
                }
            }
        }
        private void DrawStraightBorder(PaintEventArgs e, Rectangle ObjectBounds, Color North, Color South, int PenSizing = 1) {
            bool AreSame = ColorsSame(North, South);
            if (AreSame) {
                using (SolidBrush Br = new SolidBrush(North)) {
                    using (Pen Pn = new Pen(Br, PenSizing)) {
                        if (PenSizing == 1) {
                            Rectangle Rect = new Rectangle(ObjectBounds.X, ObjectBounds.Y, ObjectBounds.Width - 1, ObjectBounds.Height - 1);
                            e.Graphics.DrawRectangle(Pn, Rect);
                        }
                        else {
                            Pn.Alignment = PenAlignment.Inset;
                            e.Graphics.DrawRectangle(Pn, ObjectBounds);
                        }
                    }
                }
            }
            else {
                using (LinearGradientBrush Br = new LinearGradientBrush(ObjectBounds, North, South, LinearGradientMode.Vertical)) {
                    using (Pen Pn = new Pen(Br, PenSizing)) {
                        if (PenSizing == 1) {
                            Rectangle Rect = new Rectangle(ObjectBounds.X, ObjectBounds.Y, ObjectBounds.Width - 1, ObjectBounds.Height - 1);
                            e.Graphics.DrawRectangle(Pn, Rect);
                        }
                        else {
                            Pn.Alignment = PenAlignment.Inset;
                            e.Graphics.DrawRectangle(Pn, ObjectBounds);
                        }
                    }
                }
            }
        }
        private void DrawShadowBorder(PaintEventArgs e, Rectangle ObjectBounds) {
            int ShadowWidth = 5;
            int ShadowWidthContracted = ShadowWidth - 1;

            using (LinearGradientBrush NthBr = new LinearGradientBrush(GetDockedRectangle(ObjectBounds, SideDocks.North, ShadowWidth), BorderColorShadow, Color.Transparent, LinearGradientMode.Vertical)) {
                e.Graphics.FillRectangle(NthBr, GetDockedRectangle(ObjectBounds, SideDocks.North, ShadowWidthContracted));
            }
            using (LinearGradientBrush SthBr = new LinearGradientBrush(GetDockedRectangle(ObjectBounds, SideDocks.South, ShadowWidth), Color.Transparent, BorderColorShadow, LinearGradientMode.Vertical)) {
                e.Graphics.FillRectangle(SthBr, GetDockedRectangle(ObjectBounds, SideDocks.South, ShadowWidthContracted));
            }
            using (LinearGradientBrush WstBr = new LinearGradientBrush(GetDockedRectangle(ObjectBounds, SideDocks.West, ShadowWidth), BorderColorShadow, Color.Transparent, LinearGradientMode.Horizontal)) {
                e.Graphics.FillRectangle(WstBr, GetDockedRectangle(ObjectBounds, SideDocks.West, ShadowWidthContracted));
            }
            using (LinearGradientBrush EstBr = new LinearGradientBrush(GetDockedRectangle(ObjectBounds, SideDocks.East, ShadowWidth), Color.Transparent, BorderColorShadow, LinearGradientMode.Horizontal)) {
                e.Graphics.FillRectangle(EstBr, GetDockedRectangle(ObjectBounds, SideDocks.East, ShadowWidthContracted));
            }
        }
        private void DrawRoundedBorder(PaintEventArgs e, Rectangle ObjectBounds, GraphicsPath Path, Color North, Color South, int PenSizing = 1) {
            bool AreSame = ColorsSame(North, South);
            if (AreSame) {
                using (SolidBrush Br = new SolidBrush(North)) {
                    using (Pen Pn = new Pen(Br, PenSizing)) {
                        Pn.Alignment = PenAlignment.Center;
                        e.Graphics.DrawPath(Pn, Path);
                    }
                }
            }
            else {
                using (LinearGradientBrush Br = new LinearGradientBrush(ObjectBounds, North, South, LinearGradientMode.Vertical)) {
                    using (Pen Pn = new Pen(Br, PenSizing)) {
                        Pn.Alignment = PenAlignment.Center;
                        e.Graphics.DrawPath(Pn, Path);
                    }
                }
            }
        }
        private Rectangle GetDockedRectangle(Rectangle ObjectBounds, SideDocks Docker, int Thickness) {
            switch (Docker) {
                case SideDocks.North:
                    return new Rectangle(ObjectBounds.X, ObjectBounds.Y, ObjectBounds.Width, Thickness);
                case SideDocks.South:
                    return new Rectangle(ObjectBounds.X, ObjectBounds.Y + ObjectBounds.Height - Thickness, ObjectBounds.Width, Thickness);
                case SideDocks.East:
                    return new Rectangle(ObjectBounds.X + ObjectBounds.Width - Thickness, ObjectBounds.Y, Thickness, ObjectBounds.Height);
                case SideDocks.West:
                    return new Rectangle(ObjectBounds.X, ObjectBounds.Y, Thickness, ObjectBounds.Height);
                default:
                    return ObjectBounds;
            }
        }
        private bool ColorsSame(Color Input1, Color Input2) {
            if (Input1.ToArgb() == Input2.ToArgb()) { return true; }
            return false;
        }
        MouseStates MouseState = MouseStates.Exited;

        public Button() {
            DoubleBuffered = true;
            MouseClick += Button_OnMouseClick;
            MouseDown += Button_OnMouseDown;
            MouseUp += Button_OnMouseUp;
            MouseMove += Button_OnMouseMove;
            MouseDoubleClick += Button_OnMouseDoubleClick;
            MouseHover += Button_OnMouseHover;
            MouseEnter += Button_OnMouseEnter;
            MouseLeave += Button_OnMouseLeave;
            secondaryFont = this.Font;
        }

        private void Button_OnMouseDoubleClick(object? sender, MouseEventArgs e) {

            //MouseState = MouseStates.Down; Invalidate();
        }
        private void Button_OnMouseClick(object? sender, MouseEventArgs e) {

            if (type == ButtonType.Checkbox) {
                Checked = !Checked;
            }
            else if (type == ButtonType.CheckBoxLimited) {
                if (allowGroupUnchecking == true) {
                    Checked = !Checked;
                }
                else { Checked = true; }
            }
            else if (type == ButtonType.RadioButton) {
                Checked = true;
            }
            MouseState = MouseStates.Down; Invalidate();
            ButtonClicked?.Invoke(this);
        }

        private void Button_OnMouseEnter(object? sender, EventArgs e) {
            MouseState = MouseStates.Hover; Invalidate();
        }

        private void Button_OnMouseLeave(object? sender, EventArgs e) {
            IsMouseDown = false;
            MouseState = MouseStates.Exited; Invalidate();
        }

        private void Button_OnMouseHover(object? sender, EventArgs e) {
            MouseState = MouseStates.Hover; Invalidate();
        }
        bool IsMouseDown = false;
        private void Button_OnMouseMove(object? sender, MouseEventArgs e) {
            if (IsMouseDown == false) {
                MouseState = MouseStates.Hover;
            }
            Invalidate();
        }

        private void Button_OnMouseUp(object? sender, MouseEventArgs e) {
            IsMouseDown = false;
            MouseState = MouseStates.Hover; Invalidate();
        }

        private void Button_OnMouseDown(object? sender, MouseEventArgs e) {
            IsMouseDown = true;
            MouseState = MouseStates.Down; Invalidate();
        }
        private enum MouseStates {
            Exited = 0x00,
            Hover = 0x01,
            Down = 0x02
        }
        private enum SideDocks {
            North = 0x00,
            East = 0x01,
            South = 0x02,
            West = 0x03
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // Button
            // 
            this.Name = "Button";
            this.Size = new System.Drawing.Size(160, 61);
            this.Load += new System.EventHandler(this.Button_Load);
            this.ResumeLayout(false);

        }

        private void Button_Load(object? sender, EventArgs e) {

        }
    }
    public enum ButtonTextVertical {
        Top = 0x00,
        Middle = 0x01,
        Bottom = 0x02
    }
    public enum ButtonTextHorizontal {
        Left = 0x00,
        Center = 0x01,
        Right = 0x02
    }
    public enum ButtonType {
        Button = 0x00,
        Checkbox = 0x01,
        CheckBoxLimited = 0x04,
        RadioButton = 0x02
    }
    public enum ButtonStyle {
        Square = 0x00,
        Round = 0x01,
        SquareNoBorder = 0x02,
        RoundNoBorder = 0x03,
        ShadowSquare = 0x04,
        Text = 0x05
    }
}
