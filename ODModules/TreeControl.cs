using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using Handlers;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ComponentModel;

namespace ODModules {
    public class TreeControl : UserControl {

        #region Properties
        private List<TreeItem> nodes = new List<TreeItem>();
        [System.ComponentModel.Category("Behaviour")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<TreeItem> Nodes {
            get {
                return nodes;
            }
        }
        private Color _SelectedColor;
        [System.ComponentModel.Category("Appearance")]
        public Color SelectedColor {
            get {
                return _SelectedColor;
            }
            set {
                _SelectedColor = value;
                Invalidate();
            }
        }

        [System.ComponentModel.Category("Scrolling")]
        private int scrollItems = 3;
        public int ScrollItems {
            get {
                return scrollItems;
            }
            set {
                scrollItems = value;
            }
        }

        private int _VerScroll;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                return _VerScroll;
            }
            set {
                if (value < 0)
                    _VerScroll = 0;
                //else if (value > items.Count - 1)
                //_VerScroll = items.Count - 1;
                else
                    _VerScroll = value;

                //if (InSelection == true) {
                //    SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                //    SelectValuesList(SELTEST);
                //}
                Invalidate();
            }
        }
        int lines = 0;
        [System.ComponentModel.Category("Reporting")]
        public int Lines {
            get {
                return lines;
            }
        }

        #endregion
        int ScrollSize = 10;
        int GenericLine_Height = 0;
        int MaximumVerticalItems = 100;
        int LineTextOffset = 0;
        int Xscroll = 0;
        int ScrollXDifference = 0;

