using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Handlers;

namespace ODModules {
    public class ButtonGrid : UserControl {
        public ButtonGrid() {
            DoubleBuffered = true;
            MouseClick += ButtonGrid_MouseClick;
            MouseDown += ButtonGrid_MouseDown;
            MouseMove += ButtonGrid_MouseMove;
            MouseUp += ButtonGrid_MouseUp;
            MouseLeave += ButtonGrid_MouseLeave;
            MouseWheel += ButtonGrid_MouseWheel;
            Resize += ButtonGrid_Resize;
        }
        [Category("Buttons")]
        public event ButtonClickedEventHandler? ButtonClicked;

        public delegate void ButtonClickedEventHandler(object? Sender, GridButton Button, Point GridLocation);




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
        Size buttonSize = new Size(120, 120);
        [System.ComponentModel.Category("Appearance")]
        public Size ButtonSize {
            get { return buttonSize; }
            set {
                buttonSize = value;
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
        bool centerButtons = true;
        [System.ComponentModel.Category("Appearance")]
        public bool CenterButtons {
            get {
                return centerButtons;
            }
            set {
                centerButtons = value;
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

        private List<GridButton> buttons = new List<GridButton>();
        private List<GridButton>? filteredButtons;
        [System.ComponentModel.Category("Buttons")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<GridButton> Buttons {
            get { return buttons; }
        }
        [System.ComponentModel.Category("Buttons")]
        public List<GridButton> CurrentButtons {
            get {
                if (filter == null) {
                    return buttons;
                }
                else if (filter == "") {
                    return buttons;
                }
                else if (filter.Length == 0) {
                    return buttons;
                }
                else {
                    if (filteredButtons == null) {
                        return buttons;
                    }
                    else {
                        return filteredButtons;
                    }
                }
            }
        }
        private Color _ScrollBarNorth = Color.DarkTurquoise;
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarNorth {
            get {
                return _ScrollBarNorth;
            }
            set {
                _ScrollBarNorth = value;
                Invalidate();
            }
        }
        private Color _ScrollBarSouth = Color.DeepSkyBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarSouth {
            get {
                return _ScrollBarSouth;
            }
            set {
                _ScrollBarSouth = value;
                Invalidate();
            }
        }
        private Color _ScrollBarMouseDown = Color.FromArgb(64, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarMouseDown {
            get {
                return _ScrollBarMouseDown;
            }
            set {
                _ScrollBarMouseDown = value;
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
        private string filter;
        [System.ComponentModel.Category("Filtering")]
        public string Filter {
            get {
                return filter;
            }
            set {
                filter = value;
                ReevaluateList();
            }
        }
        private void ReevaluateList() {
            filteredButtons = buttons.Where(x => x.Text.ToLower().Contains(filter.ToLower())).ToList();
            Invalidate();
        }

        private int _VerScroll = 0;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                if (_VerScroll < 0) {
                    return 0;
                }
                return _VerScroll;
            }
            set {
                if (value < 0) {
                    _VerScroll = 0;
                }
                else if (Rows == RowCount) {
                    _VerScroll = 0;
                }
                else if (value > RowCount - 1) {
                    if (Rows > RowCount) {
                        _VerScroll = 0;
                    }
                    else {
                        _VerScroll = RowCount - 1;
                    }
                }
                else {
                    if (Rows > RowCount) {
                        _VerScroll = 0;
                    }
                    else {
                        _VerScroll = value;
                    }
                }


                //if (InSelection == true) {
                //    SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                //    SelectValuesList(SELTEST);
                //}
                Invalidate();
            }
        }
        int Columns = 2;
        int Rows = 3;
        int RowCount = 0;

        Rectangle GridRectangle = new Rectangle(0, 0, 10, 10);
        int Yscroll = 0;
        private void SetupFrame(PaintEventArgs e) {
            using (System.Drawing.Font GenericSize = new System.Drawing.Font(Font.FontFamily, 9.0f, Font.Style)) {
                ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
            }
            GridRectangle = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - ScrollSize, Height - Padding.Top - Padding.Bottom);
            Columns = (int)Math.Floor((float)GridRectangle.Width / (float)ButtonSize.Width);
            Rows = (int)Math.Floor((float)GridRectangle.Height / (float)ButtonSize.Height);
            if (Columns > 0) {
                RowCount = (int)Math.Ceiling((float)CurrentButtons.Count / (float)Columns);
            }
            else { RowCount = 0; }
        }
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            SetupFrame(e);
            int k = (VerScroll * Columns);
            int XDefault = Padding.Left;
            if (centerButtons == true) {
                XDefault = (int)((float)(GridRectangle.Width - (Columns * ButtonSize.Width)) / 2.0f);
            }
            int XOffset = XDefault;
            int YOffset = Padding.Top;
            for (int j = 0; j < Rows + 1; j++) {
                XOffset = XDefault;
                for (int i = 0; i < Columns; i++) {
                    if (CurrentButtons.Count > 0) {
                        if (k < CurrentButtons.Count) {
                            Rectangle CurrentButton = new Rectangle(XOffset, YOffset, ButtonSize.Width, ButtonSize.Height);
                            CurrentButton = ReadjustRectangle(CurrentButton);
                            DrawButton(e, CurrentButton, CurrentButtons[k], j + VerScroll, i);
                            k++;
                        }
                    }
                    XOffset += ButtonSize.Width;
                }
                YOffset += ButtonSize.Height;
            }
            RenderScrollBar(e);
            base.OnPaint(e);
        }
        private Rectangle ReadjustRectangle(Rectangle rect) {
            int WidthAdj = rect.Width - (buttonPadding.Left + buttonPadding.Right);
            int HeightAdj = rect.Height - (buttonPadding.Top + buttonPadding.Bottom);
            return new Rectangle(rect.X + buttonPadding.Left, rect.Y + buttonPadding.Top, WidthAdj, HeightAdj);
        }
        Point PressedButtonLocation = new Point(-1, -1);
        GridButton? PressedButton = null;
        private void DrawButton(PaintEventArgs e, Rectangle ButtonBounds, GridButton Btn, int ButtonRow, int ButtonColumn) {
            Color BackNorth = Color.Black;
            Color BackSouth = Color.Black;

            Color BorderNorth = Color.Black;
            Color BorderSouth = Color.Black;

            Rectangle ButtonFill = ButtonBounds;
            Rectangle ButtonRound = ButtonBounds;
            MouseStates ButtonState = GetState(ButtonBounds);
            //if (ButtonState == MouseStates.Down) {
            //    PressedButtonLocation = new Point(ButtonRow, ButtonColumn);
            //    PressedButton = Btn;
            //}
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

            //
            //
            //
            //
            //
            //
            DrawText(e, ButtonBounds, Btn);
        }
        private void DrawText(PaintEventArgs e, Rectangle ButtonBounds, GridButton Btn) {
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
        private int DrawImage(PaintEventArgs e, Rectangle ButtonBounds, GridButton Btn, int Offset) {
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
        Rectangle VerticalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF VerticalScrollThumb = new Rectangle(0, 0, 0, 0);
        int ScrollBarButtonSize = 0;
        int ScrollSize = 10;

        bool ShowVertScroll = true;
        bool ShowHorzScroll = false;
        private void RenderScrollBar(PaintEventArgs e) {
            Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
            if (RowCount < Rows) {
                ShowVertScroll = false;
            }
            else {
                if (RowCount > Rows) {
                    if (RowCount > 1) {
                        ShowVertScroll = true;
                    }
                    else { ShowVertScroll = false; }
                }
                else {
                    ShowVertScroll = false;
                }
            }
            if (ShowVertScroll == true) {
                VerticalScrollBar = new Rectangle(Width - ScrollSize, 0, ScrollSize, Height);

                if (ShowHorzScroll == true) { VerticalScrollBar.Height -= ScrollSize; }

                using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, VerticalScrollBar);
                }
                RenderVerticalBar(e);
            }

        }
        private void RenderVerticalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(VerticalScrollBar, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                ScrollBarButtonSize = ScrollSize;
                VerticalScrollBounds = new Rectangle(VerticalScrollBar.X, VerticalScrollBar.Y + ScrollBarButtonSize, VerticalScrollBar.Width, VerticalScrollBar.Height - (2 * ScrollBarButtonSize));
                if (RowCount > 0) {
                    float ViewableItems = ((float)Rows / 2.0f) / (float)RowCount;
                    if (RowCount < Rows) {
                        ViewableItems = 1;
                    }
                    float ThumbHeight = ViewableItems * VerticalScrollBounds.Height;
                    if (ThumbHeight < ScrollBarButtonSize * 2) {
                        ThumbHeight = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = 1;// + ScrollSize;
                    if (Rows != RowCount) {
                        int TempRows = RowCount <= 1 ? 2 : RowCount;
                        ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)(TempRows - 1)) + VerticalScrollBounds.Y;// + ScrollSize;
                    }
                    else {
                        ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)RowCount) + VerticalScrollBounds.Y;// + ScrollSize;
                    }
                    VerticalScrollThumb = new RectangleF(VerticalScrollBounds.X, ScrollBounds, VerticalScrollBar.Width, ThumbHeight);
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollBounds);
                }
                Rectangle Button = new Rectangle(VerticalScrollBar.X, 0, ScrollBarButtonSize, ScrollBarButtonSize);
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y + Button.Height), new Point(Button.X + Button.Width, Button.Y + Button.Height));
                        Button.Y = VerticalScrollBar.Height - Button.Height;
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y), new Point(Button.X + Button.Width, Button.Y));
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(VerticalScrollBar.X, VerticalScrollBar.Y), new Point(VerticalScrollBar.X, VerticalScrollBar.Y + VerticalScrollBar.Height));
                    }
                }
            }
        }
        private Rectangle InsetRectangle(Rectangle Input, int Amount) {
            int SizeW = Amount * 2;
            return new Rectangle(Input.X + Amount, Input.Y + Amount, Input.Width - SizeW, Input.Height - SizeW);
        }
        private enum MouseStates {
            Exited = 0x00,
            Hover = 0x01,
            Down = 0x02
        }
        bool DownLatched = false;

        private MouseStates GetState(Rectangle ButtonBounds) {
            if (ButtonBounds.Contains(CurrentPosition)) {
                if (IsMouseDown == true) {
                    if (DownLatched == false) {
                        DownLatched = true;
                        return MouseStates.Down;
                    }
                    else {
                        if (ButtonBounds.Contains(DownLocation)) {
                            return MouseStates.Down;
                        }
                    }
                }
                if (DownLatched == false) {
                    return MouseStates.Hover;
                }
            }
            if (DownLatched == true) {
                if (ButtonBounds.Contains(DownLocation)) {
                    return MouseStates.Down;
                }
            }
            return MouseStates.Exited;
        }
        private void GetBackColors(MouseStates MouseState, GridButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBack = backColorNorth;
            Color TempSouthBack = backColorSouth;
            NorthColor = TempNorthBack; SouthColor = TempSouthBack;
            if (Enabled == true) {
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
                NorthColor = WhiteLightenColor(TempNorthBack, 64);
                SouthColor = WhiteLightenColor(TempSouthBack, 64);
            }
        }
        private void GetBorderColors(MouseStates MouseState, GridButton Btn, out Color NorthColor, out Color SouthColor) {
            Color TempNorthBorder = borderColorNorth;
            Color TempSouthBorder = borderColorSouth;
            NorthColor = TempNorthBorder; SouthColor = TempSouthBorder;
            if (Enabled == true) {
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
        private void ButtonGrid_MouseLeave(object? sender, EventArgs e) {
            //CurrentPosition = new Point(-1, -1);
            Invalidate();
        }
        private void ButtonGrid_MouseUp(object? sender, MouseEventArgs e) {
            IsMouseDown = false;
            DownLatched = false;
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            if (IsPointReset(PressedButtonLocation) == false) {
                //if (PressedButton != null) {
                //    ResetAllothers(PressedButton);
                //    //ButtonClicked?.Invoke(this, PressedButton, PressedButtonLocation);
                //}
            }
            PressedButtonLocation = new Point(-1, -1);
            Invalidate();
        }
        private void ButtonGrid_MouseClick(object? sender, MouseEventArgs e) {
           
            if (IsPointReset(e.Location) == false) {
                ClickedButtonResult Result = GetButtonPosition(e.Location);
                if (Result.Button != null) {
                    ResetAllothers(Result.Button);
                    CurrentPosition = e.Location;
                    DownLocation = e.Location;
                    IsMouseDown = true;
                    Invalidate();
                    ButtonClicked?.Invoke(this, Result.Button, Result.Position);
                }
            }

        }
        private ClickedButtonResult GetButtonPosition(Point SelectedPoint) {
            Point Output = new Point(-1, -1);
            int ButtonIndex = -1;
            int k = (VerScroll * Columns);
            int XDefault = Padding.Left;
            if (centerButtons == true) {
                XDefault = (int)((float)(GridRectangle.Width - (Columns * ButtonSize.Width)) / 2.0f);
            }
            int XOffset = XDefault;
            int YOffset = Padding.Top;
            bool BreakAll = false;
            for (int j = 0; j < Rows + 1; j++) {
                XOffset = XDefault;
                for (int i = 0; i < Columns; i++) {
                    if (CurrentButtons.Count > 0) {
                        if (k < CurrentButtons.Count) {
                            Rectangle CurrentButton = new Rectangle(XOffset, YOffset, ButtonSize.Width, ButtonSize.Height);
                            CurrentButton = ReadjustRectangle(CurrentButton);
                            if (CurrentButton.Contains(SelectedPoint)) {
                                Output = new Point(i, j + VerScroll);
                                ButtonIndex = k;
                                BreakAll = true;
                                break;
                            }

                            k++;
                        }
                    }
                    XOffset += ButtonSize.Width;
                }
                if (BreakAll == true) {
                    break;
                }
                YOffset += ButtonSize.Height;
            }
            GridButton? Btn = null;
            if (ButtonIndex != -1) {
                Btn = buttons[ButtonIndex];
            }
            return new ClickedButtonResult(Btn, Output, ButtonIndex);
        }
        private void ResetAllothers(GridButton ThisBtn) {
            if (ThisBtn.Type == ButtonType.RadioButton) {
                ThisBtn.Checked = true;
                if (ThisBtn.Checked == true) {
                    try {
                        foreach (GridButton Btn in buttons) {
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
        private bool IsPointReset(Point Input) {
            if ((Input.X < 0) || (Input.Y < 0)) { return true; }
            return false;
        }
        private void ButtonGrid_MouseMove(object? sender, MouseEventArgs e) {
            if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Vertical)) {
                if (Rows > 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, ThumbDelta);
                    Invalidate();
                }
                CurrentPosition = new Point(-1, -1);
            }
            else {
                CurrentPosition = e.Location;
            }

            Invalidate();
        }
        int ScrollItems = 1;
        private void ButtonGrid_MouseWheel(object? sender, MouseEventArgs e) {
            int D = e.Delta;
            int DC = ScrollItems * (int)Math.Abs((double)D / (double)120);
            if (D > 0) {
                if (VerScroll > 0)
                    VerScroll -= DC;
            }
            else if (VerScroll <= RowCount)
                VerScroll += Math.Abs(DC);
        }
        private int GetVerticalScrollFromCursor(int MousePositionY, float ThumbPosition) {
            return (int)((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition) * RowCount) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
        }
        bool InScrollBounds = false;
        bool ScrollStart = false;
        float ThumbDelta = 0;
        ScrollArea ScrollHit = ScrollArea.None;
        enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        Point DownLocation = new Point(0, 0);
        private void ButtonGrid_MouseDown(object? sender, MouseEventArgs e) {
            DownLatched = false;
           
            if ((ShowVertScroll == true) && (e.X >= Width - ScrollSize)) {
                ScrollHit = ScrollArea.Vertical;
                if (ScrollStart == false) {
                    ThumbDelta = e.Y - VerticalScrollThumb.Y;
                    if (ThumbDelta < 0) {
                        ThumbDelta = 0;
                    }
                    else if (ThumbDelta > VerticalScrollThumb.Height + VerticalScrollThumb.Height) {
                        ThumbDelta = VerticalScrollThumb.Height;
                    }
                    ScrollStart = true;
                }
                InScrollBounds = true;
            }
            else {
                IsMouseDown = true;
                CurrentPosition = e.Location;
                DownLocation = e.Location;
            }

            Invalidate();
        }
        bool Latched = false;
        long LastTick = 0;
        private void ButtonGrid_Resize(object? sender, EventArgs e) {
            if (DateTime.Now.Ticks - LastTick > 1000) {
                Latched = false;
            }
            LastTick = DateTime.Now.Ticks;
            if (Latched == false) {
                int TempRowCount = RowCount < 1 ? 1 : RowCount;
                float Percent = (float)VerScroll / (float)TempRowCount;
                Latched = true;
                VerScroll = (int)(Percent * (float)TempRowCount);
            }
            Invalidate();

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
            // ButtonGrid
            // 
            this.Name = "ButtonGrid";
            this.Load += new System.EventHandler(this.ButtonGrid_Load);
            this.ResumeLayout(false);

        }

        private void ButtonGrid_Load(object sender, EventArgs e) {

        }
    }
    public class ClickedButtonResult {
        GridButton ?button = null;
        public GridButton ?Button {
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
        public ClickedButtonResult(GridButton ?Button, Point Position, int Index) {
            this.button = Button;
            this.index = Index;
            this.position = Position;
        }
    }
    public class GridButton {
        #region Properties
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
        object? tag = null;
        [System.ComponentModel.Category("Data")]
        public object? Tag {
            get {
                return tag;
            }
            set {
                tag = value;
            }
        }
        string command = "";
        [System.ComponentModel.Category("Data")]
        public string Command {
            get {
                return command;
            }
            set {
                command = value;
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
            }
        }
        #endregion
    }
}
