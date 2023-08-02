using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Imaging;
using Handlers;
namespace ODModules {
    public class HeaderControl : UserControl {
        private System.Windows.Forms.Timer animation = new System.Windows.Forms.Timer();
        //private int VISCNT = 0;
        //public List<XTabs> Tabs = new List<XTabs>();
        private int selected = 0;
        public int Selected {
            get {
                return selected;
            }
            set {
                selected = value;
                Invalidate();
                if (bindedTabControl != null) {
                    try {
                        bindedTabControl.SelectedIndex = selected;
                    }
                    catch { }
                }

            }
        }
        private int Selected_Old = -1;
        public event ValueChangedEventHandler ?ValueChanged;

        public delegate void ValueChangedEventHandler();

        public HeaderControl() {
            InitializeComponent();
            StepMag = 0.0f;
            animation.Tick += Ticker;
            animation.Interval = 1;
            TabFont = Font;
            // MyBase.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            DoubleBuffered = true;
            DateFont = Font;
            ClockFont = Font;
        }
        private bool _AllowMouseWheel = false;
        [System.ComponentModel.Category("Control")]
        public bool AllowMouseWheel {
            get {
                return _AllowMouseWheel;
            }
            set {
                _AllowMouseWheel = value;
            }
        }
        private TabControl? bindedTabControl = null;
        [System.ComponentModel.Category("Control")]
        public TabControl? BindedTabControl {
            get {
                return bindedTabControl;
            }
            set {
                if (bindedTabControl != null) {
                    bindedTabControl.ControlAdded -= BindedTabControl_ControlAdded;
                    bindedTabControl.ControlRemoved -= BindedTabControl_ControlRemoved;
                }
                bindedTabControl = value;
                if (bindedTabControl != null) {
                    bindedTabControl.ControlAdded += BindedTabControl_ControlAdded;
                    bindedTabControl.ControlRemoved += BindedTabControl_ControlRemoved;
                }
                Invalidate();
            }
        }
        private void BindedTabControl_ControlRemoved(object? sender, ControlEventArgs e) {
            if (bindedTabControl != null) {
                if (selected >= bindedTabControl.TabPages.Count - 1) {
                    if (Old_Selected < 0) {
                        Old_Selected = selected;
                    }
                    selected = bindedTabControl.TabPages.Count - 1;
                    ReDraw();
                }
            }
        }
        private void BindedTabControl_ControlAdded(object? sender, ControlEventArgs e) {
            Invalidate();
        }