        private void DetermineLineCount() {
            ulong Count = 0;
            try {
                if (nodes.Count > 0) {
                    foreach (TreeItem Ti in nodes) {
                        Count++;
                        if (Ti.Expanded == true) {
                            Count += CaptureNodeCount(Ti);
                        }
                    }
                }
                lines = (int)Count;
            }
            catch {
                lines = nodes.Count;
            }
        }
        private ulong CaptureNodeCount(TreeItem? ParentItem) {
            ulong Count = 0;
            if (ParentItem == null) { return 0; }
            if (ParentItem.Nodes.Count == 0) { return 0; }
            foreach (TreeItem Ti in ParentItem.Nodes) {
                Count++;
                if (Ti.Expanded == true) {
                    Count += CaptureNodeCount(Ti);
                }
            }
            return Count;
        }
        private void RenderSetup(PaintEventArgs e) {

            DetermineLineCount();
            using (System.Drawing.Font GenericSize = new System.Drawing.Font(Font.FontFamily, 9.0f, Font.Style)) {
                ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
            }
            int sz_genx = (int)e.Graphics.MeasureString("0", Font).Width - 2;
            GenericLine_Height = (int)e.Graphics.MeasureString("0", Font).Height;

            int HorizontalScrollHeight = 0;
            if (ShowHorzScroll == true) { HorizontalScrollHeight = ScrollSize; }
            MaximumVerticalItems = (int)Math.Floor((this.Height - HorizontalScrollHeight) / (double)GenericLine_Height);
            //if (items.Count < MaximumVerticalItems) {
            //    _VerScroll = 0;
            //}
            //int LineOffset = 2;
            //InvokeScrollCheck();

            // if ((ColumnsTotalWidth * sz_genx) > this.Width) {

            //if (_AllowColumnSpanning == false) {
            //    ColumnsTotalWidth = MeasureColumns() + ScrollSize;
            //    if (ColumnsTotalWidth > this.Width) {
            //        ScrollXDifference = ColumnsTotalWidth - Width;
            //        // Xscroll = (int)((double)HorScroll / (double)100) * (ColumnsTotalWidth * sz_genx);
            //        Xscroll = (int)_HorScroll;
            //        ShowHorzScroll = true;
            //    }
            //    else {
            //        ScrollXDifference = 0;
            //        Xscroll = 0;
            //        _HorScroll = 0;
            //        ShowHorzScroll = false;
            //    }
            //}
            //else {
            //    ColumnsTotalWidth = MeasureColumnsExclusive();
            //    if ((_SpanColumn > -1) && (_SpanColumn < columns.Count)) {
            //        columns[_SpanColumn].Width = Width - ColumnsTotalWidth;
            //    }
            //    ScrollXDifference = 0;
            //    Xscroll = 0;
            //    _HorScroll = 0;
            //    ShowHorzScroll = false;
            //}
            LineTextOffset = 5 - (int)(((float)Xscroll / 100.0f) * (float)ScrollXDifference);

            //SetMaxScroll();
            //TileWidth = (int)e.Graphics.MeasureString("W", Font).Width * 6;
            //TilePadding = 3;
            //TilesPerLine = (int)Math.Floor((float)Width / (float)TileWidth);
            //TilesPerPage = (int)Math.Floor(((float)Height) / ((float)GenericLine_Height * TilePadding));
        }
        protected override void OnPaint(PaintEventArgs e) {
            RenderSetup(e);
            int CurrentLine = VerScroll;
            if (CurrentLine < 0) {
                CurrentLine = 0;
            }
            int CurrentStartingLine = 0;
            for (int Line = CurrentStartingLine; Line < MaximumVerticalItems + 1; Line++) {
               if (CurrentStartingLine < nodes.Count) {
                   if (CurrentLine < nodes.Count) {
                       Rectangle LineBounds = new Rectangle(0, ListLinePoint(Line), Width, GenericLine_Height);
                       RenderLine(e, nodes[CurrentLine], LineBounds);
                       CurrentLine++;
                   }
               }
            }

            //for (int Line = CurrentStartingLine; Line < MaximumVerticalItems + 1; Line++) {
            //    if (CurrentStartingLine < items.Count) {
            //        if (CurrentLine < items.Count) {
            //            Rectangle LineBounds = new Rectangle(0, ListLinePoint(Line), Width, GenericLine_Height);
            //            RenderLine(e, CurrentLine, LineBounds);
            //            CurrentLine++;
            //        }
            //    }
            //}
        }
        //private int DrawNode(PaintEventArgs e, TreeItem CurrentNode, int StartingLine, Rectangle LineBounds) {
        //    int LinesToDraw = StartingLine;
        //    if (CurrentNode.Nodes.Count > 0) {
        //        if (CurrentNode.Expanded == true) {
        //
        //        }
        //    }
        //}
        private void RenderLine(PaintEventArgs e, TreeItem Node, Rectangle BoundingRectangle) {
           // int LinePositionY = (int)(((float)BoundingRectangle.Height - (float)e.Graphics.MeasureString("0", Font).Height) / 2.0f) + BoundingRectangle.Y;
            //RenderLineColouring(e, CurrentLine, BoundingRectangle);
            RenderNodeText(e, Node, BoundingRectangle);


        }
        private void RenderNodeText(PaintEventArgs e, TreeItem Node, Rectangle TextRectangle) {
            Color ItemForeColor = Node.UseForeColor == true ? Node.ForeColor : ForeColor;
            using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                    e.Graphics.DrawString(Node.Text, Font, TxtBrush, TextRectangle, FormatFlags);
                }
            }
        }


        bool ShowVertScroll = true;
        bool ShowHorzScroll = true;
        Rectangle HorizontalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle HorizontalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF HorizontalScrollThumb = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF VerticalScrollThumb = new Rectangle(0, 0, 0, 0);
        int ScrollBarButtonSize = 0;


        private int ListLinePoint(int PositionY, PointLineCalcuation CalculationType = PointLineCalcuation.LineToPositiion) {
            if (CalculationType == PointLineCalcuation.LineToPositiion) {
                return (GenericLine_Height * PositionY);// + LineHeaderHeight;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLine) {
                return (int)Math.Floor((float)(PositionY) / (float)GenericLine_Height) + VerScroll;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLineWithoutScroll) {
                return (int)((float)(PositionY) / (float)GenericLine_Height);
            }
            else if (CalculationType == PointLineCalcuation.LineToPositiionScrollFactored) {
                return (GenericLine_Height * (PositionY - VerScroll));
            }
            else {
                return 0;
            }
        }
        private enum PointLineCalcuation {
            LineToPositiion = 0x00,
            PositionToLine = 0x01,
            PositionToLineWithoutScroll = 0x02,
            LineToPositiionScrollFactored = 0x03
        }
    }


    public class TreeObject {
        private bool selected = false;
        [System.ComponentModel.Category("Control")]
        public bool Selected { get => selected; set => selected = value; }
        private bool expanded = false;
        [System.ComponentModel.Category("Control")]
        public bool Expanded { get => expanded; set => expanded = value; }
        private string name = "";
        [System.ComponentModel.Category("Design")]
        public string Name { get => name; set => name = value; }
        private string text = "";
        [System.ComponentModel.Category("Appearance")]
        public string Text { get => text; set => text = value; }
        private Color foreColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ForeColor { get => foreColor; set => foreColor = value; }
        private Color backColor = Color.Transparent;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColor { get => backColor; set => backColor = value; }

        private bool useBackColor = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseBackColor { get => useBackColor; set => useBackColor = value; }
        private bool useForeColor = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseForeColor { get => useForeColor; set => useForeColor = value; }

        private object? tag;
        [System.ComponentModel.Category("Data")]
        public object? Tag { get => tag; set => tag = value; }
        [System.ComponentModel.Category("Appearance")]
        public bool Checked { get => ischecked; set => ischecked = value; }
        private bool ischecked = false;
    }
    public class TreeItem : TreeObject {
        private List<TreeItem> nodes = new List<TreeItem>();
        [System.ComponentModel.Category("Nodes")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<TreeItem> Nodes {
            get { return nodes; }
        }
        public TreeItem() {

        }
        public TreeItem(string Text) {
            this.Text = Text;
        }
    }
}
