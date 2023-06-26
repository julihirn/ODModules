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
    public partial class ValuePainter : UserControl {
        public delegate void StatusUpdateHandler(object sender, PaintedEventArgs e);
        public event StatusUpdateHandler? ValueInRange;


        public ValuePainter() {
            //InitializeComponent();
            spanStart = 0;
            spanEnd = 100;

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
        private List<PaintedValueEvent> paintedEvents = new List<PaintedValueEvent>();
        [System.ComponentModel.Category("Events")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<PaintedValueEvent> PaintedEvents {
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
        private bool currentValueMarker = true;
        [System.ComponentModel.Category("Show/Hide")]
        public bool CurrentValueMarker {
            get {
                return currentValueMarker;
            }
            set {
                currentValueMarker = value;
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
                    foreach (PaintedValueEvent e in PaintedEvents) {
                        e.IsVisible = true;
                    }
                }
                Invalidate();
            }
        }
        private Color currentValueMarkerColor = Color.Red;
        [System.ComponentModel.Category("Appearance")]
        public Color CurrentValueMarkerColor {
            get {
                return currentValueMarkerColor;
            }
            set {
                currentValueMarkerColor = value;
                Invalidate();
            }
        }
        private Color valueMarkerColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color ValueMarkerColor {
            get {
                return valueMarkerColor;
            }
            set {
                valueMarkerColor = value;
                Invalidate();
            }
        }
        private Color valueMarkerTextColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ValueMarkerTextColor {
            get {
                return valueMarkerTextColor;
            }
            set {
                valueMarkerTextColor = value;
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
        private Color selectedborderColor;
        [Category("Appearance")]
        public Color SelectedBorderColor {
            get { return selectedborderColor; }
            set { selectedborderColor = value; Invalidate(); }
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

        private decimal selectStart;
        private decimal selectEnd;
        private decimal spanStart;
        private decimal spanEnd;
        private decimal currentValue = 0;
        [System.ComponentModel.Category("Value")]
        decimal PastValue = 0;
        public decimal Value {
            get { return currentValue; }
            set {
                currentValue = value;

                if (PastValue != value) {
                    PastValue = value;
                    if (isActive == true) {
                        EvaluateEvents();
                    }
                    Invalidate();
                }
            }
        }
        [System.ComponentModel.Category("Value")]
        public decimal SpanStart {
            get { return spanStart; }
            set {
                spanStart = value;
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Value")]
        public decimal SpanEnd {
            get { return spanEnd; }
            set {
                spanEnd = value;
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Value")]
        public decimal SelectStart { get => selectStart; }
        [System.ComponentModel.Category("Value")]
        public decimal SelectEnd { get => selectEnd; }
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
            DrawMarkers(e);
            DrawSelectionBox(e);
            if (currentValueMarker == true) {
                DrawCurrentValueMarker(e);
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
                foreach (PaintedValueEvent Pte in PaintedEvents) {
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
        private void DrawCurrentValueMarker(PaintEventArgs e) {
            DateTime Now = DateTime.Now;
            if ((currentValue >= spanStart) && (currentValue < spanEnd)) {
                decimal PosX = currentValue - spanStart;
                decimal Diff = Math.Abs(spanEnd) - Math.Abs(spanStart);
                int PositionX = (int)((PosX / Diff) * (decimal)Width);
                using (SolidBrush PenBrush = new SolidBrush(currentValueMarkerColor)) {
                    using (Pen MarkPen = new Pen(currentValueMarkerColor)) {
                        Point NorthPoint = new Point(PositionX, 0);
                        Point SouthPoint = new Point(PositionX, Height);
                        e.Graphics.DrawLine(MarkPen, NorthPoint, SouthPoint);

                    }
                }
                DrawMarkerTriangles(e, PositionX, currentValueMarkerColor);
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
        private void DrawMarkers(PaintEventArgs e) {
            decimal Difference = Math.Abs(spanEnd) - Math.Abs(spanStart);
            if (Width > 0) {
                int Spacing = (2 * (int)UnitTextSize.Width);
                int TextLocationY = (int)((float)(Height - UnitTextSize.Height) / 2.0f);
                int MarkerSelectWidth = (int)e.Graphics.MeasureString(spanStart.ToString("0.00"), Font).Width;
                int MarkerEndWidth = (int)e.Graphics.MeasureString(spanEnd.ToString("0.00"), Font).Width;
                MarkerSelectWidth = Math.Max(MarkerSelectWidth, MarkerEndWidth);
                int MaximumLabels = (int)(((float)Width / (float)MarkerSelectWidth / 1.5f));
                using (SolidBrush TextBrush = new SolidBrush(valueMarkerTextColor)) {

                    for (int i = 1; i < MaximumLabels; i++) {
                        decimal ValueAtPixel = spanStart + (((decimal)i / (decimal)MaximumLabels) * Difference);
                        string Output = ValueAtPixel.ToString("0.00");
                        int TextLocationX = (int)(((float)i / (float)MaximumLabels) * (float)Width);
                        using (SolidBrush MarkerBrush = new SolidBrush(valueMarkerColor)) {
                            using (Pen MarkPen = new Pen(MarkerBrush)) {
                                Point NorthPoint = new Point(TextLocationX, 0);
                                Point SouthPoint = new Point(TextLocationX, Height);
                                e.Graphics.DrawLine(MarkPen, NorthPoint, SouthPoint);
                            }
                        }
                        e.Graphics.DrawString(Output, Font, TextBrush, new Point(TextLocationX - (int)((float)e.Graphics.MeasureString(ValueAtPixel.ToString("0.00"), Font).Width / 2.0f), TextLocationY));
                    }
                }
            }
        }
        private void DrawSelectionBox(PaintEventArgs e) {
            if (Selection == CursorMode.InSelection) {
                Rectangle SelectionBox;
                int SelectionStart = GetValueToPoint(selectStart);
                int SelectionEnd = GetValueToPoint(selectEnd);
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
        private Rectangle GetEventBox(PaintedValueEvent Event) {
            decimal EventValueStart;
            decimal EventValueEnd;
            if (Event.ValueRangeStart < spanStart) { EventValueStart = spanStart; }
            else { EventValueStart = Event.ValueRangeStart; }
            if (Event.ValueRangeEnd > spanEnd) { EventValueEnd = spanEnd; }
            else { EventValueEnd = Event.ValueRangeEnd; }
            Rectangle EventBoundingBox = new Rectangle(0, 0, 0, Height);
            int SelectionStart = GetValueToPoint(EventValueStart);
            int SelectionEnd = GetValueToPoint(EventValueEnd);
            EventBoundingBox = new Rectangle(SelectionStart, 0, SelectionEnd - SelectionStart, Height);
            return EventBoundingBox;
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
            foreach (PaintedValueEvent Pve in PaintedEvents) {
                int index = LinkedEvents.FindIndex(item => item.ID == Pve.ID);
                bool Status = false;
                if (Pve.ContainsValue(currentValue)) { Status = true; }
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
        private void ValuePainter_Load(object? sender, EventArgs e) {

        }
        protected override void OnLostFocus(EventArgs e) {
            Invalidate();
            Selection = CursorMode.Nothing;
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
            Selection = CursorMode.Nothing;
            Invalidate();
            //base.OnMouseDown(e);
            //selectStart = GetTimeAtCursor(e.X);
            //Selection = CursorMode.InSelection;
        }
        private void OnMouseUp(object? sender, MouseEventArgs e) {
            //if (Mode == EditMode.Zoom) {
            //    if (selectStart < selectEnd) {
            //        spanStart = selectStart;
            //        SpanEnd = selectEnd;
            //    }
            //    if (selectStart > selectEnd) {
            //        spanStart = selectEnd;
            //        SpanEnd = selectStart;
            //    }
            //}
            if (mode == EditMode.Paint) {
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
            Selection = CursorMode.Nothing;
            selectStart = GetValueAtCursor(e.X);
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
                foreach (PaintedValueEvent Pte in PaintedEvents) {
                    if (Pte.Selected == true) {
                        Pte.LatchCurrentPosition();
                    }
                }
            }

        }
        private void OnMouseMove(object? sender, MouseEventArgs e) {
            if (Selection == CursorMode.InSelection) {
                selectEnd = GetValueAtCursor(e.X);
                Invalidate();
            }
            else if (Selection == CursorMode.InMove) {
                selectEnd = GetValueAtCursor(e.X);
                PerformMove();
                Invalidate();
            }
        }
        private void ClearSelection() {
            foreach (PaintedValueEvent Pte in PaintedEvents) {
                Pte.Selected = false;
            }
        }
        #endregion
        #region Tools
        public void CleanAndMerge() {
            for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                if (PaintedEvents[i].ValueRangeStart == PaintedEvents[i].ValueRangeEnd) {
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
            decimal ValueEventStart = 0;
            decimal ValueEventEnd = SpanEnd;

            decimal DifferenceMin = 0;
            decimal DifferenceMax = 0;
            int i = 0;
            int j = 0;
            foreach (PaintedValueEvent Pte in PaintedEvents) {
                if (Pte.ID == currentPaintID) {
                    if ((Pte.ValueRangeStart >= selectStart) && (Pte.ValueRangeEnd <= selectStart)) {
                        return;//Do Nothing
                    }
                    if (Pte.ValueRangeStart <= selectStart) {
                        if (i == 0) {
                            ValueEventStart = Pte.ValueRangeStart;
                            DifferenceMin = selectStart - ValueEventStart;
                        }
                        else {
                            decimal CurrentDifference = selectStart - Pte.ValueRangeStart;
                            if (DifferenceMin > CurrentDifference) { DifferenceMin = CurrentDifference; ValueEventStart = Pte.ValueRangeStart; }
                        }
                        i++;
                    }
                    if (Pte.ValueRangeEnd >= selectStart) {
                        if (j == 0) {
                            ValueEventEnd = Pte.ValueRangeEnd;
                            DifferenceMin = ValueEventEnd - selectStart;
                        }
                        else {
                            decimal CurrentDifference = Pte.ValueRangeEnd - selectStart;
                            if (DifferenceMax > CurrentDifference) { DifferenceMax = CurrentDifference; ValueEventEnd = Pte.ValueRangeEnd; }
                        }
                        j++;
                    }
                }
            }
            PaintedValueEvent PaintEvent = new PaintedValueEvent(currentPaintColor, ValueEventStart, ValueEventEnd, currentPaintID, currentPaintTag);
            PaintedEvents.Add(PaintEvent);
            MergeEvents();
        }
        private void PerformMove() {
            PaintedValueEvent? MinimumLeftShift = null;
            PaintedValueEvent? MinimumRightShift = null;
            int i = 0;
            foreach (PaintedValueEvent Pte in PaintedEvents) {
                if (Pte.Selected == true) {
                    if (i == 0) { MinimumLeftShift = Pte; MinimumRightShift = Pte; }
                    if (MinimumLeftShift != null) {
                        if (MinimumLeftShift.ValueRangeStart > Pte.ValueRangeStart) {
                            MinimumLeftShift = Pte;
                        }
                    }
                    if (MinimumRightShift != null) {
                        if (MinimumRightShift.ValueRangeEnd < Pte.ValueRangeEnd) {
                            MinimumRightShift = Pte;
                        }
                    }
                    i++;
                }
            }
            if ((MinimumLeftShift != null) && (MinimumRightShift != null)) {
                decimal CompareDifference = selectEnd - selectStart;
                decimal Difference = CompareDifference;
                if (MinimumLeftShift.ValueRangeStart <= 0) {
                    Difference = -MinimumLeftShift.BeforeMovedStart;
                    if (CompareDifference > Difference) {
                        Difference = CompareDifference;
                    }
                }
                else if (MinimumRightShift.ValueRangeEnd >= SpanEnd) {
                    Difference = SpanEnd - MinimumRightShift.BeforeMovedEnd;
                    if (CompareDifference < Difference) {
                        Difference = CompareDifference;
                    }
                }
                foreach (PaintedValueEvent Pte in PaintedEvents) {
                    if (Pte.Selected == true) {
                        Pte.Move(Difference);
                    }
                }
            }
        }
        private void PerformSelect() {
            decimal ValueEventStart;
            decimal ValueEventEnd;
            if (selectStart < selectEnd) {
                ValueEventStart = selectStart;
                ValueEventEnd = selectEnd;
            }
            else if (selectStart > selectEnd) {
                ValueEventStart = selectEnd;
                ValueEventEnd = selectStart;
            }
            else {
                return; //We don't want to paint invalid data.
            }
            foreach (PaintedValueEvent Pte in PaintedEvents) {
                if ((showAllPaintedEvents == true) || (Pte.ID == currentPaintID)) {
                    if (Pte.ContainsSelection(ValueEventStart, ValueEventEnd)) { Pte.Selected = true; }
                    else { Pte.Selected = false; }
                }
            }
        }
        private void PerformPaint() {
            decimal ValueEventStart;
            decimal ValueEventEnd;
            if (selectStart < selectEnd) {
                ValueEventStart = selectStart;
                ValueEventEnd = selectEnd;
            }
            else if (selectStart > selectEnd) {
                ValueEventStart = selectEnd;
                ValueEventEnd = selectStart;
            }
            else {
                return; //We don't want to paint invalid data.
            }
            PaintedValueEvent PaintEvent = new PaintedValueEvent(currentPaintColor, ValueEventStart, ValueEventEnd, currentPaintID, currentPaintTag);
            PaintedEvents.Add(PaintEvent);
            MergeEvents();
            //Invalidate();
        }
        private void PerformErase() {
            if (PaintedEvents.Count > 0) {
                decimal ValueEventStart;
                decimal ValueEventEnd;
                if (selectStart < selectEnd) {
                    ValueEventStart = selectStart;
                    ValueEventEnd = selectEnd;
                }
                else if (selectStart > selectEnd) {
                    ValueEventStart = selectEnd;
                    ValueEventEnd = selectStart;
                }
                else {
                    return; //We don't want to paint invalid data.
                }
                try {
                    for (int i = PaintedEvents.Count - 1; i >= 0; i--) {
                        if (currentPaintID == PaintedEvents[i].ID) {
                            BarOverlappingType Overlapping = PaintedEvents[i].ExtensionType(ValueEventStart, ValueEventEnd);
                            if (Overlapping != BarOverlappingType.NoOverlap) {
                                if (Overlapping == BarOverlappingType.Engulfs) {
                                    PaintedEvents.RemoveAt(i);
                                }
                                else if (Overlapping == BarOverlappingType.RightOverlap) {
                                    PaintedEvents[i].ValueRangeEnd = ValueEventStart;
                                }
                                else if (Overlapping == BarOverlappingType.LeftOverlap) {
                                    PaintedEvents[i].ValueRangeStart = ValueEventEnd;
                                }
                                else if (Overlapping == BarOverlappingType.FullyContained) {
                                    PaintedValueEvent SpiltBar = new PaintedValueEvent(PaintedEvents[i].DisplayColor, ValueEventEnd, PaintedEvents[i].ValueRangeEnd, PaintedEvents[i].ID, PaintedEvents[i].Tag);
                                    PaintedEvents.Add(SpiltBar);
                                    PaintedEvents[i].ValueRangeEnd = ValueEventStart;
                                }
                            }
                        }
                    }
                }
                catch { }
                // Invalidate();
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
                                    decimal ModifiedStart;
                                    decimal ModifiedEnd;
                                    if (Overlapping == BarOverlappingType.Engulfs) {
                                        ModifiedStart = PaintedEvents[i].ValueRangeStart;
                                        ModifiedEnd = PaintedEvents[i].ValueRangeEnd;
                                        PaintedEvents[j].ValueRangeStart = ModifiedStart;
                                        PaintedEvents[j].ValueRangeEnd = ModifiedEnd;
                                    }
                                    else if (Overlapping == BarOverlappingType.LeftOverlap) {
                                        ModifiedStart = PaintedEvents[i].ValueRangeStart;
                                        ModifiedEnd = PaintedEvents[j].ValueRangeEnd;
                                        PaintedEvents[j].ValueRangeStart = ModifiedStart;
                                        PaintedEvents[j].ValueRangeEnd = ModifiedEnd;
                                    }
                                    else if (Overlapping == BarOverlappingType.RightOverlap) {
                                        ModifiedStart = PaintedEvents[j].ValueRangeStart;
                                        ModifiedEnd = PaintedEvents[i].ValueRangeEnd;
                                        PaintedEvents[j].ValueRangeStart = ModifiedStart;
                                        PaintedEvents[j].ValueRangeEnd = ModifiedEnd;
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
        private decimal GetValueAtCursor(int X) {
            decimal ValueAtCursor = SpanStart + (((decimal)X / (decimal)Width) * (Math.Abs(SpanEnd) - Math.Abs(spanStart)));
            return ValueAtCursor;
        }
        private int GetValueToPoint(decimal TimeAtPoint) {
            int LocationX = (int)((TimeAtPoint - SpanStart) / (Math.Abs(SpanEnd) - Math.Abs(spanStart)) * (decimal)Width);
            return LocationX;
        }
        #endregion
        #region Enumerations
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
        #endregion
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // ValuePainter
            // 
            this.Name = "ValuePainter";
            this.Load += new System.EventHandler(this.ValuePainter_Load);
            this.ResumeLayout(false);
        }


    }
    public class PaintedValueEvent {
        private Color displayColor = Color.Crimson;
        private decimal valueStart = 0.0m;
        private decimal valueEnd = 0.0m;

        private decimal LatchedValueStart = 0.0m;
        public decimal BeforeMovedStart { get { return LatchedValueStart; } }
        private decimal LatchedValueEnd = 0.0m;
        public decimal BeforeMovedEnd { get { return LatchedValueEnd; } }
        public bool Selected = false;
        private string iD = "";
        private object? tag = null;
        public object? Tag { get => tag; set => tag = value; }
        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set => isVisible = value; }

        public void LatchCurrentPosition() {
            LatchedValueStart = valueStart;
            LatchedValueEnd = valueEnd;
        }
        public void Move(decimal Amount) {
            valueStart = LatchedValueStart + Amount;
            valueEnd = LatchedValueEnd + Amount;
        }
        public bool Contains(decimal SelectStart, decimal SelectEnd) {
            decimal ValueEventStart;
            decimal ValueEventEnd;
            if (SelectStart < SelectEnd) {
                ValueEventStart = SelectStart;
                ValueEventEnd = SelectEnd;
            }
            else if (SelectStart > SelectEnd) {
                ValueEventStart = SelectEnd;
                ValueEventEnd = SelectStart;
            }
            else {
                ValueEventStart = 0;
                ValueEventEnd = 0; //Out of range
            }
            if ((valueStart <= ValueEventStart) && (valueEnd > ValueEventEnd)) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool ContainsSelection(decimal SelectStart, decimal SelectEnd) {
            decimal ValueEventStart;
            decimal ValueEventEnd;
            if (SelectStart < SelectEnd) {
                ValueEventStart = SelectStart;
                ValueEventEnd = SelectEnd;
            }
            else if (SelectStart > SelectEnd) {
                ValueEventStart = SelectEnd;
                ValueEventEnd = SelectStart;
            }
            else {
                ValueEventStart = -1;
                ValueEventEnd = -1;
            }
            if ((ValueEventStart <= valueStart) && (valueEnd <= ValueEventEnd)) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool ContainsValue(decimal Value) {
            if ((valueStart <= Value) && (valueEnd >= Value)) {
                return true;
            }
            return false;
        }
        public BarOverlappingType EventOverlapping(PaintedValueEvent Event) {
            if (this.Equals(Event) == true) {
                decimal ValueEventStart = Event.ValueRangeStart;
                decimal ValueEventEnd = Event.ValueRangeEnd;
                return ExtensionType(ValueEventStart, ValueEventEnd);
            }
            else { return BarOverlappingType.NoOverlap; }
        }
        public BarOverlappingType ExtensionType(decimal SelectStart, decimal SelectEnd) {
            decimal EventStart;
            decimal EventEnd;
            if (Contains(SelectStart, SelectEnd) == true) {
                return BarOverlappingType.FullyContained;
            }
            else {
                if (SelectStart < SelectEnd) {
                    EventStart = SelectStart;
                    EventEnd = SelectEnd;
                }
                else if (SelectStart > SelectEnd) {
                    EventStart = SelectEnd;
                    EventEnd = SelectStart;
                }
                else {
                    EventStart = 0;
                    EventEnd = 0; //Out of range
                    return BarOverlappingType.NoOverlap;
                }
                if (EventStart > this.valueEnd) {
                    return BarOverlappingType.NoOverlap;
                }
                else if (EventEnd < this.valueStart) {
                    return BarOverlappingType.NoOverlap;
                }
                else {
                    if ((EventStart <= this.valueStart) && (EventEnd >= this.valueEnd)) {
                        return BarOverlappingType.Engulfs;
                    }
                    else {
                        if ((EventStart < this.valueStart) && (EventEnd <= this.valueEnd)) {
                            return BarOverlappingType.LeftOverlap;
                        }
                        else if ((EventStart >= this.valueStart) && (EventEnd > this.valueEnd)) {
                            return BarOverlappingType.RightOverlap;
                        }
                    }
                }
            }
            return BarOverlappingType.NoOverlap;
        }
        public PaintedValueEvent() { }
        public PaintedValueEvent(Color displayColor, decimal Start, decimal End, string iD, object? Tag) {
            this.displayColor = displayColor;
            this.valueStart = Start;
            this.valueEnd = End;
            this.iD = iD;
            this.tag = Tag;
        }
        private bool isCurrent = false;
        public bool IsCurrent {
            get {
                return isCurrent;
            }
        }
        public override bool Equals(object? obj) {
            if (obj == null) { return false; }
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
        public decimal ValueRangeStart { get => valueStart; set => valueStart = value; }
        public decimal ValueRangeEnd { get => valueEnd; set => valueEnd = value; }
        public string ID { get => iD; set => iD = value; }
        public string Member { get => member; set => member = value; }
        private string member = "";

    }

}
