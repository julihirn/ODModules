﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using static ODModules.ListControl;
namespace ODModules {
    public partial class BitToggle : UserControl {

        [Category("Value")]
        public event BitToggledHandler? BitToggled;
        public delegate void BitToggledHandler(object sender, int Bit, string Value);

        [Category("Value")]
        public event ValueChangedHandler? ValueChanged;
        public delegate void ValueChangedHandler(object sender, string Value);

        System.Windows.Forms.Timer Tmr = new System.Windows.Forms.Timer();

        public BitToggle() {
            InitializeComponent();
            DoubleBuffered = true;
            MouseClick += BitToggle_MouseClick;
            MouseDown += BitToggle_MouseDown;
            MouseUp += BitToggle_MouseUp;
            MouseMove += BitToggle_MouseMove;
            MouseLeave += BitToggle_MouseLeave;
            Resize += BitToggle_Resize;
            SizeChanged += BitToggle_SizeChanged;
            OnResizing = true;
            Tmr.Interval = 1000;
            Tmr.Enabled = false;
            Tmr.Tick += Tmr_Tick;
        }



        int MaxBits = 64;
        string byteValue = "0";
        [Category("Data")]
        public string Value {
            get { return byteValue; }
            set {
                string TempString = value.Replace(" ", "").TrimStart('0');
                if (Regex.IsMatch(TempString, @"^(?:[0-1]+)$")) {
                    TrimBits(TempString);
                }
                else {
                    byteValue = "0";
                }
                ValueChanged?.Invoke(this, byteValue);
                Invalidate();
            }
        }
        int bits = 32;
        [Category("Data")]
        public int Bits {
            get { return bits; }
            set {
                if (value <= 0) { bits = 1; }
                else if (value >= MaxBits) { bits = MaxBits; }
                else { bits = value; }
                TrimBits(byteValue);
                Invalidate();
            }
        }
        Color inactiveToggleForeColor = Color.Gray;
        [Category("Appearance")]
        public Color InactiveToggleForeColor {
            get { return inactiveToggleForeColor; }
            set { inactiveToggleForeColor = value; Invalidate(); }
        }
        Color activeToggleForeColor = Color.Black;
        [Category("Appearance")]
        public Color ActiveToggleForeColor {
            get { return activeToggleForeColor; }
            set { activeToggleForeColor = value; Invalidate(); }
        }
        Color selectedForeColor = Color.Blue;
        [Category("Appearance")]
        public Color SelectedForeColor {
            get { return selectedForeColor; }
            set { selectedForeColor = value; Invalidate(); }
        }
        Color mouseOverForeColor = Color.Blue;
        [Category("Appearance")]
        public Color MouseOverForeColor {
            get { return mouseOverForeColor; }
            set { mouseOverForeColor = value; Invalidate(); }
        }
        Color mouseDownForeColor = Color.WhiteSmoke;
        [Category("Appearance")]
        public Color MouseDownForeColor {
            get { return mouseDownForeColor; }
            set { mouseDownForeColor = value; Invalidate(); }
        }
        WordSize togglerSize = WordSize.QWord;
        [Category("Appearance")]
        public WordSize TogglerSize {
            get { return togglerSize; }
            set {
                togglerSize = value;
                switch (value) {
                    case WordSize.Byte: Rows = 1; Columns = 9; MaxBits = 8; break;
                    case WordSize.Word: Rows = 1; Columns = 19; MaxBits = 16; break;
                    case WordSize.SWord: Rows = 2; Columns = 19; MaxBits = 24; break;
                    case WordSize.DWord: Rows = 2; Columns = 19; MaxBits = 32; break;
                    case WordSize.TWord: Rows = 3; Columns = 19; MaxBits = 48; break;
                    case WordSize.QWord: Rows = 4; Columns = 19; MaxBits = 64; break;
                }
                Invalidate();
            }
        }
        bool allowKeyboardControl = true;
        [Category("Control")]
        public bool AllowKeyboardControl {
            get { return allowKeyboardControl; }
            set { allowKeyboardControl = value; }
        }
        bool allowKeyboardInvertControl = true;
        [Category("Control")]
        public bool AllowKeyboardInvertControl {
            get { return allowKeyboardInvertControl; }
            set { allowKeyboardInvertControl = value; }
        }
        public enum WordSize {
            Byte = 0x00,
            Word = 0x01,
            SWord = 0x02,
            DWord = 0x03,
            TWord = 0x04,
            QWord = 0x05
        }
        private void TrimBits(string Input) {
            if (Input.Length > bits) {
                int Overflow = Input.Length - bits;
                byteValue = Input.Substring(Overflow, bits); ;
            }
            else {
                byteValue = Input;
            }
        }
        int ToggleHeight = 10;
        int BitCountHeight = 10;
        int BitCountWidth = 10;
        int LabelHeight = 0;

