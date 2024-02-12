using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Reflection;

namespace ODModules {
    class MenuStripColorTable : ToolStripRenderer {
        private Color ItemCheckedBackColor = Color.FromArgb(128, 128, 128, 128);
        [System.ComponentModel.Category("Appearance")]
        public Color ItemCheckedBackColorNorth {
            get { return ItemCheckedBackColor; }
            set {
                ItemCheckedBackColor = value;
            }
        }
        private Color ItemCheckedBackColor2 = Color.FromArgb(128, 128, 128, 128);
        [System.ComponentModel.Category("Appearance")]
        public Color ItemCheckedBackColorSouth {
            get { return ItemCheckedBackColor2; }
            set {
                ItemCheckedBackColor2 = value;
            }
        }
        private Color ItemSelectedBackColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedBackColorNorth {
            get { return ItemSelectedBackColor; }
            set {
                ItemSelectedBackColor = value;
            }
        }
        private Color ItemSelectedBackColor2 = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedBackColorSouth {
            get { return ItemSelectedBackColor2; }
            set {
                ItemSelectedBackColor2 = value;
            }
        }
        private Color stripItemSelectedBackColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color StripItemSelectedBackColorNorth {
            get { return stripItemSelectedBackColor; }
            set {
                stripItemSelectedBackColor = value;
            }
        }
        private Color itemSelectedForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedForeColor {
            get { return itemSelectedForeColor; }
            set {
                itemSelectedForeColor = value;
            }
        }
        private Color itemForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemForeColor {
            get { return itemForeColor; }
            set {
                itemForeColor = value;
            }
        }
        private Color stripItemSelectedBackColor2 = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color StripItemSelectedBackColorSouth {
            get { return stripItemSelectedBackColor2; }
            set {
                stripItemSelectedBackColor2 = value;
            }
        }
        private bool useNorthFadeIn = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseNorthFadeIn {
            get { return useNorthFadeIn; }
            set {
                useNorthFadeIn = value;
            }
        }
        private Color backColorFadeIn = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorNorthFadeIn {
            get { return backColorFadeIn; }
            set {
                backColorFadeIn = value;
            }
        }
        private Color BackColor = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorNorth {
            get { return BackColor; }
            set {
                BackColor = value;
            }
        }
        private Color BackColor2 = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorSouth {
            get { return BackColor2; }
            set {
                BackColor2 = value;
            }
        }
        private Color menuBackColor = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorNorth {
            get { return menuBackColor; }
            set {
                menuBackColor = value;
            }
        }
        private Color menuBackColor2 = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorSouth {
            get { return menuBackColor2; }
            set {
                menuBackColor2 = value;
            }
        }
        private Color menuBorderColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBorderColor {
            get { return menuBorderColor; }
            set {
                menuBorderColor = value;
            }
        }
        private Color menuSeparatorColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuSeparatorColor {
            get { return menuSeparatorColor; }
            set {
                menuSeparatorColor = value;
            }
        }
        private Color menuSymbolColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuSymbolColor {
            get { return menuSymbolColor; }
            set {
                menuSymbolColor = value;
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

        public MenuStripColorTable() {
           
        }

        [System.ComponentModel.Category("Appearance")]
        public bool ShowInsetShadow {
            get { return showInsetShadow; }
            set {
                showInsetShadow = value;
                //Invalidate();
            }
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e) {

            base.OnRenderItemBackground(e);
        }
        private Point GetAbsolutePoint(ToolStripItemRenderEventArgs e) {
            Point Current = new Point(0, 0);
            if (e.Item.GetType() == typeof(ToolStripMenuItem)) {
                ToolStripMenuItem Tmi = (ToolStripMenuItem)e.Item;
                Current = AddPoints(Tmi.Bounds.Location, Current);
                if (Tmi.Owner != null) {
                    Current = AddPoints(Tmi.Owner.PointToScreen(Tmi.Owner.Bounds.Location), Current);
                }
            }
            else if (e.Item.GetType() == typeof(ToolStripDropDownButton)) {
                ToolStripDropDownButton Tmi = (ToolStripDropDownButton)e.Item;
                Current = AddPoints(Tmi.Bounds.Location, Current);
                if (Tmi.Owner != null) {
                    Current = AddPoints(Tmi.Owner.PointToScreen(Tmi.Owner.Bounds.Location), Current);
                }
            }

            return Current;
        }
        private Point AddPoints(Point A, Point B) {
            return new Point(A.X + B.X, A.Y + B.Y);
        }
        private enum DrawSide {
            Down = 0x00,
            Right = 0x01
        }
        private DrawSide DetermineDrawSide(ToolStripItemRenderEventArgs e) {
            DrawSide DisplayDropDown = DrawSide.Down;
            if (e.Item.GetType() == typeof(ToolStripMenuItem)) {
                Point MenuItemLocation = GetAbsolutePoint(e);
                Point DropDownLocation = ((ToolStripMenuItem)e.Item).DropDown.Bounds.Location;
                if (MenuItemLocation.Y > DropDownLocation.Y) {
                    DisplayDropDown = DrawSide.Right;
                }
            }
            else if (e.Item.GetType() == typeof(ToolStripDropDownButton)) {

                Point MenuItemLocation = GetAbsolutePoint(e);
                Point MarginSize = new Point(e.Item.Margin.Left + e.Item.Margin.Right, e.Item.Margin.Top + e.Item.Margin.Bottom);
                Point DropDownLocation = AddPoints(((ToolStripDropDownButton)e.Item).DropDown.Bounds.Location, MarginSize);
                if (MenuItemLocation.Y > DropDownLocation.Y) {
                    DisplayDropDown = DrawSide.Right;
                }
            }
            return DisplayDropDown;
        }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {  //37 x16
                                                                                              //Debug.Print(e.Item.ToString() + ", " + e.Item.GetType().ToString());
            if (e.Item.Pressed) {
                if ((e.Item.Owner.GetType() == typeof(MenuStrip)) || (e.Item.Owner.GetType() == typeof(ToolStrip))) {
                    Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
                    using (SolidBrush NorthBrush = new SolidBrush(menuBackColor)) {
                        e.Graphics.FillRectangle(NorthBrush, ContextWindow);
                    }
                    DrawSide DisplayDropDown = DetermineDrawSide(e);
                    using (SolidBrush BorderBrush = new SolidBrush(menuBorderColor)) {
                        using (Pen BorderPen = new Pen(BorderBrush)) {
                            int Offset = (int)BorderPen.Width;
                            if (DisplayDropDown == DrawSide.Down) {
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y + ContextWindow.Height);
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                            }
                            else if (DisplayDropDown == DrawSide.Right) {
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                                e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y + ContextWindow.Height - Offset, ContextWindow.X + ContextWindow.Width, ContextWindow.Y + ContextWindow.Height - Offset);
                            }
                        }
                    }
                }
            }
            else if (e.Item.Selected) {
                if ((e.Item.Owner.GetType() == typeof(MenuStrip)) || (e.Item.Owner.GetType() == typeof(ToolStrip))) {
                    using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, stripItemSelectedBackColor, stripItemSelectedBackColor2, 90)) {
                        e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                    }
                    e.Item.ForeColor = itemSelectedForeColor;
                }
                else {
                    using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, ItemSelectedBackColor, ItemSelectedBackColor2, 90)) {
                        e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                    }
                    e.Item.ForeColor = itemSelectedForeColor;
                }
            }
            else {
                e.Graphics.FillRectangle(Brushes.Transparent, e.Item.ContentRectangle);
                e.Item.ForeColor = itemForeColor;
            }
            base.OnRenderMenuItemBackground(e);
        }
        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) {



            base.OnRenderToolStripContentPanelBackground(e);
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e) {


            base.OnRenderToolStripPanelBackground(e);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
            base.OnRenderToolStripBackground(e);
            //Debug.Print(e.ToolStrip.ToString());
            Rectangle ContextWindow = new Rectangle(0, 0, e.AffectedBounds.Width, e.AffectedBounds.Height);
            if ((ContextWindow.Width != 0) && (ContextWindow.Height != 0)) {
                if ((e.ToolStrip.GetType() == typeof(MenuStrip)) || (e.ToolStrip.GetType() == typeof(ToolStrip)) || (e.ToolStrip.GetType() == typeof(ToolStripOverflow))) {
                    using (LinearGradientBrush BackgroundBrush = new LinearGradientBrush(ContextWindow, BackColor, BackColor2, 90.0f)) {
                        e.Graphics.FillRectangle(BackgroundBrush, ContextWindow);
                    }
                    if (useNorthFadeIn == true) {
                        Rectangle FadeInRectanagle = new Rectangle(ContextWindow.X, ContextWindow.Y, ContextWindow.Width, ContextWindow.Height / 4);
                        using (LinearGradientBrush BackgroundBrush = new LinearGradientBrush(FadeInRectanagle, backColorFadeIn, Color.Transparent, 90.0f)) {
                            e.Graphics.FillRectangle(BackgroundBrush, FadeInRectanagle);
                        }
                    }
                }
                else if (e.ToolStrip.GetType() == typeof(ToolStripDropDownMenu)) {
                    using (LinearGradientBrush BackgroundBrush = new LinearGradientBrush(ContextWindow, menuBackColor, menuBackColor2, 90.0f)) {
                        if ((e.ConnectedArea.Width != 0) && (e.ConnectedArea.Height != 0)) {
                            using (SolidBrush NorthBrush = new SolidBrush(menuBackColor)) {
                                e.Graphics.FillRectangle(NorthBrush, e.ConnectedArea);
                            }
                        }
                        e.Graphics.FillRectangle(BackgroundBrush, ContextWindow);
                    }
                   
                    using (SolidBrush BorderBrush = new SolidBrush(menuBorderColor)) {
                        using (Pen BorderPen = new Pen(BorderBrush, 1)) {
                            int Offset = (int)BorderPen.Width;
                            int ParentOffset = 0;
                            Rectangle ParentRectangle = new Rectangle(0, 0, 0, 0);
                            if (((ToolStripDropDownMenu)e.ToolStrip).OwnerItem.OwnerItem == null) {
                                ParentOffset = ((ToolStripDropDownMenu)e.ToolStrip).OwnerItem.Width;
                                ParentRectangle = ((ToolStripDropDownMenu)e.ToolStrip).OwnerItem.Bounds;
                                //ToolStripDropDownMenu Ddmi = (ToolStripDropDownMenu)e.ToolStrip;
                                //ToolStripMenuItem Mbtn = (ToolStripMenuItem)Ddmi.OwnerItem;
                                //Debug.Print(((ToolStripDropDownButton)Ddmi.OwnerItem).DropDownDirection.ToString());
                                //Debug.Print(e.ToolStrip.PointToScreen(ParentRectangle.Location).ToString());
                                //Debug.Print(((ToolStripDropDownMenu)e.ToolStrip).DisplayRectangle.ToString());
                                //Debug.Print("");
                            }
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X + ParentOffset, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y + ContextWindow.Height - Offset, ContextWindow.X + ContextWindow.Width, ContextWindow.Y + ContextWindow.Height - Offset);
                        }
                    }
                    int ShadowSize = 2;
                    if (showInsetShadow) {
                        Rectangle ShadowRectangleTop = new Rectangle(ContextWindow.X, ContextWindow.Y, ContextWindow.Width, ShadowSize);
                        Rectangle ShadowRectangleBottom = new Rectangle(ContextWindow.X, ContextWindow.Y + ContextWindow.Height - ShadowSize, ContextWindow.Width, ShadowSize);
                        Rectangle ShadowRectangleLeft = new Rectangle(ContextWindow.X, ContextWindow.Y, ShadowSize, ContextWindow.Height);
                        Rectangle ShadowRectangleRight = new Rectangle(ContextWindow.X + ContextWindow.Width - ShadowSize, ContextWindow.Y, ShadowSize, ContextWindow.Height);
                        //using (LinearGradientBrush ShadowBrushTop = new LinearGradientBrush(ShadowRectangleTop, insetshadowColor, Color.Transparent, 90)) {
                        //    e.Graphics.FillRectangle(ShadowBrushTop, ShadowRectangleTop);
                        //}
                        //using (LinearGradientBrush ShadowBrushBottom = new LinearGradientBrush(ShadowRectangleBottom, insetshadowColor, Color.Transparent, 270)) {
                        //    e.Graphics.FillRectangle(ShadowBrushBottom, ShadowRectangleBottom);
                        //}
                        //using (LinearGradientBrush ShadowBrushLeft = new LinearGradientBrush(ShadowRectangleLeft, insetshadowColor, Color.Transparent, 0.0f)) {
                        //    e.Graphics.FillRectangle(ShadowBrushLeft, ShadowRectangleLeft);
                        //}
                        //using (LinearGradientBrush ShadowBrushRight = new LinearGradientBrush(ShadowRectangleRight, insetshadowColor, Color.Transparent, 180)) {
                        //    e.Graphics.FillRectangle(ShadowBrushRight, ShadowRectangleRight);
                        //}
                    }
                }
            }
            // Debug.Print(e.ToolStrip.ToString());


            // 
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
            base.OnRenderDropDownButtonBackground(e);
            if (e.Item.Pressed) {
                Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
                using (SolidBrush NorthBrush = new SolidBrush(menuBackColor)) {
                    e.Graphics.FillRectangle(NorthBrush, ContextWindow);
                }
                //  Debug.Print(e.Item..ToString());

                DrawSide DisplayDropDown = DetermineDrawSide(e);
                using (SolidBrush BorderBrush = new SolidBrush(menuBorderColor)) {
                    using (Pen BorderPen = new Pen(BorderBrush)) {
                        int Offset = (int)BorderPen.Width;
                        if (DisplayDropDown == DrawSide.Down) {
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                        }
                        else if (DisplayDropDown == DrawSide.Right) {
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y + ContextWindow.Height - Offset, ContextWindow.X + ContextWindow.Width, ContextWindow.Y + ContextWindow.Height - Offset);
                        }
                    }
                }
            }
            else if (e.Item.Selected) {
                using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, ItemSelectedBackColor, ItemSelectedBackColor2, 90)) {
                    e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                }
                e.Item.ForeColor = itemSelectedForeColor;
            }
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
            base.OnRenderSeparator(e);
            Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
            using (SolidBrush BorderBrush = new SolidBrush(menuSeparatorColor)) {
                using (Pen BorderPen = new Pen(BorderBrush)) {
                    int Offset = (int)BorderPen.Width;
                    if (e.Vertical) {
                        e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                    }
                    else {
                        e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                    }
                }
            }

        }
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            base.OnRenderButtonBackground(e);
            if (e.Item.GetType() == typeof(ToolStripButton)) {
                if (((ToolStripButton)e.Item).Checked) {
                    using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, ItemCheckedBackColor, ItemCheckedBackColor2, 90)) {
                        e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                    }
                }
            }
            if (e.Item.Pressed) {
                Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
                using (SolidBrush NorthBrush = new SolidBrush(menuBackColor)) {
                    e.Graphics.FillRectangle(NorthBrush, ContextWindow);
                }
            }
            else if (e.Item.Selected) {
                using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, ItemSelectedBackColor, ItemSelectedBackColor2, 90)) {
                    e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                }
                e.Item.ForeColor = itemSelectedForeColor;
            }
            base.OnRenderButtonBackground(e);
        }
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) {
            Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
            if (e.Item.Pressed) {

                using (SolidBrush NorthBrush = new SolidBrush(menuBackColor)) {
                    e.Graphics.FillRectangle(NorthBrush, ContextWindow);
                }
                DrawSide DisplayDropDown = DetermineDrawSide(e);
                using (SolidBrush BorderBrush = new SolidBrush(menuBorderColor)) {
                    using (Pen BorderPen = new Pen(BorderBrush)) {
                        int Offset = (int)BorderPen.Width;
                        if (DisplayDropDown == DrawSide.Down) {
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y, ContextWindow.X + ContextWindow.Width - Offset, ContextWindow.Y + ContextWindow.Height);
                            e.Graphics.DrawLine(BorderPen, ContextWindow.X, ContextWindow.Y, ContextWindow.X + ContextWindow.Width, ContextWindow.Y);
                        }
                        else if (DisplayDropDown == DrawSide.Right) {

                        }

                    }
                }
            }
            else if (e.Item.Selected) {
                using (LinearGradientBrush SelectMenuItem = new System.Drawing.Drawing2D.LinearGradientBrush(e.Item.ContentRectangle, ItemSelectedBackColor, ItemSelectedBackColor2, 90)) {
                    e.Graphics.FillRectangle(SelectMenuItem, e.Item.ContentRectangle);
                }
                e.Item.ForeColor = itemSelectedForeColor;
            }
            int DpiSize = (int)(5.0f * (e.Graphics.DpiX / 96.0f));
            DrawArrow(e.Graphics, new Rectangle(e.Item.Size.Width - (2 * DpiSize), 0, DpiSize, e.Item.Size.Height), ArrowDirection.Down, DpiSize);
            base.OnRenderSplitButtonBackground(e);
        }
        private void DrawArrow(Graphics g, Rectangle ArrowBounds, ArrowDirection Direction, int Size, bool DrawLineAbove = false) {
            Rectangle ArrowRectangle = ScaleAndCenter(ArrowBounds, Size);
            using (SolidBrush ActionBrush = new SolidBrush(menuSymbolColor)) {
                if (Direction == ArrowDirection.Up) {
                    int Push = (ArrowRectangle.Height / 8);
                    g.FillPolygon(ActionBrush, new Point[]{
                        new Point(ArrowRectangle.Left,ArrowRectangle.Bottom - Push),
                        new Point(ArrowRectangle.Right, ArrowRectangle.Bottom- Push),
                        new Point(ArrowRectangle.X + (ArrowRectangle.Width / 2), ArrowRectangle.Top+ Push)});
                }
                else if (Direction == ArrowDirection.Down) {
                    int Push = (ArrowRectangle.Height / 8);
                    g.FillPolygon(ActionBrush, new Point[]{
                        new Point(ArrowRectangle.Left,ArrowRectangle.Top + Push),
                        new Point(ArrowRectangle.Right, ArrowRectangle.Top+ Push),
                        new Point(ArrowRectangle.X + (ArrowRectangle.Width / 2), ArrowRectangle.Bottom- Push)});
                    if (DrawLineAbove == true) {
                        using (Pen ActionPen = new Pen(ActionBrush)) {
                            g.DrawLine(ActionPen, ArrowRectangle.X, ArrowRectangle.Y - 6, ArrowRectangle.X, ArrowRectangle.Y - 6);
                        }
                    }
                }
            }
        }
        private Rectangle ScaleAndCenter(Rectangle BoundingRectangle, int Size) {
            Point Center = new Point(BoundingRectangle.X + ((BoundingRectangle.Width - Size) / 2), BoundingRectangle.Y + ((BoundingRectangle.Height - Size) / 2));
            return new Rectangle(Center, new Size(Size, Size));
        }
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(e.ArrowRectangle.Location, e.ArrowRectangle.Size);
            r.Inflate(-e.ArrowRectangle.Size.Width / 4, -e.ArrowRectangle.Size.Height / 4);
            using (SolidBrush ActionBrush = new SolidBrush(menuSymbolColor)) {
                using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                    if (e.Direction == ArrowDirection.Right) {
                        e.Graphics.DrawLines(ActionPen, new Point[]{
        new Point(r.Left, r.Top),
        new Point(r.Right, r.Top + r.Height /2),
        new Point(r.Left, r.Top+ r.Height)});
                    }
                    else if (e.Direction == ArrowDirection.Left) {
                        e.Graphics.DrawLines(ActionPen, new Point[]{
        new Point(r.Right, r.Top),
        new Point(r.Left, r.Top + r.Height /2),
        new Point(r.Right, r.Top+ r.Height)});
                    }
                    else {
                        int DpiSize = (int)(5.0f * (e.Graphics.DpiX / 96.0f));
                        DrawArrow(e.Graphics, e.ArrowRectangle, e.Direction, DpiSize);
                    }
                    //else if (e.Direction == ArrowDirection.Up) {
                    //    //                e.Graphics.DrawLines(ActionPen, new Point[]{
                    //    //new Point(r.Left, r.Bottom),
                    //    //new Point(r.Left  + r.Top),
                    //    //new Point(r.Left + r.Height, r.Bottom)});
                    //    e.ArrowColor = menuSymbolColor;
                    //    base.OnRenderArrow(e);
                    //}
                    //else if (e.Direction == ArrowDirection.Down) {
                    //    //                e.Graphics.DrawLines(ActionPen, new Point[]{
                    //    //new Point(r.Left, r.Top),
                    //    //new Point(r.Left  + r.Bottom),
                    //    //new Point(r.Left + r.Height, r.Top)});
                    //    e.ArrowColor = menuSymbolColor;
                    //    base.OnRenderArrow(e);
                    //}
                }
            }
            //if (e.Item.Selected) {
            //    e.ArrowColor = itemSelectedForeColor;
            //}
            //else {
            //    e.ArrowColor = itemForeColor;
            //}
        }
        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
            base.OnRenderGrip(e);
            if (e.GripStyle == ToolStripGripStyle.Visible) {
                int XPad = 0; int YPad = 0;
                if (e.GripDisplayStyle == ToolStripGripDisplayStyle.Horizontal) {
                    XPad = 4;
                }
                else { YPad = 4; }
                Rectangle ContextWindow = new Rectangle(e.GripBounds.X + XPad, e.GripBounds.Y + YPad, e.GripBounds.Width - (2 * XPad), e.GripBounds.Height - (2 * YPad));
                using (HatchBrush GripBrush = new HatchBrush(HatchStyle.Percent20, MenuBorderColor, Color.Transparent)) {
                    e.Graphics.FillRectangle(GripBrush, ContextWindow);
                }
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(e.ImageRectangle.Location, e.ImageRectangle.Size);
            //r.Inflate(-e.ImageRectangle.Size.Width / 3, -e.ImageRectangle.Size.Height / 3);
            using (SolidBrush ActionBrush = new SolidBrush(menuSymbolColor)) {
                using (Pen ActionPen = new Pen(ActionBrush, 1)) {
                    //            e.Graphics.DrawLines(ActionPen, new Point[]{
                    //new Point(r.Left, r.Bottom - r.Height /2),
                    //new Point(r.Left + r.Width /3,  r.Bottom),
                    //new Point(r.Right, r.Top)});
                    float BX = r.X;
                    float BY = r.Y;
                    float BW = r.Width;
                    float BH = r.Height;
                    e.Graphics.DrawLines(ActionPen, new PointF[]{
                        new PointF(BX + 0.19f * BW, BY + 0.54f * BW),
                        new PointF(BX + 0.37f * BW, BY + 0.72f * BH),
                        new PointF(BX + 0.82f * BW, BY + 0.28f * BH)
                    });
                    //e.Graphics.DrawLine(ActionPen, BX + 0.19f * BW, BY + 0.54f * BW, BX + 0.37f * BW, BY + 0.72f * BH);
                    //e.Graphics.DrawLine(ActionPen, BX + 0.37f * BW, BY + 0.72f * BW, BX + 0.82f * BW, BY + 0.28f * BH);
                }
            }
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) {
            // base.OnRenderOverflowButtonBackground(e);
            //Debug.Print(e.Item.GetType().ToString());
            e.Graphics.InterpolationMode = InterpolationMode.Bicubic;
            Rectangle ContextWindow = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
            if ((ContextWindow.Width != 0) && (ContextWindow.Height != 0)) {
                using (LinearGradientBrush BackgroundBrush = new LinearGradientBrush(ContextWindow, BackColor, BackColor2, 90.0f)) {
                    e.Graphics.FillRectangle(BackgroundBrush, ContextWindow);
                }
            }
            //new Rectangle(e.Item.arr.Location, e.ArrowRectangle.Size);
            //using (SolidBrush ActionBrush = new SolidBrush(menuSymbolColor)) {
            //    using (Pen ActionPen = new Pen(ActionBrush, 2)) {
            //        e.Graphics.DrawLines(ActionPen, new Point[]{
            //        new Point(r.Right, r.Top),
            //        new Point(r.Left, r.Top + r.Height /2),
            //        new Point(r.Right, r.Top+ r.Height)});
            //    }
            //}
            int DpiSize = (int)(5.0f * (e.Graphics.DpiX / 96.0f));
            Size ArrowRectangle = new Size(ContextWindow.Width - 2, ContextWindow.Width - 2);
            Rectangle r = new Rectangle(new Point(ContextWindow.X + ((ContextWindow.Width - ArrowRectangle.Width) / 2), ContextWindow.Y + ContextWindow.Height - ArrowRectangle.Height - 2), ArrowRectangle);
            DrawArrow(e.Graphics, r, ArrowDirection.Down, DpiSize, true);

        }
    }
}
