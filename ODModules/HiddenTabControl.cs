using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {
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
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms.VisualStyles;

    public class HiddenTabControl : TabControl {
        private bool FlagControl = false;
        private Color _DefaultColor1;
        //private Point MousePos;
        //private bool MSE = false;
        [System.ComponentModel.Category("Appearance")]
        public Color DefaultColor1 {
            get {
                return _DefaultColor1;
            }
            set {
                _DefaultColor1 = value;
                Invalidate();
            }
        }
        private bool _DebugMode;
        [System.ComponentModel.Category("Appearance")]
        public bool DebugMode {
            get {
                return _DebugMode;
            }
            set {
                _DebugMode = value;
                DebugModeTab();
                Invalidate();
            }
        }
        public HiddenTabControl() {
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.DoubleBuffer, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            DebugModeTab();
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            // Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DefaultColor1 = Color.DodgerBlue;
            ForeColor = Color.White;
            if (!this.DesignMode) this.Multiline = true;

        }
        protected override bool ProcessMnemonic(char charCode) {
            foreach (TabPage p in this.TabPages) {
                if (Control.IsMnemonic(charCode, p.Text)) {
                    this.SelectedTab = p;
                    this.Focus();
                    return true;
                }
            }
            return false;
        } // ProcessMnemonic
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            //base.OnPaint(e);
            using (SolidBrush br = new SolidBrush(DefaultColor1)) {
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, Width, Height));
            }
            DrawControl(e.Graphics);

        }
        private void DebugModeTab() {
            if (DebugMode == true) {
                this.Appearance = TabAppearance.Normal;
                this.ItemSize = new Size(20, 20);
                this.SizeMode = TabSizeMode.Normal;
            }
            else {
                this.Appearance = TabAppearance.FlatButtons;
                this.ItemSize = new Size(0, 0);
                this.Margin = new Padding(0);
                this.SizeMode = TabSizeMode.Fixed;
            }
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x1328 && !this.DesignMode)
                m.Result = new IntPtr(1);
            else
                base.WndProc(ref m);
        }
        internal void DrawControl(Graphics g) {
            if (!Visible)
                return;
            System.Windows.Forms.VisualStyles.VisualStyleRenderer render = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Tab.Pane.Normal);
            Rectangle TabControlArea = this.ClientRectangle;
            Rectangle TabArea = this.DisplayRectangle;
            SizeF txtsz = g.MeasureString("W", Font);
            TabArea.Y = TabArea.Y + 1;
            TabArea.Height = Convert.ToInt32(txtsz.Height * 2);
            TabArea.Width = TabArea.Width + 1;
            int nDelta = SystemInformation.Border3DSize.Width;
            TabArea.Inflate(nDelta, nDelta);
            render.DrawBackground(g, TabArea);
            using (SolidBrush brn = new SolidBrush(DefaultColor1)) {
                g.FillRectangle(brn, TabArea);
            }
            int i = 0;
            while (i < this.TabCount) {
                DrawTab(g, this.TabPages[i], i);
                i += 1;
            }
        }
        internal void DrawTab(Graphics g, TabPage tabPage, int nIndex) {
            if (DebugMode == true) {
                tabPage.Font = Font;
                Rectangle recBounds = this.GetTabRect(nIndex);
                RectangleF tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                bool bSelected = (this.SelectedIndex == nIndex);
                System.Windows.Forms.VisualStyles.VisualStyleRenderer render = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Tab.Pane.Normal);
                render = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Pressed);
                if (bSelected) {
                    recBounds.Height = recBounds.Height + 10;
                    render = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Tab.TabItem.Pressed);
                    // render.DrawBackground(g, recBounds)
                    g.FillRectangle(Brushes.Blue, recBounds);
                    render.DrawEdge(g, recBounds, Edges.Diagonal, EdgeStyle.Sunken, EdgeEffects.Flat);
                }
                else {
                    recBounds.Y = recBounds.Y + 1;
                    using (SolidBrush brn = new SolidBrush(DefaultColor1)) {
                        g.FillRectangle(brn, recBounds);
                    }
                }
                using (StringFormat stringFormat = new StringFormat()) {
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    if (FlagControl) {
                        stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                    }
                    else {
                        stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                    }
                    using (SolidBrush Br = new SolidBrush(tabPage.ForeColor)) {
                        g.DrawString(tabPage.Text, Font, Br, tabTextArea, stringFormat);
                    }
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {
            using(SolidBrush Br = new SolidBrush(DefaultColor1)) {
                pevent.Graphics.FillRectangle(Br, this.Bounds);
            }
           // base.OnPaintBackground(pevent);
        }
    }

}
