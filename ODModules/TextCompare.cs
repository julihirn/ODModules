using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Handlers;
using System.Diagnostics;

namespace ODModules {


    public class TextCompare : System.Windows.Forms.UserControl {
        public event EventHandler? TextSizeChanged;
        private string compareFrom = "";
        private string compareTo = "";
        private int textPosition = 0;
        [System.ComponentModel.Category("Text Comparison")]
        public string CompareFrom {
            get {
                return compareFrom;
            }
            set {
                compareFrom = value;
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Text Comparison")]
        public string CompareTo {
            get { return compareTo; }
            set { compareTo = value; Invalidate(); }
        }
        public int TextPosition {
            get { return textPosition; }
            set {
                if (value <= Longeststring - 1) {
                    if (value >= 0) {
                        textPosition = value;
                    }
                    else {
                        textPosition = 0;
                    }
                }
                else {
                    if (Longeststring == 0) {
                        textPosition = 0;
                    }
                    else {
                        textPosition = Longeststring - 1;
                    }
                }
                Invalidate();
            }

        }
        [System.ComponentModel.Category("Appearance")]
        public Color DifferenceColor {
            get { return differenceColor; }
            set { differenceColor = value; Invalidate(); }
        }
        [System.ComponentModel.Category("Appearance")]
        public Color SameColor {
            get { return sameColor; }
            set { sameColor = value; Invalidate(); }
        }
        private Color gridColor = Color.FromArgb(255, 100, 100, 100);
        [System.ComponentModel.Category("Appearance")]
        public Color GridColor {
            get { return gridColor; }
            set { gridColor = value; Invalidate(); }
        }
        [System.ComponentModel.Category("Appearance")]
        public Color EmptyTextColor {
            get { return emptyTextColor; }
            set { emptyTextColor = value; Invalidate(); }
        }
        [System.ComponentModel.Category("Appearance")]
        public bool UseEmptyTextColor {
            get { return useEmptyTextColor; }
            set { useEmptyTextColor = value; Invalidate(); }
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
        bool ShowHorzScroll = true;
        [System.ComponentModel.Category("Scrolling")]
        public bool ShowScrollBar {
            get {
                return ShowHorzScroll;
            }
            set {
                ShowHorzScroll = value;
                Invalidate();
            }
        }

        private bool useEmptyTextColor = false;

        private Color sameColor = Color.FromArgb(192, 255, 192);
        private Color differenceColor = Color.FromArgb(255, 192, 192);
        private Color emptyTextColor = Color.FromArgb(255, 224, 192);
        public int MaximumLength {
            get {
                return Longeststring - 1;
            }
        }
        int Longeststring = 0;
        private void DetermineLongestString() {
            int OldLongest = Longeststring;
            if (compareFrom.Length >= compareTo.Length) {
                Longeststring = compareFrom.Length;
            }
            else if (compareFrom.Length < compareTo.Length) {
                Longeststring = compareTo.Length;
            }

            if (OldLongest != Longeststring) {
                //Invoke that the scroll has to be changed
                if (TextPosition - 1 > Longeststring - 1) {
                    if (Longeststring > 0) {
                        TextPosition = Longeststring - 1;
                    }
                }
                EventHandler? handler = TextSizeChanged;
                //int Objs = Longeststring - 1;
                handler?.Invoke(this, EventArgs.Empty);
            }
        }

        public TextCompare() {
            DoubleBuffered = true;
            MouseDown += TextCompare_MouseDown;
            MouseUp += TextCompare_MouseUp;
            MouseMove += TextCompare_MouseMove;
            MouseClick += TextCompare_MouseClick;
            MouseWheel += TextCompare_MouseWheel;
        }

        private void TextCompare_MouseWheel(object? sender, MouseEventArgs e) {
            if (ShowHorzScroll == true) {
                int MaxDiff = Longeststring - WindowCharacters;
                if (Longeststring > WindowCharacters) {
                    if (e.Delta > 0) {
                        if (TextPosition < MaxDiff) {
                            TextPosition++;
                        }
                        else { TextPosition = MaxDiff; }
                    }
                    else {
                        if (TextPosition > 0) {
                            TextPosition--;
                        }
                        else { TextPosition = 0; }
                    }
                }
            }
        }

        bool MouseIsDown = false;
        bool InScrollBounds = false;
        bool ScrollStart = false;
        ScrollArea ScrollHit = ScrollArea.None;
        float ThumbDelta = 0;
        Point ScrollOutofBoundsDelta = new Point(0, 0);
        enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        private int GetHorizontalScrollFromCursor(int MousePositionX, float ThumbPosition) {
            float Diff = (float)(Longeststring - WindowCharacters);
            int TempScroll = (int)((float)((MousePositionX - HorizontalScrollBounds.X - ThumbPosition) * Diff) / (HorizontalScrollBounds.Width - HorizontalScrollThumb.Width));
            if (TempScroll > Diff) {
                TempScroll = (int)Diff;
            }
            return TempScroll;
        }
        private void TextCompare_MouseMove(object? sender, MouseEventArgs e) {
            if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Horizontal)) {
                TextPosition = GetHorizontalScrollFromCursor(e.X, ThumbDelta);
                Invalidate();
            }
        }
        private void TextCompare_MouseUp(object? sender, MouseEventArgs e) {
            MouseIsDown = false;
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            Invalidate();
        }
        private void TextCompare_MouseDown(object? sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MouseIsDown = true;
            }
            if ((ShowHorzScroll == true) && (e.Y > Height - ScrollSize)) {
                ScrollHit = ScrollArea.Horizontal;
                if (ScrollStart == false) {
                    ThumbDelta = e.X - HorizontalScrollThumb.X;
                    if (ThumbDelta < 0) {
                        ThumbDelta = 0;
                    }
                    else if (ThumbDelta > HorizontalScrollThumb.X + HorizontalScrollThumb.Width) {
                        ThumbDelta = HorizontalScrollThumb.Width;
                    }
                    ScrollStart = true;
                }
                InScrollBounds = true;
            }
        }
        private void TextCompare_MouseClick(object? sender, MouseEventArgs e) {
            if ((InScrollBounds == true) && (e.Y >= Height - ScrollSize)) {
                InScrollBounds = true;
                if (ShowHorzScroll == true) {
                    float ThumbDeltaTest = e.X - HorizontalScrollThumb.X;
                    if (ThumbDeltaTest < 0) {
                        TextPosition = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                    else if (ThumbDeltaTest > HorizontalScrollThumb.Width) {
                        TextPosition = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                }
            }
        }
        #region Control Drawing
        private void DrawDifferences(PaintEventArgs e, Rectangle Rect) {
        
            int CharWidth = (int)e.Graphics.MeasureString("W", Font).Width;
            int CharHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            int MaxCharWin = (int)Math.Ceiling((double)Rect.Width / (double)CharWidth);
            for (int i = textPosition; i < textPosition + MaxCharWin; i++) {
                bool InBounds = false;
                bool PreventOverride = false;
                Rectangle CompareBox = new Rectangle(((i - textPosition) * CharWidth) + Rect.X, Rect.Y, CharWidth, Rect.Height);
                if (i <= compareFrom.Length - 1) {
                    if (compareFrom.Length > 0) {
                        //TopString += compareFrom[i];
                        InBounds = true;
                    }
                    else { InBounds = false; PreventOverride = true; }
                }
                else { InBounds = false; PreventOverride = true; }
                if (i <= compareTo.Length - 1) {
                    if (compareTo.Length > 0) {
                        //BottomString += compareTo[i];

                        if (PreventOverride == false) {
                            InBounds = true;
                        }
                    }
                    else { InBounds = false; }
                }
                else { InBounds = false; }

                if (InBounds == false) {
                    Color EmptyStringColor = differenceColor;
                    if (useEmptyTextColor == true) { EmptyStringColor = emptyTextColor; }
                    using (SolidBrush br = new SolidBrush(EmptyStringColor)) {
                        e.Graphics.FillRectangle(br, CompareBox);
                    }
                }
                else {
                    Color EmptyStringColor = differenceColor;
                    if (compareFrom[i] == compareTo[i]) { EmptyStringColor = SameColor; }
                    using (SolidBrush br = new SolidBrush(EmptyStringColor)) {
                        e.Graphics.FillRectangle(br, CompareBox);
                    }

                }
                if ((compareFrom.Length > 0) && (i < compareFrom.Length)) {
                    DrawChar(e, compareFrom[i], CompareBox, 0);
                }
                if ((compareTo.Length > 0) && (i < compareTo.Length)) {
                    DrawChar(e, compareTo[i], CompareBox, CharHeight);
                }
                if (i != textPosition) {
                    using (SolidBrush GridBrush = new SolidBrush(gridColor)) {
                        using (Pen GridPen = new Pen(GridBrush)) {
                            e.Graphics.DrawLine(GridPen, CompareBox.X, 0, CompareBox.X, Height);
                        }
                    }
                }
            }
        }
        private void DrawChar(PaintEventArgs e, char Input, Rectangle BoundingBox, int VerticalOffset) {
            using (StringFormat StrFrmt = new StringFormat()) {
                string InputB = Input.ToString();
                Rectangle Rect = BoundingBox;
                Rect.Y += VerticalOffset;
                StrFrmt.LineAlignment = StringAlignment.Near;
                StrFrmt.Alignment = StringAlignment.Center;

                if (InputB != "") {
                    using (SolidBrush br = new SolidBrush(ForeColor)) {
                        e.Graphics.DrawString(InputB, Font, br, Rect, StrFrmt);
                    }
                }
            }
        }
        int WindowCharacters = 100;
        int GenericCharacterWidth = 3;
        protected override void OnPaint(PaintEventArgs e) {
            using (System.Drawing.Font GenericSize = new System.Drawing.Font(Font.FontFamily, 9.0f, Font.Style)) {
                ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
           // e.Graphics.TextContrast = 1;
           
            GenericCharacterWidth = (int)e.Graphics.MeasureString("W", Font).Width;
            WindowCharacters = (Width / GenericCharacterWidth) - 1;
            if (WindowCharacters < 1) { WindowCharacters = 1; }
            DetermineLongestString();

            DrawDifferences(e, new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom));
            // DrawText(e, TopString, 0);
            // DrawText(e, BottomString, (int)e.Graphics.MeasureString("W", Font).Height);
            RenderScrollBar(e);
        }
        private void RenderScrollBar(PaintEventArgs e) {
            Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
            HorizontalScrollBar = new Rectangle(0, Height - ScrollSize, Width, ScrollSize);
            using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillRectangle(HeaderBackBrush, HorizontalScrollBar);
            }
            RenderHorizontalBar(e);
            using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                    e.Graphics.DrawLine(ScrollBarBorderPen, new Point(HorizontalScrollBar.X, HorizontalScrollBar.Y), new Point(HorizontalScrollBar.Width, HorizontalScrollBar.Y));
                }
            }
        }
        int ScrollSize = 10;
        int ScrollBarButtonSize = 0;
        Rectangle HorizontalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle HorizontalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF HorizontalScrollThumb = new Rectangle(0, 0, 0, 0);
        private void RenderHorizontalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(Bounds, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                //e.Graphics.FillRectangle(HeaderForeBrush, VerticalBar);
                ScrollBarButtonSize = ScrollSize;
                HorizontalScrollBounds = new Rectangle(HorizontalScrollBar.X + ScrollBarButtonSize, HorizontalScrollBar.Y, HorizontalScrollBar.Width - (2 * ScrollBarButtonSize), HorizontalScrollBar.Height);
                float WidthOverCurrent = 1;
                float TempLongest = 0f;
                if (Longeststring > WindowCharacters) {
                    WidthOverCurrent = Longeststring * GenericCharacterWidth;
                    TempLongest = textPosition / (float)(Longeststring - WindowCharacters);
                }
                float ViewableLines = (float)Width / (WidthOverCurrent);
                float ThumbWidth = ViewableLines * HorizontalScrollBounds.Width;
                if (ThumbWidth < ScrollBarButtonSize * 2) {
                    ThumbWidth = ScrollBarButtonSize * 2;
                }
                // w = 20, ls = 30, diff = 10, x
                float ScrollBounds = (HorizontalScrollBounds.Width - ThumbWidth) * TempLongest + HorizontalScrollBounds.X;
                HorizontalScrollThumb = new RectangleF(ScrollBounds, HorizontalScrollBounds.Y, ThumbWidth, HorizontalScrollBar.Height);
                e.Graphics.FillRectangle(HeaderForeBrush, HorizontalScrollThumb);
                //Buttons
                Rectangle Button = new Rectangle(0, HorizontalScrollBar.Y, ScrollBarButtonSize, ScrollBarButtonSize);
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X + Button.Width, Button.Y), new Point(Button.X + Button.Width, Button.Y + Button.Height));
                        Button.X = HorizontalScrollBar.Width - Button.Width;
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y), new Point(Button.X, Button.Y + Button.Height));
                    }
                }
            }
        }
        #endregion
        protected override void OnResize(EventArgs e) {
            // base.OnResize(e);
            if (ShowHorzScroll == true) {
                float TempLongest = 1;
                if (Longeststring > WindowCharacters) {
                    TempLongest = TextPosition / (float)(Longeststring - WindowCharacters);
                    if (TempLongest >= 1.0f) {
                        TextPosition = Longeststring - WindowCharacters;
                    }
                }
                else { TextPosition = 0; }
            }
            Invalidate();
        }
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // TextCompare
            // 
            this.Name = "TextCompare";
            this.Load += new System.EventHandler(this.TextCompare_Load);
            this.ResumeLayout(false);

        }

        private void TextCompare_Load(object? sender, EventArgs e) {

        }
    }
}
