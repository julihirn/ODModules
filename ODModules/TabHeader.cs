using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics;

namespace ODModules {
    public class TabHeader : UserControl {
        [Category("Button Actions")]
        public event AddButtonClickedHandler? AddButtonClicked;
        public delegate void AddButtonClickedHandler(object sender);
        [Category("Button Actions")]
        public event CloseButtonClickedHandler? CloseButtonClicked;
        public delegate void CloseButtonClickedHandler(object sender, int Index);

        public event SelectedIndexChangedHandler? SelectedIndexChanged;
        public delegate void SelectedIndexChangedHandler(object sender, int CurrentIndex, int PreviousIndex);
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
        private List<Tab> tabs = new List<Tab>();
        [System.ComponentModel.Category("Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<Tab> Tabs {
            get {
                return tabs;
            }
        }
        private bool useBindingTabControl = true;
        [System.ComponentModel.Category("Control")]
        public bool UseBindingTabControl {
            get {
                return useBindingTabControl;
            }
            set {
                useBindingTabControl = value;
                Invalidate();
            }
        }
        int selectedIndex = 0;
        [System.ComponentModel.Category("Control")]
        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                int previousindex = selectedIndex;
                selectedIndex = value;
                if (useBindingTabControl == true) {
                    if (bindedTabControl != null) {
                        if (selectedIndex > bindedTabControl.TabPages.Count) {
                            selectedIndex = bindedTabControl.TabPages.Count - 1;
                        }
                    }
                }
                else {
                    if (value >= tabs.Count - 1) {
                        selectedIndex = tabs.Count - 1;
                    }
                    else if (value <= 0) {
                        selectedIndex = 0;
                    }
                }
                SelectedIndexChanged?.Invoke(this, selectedIndex, previousindex);
                Invalidate();
            }
        }
        TabStyles tabStyle = TabStyles.Normal;
        [System.ComponentModel.Category("Appearance")]
        public TabStyles TabStyle {
            get {
                return tabStyle;
            }
            set {
                tabStyle = value;
            }
        }
        Color tabBackColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabBackColor {
            get {
                return tabBackColor;
            }
            set {
                tabBackColor = value;
            }
        }
        Color tabSelectedBorderColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabSelectedBorderColor {
            get {
                return tabSelectedBorderColor;
            }
            set {
                tabSelectedBorderColor = value;
            }
        }
        Color tabSelectedForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color TabSelectedForeColor {
            get {
                return tabSelectedForeColor;
            }
            set {
                tabSelectedForeColor = value;
            }
        }
        Color tabBorderColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabBorderColor {
            get {
                return tabBorderColor;
            }
            set {
                tabBorderColor = value;
            }
        }
        Color tabSelectedBackColor = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabSelectedBackColor {
            get {
                return tabSelectedBackColor;
            }
            set {
                tabSelectedBackColor = value;
            }
        }
        Color tabHoverBackColor = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabHoverBackColor {
            get {
                return tabHoverBackColor;
            }
            set {
                tabHoverBackColor = value;
            }
        }
        Color tabClickedBackColor = Color.DarkGray;
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
        private Color arrowHoverColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ArrowHoverColor {
            get {
                return arrowHoverColor;
            }
            set {
                arrowHoverColor = value;
                Invalidate();
            }
        }
        private Color arrowDisabledColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ArrowDisabledColor {
            get {
                return arrowDisabledColor;
            }
            set {
                arrowDisabledColor = value;
                Invalidate();
            }
        }
        private Color closeHoverColor = Color.Red;
        [System.ComponentModel.Category("Appearance")]
        public Color CloseHoverColor {
            get {
                return closeHoverColor;
            }
            set {
                closeHoverColor = value;
                Invalidate();
            }
        }
        private bool showAddButton = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowAddButton {
            get {
                return showAddButton;
            }
            set {
                showAddButton = value;
                Invalidate();
            }
        }
        public void ClearTabs() {
            for(int i = tabs.Count - 1; i >= 0; i--) {
                tabs[i].Tag = null;
                tabs.RemoveAt(i);
            }
        }
        public TabHeader() {
            DoubleBuffered = true;
            MouseWheel += TabHeader_MouseWheel;
            Resize += TabHeader_Resize;
            MouseClick += TabHeader_MouseClick;
            MouseMove += TabHeader_MouseMove;
            MouseLeave += TabHeader_MouseLeave;
            LostFocus += TabHeader_LostFocus;
            MouseHover += TabHeader_MouseHover;
        }

