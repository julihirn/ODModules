using Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public class PainterEventEditor : UserControl {

        public event ValueChangedEventHandler? ValueChanged;

        public delegate void ValueChangedEventHandler();


        public PainterEventEditor() {
            AllowMouseWheel = true;
            ShadowColor = Color.FromArgb(128, 0, 0, 0);
            ColumnColor = Color.LightGray;
            SelectedColor = Color.SkyBlue;
            GridlineColor = Color.LightGray;
            SelectionColor = Color.Gray;
            RowColor = Color.LightGray;
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            ScrollOutofBounds = new System.Timers.Timer(10);
            ScrollOutofBounds.Elapsed += ScrollOutofBounds_Elapsed;
            MouseClick += PainterEventEditor_OnMouseClick;
            MouseDown += PainterEventEditor_OnMouseDown;
            MouseUp += PainterEventEditor_OnMouseUp;
            MouseWheel += PainterEventEditor_OnMouseWheel;
            MouseHover += PainterEventEditor_OnMouseHover;
            MouseEnter += PainterEventEditor_OnMouseEnter;
            MouseMove += PainterEventEditor_OnMouseMove;
            KeyDown += PainterEventEditor_OnKeyDown;
            KeyUp += PainterEventEditor_OnKeyUp;
        }
        #region Support Functions and Methods
        public int SelectedItems() {
            try {
                int cnt = 0;
                bool bol = false;
                for (int i = 0; i <= GetItemCount() - 1; i++) {
                    if (ItemIsSelected(i) == true) {
                        cnt += 1;
                        if (bol == false) {
                            _IndexCount = i;
                            //_CurrentString = items[i].Text;
                            bol = true;
                        }
                    }
                }
                _SelectionCount = cnt;
                _LineCount = GetItemCount();
                ValueChanged?.Invoke();
                return cnt;
            }
            catch {
                _SelectionCount = 0;
                return 0;
            }
        }
        private int GetItemCount() {
            if (linkedPainter == null) { return 0; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    return Painter.PaintedEvents.Count;
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    return Painter.PaintedEvents.Count;
                }
                else { return 0; }
            }
        }
        #endregion
        #region Properties
        private Control? linkedPainter;
        [System.ComponentModel.Category("Control")]
        public Control? LinkedPainter {
            get {
                return linkedPainter;
            }
            set {
                if (value == null) {
                    linkedPainter = null;
                }
                else {
                    if (value.GetType() == typeof(ValuePainter)) {
                        linkedPainter = value;
                    }
                    else if (value.GetType() == typeof(TimePainter)) {
                        linkedPainter = value;
                    }
                    else {
                        linkedPainter = null;
                    }
                }
                Invalidate();
            }
        }
        private List<PaintColumn> columns = new List<PaintColumn>();
        [System.ComponentModel.Category("List Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<PaintColumn> Columns {
            get {
                return columns;
            }
        }
        private Color _ShadowColor;
        [System.ComponentModel.Category("Appearance")]
        public Color ShadowColor {
            get {
                return _ShadowColor;
            }
            set {
                _ShadowColor = value;
                Invalidate();
            }
        }
        private Color _ColumnColor = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnColor {
            get {
                return _ColumnColor;
            }
            set {
                _ColumnColor = value;
                Invalidate();
            }
        }
        private Color _ColumnForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnForeColor {
            get {
                return _ColumnForeColor;
            }
            set {
                _ColumnForeColor = value;
                Invalidate();
            }
        }
        private Color _ColumnLineColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnLineColor {
            get {
                return _ColumnLineColor;
            }
            set {
                _ColumnLineColor = value;
                Invalidate();
            }
        }
        private Color _ColumnTickColor = Color.Gainsboro;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnTickColor {
            get {
                return _ColumnTickColor;
            }
            set {
                _ColumnTickColor = value;
                Invalidate();
            }
        }
        private Color _SelectedColor;
        [System.ComponentModel.Category("Appearance")]
        public Color SelectedColor {
            get {
                return _SelectedColor;
            }
            set {
                _SelectedColor = value;
                Invalidate();
            }
        }
        private Color _SelectionColor;
        [System.ComponentModel.Category("Appearance")]
        public Color SelectionColor {
            get {
                return _SelectionColor;
            }
            set {
                _SelectionColor = value;
                Invalidate();
            }
        }
        private Color _GridlineColor;
        [System.ComponentModel.Category("Appearance")]
        public Color GridlineColor {
            get {
                return _GridlineColor;
            }
            set {
                _GridlineColor = value;
                Invalidate();
            }
        }
        private Color _RowColor;
        [System.ComponentModel.Category("Appearance")]
        public Color RowColor {
            get {
                return _RowColor;
            }
            set {
                _RowColor = value;
                Invalidate();
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
        private bool _AllowMouseWheel;
        [System.ComponentModel.Category("Control")]
        public bool AllowMouseWheel {
            get {
                return _AllowMouseWheel;
            }
            set {
                _AllowMouseWheel = value;
            }
        }
        private bool _ShowGrid;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowGrid {
            get {
                return _ShowGrid;
            }
            set {
                _ShowGrid = value;
                Invalidate();
            }
        }
        private bool _ShowRowColors;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowRowColors {
            get {
                return _ShowRowColors;
            }
            set {
                _ShowRowColors = value;
                Invalidate();
            }
        }
        private bool _AllowColumnSpanning;
        [System.ComponentModel.Category("Appearance")]
        public bool AllowColumnSpanning {
            get {
                return _AllowColumnSpanning;
            }
            set {
                _AllowColumnSpanning = value;
                Invalidate();
            }
        }
        private int _SpanColumn;
        [System.ComponentModel.Category("Appearance")]
        public int SpanColumn {
            get {
                return _SpanColumn;
            }
            set {
                _SpanColumn = value;
                Invalidate();
            }
        }
        private int _VerScroll;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                if (_VerScroll < 0) {
                    return 0;
                }
                return _VerScroll;
            }
            set {
                if (value < 0) { _VerScroll = 0; }
                else {
                    if (linkedPainter == null) { _VerScroll = 0; }
                    else {
                        if (linkedPainter.GetType() == typeof(TimePainter)) {
                            TimePainter Painter = (TimePainter)linkedPainter;
                            if (value > Painter.PaintedEvents.Count - 1) {
                                _VerScroll = Painter.PaintedEvents.Count - 1;
                            }
                            else { _VerScroll = value; }
                        }
                        else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                            ValuePainter Painter = (ValuePainter)linkedPainter;
                            if (value > Painter.PaintedEvents.Count - 1) {
                                _VerScroll = Painter.PaintedEvents.Count - 1;
                            }
                            else { _VerScroll = value; }
                        }
                        else { _VerScroll = 0; }
                    }
                    if (InSelection == true) {
                        SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                        SelectValuesList(SELTEST);
                    }
                    Invalidate();
                }
            }
        }
        private void InvokeScrollCheck() {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (_VerScroll > Painter.PaintedEvents.Count - 1) {
                        if (Painter.PaintedEvents.Count > 0) {
                            _VerScroll = Painter.PaintedEvents.Count - 1;
                        }
                        else { _VerScroll = 0; }
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (_VerScroll > Painter.PaintedEvents.Count - 1) {
                        if (Painter.PaintedEvents.Count > 0) {
                            _VerScroll = Painter.PaintedEvents.Count - 1;
                        }
                        else { _VerScroll = 0; }
                    }
                }
            }
        }
        private decimal _VerScrollMax;
        [System.ComponentModel.Category("Scrolling")]
        public decimal VerScrollMax {
            get {
                return _VerScrollMax;
            }
        }
        private int _SelectionCount;
        [System.ComponentModel.Category("Values")]
        public int SelectionCount {
            get {
                return _SelectionCount;
            }
        }
        private int _LineCount;
        [System.ComponentModel.Category("Values")]
        public int LineCount {
            get {
                return _LineCount;
            }
        }
        private int _IndexCount;
        [System.ComponentModel.Category("Values")]
        public int IndexCount {
            get {
                return _IndexCount;
            }
        }
        private string _CurrentString = "";
        [System.ComponentModel.Category("Values")]
        public string CurrentString {
            get {
                return _CurrentString;
            }
        }
        private decimal _HorScroll;
        [System.ComponentModel.Category("Scrolling")]
        public decimal HorScroll {
            get {
                return _HorScroll;
            }
            set {
                if (value < 0)
                    _HorScroll = 0;
                else if (value > 100)
                    _HorScroll = 100;
                else
                    _HorScroll = value;
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Scrolling")]
        private int scrollItems = 3;
        public int ScrollItems {
            get {
                return scrollItems;
            }
            set {
                scrollItems = value;
            }
        }
        [System.ComponentModel.Category("Highlights")]
        int highlightalpha = 128;
        int highlightsidealpha = 78;
        public int HighlightStrength {
            get {
                return highlightalpha;
            }
            set {
                if (value < 20)
                    highlightalpha = 20;
                else if (value > 255)
                    highlightalpha = 255;
                else
                    highlightalpha = value;
                if (value < 50) {
                    highlightsidealpha = value + 50;
                }
                else {
                    highlightsidealpha = value - 50;
                }
                Invalidate();
            }
        }



        #endregion
        #region Render Setup
        int Offset = 5;
        int ColumnsTotalWidth = 0;
        int LineHeaderHeight = 0;
        int ScrollSize = 10;
        int GenericLine_Height = 0;
        int MaximumVerticalItems = 100;
        int ScrollXDifference = 0;
        int Xscroll = 0;
        int LineTextOffset = 0;

        decimal BarMinimum = 0;
        decimal BarMaximum = 100;
        private void RenderSetup(PaintEventArgs e) {
            using (System.Drawing.Font GenericSize = new System.Drawing.Font(Font.FontFamily, 9.0f, Font.Style)) {
                ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
            }
            int sz_genx = (int)e.Graphics.MeasureString("0", Font).Width - 2;
            GenericLine_Height = (int)e.Graphics.MeasureString("0", Font).Height;

            int HorizontalScrollHeight = 0;
            if (ShowHorzScroll == true) { HorizontalScrollHeight = ScrollSize; }
            MaximumVerticalItems = (int)Math.Floor((this.Height - LineHeaderHeight - HorizontalScrollHeight) / (double)GenericLine_Height);
            if (GetItemCount() < MaximumVerticalItems) {
                _VerScroll = 0;
            }
            int LineOffset = 2;
            LineHeaderHeight = (int)GenericLine_Height + (LineOffset * 3);
            InvokeScrollCheck();

            if (_AllowColumnSpanning == false) {
                ColumnsTotalWidth = MeasureColumns() + ScrollSize;
                if (ColumnsTotalWidth > this.Width) {
                    ScrollXDifference = ColumnsTotalWidth - Width;
                    // Xscroll = (int)((double)HorScroll / (double)100) * (ColumnsTotalWidth * sz_genx);
                    Xscroll = (int)_HorScroll;
                    ShowHorzScroll = true;
                }
                else {
                    ScrollXDifference = 0;
                    Xscroll = 0;
                    _HorScroll = 0;
                    ShowHorzScroll = false;
                }
            }
            else {
                ColumnsTotalWidth = MeasureColumnsExclusive();
                if ((_SpanColumn > -1) && (_SpanColumn < columns.Count)) {
                    columns[_SpanColumn].Width = Width - ColumnsTotalWidth;
                }
                ScrollXDifference = 0;
                Xscroll = 0;
                _HorScroll = 0;
                ShowHorzScroll = false;
            }
            LineTextOffset = 0 - (int)(((float)Xscroll / 100.0f) * (float)ScrollXDifference);

            SetMaxScroll();
            RetrieveRange();
        }
        #endregion
        #region Render
        protected override void OnPaint(PaintEventArgs e) {
            RenderList(e);
        }
        private void RenderList(PaintEventArgs e) {
            try {
                RenderSetup(e);
                int CurrentStartingLine = 0;

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                int CurrentLine = VerScroll;
                if (CurrentLine < 0) {
                    CurrentLine = 0;
                }
                int WindowSize = Width - ScrollSize;
                //if (CurrentStartingLine < items.Count) {
                for (int Line = CurrentStartingLine; Line < MaximumVerticalItems + 1; Line++) {
                    if (CurrentStartingLine < GetItemCount()) {
                        if (CurrentLine < GetItemCount()) {
                            Rectangle LineBounds = new Rectangle(0, ListLinePoint(Line), Width, GenericLine_Height);
                            RenderLine(e, CurrentLine, LineBounds);
                            CurrentLine++;
                        }
                    }
                    if (_ShowGrid) {
                        using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                            using (Pen LinePed = new Pen(LineBrush)) {
                                e.Graphics.DrawLine(LinePed, new Point(0, ListLinePoint(Line + 1)), new Point(WindowSize, ListLinePoint(Line + 1)));
                            }
                        }
                    }
                }
                //}
                if (InSelection == true && ShiftKey == false && CtrlKey == false) {
                    RenderSelectionRectangle(e, new Point(SelectionStart.X, ListLinePoint(SelectedItemstart)), SelectionEnd);
                }
                RenderHeader(e, LineHeaderHeight);
                RenderScrollBar(e);
            }
            catch { }
        }
        #endregion

        private void RenderLine(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            int LinePositionY = (int)(((float)BoundingRectangle.Height - (float)e.Graphics.MeasureString("0", Font).Height) / 2.0f) + BoundingRectangle.Y;
            RenderLineColouring(e, CurrentLine, BoundingRectangle);
            int Inc = 0;
            int Xpos = LineTextOffset;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    //RenderLineBackItem(e, Xpos, LinePositionY, i, CurrentLine);
                    if (columns[i].Source == PaintColumnDataSource.DisplayColor) {
                        Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[i].Width, (int)GenericLine_Height);
                        Color ItemColor = RetrieveColorData(CurrentLine);
                        using (SolidBrush TxtBrush = new SolidBrush(ItemColor)) {
                            e.Graphics.FillRectangle(TxtBrush, ItemRectangle);
                        }
                    }
                    RenderLineDataBar(e, Xpos, LinePositionY, i, CurrentLine);
                    RenderLineItem(e, Xpos, LinePositionY, i, CurrentLine);

                    Xpos += columns[i].Width;
                    Inc++;
                }
            }
        }
        private void RenderLineColouring(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            if (ShowRowColors == true) {
                if (CurrentLine % 2 == 0) {
                    using (SolidBrush AlternLineColor = new SolidBrush(RowColor)) {
                        e.Graphics.FillRectangle(AlternLineColor, BoundingRectangle);
                    }
                }
            }
            if (ItemIsSelected(CurrentLine) == true) {
                using (SolidBrush SelectedLine = new SolidBrush(SelectedColor)) {
                    e.Graphics.FillRectangle(SelectedLine, BoundingRectangle);
                }
            }
        }
        private void RenderLineItem(PaintEventArgs e, int Xpos, int LinePositionY, int Column, int Item) {
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            int TextOffset = Offset;
            RenderItemText(e, Xpos, LinePositionY, TextOffset, Column, Item);
        }
        private void RenderLineDataBar(PaintEventArgs e, int Xpos, int LinePositionY, int Column, int Item) {
            if (columns[Column].Source != PaintColumnDataSource.RangeEditor) { return; }
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);

            Rectangle BarRectangle = GetBarRectangle(ItemRectangle, Column, Item);
            BarRectangle.Inflate(0, -1);
            //highlightalpha
            Color BorderBarColor = RetrieveColorData(Item);
            Color FillBarColor = Color.FromArgb(highlightalpha, BorderBarColor.R, BorderBarColor.G, BorderBarColor.B);
            using (SolidBrush BarBrush = new SolidBrush(FillBarColor)) {
                e.Graphics.FillRectangle(BarBrush, BarRectangle);
            }
            using (SolidBrush BarBrush = new SolidBrush(BorderBarColor)) {
                using (Pen BarPen = new Pen(BarBrush)) {
                    e.Graphics.DrawRectangle(BarPen, BarRectangle);
                }
            }
        }
        private void RenderItemText(PaintEventArgs e, int Xpos, int LinePositionY, int TextOffset, int Column, int Item) {
            if (columns[Column].Source == PaintColumnDataSource.DisplayColor) { return; }
            else if (columns[Column].Source == PaintColumnDataSource.RangeEditor) { return; }
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            if (columns[Column].ItemAlignment != ItemTextAlignment.None) {
                Color ItemForeColor = ForeColor;
                Rectangle TextRectangle = Rectangle.Empty;
                string TextString = "";
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                    if (columns[Column].ItemAlignment == ItemTextAlignment.Left) { FormatFlags.Alignment = StringAlignment.Near; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Center) { FormatFlags.Alignment = StringAlignment.Center; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Right) { FormatFlags.Alignment = StringAlignment.Far; }
                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                    TextString = RetrieveData(columns[Column].Source, Item, columns[Column].PropertyName);
                    using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                        e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                    }
                }
            }
        }

        private Rectangle GetBarRectangle(Rectangle ItemRectangle, int Column, int Item) {
            decimal Start = 0;
            decimal End = 0;
            RetrieveRangeData(Item, out Start, out End);
            decimal BarDifference = BarMaximum - BarMinimum == 0 ? 1 : BarMaximum - BarMinimum;

            decimal BarStartRange = (Start - Math.Abs(BarMinimum)) / BarDifference;
            decimal BarEndRange = (End - Math.Abs(BarMinimum)) / BarDifference;
            decimal BarDifferenceRange = BarEndRange - BarStartRange;

            int BarStart = ItemRectangle.X + (int)(BarStartRange * (decimal)ItemRectangle.Width);
            int BarSize = (int)(BarDifferenceRange * (decimal)ItemRectangle.Width);

            return new Rectangle(BarStart, ItemRectangle.Y, BarSize, ItemRectangle.Height);
        }

        #region Render Selection
        private void RenderSelectionRectangle(PaintEventArgs e, Point StartPoint, Point EndPoint) {
            Point PBX = SelectionBounds(SelectionStart.X, EndPoint.X);
            Point PBY = SelectionBounds(SelectionStart.Y, EndPoint.Y);
            Color SelectionFillColor = Color.FromArgb(100, SelectionColor.R, SelectionColor.G, SelectionColor.B);
            using (SolidBrush SelectionFillBrush = new SolidBrush(SelectionFillColor)) {
                e.Graphics.FillRectangle(SelectionFillBrush, PBX.X, PBY.X, PBX.Y, PBY.Y);
            }
            using (SolidBrush SelectionBorderBrush = new SolidBrush(SelectionColor)) {
                using (Pen SelectionBorderPen = new Pen(SelectionBorderBrush)) {
                    e.Graphics.DrawRectangle(SelectionBorderPen, PBX.X, PBY.X, PBX.Y, PBY.Y);
                }
            }
        }
        #endregion 
        #region Render Header
        private void RenderHeader(PaintEventArgs e, int HeaderHeight) {
            using (SolidBrush HeaderBackBrush = new SolidBrush(ColumnColor)) {
                e.Graphics.FillRectangle(HeaderBackBrush, 0, 0, this.Width, HeaderHeight);
            }
            int Xpos = LineTextOffset;
            int Inc = 0;
            using (SolidBrush HeaderTextBrush = new SolidBrush(_ColumnForeColor)) {
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    for (int i = 0; i < columns.Count; i++) {
                        if (columns[i].Visible) {
                            if (columns[i].ColumnAlignment != ColumnTextAlignment.None) {
                                if (columns[i].ColumnAlignment == ColumnTextAlignment.Left) { FormatFlags.Alignment = StringAlignment.Near; }
                                else if (columns[i].ColumnAlignment == ColumnTextAlignment.Center) { FormatFlags.Alignment = StringAlignment.Center; }
                                else if (columns[i].ColumnAlignment == ColumnTextAlignment.Right) { FormatFlags.Alignment = StringAlignment.Far; }

                                if (columns[i].Source != PaintColumnDataSource.RangeEditor) {
                                    e.Graphics.DrawString(columns[i].Text, Font, HeaderTextBrush, new Rectangle(Xpos + Offset, 2, columns[i].Width - (2 * Offset), (int)GenericLine_Height), FormatFlags);
                                }
                                else {
                                    using (SolidBrush TickBrush = new SolidBrush(_ColumnTickColor)) {
                                        using (Pen TickPen = new Pen(TickBrush, 1)) {
                                            int BarWidth = columns[i].Width;
                                            int Ticks = (int)(10.0f * Math.Ceiling((float)BarWidth / 150.0f));
                                            int BarStart = Xpos;

                                            int LargeTick = (int)((float)GenericLine_Height / 1.6f);
                                            int SmallTick = (int)((float)GenericLine_Height / 1.1f);
                                            for (int j = 1; j < Ticks; j++) {
                                                int TickXPos = BarStart + (int)((float)BarWidth * ((float)j / (float)Ticks));
                                                if (j % 2 == 0) {
                                                    e.Graphics.DrawLine(TickPen, TickXPos, LargeTick, TickXPos, HeaderHeight);
                                                }
                                                else {
                                                    e.Graphics.DrawLine(TickPen, TickXPos, SmallTick, TickXPos, HeaderHeight);
                                                }

                                            }
                                        }
                                    }
                                }

                            }
                            if (Inc != 0) {
                                using (SolidBrush LineBrush = new SolidBrush(_ColumnLineColor)) {
                                    using (Pen LinePed = new Pen(LineBrush)) {
                                        e.Graphics.DrawLine(LinePed, new Point(Xpos, 0), new Point(Xpos, HeaderHeight));
                                    }
                                }
                                if (_ShowGrid) {
                                    using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                                        using (Pen LinePed = new Pen(LineBrush)) {
                                            e.Graphics.DrawLine(LinePed, new Point(Xpos, HeaderHeight), new Point(Xpos, Height));
                                        }
                                    }
                                }
                            }
                            Xpos += columns[i].Width;
                            Inc++;
                        }
                    }
                }
            }
            using (SolidBrush LineBrush = new SolidBrush(_ColumnLineColor)) {
                using (Pen LinePed = new Pen(LineBrush)) {
                    e.Graphics.DrawLine(LinePed, new Point(Xpos, 0), new Point(Xpos, HeaderHeight));
                }
            }
            if (_ShowGrid) {
                using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                    using (Pen LinePed = new Pen(LineBrush)) {
                        e.Graphics.DrawLine(LinePed, new Point(Xpos, HeaderHeight), new Point(Xpos, Height));
                    }
                }
            }
        }
        #endregion
        #region Render Scrollbars
        bool ShowVertScroll = true;
        bool ShowHorzScroll = true;
        Rectangle HorizontalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle HorizontalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF HorizontalScrollThumb = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF VerticalScrollThumb = new Rectangle(0, 0, 0, 0);
        int ScrollBarButtonSize = 0;
        private void RenderScrollBar(PaintEventArgs e) {
            Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
            if (ShowVertScroll == true) {
                VerticalScrollBar = new Rectangle(Width - ScrollSize, LineHeaderHeight, ScrollSize, Height - LineHeaderHeight);

                if (ShowHorzScroll == true) { VerticalScrollBar.Height -= ScrollSize; }

                using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, VerticalScrollBar);
                }
                RenderVerticalBar(e);
            }
            if (ShowHorzScroll == true) {
                HorizontalScrollBar = new Rectangle(0, Height - ScrollSize, Width, ScrollSize);
                if (ShowVertScroll == true) { HorizontalScrollBar.Width -= ScrollSize; }
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
            if ((ShowVertScroll == true) && (ShowHorzScroll == true)) {
                Rectangle Spacer = new Rectangle(Width - ScrollSize, Height - ScrollSize, ScrollSize, ScrollSize);
                using (SolidBrush HeaderBackBrush = new SolidBrush(BorderLineColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, Spacer);
                }
            }
        }
        private void RenderVerticalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(VerticalScrollBar, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                ScrollBarButtonSize = ScrollSize;
                VerticalScrollBounds = new Rectangle(VerticalScrollBar.X, VerticalScrollBar.Y + ScrollBarButtonSize, VerticalScrollBar.Width, VerticalScrollBar.Height - (2 * ScrollBarButtonSize));
                if (GetItemCount() > 0) {
                    float ViewableItems = ((float)MaximumVerticalItems / 2.0f) / (float)GetItemCount();
                    if (GetItemCount() < MaximumVerticalItems) {
                        ViewableItems = 1;
                    }
                    float ThumbHeight = ViewableItems * VerticalScrollBounds.Height;
                    if (ThumbHeight < ScrollBarButtonSize * 2) {
                        ThumbHeight = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)GetItemCount()) + VerticalScrollBounds.Y;// + ScrollSize;
                    VerticalScrollThumb = new RectangleF(VerticalScrollBounds.X, ScrollBounds, VerticalScrollBar.Width, ThumbHeight);
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollBounds);
                }
                Rectangle Button = new Rectangle(VerticalScrollBar.X, LineHeaderHeight, ScrollBarButtonSize, ScrollBarButtonSize);
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y + Button.Height), new Point(Button.X + Button.Width, Button.Y + Button.Height));
                        Button.Y = VerticalScrollBar.Height + LineHeaderHeight - Button.Height;
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y), new Point(Button.X + Button.Width, Button.Y));
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(VerticalScrollBar.X, VerticalScrollBar.Y), new Point(VerticalScrollBar.X, VerticalScrollBar.Y + VerticalScrollBar.Height));
                    }
                }
            }
        }
        private void RenderHorizontalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(Bounds, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                //e.Graphics.FillRectangle(HeaderForeBrush, VerticalBar);
                ScrollBarButtonSize = ScrollSize;
                HorizontalScrollBounds = new Rectangle(HorizontalScrollBar.X + ScrollBarButtonSize, HorizontalScrollBar.Y, HorizontalScrollBar.Width - (2 * ScrollBarButtonSize), HorizontalScrollBar.Height);
                //if (View == DIR_VIEW.ListLine) {
                if (columns.Count > 0) {
                    float WidthOverCurrent = 1;
                    if (ColumnsTotalWidth > 0) {
                        WidthOverCurrent = ColumnsTotalWidth;
                    }
                    float ViewableLines = (float)Width / (WidthOverCurrent);
                    float ThumbWidth = ViewableLines * HorizontalScrollBounds.Width;
                    if (ThumbWidth < ScrollBarButtonSize * 2) {
                        ThumbWidth = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (HorizontalScrollBounds.Width - ThumbWidth) * ((float)_HorScroll / 100.0f) + HorizontalScrollBounds.X;
                    HorizontalScrollThumb = new RectangleF(ScrollBounds, HorizontalScrollBounds.Y, ThumbWidth, HorizontalScrollBar.Height);
                    e.Graphics.FillRectangle(HeaderForeBrush, HorizontalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, HorizontalScrollBounds);
                }
                // }
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

        #region Interface Methods
        private void InvalidateLinked() {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    Painter.Invalidate();
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    Painter.Invalidate();
                }
            }
        }
        private void PerformCleanAndMerge() {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    Painter.CleanAndMerge();
                    // Painter.Invalidate();
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    Painter.CleanAndMerge();
                    //Painter.Invalidate();
                }
            }
            Invalidate();
        }
        #endregion
        #region Get Data
        private void RetrieveRange() {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    BarMinimum = Painter.SpanStart.Ticks;
                    BarMaximum = Painter.SpanEnd.Ticks;
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    BarMinimum = Painter.SpanStart;
                    BarMaximum = Painter.SpanEnd;
                }
                else { return; }
            }
        }
        private void RetrieveRangeData(int Item, out decimal Start, out decimal End) {
            if (linkedPainter == null) {
                Start = 0;
                End = 0;
            }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) {
                        Start = 0;
                        End = 0;
                        return;
                    }
                    Start = Painter.PaintedEvents[Item].TimeStart.Ticks;
                    End = Painter.PaintedEvents[Item].TimeEnd.Ticks;
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) {
                        Start = 0;
                        End = 0;
                        return;
                    }
                    Start = Painter.PaintedEvents[Item].ValueRangeStart;
                    End = Painter.PaintedEvents[Item].ValueRangeEnd;
                }
                else {
                    Start = 0;
                    End = 0;
                }
            }
        }
        private Color RetrieveColorData(int Item) {
            if (linkedPainter == null) { return Color.Transparent; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) { return Color.Transparent; }
                    return Painter.PaintedEvents[Item].DisplayColor;
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) { return Color.Transparent; }
                    return Painter.PaintedEvents[Item].DisplayColor;
                }
                else { return Color.Transparent; }
            }
        }
        private string RetrieveData(PaintColumnDataSource SourceType, int Item, string TagSearch) {
            if (linkedPainter == null) { return ""; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) { return ""; }
                    switch (SourceType) {
                        case PaintColumnDataSource.ID:
                            return Painter.PaintedEvents[Item].ID;
                        case PaintColumnDataSource.Member:
                            return Painter.PaintedEvents[Item].Member;
                        case PaintColumnDataSource.TagData:
                            return GetPropValue(Painter.PaintedEvents[Item].Tag, TagSearch);
                        case PaintColumnDataSource.StartValue:
                            return new DateTime(Painter.PaintedEvents[Item].TimeStart.Ticks).ToString("HH:mm:ss");
                        case PaintColumnDataSource.EndValue:
                            return new DateTime(Painter.PaintedEvents[Item].TimeEnd.Ticks).ToString("HH:mm:ss");
                        default:
                            return "";
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Item >= Painter.PaintedEvents.Count) { return ""; }
                    switch (SourceType) {
                        case PaintColumnDataSource.ID:
                            return Painter.PaintedEvents[Item].ID;
                        case PaintColumnDataSource.Member:
                            return Painter.PaintedEvents[Item].Member;
                        case PaintColumnDataSource.TagData:
                            return GetPropValue(Painter.PaintedEvents[Item].Tag, TagSearch);
                        case PaintColumnDataSource.StartValue:
                            return Painter.PaintedEvents[Item].ValueRangeStart.ToString("0.000");
                        case PaintColumnDataSource.EndValue:
                            return Painter.PaintedEvents[Item].ValueRangeEnd.ToString("0.000");
                        default:
                            return "";
                    }
                }
                else { return ""; }
            }
        }
        private string GetPropValue(object? src, string propName) {
            try {
                if (src == null) { return ""; }
                if (src.GetType() == null) { return ""; }
                if (src.GetType().GetProperty(propName) == null) { return ""; }
                PropertyInfo? ValType = src.GetType().GetProperty(propName);//.GetValue(src, null);
                if (ValType == null) { return ""; }
                else {
                    object? Value = ValType.GetValue(src, null);
                    if (Value == null) { return ""; }
                    return Value.ToString() ?? "";
                }
            }
            catch { return ""; }
        }
        #endregion
        #region Scroll Handling
        private System.Timers.Timer ScrollOutofBounds;
        private void SetMaxScroll() {
            _VerScrollMax = GetItemCount() + 1;
        }
        private int MeasureColumns() {
            int CurrentMax = 0;
            foreach (PaintColumn Col in columns) {
                if (Col.Visible == true) { CurrentMax += Col.Width; }
            }
            return CurrentMax;
        }
        private int MeasureColumnsExclusive() {
            int CurrentMax = 0;
            int i = 0;
            foreach (PaintColumn Col in columns) {
                if (i != _SpanColumn) {
                    if (Col.Visible == true) { CurrentMax += Col.Width; }
                }
                i++;
            }
            return CurrentMax;
        }
        private void ScrollOutofBounds_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            // theweh
            if ((VerScroll < VerScrollMax) || (VerScroll > 0)) {
                VerScroll += ScrollOutofBoundsDelta.Y;
            }
        }
        #endregion 
        #region Position Handling
        private int GetVerticalScrollFromCursor(int MousePositionY, float ThumbPosition) {
            return (int)((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition) * GetItemCount()) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
        }
        private int GetHorizontalScrollFromCursor(int MousePositionX, float ThumbPosition) {
            return (int)((float)((MousePositionX - HorizontalScrollBounds.X - ThumbPosition) * 100.0f) / (HorizontalScrollBounds.Width - HorizontalScrollThumb.Width));
        }

        #endregion
        #region Selection Handling
        private Point SELTEST = new Point(0, 0);
        private bool InSelection = false;
        private Point SelectionStart = new Point(0, 0);
        private Point MoveStart = new Point(0, 0);
        private Point SelectionEnd = new Point(0, 0);
        private int SelectedItemstart = -1;
        int FirstSelection = -1;
        int EditBarIndex = -1;
        private int ListLinePoint(int PositionY, PointLineCalcuation CalculationType = PointLineCalcuation.LineToPositiion) {
            if (CalculationType == PointLineCalcuation.LineToPositiion) {
                return (GenericLine_Height * PositionY) + LineHeaderHeight;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLine) {
                return (int)Math.Floor((float)(PositionY - LineHeaderHeight) / (float)GenericLine_Height) + VerScroll;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLineWithoutScroll) {
                return (int)((float)(PositionY - LineHeaderHeight) / (float)GenericLine_Height);
            }
            else if (CalculationType == PointLineCalcuation.LineToPositiionScrollFactored) {
                return (GenericLine_Height * (PositionY - VerScroll)) + LineHeaderHeight;
            }
            else {
                return 0;
            }
        }
        private Point SelectionBounds(int Starting, int Ending) {
            Point PNT = new Point(0, 0);
            if (Starting < Ending)
                PNT = new Point(Starting, Ending - Starting);
            else if (Starting > Ending)
                PNT = new Point(Ending, Starting - Ending);
            else if (Starting == Ending)
                PNT = new Point(0, 0);
            return PNT;
        }
        private int AbsMovement() {
            int Start = ListLinePoint((SelectionBounds(SelectionStart.Y, SELTEST.Y).X), PointLineCalcuation.PositionToLine);
            int Endl = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X + SelectionBounds(SelectionStart.Y, SELTEST.Y).Y, PointLineCalcuation.PositionToLine);
            int DIFF = Endl - Start;
            return DIFF;
        }
        private bool IsVaildSelectionBox() {
            bool Ret = false;
            if (SelectionBounds(SelectionStart.Y, SELTEST.Y).Y + 1 > GenericLine_Height) { Ret = true; }
            return Ret;
        }
        private void CursorClickSelect(Point MSPOS) {
            int SelectInt = -1;
            SelectInt = ListLinePoint(MSPOS.Y, PointLineCalcuation.PositionToLine);
            if (CtrlKey == true) {
                try {
                    if (ItemIsSelected(SelectInt) == true) { ItemSelect(SelectInt, false); }
                    else { ItemSelect(SelectInt, true); }
                }
                catch { }
            }
            else if (ShiftKey == true) {
                if (FirstSelection == -1) {
                    FirstSelection = SelectInt;
                }
                ClearSelected();
                if (SelectInt > FirstSelection) {
                    for (int i = FirstSelection; i <= SelectInt; i++) {
                        if ((i >= 0) && (i < GetItemCount())) { ItemSelect(i, true); }
                    }
                }
                else if (SelectInt < FirstSelection) {
                    for (int i = SelectInt; i <= FirstSelection; i++) {
                        if ((i >= 0) && (i < GetItemCount())) { ItemSelect(i, true); }
                    }
                }
                else { if ((SelectInt >= 0) && (SelectInt < GetItemCount())) { ItemSelect(SelectInt, true); } }
            }
            else if (CursorOutofBounds == false) {
                ClearSelected();
                if ((SelectInt >= 0) && (SelectInt < GetItemCount())) {
                    ItemSelect(SelectInt, true);
                    FirstSelection = SelectInt;
                }
            }
        }
        private void ClearSelected() {
            for (int i = 0; i < GetItemCount(); i++) {
                if (ItemIsSelected(i) == true)
                    ItemSelect(i, false);
            }
        }
        private void SelectValuesList(Point MSPOS, bool LockOnMouseMove = false) {
            try {
                bool Result = IsVaildSelectionBox();
                if (Result == true) {
                    Result = CursorMoveSelect();
                }
                if (Result == false) {
                    if (LockOnMouseMove == false) {
                        CursorClickSelect(MSPOS);
                    }
                }
            }
            catch { }
            Invalidate();
        }
        private Rectangle GetItemRectangle(int Line, int Column) {
            int LineY = ListLinePoint(Line, PointLineCalcuation.LineToPositiionScrollFactored);
            int ColumnEnd = LineTextOffset;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    if (i == Column) {
                        return new Rectangle(ColumnEnd, LineY, columns[i].Width, (int)GenericLine_Height);
                    }
                    ColumnEnd += columns[i].Width;
                }
            }
            return new Rectangle(-1, -1, 0, 0);
        }
        private int GetColumn(Point MouseLocation) {
            int SelectedColumn = -1;
            int ColumnStart = 0;
            int ColumnEnd = LineTextOffset;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    ColumnEnd += columns[i].Width;
                    ColumnStart = ColumnEnd - columns[i].Width;
                    if ((MouseLocation.X >= ColumnStart) && (MouseLocation.X < ColumnEnd)) {
                        SelectedColumn = i;
                        break;
                    }
                }
            }
            return SelectedColumn;
        }
        #endregion 
        #region Selection Support
        private bool ItemIsSelected(int Item) {
            if (linkedPainter == null) { return false; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        return Painter.PaintedEvents[Item].Selected;
                    }
                    else { return false; }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        return Painter.PaintedEvents[Item].Selected;
                    }
                    else { return false; }
                }
                else { return false; }
            }
        }
        private void LatchItemPosition(int Item) {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        Painter.PaintedEvents[Item].LatchCurrentPosition();
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        Painter.PaintedEvents[Item].LatchCurrentPosition();
                    }
                }
            }
        }
        private void ItemSelect(int Item, bool Select) {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        Painter.PaintedEvents[Item].Selected = Select;
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    if (Painter.PaintedEvents.Count > Item) {
                        Painter.PaintedEvents[Item].Selected = Select;
                    }
                }
            }
        }
        public void LineClearSelection(bool Render = true) {
            for (int i = 0; i <= GetItemCount() - 1; i++) {
                ItemSelect(i, false);
            }
            if (Render == true) {
                Invalidate();
            }
        }
        public void LineSingleSelect(int Index) {
            ItemSelect(Index, true);
            Invalidate();
        }
        public void CentreLine(int Line, bool SelectLine = true, bool ClearSelection = false) {
            if (Line < 0) { return; }
            if (GetItemCount() < MaximumVerticalItems) {
                VerScroll = 0;
            }
            else {
                //int First = GetFirstSelected();
                if (Line == VerScroll) {
                    VerScroll = Line;
                }
                else if (Line >= (VerScroll + MaximumVerticalItems)) {
                    VerScroll = Line - MaximumVerticalItems;
                }
                else if (Line < VerScroll) {
                    VerScroll = Line;
                }
                else if ((Line >= VerScroll) && (Line <= (VerScroll + MaximumVerticalItems))) {

                }
            }
            if (ClearSelection == true)
                LineClearSelection(false);
            if (SelectLine == true)
                LineSingleSelect(Line);
        }
        private bool CursorMoveSelect() {
            int Start = -1;
            int Endl = -1;
            if (CursorOutofBounds == false) {
                ClearSelected();
            }
            Start = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X, PointLineCalcuation.PositionToLine);
            Endl = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X + SelectionBounds(SelectionStart.Y, SELTEST.Y).Y, PointLineCalcuation.PositionToLine);
            if (Endl > GetItemCount() - 1) { Endl = GetItemCount() - 1; }
            if (GetItemCount() > 1) {
                for (int i = Start; i <= Endl; i++) {
                    if ((i >= 0) && (i < GetItemCount())) { ItemSelect(i, true); }
                }
            }
            return true;
        }
        #endregion
        #region Editing Support
        private void BarEdit(Point MouseLocation) {
            int NormalisedLocation = MouseLocation.X - CurrentCell.X;
            if (BarEditType == EditType.LeftResize) {
                this.Cursor = Cursors.SizeWE;
                ResizeBar((decimal)NormalisedLocation / (decimal)CurrentCell.Width, false);
            }
            else if (BarEditType == EditType.RightResize) {
                this.Cursor = Cursors.SizeWE;
                ResizeBar((decimal)NormalisedLocation / (decimal)CurrentCell.Width, true);
            }
            else if (BarEditType == EditType.Move) {
                this.Cursor = Cursors.SizeAll;
                int NormalisedLocationMove = MoveStart.X - CurrentCell.X;
                int NormalisedLocationStart = MouseLocation.X - CurrentCell.X;
                MoveBar((decimal)(NormalisedLocationStart - NormalisedLocationMove) / (decimal)CurrentCell.Width);
            }
            Invalidate();
        }
        private void MoveBar(decimal CursorPosition) {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    long SpanStart = Painter.SpanStart.Ticks;
                    long SpanEnd = Painter.SpanEnd.Ticks;
                    if ((Painter.PaintedEvents.Count > EditBarIndex) && (EditBarIndex >= 0)) {
                        long LatchedStart = Painter.PaintedEvents[EditBarIndex].BeforeMovedStart.Ticks;
                        long SpanRange = SpanEnd - SpanStart;

                        long Current = LatchedStart + (long)((decimal)SpanRange * CursorPosition);//(long)(CursorPosition * (decimal)(SpanEnd - SpanStart)) + SpanStart;
                        long LatchedEventDifference = Painter.PaintedEvents[EditBarIndex].BeforeMovedEnd.Ticks - LatchedStart;
                        if (Current < SpanStart) { Current = SpanStart; }
                        if ((Current + LatchedEventDifference) > SpanEnd) { Current = SpanEnd - LatchedEventDifference; }
                        Painter.PaintedEvents[EditBarIndex].TimeStart = new TimeSpan(Current);
                        Painter.PaintedEvents[EditBarIndex].TimeEnd = new TimeSpan(Current + LatchedEventDifference);
                        Painter.Invalidate();
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    decimal SpanStart = Painter.SpanStart;
                    decimal SpanEnd = Painter.SpanEnd;
                    if ((Painter.PaintedEvents.Count > EditBarIndex) && (EditBarIndex >= 0)) {
                        decimal LatchedStart = Painter.PaintedEvents[EditBarIndex].BeforeMovedStart;
                        decimal SpanRange = SpanEnd - SpanStart;

                        decimal Current = LatchedStart + (SpanRange * CursorPosition);//(long)(CursorPosition * (decimal)(SpanEnd - SpanStart)) + SpanStart;
                        decimal LatchedEventDifference = Painter.PaintedEvents[EditBarIndex].BeforeMovedEnd - LatchedStart;
                        //decimal Current = (CursorPosition * (SpanEnd - SpanStart)) + SpanStart;
                        //decimal LatchedEventDifference = Painter.PaintedEvents[EditBarIndex].BeforeMovedEnd - Painter.PaintedEvents[EditBarIndex].BeforeMovedStart;
                        if (Current < SpanStart) { Current = SpanStart; }

                        if ((Current + LatchedEventDifference) > SpanEnd) { Current = SpanEnd - LatchedEventDifference; }
                        Painter.PaintedEvents[EditBarIndex].ValueRangeStart = Current;
                        Painter.PaintedEvents[EditBarIndex].ValueRangeEnd = Current + LatchedEventDifference;
                        Painter.Invalidate();
                    }
                }
            }
        }
        private void ResizeBar(decimal CursorPosition, bool RightResize = false) {
            if (linkedPainter == null) { return; }
            else {
                if (linkedPainter.GetType() == typeof(TimePainter)) {
                    TimePainter Painter = (TimePainter)linkedPainter;
                    long SpanStart = Painter.SpanStart.Ticks;
                    long SpanEnd = Painter.SpanEnd.Ticks;
                    if ((Painter.PaintedEvents.Count > EditBarIndex) && (EditBarIndex >= 0)) {
                        long Current = (long)(CursorPosition * (decimal)(SpanEnd - SpanStart)) + SpanStart;
                        if (Current < SpanStart) { Current = SpanStart; }
                        else if (Current > SpanEnd) { Current = SpanEnd; }

                        if (RightResize == true) {
                            long Minimum = Painter.PaintedEvents[EditBarIndex].TimeStart.Ticks;
                            if (Current < Minimum) { Current = Minimum; }
                            Painter.PaintedEvents[EditBarIndex].TimeEnd = new TimeSpan(Current);
                        }
                        else {
                            long Minimum = Painter.PaintedEvents[EditBarIndex].TimeEnd.Ticks;
                            if (Current > Minimum) { Current = Minimum; }
                            Painter.PaintedEvents[EditBarIndex].TimeStart = new TimeSpan(Current);
                        }
                        Painter.Invalidate();
                    }
                }
                else if (linkedPainter.GetType() == typeof(ValuePainter)) {
                    ValuePainter Painter = (ValuePainter)linkedPainter;
                    decimal SpanStart = Painter.SpanStart;
                    decimal SpanEnd = Painter.SpanEnd;
                    if ((Painter.PaintedEvents.Count > EditBarIndex) && (EditBarIndex >= 0)) {
                        decimal Current = (CursorPosition * (SpanEnd - SpanStart)) + SpanStart;
                        if (Current < SpanStart) { Current = SpanStart; }
                        else if (Current > SpanEnd) { Current = SpanEnd; }

                        if (RightResize == true) {
                            decimal Minimum = Painter.PaintedEvents[EditBarIndex].ValueRangeStart;
                            if (Current < Minimum) { Current = Minimum; }
                            Painter.PaintedEvents[EditBarIndex].ValueRangeEnd = Current;
                        }
                        else {
                            decimal Minimum = Painter.PaintedEvents[EditBarIndex].ValueRangeEnd;
                            if (Current > Minimum) { Current = Minimum; }
                            Painter.PaintedEvents[EditBarIndex].ValueRangeStart = Current;
                        }
                        Painter.Invalidate();
                    }
                }
            }
        }
        private bool HitEventBox(Point MouseLocation) {
            int SelectedLine = -1;
            SelectedLine = ListLinePoint(MouseLocation.Y, PointLineCalcuation.PositionToLine);
            int SelectedColumn = GetColumn(MouseLocation);
            if (MouseLocation.Y > LineHeaderHeight) {
                if ((columns.Count > 0) && (SelectedColumn > -1) && (SelectedColumn < columns.Count)) {
                    if (columns[SelectedColumn].Source == PaintColumnDataSource.RangeEditor) {
                        if ((SelectedLine < GetItemCount()) && (SelectedLine > -1)) {
                            Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                            if (GetBarRectangle(CurrentItem, SelectedColumn, SelectedLine).Contains(MouseLocation)) {
                                //CheckChange(SelectedLine, SelectedColumn);
                                LatchItemPosition(SelectedLine);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private Rectangle EventBoxRectangle(Point MouseLocation) {
            int SelectedLine = -1;
            SelectedLine = ListLinePoint(MouseLocation.Y, PointLineCalcuation.PositionToLine);
            int SelectedColumn = GetColumn(MouseLocation);
            if (MouseLocation.Y > LineHeaderHeight) {
                if ((columns.Count > 0) && (SelectedColumn > -1) && (SelectedColumn < columns.Count)) {
                    if (columns[SelectedColumn].Source == PaintColumnDataSource.RangeEditor) {
                        if ((SelectedLine < GetItemCount()) && (SelectedLine > -1)) {
                            Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                            return GetBarRectangle(CurrentItem, SelectedColumn, SelectedLine);
                        }
                    }
                }
            }
            return new Rectangle(-1, -1, 0, 0);
        }
        private Rectangle GetCellRectangle(Point MouseLocation) {
            int SelectedLine = -1;
            SelectedLine = ListLinePoint(MouseLocation.Y, PointLineCalcuation.PositionToLine);
            EditBarIndex = SelectedLine;
            int SelectedColumn = GetColumn(MouseLocation);
            if (MouseLocation.Y > LineHeaderHeight) {
                if ((columns.Count > 0) && (SelectedColumn > -1) && (SelectedColumn < columns.Count)) {
                    if (columns[SelectedColumn].Source == PaintColumnDataSource.RangeEditor) {
                        if ((SelectedLine < GetItemCount()) && (SelectedLine > -1)) {
                            Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                            return CurrentItem;
                        }
                    }
                }
            }
            return new Rectangle(-1, -1, 0, 0);
        }
        #endregion 
        #region Mouse Events
        bool InScrollBounds = false;
        bool ScrollStart = false;
        ScrollArea ScrollHit = ScrollArea.None;
        float ThumbDelta = 0;

        private enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        bool BarEditEvent = false;
        bool HitHeader = false;
        int HitStart = 0;
        int SelectedColumn = -1;
        int OldWidth = 0;
        bool IgnoreLines = false;
        Point ScrollOutofBoundsDelta = new Point(0, 0);
        Rectangle CurrentCell = new Rectangle(-1, -1, 0, 0);
        private bool CursorOutofBounds = false;
        private void PainterEventEditor_OnMouseClick(object? sender, MouseEventArgs e) {
            if (IgnoreLines == true) { return; }
            if ((InScrollBounds == true) && (e.X >= Width - ScrollSize)) {
                InScrollBounds = true;
                float ThumbDeltaTest = e.Y - VerticalScrollThumb.Y;
                if (ThumbDeltaTest < 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, 0);
                    Invalidate();
                }
                else if (ThumbDeltaTest > VerticalScrollThumb.Height) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, 0);
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (e.Y >= Height - ScrollSize)) {
                InScrollBounds = true;
                if (ShowHorzScroll == true) {
                    float ThumbDeltaTest = e.X - HorizontalScrollThumb.X;
                    if (ThumbDeltaTest < 0) {
                        HorScroll = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                    else if (ThumbDeltaTest > HorizontalScrollThumb.Width) {
                        HorScroll = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                }
            }
            if (InScrollBounds == false) {
                if (e.Button == MouseButtons.Left) {
                    SelectValuesList(e.Location);
                }
            }
            //base.OnMouseClick(e);
        }
        private void PainterEventEditor_OnMouseEnter(object? sender, EventArgs e) {
            //base.OnMouseEnter(e);
        }
        private void PainterEventEditor_OnMouseHover(object? sender, EventArgs e) {
            //base.OnMouseHover(e);
        }
        private enum EditType {
            NoChange = 0x00,
            LeftResize = 0x01,
            Move = 0x02,
            RightResize = 0x03
        }
        EditType BarEditType = EditType.NoChange;
        private void PainterEventEditor_OnMouseMove(object? sender, MouseEventArgs e) {
            this.Cursor = Cursors.Default;
            if ((e.Y < LineHeaderHeight) && (HitHeader == false)) {
                int Xpos = LineTextOffset;
                this.Cursor = DefaultCursor;
                for (int i = 0; i < columns.Count; i++) {
                    if (columns[i].Visible) {
                        Xpos += columns[i].Width;
                        if ((e.X > Xpos - 5) && (e.X < Xpos + 5)) {
                            this.Cursor = Cursors.SizeWE;
                            break;
                        }
                    }
                }
            }
            else {
                if (HitHeader == false) {
                    this.Cursor = DefaultCursor;
                }

            }
            if (HitHeader == true) {
                if ((SelectedColumn < columns.Count) && (columns.Count > 0)) {
                    columns[SelectedColumn].Width = OldWidth + e.X - HitStart;
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Vertical)) {
                if (GetItemCount() > 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, ThumbDelta);
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Horizontal)) {
                if (columns.Count > 0) {
                    HorScroll = GetHorizontalScrollFromCursor(e.X, ThumbDelta);
                    Invalidate();
                }
            }
            else {
                if (BarEditEvent == false) {
                    if (e.Location.Y > this.Height) {
                        ScrollOutofBoundsDelta.Y = (int)((float)(e.Location.Y - this.Height) / (float)(ScrollItems * 10));
                        CursorOutofBounds = true;
                        ScrollOutofBounds.Enabled = true;
                    }
                    else if (e.Location.Y < 0) {
                        ScrollOutofBoundsDelta.Y = (int)((float)(e.Location.Y) / (float)(ScrollItems * 10));
                        ScrollOutofBounds.Enabled = true;
                        CursorOutofBounds = true;
                    }
                    else {
                        CursorOutofBounds = false;
                        ScrollOutofBounds.Enabled = false;
                    }

                    SELTEST = new Point(e.Location.X, e.Location.Y);
                    if (InSelection == true) {
                        SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                        SelectionEnd = new Point(e.Location.X, e.Location.Y);
                        SelectValuesList(e.Location, true);
                    }
                    else {
                        Rectangle HitBox = EventBoxRectangle(e.Location);
                        if ((e.X > HitBox.X - 5) && (e.X < HitBox.X + 5)) {
                            this.Cursor = Cursors.SizeWE;
                            BarEditType = EditType.LeftResize;
                        }
                        else if ((e.X > HitBox.Right - 5) && (e.X < HitBox.Right + 5)) {
                            this.Cursor = Cursors.SizeWE;
                            BarEditType = EditType.RightResize;
                        }
                        else if ((e.X > HitBox.Left) && (e.X < HitBox.Right)) {
                            this.Cursor = Cursors.SizeAll;
                            BarEditType = EditType.Move;
                        }
                    }
                    SelectedItems();
                }
                else {
                    BarEdit(e.Location);
                }
            }
            //base.OnMouseMove(e);
        }
        private void PainterEventEditor_OnMouseUp(object? sender, MouseEventArgs e) {
            // timer.Stop()
            if (e.Button == MouseButtons.Left) {
                if (InScrollBounds == false) {
                    if (IgnoreLines == false) {
                        InSelection = false;
                        SelectedItemstart = -1;
                        Invalidate();
                        SelectedItems();
                    }
                }
            }
            HitHeader = false;
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            ScrollOutofBounds.Enabled = false;
            IgnoreLines = false;
            if (BarEditEvent == true) {
                PerformCleanAndMerge();
            }
            BarEditEvent = false;
            BarEditType = EditType.NoChange;
            //base.OnMouseUp(e);
            InvalidateLinked();
        }
        private void PainterEventEditor_OnMouseDown(object? sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (e.Y < LineHeaderHeight) {
                    int Xpos = LineTextOffset;
                    for (int i = 0; i < columns.Count; i++) {
                        if (columns[i].Visible) {
                            Xpos += columns[i].Width;
                            if ((e.X > Xpos - 5) && (e.X < Xpos + 5)) {
                                HitHeader = true;
                                OldWidth = columns[i].Width;
                                SelectedColumn = i;
                                break;
                            }
                        }
                    }
                    IgnoreLines = true;
                    HitStart = e.X;
                }
                else if ((ShowVertScroll == true) && (e.X >= Width - ScrollSize)) {
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
                else if ((ShowHorzScroll == true) && (e.Y > Height - ScrollSize)) {
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
                else {
                    ScrollHit = ScrollArea.None;
                    InScrollBounds = false;

                    if (CtrlKey == false & ShiftKey == false) {
                        ClearSelected();
                    }
                    if (InSelection == false) {
                        SelectionStart = new Point(e.Location.X, e.Location.Y);
                        SelectionEnd = new Point(e.Location.X, e.Location.Y);
                        SelectedItemstart = ListLinePoint(e.Location.Y, PointLineCalcuation.PositionToLine);
                        InSelection = true;
                    }
                    if (HitEventBox(e.Location) == false) {
                        SelectedItems();
                    }
                    else {
                        BarEditEvent = true;
                        MoveStart = e.Location;
                        CurrentCell = GetCellRectangle(e.Location);
                    }
                }
            }
            //base.OnMouseDown(e);
        }
        private void PainterEventEditor_OnMouseWheel(object? sender, MouseEventArgs e) {
            if (CtrlKey == false && ShiftKey == false) {
                int D = e.Delta;
                int DC = ScrollItems * (int)Math.Abs((double)D / (double)120);
                if (D > 0) {
                    if (VerScroll > 0)
                        VerScroll -= DC;
                }
                else if (VerScroll < VerScrollMax)
                    VerScroll += Math.Abs(DC);
            }
            //base.OnMouseWheel(e);
        }
        #endregion
        #region Key Events
        private const int WM_KEYDOWN = 0x100;
        private bool ShiftKey = false;
        private bool CtrlKey = false;
        protected override bool ProcessKeyPreview(ref System.Windows.Forms.Message m) {
            bool Processed = false; // Set to true, if we have "consumed" this keystroke
            switch (m.Msg) // This Select can be longer - it's an example
            {
                case WM_KEYDOWN: {
                        switch (m.WParam.ToInt32()) // WParam it is a virtual key code, in this case
                        {
                            case (int)Keys.Escape: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.Down: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.Up: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.ShiftKey: {
                                    Processed = true;
                                    break;
                                }
                            default:
                                return false;
                        }
                        break;
                    }
                default:
                    return false;
            }
            if (!Processed) {
                Processed = base.ProcessKeyPreview(ref m);
            }

            return Processed;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Down) {
                SelectedItems();
                int inst = 0;
                if (IndexCount >= GetItemCount() - 1)
                    inst = GetItemCount() - 1;
                else
                    inst = IndexCount + 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                    Focus();
                }
                else
                    CentreLine(inst, true, true);
                Invalidate();
                InvalidateLinked();
                return true;
            }
            if (keyData == Keys.Up) {
                SelectedItems();
                int inst = 0;
                if (IndexCount <= 0)
                    inst = 0;
                else
                    inst = IndexCount - 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                }
                else
                    CentreLine(inst, true, true);
                Invalidate();
                InvalidateLinked();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void PainterEventEditor_OnKeyDown(object? sender, KeyEventArgs e) {
            ShiftKey = e.Shift;
            CtrlKey = e.Control;
            if (CtrlKey == false && ShiftKey == false) {
                if (e.KeyCode == Keys.Up) {
                }
            }
            Invalidate();
            // base.OnKeyDown(e);
        }
        private void PainterEventEditor_OnKeyUp(object? sender, KeyEventArgs e) {
            ShiftKey = e.Shift;
            CtrlKey = e.Control;
            //base.OnKeyUp(e);
        }
        #endregion
        #region Control Event
        protected override void OnResize(EventArgs e) {
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            using (SolidBrush BackBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillRectangle(BackBrush, new Rectangle(0, 0, Width, Height));
            }
        }
        #endregion
        private enum PointLineCalcuation {
            LineToPositiion = 0x00,
            PositionToLine = 0x01,
            PositionToLineWithoutScroll = 0x02,
            LineToPositiionScrollFactored = 0x03
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // PainterEventEditor
            // 
            this.Name = "PainterEventEditor";
            this.Load += new System.EventHandler(this.PainterEventEditor_Load);
            this.ResumeLayout(false);

        }

        private void PainterEventEditor_Load(object sender, EventArgs e) {

        }
    }

    public class PaintColumn {
        private string text;
        [System.ComponentModel.Category("Appearance")]
        public string Text { get => text; set => text = value; }
        [System.ComponentModel.Category("Layout")]
        private int width = 20;
        public int Width {
            get { return width; }
            set {
                if (value < 50) {
                    width = 50;
                }
                else {
                    width = value;
                }
            }
        }
        private bool visible = true;
        [System.ComponentModel.Category("Appearance")]
        public bool Visible { get => visible; set => visible = value; }
        ItemTextAlignment itemAlignment = ItemTextAlignment.Left;
        [System.ComponentModel.Category("Appearance")]
        public ItemTextAlignment ItemAlignment {
            get { return itemAlignment; }
            set {
                itemAlignment = value;
            }
        }
        ColumnTextAlignment columnAlignment = ColumnTextAlignment.Left;
        [System.ComponentModel.Category("Appearance")]
        public ColumnTextAlignment ColumnAlignment {
            get { return columnAlignment; }
            set {
                columnAlignment = value;
            }
        }
        PaintColumnDataSource source = PaintColumnDataSource.ID;
        [System.ComponentModel.Category("Data")]
        public PaintColumnDataSource Source {
            get { return source; }
            set {
                source = value;
            }
        }
        string propertyName = "";
        [System.ComponentModel.Category("Data")]
        public string PropertyName {
            get { return propertyName; }
            set {
                propertyName = value;
            }
        }
        //bool useItemForeColor = false;
        //[System.ComponentModel.Category("Appearance")]
        //public bool UseItemForeColor {
        //    get { return useItemForeColor; }
        //    set {
        //        useItemForeColor = value;
        //    }
        //}
        //bool useItemBackColor = false;
        //[System.ComponentModel.Category("Appearance")]
        //public bool UseItemBackColor {
        //    get { return useItemBackColor; }
        //    set {
        //        useItemBackColor = value;
        //    }
        //}
        public PaintColumn() {
            this.text = "Column";
        }
        public PaintColumn(string text) {
            this.text = text;
        }
        public PaintColumn(string text, int Width) {
            this.text = text;
            this.width = Width;
        }




    }
    public enum PaintColumnDataSource {
        DisplayColor = 0x00,
        ID = 0x01,
        Member = 0x02,
        StartValue = 0x03,
        EndValue = 0x04,
        TagData = 0x05,
        RangeEditor = 0x06
    }
}