        private string _HeaderText = "";
        [System.ComponentModel.Category("Appearance")]
        public string HeaderText {
            get {
                return _HeaderText;
            }
            set {
                _HeaderText = value;
                Invalidate();
            }
        }
        private Font? _ClockFont;
        [System.ComponentModel.Category("Appearance")]
        public Font? ClockFont {
            get {
                return _ClockFont;
            }
            set {
                _ClockFont = value;
                Invalidate();
            }
        }
        private Font? _DateFont;
        [System.ComponentModel.Category("Appearance")]
        public Font? DateFont {
            get {
                return _DateFont;
            }
            set {
                _DateFont = value;
                Invalidate();
            }
        }
        private Font? _TabFont;
        [System.ComponentModel.Category("Appearance")]
        public Font? TabFont {
            get {
                return _TabFont;
            }
            set {
                _TabFont = value;
                Invalidate();
            }
        }
        private bool _TabsVisible = true;
        [System.ComponentModel.Category("Appearance")]
        public bool TabsVisible {
            get {
                return _TabsVisible;
            }
            set {
                _TabsVisible = value;
                PreventRedraw = true;
                //if (value == true)
                //    this.Size = new Size(this.Width, HEAD_OFF + TAB_OFF + TAB_SPACE + TextPadding.Y + 5);
                //else
                //    this.Size = new Size(this.Width, HEAD_OFF + TextPadding.Y);
                PreventRedraw = false;
                Invalidate();
            }
        }
        private Point _TextPadding = Point.Empty;
        [System.ComponentModel.Category("Layout")]
        public Point TextPadding {
            get {
                return _TextPadding;
            }
            set {
                _TextPadding = value;
                Invalidate();
            }
        }
        private bool _Animate = false;
        [System.ComponentModel.Category("Control")]
        public bool Animate {
            get {
                return _Animate;
            }
            set {
                _Animate = value;
                Invalidate();
            }
        }
        private bool _ShowClock = false;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowClock {
            get {
                return _ShowClock;
            }
            set {
                _ShowClock = value;
                Invalidate();
            }
        }
        private void FirstRun() {
            using (var bmp = new Bitmap(10, 10)) {
                Graphics g = Graphics.FromImage(bmp);
                HEAD_OFF = Convert.ToInt32(g.MeasureString("W", Font).Height);
                if (TabFont == null) { return; }
                TAB_OFF = Convert.ToInt32(g.MeasureString("W", TabFont).Height);
                TAB_SPACE = Convert.ToInt32(g.MeasureString("W", TabFont).Width / (double)8);
                g.Dispose();
            }
            GC.Collect();
        }
        private Point OFFSET = new Point(0, 0);
        private int HEAD_OFF = 0;
        private int TAB_SPACE = 0;
        private int TAB_OFF = 0;
        private int TAB_OLD_COUNT = -1;
        protected override void OnPaint(PaintEventArgs e) {
            // Using bmp = New Bitmap(ClientSize.Width, ClientSize.Height)
            // Dim g As Graphics = Graphics.FromImage(bmp)
            Rectangle Window = new Rectangle(0, 0, this.Width, this.Height);
            using (SolidBrush FillBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillRectangle(FillBrush, Window);
            }
            DetermineTabCounts();
            DrawingSetup(e);
            using (SolidBrush txtbr = new SolidBrush(ForeColor)) {
                e.Graphics.DrawString(HeaderText, Font, txtbr, new Point(TextPadding.X, TextPadding.Y));
            }
            DrawTabs(e);
            DrawClock(e);
            // g = e.Graphics
            e.Graphics.ResetClip();
        }
        private void DrawingSetup(PaintEventArgs e) {
            HEAD_OFF = Convert.ToInt32(e.Graphics.MeasureString("W", Font).Height);
            if (TabFont == null) { return; }
            TAB_OFF = Convert.ToInt32(e.Graphics.MeasureString("W", TabFont).Height);
            TAB_SPACE = Convert.ToInt32(e.Graphics.MeasureString("W", TabFont).Width / (double)8);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        }
        private void DetermineTabCounts() {
            if (bindedTabControl != null) {
                if (TAB_OLD_COUNT != bindedTabControl.TabPages.Count) {
                    Selected_Old = selected;
                    TAB_OLD_COUNT = bindedTabControl.TabPages.Count;
                    SetSelect();
                    CurrentLineStart = DestinationLineStart;
                    CurrentLineEnd = DestinationLineEnd;
                }
                else if ((Animate == false | AnimateDirection == AnimationDirection.Off) & TabsVisible == true) {
                    Selected_Old = selected;
                    SetSelect();
                    CurrentLineStart = DestinationLineStart;
                    CurrentLineEnd = DestinationLineEnd;
                }
            }
        }
        private void DrawTabs(PaintEventArgs e) {
            OFFSET.Y = HEAD_OFF + TextPadding.Y;
            if (TabsVisible == true) {
                int OFF_X = TextPadding.X;
                if (bindedTabControl != null) {
                    if (TabFont != null) {
                        for (var i = 0; i < bindedTabControl.TabPages.Count; i++) {
                            int TAB_WIDTH = (int)e.Graphics.MeasureString(bindedTabControl.TabPages[i].Text, TabFont).Width;
                            Color COL = ForeColor;
                            if (i == bindedTabControl.SelectedIndex) {
                                COL = RenderHandler.DeterministicDarkenColor(ForeColor, BackColor, 128);
                            }
                            using (SolidBrush txtbr = new SolidBrush(COL)) {
                                e.Graphics.DrawString(bindedTabControl.TabPages[i].Text, TabFont, txtbr, new Point(OFF_X, OFFSET.Y));
                            }
                            OFF_X += TAB_WIDTH + TAB_SPACE;
                        }
                        using (SolidBrush txtbr = new SolidBrush(RenderHandler.DeterministicDarkenColor(ForeColor, BackColor, 128))) {
                            int LY = OFFSET.Y + TAB_OFF - 2;
                            using (Pen LinePen = new Pen(txtbr, 1.5f)) {
                                e.Graphics.DrawLine(LinePen, new Point(CurrentLineStart, LY), new Point(CurrentLineEnd, LY));
                            }
                        }
                    }
                }
            }
        }
        private void DrawClock(PaintEventArgs e) {
            if (ClockFont == null) { return; }
            if (DateFont == null) { return; }
            if (ShowClock == true) {
                string TME = Strings.Format(DateTime.Now.Hour, "00") + ":" + Strings.Format(DateTime.Now.Minute, "00") + ":" + Strings.Format(DateTime.Now.Second, "00");
                string DTE = DateTime.Now.ToLongDateString();
                int TME_OFF = System.Convert.ToInt32(e.Graphics.MeasureString("W", ClockFont).Height);
                int TXT_OFF = System.Convert.ToInt32(e.Graphics.MeasureString("W", DateFont).Height);
                int TME_WDH = System.Convert.ToInt32(e.Graphics.MeasureString(TME, ClockFont).Width);
                int DTE_WDH = System.Convert.ToInt32(e.Graphics.MeasureString(DTE, DateFont).Width);
                int TXT_LOC = 5;
                using (SolidBrush cbr = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString(TME, ClockFont, cbr, new Point(Width - TME_WDH - TXT_OFF, TXT_LOC));
                }
                TXT_LOC += TME_OFF - 5;
                using (SolidBrush cbr = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString(DTE, DateFont, cbr, new Point(Width - DTE_WDH - TXT_OFF, TXT_LOC));
                }
                TXT_LOC += TXT_OFF;
                using (SolidBrush cbr = new SolidBrush(ForeColor)) {
                    using (Pen PN_LNE = new Pen(cbr)) {
                        // Dim LNE_y As Integer = PADY + TME_OFF - 5 + TXT_OFF
                        int LNE_OFF = Width - DTE_WDH - TXT_OFF + 2;
                        e.Graphics.DrawLine(PN_LNE, LNE_OFF, TXT_LOC, LNE_OFF + DTE_WDH - 8, TXT_LOC);
                    }
                }
            }
        }
        private int SelectLineStart = 0;
        private int SelectLineEnd = 0;
        private int CurrentLineStart = 0;
        private int CurrentLineEnd = 0;
        private int DestinationLineStart = 0;
        private int DestinationLineEnd = 0;
        private int Old_Selected = -1;
        private void SetSelect() {
            int OFF_X = TextPadding.X;
            if (bindedTabControl != null) {
                if (TabFont != null) {
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1))) {
                        for (int i = 0; i < bindedTabControl.TabPages.Count; i++) {
                            //if (bindedTabControl.TabPages[i].Visible == true) {
                            int TAB_WIDTH = (int)graphics.MeasureString(bindedTabControl.TabPages[i].Text, TabFont).Width;
                            if (Old_Selected == i) {
                                SelectLineStart = OFF_X;
                                SelectLineEnd = OFF_X + TAB_WIDTH - 1;
                            }
                            if (selected == i) {
                                DestinationLineStart = OFF_X;
                                DestinationLineEnd = OFF_X + TAB_WIDTH - 1;
                            }
                            OFF_X += TAB_WIDTH + TAB_SPACE;
                        }
                    }
                }
            }
        }
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // HeaderControl
            // 
            this.Name = "HeaderControl";
            this.Size = new System.Drawing.Size(69, 35);
            this.Load += new System.EventHandler(this.HeaderControl_Load);
            this.ResumeLayout(false);

        }
        private bool PreventRedraw = false;
        private void Header_Resize(object sender, EventArgs e) {
            if (PreventRedraw == false)
                Invalidate();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Right) {
                SelectValue(true);
                // RaiseEvent ValueChanged()
                return true;
            }
            if (keyData == Keys.Left) {
                SelectValue(false);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void SelectValueByName(string Input) {
            if (Input != "" | Input.Replace(" ", "") != "") {
                if (bindedTabControl == null) { return; }
                if (Animate == true) {
                    Selected_Old = selected;
                }
                for (var i = 0; i <= bindedTabControl.TabPages.Count; i++) {
                    if (bindedTabControl.TabPages[i].Text.ToLower().Contains(Input.ToLower())) {
                        selected = i;
                        break;
                    }
                }
                ReDraw();
                ValueChanged?.Invoke();
            }
        }
        private void SelectValue(bool Up) {
            if (bindedTabControl == null) { return; }
            if (Animate == true) {
                Selected_Old = selected;
            }

            if (Up == true) {
                int Test_Selected = selected + 1;
                // Selected += 1
                if (Test_Selected <= bindedTabControl.TabPages.Count - 1) {
                    int HoldSelected = Test_Selected; // Selected
                    for (var i = HoldSelected; i <= bindedTabControl.TabPages.Count - 1; i++) {
                        // if (bindedTabControl.TabPages[i].Visible == true & bindedTabControl.TabPages[i].Type == Type) {
                        selected = i;
                        break;
                        // }
                    }
                }
                else
                    for (int i = bindedTabControl.TabPages.Count - 1; i >= 0; i += -1) {
                        selected = i;
                        break;
                    }
                ReDraw();
            }
            else {
                int Test_Selected = selected - 1; // Selected -= 1
                if (Test_Selected >= 0) {
                    int HoldSelected = Test_Selected; // Selected
                    for (int i = HoldSelected; i >= 0; i += -1) {
                        selected = i;
                        break;
                    }
                }
                else {
                    selected = 0;
                }
                ReDraw();
            }
        }
        private void ReDraw() {
            if (Animate == false)
                Invalidate();
            else {
                SetSelect();
                SetupAnimation();
                if (Selected_Old == selected) {
                    AnimateDirection = AnimationDirection.Off;
                    CurrentLineStart = DestinationLineStart;
                    CurrentLineEnd = DestinationLineEnd;
                    Invalidate();
                }
                else if (AnimateDirection != AnimationDirection.Off)
                    animation.Enabled = true;
                else {
                    Selected_Old = selected;
                    CurrentLineStart = DestinationLineStart;
                    CurrentLineEnd = DestinationLineEnd;
                    Invalidate();
                }
            }
        }
        private void Header_Load(object? sender, EventArgs e) {
            FirstRun();
        }
        private AnimationDirection AnimateDirection = AnimationDirection.Off;
        private float StepMag = 0.0f;
        private void SetupAnimation() {
            if (Selected_Old < selected) {
                AnimateDirection = AnimationDirection.Right;
                StepMag = 0.5f;
            }
            else if (Selected_Old > selected) {
                AnimateDirection = AnimationDirection.Left;
                StepMag = 0.5f;
            }
            else if (Selected_Old == selected)
                AnimateDirection = AnimationDirection.Off;
            steps = 0;
        }
        private int steps = 0;
        private void Ticker(object? sender, EventArgs e) {
            if (AnimateDirection == AnimationDirection.Left) {
                if (CurrentLineStart > DestinationLineStart)
                    CurrentLineStart -= 8;
                if (CurrentLineStart <= DestinationLineStart)
                    CurrentLineStart = DestinationLineStart;
                if (steps >= 1) {
                    if (CurrentLineEnd > DestinationLineEnd)
                        CurrentLineEnd -= 8;
                    if (CurrentLineEnd <= DestinationLineEnd) {
                        CurrentLineEnd = DestinationLineEnd;
                        animation.Enabled = false;
                        Selected_Old = selected;
                        AnimateDirection = AnimationDirection.Off;
                    }
                }
                steps += 1;
                Invalidate();
            }
            if (AnimateDirection == AnimationDirection.Right) {
                if (CurrentLineEnd < DestinationLineEnd)
                    CurrentLineEnd += 8;
                if (CurrentLineEnd >= DestinationLineEnd)
                    CurrentLineEnd = DestinationLineEnd;
                if (steps >= 1) {
                    if (CurrentLineStart < DestinationLineStart)
                        CurrentLineStart += 8;
                    if (CurrentLineStart >= DestinationLineStart) {
                        CurrentLineStart = DestinationLineStart;
                        animation.Enabled = false;
                        Selected_Old = selected;
                        AnimateDirection = AnimationDirection.Off;
                    }
                }
                steps += 1;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e) {
            Invalidate();
        }
        protected override void OnMouseClick(MouseEventArgs e) {
            if (TabsVisible == true) {
                if (e.Y >= OFFSET.Y) {
                    int OFF_X = TextPadding.X;
                    if (bindedTabControl != null) {
                        if (TabFont != null) {
                            for (var i = 0; i < bindedTabControl.TabPages.Count; i++) {
                                using (var bmp = new Bitmap(1, 1)) {
                                    Graphics g = Graphics.FromImage(bmp);
                                    int TAB_WIDTH = (int)g.MeasureString(bindedTabControl.TabPages[i].Text, TabFont).Width;
                                    if (e.X >= OFF_X & e.X < OFF_X + TAB_WIDTH) {
                                        selected = i;
                                        break;
                                    }
                                    OFF_X += TAB_WIDTH + TAB_SPACE;
                                    g.Dispose();
                                }
                            }
                        }
                    }
                }
                //Selected_Old = selected;
                ReDraw();
                ValueChanged?.Invoke();
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (AllowMouseWheel == true) {
                if (TabsVisible == true) {
                    if (e.Delta < 0) {
                        SelectValue(true);
                        ValueChanged?.Invoke();
                    }
                    else {
                        SelectValue(false);
                        ValueChanged?.Invoke();
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            if (TabsVisible == true) { ValueChanged?.Invoke(); }
        }


        private enum AnimationDirection {
            Off = 0,
            Left = 1,
            Right = 2
        }

        private void HeaderControl_Load(object ?sender, EventArgs e) {

        }
    }
}