        private void TabHeader_MouseHover(object? sender, EventArgs e) {
            Invalidate();
        }

        private void TabHeader_LostFocus(object? sender, EventArgs e) {
            MousePosition = new Point(-1, -1);
            HoverButton = -1;
            Invalidate();
        }

        private void TabHeader_MouseLeave(object? sender, EventArgs e) {
            MousePosition = new Point(-1, -1);
            HoverButton = -1;
            Invalidate();
        }
        int HoverButton = -1;
        private void TabHeader_MouseMove(object? sender, MouseEventArgs e) {
            MousePosition = e.Location;
            int X = GetButton(e.Location);
            HoverButton = X;
            Invalidate();
        }
        private int GetButton(Point Pnt) {
            if (Pnt.Y >= Height - StandardButtonSize.Height) {
                int Temp = (Pnt.X - Padding.Left) / StandardButtonSize.Width;
                int CurrentButton = Temp + TabStart;
                if (OverflowButtonsVisible == true) {
                    if ((CurrentButton >= TabStart) && (CurrentButton < TabCount) && (CurrentButton < TabStart + MaximumTabs)) {
                        return CurrentButton;
                    }
                }
                else {
                    if ((CurrentButton >= TabStart) && (CurrentButton < TabCount)) {
                        return CurrentButton;
                    }
                }
                return -1;
            }
            else {
                return -1;
            }
        }
        Point MousePosition = new Point(-1, -1);
        private void TabHeader_MouseClick(object? sender, MouseEventArgs e) {
            int X = GetButton(e.Location);
            if (X == -1) {
                if (OverflowButtonsVisible == true) {
                    if (LeftOffsetRectangle.Contains(e.Location)) {
                        if (0 < TabStart) {
                            TabStart--;
                        }
                    }
                    if (RightOffsetRectangle.Contains(e.Location)) {
                        if (TabCount - 1 > TabStart + MaximumTabs - 1) {
                            TabStart++;
                        }
                    }
                }
                if (showAddButton == true) {
                    if (AddButtonOffsetRectangle.Contains(e.Location)) {
                        AddButtonClicked?.Invoke(this);
                    }
                }
            }
            else {
                if (IsOnCloseButton(X, e.Location) == true) {
                    CloseButtonClicked?.Invoke(this, X);
                }
                else {
                    SelectedIndex = X;
                }
            }
            Invalidate();
        }
        private bool IsOnCloseButton(int TabIndex, Point MPosition) {
            int RunningSize = Padding.Left;
            int ZeroedIndex = TabIndex - TabStart;
            RunningSize += StandardButtonSize.Width * ZeroedIndex;
            Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), StandardButtonSize);
            int CrossSize = GenericTextHeight / 2;
            int CentreCross = (TabRectangle.Height - CrossSize) / 2;
            Rectangle CloseMarker = new Rectangle(TabRectangle.X + TabRectangle.Width - TextPadding, TabRectangle.Y + CentreCross, CrossSize, CrossSize);
            return CloseMarker.Contains(MPosition);
        }
        private void TabHeader_Resize(object? sender, EventArgs e) {
            Invalidate();
        }

        private void TabHeader_MouseWheel(object? sender, MouseEventArgs e) {
            HoverButton = -1;
            if (e.Delta > 0) {
                SelectedIndex--;
                if (OverflowButtonsVisible == true) {
                    if (SelectedIndex < TabStart) {
                        TabStart--;
                    }
                }
            }
            else {
                SelectedIndex++;
                if (OverflowButtonsVisible == true) {
                    if (SelectedIndex > TabStart + MaximumTabs - 1) {
                        TabStart++;
                    }
                }
            }
            Invalidate();
        }

        [System.ComponentModel.Category("Appearance")]
        public Color TabClickedBackColor {
            get {
                return tabClickedBackColor;
            }
            set {
                tabClickedBackColor = value;
            }
        }
        private void BindedTabControl_ControlRemoved(object? sender, ControlEventArgs e) {
            if (bindedTabControl != null) {
                //if (selectedIndex >= bindedTabControl.TabPages.Count - 1) {
                //    if (Old_Selected < 0) {
                //        Old_Selected = selected;
                //    }
                //    selected = bindedTabControl.TabPages.Count - 1;
                //    ReDraw();
                //}
            }
            Invalidate();
        }
        private void BindedTabControl_ControlAdded(object? sender, ControlEventArgs e) {
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
        }
        int TabStart = 0;
        SizeF UnitSize;
        Size StandardButtonSize = new Size(10, 10);
        int GenericTextHeight = 10;
        int TextPadding = 10;
        int MaximumTabs = 10;
        int TabCount = 10;
        bool OverflowButtonsVisible = false;
        int MaximumOverflow = 0;
        int TabPadding = 10;
        private void RenderSetup(PaintEventArgs e) {
            using (Font UnitFont = new Font(this.Font.FontFamily, 6.0f)) {
                UnitSize = e.Graphics.MeasureString("W", UnitFont);
            }
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            TextPadding = (int)e.Graphics.MeasureString("W", Font).Width;
            GenericTextHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            StandardButtonSize = new Size(GenericTextHeight * 8, (int)(GenericTextHeight * 1.25f));
            int AddButtonOffset = (int)(UnitSize.Height * 1.8);
            if (showAddButton == true) {
                MaximumTabs = (int)Math.Floor((((float)(Width - Padding.Left - Padding.Right - AddButtonOffset)) / (float)StandardButtonSize.Width));
            }
            else {
                MaximumTabs = (int)Math.Floor((((float)(Width - Padding.Left - Padding.Right)) / (float)StandardButtonSize.Width));
            }
            TabPadding = Height - StandardButtonSize.Height;
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    TabCount = bindedTabControl.TabPages.Count;
                    if (bindedTabControl.TabPages.Count > MaximumTabs) {
                        OverflowButtonsVisible = true;
                        MaximumOverflow = bindedTabControl.TabPages.Count - MaximumTabs;
                    }
                    else {
                        OverflowButtonsVisible = false;
                        MaximumOverflow = 0;
                    }
                }
                else { OverflowButtonsVisible = false; MaximumOverflow = 0; TabCount = 0; }
            }
            else {
                TabCount = tabs.Count;
                if (tabs.Count > MaximumTabs) {
                    OverflowButtonsVisible = true;
                    MaximumOverflow = tabs.Count - MaximumTabs;
                }
                else {
                    OverflowButtonsVisible = false;
                    MaximumOverflow = 0;
                }
            }
            if (OverflowButtonsVisible == false) {
                TabStart = 0;
            }
            else {
                if (showAddButton == true) {
                    MaximumTabs = (int)Math.Floor((((float)(Width - Padding.Left - Padding.Right - (UnitSize.Height * 5.8))) / (float)StandardButtonSize.Width));
                }
                else {
                    MaximumTabs = (int)Math.Floor((((float)(Width - Padding.Left - Padding.Right - (UnitSize.Height * 3.8))) / (float)StandardButtonSize.Width));
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            int RunningSize = Padding.Left;
            RenderSetup(e);
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                        if (i < bindedTabControl.TabPages.Count) {
                            Action Current = Action.Normal;
                            if (i == bindedTabControl.SelectedIndex) {
                                Current = Action.Selected;
                            }
                            Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), StandardButtonSize);
                            RunningSize += DrawTab(e, bindedTabControl.TabPages[i].Text, Current, TabRectangle, false);
                        }
                    }
                }
            }
            else {
                for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                    if (i < tabs.Count) {
                        Action Current = Action.Normal;
                        if (i == selectedIndex) {
                            Current = Action.Selected;
                        }
                        else if (i == HoverButton) {
                            Current = Action.Selected;
                        }
                        Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), StandardButtonSize);
                        RunningSize += DrawTab(e, tabs[i].Text, Current, TabRectangle, true);
                    }
                }
            }
            if (tabStyle == TabStyles.Normal) {
                using (SolidBrush Br = new SolidBrush(TabBorderColor)) {
                    using (Pen Pn = new Pen(Br)) {
                        e.Graphics.DrawLine(Pn, 0, Height - 1, Width, Height - 1);
                    }
                }
            }
            DrawOverflow(e);
        }
        Rectangle LeftOffsetRectangle;
        Rectangle RightOffsetRectangle;
        Rectangle AddButtonOffsetRectangle;
        private void DrawOverflow(PaintEventArgs e) {
            Size ButtonSize = new Size((int)UnitSize.Height, (int)UnitSize.Height);
            int TabPadding = Height - StandardButtonSize.Height + ((StandardButtonSize.Height - ButtonSize.Height) / 2);
            float BtnPadding = 3.8f;
            if (showAddButton == true) {
                if (OverflowButtonsVisible == true) {
                    BtnPadding = 5.8f;
                }
                else { BtnPadding = 1.8f; }
            }
            Point CollaspeButtonPosition = new Point(Width - (int)(UnitSize.Width * BtnPadding) - Padding.Right, TabPadding);
            if (showAddButton == true) {
                AddButtonOffsetRectangle = new Rectangle(CollaspeButtonPosition, ButtonSize);
                Color AddButtonArrow = arrowColor;
                using (SolidBrush ActionBrush = new SolidBrush(AddButtonArrow)) {
                    using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                        Point[] Points;
                        ButtonPoints(AddButtonOffsetRectangle, false, out Points);
                        e.Graphics.DrawLine(ActionPen, Points[0], Points[1]);
                        e.Graphics.DrawLine(ActionPen, Points[2], Points[3]);
                    }
                }
            }
            if (OverflowButtonsVisible == false) { return; }
            if (showAddButton == true) {
                CollaspeButtonPosition = new Point(CollaspeButtonPosition.X + (ButtonSize.Width * 2), CollaspeButtonPosition.Y);
            }
            LeftOffsetRectangle = new Rectangle(CollaspeButtonPosition, ButtonSize);
            Color TabLeftArrow = arrowColor;
            if (TabStart == 0) {
                TabLeftArrow = arrowDisabledColor;
            }
            else {
                if (LeftOffsetRectangle.Contains(MousePosition)) {
                    TabLeftArrow = arrowHoverColor;
                }
            }
            using (SolidBrush ActionBrush = new SolidBrush(TabLeftArrow)) {
                using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                    Point[] Points;
                    ArrowPoints(LeftOffsetRectangle, false, out Points);
                    e.Graphics.DrawLines(ActionPen, Points);
                }
            }
            Color TabRightArrow = arrowColor;
            RightOffsetRectangle = new Rectangle(new Point(CollaspeButtonPosition.X + ButtonSize.Width * 2, CollaspeButtonPosition.Y), ButtonSize);
            if (TabStart + MaximumTabs >= TabCount) {
                TabRightArrow = arrowDisabledColor;
            }
            else {
                if (RightOffsetRectangle.Contains(MousePosition)) {
                    TabRightArrow = arrowHoverColor;
                }
            }
            using (SolidBrush ActionBrush = new SolidBrush(TabRightArrow)) {
                using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                    Point[] Points;
                    ArrowPoints(RightOffsetRectangle, true, out Points);
                    e.Graphics.DrawLines(ActionPen, Points);
                }
            }

        }
        private int DrawTab(PaintEventArgs e, string TabText, Action DisplayAction, Rectangle TabRectangle, bool HasCloseButton) {
            int CloseButtonPadding = 0;
            if (HasCloseButton == true) {
                CloseButtonPadding = TextPadding;
            }
            int CentreText = (TabRectangle.Height - GenericTextHeight) / 2;

            //Rectangle TabRectangle = new Rectangle(Location.X, Location.Y, (int)TextSize.Width + (TabPadding * 2), (int)TextSize.Height + (TabPadding / 4));
            Rectangle TextRectangle = new Rectangle(TabRectangle.X + (TextPadding / 2), TabRectangle.Y + CentreText, TabRectangle.Width - (TextPadding) - CloseButtonPadding, GenericTextHeight);
            if (tabStyle == TabStyles.Normal) {
                Color backColor = GetTabColor(DisplayAction);
                Color borderColor = GetTabColor(DisplayAction, true);
                using (SolidBrush backBrush = new SolidBrush(backColor)) {
                    e.Graphics.FillRectangle(backBrush, TabRectangle);
                }
                using (SolidBrush borderBrush = new SolidBrush(borderColor)) {
                    using (Pen borderPen = new Pen(borderBrush)) {
                        e.Graphics.DrawLines(borderPen, new Point[] {
                            new Point(TabRectangle.Left, TabRectangle.Bottom),
                            new Point(TabRectangle.Left, TabRectangle.Top),
                            new Point(TabRectangle.Right, TabRectangle.Top),
                            new Point(TabRectangle.Right, TabRectangle.Bottom)
                        });
                    }
                }
            }
            //else if (tabStyle == TabStyles.Angled) {
            //    Color backColor = GetTabColor(DisplayAction);
            //    Color borderColor = GetTabColor(DisplayAction, true);
            //    int AngleInset = TabPadding / 4;
            //    using (SolidBrush backBrush = new SolidBrush(backColor)) {
            //        
            //        e.Graphics.FillPolygon(backBrush, new Point[] {
            //            new Point(TabRectangle.Left + AngleInset, TabRectangle.Top),
            //            new Point(TabRectangle.Right - AngleInset, TabRectangle.Top),
            //            new Point(TabRectangle.Right, TabRectangle.Bottom),
            //            new Point(TabRectangle.Left, TabRectangle.Bottom),
            //        });
            //    }
            //    using (SolidBrush borderBrush = new SolidBrush(borderColor)) {
            //        using (Pen borderPen = new Pen(borderBrush)) {
            //            e.Graphics.DrawLines(borderPen, new Point[] {
            //                new Point(TabRectangle.Left, TabRectangle.Bottom),
            //                new Point(TabRectangle.Left + AngleInset, TabRectangle.Top),
            //                new Point(TabRectangle.Right- AngleInset, TabRectangle.Top),
            //                new Point(TabRectangle.Right, TabRectangle.Bottom)
            //            });
            //        }
            //    }
            //}
            else if (tabStyle == TabStyles.Underlined) {

            }
            if (HasCloseButton == true) {
                int CrossSize = GenericTextHeight / 2;
                int CentreCross = (TabRectangle.Height - CrossSize) / 2;
                Rectangle CloseMarker = new Rectangle(TabRectangle.X + TabRectangle.Width - CloseButtonPadding, TabRectangle.Y + CentreCross, CrossSize, CrossSize);
                Color CloseColor = Color.White;
                if (CloseMarker.Contains(MousePosition)) {
                    CloseColor = closeHoverColor;
                }
                using (SolidBrush ActionBrush = new SolidBrush(CloseColor)) {
                    using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y + CloseMarker.Height));
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y + CloseMarker.Height), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y));
                    }
                }
            }
            Color TabForeColor = ForeColor;
            if (DisplayAction == Action.Selected) { TabForeColor = tabSelectedForeColor; }
            using (SolidBrush textBrush = new SolidBrush(TabForeColor)) {
                using (StringFormat textFormat = new StringFormat(StringFormat.GenericTypographic)) {
                    textFormat.Trimming = StringTrimming.EllipsisCharacter;
                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString(TabText, Font, textBrush, TextRectangle, textFormat);
                }
            }
            return TabRectangle.Width;
        }
        private Color GetTabColor(Action DisplayAction, bool IsBorder = false) {
            if (DisplayAction == Action.Normal) {
                if (IsBorder == false) {
                    return tabBackColor;
                }
                else {
                    return tabBorderColor;
                }
            }
            else if (DisplayAction == Action.Selected) {
                if (IsBorder == false) {
                    return tabSelectedBackColor;
                }
                else {
                    return tabSelectedBorderColor;
                }
            }
            else if (DisplayAction == Action.MouseDown) {
                return tabClickedBackColor;
            }
            else if (DisplayAction == Action.MouseOver) {
                return tabHoverBackColor;
            }
            return tabBackColor;
        }
        private void ArrowPoints(Rectangle CollapseMarker, bool IsRight, out Point[] Points) {
            int HalfHeight = CollapseMarker.Height / 2;
            if (IsRight == true) {
                Points = new Point[]{
                         new Point(CollapseMarker.Left, CollapseMarker.Top),
                         new Point(CollapseMarker.Left + HalfHeight, CollapseMarker.Top + CollapseMarker.Width/2),
                         new Point(CollapseMarker.Left, CollapseMarker.Bottom)};
            }
            else {
                Points = new Point[]{
                         new Point(CollapseMarker.Left+ HalfHeight, CollapseMarker.Top),
                         new Point(CollapseMarker.Left , CollapseMarker.Top + CollapseMarker.Width/2),
                         new Point(CollapseMarker.Left+ HalfHeight, CollapseMarker.Bottom)};
            }
        }
        private void ButtonPoints(Rectangle CollapseMarker, bool IsRight, out Point[] Points) {
            int HalfLength = (int)((float)CollapseMarker.Width / 2.0f);
            int Length = CollapseMarker.Width;
            Points = new Point[]{
                         new Point(CollapseMarker.Left + HalfLength, CollapseMarker.Top),
                         new Point(CollapseMarker.Left + HalfLength, CollapseMarker.Top + Length -1),
                         new Point(CollapseMarker.Left, CollapseMarker.Top + HalfLength),
                         new Point(CollapseMarker.Left + Length, CollapseMarker.Top + HalfLength)};
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
        }
        private enum Action {
            Normal = 0x00,
            Selected = 0X01,
            MouseDown = 0x02,
            MouseClick = 0x04,
            MouseOver = 0x05
        }
        public enum TabStyles {
            Normal = 0x00,
            Angled = 0x01,
            Underlined = 0x02
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // TabHeaderControl
            // 
            this.Name = "TabHeaderControl";
            this.Size = new System.Drawing.Size(351, 56);
            this.Load += new System.EventHandler(this.TabHeaderControl_Load);
            this.ResumeLayout(false);

        }

        private void TabHeaderControl_Load(object sender, EventArgs e) {

        }
    }
    public class Tab {
        private object? tag = null;
        public object? Tag {
            get { return tag; }
            set { tag = value; }
        }
        private string text = "";
        public string Text {
            get { return text; }
            set { text = value; }
        }
        private bool selected = false;
        public bool Selected {
            get { return selected; }
            set { selected = value; }
        }
    }
}
