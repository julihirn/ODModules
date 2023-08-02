
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Xml;
using Handlers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ODModules {
    [DefaultEvent("CommandEntered")]
    public class ConsoleInterface : UserControl {
        #region Control Methods
        System.Windows.Forms.Timer FlashTimer = new System.Windows.Forms.Timer();
        private List<TerminalLine> Lines = new List<TerminalLine>();
        private List<TerminalHistory> History = new List<TerminalHistory>();
        [System.ComponentModel.Category("Commands")]
        public event CommandEnteredEventHandler? CommandEntered;
        public delegate void CommandEnteredEventHandler(object? sender, CommandEnteredEventArgs e);
        public ConsoleInterface() {
            DoubleBuffered = true;
            FlashTimer.Tick += FlashTimer_Tick;
            FlashTimer.Enabled = true;
            FlashTimer.Interval = 500;
            MouseClick += OnMouseClick;

            //for (int i = 0; i < 2; i++) {
            //    Lines.Add(new TerminalLine("COM1", i.ToString()));
            //}
        }
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // ConsoleInterface
            // 
            this.Name = "ConsoleInterface";
            this.Load += new System.EventHandler(this.ConsoleInterface_Load);
            this.ResumeLayout(false);

            secondaryFont = this.Font;

        }
        #endregion
        #region Console Functionality
        public string PushToString() {
            string output = "";
            foreach (TerminalLine line in Lines) {
                output += line.Line + Handlers.Constants.NewLineEnv;
            }
            return output;
        }
        public int GetLineCount() {
            return Lines.Count;
        }
        public void Clear() {
            try {
                Lines.Clear();
                VerScroll = 0;
            }
            catch { }
        }
        public void EditLine(string Text, int Line) {
            if ((Line >= 0) && (Line < Lines.Count) && (Lines.Count > 0)) {
                Lines[Line].Line = Text;
                Invalidate();
            }
        }
        public void EditLastLine(string Text) {
            if (Lines.Count > 0) {
                Lines[Lines.Count - 1].Line = Text;
                Invalidate();
            }
        }
        public void AttendToLine(string Text, int Line) {
            if ((Line >= 0) && (Line < Lines.Count) && (Lines.Count > 0)) {
                Lines[Line].Line += Text;
                Invalidate();
            }
        }
        public void AttendToLastLine(string Text, bool CheckForNewLine = true) {
            if ((Lines.Count > 0)) {

                Lines[Lines.Count - 1].Line += Text;
                if (CheckForNewLine == true) {
                    string Temp = Lines[Lines.Count - 1].Line;
                    if (Temp.Contains('\n')) {
                        Temp = Temp.Replace("\r", "");
                        List<string> LinesToPrint = SpiltString(Temp, '\n');
                        if (LinesToPrint.Count == 1) {
                            Lines[Lines.Count - 1].Line = LinesToPrint[0];
                        }
                        else if (LinesToPrint.Count > 1) {
                            Lines[Lines.Count - 1].Line = LinesToPrint[0];
                            for (int i = 1; i < LinesToPrint.Count; i++) {
                                Print(LinesToPrint[i]);
                            }
                        }
                    }
                }
                Invalidate();
            }
            else {
                Print(Text);
            }
        }
        public void AttendToLastLine(string Source, string Text, bool CheckForNewLine = true) {
            if ((Lines.Count > 0)) {
                bool LastLineIsSource = Lines[Lines.Count - 1].Source == Source ? true : false;
                if (LastLineIsSource == true) {
                    if (Lines[Lines.Count - 1].Line.Length > longLine) {
                        Print(Source, Text);
                        return;
                    }
                    Text = Text.Replace("\0", "");

                    Lines[Lines.Count - 1].Line += Text;

                    if (CheckForNewLine == true) {
                        string Temp = Lines[Lines.Count - 1].Line;
                        Temp = Temp.Replace("\u0084", "\n");
                        if (Temp.Contains('\n')) {
                            Temp = Temp.Replace("\r", "");
                            List<string> LinesToPrint = SpiltString(Temp, '\n');
                            if (LinesToPrint.Count == 1) {
                                Lines[Lines.Count - 1].Line = LinesToPrint[0];
                            }
                            else if (LinesToPrint.Count > 1) {
                                Lines[Lines.Count - 1].Line = LinesToPrint[0];
                                for (int i = 1; i < LinesToPrint.Count; i++) {
                                    Print(Source, LinesToPrint[i]);
                                }
                            }
                        }
                    }
                }
                else {
                    Print(Source, Text);
                }
                Invalidate();
            }
            else {
                Print(Source, Text);
            }
        }
        private int FindLastWithSource(string Source) {
            int LinesCount = Lines.Count - 1;
            int LineNumber = -1;
            try {
                for (int i = LinesCount; i >= 0; i--) {
                    if (Lines[i].Source == Source) {
                        LineNumber = i;
                        break;
                    }
                }
            }
            catch { }
            return LineNumber;
        }
        public void Print(string Text) {
            Print("", Text);
        }
        public void Print(string Text, Color DisplayColor) {
            Print("", Text, DisplayColor);
        }
        public void Print(string Source, string Text) {
            BufferManagement();
            int OldLineCount = Lines.Count;
            int LineDifference = 1;
            Text = Text.Replace("\r", "");
            List<string> LinesToPrint = SpiltString(Text, '\n');
            int Diff = 0;
            for (int i = 0; i < LinesToPrint.Count; i++) {
                string Temp = LinesToPrint[i];
                if (Temp.Length <= longLine) {
                    TerminalLine Line = new TerminalLine(Source, Temp);
                    Lines.Add(Line);
                }
                else {
                    Diff += SplitLongLine(Temp, Source, false, BackColor) - 1;
                }
            }
            if (allowCommandEntry == true) { OldLineCount += 1; }
            LineDifference = LinesToPrint.Count + Diff;
            if (OldLineCount == TotalWindowLines + _VerScroll) {
                if (Lines.Count - VerScroll < TotalWindowLines) { }
                else { 
                    VerScroll += LineDifference;
                }
            }
            else {
                Invalidate();
            }
        }
        public void Print(string Source, string Text, Color DisplayColor) {
            BufferManagement();
            int OldLineCount = Lines.Count;
            int LineDifference = 1;
            Text = Text.Replace("\r", "");
            List<string> LinesToPrint = SpiltString(Text, '\n');
            int Diff = 0;
            for (int i = 0; i < LinesToPrint.Count; i++) {
                string Temp = LinesToPrint[i];
                if (Temp.Length <= longLine) {
                    TerminalLine Line = new TerminalLine(Source, Temp, DisplayColor);
                    Lines.Add(Line);
                }
                else {
                    Diff += SplitLongLine(Temp, Source, true, DisplayColor) - 1;
                }
            }
            if (allowCommandEntry == true) { OldLineCount += 1; }
            LineDifference = LinesToPrint.Count + Diff;
            if ((OldLineCount >= TotalWindowLines + _VerScroll - 2) || (OldLineCount <= TotalWindowLines + _VerScroll + 2)) {
                if (Lines.Count - VerScroll < TotalWindowLines) { }
                else { VerScroll += LineDifference; }
            }
            else {
                Invalidate();
            }
        }
        private int SplitLongLine(string Input, string Source, bool UseDisplayColor, Color DisplayColor) {
            int Start = 0;
            int End = longLine;
            int i = 0;
            while(End < Input.Length) {
                int Length = longLine;
                if (Length + Start > Input.Length) {
                    Length = Input.Length - Start;
                }
                string Temp = Input.Substring(Start, Length);
                Start += longLine;
                End += longLine;
                //if (End >= Input.Length - 1) {
                //    End = Input.Length - 1;
                //}
                i++;
                if (UseDisplayColor) {
                    TerminalLine Line = new TerminalLine(Source, Temp, DisplayColor);
                    Lines.Add(Line);
                }
                else {
                    TerminalLine Line = new TerminalLine(Source, Temp);
                    Lines.Add(Line);
                }
            }
            return i;
        }
        private void BufferManagement() {
            try {
                if (Lines.Count > bufferLength) {
                    int Diff = Lines.Count - bufferLength;
                    if (Diff > 0) {
                        for (int i = Diff - 1; i >= 0; i--) {
                            Lines.RemoveAt(i);
                        }
                    }
                }
                if (VerScroll > bufferLength - 1) {
                    VerScroll = bufferLength - 1;
                }
            }
            catch { }
        }
        #endregion
        #region Properties 
        private float cursorFlashSpeed = 0.5f;
        [System.ComponentModel.Category("Control")]
        public float CursorFlashSpeed {
            get {
                return cursorFlashSpeed;
            }
            set {
                if (value < 0.41) {
                    cursorFlashSpeed = 0.41f;
                }
                else {
                    cursorFlashSpeed = value;
                }
                FlashTimer.Interval = (int)(cursorFlashSpeed * 1000.0f);
            }
        }
        private int longLine = 200;
        [System.ComponentModel.Category("Control")]
        public int MaximumLength {
            get {
                return longLine;
            }
            set {
                if (value < 80) {
                    longLine = 80;
                }
                else if (value > 1000) {
                    longLine = 1000;
                }
                else {
                    longLine = value;
                }
            }
        }
        private bool flashCursor = true;
        [System.ComponentModel.Category("Control")]
        public bool FlashCursor {
            get {
                return flashCursor;
            }
            set {
                flashCursor = value;
                if (value == true) {
                    FlashTimer.Enabled = true;
                }
                else {
                    FlashTimer.Enabled = false;
                    CursorState = true;
                }
                Invalidate();
            }
        }
        private bool extraLineAfterCommandEntered = true;
        [System.ComponentModel.Category("Control")]
        public bool ExtraLineAfterCommandEntered {
            get {
                return extraLineAfterCommandEntered;
            }
            set {
                extraLineAfterCommandEntered = value;
            }
        }
        private int zoom = 100;
        [System.ComponentModel.Category("Appearance")]
        public int Zoom {
            get {
                return zoom;
            }
            set {
                if (value < 10) {
                    zoom = 10;
                }
                else if (value > 1000) {
                    zoom = 1000;
                }
                else {
                    zoom = value;
                }

                Invalidate();
            }
        }
        private TimeStampFormat timeStamps = TimeStampFormat.NoTimeStamps;
        [System.ComponentModel.Category("Appearance")]
        public TimeStampFormat TimeStamps {
            get {
                return timeStamps;
            }
            set {
                timeStamps = value;
                Invalidate();
            }
        }
        private Color timeStampForeColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color TimeStampForeColor {
            get {
                return timeStampForeColor;
            }
            set {
                timeStampForeColor = value;
                Invalidate();
            }
        }
        private bool showOrigin = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowOrigin {
            get {
                return showOrigin;
            }
            set {
                showOrigin = value;
                Invalidate();
            }
        }
        private Color originForeColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color OriginForeColor {
            get {
                return originForeColor;
            }
            set {
                originForeColor = value;
                Invalidate();
            }
        }
        private bool printOnEntry = true;
        [System.ComponentModel.Category("Control")]
        public bool PrintOnEntry {
            get {
                return printOnEntry;
            }
            set {
                printOnEntry = value;
            }
        }
        private bool allowCommandEntry;
        [System.ComponentModel.Category("Control")]
        public bool AllowCommandEntry {
            get {
                return allowCommandEntry;
            }
            set {
                allowCommandEntry = value;
                Invalidate();
            }
        }
        private bool allowMouseWheelZoom;
        [System.ComponentModel.Category("Control")]
        public bool AllowMouseWheelZoom {
            get {
                return allowMouseWheelZoom;
            }
            set {
                allowMouseWheelZoom = value;
            }
        }
        private bool allowMouseSelection;
        [System.ComponentModel.Category("Control")]
        public bool AllowMouseSelection {
            get {
                return allowMouseSelection;
            }
            set {
                allowMouseSelection = value;
                Invalidate();
            }
        }
        private bool lineFormatting;
        [System.ComponentModel.Category("Appearance")]
        public bool LineFormatting {
            get {
                return lineFormatting;
            }
            set {
                lineFormatting = value;
                Invalidate();
            }
        }
        private Color selectionColor = Color.FromArgb(47, 47, 74);
        [System.ComponentModel.Category("Appearance")]
        public Color SelectionColor {
            get {
                return selectionColor;
            }
            set {
                selectionColor = value;
                Invalidate();
            }

        }
        private Font? secondaryFont = null;
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
        Point selectionStart = new Point(1, 1);
        [System.ComponentModel.Category("Selection")]
        Point SelectionStart {
            get { return selectionStart; }
            set {
                int NewX = value.X;
                int NewY = value.Y;
                if (value.X > selectionEnd.X) {
                    NewX = selectionEnd.X;
                    selectionEnd.X = value.X;
                }
                if (value.Y > selectionEnd.Y) {
                    NewY = selectionEnd.Y;
                    selectionEnd.Y = value.Y;
                }
                selectionStart = new Point(NewX, NewY);
            }
        }
        Point selectionEnd = new Point(5, 2);
        [System.ComponentModel.Category("Selection")]
        Point SelectionEnd {
            get { return selectionEnd; }
            set {
                int NewX = value.X;
                int NewY = value.Y;
                if (value.X < selectionStart.X) {
                    NewX = selectionStart.X;
                    selectionStart.X = value.X;
                }
                if (value.Y < selectionStart.Y) {
                    NewY = selectionStart.Y;
                    selectionStart.Y = value.Y;
                }
                selectionEnd = new Point(NewX, NewY);
            }
        }
        private int bufferLength = 10000;
        [System.ComponentModel.Category("Appearance")]
        public int BufferLength {
            get {
                return bufferLength;
            }
            set {
                if (value < 500) {
                    bufferLength = 500;
                }
                else if (value > 10000) {
                    bufferLength = 10000;
                }
                else {
                    bufferLength = value;
                }
            }
        }
        [System.ComponentModel.Category("Scrolling")]
        public decimal VerScrollMax {
            get {
                if (Lines.Count <= 0) {
                    if (Lines.Count >= TotalWindowLines) {
                        return Lines.Count + TotalWindowLines;
                    }
                    else {
                        return 0;
                    }
                }
                else {
                    return 0;
                }
            }
        }
        private int _VerScroll;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                return _VerScroll;
            }
            set {
                if (value < 0)
                    _VerScroll = 0;
                else {
                    if (Lines.Count == 0) {
                        _VerScroll = 0;
                    }
                    else {
                        if (value >= Lines.Count - 1) {
                            _VerScroll = Lines.Count - 1;
                        }
                        else {
                            _VerScroll = value;
                        }
                    }
                }
                Invalidate();
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
        #endregion
        #region Drawing 
        int TotalWindowLines = 10;
        string CommandString = "";
        int UnitTextHeight = 10;
        int UnitTextWidth = 10;
        int CommandEntryLine = 0;

        int UnitPadding = 0;
        int HalfUnitWidth = 0;
        private void RenderSetup(PaintEventArgs e) {
            try {
                if (secondaryFont != null) {
                    using (System.Drawing.Font GenericSize = new System.Drawing.Font(secondaryFont.FontFamily, 9.0f, Font.Style)) {
                        ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
                    }
                }
            }
            catch { }
        }
        protected override void OnPaint(PaintEventArgs e) {
            RenderSetup(e);
            using (Font ConsoleFont = new Font(Font.Name, Font.Size * ((float)zoom / 100.0f))) {
                //int GenericWidth = (int)e.Graphics.MeasureString("0").Width;
                //using (StringFormat Sf = new StringFormat()) {
                //   Sf = 
                //StringFormat.GenericTypographic

                if (allowMouseSelection == true) {
                    UnitTextWidth = (int)e.Graphics.MeasureString("W", ConsoleFont, int.MaxValue, StringFormat.GenericTypographic).Width + 1;
                    //UnitTextHeight = (int)e.Graphics.MeasureString("W", ConsoleFont, int.MaxValue, StringFormat.GenericTypographic).Height;
                    UnitTextHeight = (int)e.Graphics.MeasureString("W", ConsoleFont, int.MaxValue).Height;
                }
                else {
                    UnitTextWidth = (int)e.Graphics.MeasureString("W", ConsoleFont, int.MaxValue).Width;
                    UnitTextHeight = (int)e.Graphics.MeasureString("W", ConsoleFont, int.MaxValue).Height;
                }
                //}
                HalfUnitWidth = (UnitTextWidth / 2);
                UnitPadding = UnitTextWidth / 4;
                TotalWindowLines = (int)(Math.Floor((float)Height - Padding.Top - Padding.Bottom) / (float)UnitTextHeight);


                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                bool DoNotShowCursor = false;
                if (allowCommandEntry == true) {
                    if (VerScroll + TotalWindowLines < Lines.Count + 1) {
                        DoNotShowCursor = true;
                        CommandEntryLine = 0;
                    }
                    else {
                        CommandEntryLine = 1;
                    }
                }
                else { CommandEntryLine = 0; }


                if (allowMouseSelection == true) {
                    DrawSelectableTerminal(e, ConsoleFont, DoNotShowCursor);
                }
                else {
                    DrawBasicTerminal(e, ConsoleFont, DoNotShowCursor);
                }
            }
            RenderScrollBar(e);
        }
        private void DrawBasicTerminal(PaintEventArgs e, Font ConsoleFont, bool DoNotShowCursor) {
            int LastLine = (TotalWindowLines + 1) - CommandEntryLine;
            int LastLineCount = 0;
            try {
                for (int i = 0; i < LastLine; i++) {
                    int Line = VerScroll + i;
                    if ((Line >= 0) && (Line < Lines.Count)) {
                        Color LineColor = GetLineColor(Line, ForeColor);
                        if (lineFormatting == false) {
                            DrawBasisLine(e, LineColor, ConsoleFont, Line, i);
                        }
                        else {
                            DrawBasisFormattedLine(e, LineColor, ConsoleFont, Line, i);
                        }
                        LastLineCount++;
                    }
                }
            }
            catch { }
            if ((allowCommandEntry == true) && (DoNotShowCursor == false)) {
                int LastLineLength = 0;
                using (StringFormat Formatting = new StringFormat()) {
                    Formatting.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
                    LastLineLength = (int)e.Graphics.MeasureString(CommandString, ConsoleFont, new Point(UnitPadding + Padding.Left, 0), Formatting).Width + UnitPadding + Padding.Left;
                }
                int CursorLine = 0;
                if (LastLineCount < TotalWindowLines) {
                    CursorLine = LastLineCount;
                }
                using (StringFormat Formatting = new StringFormat()) {
                    Formatting.Trimming = StringTrimming.None;
                    Formatting.LineAlignment = StringAlignment.Center;
                    Formatting.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
                    int IntY = (UnitTextHeight * CursorLine) + Padding.Top;
                    using (SolidBrush TextBrush = new SolidBrush(ForeColor)) {
                        e.Graphics.DrawString(CommandString, ConsoleFont, TextBrush, new Point(UnitPadding + Padding.Left, IntY));
                        if (CursorState == true) {
                            e.Graphics.FillRectangle(TextBrush, new Rectangle(LastLineLength, IntY, UnitTextWidth, UnitTextHeight));
                        }
                    }
                }
            }
        }
        private void DrawBasisLine(PaintEventArgs e, Color LineColor, Font ConsoleFont, int Line, int i) {
            using (StringFormat Formatting = new StringFormat()) {
                Formatting.Trimming = StringTrimming.None;
                Formatting.LineAlignment = StringAlignment.Center;
                Formatting.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
                using (SolidBrush TextBrush = new SolidBrush(LineColor)) {
                    string Output = "";
                    switch (timeStamps) {
                        case TimeStampFormat.Time:
                            Output += Lines[Line].TimeStamp.ToString("HH:mm:ss") + " - "; break;
                        case TimeStampFormat.TimeWithMilliseconds:
                            Output += Lines[Line].TimeStamp.ToString("HH:mm:ss.fff") + " - "; break;
                        case TimeStampFormat.DateTime:
                            Output += Lines[Line].TimeStamp.ToString("dd/MM/yyyy HH:mm:ss") + " - "; break;
                        case TimeStampFormat.Date:
                            Output += Lines[Line].TimeStamp.ToString("dd/MM/yyyy") + " - "; break;
                        default:
                            break;
                    }
                    Output += Lines[Line].Line;
                    int LineY = (i * UnitTextHeight) + Padding.Top;
                    e.Graphics.DrawString(Output, ConsoleFont, TextBrush, new Point(UnitPadding + Padding.Left, LineY));
                }
            }
        }
        private void DrawBasisFormattedLine(PaintEventArgs e, Color LineColor, Font ConsoleFont, int Line, int i) {
            using (StringFormat Formatting = new StringFormat()) {
                Formatting.Trimming = StringTrimming.None;
                Formatting.LineAlignment = StringAlignment.Center;
                Formatting.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
                string DateStamp = "";
                int LineOffset = 0;
                switch (timeStamps) {
                    case TimeStampFormat.Time:
                        DateStamp = Lines[Line].TimeStamp.ToString("HH:mm:ss") + " - "; break;
                    case TimeStampFormat.TimeWithMilliseconds:
                        DateStamp = Lines[Line].TimeStamp.ToString("HH:mm:ss.fff") + " - "; break;
                    case TimeStampFormat.DateTime:
                        DateStamp = Lines[Line].TimeStamp.ToString("dd/MM/yyyy HH:mm:ss") + " - "; break;
                    case TimeStampFormat.Date:
                        DateStamp = Lines[Line].TimeStamp.ToString("dd/MM/yyyy") + " - "; break;
                    default:
                        break;
                }
                if (showOrigin == true) {
                    string Source = Lines[Line].Source;
                    if (Source != "") {
                        using (SolidBrush TextBrush = new SolidBrush(originForeColor)) {
                            int LineY = (i * UnitTextHeight) + Padding.Top;
                            e.Graphics.DrawString(Source, ConsoleFont, TextBrush, new Point(UnitPadding + Padding.Left + LineOffset, LineY));

                            LineOffset += (int)e.Graphics.MeasureString(Source, ConsoleFont).Width;
                        }
                    }
                }
                if (DateStamp != "") {
                    using (SolidBrush TextBrush = new SolidBrush(timeStampForeColor)) {
                        string Output = DateStamp;
                        int LineY = (i * UnitTextHeight) + Padding.Top;
                        e.Graphics.DrawString(Output, ConsoleFont, TextBrush, new Point(UnitPadding + Padding.Left + LineOffset, LineY));

                        LineOffset += (int)e.Graphics.MeasureString(Output, ConsoleFont).Width;
                    }
                }
                using (SolidBrush TextBrush = new SolidBrush(LineColor)) {
                    string Output = Lines[Line].Line;
                    int LineY = (i * UnitTextHeight) + Padding.Top;
                    e.Graphics.DrawString(Output, ConsoleFont, TextBrush, new Point(UnitPadding + Padding.Left + LineOffset, LineY));
                }
            }
        }
        private void DrawSelectableTerminal(PaintEventArgs e, Font ConsoleFont, bool DoNotShowCursor) {
            DrawSelection(e);
            int LastLine = TotalWindowLines - CommandEntryLine;
            int LastLineCount = 0;
            int TextCentring = UnitTextWidth / 4;
            using (StringFormat Formatting = new StringFormat()) {
                Formatting.Trimming = StringTrimming.None;
                Formatting.LineAlignment = StringAlignment.Center;
                Formatting.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;

                for (int i = 0; i < LastLine; i++) {
                    int Line = VerScroll + i;
                    if ((Line >= 0) && (Line < Lines.Count)) {
                        Color LineColor = GetLineColor(Line, ForeColor);
                        using (SolidBrush TextBrush = new SolidBrush(LineColor)) {
                            string Output = "";
                            switch (timeStamps) {
                                case TimeStampFormat.Time:
                                    Output += Lines[Line].TimeStamp.ToString("HH:mm:ss") + " - "; break;
                                case TimeStampFormat.TimeWithMilliseconds:
                                    Output += Lines[Line].TimeStamp.ToString("HH:mm:ss.fff") + " - "; break;
                                case TimeStampFormat.DateTime:
                                    Output += Lines[Line].TimeStamp.ToString("dd/MM/yyyy HH:mm:ss") + " - "; break;
                                case TimeStampFormat.Date:
                                    Output += Lines[Line].TimeStamp.ToString("dd/MM/yyyy") + " - "; break;
                                default:
                                    break;
                            }
                            Output += Lines[Line].Line;
                            int Xpos = UnitPadding + Padding.Left;
                            int LineY = (i * UnitTextHeight) + Padding.Top;
                            for (int j = 0; j < Output.Length; j++) {
                                e.Graphics.DrawString(Output[j].ToString(), ConsoleFont, TextBrush, new Rectangle(Xpos, LineY, UnitTextWidth, UnitTextHeight), Formatting);
                                Xpos += UnitTextWidth;
                            }

                        }
                        LastLineCount++;
                    }
                }
            }
            if ((allowCommandEntry == true) && (DoNotShowCursor == false)) {
                int LastLineLength = 0;
                int CursorLine = 0;
                if (LastLineCount < TotalWindowLines) {
                    CursorLine = LastLineCount;
                }
                using (StringFormat Formatting = new StringFormat()) {
                    Formatting.Trimming = StringTrimming.None;
                    Formatting.Alignment = StringAlignment.Near;
                    Formatting.LineAlignment = StringAlignment.Center;
                    Formatting.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
                    using (SolidBrush TextBrush = new SolidBrush(ForeColor)) {
                        int Xpos = UnitPadding + Padding.Left;
                        int Ypos = UnitTextHeight * CursorLine;
                        for (int j = 0; j < CommandString.Length; j++) {
                            Rectangle TextRectangle = new Rectangle(Xpos, Ypos, UnitTextWidth, UnitTextHeight);
                            Rectangle TextBoxRectangle = new Rectangle(TextRectangle.X + TextCentring, TextRectangle.Y, TextRectangle.Width, TextRectangle.Height);
                            e.Graphics.DrawString(CommandString[j].ToString(), ConsoleFont, TextBrush, TextRectangle, Formatting);
                            //using (Pen TextPen = new Pen(TextBrush)) {
                            //    e.Graphics.DrawRectangle(TextPen, TextBoxRectangle);
                            //}
                            Xpos += UnitTextWidth;
                        }
                        // LastLineLength = Xpos + UnitTextWidth - UnitPadding;
                        LastLineLength = Xpos + TextCentring;

                        if (CursorState == true) {
                            e.Graphics.FillRectangle(TextBrush, new Rectangle((int)(LastLineLength + 0.5f), UnitTextHeight * CursorLine, UnitTextWidth, UnitTextHeight));
                        }
                    }
                }
            }

        }
        #endregion
        #region Drawing Support 
        bool CursorState = true;
        private Color GetLineColor(int Line, Color Default) {
            try {
                if (Lines.Count > 0) {
                    if ((Line >= 0) && (Line < Lines.Count)) {
                        if (Lines[Line].UseConsoleColor == true) {
                            return Default;
                        }
                        else {
                            return Lines[Line].ForeColor;
                        }
                    }
                }
            }
            catch { }
            return Default;
        }
        #endregion
        #region Render Scrollbars
        int LineHeaderHeight = 0;
        int ScrollSize = 10;
        bool ShowVertScroll = true;
        bool ShowHorzScroll = false;
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
            //if (ShowHorzScroll == true) {
            //    HorizontalScrollBar = new Rectangle(0, Height - ScrollSize, Width, ScrollSize);
            //    if (ShowVertScroll == true) { HorizontalScrollBar.Width -= ScrollSize; }
            //    using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
            //        e.Graphics.FillRectangle(HeaderBackBrush, HorizontalScrollBar);
            //    }
            //    RenderHorizontalBar(e);
            //    using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
            //        using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
            //            e.Graphics.DrawLine(ScrollBarBorderPen, new Point(HorizontalScrollBar.X, HorizontalScrollBar.Y), new Point(HorizontalScrollBar.Width, HorizontalScrollBar.Y));
            //        }
            //    }
            //}
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
                if (Lines.Count > 0) {
                    float ViewableItems = ((float)TotalWindowLines / 2.0f) / (float)Lines.Count;
                    if (Lines.Count < TotalWindowLines) {
                        ViewableItems = 1;
                    }
                    float ThumbHeight = ViewableItems * VerticalScrollBounds.Height;
                    if (ThumbHeight < ScrollBarButtonSize * 2) {
                        ThumbHeight = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)Lines.Count) + VerticalScrollBounds.Y;// + ScrollSize;
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
        #endregion
        #region Selection Support
        private void DrawSelection(PaintEventArgs e) {
            if (allowMouseSelection) {
                if (SelectionStart.X < 0) { return; }
                if (SelectionStart.Y < 0) { return; }
                if (SelectionEnd.X < 0) { return; }
                if (SelectionEnd.Y < 0) { return; }
                if (SelectionStart.Y == SelectionEnd.Y) {
                    SelectionBox Start = SelectionCoordinates(SelectionStart);
                    SelectionBox End = SelectionCoordinates(SelectionEnd);
                    using (SolidBrush Br = new SolidBrush(selectionColor)) {
                        if (Start.State == SelectionBoxState.InBounds) {
                            int SelWidth = End.Coordinates.X - Start.Coordinates.X;
                            int SelHeight = UnitTextHeight;
                            Rectangle Rect = new Rectangle(Start.Coordinates.X, Start.Coordinates.Y, SelWidth, SelHeight);
                            e.Graphics.FillRectangle(Br, Rect);
                        }
                    }
                }
                else if (SelectionStart.Y < SelectionEnd.Y) {
                    SelectionBox Start = SelectionCoordinates(SelectionStart);
                    SelectionBox End = SelectionCoordinates(SelectionEnd);
                    if (Start.State == SelectionBoxState.OutOfBoundsAbove) {
                        if (End.State == SelectionBoxState.OutOfBoundsBelow) {
                            DrawSelectionFill(e, Start, End);
                        }
                        else if (End.State == SelectionBoxState.InBounds) {
                            DrawSelectionClippedAbove(e, Start, End);
                        }
                    }
                    else if (Start.State == SelectionBoxState.InBounds) {
                        if (End.State == SelectionBoxState.InBounds) {
                            DrawSelectionInBounds(e, Start, End);
                        }
                        else if (End.State == SelectionBoxState.OutOfBoundsBelow) {
                            DrawSelectionClippedBelow(e, Start, End);
                        }
                    }
                }
            }
        }
        private void DrawSelectionClippedBelow(PaintEventArgs e, SelectionBox SelectionStartBox, SelectionBox SelectionEndBox) {
            using (SolidBrush Br = new SolidBrush(selectionColor)) {
                int LastLine = TotalWindowLines - CommandEntryLine;
                int FirstSelWidth = Width - SelectionStartBox.Coordinates.X;
                int LastSelWidth = SelectionEndBox.Coordinates.X;
                int SelHeight = UnitTextHeight;
                Rectangle FirstRect = new Rectangle(SelectionStartBox.Coordinates.X, SelectionStartBox.Coordinates.Y, FirstSelWidth, SelHeight);
                if (SelectionStart.Y == VerScroll + LastLine - 1) {
                    e.Graphics.FillRectangle(Br, FirstRect);
                }
                else {
                    SelectionBox MiddleStart = SelectionCoordinates(new Point(0, SelectionStart.Y + 1));
                    SelectionBox MiddleEnd = SelectionCoordinates(new Point(0, SelectionEnd.Y));
                    int MiddleHeight = MiddleEnd.Coordinates.Y - MiddleStart.Coordinates.Y;
                    Rectangle MiddleRect = new Rectangle(MiddleStart.Coordinates.X, MiddleStart.Coordinates.Y, Width, MiddleHeight);
                    e.Graphics.FillRectangle(Br, MiddleRect);
                    e.Graphics.FillRectangle(Br, FirstRect);
                }
            }
        }
        private void DrawSelectionClippedAbove(PaintEventArgs e, SelectionBox SelectionStartBox, SelectionBox SelectionEndBox) {
            using (SolidBrush Br = new SolidBrush(selectionColor)) {
                int LastSelWidth = SelectionEndBox.Coordinates.X - SelectionStartBox.Coordinates.X;
                int SelHeight = UnitTextHeight;
                Rectangle LastRect = new Rectangle(SelectionStartBox.Coordinates.X, SelectionEndBox.Coordinates.Y, LastSelWidth, SelHeight);


                if (SelectionEnd.Y == VerScroll) {
                    e.Graphics.FillRectangle(Br, LastRect);
                }
                else {
                    SelectionBox MiddleStart = SelectionCoordinates(new Point(0, SelectionStart.Y + 1));
                    SelectionBox MiddleEnd = SelectionCoordinates(new Point(0, SelectionEnd.Y));
                    int MiddleHeight = MiddleEnd.Coordinates.Y - MiddleStart.Coordinates.Y;
                    Rectangle MiddleRect = new Rectangle(MiddleStart.Coordinates.X, MiddleStart.Coordinates.Y, Width, MiddleHeight);
                    e.Graphics.FillRectangle(Br, MiddleRect);
                    e.Graphics.FillRectangle(Br, LastRect);
                }
            }
        }
        private void DrawSelectionInBounds(PaintEventArgs e, SelectionBox SelectionStartBox, SelectionBox SelectionEndBox) {
            using (SolidBrush Br = new SolidBrush(selectionColor)) {
                int FirstSelWidth = Width - SelectionStartBox.Coordinates.X;
                int LastSelWidth = SelectionEndBox.Coordinates.X - UnitPadding - HalfUnitWidth;
                int SelHeight = UnitTextHeight;
                Rectangle FirstRect = new Rectangle(SelectionStartBox.Coordinates.X, SelectionStartBox.Coordinates.Y, FirstSelWidth, SelHeight);
                Rectangle LastRect = new Rectangle(UnitPadding, SelectionEndBox.Coordinates.Y, LastSelWidth, SelHeight);
                e.Graphics.FillRectangle(Br, FirstRect);
                e.Graphics.FillRectangle(Br, LastRect);
                if ((SelectionEnd.Y - SelectionStart.Y) > 1) {
                    SelectionBox MiddleStart = SelectionCoordinates(new Point(0, SelectionStart.Y + 1));
                    SelectionBox MiddleEnd = SelectionCoordinates(new Point(0, SelectionEnd.Y));
                    int MiddleHeight = MiddleEnd.Coordinates.Y - MiddleStart.Coordinates.Y;
                    Rectangle MiddleRect = new Rectangle(MiddleStart.Coordinates.X, MiddleStart.Coordinates.Y, Width, MiddleHeight);
                    e.Graphics.FillRectangle(Br, MiddleRect);
                }
            }
        }
        private void DrawSelectionFill(PaintEventArgs e, SelectionBox SelectionStartBox, SelectionBox SelectionEndBox) {
            using (SolidBrush Br = new SolidBrush(selectionColor)) {
                int FirstSelWidth = Width - SelectionStartBox.Coordinates.X;
                int LastSelHeight = Height - SelectionStartBox.Coordinates.Y;
                Rectangle FirstRect = new Rectangle(SelectionStartBox.Coordinates.X, SelectionStartBox.Coordinates.Y, FirstSelWidth, LastSelHeight);
                e.Graphics.FillRectangle(Br, FirstRect);
            }
        }

        private SelectionBox SelectionCoordinates(Point SelectionCoordinates) {
            int LastLine = TotalWindowLines - CommandEntryLine;

            if (SelectionCoordinates.Y < VerScroll) {
                return new SelectionBox(new Point(UnitPadding + HalfUnitWidth + Padding.Left, Padding.Top), SelectionBoxState.OutOfBoundsAbove);
            }
            else if (SelectionCoordinates.Y > (VerScroll + LastLine) - 1) {
                return new SelectionBox(new Point(Width, (LastLine * UnitTextHeight) + Padding.Top), SelectionBoxState.OutOfBoundsBelow);
            }
            else {
                int VScrollDiff = SelectionCoordinates.Y - VerScroll;
                Point SelectionPoint;
                SelectionPoint = new Point(UnitPadding + HalfUnitWidth + (UnitTextWidth * SelectionCoordinates.X) + Padding.Left, (VScrollDiff * UnitTextHeight) + Padding.Top);
                return new SelectionBox(SelectionPoint, SelectionBoxState.InBounds);
            }

        }
        #endregion
        #region Key Linkage

        #endregion
        #region Events
        private void ConsoleInterface_Load(object? sender, EventArgs e) {

        }
        private void FlashTimer_Tick(object? sender, EventArgs e) {
            // throw new NotImplementedException();
            CursorState = !CursorState;
            Invalidate();
        }
        public void Paste() {
            string Temp = Clipboard.GetText();
            Temp = Temp.Replace("\r", "");
            List<string> LinesToPrint = SpiltString(Temp, '\n');
            if (LinesToPrint.Count > 1) {
                for (int i = 0; i < LinesToPrint.Count - 1; i++) {
                    Print(LinesToPrint[i]);
                }
            }
            CommandString += LinesToPrint[LinesToPrint.Count - 1];
            Invalidate();
            // FlashCursor = DownState;
        }
        protected override void OnResize(EventArgs e) {
            int Adjustments = 0;
            if (allowCommandEntry == true) {
                Adjustments = 1;
            }
            if (VerScroll + TotalWindowLines == Lines.Count + Adjustments) {
                TotalWindowLines = (int)Math.Floor((float)Height / (float)UnitTextHeight);

                VerScroll = Lines.Count + Adjustments - TotalWindowLines;
            }
            Invalidate();
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (e.Delta > 0) {
                if ((ModifierKeys.HasFlag(Keys.Control)) && (AllowCommandEntry)) {
                    Zoom += 10;
                }
                else {
                    VerScroll--;
                }
            }
            else {
                if ((ModifierKeys.HasFlag(Keys.Control)) && (AllowCommandEntry)) {
                    Zoom -= 10;
                }
                else {
                    VerScroll++;
                }
            }
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            bool OldState = FlashCursor;
            FlashTimer.Enabled = false;
            CursorState = true;
            //e.SuppressKeyPress = true;
            //if (e.Control && e.KeyCode == Keys.V) {//if ((e.Modifiers & Keys.Control) == Keys.Control) {
            //    Debug.Print(e.KeyCode.ToString());
            //    // if (e.KeyCode == Keys.V) {
            //    Paste();
            //    FlashCursor = OldState;
            //    return;
            //    // }
            //}
            if (e.KeyCode == Keys.Back) {
                try {
                    if (CommandString.Length > 0) {
                        CommandString = CommandString.Remove(CommandString.Length - 1, 1);
                    }
                }
                catch { }
            }
            else if (e.KeyCode == Keys.Enter) {
                AddHistory(CommandString);
                if (printOnEntry == true) {
                    Print(CommandString);
                }
                if (extraLineAfterCommandEntered == true) {
                    Print("");
                }
                CommandEntered?.Invoke(this, new CommandEnteredEventArgs(CommandString));
                CommandString = "";
            }
            else {
                Keys kc = (Keys)e.KeyValue;
                if (IsSpecialKey(kc) == false) {
                    //
                    //        
                    //
                    //        char c = ToChar(kc, e.Shift);
                    //        if (c != 0x00) {
                    //            CommandString += c;
                    //        }
                }
            }
            Invalidate();
            FlashCursor = OldState;
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e) {
            int keyValue = e.KeyChar;
            char c = (char)keyValue;//ToChar(kc, e.Shift);
            if (char.IsLetterOrDigit(c) || char.IsSymbol(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)) {
                if (c != 0x00) {
                    if (e.KeyChar != Convert.ToChar(Keys.Enter)) {
                        CommandString += c;
                    }
                }
            }
            else {
            }
            e.Handled = false;
            //base.OnKeyPress(e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
        }
        private void AddHistory(string Input) {
            for (int i = History.Count - 1; i >= 0; i--) {
                if (History[i].Line == Input) {
                    History.RemoveAt(i);
                }
            }
            History.Add(new TerminalHistory(CommandString));
            HistorySelect = History.Count - 1;
        }
        #endregion
        #region Imput Handling




        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        bool CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
        bool NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
        bool ScrollLock = (((ushort)GetKeyState(0x91)) & 0xffff) != 0;
        private bool IsSpecialKey(Keys key) {
            switch (key) {
                case Keys.Escape:
                    return true;
                case Keys.Tab:
                    return true;
                case Keys.LShiftKey:
                    return true;
                case Keys.RShiftKey:
                    return true;
                case Keys.LControlKey:
                    return true;
                case Keys.RControlKey:
                    return true;
                case Keys.CapsLock:
                    return true;
                case Keys.Alt:
                    return true;
                case Keys.LWin:
                    return true;
                case Keys.RWin:
                    return true;
                case Keys.ShiftKey:
                    return true;
                case Keys.Shift:
                    return true;
                case Keys.Control:
                    return true;
                case Keys.ControlKey:
                    return true;
                case Keys.Home:
                    VerScroll = Lines.Count - 1;
                    return true;
                case Keys.Up:
                    return true;
                case Keys.End:
                    return true;
                case Keys.PageDown:
                    VerScroll += 1;
                    return true;
                case Keys.PageUp:
                    VerScroll -= 1;
                    return true;
                case Keys.Insert:
                    return true;
                case Keys.NumLock:
                    return true;
                case Keys.PrintScreen:
                    return true;
                case Keys.Pause:
                    return true;
                case Keys.Delete:
                    CommandString = "";
                    return true;
                default:
                    return false;
            }
        }
        private char ToChar(Keys key, bool ShiftEnable) {
            InputLanguage CurrentLanguage = InputLanguage.CurrentInputLanguage;
            char c = (char)0;
            CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
            //KeysConverter kc = new KeysConverter();
            //string keyChar = kc.ConvertToString(null, CurrentLanguage.Culture, key) ?? "";
            //Debug.Print(keyChar);
            if (key == Keys.LControlKey)
                c = (char)0;
            else if ((key >= Keys.A) && (key <= Keys.Z)) {
                if (CapsLock || ShiftEnable == true) {
                    if (key == Keys.Shift) { }
                    else { c = (char)System.Convert.ToInt32(key); }
                }
                else if (key == Keys.Shift) { }
                else {
                    c = (char)((int)'a' + System.Convert.ToInt32(key - Keys.A));
                }
            }
            else if ((key >= Keys.D0) && (key <= Keys.D9)) {
                if (ShiftEnable == true)
                    c = ShiftNumbers(key);
                else
                    c = (char)((int)'0' + System.Convert.ToInt32(key - Keys.D0));
            }
            else if ((key >= Keys.NumPad0) && (key <= Keys.NumPad9)) {
                if (NumLock == true) {
                    c = (char)((int)'0' + System.Convert.ToInt32(key - Keys.NumPad0));
                }
            }
            else if (key == Keys.Space)
                c = ' ';
            else if (key == Keys.OemBackslash)
                c = '\\';
            else if (key == Keys.Oem4) {
                if (ShiftEnable == true)
                    c = '{';
                else
                    c = '[';
            }
            else if (key == Keys.Oem6) {
                if (ShiftEnable == true)
                    c = '}';
                else
                    c = ']';
            }
            else if (key == Keys.OemOpenBrackets)
                c = '(';
            else if (key == Keys.OemCloseBrackets)
                c = ')';
            else if (key == Keys.Oemcomma) {
                if (ShiftEnable == true)
                    c = '<';
                else
                    c = ',';
            }
            else if (key == Keys.OemPeriod) {
                if (ShiftEnable == true)
                    c = '>';
                else
                    c = '.';
            }
            else if (key == Keys.OemPipe) {
                if (ShiftEnable == true)
                    c = '|';
                else
                    c = '\\';
            }
            else if (key == Keys.OemQuotes) {
                if (ShiftEnable == false)
                    c = '"';
                else
                    c = '\'';
            }
            else if (key == Keys.OemQuestion) {
                if (ShiftEnable == true)
                    c = '?';
                else
                    c = '/';
            }
            else if (key == Keys.OemSemicolon) {
                if (ShiftEnable == false)
                    c = ';';
                else
                    c = ':';
            }
            else if (key == Keys.Oemtilde) {
                if (ShiftEnable == false)
                    c = '`';
                else
                    c = '~';
            }
            else if (key == Keys.Oemplus) {
                if (ShiftEnable == false)
                    c = '=';
                else
                    c = '+';
            }
            else if (key == Keys.OemMinus) {
                if (ShiftEnable == false)
                    c = '-';
                else
                    c = '_';
            }
            else if (key == Keys.Divide) { c = '/'; }
            else if (key == Keys.Multiply) { c = '*'; }
            else if (key == Keys.Add) { c = '+'; }
            else if (key == Keys.Subtract) { c = '-'; }
            else if (key == Keys.Decimal) { c = '.'; }
            return c;
        }
        private char ShiftNumbers(Keys key) {
            switch (key) {
                case Keys.D1:
                    return '!';
                case Keys.D2:
                    return '@';
                case Keys.D3:
                    return '#';
                case Keys.D4:
                    return '$';
                case Keys.D5:
                    return '%';
                case Keys.D6:
                    return '^';
                case Keys.D7:
                    return '&';
                case Keys.D8:
                    return '*';
                case Keys.D9:
                    return '(';
                case Keys.D0:
                    return ')';
                default:
                    return (char)0x00;
            }
        }
        int HistorySelect = -1;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Down) {
                try {
                    if (History.Count >= 0) {
                        if (HistorySelect > History.Count - 1) {
                            HistorySelect = History.Count - 1;
                        }
                        CommandString = History[HistorySelect].Line;
                        if (HistorySelect == History.Count - 1) {
                        }
                        else
                            HistorySelect += 1;
                    }
                    Invalidate();
                }
                catch {
                }
                return true;
            }
            if (keyData == Keys.Up) {
                if (History.Count >= 1) {
                    if (HistorySelect <= 0)
                        CommandString = History[0].Line;
                    else {
                        CommandString = History[HistorySelect].Line;
                        HistorySelect -= 1;
                    }
                }
                Invalidate();
                return true;
            }
            if (keyData == (Keys.Control | Keys.V)) {
                Paste();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void ClearEntered() {
            CommandString = "";
            Invalidate();
        }
        #endregion
        #region Methods
        private List<string> SpiltString(string value, char chr) {
            string rechr = chr.ToString();
            string fullval = value;
            var splitval = fullval.Split(rechr.ToCharArray());
            var cnt = fullval.Split(chr);
            List<string> cp = new List<string>();
            for (int i = 0; i <= cnt.Count() - 1; i++) {
                cp.Add(splitval[i]);
            }
            return cp;
        }
        #endregion
        #region Enumerations
        public enum TimeStampFormat {
            NoTimeStamps = 0x00,
            Time = 0x01,
            TimeWithMilliseconds = 0x02,
            DateTime = 0x03,
            Date = 0x04
        }
        #endregion
        #region Position Handling
        private int GetVerticalScrollFromCursor(int MousePositionY, float ThumbPosition) {
            int ScrollTo = (int)((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition) * Lines.Count) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
            //Debug.Print(ScrollTo.ToString());
            return ScrollTo;
        }
        //HorScroll = (MouseX - hBarX - ThumbPos) * 
        private int GetHorizontalScrollFromCursor(int MousePositionX, float ThumbPosition) {
            return (int)((float)((MousePositionX - HorizontalScrollBounds.X - ThumbPosition) * 100.0f) / (HorizontalScrollBounds.Width - HorizontalScrollThumb.Width));
        }
        #endregion
        private enum SelectionBoxState {
            OutOfBoundsAbove = 0x01,
            InBounds = 0x02,
            OutOfBoundsBelow = 0x04
        }
        private class SelectionBox {
            public Point Coordinates;
            public SelectionBoxState State;
            public SelectionBox(Point Coordinates, SelectionBoxState State) {
                this.Coordinates = Coordinates;
                this.State = State;
            }
        }
        bool InScrollBounds = false;
        bool ScrollStart = false;
        ScrollArea ScrollHit = ScrollArea.None;
        float ThumbDelta = 0;
        enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        protected void OnMouseClick(object sender, MouseEventArgs e) {
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
            //base.OnMouseClick(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Vertical)) {
                if (Lines.Count > 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, ThumbDelta);
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Horizontal)) {
                //if (columns.Count > 0) {
                //    HorScroll = GetHorizontalScrollFromCursor(e.X, ThumbDelta);
                //    Invalidate();
                //}
            }
            // base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            //HitHeader = false;
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            //ScrollOutofBounds.Enabled = false;
            //IgnoreLines = false;
            // base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
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
            //base.OnMouseDown(e);
        }
    }
    public class CommandEnteredEventArgs : EventArgs {
        string command = "";
        public CommandEnteredEventArgs(string command) {
            this.command = command;
        }
        public string Command {
            get { return command; }
        }
    }
    public struct TerminalHistory {
        private string line;
        public string Line { get => line; set => line = value; }
        public TerminalHistory(string Command) {
            line = Command;
        }
    }
    public class TerminalLine {
        public TerminalLine(string Source, string Line) {
            this.line = Line;
            useConsoleColor = true;
            this.source = Source;
            foreColor = Color.Black;
            timeStamp = DateTime.Now;
        }
        public TerminalLine(string Source, string Line, Color ForeColor) {
            this.line = Line;
            useConsoleColor = false;
            foreColor = ForeColor;
            timeStamp = DateTime.Now;
            this.source = Source;
            //backColor = BackColor;
        }
        private DateTime timeStamp;
        public DateTime TimeStamp { get => timeStamp; }
        private string source = "";
        public string Source { get => source; set => source = value; }
        private string line;
        public string Line { get => line; set => line = value; }
        private bool useConsoleColor;
        public bool UseConsoleColor { get => useConsoleColor; set => useConsoleColor = value; }
        private Color foreColor;
        public Color ForeColor { get => foreColor; set => foreColor = value; }
        //private Color backColor;
        //public Color BackColor { get => backColor; set => backColor = value; }
    }
}
