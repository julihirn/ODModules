using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public partial class TimePainter : UserControl {
        public delegate void StatusUpdateHandler(object sender, PaintedEventArgs e);
        public event StatusUpdateHandler? ValueInRange;

        private static System.Timers.Timer TimeTicker = new System.Timers.Timer(500);
        public TimePainter() {
            InitializeComponent();
            spanStart = new TimeSpan(0, 0, 0, 0, 0);
            spanEnd = new TimeSpan(1, 0, 0, 0, 0);
            TimeDifference = new TimeSpan(1, 0, 0, 0, 0);
            Precision = TimePrecision.Hours;
            TimeTicker = new System.Timers.Timer();
            TimeTicker.AutoReset = true;
            TimeTicker.Interval = 500;
            TimeTicker.Elapsed += Ticker_Elapsed;
            MouseClick += OnMouseClick;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseWheel += OnMouseWheel;
            MouseMove += OnMouseMove;
            //MouseLeave += OnMouseLeave;
            //MouseEnter += OnMouseEnter;
            //MouseHover += OnMouseHover;
        }
        #region Properties
        private List<PaintedTimeEvent> paintedEvents = new List<PaintedTimeEvent>();
        [System.ComponentModel.Category("Events")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<PaintedTimeEvent> PaintedEvents {
            get { return paintedEvents; }
        }
        private bool isActive = true;
        [System.ComponentModel.Category("Control")]
        public bool IsActive {
            get {
                return isActive;
            }
            set {
                isActive = value;
            }
        }
        private bool painterSelected = false;
        [System.ComponentModel.Category("Control")]
        public bool PainterSelected {
            get {
                return painterSelected;
            }
            set {
                painterSelected = value;
                Invalidate();
            }
        }
        private bool useFocusSelect = true;
        [System.ComponentModel.Category("Control")]
        public bool UseFocusSelect {
            get {
                return useFocusSelect;
            }
            set {
                useFocusSelect = value;
                Invalidate();
            }
        }
        private bool showAllPaintedEvents = true;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowAllPaintedEvents {
            get {
                return showAllPaintedEvents;
            }
            set {
                showAllPaintedEvents = value;
                if (value == true) {
                    foreach (PaintedTimeEvent e in PaintedEvents) {
                        e.IsVisible = true;
                    }
                }
                Invalidate();
            }
        }
        private bool currentTimeMarker = true;
        [System.ComponentModel.Category("Show/Hide")]
        public bool CurrentTimeMarker {
            get {
                return currentTimeMarker;
            }
            set {
                currentTimeMarker = value;
                if (currentTimeMarker == true) {
                    TimeTicker.Enabled = true;
                    TimeTicker.Start();
                }
                else {
                    TimeTicker.Enabled = false;
                }
                Invalidate();
            }
        }
        private TimePrecision precision;
        [System.ComponentModel.Category("Time")]
        public TimePrecision Precision {
            get {
                return precision;
            }
            set {
                precision = value;

            }
        }
        private Color currentTimeMarkerColor = Color.Red;
        [System.ComponentModel.Category("Appearance")]
        public Color CurrentTimeMarkerColor {
            get {
                return currentTimeMarkerColor;
            }
            set {
                currentTimeMarkerColor = value;
                Invalidate();
            }
        }
        private Color timeMarkerColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color TimeMarkerColor {
            get {
                return timeMarkerColor;
            }
            set {
                timeMarkerColor = value;
                Invalidate();
            }
        }
        private Color timeMarkerTextColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color TimeMarkerTextColor {
            get {
                return timeMarkerTextColor;
            }
            set {
                timeMarkerTextColor = value;
                Invalidate();
            }
        }
        private Color borderColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColor {
            get {
                return borderColor;
            }
            set {
                borderColor = value;
                Invalidate();
            }
        }
        private Color selectedborderColor;
        [Category("Appearance")]
        public Color SelectedBorderColor {
            get { return selectedborderColor; }
            set { selectedborderColor = value; Invalidate(); }
        }
        private Color selectColor = SystemColors.Highlight;
        [System.ComponentModel.Category("Appearance")]
        public Color SelectColor {
            get {
                return selectColor;
            }
            set {
                selectColor = value;
                Invalidate();
            }
        }
        private Color currentPaintColor = Color.LimeGreen;
        [System.ComponentModel.Category("Painting")]
        public Color CurrentPaintColor {
            get {
                return currentPaintColor;
            }
            set {
                currentPaintColor = value;
            }
        }
        private string currentPaintID = "";
        [System.ComponentModel.Category("Painting")]
        public string CurrentPaintID {
            get {
                return currentPaintID;
            }
            set {
                currentPaintID = value;
                Invalidate();
            }
        }
        private object? currentPaintTag = null;
        [System.ComponentModel.Category("Painting")]
        public object? CurrentPaintTag {
            get {
                return currentPaintTag;
            }
            set {
                currentPaintTag = value;
            }
        }
        private int selectionOpacity = 128;
        [System.ComponentModel.Category("Appearance")]
        public int SelectionOpacity {
            get {
                return selectionOpacity;
            }
            set {
                if ((value >= 0) && (value < 256)) {
                    selectionOpacity = value;
                }
                else if (value > 255) {
                    selectionOpacity = 255;
                }
                else {
                    selectionOpacity = 0;
                }
                Invalidate();
            }
        }
        private int eventsOpacity = 128;
        [System.ComponentModel.Category("Appearance")]
        public int EventsOpacity {
            get {
                return selectionOpacity;
            }
            set {
                if ((value >= 0) && (value < 256)) {
                    eventsOpacity = value;
                }
                else if (value > 255) {
                    eventsOpacity = 255;
                }
                else {
                    eventsOpacity = 0;
                }
                Invalidate();
            }
        }
        private EditMode mode = EditMode.Select;

        private TimeSpan selectStart;
        private TimeSpan selectEnd;
        private TimeSpan spanStart;
        private TimeSpan spanEnd;
        private TimeSpan TimeDifference;
        [System.ComponentModel.Category("Time")]
        public TimeSpan SpanStart {
            get { return spanStart; }
            set {
                TimeSpan Current = PrecisionTrim(value);
                TimeSpan CurrentTimeDifference = new TimeSpan(spanEnd.Ticks - Current.Ticks);
                //switch (precision) {
                //    case TimePrecision.Hours:
                //        if (CurrentTimeDifference.TotalHours < 0) {
                //            if (Current.Days >= 1) {
                //                spanStart = new TimeSpan(0, 23, 0, 0, 0);
                //                SpanEnd = new TimeSpan(1, 0, 0, 0, 0);
                //            }
                //            else {
                //                if (spanEnd.Days >= 1) {
                //                    spanStart = new TimeSpan(0, 23, 0, 0, 0);
                //                    SpanEnd = new TimeSpan(1, 0, 0, 0, 0);
                //                }
                //                else {
                //                    SpanEnd = spanStart.Add(new TimeSpan(0, 1, 0, 0, 0));
                //                }
                //            }
                //        }
                //        else {

                //        }
                //}
                spanStart = Current;
                TimeDifference = new TimeSpan(spanEnd.Ticks - spanStart.Ticks);
                Invalidate();
            }
        }
        private TimeSpan PrecisionTrim(TimeSpan Input, bool LimitDays = true) {
            TimeSpan Output = new TimeSpan(0, 0, 0, 0, 0);
            int Days = Input.Days;
            if (LimitDays == true) {
                if (Days > 1) {
                    Days = 1;
                }
            }
            switch (precision) {
                case TimePrecision.Hours:
                    Output = new TimeSpan(Days, Input.Hours, 0, 0, 0);
                    if (Input.Minutes > 29) { Output = Output.Add(new TimeSpan(1, 0, 0)); }
                    return Output;
                case TimePrecision.Minutes:
                    Output = new TimeSpan(Days, Input.Hours, Input.Minutes, 0, 0);
                    if (Input.Seconds > 29) { Output = Output.Add(new TimeSpan(0, 1, 0)); }
                    return Output;
                case TimePrecision.Seconds:
                    Output = new TimeSpan(Days, Input.Hours, Input.Minutes, Input.Seconds, 0);
                    if (Input.Milliseconds > 499) { Output = Output.Add(new TimeSpan(0, 0, 1)); }
                    return Output;
                case TimePrecision.Milliseconds:
                    Output = new TimeSpan(Days, Input.Hours, Input.Minutes, Input.Seconds, Input.Milliseconds);
                    return Output;
                default:
                    return Output;
            }
        }
        private void PrecisionConverter(TimeSpan Input, SpanValue SelectedValue) {

        }
        [System.ComponentModel.Category("Time")]
        public TimeSpan SpanEnd {
            get { return spanEnd; }
            set {
                spanEnd = value;
                TimeDifference = new TimeSpan(spanEnd.Ticks - spanStart.Ticks);
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Time")]
        public TimeSpan SelectStart { get => selectStart; }
        [System.ComponentModel.Category("Time")]
        public TimeSpan SelectEnd { get => selectEnd; }
        [System.ComponentModel.Category("Control")]
        public EditMode Mode { get => mode; set => mode = value; }
        #endregion
        #region Drawing
        int TimeWidth = 0;
        SizeF UnitTextSize;
        protected override void OnPaint(PaintEventArgs e) {
            TimeWidth = (int)e.Graphics.MeasureString("00:00:00", Font).Width;
            UnitTextSize = e.Graphics.MeasureString("W", Font);
            DrawEvents(e);
            DrawTimeMarkers(e);
            DrawSelectionBox(e);
            if (currentTimeMarker == true) {
                DrawCurrentTimeMarker(e);
            }
            //using (SolidBrush BorderBrush = new SolidBrush(borderColor)) {
            //    using (Pen BorderPen = new Pen(BorderBrush)) {
            //        BorderPen.Alignment = PenAlignment.Inset;
            //        e.Graphics.DrawRectangle(BorderPen, new Rectangle(0, 0, Width - 1, Height - 1));
            //    }
            //}
            if (useFocusSelect == true) {
                if (this.Focused == true) {
                    DrawBorder(e, new Rectangle(0, 0, Bounds.Width, Bounds.Height), SelectedBorderColor);
                }
                else {
                    DrawBorder(e, new Rectangle(0, 0, Bounds.Width, Bounds.Height), BorderColor);
                }
            }
            else {
                if (painterSelected == true) {
                    DrawBorder(e, new Rectangle(0, 0, Bounds.Width, Bounds.Height), SelectedBorderColor);
                }
                else {
                    DrawBorder(e, new Rectangle(0, 0, Bounds.Width, Bounds.Height), BorderColor);
                }
            }


        }
        private void DrawBorder(PaintEventArgs e, Rectangle Bound, Color BorderingColour, int PenSizing = 1) {
            if (PenSizing > 0) {
                using (Brush Brs = new SolidBrush(BorderingColour)) {
                    using (Pen Pn = new Pen(Brs, PenSizing)) {
                        if (PenSizing == 1) {
                            Rectangle Rect = new Rectangle(Bound.X, Bound.Y, Bound.Width - 1, Bound.Height - 1);
                            e.Graphics.DrawRectangle(Pn, Rect);
                        }
                        else {
                            Pn.Alignment = PenAlignment.Inset;
                            e.Graphics.DrawRectangle(Pn, Bound);
                        }
                    }
                }
            }
        }
        private void DrawEvents(PaintEventArgs e) {
            if (PaintedEvents.Count > 0) {
                foreach (PaintedTimeEvent Pte in PaintedEvents) {
                    if ((showAllPaintedEvents == true) || (Pte.ID == currentPaintID)) {
                        if (Pte.IsVisible == true) {
                            Rectangle EventBounds = GetEventBox(Pte);
                            using (SolidBrush EventBrush = new SolidBrush(Color.FromArgb(eventsOpacity, Pte.DisplayColor.R, Pte.DisplayColor.G, Pte.DisplayColor.B))) {
                                e.Graphics.FillRectangle(EventBrush, EventBounds);
                            }
                            if (Pte.Selected == true) {
                                Color SelectionColor = Color.FromArgb(255, Pte.DisplayColor.R, Pte.DisplayColor.G, Pte.DisplayColor.B);
                                using (HatchBrush SelectedBrush = new HatchBrush(HatchStyle.WideUpwardDiagonal, SelectionColor, Color.Transparent)) {
                                    e.Graphics.FillRectangle(SelectedBrush, EventBounds);
                                }
                                using (SolidBrush BorderBrush = new SolidBrush(SelectionColor)) {
                                    using (Pen BorderPen = new Pen(BorderBrush)) {
                                        e.Graphics.DrawRectangle(BorderPen, EventBounds);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void DrawCurrentTimeMarker(PaintEventArgs e) {
            DateTime Midnight = DateTime.Now.Date;
            DateTime CalcStart = Midnight.Add(spanStart);
            DateTime CalcEnd = Midnight.Add(spanEnd);
            DateTime Now = DateTime.Now;
            if ((Now.Ticks >= CalcStart.Ticks) && (Now.Ticks < CalcEnd.Ticks)) {
                long PosX = Now.Ticks - CalcStart.Ticks;
                long Diff = TimeDifference.Ticks;
                int PositionX = (int)(((float)PosX / (float)Diff) * (float)Width);
                using (SolidBrush PenBrush = new SolidBrush(currentTimeMarkerColor)) {
                    using (Pen MarkPen = new Pen(currentTimeMarkerColor)) {
                        Point NorthPoint = new Point(PositionX, 0);
                        Point SouthPoint = new Point(PositionX, Height);
                        e.Graphics.DrawLine(MarkPen, NorthPoint, SouthPoint);

                    }
                }
                DrawMarkerTriangles(e, PositionX, currentTimeMarkerColor);
            }
        }
        private void DrawMarkerTriangles(PaintEventArgs e, int X, Color Marker) {
            Point U_P1 = new Point(0, 0); Point D_P1 = new Point(0, 0);
            Point U_P2 = new Point(0, 0); Point D_P2 = new Point(0, 0);
            Point U_P3 = new Point(0, 0); Point D_P3 = new Point(0, 0);
            int HalfStep = (int)(UnitTextSize.Width / 1.5f);
            int QuarterStep = (int)(UnitTextSize.Width / 4.0f);
            U_P1 = new Point(X - QuarterStep, 0);
            U_P2 = new Point(X, HalfStep);
            U_P3 = new Point(X + QuarterStep, 0);
            D_P1 = new Point(X - QuarterStep, Height);
            D_P2 = new Point(X, Height - HalfStep);
            D_P3 = new Point(X + QuarterStep, Height);
            Point[] CurvePoints = new[] { U_P1, U_P2, U_P3 };
            for (int i = 0; i < 2; i++) {
                if (i == 1) { CurvePoints = new[] { D_P1, D_P2, D_P3 }; }
                using (SolidBrush FillBrush = new SolidBrush(Marker)) {
                    e.Graphics.FillPolygon(FillBrush, CurvePoints);
                }
            }

        }
        private void DrawTimeMarkers(PaintEventArgs e) {
            if (Width > 0) {
                //-00:00-
                int TextLocationY = (int)((float)(Height - UnitTextSize.Height) / 2.0f);
                int MarkSize = TimeWidth + ((int)UnitTextSize.Width);
                int Spacing = (2 * (int)UnitTextSize.Width);
                int TimeSelectWidth = (int)e.Graphics.MeasureString("00:00:00", Font).Width;
                TimePrecision ZoomPrecision = TimePrecision.Hours;
                if (TimeDifference.TotalHours <= 1) {
                    if (TimeDifference.TotalMinutes <= 1) {
                        if (TimeDifference.TotalSeconds <= 1) {
                            ZoomPrecision = TimePrecision.Milliseconds;
                            TimeSelectWidth = (int)e.Graphics.MeasureString("00:00:00", Font).Width;
                        }
                        else { ZoomPrecision = TimePrecision.Seconds; }
                    }
                    else { ZoomPrecision = TimePrecision.Minutes; }
                }
                else { ZoomPrecision = TimePrecision.Hours; }
                //int MarkerWindowCount = (int)((float)Width / (float)MarkSize);
                //TimeSelectWidth += Spacing;
                int MaximumLabels = (int)(((float)Width / (float)TimeSelectWidth / 1.5f));
                int SortWidth = Width - (2 * TimeSelectWidth);
                float TimeSpacing = 3;
                switch (ZoomPrecision) {
                    case TimePrecision.Hours:
                        if (TimeDifference.TotalDays >= 1) {
                            TimeSpacing = (24.0f / (float)MaximumLabels);
                        }
                        else {
                            TimeSpacing = ((float)TimeDifference.TotalHours / (float)MaximumLabels);
                        }
                        break;
                    case TimePrecision.Minutes:
                        if (TimeDifference.TotalHours == 1) {
                            TimeSpacing = (60.0f / (float)MaximumLabels);
                        }
                        else {
                            TimeSpacing = ((float)TimeDifference.TotalMinutes / (float)MaximumLabels);
                        }
                        break;
                    case TimePrecision.Seconds:
                        if (TimeDifference.TotalMinutes == 1) {
                            TimeSpacing = (60.0f / (float)MaximumLabels);
                        }
                        else {
                            TimeSpacing = ((float)TimeDifference.TotalSeconds / (float)MaximumLabels);
                        }
                        break;
                    case TimePrecision.Milliseconds:
                        if (TimeDifference.TotalMinutes == 1) {
                            TimeSpacing = (60.0f / (float)MaximumLabels);
                        }
                        else {
                            TimeSpacing = ((float)TimeDifference.TotalSeconds / (float)MaximumLabels);
                        }
                        break;
                    default:
                        break;
                }

                using (SolidBrush TextBrush = new SolidBrush(timeMarkerTextColor)) {
                    for (int i = 1; i < MaximumLabels; i++) {
                        TimeSpan TimeAtPixel = AddTime(spanStart, TimeSpacing * (float)i, ZoomPrecision);
                        string Output = TimeAtPixel.ToString();
                        int TextLocationX = (int)(((float)i / (float)MaximumLabels) * (float)Width);
                        using (SolidBrush MarkerBrush = new SolidBrush(timeMarkerColor)) {
                            using (Pen MarkPen = new Pen(MarkerBrush)) {
                                Point NorthPoint = new Point(TextLocationX, 0);
                                Point SouthPoint = new Point(TextLocationX, Height);
                                e.Graphics.DrawLine(MarkPen, NorthPoint, SouthPoint);
                            }
                        }
                        e.Graphics.DrawString(Output, Font, TextBrush, new Point(TextLocationX - (int)((float)TimeSelectWidth / 2.0f), TextLocationY));
                    }
                }

            }
        }
        private void DrawSelectionBox(PaintEventArgs e) {
            if (Selection == CursorMode.InSelection) {
                Rectangle SelectionBox;
                int SelectionStart = GetTimeToPoint(selectStart);
                int SelectionEnd = GetTimeToPoint(selectEnd);
                if (selectStart < selectEnd) {
                    SelectionBox = new Rectangle(SelectionStart, 0, SelectionEnd - SelectionStart, Height);
                }
                else if (selectStart > selectEnd) {
                    SelectionBox = new Rectangle(SelectionEnd, 0, SelectionStart - SelectionEnd, Height);
                }
                else { return; }
                using (SolidBrush FillBrush = new SolidBrush(Color.FromArgb(selectionOpacity, selectColor.R, selectColor.G, selectColor.B))) {
                    e.Graphics.FillRectangle(FillBrush, SelectionBox);
                }
                Color CursorColor = selectColor;
                if (mode == EditMode.Paint) {
                    CursorColor = currentPaintColor;
                }
                using (SolidBrush BorderBrush = new SolidBrush(selectColor)) {
                    using (Pen BorderPen = new Pen(BorderBrush)) {
                        e.Graphics.DrawRectangle(BorderPen, SelectionBox);
                    }
                }
            }
        }
        #endregion
        #region Drawing Support
        private Rectangle GetEventBox(PaintedTimeEvent Event) {
            TimeSpan EventTimeStart;
            TimeSpan EventTimeEnd;
            if (Event.TimeStart.Ticks < spanStart.Ticks) { EventTimeStart = spanStart; }
            else { EventTimeStart = Event.TimeStart; }
            if (Event.TimeEnd.Ticks > spanEnd.Ticks) { EventTimeEnd = spanEnd; }
            else { EventTimeEnd = Event.TimeEnd; }
            Rectangle EventBoundingBox = new Rectangle(0, 0, 0, Height);
            int SelectionStart = GetTimeToPoint(EventTimeStart);
            int SelectionEnd = GetTimeToPoint(EventTimeEnd);
            EventBoundingBox = new Rectangle(SelectionStart, 0, SelectionEnd - SelectionStart, Height);
            return EventBoundingBox;

        }
        TimeSpan AddTime(TimeSpan Input, float TimeToAdd, TimePrecision AddPrecision) {
            int Hours = 0;
            int Minutes = 0;
            int Seconds = 0;
            int Milliseconds = 0;

            TimeSpan TimeChange;
            switch (AddPrecision) {
                case TimePrecision.Hours:
                    Hours = (int)Math.Floor(TimeToAdd);
                    Minutes = (int)(60.0f * (TimeToAdd - (float)Hours));
                    TimeChange = new TimeSpan(0, Hours, Minutes, 0, 0);
                    return Input.Add(TimeChange);
                case TimePrecision.Minutes:
                    Minutes = (int)Math.Floor(TimeToAdd);
                    Seconds = (int)(60.0f * (TimeToAdd - (float)Hours));
                    TimeChange = new TimeSpan(0, 0, Minutes, Seconds, 0);
                    return Input.Add(TimeChange);
                case TimePrecision.Seconds:
                    Seconds = (int)Math.Floor(TimeToAdd);
                    Milliseconds = (int)(60.0f * (TimeToAdd - (float)Hours));
                    TimeChange = new TimeSpan(0, 0, 0, Seconds, Milliseconds);
                    return Input.Add(TimeChange);
                default:
                    return Input;
            }
        }
        private string MarkerToTime(int i, int MaxMarkers) {
            const float MinutesPerDay = 1440;
            int MinutesRaw = (int)(((float)i / (float)MaxMarkers) * MinutesPerDay);
            int Minutes = MinutesRaw % 60;
            int Hours = (int)Math.Floor((float)MinutesRaw / 60.0f);
            string Output = Hours.ToString("D2") + ':' + Minutes.ToString("D2");
            return Output;
        }
        #endregion
        #region Events
        private void EvaluateEvents() {
            List<LinkedObjects> LinkedEvents = new List<LinkedObjects>();
            foreach (PaintedTimeEvent Pve in PaintedEvents) {
                int index = LinkedEvents.FindIndex(item => item.ID == Pve.ID);
                bool Status = false;
                if (Pve.IsCurrent) { Status = true; }
                if (index >= 0) {
                    if (Status == true) { LinkedEvents[index].Status = true; }
                }
                else {
                    LinkedObjects LinkedEvent = new LinkedObjects(Pve.ID, Status, Pve.Tag);
                    LinkedEvents.Add(LinkedEvent);
                }

            }
            ValueInRange?.Invoke(this, new PaintedEventArgs(LinkedEvents));
        }
        bool PreventOverload = false;
        private void Ticker_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            if (isActive == true) {
                if (DateTime.Now.Second <= 1) {
                    if (PreventOverload == false) {
                        EvaluateEvents();
                        PreventOverload = true;
                    }
                }
                else {
                    PreventOverload = false;
                }
            }
            else {
                PreventOverload = false;
            }

            Invalidate();
        }
        protected override void OnLostFocus(EventArgs e) {
            Invalidate();
            base.OnLostFocus(e);
        }
        protected override void OnGotFocus(EventArgs e) {
            Invalidate();
            base.OnGotFocus(e);
        }
        protected override void OnEnter(EventArgs e) {
            Invalidate();
            base.OnEnter(e);
        }
        protected override void OnLoad(EventArgs e) {
            DoubleBuffered = true;
        }
        protected override void OnResize(EventArgs e) {
            Invalidate();
        }
        private void TimePainter_Load(object sender, EventArgs e) {

        }
        private void OnMouseDoubleClick(object? sender, MouseEventArgs e) {
            //base.OnMouseDoubleClick(e);
        }
        private void OnMouseClick(object? sender, MouseEventArgs e) {
            Invalidate();
            //base.OnMouseDown(e);
            //selectStart = GetTimeAtCursor(e.X);
            //Selection = CursorMode.InSelection;
        }
        private void OnMouseUp(object? sender, MouseEventArgs e) {
            if (Mode == EditMode.Zoom) {
                if (selectStart.Ticks < selectEnd.Ticks) {
                    spanStart = selectStart;
                    SpanEnd = selectEnd;
                }
                if (selectStart.Ticks > selectEnd.Ticks) {
                    spanStart = selectEnd;
                    SpanEnd = selectStart;
                }
            }
            else if (mode == EditMode.Paint) {
                PerformPaint();
            }
            else if (mode == EditMode.Eraser) {
                PerformErase();
            }
            else if (mode == EditMode.Select) {
                PerformSelect();
            }
            else if (mode == EditMode.Move) {
                MergeEvents();
            }
            Selection = CursorMode.Nothing;
            Invalidate();
        }
        private void OnMouseWheel(object? sender, MouseEventArgs e) {
            //base.OnMouseWheel(e);
        }
        CursorMode Selection = CursorMode.Nothing;
        private void OnMouseDown(object? sender, MouseEventArgs e) {
            selectStart = GetTimeAtCursor(e.X);
            selectEnd = selectStart;
            if (Mode == EditMode.Clean) {
                Selection = CursorMode.Nothing;
                PerformClean();
            }
            else if (Mode == EditMode.Fill) {
                Selection = CursorMode.Nothing;
                PerformFill();
            }
            else if (Mode != EditMode.Move) {
                ClearSelection();
                Selection = CursorMode.InSelection;
            }
            else {
                Selection = CursorMode.InMove;
                foreach (PaintedTimeEvent Pte in PaintedEvents) {
                    if (Pte.Selected == true) {
                        Pte.LatchCurrentPosition();
                    }
                }
            }
        }
        private void ClearSelection() {
            foreach (PaintedTimeEvent Pte in PaintedEvents) {
                Pte.Selected = false;
            }
        }
        private void OnMouseMove(object? sender, MouseEventArgs e) {
            if (Selection == CursorMode.InSelection) {
                selectEnd = GetTimeAtCursor(e.X);
                Invalidate();
            }
            else if (Selection == CursorMode.InMove) {
                selectEnd = GetTimeAtCursor(e.X);
                PerformMove();
                Invalidate();
            }
        }
        #endregion
        #region Tools
        public void CleanAndMerge() {
            for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                if (PaintedEvents[i].TimeStart == PaintedEvents[i].TimeEnd) {
                    PaintedEvents.RemoveAt(i);
                }
            }
            MergeEvents();
        }
        private void PerformClean() {
            try {
                for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                    if (PaintedEvents[i].ID == currentPaintID) {
                        PaintedEvents.RemoveAt(i);
                    }
                }
            }
            catch { }
        }
        private void PerformFill() {
            long ValueEventStart = 0;
            long ValueEventEnd = SpanEnd.Ticks;

            long DifferenceMin = 0;
            long DifferenceMax = 0;
            int i = 0;
            int j = 0;
            foreach (PaintedTimeEvent Pte in PaintedEvents) {
                if (Pte.ID == currentPaintID) {
                    if ((Pte.TimeStart.Ticks >= selectStart.Ticks) && (Pte.TimeEnd.Ticks <= selectStart.Ticks)) {
                        return;//Do Nothing
                    }
                    if (Pte.TimeStart.Ticks <= selectStart.Ticks) {
                        if (i == 0) {
                            ValueEventStart = Pte.TimeStart.Ticks;
                            DifferenceMin = selectStart.Ticks - ValueEventStart;
                        }
                        else {
                            long CurrentDifference = selectStart.Ticks - Pte.TimeStart.Ticks;
                            if (DifferenceMin > CurrentDifference) { DifferenceMin = CurrentDifference; ValueEventStart = Pte.TimeStart.Ticks; }
                        }
                        i++;
                    }
                    if (Pte.TimeEnd.Ticks >= selectStart.Ticks) {
                        if (j == 0) {
                            ValueEventEnd = Pte.TimeEnd.Ticks;
                            DifferenceMin = ValueEventEnd - selectStart.Ticks;
                        }
                        else {
                            long CurrentDifference = Pte.TimeEnd.Ticks - selectStart.Ticks;
                            if (DifferenceMax > CurrentDifference) { DifferenceMax = CurrentDifference; ValueEventEnd = Pte.TimeEnd.Ticks; }
                        }
                        j++;
                    }
                }
            }
            PaintedTimeEvent PaintEvent = new PaintedTimeEvent(currentPaintColor, new TimeSpan(ValueEventStart), new TimeSpan(ValueEventEnd), currentPaintID, currentPaintTag);
            PaintedEvents.Add(PaintEvent);
            MergeEvents();
        }
        private void PerformMove() {
            PaintedTimeEvent? MinimumLeftShift = null;
            PaintedTimeEvent? MinimumRightShift = null;
            int i = 0;
            foreach (PaintedTimeEvent Pte in PaintedEvents) {
                if (Pte.Selected == true) {
                    if (i == 0) { MinimumLeftShift = Pte; MinimumRightShift = Pte; }
                    if (MinimumLeftShift != null) {
                        if (MinimumLeftShift.TimeStart.Ticks > Pte.TimeStart.Ticks) {
                            MinimumLeftShift = Pte;
                        }
                    }
                    if (MinimumRightShift != null) {
                        if (MinimumRightShift.TimeEnd.Ticks < Pte.TimeEnd.Ticks) {
                            MinimumRightShift = Pte;
                        }
                    }
                    i++;
                }
            }
            if ((MinimumLeftShift != null) && (MinimumRightShift != null)) {
                long CompareDifference = selectEnd.Ticks - selectStart.Ticks;
                long Difference = CompareDifference;
                if (MinimumLeftShift.TimeStart.Ticks <= 0) {
                    Difference = -MinimumLeftShift.BeforeMovedStart.Ticks;
                    if (CompareDifference > Difference) {
                        Difference = CompareDifference;
                    }
                }
                else if (MinimumRightShift.TimeEnd >= SpanEnd) {
                    Difference = SpanEnd.Ticks - MinimumRightShift.BeforeMovedEnd.Ticks;
                    if (CompareDifference < Difference) {
                        Difference = CompareDifference;
                    }
                }

                foreach (PaintedTimeEvent Pte in PaintedEvents) {
                    if (Pte.Selected == true) {
                        Pte.Move(Difference);
                    }
                }
            }
        }
        private void PerformSelect() {
            TimeSpan TimeEventStart;
            TimeSpan TimeEventEnd;
            if (selectStart.Ticks < selectEnd.Ticks) {
                TimeEventStart = selectStart;
                TimeEventEnd = selectEnd;
            }
            else if (selectStart.Ticks > selectEnd.Ticks) {
                TimeEventStart = selectEnd;
                TimeEventEnd = selectStart;
            }
            else {
                return; //We don't want to paint invaild data.
            }
            foreach (PaintedTimeEvent Pte in PaintedEvents) {
                if ((showAllPaintedEvents == true) || (Pte.ID == currentPaintID)) {
                    if (Pte.ContainsSelection(TimeEventStart, TimeEventEnd)) { Pte.Selected = true; }
                    else { Pte.Selected = false; }
                }
            }
        }
        private void PerformPaint() {
            TimeSpan TimeEventStart;
            TimeSpan TimeEventEnd;
            if (selectStart.Ticks < selectEnd.Ticks) {
                TimeEventStart = selectStart;
                TimeEventEnd = selectEnd;
            }
            else if (selectStart.Ticks > selectEnd.Ticks) {
                TimeEventStart = selectEnd;
                TimeEventEnd = selectStart;
            }
            else {
                return; //We don't want to paint invaild data.
            }
            PaintedTimeEvent PaintEvent = new PaintedTimeEvent(currentPaintColor, TimeEventStart, TimeEventEnd, currentPaintID, currentPaintTag);
            PaintedEvents.Add(PaintEvent);
            MergeEvents();
            //Invalidate();
        }
        private void PerformErase() {
            if (PaintedEvents.Count > 0) {
                TimeSpan TimeEventStart;
                TimeSpan TimeEventEnd;
                if (selectStart.Ticks < selectEnd.Ticks) {
                    TimeEventStart = selectStart;
                    TimeEventEnd = selectEnd;
                }
                else if (selectStart.Ticks > selectEnd.Ticks) {
                    TimeEventStart = selectEnd;
                    TimeEventEnd = selectStart;
                }
                else {
                    return; //We don't want to paint invaild data.
                }
                try {
                    for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                        if (currentPaintID == PaintedEvents[i].ID) {
                            BarOverlappingType Overlapping = PaintedEvents[i].ExtensionType(TimeEventStart, TimeEventEnd);
                            if (Overlapping != BarOverlappingType.NoOverlap) {
                                if (Overlapping == BarOverlappingType.Engulfs) {
                                    PaintedEvents.RemoveAt(i);
                                }
                                else if (Overlapping == BarOverlappingType.RightOverlap) {
                                    PaintedEvents[i].TimeEnd = TimeEventStart;
                                }
                                else if (Overlapping == BarOverlappingType.LeftOverlap) {
                                    PaintedEvents[i].TimeStart = TimeEventEnd;
                                }
                                else if (Overlapping == BarOverlappingType.FullyContained) {
                                    PaintedTimeEvent SpiltBar = new PaintedTimeEvent(PaintedEvents[i].DisplayColor, TimeEventEnd, PaintedEvents[i].TimeEnd, PaintedEvents[i].ID, PaintedEvents[i].Tag);
                                    PaintedEvents.Add(SpiltBar);
                                    PaintedEvents[i].TimeEnd = TimeEventStart;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }
        public void MergeEvents() {
            if (PaintedEvents.Count > 1) {
                try {
                    for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                        for (int j = i - 1; j >= 0; j--) {
                            BarOverlappingType Overlapping = BarOverlappingType.NoOverlap;
                            if (i < PaintedEvents.Count) {
                                Overlapping = PaintedEvents[j].EventOverlapping(PaintedEvents[i]);
                            }
                            if (i != j) {
                                if (Overlapping != BarOverlappingType.NoOverlap) {
                                    TimeSpan ModifiedStart;
                                    TimeSpan ModifiedEnd;
                                    if (Overlapping == BarOverlappingType.Engulfs) {
                                        ModifiedStart = PaintedEvents[i].TimeStart;
                                        ModifiedEnd = PaintedEvents[i].TimeEnd;
                                        PaintedEvents[j].TimeStart = ModifiedStart;
                                        PaintedEvents[j].TimeEnd = ModifiedEnd;
                                    }
                                    else if (Overlapping == BarOverlappingType.LeftOverlap) {
                                        ModifiedStart = PaintedEvents[i].TimeStart;
                                        ModifiedEnd = PaintedEvents[j].TimeEnd;
                                        PaintedEvents[j].TimeStart = ModifiedStart;
                                        PaintedEvents[j].TimeEnd = ModifiedEnd;
                                    }
                                    else if (Overlapping == BarOverlappingType.RightOverlap) {
                                        ModifiedStart = PaintedEvents[j].TimeStart;
                                        ModifiedEnd = PaintedEvents[i].TimeEnd;
                                        PaintedEvents[j].TimeStart = ModifiedStart;
                                        PaintedEvents[j].TimeEnd = ModifiedEnd;
                                    }
                                    PaintedEvents.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                catch { }
            }

        }
        private TimeSpan GetTimeAtCursor(int X) {
            long TicksAtCursor = SpanStart.Ticks + (long)(((float)X / (float)Width) * (float)TimeDifference.Ticks);
            return new TimeSpan(TicksAtCursor);
        }
        private int GetTimeToPoint(TimeSpan TimeAtPoint) {
            int LocationX = (int)(((float)(TimeAtPoint.Ticks - SpanStart.Ticks) / (float)TimeDifference.Ticks) * (float)Width);
            return LocationX;
        }
        #endregion
        #region Enumerations
        public enum EditMode {
            Select = 0x01,
            Paint = 0x02,
            Eraser = 0x03,
            Move = 0x04,
            Fill = 0x05,
            Delete = 0x06,
            Clean = 0x07,
            Zoom = 0xFF
        }
        private enum CursorMode {
            Nothing = 0x00,
            InSelection = 0x01,
            ObjectHit = 0x02,
            InMove = 0x03
        }
        public enum TimePrecision {
            Hours = 0x00,
            Minutes = 0x01,
            Seconds = 0x02,
            Milliseconds = 0x03
        }
        public enum SpanValue {
            SpanStart = 0x00,
            SpanEnd = 0x01
        }
        #endregion 
    }
    public class PaintedEventArgs : EventArgs {
        public List<LinkedObjects> Events = new List<LinkedObjects>();
        public PaintedEventArgs(List<LinkedObjects> PaintEvents) {
            Events = PaintEvents;
        }
    }
    public class LinkedObjects {
        public bool Status;
        public object? Linked = null;
        public string ID;
        public LinkedObjects(string iD, bool status, object? linked) {
            Status = status;
            Linked = linked;
            ID = iD;
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (obj.GetType() == typeof(LinkedObjects)) {
                if (((LinkedObjects)obj).ID == this.ID) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return base.ToString() ?? "";
        }
    }
    public class TriggeredEventArgs : EventArgs {
        public bool Status { get; private set; }
        public string EventID { get; private set; }
        public string MemberOf { get; private set; }
        public TriggeredEventArgs(bool status, string eventID, string memberOf) {
            Status = status;
            EventID = eventID;
            MemberOf = memberOf;
        }
    }
    public class PaintedTimeEvent {
        private Color displayColor = Color.Crimson;
        private TimeSpan timeStart = new TimeSpan(0);
        private TimeSpan timeEnd = new TimeSpan(0);
        private TimeSpan LatchedValueStart = new TimeSpan(0);
        public TimeSpan BeforeMovedStart { get { return LatchedValueStart; } }
        private TimeSpan LatchedValueEnd = new TimeSpan(0);
        public TimeSpan BeforeMovedEnd { get { return LatchedValueEnd; } }
        public bool Selected = false;
        public static System.Timers.Timer? Ticker;
        private string iD = "";
        private object? tag = null;
        public object? Tag { get => tag; set => tag = value; }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set => isVisible = value; }

        public delegate void StatusUpdateHandler(object sender, TriggeredEventArgs e);
        public event StatusUpdateHandler? OnStatusChange;
        public void LatchCurrentPosition() {
            LatchedValueStart = timeStart;
            LatchedValueEnd = timeEnd;
        }
        public void Move(long Amount) {
            timeStart = new TimeSpan(LatchedValueStart.Ticks + Amount);
            timeEnd = new TimeSpan(LatchedValueEnd.Ticks + Amount);
        }
        public bool Contains(TimeSpan SelectStart, TimeSpan SelectEnd) {
            TimeSpan TimeEventStart;
            TimeSpan TimeEventEnd;
            if (SelectStart.Ticks < SelectEnd.Ticks) {
                TimeEventStart = SelectStart;
                TimeEventEnd = SelectEnd;
            }
            else if (SelectStart.Ticks > SelectEnd.Ticks) {
                TimeEventStart = SelectEnd;
                TimeEventEnd = SelectStart;
            }
            else {
                TimeEventStart = new TimeSpan(1, 0, 0, 0, 1);
                TimeEventEnd = new TimeSpan(1, 0, 0, 0, 1); //Out of range
            }
            if ((timeStart.Ticks <= TimeEventStart.Ticks) && (timeEnd.Ticks > TimeEventEnd.Ticks)) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool ContainsSelection(TimeSpan SelectStart, TimeSpan SelectEnd) {
            TimeSpan TimeEventStart;
            TimeSpan TimeEventEnd;
            if (SelectStart.Ticks < SelectEnd.Ticks) {
                TimeEventStart = SelectStart;
                TimeEventEnd = SelectEnd;
            }
            else if (SelectStart.Ticks > SelectEnd.Ticks) {
                TimeEventStart = SelectEnd;
                TimeEventEnd = SelectStart;
            }
            else {
                TimeEventStart = new TimeSpan(1, 0, 0, 0, 1);
                TimeEventEnd = new TimeSpan(1, 0, 0, 0, 1); //Out of range
            }
            if ((TimeEventStart.Ticks <= timeStart.Ticks) && (timeEnd.Ticks <= TimeEventEnd.Ticks)) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool ContainsValue(decimal Value) {
            if ((timeStart.Ticks <= Value) && (timeEnd.Ticks >= Value)) {
                return true;
            }
            return false;
        }
        public BarOverlappingType EventOverlapping(PaintedTimeEvent Event) {
            if (this.Equals(Event) == true) {
                TimeSpan TimeEventStart = Event.TimeStart;
                TimeSpan TimeEventEnd = Event.TimeEnd;
                return ExtensionType(TimeEventStart, TimeEventEnd);
            }
            else { return BarOverlappingType.NoOverlap; }
        }
        public BarOverlappingType ExtensionType(TimeSpan SelectStart, TimeSpan SelectEnd) {
            TimeSpan TimeEventStart;
            TimeSpan TimeEventEnd;
            if (Contains(SelectStart, SelectEnd) == true) {
                return BarOverlappingType.FullyContained;
            }
            else {
                if (SelectStart.Ticks < SelectEnd.Ticks) {
                    TimeEventStart = SelectStart;
                    TimeEventEnd = SelectEnd;
                }
                else if (SelectStart.Ticks > SelectEnd.Ticks) {
                    TimeEventStart = SelectEnd;
                    TimeEventEnd = SelectStart;
                }
                else {
                    TimeEventStart = new TimeSpan(1, 0, 0, 0, 1);
                    TimeEventEnd = new TimeSpan(1, 0, 0, 0, 1); //Out of range
                }
                if (TimeEventStart.Ticks > this.timeEnd.Ticks) {
                    return BarOverlappingType.NoOverlap;
                }
                else if (TimeEventEnd.Ticks < this.timeStart.Ticks) {
                    return BarOverlappingType.NoOverlap;
                }
                else {
                    if ((TimeEventStart.Ticks <= this.timeStart.Ticks) && (TimeEventEnd.Ticks >= this.timeEnd.Ticks)) {
                        return BarOverlappingType.Engulfs;
                    }
                    else {
                        if ((TimeEventStart.Ticks < this.timeStart.Ticks) && (TimeEventEnd.Ticks <= this.timeEnd.Ticks)) {
                            return BarOverlappingType.LeftOverlap;
                        }
                        else if ((TimeEventStart.Ticks >= this.timeStart.Ticks) && (TimeEventEnd.Ticks > this.timeEnd.Ticks)) {
                            return BarOverlappingType.RightOverlap;
                        }
                    }
                }
            }
            return BarOverlappingType.NoOverlap;
        }
        public PaintedTimeEvent() {
            if (Ticker == null) {
                Ticker = new System.Timers.Timer();
                Ticker.Interval = 500;
                Ticker.AutoReset = true;
                Ticker.Start();
                Ticker.Enabled = true;
            }
            this.tag = Tag;
            Ticker.Elapsed += Ticker_Elapsed;
        }
        public PaintedTimeEvent(Color displayColor, TimeSpan timeStart, TimeSpan timeEnd, string iD, object? Tag, bool NoEventRegister = false) {
            this.displayColor = displayColor;
            this.timeStart = timeStart;
            this.timeEnd = timeEnd;
            this.iD = iD;
            if (Ticker == null) {
                Ticker = new System.Timers.Timer();
                Ticker.Interval = 500;
                Ticker.AutoReset = true;
                Ticker.Start();
                Ticker.Enabled = true;
            }
            this.tag = Tag;
            if (NoEventRegister == false) {
                Ticker.Elapsed += Ticker_Elapsed;
            }
        }
        private void Ticker_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            CheckState();
        }
        private bool isCurrent = false;
        public bool IsCurrent {
            get {
                return isCurrent;
            }
        }
        public bool isCurrentPrevious = false;
        private void CheckState() {
            DateTime Midnight = DateTime.Now.Date;
            DateTime CalcStart = Midnight.Add(timeStart);
            DateTime CalcEnd = Midnight.Add(timeEnd);
            DateTime Now = DateTime.Now;
            if ((Now.Ticks >= CalcStart.Ticks) && (Now.Ticks < CalcEnd.Ticks)) {
                isCurrent = true;
            }
            else { isCurrent = false; }
            if (isCurrent != isCurrentPrevious) {
                isCurrentPrevious = isCurrent;
                UpdateStatus(isCurrent, iD, member);
            }
        }
        private void UpdateStatus(bool status, string eventID, string memberOf) {
            if (OnStatusChange == null) return;
            TriggeredEventArgs args = new TriggeredEventArgs(status, eventID, memberOf);
            OnStatusChange(this, args);
        }

        public override bool Equals(object? obj) {
            //if (obj.GetType() == typeof(PaintedTimeEvent)) {
            //    PaintedTimeEvent pe = (PaintedTimeEvent)obj;
            //    if ((pe.iD == this.iD) && (pe.member == this.member)) {

            //    }
            //    return false;
            //}
            //else { return false; }
            if (obj == null) {
                return false;
            }
            if (GetHashCode() == obj.GetHashCode()) {
                return true;
            }
            else {
                return false;
            }
        }
        public override int GetHashCode() {
            int hashCode = -802591165;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(iD);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(member);
            return hashCode;
        }

        public Color DisplayColor { get => displayColor; set => displayColor = value; }
        public TimeSpan TimeStart { get => timeStart; set => timeStart = value; }
        public TimeSpan TimeEnd { get => timeEnd; set => timeEnd = value; }
        public string ID { get => iD; set => iD = value; }
        public string Member { get => member; set => member = value; }
        private string member = "";

    }
    public enum BarOverlappingType {
        NoOverlap = 0x00,
        FullyContained = 0x01,
        LeftOverlap = 0x02,
        RightOverlap = 0x03,
        Engulfs = 0x04
    }

}
