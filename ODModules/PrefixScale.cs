using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Diagnostics;

namespace ODModules {
    public class PrefixScale : UserControl {
        int PrefixMin = -12;
        int PrefixMax = 12;

        //public delegate void ButtonClickedHandler(object ?sender, Point MenuPoint);
        //[Category("Value")]
        //public event ButtonClickedHandler? ButtonClicked;
        private Color hovercolor;
        [Category("Appearance")]
        public Color HoverColor {
            get { return hovercolor; }
            set { hovercolor = value; Invalidate(); }
        }
        private Color downcolor;
        [Category("Appearance")]
        public Color DownColor {
            get { return downcolor; }
            set { downcolor = value; Invalidate(); }
        }
        private Color inactiveForecolor;
        [Category("Appearance")]
        public Color InactiveForecolor {
            get { return inactiveForecolor; }
            set { inactiveForecolor = value; Invalidate(); }
        }
        private NumericTextbox? linkedNumericControl;
        [Category("Control")]
        public NumericTextbox? LinkedNumericControl {
            get { return linkedNumericControl; }
            set { linkedNumericControl = value; Invalidate(); }
        }
        //int SelectedIndex = 0;
        Point Offset = new Point(0, 0);
        Point Offset2 = new Point(0, 0);
        int YOffset = 0; int YOffset2 = 0;

        int PrefixOffset = 12;//10
        int UnitWide = 12;//10
        int UnitWide2 = 12;//10
        int UnitHeight = 10;

