
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Diagnostics;
using ODModules;
using System.Collections;
using Handlers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Reflection;

namespace ODModules {
    public class Navigator : UserControl {
        public event SelectedIndexChangedHandler? SelectedIndexChanged;
        public delegate void SelectedIndexChangedHandler(object sender, int SelectedIndex);

        public Navigator() {
            this.DoubleBuffered = true;
            Animator.Interval = 10;
            Animator.Enabled = false;
            Animator.Tick += Animator_Tick;
            //for (int i = 0; i < 10; i++) {
            //    list.Add("Item" + i.ToString());
            //}
            KeyDown += Navigator_KeyDown;
            KeyPress += Navigator_KeyPress;

        }

        

        //List<string> list = new List<string>();
        System.Windows.Forms.Timer Animator = new System.Windows.Forms.Timer();

        Type? listType = null;
        bool IsListLinked = false;
        private object? linkedList = null;
        [System.ComponentModel.Category("Data")]
        public object? LinkedList {
            get {
                return linkedList;
            }
            set {
                IsListLinked = false;
                if (value == null) {
                    linkedList = null;
                }
                else {
                    if (IsList(value) == true) {
                        linkedList = value;
                        listType = linkedList.GetType().GetGenericArguments()[0];
                        IsListLinked = true;
                    }
                }
                Invalidate();
            }
        }
        string displayText = "Name";
        [System.ComponentModel.Category("Data")]
        public string DisplayText {
            get { return displayText; }
            set {
                displayText = value;
                Invalidate();
            }
        }
        private bool IsList(object o) {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
        private Color arrowColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ArrowColor {
            get {
                return arrowColor;
            }
            set {
                arrowColor = value;
                Invalidate();
            }
        }
        private Color arrowMouseOverColor = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color ArrowMouseOverColor {
            get {
                return arrowMouseOverColor;
            }
            set {
                arrowMouseOverColor = value;
                Invalidate();
            }
        }
        Color midColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color MidColor {
            get { return midColor; }
            set {
                midColor = value;
                Invalidate();
            }
        }
        Color sideShadowColor = Color.FromArgb(20, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color SideShadowColor {
            get { return sideShadowColor; }
            set {
                sideShadowColor = value;
                Invalidate();
            }
        }
        Color shadowColor = Color.FromArgb(100, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color ShadowColor {
            get { return shadowColor; }
            set {
                shadowColor = value;
                Invalidate();
            }
        }
        Color selectedColor = Color.FromArgb(100, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color SelectedColor {
            get { return selectedColor; }
            set {
                selectedColor = value;
                Invalidate();
            }
        }
        Style displayStyle = Style.Right;
        [System.ComponentModel.Category("Appearance")]
        public Style DisplayStyle {
            get { return displayStyle; }
            set {
                displayStyle = value;
                Invalidate();
            }
        }
        bool showAnimations = true;
        [System.ComponentModel.Category("Control")]
        public bool ShowAnimations {
            get { return showAnimations; }
            set {
                showAnimations = value;
                Invalidate();
            }
        }
        int previousSelectedItem = 0;
        float selectedItemChnge = 0;
        float selectionStep = 0;
        int selectedItem = 0;
        [System.ComponentModel.Category("Control")]
        public int SelectedItem {
            get { return selectedItem; }
            set {
                previousSelectedItem = selectedItem;

                int TempVal = value;
                if (TempVal < 0) { TempVal = 0; }
                if (TempVal >= ItemCount) { TempVal = (int)ItemCount - 1; }
                selectedItem = TempVal;
                if (showAnimations == true) {
                    if (ItemCount > 0) {
                        selectionStep = (float)((int)TempVal - (int)previousSelectedItem) / 10.0f;
                        selectedItemChnge = previousSelectedItem;
                        if (previousSelectedItem < selectedItem) {
                            AnimateDirection = AnimationState.Down;
                            //if (CurrentItemChange <= (EndItem - 1)) {
                            Animator.Enabled = true;
                        }
                        else if (previousSelectedItem > selectedItem) {
                            AnimateDirection = AnimationState.Up;
                            Animator.Enabled = true;
                        }
                        else {
                            AnimateDirection = AnimationState.None;
                        }
                    }
                    //selectedItemChnge = 0.0f;
                }
                else {
                    selectedItemChnge = (float)TempVal;
                }

                Invalidate();
                if (previousSelectedItem != TempVal) {
                    SelectedIndexChanged?.Invoke(this, TempVal);
                }
            }
        }
        int UnitSize = 10;

        int ItemHeight = 10;
        int ShadowWidth = 3;
        int MaxWindItems = 10;
        float ItemsStart = 0;
        float ItemsEnd = 0;
        int StartItem = 0;
        int EndItem = 10;

        int MaxItems = 10;

        int ItemCount = 0;
        protected override void OnPaint(PaintEventArgs e) {
            UnitSize = (int)e.Graphics.MeasureString("W", Font).Height;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            ItemHeight = (int)((float)UnitSize * 2f);
            ShadowWidth = UnitSize / 2;
            MaxWindItems = (Height / ItemHeight) - 2;
            ItemsStart = ItemHeight;// (Height - (MaxWindItems * ItemHeight)) / 2;
            ItemsEnd = ItemsStart + (MaxWindItems * ItemHeight);
            if (linkedList == null) { ItemCount = 0; }
            else { ItemCount = GetItemCount(linkedList); }

            EndItem = StartItem + MaxWindItems;
            DetermineOverflow();
            DrawSideShadow(e);
            float SelectedItemPos = (float)ItemsStart + ((selectedItemChnge - (float)StartItem) * (float)ItemHeight);//(float)ItemsStart + (selectedItemChnge * (float)ItemHeight);
            SelectedItemPos = (float)Math.Round((double)SelectedItemPos, 4);
            DrawSelectedBottom(e, SelectedItemPos);
            DrawItems(e);
            DrawSelectedTop(e, SelectedItemPos);
            DrawOverflowArrows(e);
            //DrawSelectedBottom(e, 10);
            //DrawSelectedTop(e, 10);
        }
        private void DrawSideShadow(PaintEventArgs e) {
            if (displayStyle == Style.Right) {
                Rectangle VSHAD = new Rectangle(Width - ShadowWidth, 0, ShadowWidth, Height);
                Rectangle VSHADD = new Rectangle(Width - ShadowWidth - 2, 0, ShadowWidth + 2, Height);
                using (LinearGradientBrush linbr1 = new LinearGradientBrush(VSHADD, Color.FromArgb(0, 0, 0, 0), sideShadowColor, 0.0f)) {
                    e.Graphics.FillRectangle(linbr1, VSHAD);
                }
            }
            else {
                Rectangle VSHAD = new Rectangle(0, 0, ShadowWidth, Height);
                Rectangle VSHADD = new Rectangle(0, 0, ShadowWidth + 2, Height);
                using (LinearGradientBrush linbr1 = new LinearGradientBrush(VSHADD, Color.FromArgb(0, 0, 0, 0), sideShadowColor, 180.0f)) {
                    e.Graphics.FillRectangle(linbr1, VSHAD);
                }
            }
        }
        private void DrawItems(PaintEventArgs e) {
            if (ItemCount <= 0) { return; }
            int PadItem = 0;
            if (displayStyle == Style.Left) {
                PadItem = (int)(ItemHeight / 2.0f) + (UnitSize / 4);
            }
            else {
                PadItem = (UnitSize / 4);
            }
            using (StringFormat TextFrmt = new StringFormat()) {
                TextFrmt.LineAlignment = StringAlignment.Center;
                for (int i = 0; i < MaxWindItems; i++) {
                    float ItemVertPos = ItemsStart + (i * ItemHeight);
                    int CurrentItem = StartItem + i;
                    if ((CurrentItem >= 0) && (CurrentItem < ItemCount)) {//linkedList.Count
                        string DisplayText = GetItemValue(CurrentItem);// linkedList[CurrentItem];
                        using (SolidBrush TextBr = new SolidBrush(ForeColor)) {
                            if (displayStyle == Style.Left) {
                                RectangleF TextPos = new RectangleF(PadItem, ItemVertPos, Width - PadItem, ItemHeight);
                                e.Graphics.DrawString(DisplayText, Font, TextBr, TextPos, TextFrmt);
                            }
                            else {
                                RectangleF TextPos = new RectangleF(PadItem, ItemVertPos, Width - PadItem, ItemHeight);
                                e.Graphics.DrawString(DisplayText, Font, TextBr, TextPos, TextFrmt);
                            }
                        }
                    }
                }
            }
        }
        bool OverflowTop = false;
        bool OverflowBottom = false;
        Rectangle OverflowTopRectangle = Rectangle.Empty;
        Rectangle OverflowBottomRectangle = Rectangle.Empty;
        private void DrawOverflowArrows(PaintEventArgs e) {
            OverflowTopRectangle = new Rectangle(0, 0, Width, ItemHeight);
            OverflowBottomRectangle = new Rectangle(0, Height - ItemHeight, Width, ItemHeight);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (ItemCount <= 0) { return; }
            if (OverflowTop == true) {
                DrawArrow(e, OverflowTopRectangle, true);
            }
            if (OverflowBottom == true) {
                DrawArrow(e, OverflowBottomRectangle, false);
            }
        }
        private void DrawArrow(PaintEventArgs e, Rectangle ArrowBounds, bool TopArrow) {
            Size ButtonSize = new Size((int)UnitSize, (int)UnitSize);
            Color collapseArrowColor = arrowColor;
            int CentreX = ArrowBounds.X + ((ArrowBounds.Width - ButtonSize.Width) / 2);
            int CentreY = ArrowBounds.Y + ((ArrowBounds.Height - ButtonSize.Height) / 2);
            Rectangle CentreArrow = new Rectangle(CentreX, CentreY, ButtonSize.Width, ButtonSize.Height);
            using (SolidBrush ActionBrush = new SolidBrush(collapseArrowColor)) {
                using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                    Point[] Points;
                    ArrowPoints(CentreArrow, out Points, TopArrow);
                    e.Graphics.DrawLines(ActionPen, Points);
                }
            }
        }
        private void ArrowPoints(Rectangle CollapseMarker, out Point[] Points, bool TopArrow) {
            int HalfHeight = CollapseMarker.Height / 2;
            if (TopArrow == false) {
                Points = new Point[]{
                         new Point(CollapseMarker.Left, CollapseMarker.Top),
                         new Point(CollapseMarker.Left + CollapseMarker.Width/2, CollapseMarker.Top + HalfHeight),
                         new Point(CollapseMarker.Right, CollapseMarker.Top)};
            }
            else {
                Points = new Point[]{
                         new Point(CollapseMarker.Left, CollapseMarker.Top+ HalfHeight),
                         new Point(CollapseMarker.Left + CollapseMarker.Width/2, CollapseMarker.Top),
                         new Point(CollapseMarker.Right, CollapseMarker.Top+ HalfHeight)};
            }
        }
        private void DetermineOverflow() {
            if (ItemCount <= 0) {
                OverflowTop = false;
                OverflowBottom = false;
                return;
            }
            if (StartItem > 0) { OverflowTop = true; }
            else { OverflowTop = false; }
            if (ItemCount > EndItem) { OverflowBottom = true; }
            else { OverflowBottom = false; }

        }
        //private int GetItemCount(List<object> list) {
        //    if (list is ICollection) {
        //        return list.Count();
        //    }
        //    return 0;
        //}
        private int GetItemCount(object AnyObject) {
            if (AnyObject == null) { return 0; }
            IEnumerable? list = AnyObject as IEnumerable;
            if (list != null) {
                return list.Cast<object>().Count();
                //foreach (var item in list) {
                //    // Do whathever you want.
                //}
            }
            return 0;
        }
        private string GetItemValue(int Index) {
            if (linkedList == null) { return ""; }
            IEnumerable? list = linkedList as IEnumerable;
            if (list != null) {
                //if (linkedList != null) {
                if (listType != null) {
                    var Data = list.Cast<object>().ToList();
                    if (Data[Index] != null) {
                        object Item = Data[Index];
                        if (Item.GetType() == listType) {
                            PropertyInfo? Val = Item.GetType().GetProperty(displayText);
                            if (Val != null) {
                                object? SubVal = Val.GetValue(Item, null);
                                if (SubVal != null) {
                                    return SubVal.ToString() ?? "";
                                }
                            }

                        }
                    }
                }
            }
            return "";
        }
        private void DrawSelectedTop(PaintEventArgs e, float Y) {
            int CurrentSelectedItem = GetCurrentSelectedItem();
            if ((CurrentSelectedItem >= StartItem) && (CurrentSelectedItem < EndItem)) { }
            else { return; }
            float ShadowWidthF = (float)ShadowWidth;
            RectangleF NSHAD = new RectangleF(0, Y, Width, ShadowWidthF);
            RectangleF SSHAD = new RectangleF(0, Y + ItemHeight - ShadowWidthF, Width, ShadowWidth + 2);
            RectangleF NSHADD = new RectangleF(0, Y, Width, ShadowWidthF);
            RectangleF SSHADD = new RectangleF(0, Y + ItemHeight - ShadowWidthF, Width, ShadowWidth + 2);
            using (LinearGradientBrush lingrad1 = new LinearGradientBrush(NSHADD, shadowColor, Color.FromArgb(0, 0, 0, 0), 90.0f)) {
                e.Graphics.FillRectangle(lingrad1, NSHAD);
            }
            using (LinearGradientBrush lingrad2 = new LinearGradientBrush(SSHADD, Color.FromArgb(0, 0, 0, 0), shadowColor, 90.0f)) {
                e.Graphics.FillRectangle(lingrad2, SSHAD);
            }
            if (displayStyle == Style.Right) {
                PointF point1 = new PointF(this.Width, Y);
                PointF point2 = new PointF(Width - (int)(ItemHeight / 2.0f), Convert.ToSingle(Y + ItemHeight / 2d));
                PointF point3 = new PointF(this.Width, Y + ItemHeight);
                PointF[] curvePoints = new[] { point1, point2, point3 };
                using (SolidBrush Plotfill = new SolidBrush(midColor)) {
                    e.Graphics.FillPolygon(Plotfill, curvePoints);
                }
            }
            else {
                PointF point1 = new PointF(-1f, Y);
                PointF point2 = new PointF((int)(ItemHeight / 2.0f), Convert.ToSingle(Y + ItemHeight / 2d));
                PointF point3 = new PointF(-1f, Y + ItemHeight);
                PointF[] curvePoints = new[] { point1, point2, point3 };
                using (SolidBrush Plotfill = new SolidBrush(midColor)) {
                    e.Graphics.FillPolygon(Plotfill, curvePoints);
                }
            }
        }
        private void DrawSelectedBottom(PaintEventArgs e, float Y) {
            int CurrentSelectedItem = GetCurrentSelectedItem();
            if ((CurrentSelectedItem >= StartItem) && (CurrentSelectedItem < EndItem)) { }
            else { return; }
            RectangleF ItemBox = new RectangleF(0, Y, Width, ItemHeight);
            using (SolidBrush Plotfill = new SolidBrush(selectedColor)) {
                e.Graphics.FillRectangle(Plotfill, ItemBox);
            }
        }
        private int GetCurrentSelectedItem() {
            return (int)Math.Floor((float)Math.Round(selectedItemChnge,4));
        }

        protected override void OnResize(EventArgs e) {
            if ((EndItem) > ItemCount) {
                if (ItemCount > 0) {
                    if (StartItem > 0) {
                        StartItem--;
                    }
                }
            }
            Invalidate();
            base.OnResize(e);
        }
        private int GetSelectedItem(Point Pos) {
            //y = ItemsStart + (i * ItemHeight);
            return (int)Math.Floor(((double)Pos.Y - (double)ItemsStart) / (double)ItemHeight) + StartItem;
        }

        protected override void OnMouseClick(MouseEventArgs e) {

            if (e.Location.Y < ItemsStart) {
                if (OverflowTop == true) {
                    if (StartItem > 0) {
                        StartItem--;
                        Invalidate();
                    }
                }
            }
            else if (e.Location.Y >= ItemsEnd) {
                if (OverflowBottom == true) {
                    if (EndItem < ItemCount) {
                        StartItem++;
                        Invalidate();
                    }
                }
            }
            else {
                SelectedItem = GetSelectedItem(e.Location);
            }
            base.OnMouseClick(e);
        }
        AnimationState AnimateDirection = AnimationState.None;
        private void Animator_Tick(object? sender, EventArgs e) {
            int CurrentItemChange = (int)Math.Round((float)selectedItemChnge, 2);
            if (AnimateDirection == AnimationState.Down) {
                if (selectedItem <= CurrentItemChange) {
                    EndAnimation();
                }
                else if (CurrentItemChange >= (EndItem - 1)) {
                    EndAnimation();
                }
                else {
                    selectedItemChnge += selectionStep;
                }
            }
            else if (AnimateDirection == AnimationState.Up) {
                decimal CurrentItemChanged = (decimal)Math.Round((float)selectedItemChnge, 2);
                if (selectedItem >= CurrentItemChanged) {
                    EndAnimation();
                }
                else {
                    selectedItemChnge += selectionStep;
                }
            }
            else {
                Animator.Enabled = false;
                selectedItem = (int)selectedItemChnge;
                previousSelectedItem = selectedItem;
            }
            Invalidate();
        }
        private void EndAnimation() {
            Animator.Enabled = false;
            //CurrentItemChange = selectedItem;
            previousSelectedItem = selectedItem;
            selectedItemChnge = selectedItem;
            AnimateDirection = AnimationState.None;
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            Reselect(e.Delta);
        }
        private void Navigator_KeyDown(object? sender, KeyEventArgs e) {
        }
        private void Navigator_KeyPress(object? sender, KeyPressEventArgs e) {
           
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Down) {
                Reselect(-1);
                return true;
            }
            if (keyData == Keys.Up) {
                Reselect(1);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void Reselect(int Delta) {
            if (Delta < 0) {
                SelectedItem++;
                if (EndItem <= ItemCount) {
                    if (SelectedItem >= EndItem) {
                        StartItem++;
                        Invalidate();
                    }
                }
            }
            else {
                
                if (StartItem > 0) {
                    //Debug.Print(SelectedItem.ToString() + " " + StartItem.ToString());
                    if (SelectedItem <= StartItem) {
                        StartItem--;
                        Invalidate();
                    }
                }
                SelectedItem--;
            }
        }

        private enum AnimationState {
            None = 0x00,
            Down = 0x01,
            Up = 0x02
        }
        public enum Style {
            Left = 0x01,
            Right = 0x02
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // Navigator
            // 
            this.Name = "Navigator";
            this.Load += new System.EventHandler(this.Navigator_Load);
            this.ResumeLayout(false);

        }

        private void Navigator_Load(object sender, EventArgs e) {

        }
    }
}
