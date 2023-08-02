using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        }

        private void DrawDifferences(PaintEventArgs e, Rectangle Rect) {
            int CharWidth = (int)e.Graphics.MeasureString("W", Font).Width;
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
                    DrawChar(e, compareTo[i], CompareBox, (int)e.Graphics.MeasureString("W", Font).Height);
                }
            }
        }
        private void DrawChar(PaintEventArgs e, char Input, Rectangle BoundingBox, int VerticalOffset) {
            string InputB = Input.ToString();
            Rectangle Rect = BoundingBox;
            Rect.Y += VerticalOffset;
            if (InputB != "") {
                using (SolidBrush br = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString(InputB, Font, br, Rect);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            DetermineLongestString();

            DrawDifferences(e, new Rectangle(0, 0, Width, Height));
            // DrawText(e, TopString, 0);
            // DrawText(e, BottomString, (int)e.Graphics.MeasureString("W", Font).Height);
        }

        protected override void OnResize(EventArgs e) {
            // base.OnResize(e);
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