        bool SecondUnitVisible = false;
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            UnitHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            if (linkedNumericControl != null) {
                UnitWide = (int)e.Graphics.MeasureString("Wda" + linkedNumericControl.Unit, Font).Width;
                UnitWide2 = (int)e.Graphics.MeasureString("Wda" + linkedNumericControl.SecondaryUnit, Font).Width;
                bool HasSecondary = false;
                if (linkedNumericControl.SecondaryUnitDisplay != NumericTextbox.SecondaryUnitDisplayType.NoSecondaryUnit) {
                    if (linkedNumericControl.IsSecondaryMetric == true) { HasSecondary = true; }
                }
                if (HasSecondary == true) {
                    if (linkedNumericControl.IsMetric == true) {
                        YOffset = (Height - (int)(UnitHeight * 2.5f)) / 2;
                        YOffset2 = YOffset + (int)(UnitHeight * 1.5f);
                        SecondUnitVisible = true;
                    }
                    else {
                        YOffset2 = (Height - UnitHeight) / 2;
                        SecondUnitVisible = false;
                    }
                }
                else { YOffset = (Height - UnitHeight) / 2; SecondUnitVisible = false; }
                if (linkedNumericControl.IsMetric == true) {
                    RenderPrimaryUnit(e);
                }
                if (HasSecondary == true) {
                    RenderSecondaryUnit(e);
                }
                int FadeWidth = 100;
                Rectangle FadeLeftRectangle = new Rectangle(0, 0, FadeWidth, Height);
                using (LinearGradientBrush LeftFadeBrush = new LinearGradientBrush(FadeLeftRectangle, BackColor, Color.Transparent, 0.0f)) {
                    e.Graphics.FillRectangle(LeftFadeBrush, FadeLeftRectangle);
                }
                Rectangle FadeRightRectangle = new Rectangle(Width - FadeWidth, 0, FadeWidth, Height);
                using (LinearGradientBrush RightFadeBrush = new LinearGradientBrush(FadeRightRectangle, Color.Transparent, BackColor, 0.0f)) {
                    e.Graphics.FillRectangle(RightFadeBrush, FadeRightRectangle);
                }
            }
        }
        private void RenderPrimaryUnit(PaintEventArgs e) {
            if (linkedNumericControl != null) {
                using (StringFormat UnitStringFormat = new StringFormat()) {
                    UnitStringFormat.Alignment = StringAlignment.Center;
                    if (InMove == false) {
                        Offset = new Point(PrefixToOffset(), YOffset);
                    }
                    else {
                        Offset = new Point(NewOffset, YOffset);
                    }
                    int j = 0;
                    for (int i = PrefixMin; i <= PrefixMax; i++) {
                        string DisplayString = PrefixToSymbol(i) + linkedNumericControl.Unit;
                        Color TextColor = inactiveForecolor;
                        if (CursorLocation.X != -1) {
                            if (i == MouseIndex) {
                                if (InMove == false) {
                                    if (IsMouseDown == true) {
                                        TextColor = downcolor;
                                    }
                                    else {
                                        if (MoveHit == MouseHit.Primary) {
                                            TextColor = hovercolor;
                                        }
                                    }
                                }
                            }
                        }
                        if (CurrentPrefix == i) { TextColor = ForeColor; }
                        using (SolidBrush TextBrush = new SolidBrush(TextColor)) {
                            Rectangle TextRectangle = new Rectangle(Offset.X + (j * UnitWide), Offset.Y, UnitWide, UnitHeight);
                            e.Graphics.DrawString(DisplayString, Font, TextBrush, TextRectangle, UnitStringFormat);
                        }
                        j++;
                    }
                }
            }
        }
        private void RenderSecondaryUnit(PaintEventArgs e) {
            if (linkedNumericControl != null) {
                using (StringFormat UnitStringFormat = new StringFormat()) {
                    UnitStringFormat.Alignment = StringAlignment.Center;
                    if (InMove2 == false) {
                        Offset2 = new Point(PrefixToOffset(false), YOffset2);
                    }
                    else {
                        Offset2 = new Point(NewOffset2, YOffset2);
                    }
                    int j = 0;
                    for (int i = PrefixMin; i <= PrefixMax; i++) {
                        string DisplayString = PrefixToSymbol(i) + linkedNumericControl.SecondaryUnit;
                        Color TextColor = inactiveForecolor;
                        if (CursorLocation.X != -1) {
                            if (i == MouseIndex2) {
                                if (InMove2 == false) {
                                    if (IsMouseDown2 == true) {
                                        TextColor = downcolor;
                                    }
                                    else {
                                        if (MoveHit == MouseHit.Secondary) {
                                            TextColor = hovercolor;
                                        }
                                    }
                                }
                            }
                        }
                        if (CurrentPrefix2 == i) { TextColor = ForeColor; }
                        using (SolidBrush TextBrush = new SolidBrush(TextColor)) {
                            Rectangle TextRectangle = new Rectangle(Offset2.X + (j * UnitWide2), Offset2.Y, UnitWide2, UnitHeight);
                            e.Graphics.DrawString(DisplayString, Font, TextBrush, TextRectangle, UnitStringFormat);
                        }
                        j++;
                    }
                }
            }
        }

