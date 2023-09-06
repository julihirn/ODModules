using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ObjectiveC;

namespace ODModules {
    public class TabHeader : UserControl {
        [Category("Button Actions")]
        public event AddButtonClickedHandler? AddButtonClicked;
        public delegate void AddButtonClickedHandler(object sender);
        [Category("Button Actions")]
        public event CloseButtonClickedHandler? CloseButtonClicked;
        public delegate void CloseButtonClickedHandler(object sender, int Index);
        [Category("Tab Actions")]
        public event TabClickedHandler? TabClicked;
        public delegate void TabClickedHandler(object sender, TabClickedEventArgs Tab);
        [Category("Tab Actions")]
        public event TabClickedHandler? TabRightClicked;

        public event SelectedIndexChangedHandler? SelectedIndexChanged;
        public delegate void SelectedIndexChangedHandler(object sender, int CurrentIndex, int PreviousIndex);

        public TabHeader() {
            DoubleBuffered = true;
            MouseWheel += TabHeader_MouseWheel;
            Resize += TabHeader_Resize;
            MouseClick += TabHeader_MouseClick;
            MouseMove += TabHeader_MouseMove;
            MouseLeave += TabHeader_MouseLeave;
            LostFocus += TabHeader_LostFocus;
            MouseHover += TabHeader_MouseHover;
            MouseDown += TabHeader_MouseDown;
            MouseUp += TabHeader_MouseUp;
        }
        #region Properties
        private bool allowDragReordering = true;
        [System.ComponentModel.Category("Control")]
        public bool AllowDragReordering {
            get {
                return allowDragReordering;
            }
            set {
                allowDragReordering = value;
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
                if (useBindingTabControl == true) {
                    if (bindedTabControl != null) {
                        return bindedTabControl.SelectedIndex;
                    }
                    return selectedIndex;
                }
                else { return selectedIndex; }
            }
            set {
                int previousindex = selectedIndex;
                selectedIndex = value;
                if (useBindingTabControl == true) {
                    if (bindedTabControl != null) {
                        if (bindedTabControl.TabPages.Count > 0) {
                            if (selectedIndex > bindedTabControl.TabPages.Count) {
                                selectedIndex = bindedTabControl.TabPages.Count - 1;
                            }
                            else if (selectedIndex < 0) {
                                selectedIndex = 0;
                            }
                        }
                        else { selectedIndex = 0; }
                        bindedTabControl.SelectedIndex = selectedIndex;
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
        bool allowTabResize = true;
        [System.ComponentModel.Category("Appearance")]
        public bool AllowTabResize {
            get {
                return allowTabResize;
            }
            set {
                allowTabResize = value;
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
                Invalidate();
            }
        }
        bool showTabDividers = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowTabDividers {
            get {
                return showTabDividers;
            }
            set {
                showTabDividers = value;
                Invalidate();
            }
        }
        Color tabDividerColor = Color.Gray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabDividerColor {
            get {
                return tabDividerColor;
            }
            set {
                tabDividerColor = value;
                Invalidate();
            }
        }
        Color tabSelectedShadowColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color TabSelectedShadowColor {
            get {
                return tabSelectedShadowColor;
            }
            set {
                tabSelectedShadowColor = value;
                Invalidate();
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
                Invalidate();
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
                Invalidate();
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
                Invalidate();
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
                Invalidate();
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
                Invalidate();
            }
        }
        Color tabRuleColor = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color TabRuleColor {
            get {
                return tabRuleColor;
            }
            set {
                tabRuleColor = value;
                Invalidate();
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
                Invalidate();
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
        private Color addHoverColor = Color.LimeGreen;
        [System.ComponentModel.Category("Appearance")]
        public Color AddHoverColor {
            get {
                return addHoverColor;
            }
            set {
                addHoverColor = value;
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
        [System.ComponentModel.Category("Appearance")]
        public Color TabClickedBackColor {
            get {
                return tabClickedBackColor;
            }
            set {
                tabClickedBackColor = value;
            }
        }

        #endregion
        public void ClearTabs() {
            for (int i = tabs.Count - 1; i >= 0; i--) {
                tabs[i].Tag = null;
                tabs.RemoveAt(i);
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
        #region Drawing Setup
        int TabStart = 0;
        SizeF UnitSize;
        Size CurrentButtonSize = new Size(10, 10);
        Size StandardButtonSize = new Size(10, 10);
        Size SmallButtonSize = new Size(10, 10);
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
            TabCount = GetTabCount();
            TextPadding = (int)e.Graphics.MeasureString("W", Font).Width;
            GenericTextHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            StandardButtonSize = new Size(GenericTextHeight * 8, (int)(GenericTextHeight * 1.25f));
            SmallButtonSize = new Size(GenericTextHeight * 4, StandardButtonSize.Height);
            if (allowTabResize == false) {
                CurrentButtonSize = StandardButtonSize;
            }
            else {
                int BarWidth = GetTabSpaceWidth(false);
                int LongLength = TabCount * StandardButtonSize.Width;
                if (LongLength > BarWidth) {
                    if (TabCount > 0) {
                        LongLength = (LongLength - BarWidth) / (TabCount);
                        int TempLength = StandardButtonSize.Width - LongLength - 5;
                        if (TempLength < SmallButtonSize.Width) {
                            CurrentButtonSize = SmallButtonSize;
                        }
                        else {
                            CurrentButtonSize = new Size(TempLength, SmallButtonSize.Height);
                        }
                    }
                    else { CurrentButtonSize = StandardButtonSize; }
                }
                else {
                    CurrentButtonSize = StandardButtonSize;
                }
                //if ()
            }
            MaximumTabs = (int)Math.Floor((((float)GetTabSpaceWidth(false)) / (float)CurrentButtonSize.Width));
            TabPadding = Height - CurrentButtonSize.Height;

            if (TabCount > MaximumTabs) {
                OverflowButtonsVisible = true;
                MaximumOverflow = TabCount - MaximumTabs;
            }
            else {
                OverflowButtonsVisible = false;
                MaximumOverflow = 0;
            }
            if (OverflowButtonsVisible == false) {
                TabStart = 0;
            }
            else {
                MaximumTabs = (int)Math.Floor(((float)GetTabSpaceWidth(true) / (float)CurrentButtonSize.Width));
            }
        }
        private int GetTabCount() {
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    return bindedTabControl.TabPages.Count;
                }
                else { return 0; }
            }
            else {
                return tabs.Count;
            }
        }
        private int GetTabSpaceWidth(bool IncludeOverflowButtons) {
            int AddButtonOffset = (int)(UnitSize.Height * 1.8);
            if (showAddButton == true) {
                if (IncludeOverflowButtons == false) {
                    return Width - Padding.Left - Padding.Right - AddButtonOffset;
                }
                else {
                    return Width - Padding.Left - Padding.Right - (int)(UnitSize.Height * 5.8f);
                }
            }
            else {
                return Width - Padding.Left - Padding.Right;
            }
        }
        #endregion
        #region Drawing
        protected override void OnPaint(PaintEventArgs e) {
            RenderSetup(e);
            DrawUnderTabs(e);
            if (showTabDividers == true) {
                DrawTabDividers(e);
            }
            DrawOverTabs(e);
            if (tabStyle == TabStyles.Normal) {
                using (SolidBrush Br = new SolidBrush(tabRuleColor)) {
                    using (Pen Pn = new Pen(Br)) {
                        e.Graphics.DrawLine(Pn, 0, Height - 1, Width, Height - 1);
                    }
                }
            }
            DrawOverflow(e);
        }
        private void DrawUnderTabs(PaintEventArgs e) {
            int RunningSize = Padding.Left;
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                        if ((i >= 0) && (i < bindedTabControl.TabPages.Count)) {
                            Action Current = Action.Normal;
                            if (i == bindedTabControl.SelectedIndex) {
                                Current = Action.Selected;
                            }
                            else if (i == HoverButton) {
                                Current = Action.MouseOver;
                            }
                            Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
                            if (Current != Action.Selected) {
                                DrawTab(e, bindedTabControl.TabPages[i].Text, Current, TabRectangle, false);
                            }
                            RunningSize += TabRectangle.Width;
                        }
                    }
                }
            }
            else {
                for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                    if ((i >= 0) && (i < tabs.Count)) {
                        Action Current = Action.Normal;
                        if (i == selectedIndex) {
                            Current = Action.Selected;
                        }
                        else if (i == HoverButton) {
                            Current = Action.MouseOver;
                        }
                        Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
                        if (Current != Action.Selected) {
                            DrawTab(e, tabs[i].Text, Current, TabRectangle, true);
                        }
                        RunningSize += TabRectangle.Width;
                    }
                }
            }
        }
        private void DrawTabDividers(PaintEventArgs e) {
            int RunningSize = Padding.Left;
            Action PreviousAction = Action.Selected;
            int DivideShrink = 3;
            int DivideMultiply = DivideShrink * 2;
            for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                if ((i >= 0) && (i < TabCount)) {
                    Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
                    if ((PreviousAction != Action.Selected) && (i != selectedIndex) && (i != HoverButton)) {
                        using (SolidBrush Br = new SolidBrush(tabDividerColor)) {
                            using (Pen Pn = new Pen(Br)) {
                                e.Graphics.DrawLine(Pn, RunningSize, TabRectangle.Y + 1 + DivideShrink, RunningSize, TabRectangle.Y + 1 + TabRectangle.Height - DivideMultiply);
                            }
                        }
                    }
                    if ((i == selectedIndex) || (i == HoverButton)) {
                        PreviousAction = Action.Selected;
                    }
                    else {
                        PreviousAction = Action.Normal;
                    }
                    RunningSize += TabRectangle.Width;
                }
            }
        }
        private void DrawOverTabs(PaintEventArgs e) {
            int RunningSize = Padding.Left;
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                        if ((i >= 0) && (i < bindedTabControl.TabPages.Count)) {
                            if (i == bindedTabControl.SelectedIndex) {
                                if (InDrag == true) {
                                    RunningSize += MousePos.X - MouseDownLocation.X;
                                }
                            }
                            Action Current = Action.Normal;
                            Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
                            if (i == bindedTabControl.SelectedIndex) {
                                Current = Action.Selected;
                                DrawTab(e, bindedTabControl.TabPages[i].Text, Current, TabRectangle, false);
                            }
                            RunningSize += TabRectangle.Width;
                        }
                    }
                }
            }
            else {
                for (int i = TabStart; i < TabStart + MaximumTabs; i++) {
                    if ((i >= 0) && (i < tabs.Count)) {
                        if (i == selectedIndex) {
                            if (InDrag == true) {
                                RunningSize += MousePos.X - MouseDownLocation.X;
                            }
                        }
                        Action Current = Action.Normal;
                        Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
                        if (i == selectedIndex) {
                            Current = Action.Selected;
                            DrawTab(e, tabs[i].Text, Current, TabRectangle, true);
                        }
                        RunningSize += TabRectangle.Width;
                    }
                }
            }
        }
        Rectangle LeftOffsetRectangle;
        Rectangle RightOffsetRectangle;
        Rectangle AddButtonOffsetRectangle;
        private void DrawOverflow(PaintEventArgs e) {
            Size ButtonSize = new Size((int)UnitSize.Height, (int)UnitSize.Height);
            int TabPadding = Height - CurrentButtonSize.Height + ((CurrentButtonSize.Height - ButtonSize.Height) / 2);
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
                if (AddButtonOffsetRectangle.Contains(MousePos)) {
                    AddButtonArrow = addHoverColor;
                }
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
                if (LeftOffsetRectangle.Contains(MousePos)) {
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
                if (RightOffsetRectangle.Contains(MousePos)) {
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
                CloseButtonPadding = TextPadding / 2;
            }
            int CentreText = (TabRectangle.Height - GenericTextHeight) / 2;

            //Rectangle TabRectangle = new Rectangle(Location.X, Location.Y, (int)TextSize.Width + (TabPadding * 2), (int)TextSize.Height + (TabPadding / 4));
            Rectangle TextRectangle = new Rectangle(TabRectangle.X + (TextPadding / 2), TabRectangle.Y + CentreText, TabRectangle.Width - (TextPadding) - CloseButtonPadding, GenericTextHeight);
            if (tabStyle == TabStyles.Normal) {

                if (DisplayAction == Action.Selected) {
                    DrawRectangleShadow(e, TabRectangle);
                }
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
            else if (tabStyle == TabStyles.Angled) {
                Color backColor = GetTabColor(DisplayAction);
                Color borderColor = GetTabColor(DisplayAction, true);
                int AngleInset = 4;
                using (SolidBrush backBrush = new SolidBrush(backColor)) {

                    e.Graphics.FillPolygon(backBrush, new Point[] {
                        new Point(TabRectangle.Left + AngleInset, TabRectangle.Top),
                        new Point(TabRectangle.Right - AngleInset, TabRectangle.Top),
                        new Point(TabRectangle.Right, TabRectangle.Bottom),
                        new Point(TabRectangle.Left, TabRectangle.Bottom),
                    });
                }
                using (SolidBrush borderBrush = new SolidBrush(borderColor)) {
                    using (Pen borderPen = new Pen(borderBrush)) {
                        e.Graphics.DrawLines(borderPen, new Point[] {
                            new Point(TabRectangle.Left, TabRectangle.Bottom),
                            new Point(TabRectangle.Left + AngleInset, TabRectangle.Top),
                            new Point(TabRectangle.Right- AngleInset, TabRectangle.Top),
                            new Point(TabRectangle.Right, TabRectangle.Bottom)
                        });
                    }
                }
            }
            else if (tabStyle == TabStyles.Underlined) {

            }
            if (HasCloseButton == true) {
                int CrossSize = GenericTextHeight / 2;
                int CentreCross = (TabRectangle.Height - CrossSize) / 2;
                Rectangle CloseMarker = new Rectangle(TabRectangle.X + TabRectangle.Width - (CloseButtonPadding * 2), TabRectangle.Y + CentreCross, CrossSize, CrossSize);
                Color CloseColor = ForeColor;
                if (DisplayAction == Action.Selected) {
                    CloseColor = TabSelectedForeColor;
                }
                if (CloseMarker.Contains(MousePos)) {
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
            DrawText(e, TextRectangle, TabForeColor, TabText);
            return TabRectangle.Width;
        }
        private void DrawText(PaintEventArgs e, Rectangle TextRectangle, Color TextColor, string TabText) {
            using (LinearGradientBrush textBrush = new LinearGradientBrush(TextRectangle, Color.Transparent, Color.Transparent, 0.0f)) {
                ColorBlend cb = new ColorBlend();
                cb.Positions = new[] { 0, 0.84f, 1 };
                cb.Colors = new[] { TextColor, TextColor, Color.Transparent };
                textBrush.InterpolationColors = cb;

                using (StringFormat textFormat = new StringFormat()) {
                    textFormat.Trimming = StringTrimming.None;
                    textFormat.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString(TabText, Font, textBrush, TextRectangle, textFormat);
                }
            }
        }
        private void DrawRectangleShadow(PaintEventArgs e, Rectangle TabRectangle) {
            using (GraphicsPath Path = new GraphicsPath()) {
                Rectangle ShadowRectangle = GrowRectangle(TabRectangle, 5, 5, 5, 5);

                Path.AddRectangle(ShadowRectangle);
                using (PathGradientBrush PathBrush = new PathGradientBrush(Path)) {
                    PathBrush.CenterColor = tabSelectedShadowColor;
                    PathBrush.SurroundColors = new Color[] { Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent };
                    PathBrush.FocusScales = new PointF(0.75f, 0f);
                    e.Graphics.FillPath(PathBrush, Path);
                    using (SolidBrush Br = new SolidBrush(BackColor)) {
                        e.Graphics.FillRectangle(Br, TabRectangle);
                    }
                }
            }
        }

        #endregion
        #region Drawing Support
        private Point GetCentre(Rectangle Input) {
            return new Point(Input.X + (Input.Width / 2), Input.Y + (Input.Height / 2));
        }
        private Rectangle GrowRectangle(Rectangle Input, int Left, int Right, int Top, int Bottom) {
            return new Rectangle(Input.X - Left, Input.Y - Top, Input.Width + Left + Right, Input.Height + Top + Bottom);
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
                         new Point(CollapseMarker.Left + Length - 1, CollapseMarker.Top + HalfLength)};
        }


        #endregion
        #region Mouse Events
        bool MouseDwm = false;
        bool InDrag = false;
        Point MouseDownLocation = Point.Empty;
        Point MouseDownLocationAbsolute = Point.Empty;
        private void TabHeader_MouseUp(object? sender, MouseEventArgs e) {
            MouseDwm = false;
            InDrag = false;
        }
        int DragOffsetDelta = 0;
        private void TabHeader_MouseDown(object? sender, MouseEventArgs e) {
            MouseDownLocation = e.Location;
            MouseDownLocationAbsolute = e.Location;
            if (e.Button == MouseButtons.Left) {
                int X = GetButton(e.Location);
                if (X == -1) { }
                else {
                    selectedIndex = X;
                    Invalidate();
                }
                DragOffsetDelta = 0;
                Rectangle BtnRectangle = ShrinkRectangle(GetButtonRectangle(X), 0.5f, 0);
                DragOffsetDelta = BtnRectangle.Right - MouseDownLocationAbsolute.X;

            }


            MouseDwm = true;
        }
        private void TabHeader_MouseHover(object? sender, EventArgs e) {
            Invalidate();
        }
        private void TabHeader_LostFocus(object? sender, EventArgs e) {
            MousePos = new Point(-1, -1);
            HoverButton = -1;
            LatchDelta = false;
            InDrag = false;
            MouseDwm = false;
            Invalidate();
        }
        private void TabHeader_MouseLeave(object? sender, EventArgs e) {
            MousePos = new Point(-1, -1);
            LatchDelta = false;
            HoverButton = -1;
            Invalidate();
        }
        int HoverButton = -1;
        private void TabHeader_MouseMove(object? sender, MouseEventArgs e) {
            this.Cursor = Cursors.Default;
            if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.None)) {
                if (MouseMoved(e.Location, 5, false) == false) {
                    MousePos = e.Location;
                    int X = GetButton(e.Location);
                    HoverButton = X;
                }
                else {
                    if (AllowDragReordering) {
                        HoverButton = -1;
                        MousePos = e.Location;
                        InDrag = true;
                        MoveTab();
                    }
                }
            }
            Invalidate();
        }
        private void TabHeader_MouseClick(object? sender, MouseEventArgs e) {
            int X = GetButton(MouseDownLocation);
            if (e.Button == MouseButtons.Left) {
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
                        if (InDrag == false) {
                            CloseButtonClicked?.Invoke(this, X);
                        }
                    }
                    else {
                        SelectTab(X);
                        SelectedIndex = X;
                        object? Data = GetTabData(X);
                        if (Data != null) {
                            Point HitLocation = new Point(0, 0);
                            TabClickedEventArgs TabEventArgs = new TabClickedEventArgs(Data, X, HitLocation, Cursor.Position, GetButtonRectangle(e.Location), (TextPadding / 2));
                            TabClicked?.Invoke(this, TabEventArgs);
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right) {
                if (X >= 0) {
                    object? Data = GetTabData(X);
                    if (Data != null) {
                        Point HitLocation = new Point(0, 0);
                        TabClickedEventArgs TabEventArgs = new TabClickedEventArgs(Data, X, HitLocation, Cursor.Position, GetButtonRectangle(e.Location), (TextPadding / 2));
                        TabRightClicked?.Invoke(this, TabEventArgs);
                    }
                }
            }
            Invalidate();
        }
        private void TabHeader_MouseWheel(object? sender, MouseEventArgs e) {
            HoverButton = -1;
            if (InDrag == true) { return; }
            if (e.Delta > 0) {
                SelectedIndex--;
                SelectTab(SelectedIndex);
                if (OverflowButtonsVisible == true) {
                    if (SelectedIndex < TabStart) {
                        TabStart--;
                    }
                }
            }
            else {
                SelectedIndex++;
                SelectTab(SelectedIndex);
                if (OverflowButtonsVisible == true) {
                    if (SelectedIndex > TabStart + MaximumTabs - 1) {
                        TabStart++;
                    }
                }
            }
            Invalidate();
        }
        private bool MouseMoved(Point MousePos, int Magnitude, bool IgnoreMouseState = false) {
            if (IgnoreMouseState == false) { if (MouseDwm == false) { return false; } }
            if (Math.Abs(MousePos.X - MouseDownLocation.X) >= Magnitude) {
                return true;
            }
            return false;
        }
        #endregion
        private Rectangle GetButtonRectangle(Point Pnt) {
            int Index = GetButton(Pnt) - TabStart;
            int XPosition = Padding.Left + (CurrentButtonSize.Width * Index);
            if (Index < 0) { return Rectangle.Empty; }
            return new Rectangle(XPosition, TabPadding, CurrentButtonSize.Width, CurrentButtonSize.Height);
        }
        private int GetButton(Point Pnt) {
            if (Pnt.Y >= Height - CurrentButtonSize.Height) {
                int Temp = (Pnt.X - Padding.Left) / CurrentButtonSize.Width;
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
        Rectangle GetButtonRectangle(int Button) {
            if (Button < 0) { return Rectangle.Empty; }
            if (Button >= TabCount) { return Rectangle.Empty; }
            int x = (CurrentButtonSize.Width * Button) - (TabStart * CurrentButtonSize.Width) + Padding.Left;
            if (OverflowButtonsVisible == true) {
                if ((Button >= TabStart) && (Button < TabCount) && (Button < TabStart + MaximumTabs)) {
                    return new Rectangle(x, TabPadding, CurrentButtonSize.Width, CurrentButtonSize.Height);
                }
            }
            else {
                if ((Button >= TabStart) && (Button < TabCount)) {
                    return new Rectangle(x, TabPadding, CurrentButtonSize.Width, CurrentButtonSize.Height);
                }
            }
            //x=w*y-s*w+p
            return Rectangle.Empty;
        }
        Point MousePos = new Point(-1, -1);
        bool LatchDelta = false;
        private void MoveTab() {
            Rectangle CurrentRectangle = GetButtonRectangle(MousePos);
            int CurrentTab = GetButton(MouseDownLocation);
            Rectangle PreviousTab = ShrinkRectangle(GetButtonRectangle(CurrentTab - 1), 0.5f, 0);
            Rectangle NextTab = ShrinkRectangle(GetButtonRectangle(CurrentTab + 1), 0.5f, 0);
            int Delta = MousePos.X - MouseDownLocation.X;
            if (LatchDelta == true) {
                Delta -= DragOffsetDelta;
            }

            if (Delta < 0) {
                if (CurrentTab <= 0) { return; }
                int Cur = CurrentRectangle.X + Delta;
                if (Cur < PreviousTab.Right - 3) {
                    Swap(CurrentTab, CurrentTab - 1);
                    SelectedIndex = CurrentTab - 1;
                    if (OverflowButtonsVisible) {
                        if (SelectedIndex < TabStart) {
                            TabStart--;
                            PreviousTab = ShrinkRectangle(GetButtonRectangle(CurrentTab - 1), 0.5f, 0);
                        }
                    }
                    MouseDownLocation = new Point(PreviousTab.Right - DragOffsetDelta, MouseDownLocation.Y);
                    LatchDelta = true;
                }
            }
            else if (Delta > 0) {
                if (CurrentTab >= TabCount - 1) { return; }
                int Cur = CurrentRectangle.Right + Delta;
                if (Cur > NextTab.Right + 3) {
                    Swap(CurrentTab, CurrentTab + 1);
                    SelectedIndex = CurrentTab + 1;
                    if (OverflowButtonsVisible) {
                        if (SelectedIndex > TabStart + MaximumTabs - 1) {
                            TabStart++;
                            NextTab = ShrinkRectangle(GetButtonRectangle(CurrentTab + 1), 0.5f, 0);
                        }
                    }
                    MouseDownLocation = new Point(NextTab.Right - DragOffsetDelta, MouseDownLocation.Y);
                    LatchDelta = true;
                }
            }
        }
        public void Swap(int indexA, int indexB) {
            if (indexA < 0) { return; }
            if (indexB < 0) { return; }
            if (indexA >= TabCount) { return; }
            if (indexB >= TabCount) { return; }
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    TabPage TabTemp = bindedTabControl.TabPages[indexA];
                    bindedTabControl.TabPages[indexA] = bindedTabControl.TabPages[indexB];
                    bindedTabControl.TabPages[indexB] = TabTemp;
                }
            }
            else {
                Tab TabTemp = tabs[indexA];
                tabs[indexA] = tabs[indexB];
                tabs[indexB] = TabTemp;
            }
        }
        private Rectangle ShrinkRectangle(Rectangle Input, float XFactor, float YFactor) {
            return new Rectangle(Input.X, Input.Y, (int)((float)Input.Width * XFactor), (int)((float)Input.Height * YFactor));
        }
        private object? GetTabData(int X) {
            if (X < 0) { return null; }
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    if (bindedTabControl.TabPages.Count > 0) {
                        if (X < bindedTabControl.TabPages.Count) {
                            return bindedTabControl.TabPages[X];
                        }
                    }
                }
                else {
                    if (X < tabs.Count) {
                        return tabs[X];
                    }
                }
            }
            else {
                if (X < tabs.Count) {
                    return tabs[X];
                }
            }
            return null;
        }
        private bool IsOnCloseButton(int TabIndex, Point MPosition) {
            int RunningSize = Padding.Left;
            int ZeroedIndex = TabIndex - TabStart;
            RunningSize += CurrentButtonSize.Width * ZeroedIndex;
            Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), CurrentButtonSize);
            int CrossSize = GenericTextHeight;/// 2;
            int CentreCross = (TabRectangle.Height - CrossSize) / 2;
            Rectangle CloseMarker = new Rectangle(TabRectangle.X + TabRectangle.Width - TextPadding, TabRectangle.Y + CentreCross, CrossSize + 1, CrossSize + 1);
            return CloseMarker.Contains(MPosition);
        }
        private void SelectTab(int TabIndex) {
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    bindedTabControl.SelectedIndex = TabIndex;
                }
            }
        }
        private void TabHeader_Resize(object? sender, EventArgs e) {
            Invalidate();
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
    public class TabClickedEventArgs : EventArgs {
        public object SelectedTab;
        private Point location;
        private Point screenLocation;
        private Rectangle textarea;
        private int textOffset = 0;
        public Point ScreenLocation {
            get { return screenLocation; }
        }
        public Rectangle TextArea {
            get { return textarea; }
        }
        public Point Location {
            get { return location; }
        }
        int index = -1;
        public int Index {
            get { return index; }
        }
        public int TextOffset {
            get { return textOffset; }
        }

        public TabClickedEventArgs(object SelectedTab, int Index, Point HitPoint, Point AbsolutePosition, Rectangle textarea, int TextOffset) {
            this.SelectedTab = SelectedTab;
            this.index = Index;
            location = HitPoint;
            screenLocation = AbsolutePosition;
            this.textarea = textarea;
            this.textOffset = TextOffset;
        }
    }
}
