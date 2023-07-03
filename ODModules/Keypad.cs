using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ODModules {
    public class Keypad : UserControl {
        [Category("Buttons")]
        public event ButtonClickedEventHandler? ButtonClicked;

        public delegate void ButtonClickedEventHandler(object? Sender, KeypadButton Button, Point GridLocation);
        public Keypad() {
            DoubleBuffered = true;
            MouseClick += Keypad_MouseClick;
            MouseDown += Keypad_MouseDown;
            MouseMove += Keypad_MouseMove;
            MouseUp += Keypad_MouseUp;
            MouseLeave += Keypad_MouseLeave;
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
        Padding buttonPadding = Padding.Empty;
        [System.ComponentModel.Category("Appearance")]
        public Padding ButtonPadding {
            get { return buttonPadding; }
            set {
                buttonPadding = value;
                Invalidate();
            }
        }

        ButtonStyle style = ButtonStyle.Square;
        [System.ComponentModel.Category("Appearance")]
        public ButtonStyle Style {
            get { return style; }
            set {
                style = value;
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

        private List<KeypadButton> buttons = new List<KeypadButton>();
        [System.ComponentModel.Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<KeypadButton> Buttons {
            get { return buttons; }
        }
        int rows = 2;
        [System.ComponentModel.Category("Buttons")]
        public int Rows {
            get { return rows; }
            set {
                if (value < 0) {
                    rows = 1;
                }
                else {
                    rows = value;
                }
            }
        }
        int columns = 2;
        [System.ComponentModel.Category("Buttons")]
        public int Columns {
            get { return columns; }
            set {
                if (value < 0) {
                    columns = 1;
                }
                else {
                    columns = value;
                }
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
        Size imageSize = new Size(32, 32);
        [System.ComponentModel.Category("Appearance")]
        public Size ImageSize {
            get { return imageSize; }
            set {
                imageSize = value;
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

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int k = 0;
            int ButtonWidth = (Width - Padding.Left - Padding.Right) / columns;
            int ButtonHeight = (Height - Padding.Top - Padding.Bottom) / rows;
            int XDefault = Padding.Left;
            int XOffset = XDefault;
            int YOffset = Padding.Top;
            for (int j = 0; j < rows; j++) {
                XOffset = XDefault;
                for (int i = 0; i < columns; i++) {
                    if (buttons.Count > 0) {
                        if (k < buttons.Count) {
                            Rectangle CurrentButton = new Rectangle(XOffset, YOffset, ButtonWidth, ButtonHeight);
                            CurrentButton = ReadjustRectangle(CurrentButton);
                            DrawButton(e, CurrentButton, buttons[k]);
                            k++;
                        }
                    }
                    XOffset += ButtonWidth;
                }
                YOffset += ButtonHeight;
            }

            base.OnPaint(e);
        }
        private Rectangle ReadjustRectangle(Rectangle rect) {
            int WidthAdj = rect.Width - (buttonPadding.Left + buttonPadding.Right);
            int HeightAdj = rect.Height - (buttonPadding.Top + buttonPadding.Bottom);
            return new Rectangle(rect.X + buttonPadding.Left, rect.Y + buttonPadding.Top, WidthAdj, HeightAdj);
        }

        private void DrawButton(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn) {
            Color BackNorth = Color.Black;
            Color BackSouth = Color.Black;

            Color BorderNorth = Color.Black;
            Color BorderSouth = Color.Black;

            Rectangle ButtonFill = ButtonBounds;
            Rectangle ButtonRound = ButtonBounds;
            MouseStates ButtonState = GetState(ButtonBounds);
            GetBackColors(ButtonState, Btn, out BackNorth, out BackSouth);
            GetBorderColors(ButtonState, Btn, out BorderNorth, out BorderSouth);

            switch (style) {
                case ButtonStyle.Square:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    DrawStraightBorder(e, ButtonBounds, BorderNorth, BorderSouth);
                    break;
                case ButtonStyle.Round:
                    ButtonRound = new Rectangle(ButtonBounds.X + 2, ButtonBounds.Y + 2, ButtonBounds.Width - 4, ButtonBounds.Height - 4);
                    using (GraphicsPath GrPath = RoundedRectangle(ButtonRound, borderRadius)) {
                        DrawRoundedFill(e, ButtonFill, GrPath, BackNorth, BackSouth);
                        DrawRoundedBorder(e, ButtonFill, GrPath, BorderNorth, BorderSouth);
                    }
                    break;
                case ButtonStyle.SquareNoBorder:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    break;
                case ButtonStyle.RoundNoBorder:
                    ButtonBounds = new Rectangle(ButtonBounds.X, ButtonBounds.Y, ButtonBounds.Width - 4, ButtonBounds.Height - 4);
                    using (GraphicsPath GrPath = RoundedRectangle(ButtonBounds, borderRadius)) {
                        DrawRoundedFill(e, ButtonFill, GrPath, BackNorth, BackSouth);
                    }
                    break;
                case ButtonStyle.ShadowSquare:
                    DrawStraightFill(e, ButtonBounds, BackNorth, BackSouth);
                    DrawShadowBorder(e, ButtonBounds);
                    break;
            }
            DrawText(e, ButtonBounds, Btn);
        }
        private Rectangle InsetRectangle(Rectangle Input, int Amount) {
            int SizeW = Amount * 2;
            return new Rectangle(Input.X + Amount, Input.Y + Amount, Input.Width - SizeW, Input.Height - SizeW);
        }
        private void DrawText(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn) {
            bool HasPrimary = Btn.Text.Length > 0;
            bool HasSecondary = Btn.SecondaryText.Length > 0;
            Rectangle InsetRect = InsetRectangle(ButtonBounds, 5);
            SizeF PrimarySize = new Size(0, 0);
            SizeF SecondarySize = new Size(0, 0);
            int TotalSize = 0;
            if (HasPrimary) {
                PrimarySize = e.Graphics.MeasureString(Btn.Text, Font, InsetRect.Width);
                TotalSize = (int)PrimarySize.Height;
            }
            if (HasSecondary) {
                if (SecondaryFont != null) {
                    SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, SecondaryFont, InsetRect.Width);
                }
                else {
                    SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, Font, InsetRect.Width);
                }
                TotalSize += (int)SecondarySize.Height;
            }
            if (Btn.Icon != null) {
                if (Btn.IconInline == false) {
                    TotalSize += imageSize.Height;
                }
            }
            int RunningHeight = InsetRect.Y;
            if (Btn.Icon != null) {
                if (Btn.IconInline == true) {

                    // if (Btn.TextVerticalAlignment == ButtonTextVertical.Top) {
                    //    DrawImage(e, InsetRect, Btn, 0);
                    //}
                    //else if (Btn.TextVerticalAlignment == ButtonTextVertical.Middle) {
                    //    int YCentre = (InsetRect.Height - (TotalSize + 5)) / 2;
                    //    RunningHeight += YCentre;
                    //    RunningHeight += DrawImage(e, InsetRect, Btn, YCentre);
                    //}
                    //else if (Btn.TextVerticalAlignment == ButtonTextVertical.Bottom) {
                    //    int YCentre = (InsetRect.Height - (TotalSize + 5));
                    //    RunningHeight += YCentre;
                    //    RunningHeight += DrawImage(e, InsetRect, Btn, YCentre);
                    //}
                }
                else {
                    if (Btn.TextVerticalAlignment == ButtonTextVertical.Top) {
                        DrawImage(e, InsetRect, Btn, 0);
                    }
                    else if (Btn.TextVerticalAlignment == ButtonTextVertical.Middle) {
                        int YCentre = (InsetRect.Height - (TotalSize + 5)) / 2;
                        RunningHeight += YCentre;
                        RunningHeight += DrawImage(e, InsetRect, Btn, YCentre);
                    }
                    else if (Btn.TextVerticalAlignment == ButtonTextVertical.Bottom) {
                        int YCentre = (InsetRect.Height - (TotalSize + 5));
                        RunningHeight += YCentre;
                        RunningHeight += DrawImage(e, InsetRect, Btn, YCentre);
                    }
                    RunningHeight += 5;
                }
            }
            if (Btn.IconInline == false) {
                using (StringFormat PriStrFrmt = new StringFormat()) {
                    if (Btn.TextHorizontalAlignment == ButtonTextHorizontal.Center) {
                        PriStrFrmt.Alignment = StringAlignment.Center;
                    }
                    else if (Btn.TextHorizontalAlignment == ButtonTextHorizontal.Right) {
                        PriStrFrmt.Alignment = StringAlignment.Far;
                    }
                    else {
                        PriStrFrmt.Alignment = StringAlignment.Near;
                    }
                    PriStrFrmt.Trimming = StringTrimming.Character;
                    if (Btn.TextVerticalAlignment == ButtonTextVertical.Middle) {
                        int YCentre = (InsetRect.Height - (TotalSize + 5)) / 2;
                        RunningHeight += YCentre;
                    }
                    else if (Btn.TextVerticalAlignment == ButtonTextVertical.Bottom) {
                        int YCentre = (InsetRect.Height - (TotalSize + 5));
                        RunningHeight += YCentre;
                    }
                    if (HasPrimary) {
                        using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Btn.Text, Font, TxtBr, new RectangleF(InsetRect.X, RunningHeight, InsetRect.Width, PrimarySize.Height), PriStrFrmt);
                        }
                        RunningHeight += (int)PrimarySize.Height;
                    }
                    if (HasSecondary) {
                        Font TempFont = Font;
                        if (secondaryFont != null) {
                            TempFont = secondaryFont;
                        }
                        using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Btn.SecondaryText, TempFont, TxtBr, new RectangleF(InsetRect.X, RunningHeight, InsetRect.Width, SecondarySize.Height), PriStrFrmt);
                        }
                        RunningHeight += (int)PrimarySize.Height;
                    }
                }
            }
        }
        private int DrawImage(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn, int Offset) {
            if (Btn.Icon == null) { return 0; }
            if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Left) {
                Point IconLocation = new Point(ButtonBounds.X, ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Center) {
                Point IconLocation = new Point(ButtonBounds.X + ((ButtonBounds.Width - imageSize.Width) / 2), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Right) {
                Point IconLocation = new Point(ButtonBounds.X + (ButtonBounds.Width - imageSize.Width), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            return imageSize.Height;
        }
        private int DrawImageInline(PaintEventArgs e, Rectangle ButtonBounds, GridButton Btn, int Offset) {
            if (Btn.Icon == null) { return 0; }
            if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Left) {
                Point IconLocation = new Point(ButtonBounds.X, ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Center) {
                Point IconLocation = new Point(ButtonBounds.X + ((ButtonBounds.Width - imageSize.Width) / 2), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Right) {
                Point IconLocation = new Point(ButtonBounds.X + (ButtonBounds.Width - imageSize.Width), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            return imageSize.Height;
        }
        private enum MouseStates {
            Exited = 0x00,
            Hover = 0x01,
            Down = 0x02
        }
        private MouseStates GetState(Rectangle ButtonBounds) {
            if (ButtonBounds.Contains(CurrentPosition)) {
                if (IsMouseDown == true) {
                    return MouseStates.Down;
                }
                return MouseStates.Hover;
            }
            return MouseStates.Exited;
        }
        private void GetBackColors(MouseStates MouseState, KeypadButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBack = backColorNorth;
            Color TempSouthBack = backColorSouth;
            if (Btn.UseCustomColors == true) {
                TempNorthBack = Btn.BackColorNorth;
                TempSouthBack = Btn.BackColorSouth;
            }

            NorthColor = TempNorthBack; SouthColor = TempSouthBack;
            if (Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
                        if (Btn.Type != ButtonType.Button) {
                            if (Btn.Checked == true) {
                                if (IsMouseDown != true) {
                                    NorthColor = AddColors(TempNorthBack, backColorCheckedNorth);
                                    SouthColor = AddColors(TempSouthBack, backColorCheckedSouth);
                                }
                                else {
                                    NorthColor = AddColors(TempNorthBack, backColorDownNorth);
                                    SouthColor = AddColors(TempSouthBack, backColorDownSouth);
                                }
                            }
                        }
                        break;
                    case MouseStates.Hover:
                        NorthColor = AddColors(TempNorthBack, backColorHoverNorth);
                        SouthColor = AddColors(TempSouthBack, backColorHoverSouth); break;
                    case MouseStates.Down:
                        NorthColor = AddColors(TempNorthBack, backColorDownNorth);
                        SouthColor = AddColors(TempSouthBack, backColorDownSouth); break;
                    default:
                        break;
                }
            }
            else {
                NorthColor = WhiteLightenColor(TempNorthBack, 64);
                SouthColor = WhiteLightenColor(TempSouthBack, 64);
            }
        }
        private void GetBorderColors(MouseStates MouseState, KeypadButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBorder = borderColorNorth;
            Color TempSouthBorder = borderColorSouth;
            if (Btn.UseCustomColors == true) {
                TempNorthBorder = Btn.BorderColorNorth;
                TempSouthBorder = Btn.BorderColorSouth;
            }
            NorthColor = TempNorthBorder; SouthColor = TempSouthBorder;
            if (Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
                        if (Btn.Type != ButtonType.Button) {
                            if (Btn.Checked == true) {
                                if (IsMouseDown != true) {
                                    NorthColor = AddColors(TempNorthBorder, borderColorCheckedNorth);
                                    SouthColor = AddColors(TempSouthBorder, borderColorCheckedSouth);
                                }
                                else {
                                    NorthColor = AddColors(TempNorthBorder, borderColorDownNorth);
                                    SouthColor = AddColors(TempSouthBorder, borderColorDownSouth);
                                }
                            }
                        }
                        break;
                    case MouseStates.Hover:
                        NorthColor = AddColors(TempNorthBorder, borderColorHoverNorth);
                        SouthColor = AddColors(TempSouthBorder, borderColorHoverSouth); break;
                    case MouseStates.Down:
                        NorthColor = AddColors(TempNorthBorder, borderColorDownNorth);
                        SouthColor = AddColors(TempSouthBorder, borderColorDownSouth); break;
                    default:
                        break;
                }
            }
            else {
                NorthColor = WhiteLightenColor(TempNorthBorder, 64);
                SouthColor = WhiteLightenColor(TempSouthBorder, 64);
            }
        }
        private Color WhiteLightenColor(Color Input, int Alpha) {
            int AR = Convert.ToInt32(Math.Floor(Input.R + Alpha * ((255 - Input.R) / (double)255)));
            int AG = Convert.ToInt32(Math.Floor(Input.G + Alpha * ((255 - Input.G) / (double)255)));
            int AB = Convert.ToInt32(Math.Floor(Input.B + Alpha * ((255 - Input.B) / (double)255)));
            return Color.FromArgb(AR, AG, AB);
        }
        private Color AddColors(Color A, Color B) {
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
        bool IsMouseDown = false;
        Point CurrentPosition = new Point(-1, -1);
        private void Keypad_MouseLeave(object? sender, EventArgs e) {
            CurrentPosition = new Point(-1, -1);
            Invalidate();
        }
        private void Keypad_MouseUp(object? sender, MouseEventArgs e) {
            IsMouseDown = false;
            Invalidate();
        }
        private void Keypad_MouseMove(object? sender, MouseEventArgs e) {
            CurrentPosition = e.Location;
            Invalidate();
        }
        private void Keypad_MouseDown(object? sender, MouseEventArgs e) {
            IsMouseDown = true;
            Invalidate();
        }
        private bool IsPointReset(Point Input) {
            if ((Input.X < 0) || (Input.Y < 0)) { return true; }
            return false;
        }
        private void Keypad_MouseClick(object? sender, MouseEventArgs e) {
            if (IsPointReset(e.Location) == false) {
                ClickedKeypadButtonResult Result = GetButtonPosition(e.Location);
                if (Result.Button != null) {
                   // ResetAllothers(Result.Button);
                    CurrentPosition = e.Location;
                    IsMouseDown = true;
                    Invalidate();
                    ButtonClicked?.Invoke(this, Result.Button, Result.Position);
                }
            }
        }

        private ClickedKeypadButtonResult GetButtonPosition(Point SelectedPoint) {
            Point Output = new Point(-1, -1);
            int ButtonIndex = -1;
            int k = 0;
            int ButtonWidth = (Width - Padding.Left - Padding.Right) / columns;
            int ButtonHeight = (Height - Padding.Top - Padding.Bottom) / rows;
            int XDefault = Padding.Left;
            int XOffset = XDefault;
            int YOffset = Padding.Top;
            bool BreakAll = false;
            for (int j = 0; j < rows; j++) {
                XOffset = XDefault;
                for (int i = 0; i < columns; i++) {
                    if (buttons.Count > 0) {
                        if (k < buttons.Count) {
                            Rectangle CurrentButton = new Rectangle(XOffset, YOffset, ButtonWidth, ButtonHeight);
                            CurrentButton = ReadjustRectangle(CurrentButton);
                            if (CurrentButton.Contains(SelectedPoint)) {
                                Output = new Point(i, j);
                                ButtonIndex = k;
                                BreakAll = true;
                                break;
                            }
                            k++;
                        }
                    }
                    XOffset += ButtonWidth;
                }
                YOffset += ButtonHeight;
                if (BreakAll == true) { break; }
            }

            KeypadButton? Btn = null;
            if (ButtonIndex != -1) {
                Btn = buttons[ButtonIndex];
            }
            return new ClickedKeypadButtonResult(Btn, Output, ButtonIndex);
        }

        protected override void OnResize(EventArgs e) {
            Invalidate();
            base.OnResize(e);
        }

        private enum SideDocks {
            North = 0x00,
            East = 0x01,
            South = 0x02,
            West = 0x03
        }
    }
    public class ClickedKeypadButtonResult {
        KeypadButton? button = null;
        public KeypadButton? Button {
            get { return button; }
        }
        int index = -1;
        public int Index {
            get { return index; }
        }
        Point position = new Point(-1, -1);
        public Point Position {
            get { return position; }
        }
        public ClickedKeypadButtonResult(KeypadButton? Button, Point Position, int Index) {
            this.button = Button;
            this.index = Index;
            this.position = Position;
        }
    }
    public class KeypadButton {
        #region Properties
        bool useCustomColors = false;
        public bool UseCustomColors {
            get {
                return useCustomColors;
            }
            set {
                useCustomColors = value;
            }
        }
        bool iconInline = false;
        [System.ComponentModel.Category("Appearance")]
        public bool IconInline {
            get {
                return iconInline;
            }
            set {
                iconInline = value;
            }
        }
        Image? icon = null;
        [System.ComponentModel.Category("Appearance")]
        public Image? Icon {
            get {
                return icon;
            }
            set {
                icon = value;
            }
        }
        bool enabled = false;
        [System.ComponentModel.Category("Appearance")]
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                enabled = value;
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
            }
        }
        ButtonTextHorizontal imageHorizontalAlignment = ButtonTextHorizontal.Center;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextHorizontal ImageHorizontalAlignment {
            get { return imageHorizontalAlignment; }
            set {
                imageHorizontalAlignment = value;
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
            }
        }
        bool isChecked = false;
        [System.ComponentModel.Category("Control")]
        public bool Checked {
            get {
                return isChecked;
            }
            set {
                isChecked = value;
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        string text = "";
        [System.ComponentModel.Category("Appearance")]
        public string Text {
            get { return text; }
            set {
                text = value;
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
            }
        }
        Point position = new Point(0, 0);
        [System.ComponentModel.Category("Position")]
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
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
            }
        }
        ButtonTextHorizontal textHorizontalAlignment = ButtonTextHorizontal.Center;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextHorizontal TextHorizontalAlignment {
            get { return textHorizontalAlignment; }
            set {
                textHorizontalAlignment = value;
            }
        }
        ButtonTextVertical textVerticalAlignment = ButtonTextVertical.Middle;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextVertical TextVerticalAlignment {
            get { return textVerticalAlignment; }
            set {
                textVerticalAlignment = value;
            }
        }




        #endregion
    }
}