        private int PointToPrefixIndex(int X, bool IsPrimary = true) {
            if (IsPrimary == true) {
                int Index = CurrentPrefix + (int)Math.Floor((float)(X - ((Width - UnitWide) / 2)) / (float)UnitWide);
                return Index;
            }
            else {
                int Index = CurrentPrefix2 + (int)Math.Floor((float)(X - ((Width - UnitWide2) / 2)) / (float)UnitWide2);
                return Index;
            }
        }
        private int OffsetToIndex(int X, bool IsPrimary = true) {
            if (IsPrimary == true) {
                int Index = (int)Math.Floor((float)(X - ((Width - UnitWide) / 2)) / (float)UnitWide);
                return Index;
            }
            else {
                int Index = (int)Math.Floor((float)(X - ((Width - UnitWide2) / 2)) / (float)UnitWide2);
                return Index;
            }
        }
        private int IndexToOffset(int Index, bool IsPrimary = true) {
            if (linkedNumericControl != null) {
                if (IsPrimary == true) {
                    int ItemIndex = Index + PrefixOffset;
                    int OffsetX = ((Width - UnitWide) / 2) - (ItemIndex * UnitWide);
                    return OffsetX;
                }
                else {
                    int ItemIndex = Index + PrefixOffset;
                    int OffsetX = ((Width - UnitWide2) / 2) - (ItemIndex * UnitWide2);
                    return OffsetX;
                }
            }
            else { return 0; }
        }
        int CurrentPrefix = 0;
        int CurrentPrefix2 = 0;
        private int PrefixToOffset(bool PrimaryPrefix = true) {
            if (linkedNumericControl != null) {
                if (PrimaryPrefix == true) {
                    CurrentPrefix = (int)linkedNumericControl.Prefix;
                    int ItemIndex = CurrentPrefix + PrefixOffset;
                    int OffsetX = ((Width - UnitWide) / 2) - (ItemIndex * UnitWide);
                    return OffsetX;
                }
                else {
                    CurrentPrefix2 = (int)linkedNumericControl.SecondaryPrefix;
                    int ItemIndex = CurrentPrefix2 + PrefixOffset;
                    int OffsetX = ((Width - UnitWide2) / 2) - (ItemIndex * UnitWide2);
                    return OffsetX;
                }
            }
            else { return 0; }
        }
        private string PrefixToSymbol(int prefix) {
            switch (prefix) {
                case 12: return "Q";//
                case 11: return "R";//
                case 10: return "Y";//
                case 9: return "Z";//
                case 8: return "E";
                case 7: return "P";//
                case 6: return "T";
                case 5: return "G";
                case 4: return "M";//
                case 3: return "k";
                case 2: return "h";
                case 1: return "da";
                case 0: return "";
                case -1: return "d";
                case -2: return "c";
                case -3: return "m";//
                case -4: return "μ";
                case -5: return "n";
                case -6: return "p";//
                case -7: return "f";
                case -8: return "a";//
                case -9: return "z";//
                case -10: return "y";//
                case -11: return "r";//
                case -12: return "q";//
                default: return "";
            }
        }
        private ButtonState GetBoundState(Rectangle Bound) {
            if (Bound.Contains(CursorLocation)) {
                if (IsMouseDown == false) {
                    return ButtonState.MouseOver;
                }
                else {
                    return ButtonState.MouseDown;
                }
            }
            return ButtonState.Normal;
        }
        Point CursorLocation = new Point(0, 0);
        Point DownLocation = new Point(0, 0);
        int OldOffset = 0; int NewOffset = 0;
        int OldOffset2 = 0; int NewOffset2 = 0;

        bool IsMouseDown = false;
        bool InMove = false;

