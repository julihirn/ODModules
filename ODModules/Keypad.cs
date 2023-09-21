using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;

namespace ODModules {
    public class Keypad : UserControl {
        [Category("Buttons")]
        public event ButtonClickedEventHandler? ButtonRightClicked;
        [Category("Buttons")]
        public event ButtonClickedEventHandler? ButtonClicked;

        public delegate void ButtonClickedEventHandler(object? Sender, KeypadButton Button, Point GridLocation, int Index);
        public Keypad() {
            DoubleBuffered = true;
            MouseClick += Keypad_MouseClick;
            MouseDown += Keypad_MouseDown;
            MouseMove += Keypad_MouseMove;
            MouseUp += Keypad_MouseUp;
            MouseLeave += Keypad_MouseLeave;
            KeyUp += ButtonGrid_KeyUp;
            KeyDown += ButtonGrid_KeyDown;
            this.SetStyle(ControlStyles.Selectable, true);
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
        private List<KeypadButton>? externalItems = null;
        [System.ComponentModel.Category("Buttons")]
        public List<KeypadButton>? ExternalItems {
            get {
                return externalItems;
            }
            set {
                externalItems = value;
                Invalidate();
            }
        }
        private bool useLocalList = true;
        [System.ComponentModel.Category("Buttons")]
        public bool UseLocalList {
            get {
                return useLocalList;
            }
            set {
                useLocalList = value;
            }
        }
        private List<KeypadButton> CurrentButtons {
            get {
                if (useLocalList == true) {
                    return buttons;
                }
                else {
                    if (externalItems == null) {
                        return buttons;
                    }
                    else {
                        return externalItems;
                    }
                }
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
        Color borderColorDisabledNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorDisabledNorth {
            get {
                return borderColorDisabledNorth;
            }
            set {
                borderColorDisabledNorth = value;
                Invalidate();
            }
        }
        Color borderColorDisabledSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorDisabledSouth {
            get {
                return borderColorDisabledSouth;
            }
            set {
                borderColorDisabledSouth = value;
                Invalidate();
            }
        }

        Color backColorDisabledNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorDisabledNorth {
            get {
                return backColorDisabledNorth;
            }
            set {
                backColorDisabledNorth = value;
                Invalidate();
            }
        }
        Color backColorDisabledSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorDisabledSouth {
            get {
                return backColorDisabledSouth;
            }
            set {
                backColorDisabledSouth = value;
                Invalidate();
            }
        }

        Color backColorMarkedNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorMarkedNorth {
            get {
                return backColorMarkedNorth;
            }
            set {
                backColorMarkedNorth = value;
                Invalidate();
            }
        }
        Color backColorMarkedSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorMarkedSouth {
            get {
                return backColorMarkedSouth;
            }
            set {
                backColorMarkedSouth = value;
                Invalidate();
            }
        }

        Color borderColorMarkedNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorMarkedNorth {
            get {
                return borderColorMarkedNorth;
            }
            set {
                borderColorMarkedNorth = value;
                Invalidate();
            }
        }
        Color borderColorMarkedSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColorMarkedSouth {
            get {
                return borderColorMarkedSouth;
            }
            set {
                borderColorMarkedSouth = value;
                Invalidate();
            }
        }
        int markedIndex = -1;
        [System.ComponentModel.Category("Appearance")]
        public int MarkedIndex {
            get {
                return markedIndex;
            }
            set {
                markedIndex = value;
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
        bool iconInline = false;
        [System.ComponentModel.Category("Appearance")]
        public bool IconInline {
            get {
                return iconInline;
            }
            set {
                iconInline = value;
                Invalidate();
            }
        }
        private bool allowTextWrapping = true;
        [System.ComponentModel.Category("Appearance")]
        public bool AllowTextWrapping {
            get {
                return allowTextWrapping;
            }
            set {
                allowTextWrapping = value;
                Invalidate();
            }
        }
        ButtonTextHorizontal imageHorizontalAlignment = ButtonTextHorizontal.Center;
        [System.ComponentModel.Category("Appearance")]
        public ButtonTextHorizontal ImageHorizontalAlignment {
            get { return imageHorizontalAlignment; }
            set {
                imageHorizontalAlignment = value;
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
                    if (CurrentButtons.Count > 0) {
                        if (k < CurrentButtons.Count) {
                            Rectangle CurrentButton = new Rectangle(XOffset, YOffset, ButtonWidth, ButtonHeight);
                            CurrentButton = ReadjustRectangle(CurrentButton);
                            DrawButton(e, CurrentButton, CurrentButtons[k], k);
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

        private void DrawButton(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn, int Index) {
            Color BackNorth = Color.Black;
            Color BackSouth = Color.Black;

            Color BorderNorth = Color.Black;
            Color BorderSouth = Color.Black;

            Rectangle ButtonFill = ButtonBounds;
            Rectangle ButtonRound = ButtonBounds;
            MouseStates ButtonState = GetState(ButtonBounds, Index);
            GetBackColors(ButtonState, Index, Btn, out BackNorth, out BackSouth);
            GetBorderColors(ButtonState, Index, Btn, out BorderNorth, out BorderSouth);

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
        private int DrawImage(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn, int Offset) {
            if (Btn.Icon == null) { return 0; }
            if (GetButtonImageHorizontalAlignment(Btn) == ButtonTextHorizontal.Left) {
                Point IconLocation = new Point(ButtonBounds.X, ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (GetButtonImageHorizontalAlignment(Btn) == ButtonTextHorizontal.Center) {
                Point IconLocation = new Point(ButtonBounds.X + ((ButtonBounds.Width - imageSize.Width) / 2), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else if (GetButtonImageHorizontalAlignment(Btn) == ButtonTextHorizontal.Right) {
                Point IconLocation = new Point(ButtonBounds.X + (ButtonBounds.Width - imageSize.Width), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            return imageSize.Height;
        }
        private int DrawImageInline(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn, int Offset) {
            if (Btn.Icon == null) { return 0; }
            if (Btn.ImageHorizontalAlignment == ButtonTextHorizontal.Right) {
                Point IconLocation = new Point(ButtonBounds.X + (ButtonBounds.Width - imageSize.Width - 2), ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            else {
                Point IconLocation = new Point(ButtonBounds.X + 2, ButtonBounds.Y + Offset);
                e.Graphics.DrawImage(Btn.Icon, new Rectangle(IconLocation, imageSize));
            }
            return imageSize.Width + 2;
        }
        private void DrawText(PaintEventArgs e, Rectangle ButtonBounds, KeypadButton Btn) {
            bool HasPrimary = Btn.Text.Length > 0;
            bool HasSecondary = Btn.SecondaryText.Length > 0;
            Rectangle InsetRect = InsetRectangle(ButtonBounds, 5);
            SizeF PrimarySize = new Size(0, 0);
            SizeF SecondarySize = new Size(0, 0);
            float TotalSize = 0;
            float RunningHeight = InsetRect.Y;
            using (StringFormat PriStrFrmt = new StringFormat(StringFormat.GenericTypographic)) {
                PriStrFrmt.FormatFlags = StringFormatFlags.FitBlackBox;
                PriStrFrmt.Trimming = StringTrimming.EllipsisCharacter;
                if (GetButtonTextHorizontalAlignment(Btn) == ButtonTextHorizontal.Center) {
                    PriStrFrmt.Alignment = StringAlignment.Center;
                }
                else if (GetButtonTextHorizontalAlignment(Btn) == ButtonTextHorizontal.Right) {
                    PriStrFrmt.Alignment = StringAlignment.Far;
                }
                else {
                    PriStrFrmt.Alignment = StringAlignment.Near;
                }
                PriStrFrmt.Trimming = StringTrimming.Character;
                if (allowTextWrapping == false) {
                    PriStrFrmt.FormatFlags |= StringFormatFlags.NoWrap;
                }
                if (HasPrimary) {
                    PrimarySize = e.Graphics.MeasureString(Btn.Text, Font, InsetRect.Width, PriStrFrmt);
                    TotalSize = (int)PrimarySize.Height;
                }
                if (HasSecondary) {
                    if (SecondaryFont != null) {
                        SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, SecondaryFont, InsetRect.Width, PriStrFrmt);
                    }
                    else {
                        SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, Font, InsetRect.Width, PriStrFrmt);
                    }
                    if (InsetRect.Height > TotalSize + (int)SecondarySize.Height) {
                        TotalSize += (int)SecondarySize.Height;
                    }
                    else { HasSecondary = false; }
                }
                bool NoIcon = false;
                if (Btn.Icon != null) {
                    if (GetButtonInline(Btn) == false) {
                        if (InsetRect.Height > TotalSize + imageSize.Height) {
                            TotalSize += imageSize.Height;
                        }
                        else { NoIcon = true; }
                    }
                }
                if (Btn.Icon != null) {
                    if ((GetButtonInline(Btn) == false) && (NoIcon == false)) {
                        if (GetButtonTextVerticalAlignment(Btn) == ButtonTextVertical.Top) {
                            RunningHeight += DrawImage(e, InsetRect, Btn, 0);
                        }
                        else if (GetButtonTextVerticalAlignment(Btn) == ButtonTextVertical.Middle) {
                            float YCentre = (InsetRect.Height - TotalSize) / 2.0f;
                            RunningHeight += YCentre;
                            RunningHeight += DrawImage(e, InsetRect, Btn, (int)YCentre);
                        }
                        else if (GetButtonTextVerticalAlignment(Btn) == ButtonTextVertical.Bottom) {
                            float YCentre = (InsetRect.Height - TotalSize);
                            RunningHeight += YCentre;
                            RunningHeight += DrawImage(e, InsetRect, Btn, (int)YCentre);
                        }
                        RunningHeight += 5;
                    }
                }
                if (GetButtonInline(Btn) == false) {
                    if (GetButtonTextVerticalAlignment(Btn) == ButtonTextVertical.Middle) {
                        float YCentre = (InsetRect.Height - TotalSize) / 2.0f;
                        RunningHeight += YCentre;
                    }
                    else if (GetButtonTextVerticalAlignment(Btn) == ButtonTextVertical.Bottom) {
                        float YCentre = InsetRect.Height - TotalSize;
                        RunningHeight += YCentre;
                    }
                    if (HasPrimary) {
                        if (PrimarySize.Height > InsetRect.Height) {
                            PrimarySize = new SizeF(PrimarySize.Width, InsetRect.Height);

                        }
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
                    //}
                }
                else {
                    int YCentre = (InsetRect.Height - imageSize.Height) / 2;
                    int ImageSize = DrawImageInline(e, InsetRect, Btn, YCentre);
                    if (HasPrimary) {
                        PrimarySize = e.Graphics.MeasureString(Btn.Text, Font, InsetRect.Width - ImageSize, PriStrFrmt);
                        TotalSize = PrimarySize.Height;
                    }
                    if (HasSecondary) {
                        if (SecondaryFont != null) {
                            SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, SecondaryFont, InsetRect.Width - ImageSize, PriStrFrmt);
                        }
                        else {
                            SecondarySize = e.Graphics.MeasureString(Btn.SecondaryText, Font, InsetRect.Width - ImageSize, PriStrFrmt);
                        }
                        TotalSize += SecondarySize.Height;
                    }
                    RunningHeight += ((InsetRect.Height - TotalSize) / 2.0f);
                    Rectangle TextRectangle = new Rectangle(InsetRect.X + ImageSize, 0, InsetRect.Width - ImageSize, 0);
                    if (GetButtonImageHorizontalAlignment(Btn) == ButtonTextHorizontal.Right) {
                        TextRectangle = new Rectangle(InsetRect.X, (int)RunningHeight, InsetRect.Width - ImageSize, 0);
                    }
                    if (HasPrimary) {
                        using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Btn.Text, Font, TxtBr, new RectangleF(TextRectangle.X, RunningHeight, TextRectangle.Width, PrimarySize.Height), PriStrFrmt);
                        }
                        RunningHeight += PrimarySize.Height;
                    }
                    if (HasSecondary) {
                        Font TempFont = Font;
                        if (secondaryFont != null) {
                            TempFont = secondaryFont;
                        }
                        using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Btn.SecondaryText, TempFont, TxtBr, new RectangleF(TextRectangle.X, RunningHeight, TextRectangle.Width, SecondarySize.Height), PriStrFrmt);
                        }
                    }
                    //}
                }
            }
        }
        private ButtonTextVertical GetButtonTextVerticalAlignment(KeypadButton Btn) {
            if (Btn.UseButtonFormatting == true) {
                return Btn.TextVerticalAlignment;
            }
            else {
                return this.textVerticalAlignment;
            }
        }
        private ButtonTextHorizontal GetButtonTextHorizontalAlignment(KeypadButton Btn) {
            if (Btn.UseButtonFormatting == true) {
                return Btn.TextHorizontalAlignment;
            }
            else {
                return this.textHorizontalAlignment;
            }
        }
        private ButtonTextHorizontal GetButtonImageHorizontalAlignment(KeypadButton Btn) {
            if (Btn.UseButtonFormatting == true) {
                return Btn.ImageHorizontalAlignment;
            }
            else {
                return this.imageHorizontalAlignment;
            }
        }
        private bool GetButtonInline(KeypadButton Btn) {
            if (Btn.UseButtonFormatting == true) {
                return Btn.IconInline;
            }
            else {
                return this.iconInline;
            }
        }
        private enum MouseStates {
            Exited = 0x00,
            Hover = 0x01,
            Down = 0x02
        }
        private MouseStates GetState(Rectangle ButtonBounds, int ButtonIndex = -1) {
            if (KeyDownState == true) {
                if (ButtonIndex == DownIndex) {
                    return MouseStates.Down;
                }
                return MouseStates.Exited;
            }
            else {
                if (ButtonBounds.Contains(CurrentPosition)) {
                    if (IsMouseDown == true) {
                        return MouseStates.Down;
                    }
                    return MouseStates.Hover;
                }
                return MouseStates.Exited;
            }

        }
        private void GetBackColors(MouseStates MouseState, int Index, KeypadButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBack = backColorNorth;
            Color TempSouthBack = backColorSouth;
            if (Btn.UseCustomColors) {
                TempNorthBack = Btn.BackColorNorth;
                TempSouthBack = Btn.BackColorSouth;
            }
            if (Index == markedIndex) {
                TempNorthBack = backColorMarkedNorth;
                TempSouthBack = backColorMarkedSouth;
            }
            NorthColor = TempNorthBack; SouthColor = TempSouthBack;
            if (Btn.Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
                        if (Btn.Type != ButtonType.Button) {
                            if (Btn.Checked == true) {
                                // if (IsMouseDown != true) {
                                NorthColor = AddColors(TempNorthBack, backColorCheckedNorth);
                                SouthColor = AddColors(TempSouthBack, backColorCheckedSouth);
                                // }
                                // else {
                                //NorthColor = AddColors(TempNorthBack, backColorDownNorth);
                                //SouthColor = AddColors(TempSouthBack, backColorDownSouth);
                                //}
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
                NorthColor = backColorDisabledNorth;//WhiteLightenColor(TempNorthBack, 64);
                SouthColor = backColorDisabledSouth;// WhiteLightenColor(TempSouthBack, 64);
            }
        }
        private void GetBorderColors(MouseStates MouseState, int Index, KeypadButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBorder = borderColorNorth;
            Color TempSouthBorder = borderColorSouth;
            if (Btn.UseCustomColors) {
                TempNorthBorder = Btn.BorderColorNorth;
                TempSouthBorder = Btn.BorderColorSouth;
            }
            if (Index == markedIndex) {
                TempNorthBorder = borderColorMarkedNorth;
                TempSouthBorder = borderColorMarkedSouth;
            }
            NorthColor = TempNorthBorder; SouthColor = TempSouthBorder;
            if (Btn.Enabled == true) {
                switch (MouseState) {
                    case MouseStates.Exited:
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
                NorthColor = borderColorDisabledNorth;//WhiteLightenColor(TempNorthBorder, 64);
                SouthColor = borderColorDisabledSouth;//(TempSouthBorder, 64);
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
        Point DownLocation = new Point(0, 0);
        private void Keypad_MouseDown(object? sender, MouseEventArgs e) {
            IsMouseDown = true;
            CurrentPosition = e.Location;
            DownLocation = e.Location;
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
                    // 
                    if (Result.Button.Enabled == false) { return; }
                    if (e.Button == MouseButtons.Left) {
                        ResetAllothers(Result.Button);
                        CurrentPosition = e.Location;
                        IsMouseDown = true;
                        Invalidate();
                        ButtonClicked?.Invoke(this, Result.Button, Result.Position, Result.Index);
                    }
                    else if (e.Button == MouseButtons.Right) {
                        CurrentPosition = e.Location;
                        IsMouseDown = true;
                        Invalidate();
                        ButtonRightClicked?.Invoke(this, Result.Button, Result.Position, Result.Index);
                    }
                }
            }
        }
        private void ResetAllothers(KeypadButton ThisBtn) {
            if (ThisBtn.Type == ButtonType.RadioButton) {
                ThisBtn.Checked = true;
                if (ThisBtn.Checked == true) {
                    try {
                        foreach (KeypadButton Btn in buttons) {
                            if (Btn != ThisBtn) {
                                if (Btn.Type == ButtonType.RadioButton) {
                                    if (Btn.RadioButtonGroup == ThisBtn.RadioButtonGroup) { Btn.Checked = false; }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            else if (ThisBtn.Type == ButtonType.Checkbox) {
                ThisBtn.Checked = !ThisBtn.Checked;
            }
            else if (ThisBtn.Type == ButtonType.CheckBoxLimited) {
                //if (ThisBtn.Checked == true) {
                //    try {
                //        if (this.Parent != null) {
                //            List<ODModules.Button> Btns = new List<ODModules.Button>();
                //            int CheckedCount = 0;
                //            foreach (ODModules.Button Btn in this.Parent.Controls) {
                //                if (Btn != this) {
                //                    if (Btn.Type == ButtonType.CheckBoxLimited) {
                //                        if (Btn.RadioButtonGroup == this.RadioButtonGroup) {
                //                            if (Btn.Checked == true) {
                //                                Btns.Add(Btn);
                //                                CheckedCount++;
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //            Btns.Sort((x, y) => x.LastChecked.Ticks.CompareTo(y.LastChecked.Ticks));
                //            if (CheckedCount >= groupMaximumChecked) {
                //                int Diff = CheckedCount + 1 - groupMaximumChecked;
                //                for (int i = 0; i < Diff; i++) {
                //                    Btns[i].Checked = false;
                //                }
                //            }
                //        }
                //    }
                //    catch { }
                //}
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            //Debug.WriteLine("Process: " + keyData.ToString());
            // return base.ProcessCmdKey(ref msg, keyData);
            KeyEventArgs kea = new KeyEventArgs(keyData);
            ButtonGrid_KeyDown(this, kea);
            return true;
        }
        bool KeyDownState = false;
        int DownIndex = -1;
        private void ButtonGrid_KeyDown(object? sender, KeyEventArgs e) {

            int i = 0;
            foreach (KeypadButton Btn in CurrentButtons) {
                if (Btn.Enabled) {
                    if (Btn.ShortCutKeys == e.KeyData) {
                        ButtonClicked?.Invoke(this, Btn, new Point(0, 0), i);
                        KeyDownState = true;
                        DownIndex = i;
                        break;
                    }
                    else if (Btn.SecondaryShortCutKeys == e.KeyData) {
                        ButtonClicked?.Invoke(this, Btn, new Point(0, 0), i);
                        KeyDownState = true;
                        DownIndex = i;
                        break;
                    }
                }
                i++;
            }
            Invalidate();
        }
        private void ButtonGrid_KeyUp(object? sender, KeyEventArgs e) {
            KeyDownState = false;
            DownIndex = -1;
            Invalidate();
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
                    if (CurrentButtons.Count > 0) {
                        if (k < CurrentButtons.Count) {
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
                Btn = CurrentButtons[ButtonIndex];
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

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // Keypad
            // 
            this.Name = "Keypad";
            this.Load += new System.EventHandler(this.Keypad_Load);
            this.ResumeLayout(false);

        }

        private void Keypad_Load(object sender, EventArgs e) {

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
    public class KeypadButton : ArrayButton {
        public override string? ToString() {
            return Text;
        }
    }
}
