using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {
    public class TabHeader : UserControl {
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

        public TabHeader() {
            DoubleBuffered = true;
            MouseWheel += TabHeader_MouseWheel;
        }

        private void TabHeader_MouseWheel(object? sender, MouseEventArgs e) {
            if (e.Delta > 0) {
                SelectedIndex++;
            }
            else {
                SelectedIndex--;
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
        Size StandardButtonSize = new Size(0, 10);
        int GenericTextHeight = 10;
        int TextPadding = 10;
        protected override void OnPaint(PaintEventArgs e) {
            int RunningSize = Padding.Left;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            TextPadding = (int)e.Graphics.MeasureString("W", Font).Width;
            GenericTextHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            StandardButtonSize = new Size(GenericTextHeight * 8, (int)(GenericTextHeight * 1.25f));
            int TabPadding = Height - StandardButtonSize.Height;
            if (useBindingTabControl == true) {
                if (bindedTabControl != null) {
                    for (int i = 0; i < bindedTabControl.TabPages.Count; i++) {
                        Action Current = Action.Normal;
                        if (i == bindedTabControl.SelectedIndex) {
                            Current = Action.Selected;
                        }
                        Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), StandardButtonSize);
                        RunningSize += DrawTab(e, bindedTabControl.TabPages[i].Text, Current, TabRectangle, false);
                    }
                }
            }
            else {
                for (int i = TabStart; i < tabs.Count; i++) {
                    Action Current = Action.Normal;
                    if (i == selectedIndex) {
                        Current = Action.Selected;
                    }
                    Rectangle TabRectangle = new Rectangle(new Point(RunningSize, TabPadding), StandardButtonSize);
                    RunningSize += DrawTab(e, tabs[i].Text, Current, TabRectangle, true);
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
                using (SolidBrush ActionBrush = new SolidBrush(Color.White)) {
                    using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y + CloseMarker.Height));
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y + CloseMarker.Height), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y));
                    }
                }
            }
            Color TabForeColor = ForeColor;
            if (DisplayAction == Action.Selected) { TabForeColor = tabSelectedForeColor; }
            using (SolidBrush textBrush = new SolidBrush(TabForeColor)) {
                using (StringFormat textFormat = new StringFormat()) {
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
            return tabBackColor;
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
            MouseClick = 0x04
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