        bool IsMouseDown2 = false;
        bool InMove2 = false;
        public PrefixScale() {
            DoubleBuffered = true;
            inactiveForecolor = Color.Gray;
            hovercolor = Color.FromArgb(255, 255, 192);
            downcolor = Color.FromArgb(255, 128, 0);
        }
        int MouseIndex = 0;
        int MouseIndex2 = 0;
        MouseHit ObjectHit = MouseHit.Nothing;
        MouseHit MoveHit = MouseHit.Nothing;
        protected override void OnMouseClick(MouseEventArgs e) {
            if (linkedNumericControl != null) {
                //if (linkedNumericControl.IsMetric == true) {
                //if ((CursorLocation.X >= DownLocation.X - (UnitWide / 2)) && (CursorLocation.X <= DownLocation.X + (UnitWide / 2))) {
                if (InDownMove == false) {
                    if (MoveHit == MouseHit.Primary) {
                        if (MouseIndex <= PrefixMin) { MouseIndex = PrefixMin; }
                        if (MouseIndex >= PrefixMax) { MouseIndex = PrefixMax; }
                        linkedNumericControl.Prefix = (NumericTextbox.MetricPrefix)MouseIndex;
                    }
                    else if (MoveHit == MouseHit.Secondary) {
                        if (MouseIndex2 <= PrefixMin) { MouseIndex2 = PrefixMin; }
                        if (MouseIndex2 >= PrefixMax) { MouseIndex2 = PrefixMax; }
                        linkedNumericControl.SecondaryPrefix = (NumericTextbox.MetricPrefix)MouseIndex2;
                    }
                }
            }
            Invalidate();
            //}
            //}
            //}
            base.OnMouseClick(e);
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            CursorLocation = new Point(-1, -1);
            InMove = false; InMove2 = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            CursorLocation = e.Location;

            MoveHit = GetMouseHit(e.Location);
            if (MoveHit == MouseHit.Primary) {
                MouseIndex = PointToPrefixIndex(CursorLocation.X);
            }
            else if (MoveHit == MouseHit.Secondary) {
                MouseIndex2 = PointToPrefixIndex(CursorLocation.X, false);
            }
            if (IsMouseDown == true) {
                //MouseIndex = PointToPrefixIndex(CursorLocation.X);
                if ((CursorLocation.X >= DownLocation.X - (UnitWide / 2)) && (CursorLocation.X <= DownLocation.X + (UnitWide / 2))) { }
                else {
                    if (linkedNumericControl != null) {
                        if (linkedNumericControl.IsMetric == true) {
                            PerformSingleMove(true);
                            InDownMove = true;
                            //InMove = true;
                            //int Diff = CursorLocation.X - DownLocation.X;
                            //NewOffset = OldOffset + Diff;
                            //int MinOffset = IndexToOffset(PrefixMin);
                            //int MaxOffset = IndexToOffset(PrefixMax);
                            //if (MinOffset < NewOffset) {
                            //    NewOffset = MinOffset;
                            //}
                            //if (MaxOffset > NewOffset) {
                            //    NewOffset = MaxOffset;
                            //}
                            //PrefixToOffset();
                            //int NewIndex = ((-1) * OffsetToIndex(NewOffset)) - PrefixOffset;
                            //linkedNumericControl.Prefix = (NumericTextbox.MetricPrefix)NewIndex;
                            //CurrentPrefix = (int)linkedNumericControl.Prefix;
                        }
                    }
                }
            }
            if (IsMouseDown2 == true) {
                MouseIndex2 = PointToPrefixIndex(CursorLocation.X, false);
                if ((CursorLocation.X >= DownLocation.X - (UnitWide2 / 2)) && (CursorLocation.X <= DownLocation.X + (UnitWide2 / 2))) { }
                else {
                    if (linkedNumericControl != null) {
                        if (linkedNumericControl.IsSecondaryMetric == true) {
                            PerformSingleMove(false);
                            InDownMove = true;
                        }
                    }
                }
            }
            Invalidate();
            base.OnMouseMove(e);
        }
        private enum MouseHit {
            Nothing = 0x00,
            Primary = 0x01,
            Secondary = 0x02
        }
        bool InDownMove = false;
        private void PerformSingleMove(bool PrimaryUnit = true) {
            if (linkedNumericControl != null) {
                if (PrimaryUnit == true) { InMove = true; }
                else { InMove2 = true; }
                int Diff = CursorLocation.X - DownLocation.X;
                if (PrimaryUnit == true) {
                    NewOffset = OldOffset + Diff;
                }
                else {
                    NewOffset2 = OldOffset2 + Diff;
                }
                int MinOffset = IndexToOffset(PrefixMin, PrimaryUnit);
                int MaxOffset = IndexToOffset(PrefixMax, PrimaryUnit);
                if (PrimaryUnit == true) {
                    if (MinOffset < NewOffset) {
                        NewOffset = MinOffset;
                    }
                    if (MaxOffset > NewOffset) {
                        NewOffset = MaxOffset;
                    }
                }
                else {
                    if (MinOffset < NewOffset2) {
                        NewOffset2 = MinOffset;
                    }
                    if (MaxOffset > NewOffset2) {
                        NewOffset2 = MaxOffset;
                    }
                }
                PrefixToOffset(PrimaryUnit);
                int NewIndex = ((-1) * OffsetToIndex(PrimaryUnit == true ? NewOffset : NewOffset2, PrimaryUnit)) - PrefixOffset;
                if (PrimaryUnit == true) {
                    linkedNumericControl.Prefix = (NumericTextbox.MetricPrefix)NewIndex;
                    CurrentPrefix = (int)linkedNumericControl.Prefix;
                }
                else {
                    linkedNumericControl.SecondaryPrefix = (NumericTextbox.MetricPrefix)NewIndex;
                    CurrentPrefix2 = (int)linkedNumericControl.SecondaryPrefix;
                }
                Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            ObjectHit = MouseHit.Nothing;
            IsMouseDown = false;
            InMove = false;
            IsMouseDown2 = false;
            InMove2 = false;
            InDownMove = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            ObjectHit = GetMouseHit(e.Location);
            if (ObjectHit == MouseHit.Primary) {
                IsMouseDown = true;
                OldOffset = PrefixToOffset();
            }
            else if (ObjectHit == MouseHit.Secondary) {
                IsMouseDown2 = true;
                OldOffset2 = PrefixToOffset(false);
            }
            DownLocation = CursorLocation;

            Invalidate();
            base.OnMouseDown(e);
        }
        private MouseHit GetMouseHit(Point CurrentLocation) {
            if (SecondUnitVisible == true) {
                if ((CurrentLocation.Y >= Offset.Y) && (CurrentLocation.Y < (Offset.Y + UnitHeight))) {
                    return MouseHit.Primary;
                }
                else if ((CurrentLocation.Y >= Offset.Y + (int)(UnitHeight * 1.5f)) && (CurrentLocation.Y < (Offset.Y + (int)(UnitHeight * 2.5f)))) {
                    return MouseHit.Secondary;
                }
                else { return MouseHit.Nothing; }
            }
            else {
                if (linkedNumericControl != null) {
                    if (linkedNumericControl.IsMetric == true) { return MouseHit.Primary; }
                    else {
                        if (linkedNumericControl.IsSecondaryMetric == true) { return MouseHit.Secondary; }
                        else { return MouseHit.Nothing; }
                    }
                }
                else { return MouseHit.Nothing; }
            }
        }
        private Color GetColorMouseAction(Rectangle Bound) {
            switch (GetBoundState(Bound)) {
                case ButtonState.MouseOver:
                    return hovercolor;
                case ButtonState.MouseDown:
                    return downcolor;
                case ButtonState.Normal:
                    return ForeColor;
            }
            return ForeColor;
        }
        protected override void OnMouseWheel(MouseEventArgs e) {

            if (linkedNumericControl != null) {
                if (SecondUnitVisible == true) {
                    if (ModifierKeys.HasFlag(Keys.Control)) {
                        PrefixTick(e.Delta, false);
                    }
                    else {
                        PrefixTick(e.Delta);
                    }
                }
                else {
                    if (linkedNumericControl.IsMetric == true) {
                        PrefixTick(e.Delta);
                    }
                    else if (linkedNumericControl.IsSecondaryMetric == true) {
                        PrefixTick(e.Delta,false);
                    }
                }
            }
        }
        private void PrefixTick(int Delta, bool IsPrimary = true) {
            if (linkedNumericControl != null) {
                if (IsPrimary == true) {
                    if (Delta > 0) {
                        MouseIndex++;
                    }
                    else {
                        MouseIndex--;
                    }
                    if (MouseIndex < PrefixMin) { MouseIndex = PrefixMin; }
                    if (MouseIndex > PrefixMax) { MouseIndex = PrefixMax; }
                    linkedNumericControl.Prefix = (NumericTextbox.MetricPrefix)MouseIndex;
                    Invalidate();
                }
                else {
                    if (Delta > 0) {
                        MouseIndex2++;
                    }
                    else {
                        MouseIndex2--;
                    }
                    if (MouseIndex2 < PrefixMin) { MouseIndex2 = PrefixMin; }
                    if (MouseIndex2 > PrefixMax) { MouseIndex2 = PrefixMax; }
                    linkedNumericControl.SecondaryPrefix = (NumericTextbox.MetricPrefix)MouseIndex2;
                    Invalidate();
                }
            }
        }
        public enum TextAlign {
            Left = 0x01,
            Center = 0x00,
            Right = 0x02
        }
        private enum ButtonState {
            Normal = 0x00,
            MouseOver = 0x01,
            MouseDown = 0x02
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // PrefixScale
            // 
            this.Name = "PrefixScale";
            this.Load += new System.EventHandler(this.PrefixScale_Load);
            this.ResumeLayout(false);

        }

        private void PrefixScale_Load(object? sender, EventArgs e) {

        }
    }
}