        int TotalHeight = 0;
        int ToggleWidth = 10;
        Rectangle InsetRectangle = Rectangle.Empty;
        int Columns = 19;
        int Rows = 4;
        Size CellSize = Size.Empty;

        const int BitTogglerMinimumSize = 8;
        int ToggleFontSize = 10;

        bool OnResizing = false;

        bool BitNumbersVisible = true;
        bool LabelsVisible = true;
        private Rectangle PadRectangle(Rectangle Input, int Pad) {
            int Dual = (Pad * 2);
            return new Rectangle(Input.X + Pad, Input.Y + Pad, Input.Width - Dual, Input.Height - Dual);
        }
        int HorizontalOffset = 0;
        protected override void OnPaint(PaintEventArgs e) {
            Rectangle PaddingRectangle = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom);
            InsetRectangle = PadRectangle(PaddingRectangle, (int)e.Graphics.MeasureString("W", Font).Width);
            CellSize = new Size(InsetRectangle.Width / Columns, InsetRectangle.Height / Rows);
            HorizontalOffset = (int)((float)(InsetRectangle.Width - (CellSize.Width * (Columns))) / 2.0f);
            if (OnResizing == true) {
                for (int i = BitTogglerMinimumSize; i < 20; i++) {
                    using (Font TempFnt = new Font(Font.FontFamily, i)) {
                        int CurrentWidth = (int)e.Graphics.MeasureString("0", TempFnt).Width;
                        if (CurrentWidth > CellSize.Width) { break; }
                        ToggleFontSize = i;
                    }
                }
                OnResizing = false;
                using (Font TempFnt = new Font(Font.FontFamily, ToggleFontSize)) {
                    ToggleHeight = (int)e.Graphics.MeasureString("0", TempFnt).Height;
                    ToggleWidth = (int)e.Graphics.MeasureString("0", TempFnt).Width;
                }
                BitCountHeight = (int)e.Graphics.MeasureString("0", Font).Height;
            }
            BitCountWidth = (int)e.Graphics.MeasureString("00", Font).Width;

