using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ODModules {
    public class ContextMenu : ContextMenuStrip {
        public ContextMenu() {
            Renderer = new ContextRender();
            ForeColor = Color.White;
            DoubleBuffered = true;
        }
        #region Properties
        private Color borderColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColor {
            get { return borderColor; }
            set {
                borderColor = value;
                if (IsRendererVaild() == true) {
                    ((ContextRender)Renderer).BorderColor = borderColor;
                }
            }
        }
        private Color mouseOverColor = Color.FromArgb(127, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color MouseOverColor {
            get { return mouseOverColor; }
            set {
                mouseOverColor = value;
                if (IsRendererVaild() == true) {
                    ((ContextRender)Renderer).MouseOverColor = mouseOverColor;
                }
            }
        }
        private Color menuBackColor = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorNorth {
            get { return menuBackColor; }
            set {
                menuBackColor = value;
                Invalidate();
            }
        }
        private Color menuBackColor2 = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorSouth {
            get { return menuBackColor2; }
            set {
                menuBackColor2 = value;
                Invalidate();
            }
        }
        private Color insetshadowColor = Color.FromArgb(128, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color InsetShadowColor {
            get { return insetshadowColor; }
            set {
                insetshadowColor = value;
                if (IsRendererVaild() == true) {
                    ((ContextRender)Renderer).InsetShadowColor = insetshadowColor;
                }
                Invalidate();
            }
        }
        private Color actionSymbolForeColor = Color.FromArgb(200, 200, 200);
        [System.ComponentModel.Category("Appearance")]
        public Color ActionSymbolForeColor {
            get { return actionSymbolForeColor; }
            set {
                actionSymbolForeColor = value;
                if (IsRendererVaild() == true) {
                    ((ContextRender)Renderer).ActionSymbolForeColor = actionSymbolForeColor;
                }
                Invalidate();
            }
        }
        private bool showInsetShadow = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowInsetShadow {
            get { return showInsetShadow; }
            set {
                showInsetShadow = value;
                Invalidate();
            }
        }
        private bool showItemInsetShadow = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowItemInsetShadow {
            get { return showItemInsetShadow; }
            set {
                showItemInsetShadow = value;
                if (IsRendererVaild() == true) {
                    ((ContextRender)Renderer).ShowInsetShadow = showItemInsetShadow;
                }
            }
        }
        #endregion
        #region Drawing
        protected override void OnPaint(PaintEventArgs e) {
            Rectangle ContextWindow = new Rectangle(0, 0, Width, Height);
            using (LinearGradientBrush BackgroundBrush = new LinearGradientBrush(ContextWindow, menuBackColor, menuBackColor2, 90)) {
                e.Graphics.FillRectangle(BackgroundBrush, ContextWindow);
            }
            int ShadowSize = 4;
            if (showInsetShadow) {
                Rectangle ShadowRectangleTop = new Rectangle(ContextWindow.X, ContextWindow.Y, ContextWindow.Width, ShadowSize);
                Rectangle ShadowRectangleBottom = new Rectangle(ContextWindow.X, ContextWindow.Y + ContextWindow.Height - ShadowSize, ContextWindow.Width, ShadowSize);
                Rectangle ShadowRectangleLeft = new Rectangle(ContextWindow.X, ContextWindow.Y, ShadowSize, ContextWindow.Height);
                Rectangle ShadowRectangleRight = new Rectangle(ContextWindow.X + ContextWindow.Width - ShadowSize, ContextWindow.Y, ShadowSize, ContextWindow.Height);
                using (LinearGradientBrush ShadowBrushTop = new LinearGradientBrush(ShadowRectangleTop, insetshadowColor, Color.Transparent, 90)) {
                    e.Graphics.FillRectangle(ShadowBrushTop, ShadowRectangleTop);
                }
                using (LinearGradientBrush ShadowBrushBottom = new LinearGradientBrush(ShadowRectangleBottom, insetshadowColor, Color.Transparent, 270)) {
                    e.Graphics.FillRectangle(ShadowBrushBottom, ShadowRectangleBottom);
                }
                using (LinearGradientBrush ShadowBrushLeft = new LinearGradientBrush(ShadowRectangleLeft, insetshadowColor, Color.Transparent, 0.0f)) {
                    e.Graphics.FillRectangle(ShadowBrushLeft, ShadowRectangleLeft);
                }
                using (LinearGradientBrush ShadowBrushRight = new LinearGradientBrush(ShadowRectangleRight, insetshadowColor, Color.Transparent, 180)) {
                    e.Graphics.FillRectangle(ShadowBrushRight, ShadowRectangleRight);
                }
            }
            base.OnPaint(e);
        }
        #endregion
        #region Support Functions
        private bool IsRendererVaild() {
            if (Renderer != null) {
                if (Renderer.GetType() == typeof(ContextRender)) {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Events

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
        }
        #endregion
    }
    public class ContextRender : ToolStripProfessionalRenderer {
        #region Properties
        private Color borderColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderColor {
            get { return borderColor; }
            set {
                borderColor = value;
            }
        }
        private Color mouseOverColor = Color.FromArgb(127, 0, 0, 0);

        public ContextRender() {
        }

        [System.ComponentModel.Category("Appearance")]
        public Color MouseOverColor {
            get { return mouseOverColor; }
            set {
                mouseOverColor = value;
            }
        }
        private Color insetshadowColor = Color.FromArgb(128, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color InsetShadowColor {
            get { return insetshadowColor; }
            set {
                insetshadowColor = value;
            }
        }
        private bool showInsetShadow = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowInsetShadow {
            get { return showInsetShadow; }
            set {
                showInsetShadow = value;
            }
        }
        private Color actionSymbolForeColor = Color.FromArgb(200, 200, 200);
        [System.ComponentModel.Category("Appearance")]
        public Color ActionSymbolForeColor {
            get { return actionSymbolForeColor; }
            set {
                actionSymbolForeColor = value;
            }
        }
        #endregion
        #region Drawing
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            Rectangle ItemBounds = new Rectangle(Point.Empty, e.Item.Size);
            Color ItemColor = Color.Transparent;
            if (e.Item.Selected == true) { ItemColor = mouseOverColor; }
            using (SolidBrush ItemSelectedBrush = new SolidBrush(ItemColor)) {
                e.Graphics.FillRectangle(ItemSelectedBrush, ItemBounds);
            }
            if (e.Item.Selected) {
                int ShadowSize = 4;
                if (showInsetShadow) {
                    Rectangle ShadowRectangleTop = new Rectangle(ItemBounds.X, ItemBounds.Y, ItemBounds.Width, ShadowSize);
                    Rectangle ShadowRectangleBottom = new Rectangle(ItemBounds.X, ItemBounds.Y + ItemBounds.Height - ShadowSize, ItemBounds.Width, ShadowSize);
                    Rectangle ShadowRectangleLeft = new Rectangle(ItemBounds.X, ItemBounds.Y, ShadowSize, ItemBounds.Height);
                    Rectangle ShadowRectangleRight = new Rectangle(ItemBounds.X + ItemBounds.Width - ShadowSize, ItemBounds.Y, ShadowSize, ItemBounds.Height);
                    using (LinearGradientBrush ShadowBrushTop = new LinearGradientBrush(ShadowRectangleTop, insetshadowColor, Color.Transparent, 90)) {
                        e.Graphics.FillRectangle(ShadowBrushTop, ShadowRectangleTop);
                    }
                    using (LinearGradientBrush ShadowBrushBottom = new LinearGradientBrush(ShadowRectangleBottom, insetshadowColor, Color.Transparent, 270)) {
                        e.Graphics.FillRectangle(ShadowBrushBottom, ShadowRectangleBottom);
                    }
                    using (LinearGradientBrush ShadowBrushLeft = new LinearGradientBrush(ShadowRectangleLeft, insetshadowColor, Color.Transparent, 0.0f)) {
                        e.Graphics.FillRectangle(ShadowBrushLeft, ShadowRectangleLeft);
                    }
                    using (LinearGradientBrush ShadowBrushRight = new LinearGradientBrush(ShadowRectangleRight, insetshadowColor, Color.Transparent, 180)) {
                        e.Graphics.FillRectangle(ShadowBrushRight, ShadowRectangleRight);
                    }
                }
            }
        }
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            using (SolidBrush BorderBrush = new SolidBrush(borderColor)) {
                using (Pen BorderPen = new Pen(BorderBrush)) {
                    BorderPen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawRectangle(BorderPen, 0, 0, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
                }
            }
        }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
            base.OnRenderItemText(e);
        }
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(e.ArrowRectangle.Location, e.ArrowRectangle.Size);
            r.Inflate(-2, -6);
            using (SolidBrush ActionBrush = new SolidBrush(actionSymbolForeColor)) {
                using (Pen ActionPen = new Pen(ActionBrush, 2)) {
                    e.Graphics.DrawLines(ActionPen, new Point[]{
        new Point(r.Left, r.Top),
        new Point(r.Right, r.Top + r.Height /2),
        new Point(r.Left, r.Top+ r.Height)});
                }
            }
        }
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(e.ImageRectangle.Location, e.ImageRectangle.Size);
            r.Inflate(-4, -6);
            using (SolidBrush ActionBrush = new SolidBrush(actionSymbolForeColor)) {
                using (Pen ActionPen = new Pen(ActionBrush, 2)) {
                    e.Graphics.DrawLines(ActionPen, new Point[]{
        new Point(r.Left, r.Bottom - r.Height /2),
        new Point(r.Left + r.Width /3,  r.Bottom),
        new Point(r.Right, r.Top)});
                }
            }
        }
        #endregion
    }
}
