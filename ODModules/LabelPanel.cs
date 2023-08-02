using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public partial class LabelPanel : Panel {
        public delegate void CloseClickedHandler(object sender, Point HitPoint);
        [Category("Button Actions")]
        public event CloseClickedHandler? CloseButtonClicked;
        public delegate void CollapseClickedHandler(object sender, Point HitPoint);
        [Category("Button Actions")]
        public event CollapseClickedHandler? CollapseButtonClicked;
        public LabelPanel() {
            InitializeComponent();
            labelFont = new Font(Font.FontFamily, 8);
            DoubleBuffered = true;
            //this.Paint += DropShadow;
            OldHeight = Height;
            OldMaxSize = MaximumSize;

        }
        #region Properties
        [Browsable(true)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
                Invalidate();
            }
        }
        bool PaddingSettingChanged = true;
        private Font labelFont;
        [System.ComponentModel.Category("Appearance")]
        public Font LabelFont {
            get {
                return labelFont;
            }
            set {
                labelFont = value;
                PaddingSettingChanged = true;
                Invalidate();
            }
        }
        private Color labelColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color LabelForeColor {
            get {
                return labelColor;
            }
            set {
                labelColor = value;
                Invalidate();
            }
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
        private Color closeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color CloseColor {
            get {
                return closeColor;
            }
            set {
                closeColor = value;
                Invalidate();
            }
        }
        private Color closeMouseOverColor = Color.Red;
        [System.ComponentModel.Category("Appearance")]
        public Color CloseMouseOverColor {
            get {
                return closeMouseOverColor;
            }
            set {
                closeMouseOverColor = value;
                Invalidate();
            }
        }
        private Color labelBackColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color LabelBackColor {
            get {
                return labelBackColor;
            }
            set {
                labelBackColor = value;
                Invalidate();
            }
        }
        private Color dropShadowColor = Color.FromArgb(128, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color DropShadowColor {
            get {
                return dropShadowColor;
            }
            set {
                dropShadowColor = value;
                if (this.Parent != null) {
                    this.Parent.Invalidate();
                }
            }
        }
        private bool dropShadow = false;
        [System.ComponentModel.Category("Appearance")]
        public bool DropShadow {
            get {
                return dropShadow;
            }
            set {
                dropShadow = value;
                if (this.Parent != null) {
                    this.Parent.Invalidate();
                }
            }
        }
        private bool collapsible = true;
        [System.ComponentModel.Category("Appearance")]
        public bool PanelCollapsible {
            get {
                return collapsible;
            }
            set {
                collapsible = value;
                if (value == false) {
                    Collapsed = false;
                }
            }
        }
        private bool showCloseButton = true;
        [System.ComponentModel.Category("Appearance")]
        public bool ShowCloseButton {
            get {
                return showCloseButton;
            }
            set {
                showCloseButton = value;
                Invalidate();
            }
        }
        private bool overrideCollapseControl = false;
        [System.ComponentModel.Category("Control")]
        public bool OverrideCollapseControl {
            get {
                return overrideCollapseControl;
            }
            set {
                overrideCollapseControl = value;
            }
        }
        //ResizeDirection
        private ResizeDirection resizeControl = ResizeDirection.None;
        [System.ComponentModel.Category("Control")]
        public ResizeDirection ResizeControl {
            get {
                return resizeControl;
            }
            set {
                resizeControl = value;
            }
        }
        int OldHeight = 0;
        private bool collapsed = false;
        Size OldMaxSize;
        [System.ComponentModel.Category("Appearance")]
        public bool Collapsed {
            get {
                return collapsed;
            }
            set {
                collapsed = value;
                if (overrideCollapseControl == true) {
                    if (value == true) {
                        OldHeight = Height;
                        OldMaxSize = MaximumSize;
                        Height = TextHeight;
                        MaximumSize = OldMaxSize;
                    }
                    else {
                        Height = OldHeight;
                        MaximumSize = OldMaxSize;
                    }
                    Invalidate();
                    if (this.Parent != null) {
                        this.Parent.Invalidate();
                    }
                }
            }
        }
        #endregion
        #region Drawing
        float TextPadding = 10;
        int TextHeight = 20;
        //int BasicTextSize = 10;
        SizeF UnitSize;
        Rectangle CollapseMarker;
        Rectangle CloseMarker;
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Font UnitFont = new Font(this.Font.FontFamily, 6.0f)) {
                UnitSize = e.Graphics.MeasureString("W", UnitFont);
            }
            if (PaddingSettingChanged == true) {
                TextPadding = e.Graphics.MeasureString("W", labelFont).Width;
                TextHeight = (int)e.Graphics.MeasureString("W", labelFont).Height + (int)(TextPadding / 4.0f);

                Padding Pad = new Padding(Padding.Left, TextHeight, Padding.Right, Padding.Bottom);
                Padding = Pad;
                PaddingSettingChanged = false;
            }
            using (SolidBrush BackColorBrush = new SolidBrush(labelBackColor)) {
                e.Graphics.FillRectangle(BackColorBrush, new Rectangle(0, 0, Width, TextHeight));
            }
            DrawActionButtons(e);
            if (Text.Length > 0) {
                using (SolidBrush ForeColorBrush = new SolidBrush(labelColor)) {
                    e.Graphics.DrawString(Text, labelFont, ForeColorBrush, new Point((int)(TextPadding / 4.0f), (int)(TextPadding / 4.0f)));
                }
            }
        }
        private void DrawActionButtons(PaintEventArgs e) {
            Size ButtonSize = new Size((int)UnitSize.Height, (int)UnitSize.Height);



            int CollaspeOffset = 0;
            if (showCloseButton == true) {
                int CloseSmall = (int)(UnitSize.Height * 0.8f);
                Point CloseButtonPosition = new Point(Width - (int)(UnitSize.Width * 1.8) - CollaspeOffset, (int)(TextPadding / 4.0f) + (int)((TextHeight - CloseSmall) / 2.0f));
                CloseMarker = new Rectangle(CloseButtonPosition, new Size(CloseSmall, CloseSmall));
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color collapseCloseColor = closeColor;
                if (CloseMarker_MouseInRegion == true) {
                    collapseCloseColor = closeMouseOverColor;
                }
                using (SolidBrush ActionBrush = new SolidBrush(collapseCloseColor)) {
                    using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y + CloseMarker.Height));
                        e.Graphics.DrawLine(ActionPen, new Point(CloseMarker.X, CloseMarker.Y + CloseMarker.Height), new Point(CloseMarker.X + CloseMarker.Width, CloseMarker.Y));
                    }
                }
                CollaspeOffset = (int)(ButtonSize.Width * 1.8f);
            }

            if (collapsible == true) {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Point CollaspeButtonPosition = new Point(Width - (int)(UnitSize.Width * 1.8) - CollaspeOffset, (int)(TextPadding / 4.0f) + (int)((TextHeight - (UnitSize.Height / 2)) / 2.0f));
                CollapseMarker = new Rectangle(CollaspeButtonPosition, ButtonSize);
                //CollapseMarker.Inflate(-6, -8);
                Color collapseArrowColor = arrowColor;
                if (CollapseMarker_MouseInRegion == true) {
                    collapseArrowColor = arrowMouseOverColor;
                }
                using (SolidBrush ActionBrush = new SolidBrush(collapseArrowColor)) {
                    using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                        Point[] Points;
                        ArrowPoints(CollapseMarker, out Points);
                        e.Graphics.DrawLines(ActionPen, Points);
                    }
                }
            }
        }
        private void ArrowPoints(Rectangle CollapseMarker, out Point[] Points) {
            int HalfHeight = CollapseMarker.Height / 2;
           if  (Dock == DockStyle.Bottom){
                if (collapsed == false) {
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
            else {
                if (collapsed == true) {
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
        }
        private void DropBorderShadow(object? sender, PaintEventArgs e) {
            if (dropShadow == false) { return; }
            if (dropShadowColor.A > 1) {
                int Step = 5;
                float AlphaDecremant = (float)dropShadowColor.A / (float)(Step - 1);
                int Alpha = dropShadowColor.A;
                using (SolidBrush ShadowBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0))) {
                    Point pt = this.Location;
                    for (int sp = 1; sp < Step; sp++) {
                        if (Alpha <= 0) { Alpha = 0; }
                        Color ShadowColor = Color.FromArgb(Alpha, dropShadowColor.R, dropShadowColor.G, dropShadowColor.B);
                        using (Pen ShadowPen = new Pen(ShadowColor)) {
                            e.Graphics.DrawRectangle(ShadowPen, pt.X - sp, pt.Y - sp, this.Width + (2 * sp), this.Height + (2 * sp));
                        }
                        if (Alpha > 0) { Alpha -= (int)AlphaDecremant; }
                    }
                }
            }

        }
        #endregion
        #region Events
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
        }
        protected override void OnResize(EventArgs eventargs) {
            Invalidate();
            base.OnResize(eventargs);
        }
        private void LabelPanel_Load(object sender, EventArgs e) {

        }

        protected override void OnParentChanged(EventArgs e) {
            base.OnParentChanged(e);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (this.Parent != null) {
                this.Parent.Paint += DropBorderShadow;
            }
        }
        protected override void OnHandleDestroyed(EventArgs e) {
            base.OnHandleDestroyed(e);
            if (this.Parent != null) {
                this.Parent.Paint -= DropBorderShadow;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            if (CollapseMarker.Contains(e.Location)) {
                if (collapsible == true) {
                    Collapsed = !Collapsed;
                    CollapseButtonClicked?.Invoke(this, e.Location);
                }
            }
            if (showCloseButton) {
                if (CloseMarker.Contains(e.Location)) {
                    CloseButtonClicked?.Invoke(e, e.Location);
                }
            }
            base.OnMouseClick(e);
        }
        bool CollapseMarker_MouseInRegion = false;
        bool CloseMarker_MouseInRegion = false;
        protected override void OnDockChanged(EventArgs e) {
            if (Dock == DockStyle.Fill) {
                PanelCollapsible = false;
            }
            base.OnDockChanged(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            CollapseMarker_MouseInRegion = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseHover(EventArgs e) {

            base.OnMouseHover(e);
        }
        bool InResize = false;

        protected override void OnMouseMove(MouseEventArgs e) {

            bool HitButton = false;
            if (collapsible == true) {
                if (CollapseMarker.Contains(e.Location)) {
                    CollapseMarker_MouseInRegion = true;
                    HitButton = true;
                }
                else {
                    CollapseMarker_MouseInRegion = false;
                }

                Invalidate();
            }
            if (showCloseButton) {
                if (CloseMarker.Contains(e.Location)) {
                    CloseMarker_MouseInRegion = true;
                    HitButton = true;
                }
                else {
                    CloseMarker_MouseInRegion = false;
                }
                Invalidate();
            }
            if (HitButton == false) {
                if (InResize == false) {
                    if (InResizeZone(e.Location) == true) {
                        if ((resizeControl == ResizeDirection.Top) || (resizeControl == ResizeDirection.Bottom)) {
                            this.Cursor = Cursors.SizeNS;
                        }
                        else if ((resizeControl == ResizeDirection.Left) || (resizeControl == ResizeDirection.Right)) {
                            this.Cursor = Cursors.SizeWE;
                        }
                        else {
                            this.Cursor = Cursors.Default;
                        }
                    }
                    else {
                        this.Cursor = Cursors.Default;
                    }
                }
                else {
                    if (resizeControl == ResizeDirection.Top) {
                        SizeTop();
                    }
                    else if (resizeControl == ResizeDirection.Bottom) {
                        SizeBottom();
                    }
                    else if (resizeControl == ResizeDirection.Right) {
                        SizeRight();
                    }
                    else if (resizeControl == ResizeDirection.Left) {
                        SizeLeft();
                    }
                }
            }
            CurrentLocation = Cursor.Position;
            base.OnMouseMove(e);
        }
        private void SizeTop() {
            if (Dock == DockStyle.None) {
                int NewY = OldLocation.Y + (CurrentLocation.Y - DownLocation.Y);
                int NewHeight = OldSize.Height + (DownLocation.Y - CurrentLocation.Y);
                if (TextHeight > NewHeight) {
                    this.Location = new Point(OldLocation.X, OldLocation.Y + OldSize.Height - TextHeight);
                    this.Size = new Size(OldSize.Width, TextHeight);
                }
                else {
                    this.Location = new Point(OldLocation.X, NewY);
                    this.Size = new Size(OldSize.Width, NewHeight);
                }
            }
            else if (Dock == DockStyle.Bottom) {
                this.Size = new Size(OldSize.Width, ConditionSize(OldSize.Height + (DownLocation.Y - CurrentLocation.Y)));
            }
        }
        private void SizeLeft() {
            if (Dock == DockStyle.None) {
                int NewX = OldLocation.X + (CurrentLocation.X - DownLocation.X);
                int NewWidth = OldSize.Width + (DownLocation.X - CurrentLocation.X);
                if (TextHeight > NewWidth) {
                    this.Location = new Point(OldLocation.X+ OldSize.Width - TextHeight, OldLocation.Y);
                    this.Size = new Size(TextHeight, OldSize.Height);
                }
                else {
                    this.Location = new Point(NewX, OldLocation.Y);
                    this.Size = new Size(NewWidth, OldSize.Height);
                }
            }
            else if (Dock == DockStyle.Right) {
                this.Size = new Size(ConditionSize(OldSize.Width + (DownLocation.X - CurrentLocation.X)), OldSize.Height);
            }
        }
        private void SizeBottom() {
            if (Dock == DockStyle.None) {
                int NewHeight = OldSize.Height + (CurrentLocation.Y - DownLocation.Y);
                if (TextHeight > NewHeight) {
                    this.Size = new Size(OldSize.Width, TextHeight);
                }
                else {
                    this.Size = new Size(OldSize.Width, NewHeight);
                }
            }
            else if (Dock == DockStyle.Top) {
                this.Size = new Size(OldSize.Width, ConditionSize(OldSize.Height + (CurrentLocation.Y - DownLocation.Y)));
            }
        }
        private void SizeRight() {
            if (Dock == DockStyle.None) {
                int NewWidth = OldSize.Width + (CurrentLocation.X - DownLocation.X);
                if (TextHeight > NewWidth) {
                    this.Size = new Size(TextHeight, OldSize.Height);
                }
                else {
                    this.Size = new Size(NewWidth, OldSize.Height);
                }
            }
            else if (Dock == DockStyle.Left) {
                this.Size = new Size(ConditionSize(OldSize.Width + (CurrentLocation.X - DownLocation.X)), OldSize.Height);
            }
        }
        private int ConditionSize(int Requested) {
            if (TextHeight > Requested) { return TextHeight; }
            else {
                return Requested;
            }
        }
        Point DownLocation = new Point(0, 0);
        Point CurrentLocation = new Point(0, 0);

        Point OldLocation = new Point(0, 0);
        Size OldSize = new Size(0, 0);
        private bool InResizeZone(Point CursorLocation) {
            int InsetSize = 5;
            if (collapsed == true) { return false; }
            if (resizeControl == ResizeDirection.Top) {
                if (CursorLocation.Y < InsetSize) {
                    return true;
                }
            }
            else if (resizeControl == ResizeDirection.Bottom) {
                if (CursorLocation.Y >= Height - InsetSize) {
                    return true;
                }
            }
            else if (resizeControl == ResizeDirection.Left) {
                if (CursorLocation.X < InsetSize) {
                    return true;
                }
            }
            else if (resizeControl == ResizeDirection.Right) {
                if (CursorLocation.X >= Width - InsetSize) {
                    return true;
                }
            }
            return false;
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            if (InResize == false) {
                if (InResizeZone(e.Location) == true) {
                    DownLocation = Cursor.Position;
                    OldLocation = this.Location;
                    OldSize = this.Size;
                    InResize = true;
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            InResize = false;
            base.OnMouseUp(e);
        }
        #endregion
        public enum ResizeDirection {
            None = 0x00,
            Top = 0x01,
            Bottom = 0x02,
            Left = 0x03,
            Right = 0x04
        }

    }
}