            if ((ToggleHeight + BitCountHeight) > CellSize.Height) {
                BitNumbersVisible = false;
                TotalHeight = ToggleHeight;
            }
            else {
                BitNumbersVisible = true;
                TotalHeight = ToggleHeight + BitCountHeight; ;
            }

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            DrawTogglers(e);
        }
        private void DrawTogglers(PaintEventArgs e) {
            for (int i = MaxBits - 1; i >= 0; i--) {
                DrawToggler(e, i);
            }
        }
        private void DrawToggler(PaintEventArgs e, int Bit) {
            //int Index = (MaxBits - 1) - Bit;
            Rectangle BoundsRectangle = GetBitRectangle(Bit);

            Rectangle ToggleRectangle = SpiltRectangle(BoundsRectangle, TextItem.Toggler);
            DrawToggle(e, Bit, ToggleRectangle);
            if (BitNumbersVisible) {
                if (Bit % 4 == 0) {
                    Rectangle TextRectangle = SpiltRectangle(BoundsRectangle, TextItem.Counter);
                    using (StringFormat TxtFrmt = new StringFormat()) {
                        TxtFrmt.Alignment = StringAlignment.Center;
                        TxtFrmt.Trimming = StringTrimming.None;
                        using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                            e.Graphics.DrawString(Bit.ToString(), Font, TxtBr, TextRectangle, TxtFrmt);
                        }
                    }
                }
            }
        }
        private void DrawToggle(PaintEventArgs e, int Bit, Rectangle ToggleRectangle) {
            using (Font TempFnt = new Font(Font.FontFamily, ToggleFontSize)) {
                if (Bit > bits - 1) {
                    using (SolidBrush TxtBr = new SolidBrush(inactiveToggleForeColor)) {
                        using (StringFormat Sf = new StringFormat()) {
                            Sf.LineAlignment = StringAlignment.Center;
                            Sf.Alignment = StringAlignment.Center;
                            e.Graphics.DrawString("0", TempFnt, TxtBr, ToggleRectangle, Sf);
                        }
                    }
                }
                else {
                    string BitString = "0";
                    if (Bit < byteValue.Length) {
                        int StringIndex = byteValue.Length - 1 - Bit;
                        BitString = byteValue[StringIndex].ToString();
                    }
                    Color ToggleForeColor = activeToggleForeColor;
                    if (IsMouseDown) {
                        if (Bit == MouseBit) {
                            ToggleForeColor = mouseDownForeColor;
                        }
                    }
                    else {
                        if (Bit == KeyboardBit) {
                            ToggleForeColor = selectedForeColor;
                        }
                        else if (Bit == MouseBit) {
                            ToggleForeColor = mouseOverForeColor;
                        }
                    }
                    using (SolidBrush TxtBr = new SolidBrush(ToggleForeColor)) {
                        using (StringFormat Sf = new StringFormat()) {
                            Sf.LineAlignment = StringAlignment.Center;
                            Sf.Alignment = StringAlignment.Center;
                            e.Graphics.DrawString(BitString, TempFnt, TxtBr, ToggleRectangle, Sf);
                        }
                    }
                }
            }
        }
        private enum TextItem {
            Toggler = 0x01,
            Counter = 0x02,
            Labels = 0x03
        }
        private Rectangle SpiltRectangle(Rectangle Input, TextItem Item) {
            int CentreHeightStart = Input.Y + ((Input.Height - TotalHeight) / 2);
            int CountWidth = Input.Width;
            int CountStart = Input.X;
            if (Item == TextItem.Counter) {
                if (BitCountWidth >= CountWidth) {
                    CountStart = Input.X + ((Input.Width - (BitCountWidth + 6)) / 2);
                    CountWidth = (BitCountWidth + 6);
                }
            }
            switch (Item) {
                case TextItem.Toggler:
                    return new Rectangle(Input.X, CentreHeightStart, Input.Width, ToggleHeight);
                case TextItem.Counter:
                    return new Rectangle(CountStart, CentreHeightStart + ToggleHeight, CountWidth, BitCountHeight);
            }
            return Rectangle.Empty;
        }
        private Rectangle GetBitRectangle(int Bit) {
            int RelativeBit = (MaxBits - 1) - Bit;
            int BitModul = RelativeBit % 16;
            int HorizontalSpacing = BitModul / 4;
            int VerticalSpacing = RelativeBit / 16;

            //bool IsNewLine = BitModul == 0;
            //if (IsNewLine) {
            //    HorizontalSpacing = 0;
            //}
            int i = BitModul + HorizontalSpacing;
            int j = VerticalSpacing;
            Point Pnt = new Point((i * CellSize.Width) + InsetRectangle.X + HorizontalOffset, (j * CellSize.Height) + InsetRectangle.Y);
            return new Rectangle(Pnt, CellSize);
        }

        private int GetBit(Point Input, bool RestrictSize) {
            if (CellSize.Width <= 0) { return -1; }
            if (CellSize.Height <= 0) { return -1; }
            Point RelativePoint = new Point(Input.X - (InsetRectangle.X + HorizontalOffset), Input.Y - InsetRectangle.Location.Y);
            int Column = RelativePoint.X / CellSize.Width;
            int Row = RelativePoint.Y / CellSize.Height;
            int HorizontalSpacing = Column / 5;
            Point Pnt = new Point((Column * CellSize.Width) + InsetRectangle.X + HorizontalOffset, (Row * CellSize.Height) + InsetRectangle.Y);
            // Size HalfSize = new Size(CellSize.Width, CellSize.Height / 2);
            Rectangle TogglerBounds = SpiltRectangle(new Rectangle(Pnt, CellSize), TextItem.Toggler);
            if (!TogglerBounds.Contains(Input)) { return -1; }
            if (!InsetRectangle.Contains(Input)) { return -1; }
            if ((Column == 4) || (Column == 9) || (Column == 14) || (Column >= 19)) {
                return -1;
            }
            Column -= HorizontalSpacing;

            int Bit = (MaxBits - 1) - (Column + (Row * 16));
            if (RestrictSize == false) {
                return Bit;
            }
            if (Bit < Bits) { return Bit; }
            return -1;
        }
        private void BitToggle_Load(object sender, EventArgs e) {

        }
        bool IsMouseDown = false;
        int MouseBit = -1;
        int KeyboardBit = -1;
        int LastMouseBit = -1;
        private void BitToggle_MouseLeave(object? sender, EventArgs e) {
            MouseBit = -1;
        }
        private void BitToggle_MouseMove(object? sender, MouseEventArgs e) {
            if (IsMouseDown) {

            }
            else {
                MouseBit = GetBit(e.Location, true);
                LastMouseBit = MouseBit;
                Invalidate();
            }
        }
        private void BitToggle_MouseUp(object? sender, MouseEventArgs e) {
            IsMouseDown = false;
            Invalidate();
        }
        private void BitToggle_MouseDown(object? sender, MouseEventArgs e) {
            IsMouseDown = true;
            Invalidate();
        }
        private void BitToggle_MouseClick(object? sender, MouseEventArgs e) {
            SetBit(MouseBit);
        }
        private void BitToggle_SizeChanged(object? sender, EventArgs e) {
            OnResizing = true;
            Invalidate();
        }
        private void BitToggle_Resize(object? sender, EventArgs e) {
            OnResizing = true;
            Invalidate();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (allowKeyboardControl == false) { return base.ProcessCmdKey(ref msg, keyData); }
            if (keyData == Keys.Down) {
                if (Rows > 1) {
                    int Temp = KeyboardBit - 16;
                    if (Temp >= 0) {
                        KeyboardBit -= 16;
                    }

                    StartTimer();
                    Invalidate();
                }
                return true;
            }
            else if (keyData == Keys.Up) {
                if (Rows > 1) {
                    int Temp = KeyboardBit + 16;
                    if (Temp <= bits - 1) {
                        if (MaxBits > Temp) {
                            KeyboardBit += 16;
                        }
                    }

                    StartTimer();
                    Invalidate();
                }
                return true;
            }
            else if (keyData == Keys.Right) {
                if (KeyboardBit < 0) {
                    KeyboardBit = bits - 1;
                }
                else if (KeyboardBit == 0) {
                    KeyboardBit = bits - 1;
                }
                else { KeyboardBit--; }
                StartTimer();
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Left) {
                if (KeyboardBit < 0) {
                    KeyboardBit = bits - 1;
                }
                else if (KeyboardBit == bits - 1) {
                    KeyboardBit = 0;
                }
                else { KeyboardBit++; }
                StartTimer();
                Invalidate();
                return true;
            }
            else if (keyData == Keys.Space) {
                SetBit(KeyboardBit);
                StartTimer();
                Invalidate();
                return true;
            }
            else if (keyData == (Keys.Space & Keys.Control)) {
                if (allowKeyboardInvertControl) {
                    SetInvertBit(KeyboardBit);
                    StartTimer();
                    Invalidate();
                    return true;
                }
                else { return base.ProcessCmdKey(ref msg, keyData); }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        int TimeOutTick = 0;
        const int Timeout = 5;
        private void StartTimer() {
            TimeOutTick = 0;
            Tmr.Enabled = true;
        }
        private void Tmr_Tick(object? sender, EventArgs e) {
            if (TimeOutTick >= Timeout) {
                Tmr.Enabled = false;
                TimeOutTick = 0;
                KeyboardBit = -1;
                Invalidate();
            }
            else {
                TimeOutTick++;
            }
        }
        private void SetInvertBit(int Bit) {
            if (Bit >= bits) { return; }
            if (Bit < 0) { return; }
            string Output = "";
            string Temp = byteValue;
            if (Bit >= Temp.Length) {
                int SizeDifference = Bit - Temp.Length;
                string BuildStr = "";
                for (int i = SizeDifference; i >= 0; i--) {
                    if (i == SizeDifference) {
                        BuildStr += "1";
                    }
                    else {
                        BuildStr += "0";
                    }
                    Output = BuildStr + Temp;
                }
            }
            else {
                int Index = Temp.Length - 1 - Bit;
                for (int i = 0; i < Temp.Length; i++) {
                    if (Index != i) {
                        Output += Temp[i] == '0' ? "1" : "0";
                    }
                    else {
                        Output += Temp[i];
                    }
                }
            }
            Value = Output;
            BitToggled?.Invoke(this, Bit, Value);
        }
        private void SetBit(int Bit) {
            if (Bit >= bits) { return; }
            if (Bit < 0) { return; }
            string Output = "";
            string Temp = byteValue;
            if (Bit >= Temp.Length) {
                int SizeDifference = Bit - Temp.Length;
                string BuildStr = "";
                for (int i = SizeDifference; i >= 0; i--) {
                    if (i == SizeDifference) {
                        BuildStr += "1";
                    }
                    else {
                        BuildStr += "0";
                    }
                    Output = BuildStr + Temp;
                }
            }
            else {

                int Index = Temp.Length - 1 - Bit;
                for (int i = 0; i < Temp.Length; i++) {
                    if (Index == i) {
                        Output += Temp[i] == '0' ? "1" : "0";
                    }
                    else {
                        Output += Temp[i];
                    }
                }
            }
            Value = Output;
            BitToggled?.Invoke(this, Bit, Value);
        }
    }
}
