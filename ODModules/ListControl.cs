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
using System.Reflection.Metadata.Ecma335;
using static Handlers.ConversionHandler;
using System.Collections;

namespace ODModules {
    public class ListControl : UserControl {

        //private bool mwe = false;
        ////private int MessageBlockSize = 0;
        //private int ScrollMax = 100;
        //private decimal ScrollPercent = 1;
        //private bool ScrollEdit = false;

        public delegate void DropDownClickedHandler(object sender, DropDownClickedEventArgs e);
        public delegate void ItemCheckedChangedHandler(object sender, ItemCheckedChangeEventArgs e);
        [Category("Item Actions")]
        public event DropDownClickedHandler? DropDownClicked;
        [Category("Item Actions")]
        public event ItemCheckedChangedHandler? ItemCheckedChanged;

        [Category("Items")]
        public event ItemClickedHandler? ItemClicked;
        [Category("Items")]
        public event ItemClickedHandler? ItemRightClicked;
        [Category("Items")]
        public event ItemClickedHandler? ItemMiddleClicked;

        public delegate void ItemClickedHandler(object sender, ListItem Item, int Index, Rectangle ItemBounds);


        //public event CommandEnteredEventHandler? CommandEntered;

        //public delegate void CommandEnteredEventHandler();

        public event ValueChangedEventHandler? ValueChanged;

        public delegate void ValueChangedEventHandler();

        public ListControl() {
            InitializeComponent();
            AllowMouseWheel = true;
            ShadowColor = Color.FromArgb(128, 0, 0, 0);
            ColumnColor = Color.LightGray;
            SelectedColor = Color.SkyBlue;
            GridlineColor = Color.LightGray;
            SelectionColor = Color.Gray;
            RowColor = Color.LightGray;
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            ScrollOutofBounds = new System.Timers.Timer(10);
            this.MouseDown += LineInterface_MouseDown;
            this.MouseMove += LineInterface_MouseMove;
            this.MouseUp += LineInterface_MouseUp;
            this.MouseWheel += LineInterface_MouseWheel;
            this.MouseClick += LineInterface_MouseClick;
            this.KeyPress += LineInterface_KeyPress;
            this.KeyUp += LineInterface_KeyUp;
            this.KeyDown += LineEditingInterface_KeyDown;
            this.Resize += LineEditingInterface_Resize;
            ScrollOutofBounds.Elapsed += ScrollOutofBounds_Elapsed;
            //Columns.Add(new Column("Test 1", 100));
            //Columns[0].UseItemBackColor = true;
            //Columns.Add(new Column("Test 2", 100));
            //Columns[1].ColumnAlignment = ColumnTextAlignment.Center;
            //Columns[1].DisplayType = ColumnDisplayType.Checkbox;
            //Columns.Add(new Column("Test 3", 60));
            //for (int i = 0; i < 100; i++) {
            //    ListItem li = new ListItem("a" + i.ToString());
            //    li.SubItems.Add(new ListSubItem("A very very long piece of data" + i.ToString()));
            //    li.SubItems.Add(new ListSubItem("C" + i.ToString()));
            //    li.BackColor = Color.FromKnownColor((KnownColor)i);

            //    Items.Add(li);
            //}
        }
        #region Functions
        public void ScaleColumnWidths() {
           float Scaler =  this.DeviceDpi / 96.0f;
           
            foreach (Column column in Columns) {
                column.Width = (int)((float)column.Width * Scaler);
            }
        }
        public void FocusToLast() {
            if (IsFirstTime == true) { Invalidate(); }
            if (items.Count - 1 > MaximumVerticalItems) {
                VerScroll = items.Count - 1 - MaximumVerticalItems;
            }
            else {
                VerScroll = 0;
            }
            Invalidate();
        }
        public void FocusToLines(int Offset) {
            if (items.Count - 1 > Offset) {
                VerScroll = items.Count - 1 - Offset;
            }
            else {
                VerScroll = 0;
            }
            Invalidate();
        }
        public void LineMove(bool MoveDown) {
            if (MoveDown == false) {
                if (useLocalList == true) {
                    for (int i = 0; i <= Items.Count - 1; i++) {
                        if (Items[i].Selected == true) {
                            if (i > 0) {
                                ListItem old_val = Items[i - 1];
                                ListItem new_val = Items[i];
                                Items[i - 1] = new_val;
                                Items[i] = old_val;
                            }
                        }
                    }
                }
                else {
                    if (externalItems == null) { return; }
                    for (int i = 0; i <= externalItems.Count - 1; i++) {
                        if (externalItems[i].Selected == true) {
                            if (i > 0) {
                                ListItem old_val = externalItems[i - 1];
                                ListItem new_val = externalItems[i];
                                externalItems[i - 1] = new_val;
                                externalItems[i] = old_val;
                            }
                        }
                    }
                }
            }
            else {
                if (useLocalList == true) {
                    for (int i = Items.Count - 1; i >= 0; i += -1) {
                        if (Items[i].Selected == true) {
                            if (i < Items.Count - 1) {
                                ListItem old_val = Items[i + 1];
                                ListItem new_val = Items[i];
                                Items[i] = old_val;
                                Items[i + 1] = new_val;
                            }
                        }
                    }
                }
                else {
                    if (externalItems == null) { return; }
                    for (int i = externalItems.Count - 1; i >= 0; i += -1) {
                        if (externalItems[i].Selected == true) {
                            if (i < externalItems.Count - 1) {
                                ListItem old_val = externalItems[i + 1];
                                ListItem new_val = externalItems[i];
                                externalItems[i] = old_val;
                                externalItems[i + 1] = new_val;
                            }
                        }
                    }
                }
            }
            Invalidate();
        }
        public void LineFlipSelected() {
            List<ListItem> temp = new List<ListItem>();
            List<int> post = new List<int>();
            if (useLocalList == true) {
                for (int i = Items.Count - 1; i >= 0; i += -1) {
                    if (Items[i].Selected == true) {
                        temp.Add(CloneListItem(Items[i]));
                        post.Add(i);
                    }
                }
                for (int j = 0; j < post.Count; j++) {
                    if (Items[post[j]].Selected == true) {
                        int inv = post.Count - 1 - j;
                        Items[post[j]] = temp[inv];
                    }
                }
            }
            Invalidate();
        }
        private ListItem CloneListItem(ListItem Item) {
            ListItem NewItem = new ListItem();
            NewItem.Text = Item.Text;
            NewItem.BackColor = Item.BackColor;
            NewItem.Checked = Item.Checked;
            NewItem.ForeColor = Item.ForeColor;
            NewItem.Tag = Item.Tag;
            NewItem.Name = Item.Name;
            for(int i=0;i < Item.SubItems.Count; i++) {
                NewItem.SubItems.Add(CloneSubItem(Item.SubItems[i]));
            }
            return NewItem;
        }
        private ListSubItem CloneSubItem(ListSubItem Item) {
            ListSubItem NewItem = new ListSubItem();
            NewItem.Text = Item.Text;
            NewItem.BackColor = Item.BackColor;
            NewItem.Checked = Item.Checked;
            NewItem.ForeColor = Item.ForeColor;
            NewItem.Tag = Item.Tag;
            NewItem.Name = Item.Name;

            return NewItem;
        }
        public void LineRemoveSelected() {
            //int sellines = SelectedItems();
            //while (sellines > 0) {
            //    int i;
            //    for (i = 0; i <= Items.Count - 1; i++) {
            //        if (i <= Items.Count - 1) {
            //            ListItem sKey = Items.ElementAt(i);
            //            if (sKey.Selected == true) {
            //                Items.Remove(sKey);
            //            }
            //        }
            //    }
            //    sellines = SelectedItems();
            //}
            if (useLocalList == true) {
                for (int i = Items.Count - 1; i >= 0; i--) {
                    if (Items[i].Selected == true) {
                        Items[i].SubItems.Clear();
                        Items.RemoveAt(i);
                    }
                }
            }
            else {
                if (externalItems == null) { return; }
                for (int i = externalItems.Count - 1; i >= 0; i--) {
                    if (externalItems[i].Selected == true) {
                        externalItems[i].SubItems.Clear();
                        externalItems.RemoveAt(i);
                    }
                }
            }
            Invalidate();
        }
        public void LineRemoveAll() {
            if (useLocalList == true) {
                for (int i = Items.Count - 1; i >= 0; i--) {
                    Items[i].SubItems.Clear();
                    Items.RemoveAt(i);
                }
            }
            else {
                if (externalItems == null) { return; }
                for (int i = externalItems.Count - 1; i >= 0; i--) {
                    externalItems[i].SubItems.Clear();
                    externalItems.RemoveAt(i);
                }
            }

            Invalidate();
        }
        public void LineSelectAll() {
            for (int i = 0; i < CurrentItems.Count; i++) {
                CurrentItems[i].Selected = true;
            }
            Invalidate();
        }
        public void LineRemove(int Index, bool RefreshInterface = true) {
            if (useLocalList == true) {
                if (Index < 0) { return; }
                if (Index >= Items.Count) { return; }
                Items[Index].SubItems.Clear();
                Items.RemoveAt(Index);
            }
            else {
                if (ExternalItems == null) { return; }
                if (Index < 0) { return; }
                if (Index >= ExternalItems.Count) { return; }
                ExternalItems[Index].SubItems.Clear();
                ExternalItems.RemoveAt(Index);
            }
            if (RefreshInterface == true) { Invalidate(); }
        }
        public void LineInsert(int Index, ListItem? Item, bool AtIndex = true) {
            if (Item == null) { return; }
            if (Index < 0) {
                if (useLocalList == true) {
                    Items.Add(Item);
                }
                else {
                    if (externalItems == null) { return; }
                    externalItems.Add(Item);
                }
            }
            else {
                if (AtIndex == true) {
                    if (useLocalList == true) {
                        if (Index < Items.Count) {
                            Items.Insert(Index, Item);
                        }
                        else {
                            Items.Add(Item);
                        }
                    }
                    else {
                        if (externalItems == null) { return; }
                        if (Index < externalItems.Count) {
                            externalItems.Insert(Index, Item);
                        }
                        else {
                            externalItems.Add(Item);
                        }
                    }
                }
                else {
                    if (useLocalList == true) {
                        if (Index + 1 < Items.Count) {
                            Items.Insert(Index + 1, Item);
                        }
                        else {
                            Items.Add(Item);
                        }
                    }
                    else {
                        if (externalItems == null) { return; }
                        if (Index + 1 < externalItems.Count) {
                            externalItems.Insert(Index + 1, Item);
                        }
                        else {
                            externalItems.Add(Item);
                        }
                    }
                }
            }
        }
        #endregion 
        #region Support Functions and Methods
        private int LastSelectionCount = 0;
        public int SelectedItems() {
            try {
                int cnt = 0;
                bool bol = false;
                for (int i = 0; i < CurrentItems.Count; i++) {
                    if (CurrentItems[i].Selected == true) {
                        cnt += 1;
                        if (bol == false) {
                            _IndexCount = i;
                            _CurrentString = CurrentItems[i].Text;
                            bol = true;
                        }
                    }
                }
                _SelectionCount = cnt;
                _LineCount = CurrentItems.Count;
                if (LastSelectionCount != cnt) {
                    LastSelectionCount = cnt;
                    ValueChanged?.Invoke();
                }
                return cnt;
            }
            catch {
                _SelectionCount = 0;
                return 0;
            }
        }
        private byte[] SpanBytes(byte[] ByteArray) {
            byte[] Output = ByteArray;
            if (Output.Count() < 4) {
                for (int i = 0; i <= 4 - Output.Count() - 1; i++) {
                    byte newItem = 9;
                    Array.Resize(ref Output, Output.Length + 1);
                    Output[Output.Length - 1] = newItem;
                }
            }
            return Output;
        }
        #endregion
        #region Properties
        private List<ListItem> items = new List<ListItem>();
        private List<ListItem>? filtereditems;
        [System.ComponentModel.Category("List Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ListItem> Items {
            get {
                return items;
            }
        }
        [System.ComponentModel.Category("List Data")]
        public List<ListItem> CurrentItems {
            get {
                if (useLocalList == true) {
                    if (filter == null) {
                        return items;
                    }
                    else if (filter == "") {
                        return items;
                    }
                    else if (filter.Length == 0) {
                        return items;
                    }
                    else {
                        if (filtereditems == null) {
                            return items;
                        }
                        else {
                            return filtereditems;
                        }
                    }
                }
                else {
                    if (externalItems == null) {
                        return items;
                    }
                    else {
                        return externalItems;
                    }
                }
            }
        }
        private List<ListItem>? externalItems = null;
        [System.ComponentModel.Category("List Data")]
        public List<ListItem>? ExternalItems {
            get {
                return externalItems;
            }
            set {
                externalItems = value;
                if (useLocalList == false) {
                    VerScroll = 0;
                }
                Invalidate();
            }
        }
        private bool useLocalList = true;
        [System.ComponentModel.Category("List Data")]
        public bool UseLocalList {
            get {
                return useLocalList;
            }
            set {
                useLocalList = value;
                ReevaluateList();
            }
        }
        private List<Column> columns = new List<Column>();
        [System.ComponentModel.Category("List Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<Column> Columns {
            get {
                return columns;
            }
        }
        private string filter;
        [System.ComponentModel.Category("Filtering")]
        public string Filter {
            get {
                return filter;
            }
            set {
                filter = value;
                ReevaluateList();
            }
        }
        private int filterColumn;
        [System.ComponentModel.Category("Filtering")]
        public int FilterColumn {
            get {
                return filterColumn;
            }
            set {
                filterColumn = value;
                ReevaluateList();
            }
        }
        private void ReevaluateList() {
            if (filterColumn <= columns.Count) {
                if (filterColumn == 0) {
                    filtereditems = items.Where(x => x.Text == filter).ToList();
                }
                else {
                    int Index = filterColumn - 1;
                    filtereditems = items.Where(x => x.SubItems[Index].Text.ToLower().Contains(filter.ToLower())).ToList();
                }
            }
            Invalidate();
        }


        private Color _ShadowColor;
        [System.ComponentModel.Category("Appearance")]
        public Color ShadowColor {
            get {
                return _ShadowColor;
            }
            set {
                _ShadowColor = value;
                Invalidate();
            }
        }
        private Color markerBorderColor = Color.LimeGreen;
        [System.ComponentModel.Category("Appearance")]
        public Color MarkerBorderColor {
            get {
                return markerBorderColor;
            }
            set {
                markerBorderColor = value;
                Invalidate();
            }
        }
        private Color markerFillColor = Color.FromArgb(100, 50, 205, 50);
        [System.ComponentModel.Category("Appearance")]
        public Color MarkerFillColor {
            get {
                return markerFillColor;
            }
            set {
                markerFillColor = value;
                Invalidate();
            }
        }
        private Color _ColumnColor = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnColor {
            get {
                return _ColumnColor;
            }
            set {
                _ColumnColor = value;
                Invalidate();
            }
        }
        private Color _ColumnForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnForeColor {
            get {
                return _ColumnForeColor;
            }
            set {
                _ColumnForeColor = value;
                Invalidate();
            }
        }
        private Color _ColumnLineColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color ColumnLineColor {
            get {
                return _ColumnLineColor;
            }
            set {
                _ColumnLineColor = value;
                Invalidate();
            }
        }
        private Color dropDownMouseOver = Color.LightGray;
        [System.ComponentModel.Category("Appearance")]
        public Color DropDownMouseOver {
            get {
                return dropDownMouseOver;
            }
            set {
                dropDownMouseOver = value;
                Invalidate();
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
        private Color _SelectionColor;
        [System.ComponentModel.Category("Appearance")]
        public Color SelectionColor {
            get {
                return _SelectionColor;
            }
            set {
                _SelectionColor = value;
                Invalidate();
            }
        }
        private Color dropDownMouseDown = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color DropDownMouseDown {
            get {
                return dropDownMouseDown;
            }
            set {
                dropDownMouseDown = value;
                Invalidate();
            }
        }
        private Color _GridlineColor;
        [System.ComponentModel.Category("Appearance")]
        public Color GridlineColor {
            get {
                return _GridlineColor;
            }
            set {
                _GridlineColor = value;
                Invalidate();
            }
        }
        private Color _RowColor;
        [System.ComponentModel.Category("Appearance")]
        public Color RowColor {
            get {
                return _RowColor;
            }
            set {
                _RowColor = value;
                Invalidate();
            }
        }
        private Color _ScrollBarNorth = Color.DarkTurquoise;
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarNorth {
            get {
                return _ScrollBarNorth;
            }
            set {
                _ScrollBarNorth = value;
                Invalidate();
            }
        }
        private Color _ScrollBarSouth = Color.DeepSkyBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarSouth {
            get {
                return _ScrollBarSouth;
            }
            set {
                _ScrollBarSouth = value;
                Invalidate();
            }
        }
        private Color _ScrollBarMouseDown = Color.FromArgb(64, 0, 0, 0);
        [System.ComponentModel.Category("Appearance")]
        public Color ScrollBarMouseDown {
            get {
                return _ScrollBarMouseDown;
            }
            set {
                _ScrollBarMouseDown = value;
                Invalidate();
            }
        }
        private bool _AllowMouseWheel;
        [System.ComponentModel.Category("Control")]
        public bool AllowMouseWheel {
            get {
                return _AllowMouseWheel;
            }
            set {
                _AllowMouseWheel = value;
            }
        }
        private bool _ShowGrid;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowGrid {
            get {
                return _ShowGrid;
            }
            set {
                _ShowGrid = value;
                Invalidate();
            }
        }
        private bool _ShowRowColors;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowRowColors {
            get {
                return _ShowRowColors;
            }
            set {
                _ShowRowColors = value;
                Invalidate();
            }
        }
        private bool _AllowColumnSpanning;
        [System.ComponentModel.Category("Appearance")]
        public bool AllowColumnSpanning {
            get {
                return _AllowColumnSpanning;
            }
            set {
                _AllowColumnSpanning = value;
                Invalidate();
            }
        }
        private int _SpanColumn;
        [System.ComponentModel.Category("Appearance")]
        public int SpanColumn {
            get {
                return _SpanColumn;
            }
            set {
                _SpanColumn = value;
                Invalidate();
            }
        }
        private int _VerScroll = 0;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                if (_VerScroll < 0) {
                    return 0;
                }
                return _VerScroll;
            }
            set {
                if (value < 0)
                    _VerScroll = 0;
                else if (value > CurrentItems.Count - 1)
                    _VerScroll = CurrentItems.Count - 1;
                else
                    _VerScroll = value;

                if (InSelection == true) {
                    SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                    SelectValuesList(SELTEST);
                }
                Invalidate();
            }
        }
        private void InvokeScrollCheck() {
            if (_VerScroll > CurrentItems.Count - 1) {
                if (CurrentItems.Count > 0) {
                    _VerScroll = CurrentItems.Count - 1;
                }
                else { _VerScroll = 0; }
            }

        }
        private decimal _VerScrollMax;
        [System.ComponentModel.Category("Scrolling")]
        public decimal VerScrollMax {
            get {
                return _VerScrollMax;
            }
        }
        private int _SelectionCount;
        [System.ComponentModel.Category("Values")]
        public int SelectionCount {
            get {
                return _SelectionCount;
            }
        }
        private int _LineCount;
        [System.ComponentModel.Category("Values")]
        public int LineCount {
            get {
                return _LineCount;
            }
        }
        private int _IndexCount;
        [System.ComponentModel.Category("Values")]
        public int SelectedIndex {
            get {
                return _IndexCount;
            }
        }
        private bool showMarker = false;
        [System.ComponentModel.Category("Analysis")]
        public bool ShowMarker {
            get {
                return showMarker;
            }
            set {
                showMarker = value;
                Invalidate();
            }
        }
        private MarkerStyleType markerStyle = MarkerStyleType.Highlight;
        [System.ComponentModel.Category("Analysis")]
        public MarkerStyleType MarkerStyle {
            get {
                return markerStyle;
            }
            set {
                markerStyle = value;
                Invalidate();
            }
        }
        private int lineMarkerIndex;
        [System.ComponentModel.Category("Analysis")]
        public int LineMarkerIndex {
            get {
                return lineMarkerIndex;
            }
            set {
                if (value < 0) { lineMarkerIndex = 0; }
                else if (value > CurrentItems.Count) { lineMarkerIndex = CurrentItems.Count - 1; }
                else { lineMarkerIndex = value; }
                Invalidate();
            }
        }
        private string _CurrentString = "";
        [System.ComponentModel.Category("Values")]
        public string CurrentString {
            get {
                return _CurrentString;
            }
        }
        private decimal _HorScroll;
        [System.ComponentModel.Category("Scrolling")]
        public decimal HorScroll {
            get {
                return _HorScroll;
            }
            set {
                if (value < 0)
                    _HorScroll = 0;
                else if (value > 100)
                    _HorScroll = 100;
                else
                    _HorScroll = value;
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
        [System.ComponentModel.Category("Highlights")]
        int highlightalpha = 128;
        int highlightsidealpha = 78;
        public int HighlightStrength {
            get {
                return highlightalpha;
            }
            set {
                if (value < 20)
                    highlightalpha = 20;
                else if (value > 255)
                    highlightalpha = 255;
                else
                    highlightalpha = value;
                if (value < 50) {
                    highlightsidealpha = value + 50;
                }
                else {
                    highlightsidealpha = value - 50;
                }
                Invalidate();
            }
        }

        #endregion
        #region Render Setup
        int Offset = 5;
        int LineTextOffset = 0;
        int ColumnsTotalWidth = 0;
        int Xscroll = 0;
        int LineHeaderHeight = 0;
        int GenericLine_Height = 0;
        int ScrollSize = 10;
        int MaximumVerticalItems = 100;
        int TileWidth = 10;
        float TilePadding = 3;
        int TilesPerLine = 10;
        int TilesPerPage = 10;
        bool IsFirstTime = true;
        int ScrollXDifference = 0;
        private void RenderSetup(PaintEventArgs e) {
            using (System.Drawing.Font GenericSize = new System.Drawing.Font(Font.FontFamily, 9.0f, Font.Style)) {
                ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
            }
            int sz_genx = (int)e.Graphics.MeasureString("0", Font).Width - 2;
            GenericLine_Height = (int)e.Graphics.MeasureString("0", Font).Height;

            int HorizontalScrollHeight = 0;
            if (ShowHorzScroll == true) { HorizontalScrollHeight = ScrollSize; }
            MaximumVerticalItems = (int)Math.Floor((this.Height - LineHeaderHeight - HorizontalScrollHeight) / (double)GenericLine_Height);
            if (CurrentItems.Count < MaximumVerticalItems) {
                _VerScroll = 0;
            }
            int LineOffset = 2;
            LineHeaderHeight = (int)GenericLine_Height + (LineOffset * 3);
            InvokeScrollCheck();
            IsFirstTime = false;
            // if ((ColumnsTotalWidth * sz_genx) > this.Width) {

            if (_AllowColumnSpanning == false) {
                ColumnsTotalWidth = MeasureColumns() + ScrollSize;
                if (ColumnsTotalWidth > this.Width) {
                    ScrollXDifference = ColumnsTotalWidth - Width;
                    // Xscroll = (int)((double)HorScroll / (double)100) * (ColumnsTotalWidth * sz_genx);
                    Xscroll = (int)_HorScroll;
                    ShowHorzScroll = true;
                }
                else {
                    ScrollXDifference = 0;
                    Xscroll = 0;
                    _HorScroll = 0;
                    ShowHorzScroll = false;
                }
            }
            else {
                ColumnsTotalWidth = MeasureColumnsExclusive();
                if ((_SpanColumn > -1) && (_SpanColumn < columns.Count)) {
                    columns[_SpanColumn].Width = Width - ColumnsTotalWidth;
                }
                ScrollXDifference = 0;
                Xscroll = 0;
                _HorScroll = 0;
                ShowHorzScroll = false;
            }
            LineTextOffset = 0 - (int)(((float)Xscroll / 100.0f) * (float)ScrollXDifference);

            SetMaxScroll();
            TileWidth = (int)e.Graphics.MeasureString("W", Font).Width * 6;
            TilePadding = 3;
            TilesPerLine = (int)Math.Floor((float)Width / (float)TileWidth);
            TilesPerPage = (int)Math.Floor(((float)Height) / ((float)GenericLine_Height * TilePadding));
        }
        #endregion
        #region Render
        protected override void OnPaint(PaintEventArgs e) {
            RenderList(e);
        }
        private Color AlphaDark(Color Input, int Alpha) {
            double AlphaReduce = (255 - Alpha) / (double)255;
            int AR = (int)Math.Floor((double)Input.R * AlphaReduce);
            int AG = (int)Math.Floor((double)Input.G * AlphaReduce);
            int AB = (int)Math.Floor((double)Input.B * AlphaReduce);
            return Color.FromArgb(AR, AG, AB);
        }
        private void RenderList(PaintEventArgs e) {
            try {
                RenderSetup(e);
                int CurrentStartingLine = 0;

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                int CurrentLine = VerScroll;
                if (CurrentLine < 0) {
                    CurrentLine = 0;
                }
                int WindowSize = Width - ScrollSize;
                //if (CurrentStartingLine < CurrentItems.Count) {
                for (int Line = CurrentStartingLine; Line < MaximumVerticalItems + 1; Line++) {
                    if (CurrentStartingLine < CurrentItems.Count) {
                        if (CurrentLine < CurrentItems.Count) {
                            Rectangle LineBounds = new Rectangle(0, ListLinePoint(Line), Width, GenericLine_Height);

                            RenderLine(e, CurrentLine, LineBounds);
                            RenderMarker(e, LineBounds, CurrentLine);
                            CurrentLine++;
                        }
                    }
                    if (_ShowGrid) {
                        // if (Line != CurrentStartingLine) {
                        using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                            using (Pen LinePed = new Pen(LineBrush)) {
                                e.Graphics.DrawLine(LinePed, new Point(0, ListLinePoint(Line + 1)), new Point(WindowSize, ListLinePoint(Line + 1)));
                            }
                        }
                        //}
                    }
                }
                //}
                if (InSelection == true && ShiftKey == false && CtrlKey == false) {
                    RenderSelectionRectangle(e, new Point(SelectionStart.X, ListLinePoint(SelectedItemstart)), SelectionEnd);
                }
                RenderHeader(e, LineHeaderHeight);
                RenderScrollBar(e);
            }
            catch { }
        }
        #endregion

        #region Element Rendering
        private void RenderMarker(PaintEventArgs e, Rectangle BoundingRectangle, int CurrentLine) {
            if (showMarker == false) { return; }
            if (CurrentLine == lineMarkerIndex) {
                if (markerStyle == MarkerStyleType.Highlight) {
                    using (SolidBrush FillBrsh = new SolidBrush(markerFillColor)) {
                        e.Graphics.FillRectangle(FillBrsh, BoundingRectangle);
                    }
                    Rectangle MarkerBorder = new Rectangle(BoundingRectangle.X, BoundingRectangle.Y, BoundingRectangle.Width, BoundingRectangle.Height - 1);
                    using (SolidBrush BrdBrsh = new SolidBrush(markerBorderColor)) {
                        using (Pen BrdPen = new Pen(BrdBrsh)) {
                            e.Graphics.DrawRectangle(BrdPen, MarkerBorder);
                        }
                    }
                }
                else if (markerStyle == MarkerStyleType.Underline) {
                    using (SolidBrush BrdBrsh = new SolidBrush(markerBorderColor)) {
                        using (Pen BrdPen = new Pen(BrdBrsh)) {
                            int LineVert = BoundingRectangle.Y + BoundingRectangle.Height - 1;
                            e.Graphics.DrawLine(BrdPen, new Point(BoundingRectangle.X, LineVert), new Point(BoundingRectangle.X + BoundingRectangle.Width, LineVert));
                        }
                    }
                }
                else if ((markerStyle == MarkerStyleType.Pointer) || (markerStyle == MarkerStyleType.PointerWithUnderline) || (markerStyle == MarkerStyleType.PointerWithBox)) {
                    int ArrowSize = BoundingRectangle.Height / 2;
                    Point[] TrianglePoints = {
                        new Point(BoundingRectangle.X,BoundingRectangle.Y),
                        new Point(BoundingRectangle.X + ArrowSize,BoundingRectangle.Y + ArrowSize),
                        new Point(BoundingRectangle.X,BoundingRectangle.Y + BoundingRectangle.Height)
                    };
                    using (SolidBrush FillBrsh = new SolidBrush(markerFillColor)) {
                        e.Graphics.FillPolygon(FillBrsh, TrianglePoints);
                    }
                    using (SolidBrush BrdBrsh = new SolidBrush(markerBorderColor)) {
                        using (Pen BrdPen = new Pen(BrdBrsh)) {
                            e.Graphics.DrawPolygon(BrdPen, TrianglePoints);
                        }
                    }
                    if (markerStyle == MarkerStyleType.PointerWithUnderline) {
                        using (SolidBrush BrdBrsh = new SolidBrush(markerBorderColor)) {
                            using (Pen BrdPen = new Pen(BrdBrsh)) {
                                int LineVert = BoundingRectangle.Y + BoundingRectangle.Height - 1;
                                e.Graphics.DrawLine(BrdPen, new Point(BoundingRectangle.X, LineVert), new Point(BoundingRectangle.X + BoundingRectangle.Width, LineVert));
                            }
                        }
                    }
                    else if (markerStyle == MarkerStyleType.PointerWithBox) {
                        using (SolidBrush BrdBrsh = new SolidBrush(markerBorderColor)) {
                            Rectangle MarkerBorder = new Rectangle(BoundingRectangle.X, BoundingRectangle.Y, BoundingRectangle.Width, BoundingRectangle.Height - 1);
                            using (Pen BrdPen = new Pen(BrdBrsh)) {
                                e.Graphics.DrawRectangle(BrdPen, MarkerBorder);
                            }
                        }
                    }
                }
            }
        }
        private void RenderLine(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            int LinePositionY = (int)(((float)BoundingRectangle.Height - (float)e.Graphics.MeasureString("0", Font).Height) / 2.0f) + BoundingRectangle.Y;
            RenderLineColouring(e, CurrentLine, BoundingRectangle);
            int Inc = 0;
            int Xpos = LineTextOffset;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    RenderLineBackItem(e, Xpos, LinePositionY, i, CurrentLine);
                    RenderLineItem(e, Xpos, LinePositionY, i, CurrentLine);

                    Xpos += columns[i].Width;
                    Inc++;
                }
            }
        }
        private void RenderLineBackItem(PaintEventArgs e, int Xpos, int LinePositionY, int Column, int Item) {
            if (Column == 0) {
                Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
                if (columns[Column].UseItemBackColor == true) {
                    Color ItemColor = CurrentItems[Item].BackColor;
                    if (CurrentItems[Item].Selected == true) {
                        ItemColor = SelectedColor;
                    }
                    using (SolidBrush TxtBrush = new SolidBrush(ItemColor)) {
                        e.Graphics.FillRectangle(TxtBrush, ItemRectangle);
                    }
                }
            }
            else {
                if ((CurrentItems[Item].SubItems.Count > 0) && (Column - 1 < CurrentItems[Item].SubItems.Count)) {
                    Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
                    Rectangle TextRectangle = new Rectangle(ItemRectangle.X + Offset, ItemRectangle.Y, ItemRectangle.Width - (2 * Offset), ItemRectangle.Height);
                    if (columns[Column].UseItemBackColor == true) {
                        Color ItemColor = CurrentItems[Item].SubItems[Column - 1].BackColor;
                        if (CurrentItems[Item].Selected == true) {
                            ItemColor = SelectedColor;
                        }
                        using (SolidBrush TxtBrush = new SolidBrush(ItemColor)) {
                            e.Graphics.FillRectangle(TxtBrush, ItemRectangle);
                        }
                    }
                }
            }
        }
        int BoxTrim = 2;
        private void RenderLineItem(PaintEventArgs e, int Xpos, int LinePositionY, int Column, int Item) {
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            int TextOffset = Offset;
            TextOffset += RenderCheckBox(e, Xpos, LinePositionY, Column, Item);
            if ((columns[Column].DisplayType == ColumnDisplayType.Text) || (columns[Column].DisplayType == ColumnDisplayType.CheckboxWithText)) {
                RenderItemText(e, Xpos, LinePositionY, TextOffset, Column, Item);
            }
            else if (columns[Column].DisplayType == ColumnDisplayType.LineCount) {
                RenderCounter(e, Xpos, LinePositionY, TextOffset, Column, Item);
            }
            else if (columns[Column].DisplayType == ColumnDisplayType.DropDown) {
                RenderDropDown(e, Xpos, LinePositionY, TextOffset, Column, Item);
            }
        }
        private void RenderItemText(PaintEventArgs e, int Xpos, int LinePositionY, int TextOffset, int Column, int Item) {
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            if (columns[Column].ItemAlignment != ItemTextAlignment.None) {
                Color ItemForeColor = ForeColor;
                Rectangle TextRectangle = Rectangle.Empty;
                string TextString = "";
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                    if (columns[Column].ItemAlignment == ItemTextAlignment.Left) { FormatFlags.Alignment = StringAlignment.Near; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Center) { FormatFlags.Alignment = StringAlignment.Center; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Right) { FormatFlags.Alignment = StringAlignment.Far; }
                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    if (Column == 0) {
                        TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                        if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].ForeColor; }
                        TextString = CurrentItems[Item].Text;
                        using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                            e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                        }
                    }
                    else {
                        if ((CurrentItems[Item].SubItems.Count > 0) && (Column - 1 < CurrentItems[Item].SubItems.Count)) {
                            TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                            if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].SubItems[Column - 1].ForeColor; }
                            TextString = CurrentItems[Item].SubItems[Column - 1].Text;
                        }
                    }
                    using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                        e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                    }
                }
            }
        }
        private void RenderDropDown(PaintEventArgs e, int Xpos, int LinePositionY, int TextOffset, int Column, int Item) {
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            if (columns[Column].ItemAlignment != ItemTextAlignment.None) {
                Color ItemForeColor = ForeColor;
                Rectangle TextRectangle = Rectangle.Empty;
                string TextString = "";
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {

                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    if (Column == 0) {
                        TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                        if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].ForeColor; }
                        TextString = CurrentItems[Item].Text;
                        using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                            e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                        }
                    }
                    else {
                        if ((CurrentItems[Item].SubItems.Count > 0) && (Column - 1 < CurrentItems[Item].SubItems.Count)) {
                            TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                            if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].SubItems[Column - 1].ForeColor; }
                            TextString = CurrentItems[Item].SubItems[Column - 1].Text;
                        }
                    }
                    if (_AllowColumnSpanning) {
                        if (Column == Columns.Count - 1) {
                            if (ShowVertScroll == true) {
                                TextRectangle = new Rectangle(TextRectangle.X, TextRectangle.Y, TextRectangle.Width - VerticalScrollBar.Width, TextRectangle.Height);
                            }
                        }
                    }
                    Rectangle TextModifiedRectangle = TextRectangle;
                    int TextWidth = TextRectangle.Width - GenericLine_Height;
                    Rectangle ArrowRectangle = new Rectangle(TextRectangle.X + TextWidth, TextRectangle.Y, GenericLine_Height, TextRectangle.Width);
                    // if (columns[Column].ItemAlignment == ItemTextAlignment.Right) {
                    if (columns[Column].DropDownVisible == true) {
                        if (columns[Column].DropDownRight == true) {
                            TextModifiedRectangle = new Rectangle(TextRectangle.X + GenericLine_Height, TextRectangle.Y, TextWidth, TextRectangle.Height);
                            ArrowRectangle = new Rectangle(TextRectangle.X, TextRectangle.Y, GenericLine_Height, TextRectangle.Width);
                        }
                        else {
                            TextModifiedRectangle = new Rectangle(TextRectangle.X, TextRectangle.Y, TextWidth, TextRectangle.Height);
                        }
                    }
                    if (columns[Column].ItemAlignment == ItemTextAlignment.Right) {
                        FormatFlags.Alignment = StringAlignment.Far;
                    }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Center) {
                        FormatFlags.Alignment = StringAlignment.Center;
                    }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Left) {
                        FormatFlags.Alignment = StringAlignment.Near;
                    }

                    if ((TextRectangle.Contains(CurrentMousePosition)) && (Math.Abs(SelectionDelta.Y) < GenericLine_Height)) {
                        if (MouseIsDown == true) {
                            ItemForeColor = dropDownMouseDown;
                        }
                        else {
                            ItemForeColor = dropDownMouseOver;
                        }
                    }
                    using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                        if (TextModifiedRectangle.Width > 1) {
                            e.Graphics.DrawString(TextString, Font, TxtBrush, TextModifiedRectangle, FormatFlags);
                        }
                        if (columns[Column].DropDownVisible == true) {
                            int Ax = (int)((float)GenericLine_Height * 0.333f);
                            int Bx = (int)((float)GenericLine_Height * 0.666f);
                            int Cx = (int)((float)GenericLine_Height * 0.5f);
                            Point[] TrianglePoints = {
                                new Point(ArrowRectangle.X + Ax, ArrowRectangle.Y + Ax),
                                new Point(ArrowRectangle.X + Bx, ArrowRectangle.Y + Ax),
                                new Point(ArrowRectangle.X + Cx, ArrowRectangle.Y + Bx)
                            };
                            e.Graphics.FillPolygon(TxtBrush, TrianglePoints);
                        }
                    }
                }
            }
        }
        private void RenderCounter(PaintEventArgs e, int Xpos, int LinePositionY, int TextOffset, int Column, int Item) {
            Rectangle ItemRectangle = new Rectangle(Xpos, LinePositionY, columns[Column].Width, (int)GenericLine_Height);
            if (columns[Column].ItemAlignment != ItemTextAlignment.None) {
                Color ItemForeColor = ForeColor;
                Rectangle TextRectangle = Rectangle.Empty;
                int ItemOff = Item + columns[Column].CountOffset;
                string TextString = ItemOff.ToString();
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                    if (columns[Column].ItemAlignment == ItemTextAlignment.Left) { FormatFlags.Alignment = StringAlignment.Near; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Center) { FormatFlags.Alignment = StringAlignment.Center; }
                    else if (columns[Column].ItemAlignment == ItemTextAlignment.Right) { FormatFlags.Alignment = StringAlignment.Far; }
                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    if (Column == 0) {
                        TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                        if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].ForeColor; }
                        using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                            e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                        }
                    }
                    else {
                        if ((CurrentItems[Item].SubItems.Count > 0) && (Column - 1 < CurrentItems[Item].SubItems.Count)) {
                            TextRectangle = new Rectangle(ItemRectangle.X + TextOffset, ItemRectangle.Y, ItemRectangle.Width - TextOffset - Offset, ItemRectangle.Height);
                            if (columns[Column].UseItemForeColor == true) { ItemForeColor = CurrentItems[Item].SubItems[Column - 1].ForeColor; }
                        }
                    }
                    using (SolidBrush TxtBrush = new SolidBrush(ItemForeColor)) {
                        e.Graphics.DrawString(TextString, Font, TxtBrush, TextRectangle, FormatFlags);
                    }
                }
            }
        }
        private void RenderLineColouring(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            if (ShowRowColors == true) {
                if (CurrentLine % 2 == 0) {
                    using (SolidBrush AlternLineColor = new SolidBrush(RowColor)) {
                        e.Graphics.FillRectangle(AlternLineColor, BoundingRectangle);
                    }
                }
            }
            if (CurrentItems[CurrentLine].Selected == true) {
                using (SolidBrush SelectedLine = new SolidBrush(SelectedColor)) {
                    e.Graphics.FillRectangle(SelectedLine, BoundingRectangle);
                }
            }
        }
        #endregion
        #region Render Checkbox
        private int RenderCheckBox(PaintEventArgs e, int Xpos, int LinePositionY, int Column, int Item) {
            if ((columns[Column].DisplayType == ColumnDisplayType.Checkbox) || (columns[Column].DisplayType == ColumnDisplayType.CheckboxWithText)) {
                Rectangle CheckBoxRectangle = GetCheckBoxRectangle(Xpos, LinePositionY, columns[Column].Width, columns[Column].DisplayType == ColumnDisplayType.Checkbox ? false : true);
                using (SolidBrush BoxBrush = new SolidBrush(ForeColor)) {
                    using (Pen BoxPen = new Pen(BoxBrush)) {
                        BoxPen.Alignment = PenAlignment.Center;
                        bool DrawTick = false;
                        if (Column == 0) {
                            using (GraphicsPath Path = RoundedRectangle(CheckBoxRectangle, 2)) {
                                e.Graphics.DrawPath(BoxPen, Path);
                            }
                            DrawTick = CurrentItems[Item].Checked;
                        }
                        else {
                            if ((CurrentItems[Item].SubItems.Count > 0) && (Column - 1 < CurrentItems[Item].SubItems.Count)) {
                                using (GraphicsPath Path = RoundedRectangle(CheckBoxRectangle, 2)) {
                                    e.Graphics.DrawPath(BoxPen, Path);
                                }
                                DrawTick = CurrentItems[Item].SubItems[Column - 1].Checked;
                            }
                        }
                        if (DrawTick == true) {
                            BoxPen.Width = 1.6f;
                            float BX = CheckBoxRectangle.X;
                            float BY = CheckBoxRectangle.Y;
                            float BW = CheckBoxRectangle.Width;
                            float BH = CheckBoxRectangle.Height;
                            e.Graphics.DrawLines(BoxPen, new PointF[]{
                                new PointF(BX + 0.19f * BW, BY + 0.54f * BW),
                                new PointF(BX + 0.37f * BW, BY + 0.72f * BH),
                                new PointF(BX + 0.82f * BW, BY + 0.28f * BH)
                            });
                        }
                    }
                }
                return CheckBoxRectangle.Width + BoxTrim;
            }
            return 0;
        }
        private Rectangle GetCheckBoxRectangle(int Xpos, int Ypos, int ColumnWidth, bool WithText = false) {
            int BoxWidthHeight = (int)GenericLine_Height - (2 * BoxTrim);
            if (WithText == true) {
                return new Rectangle(Xpos + Offset, Ypos + BoxTrim, BoxWidthHeight, BoxWidthHeight);
            }
            else {
                int CentreBox = (int)(((float)ColumnWidth - (float)BoxWidthHeight) / 2.0f);
                return new Rectangle(Xpos + CentreBox, Ypos + BoxTrim, BoxWidthHeight, BoxWidthHeight);
            }
        }
        #endregion
        #region Render Selection
        private void RenderSelectionRectangle(PaintEventArgs e, Point StartPoint, Point EndPoint) {
            Point PBX = SelectionBounds(SelectionStart.X, EndPoint.X);
            Point PBY = SelectionBounds(SelectionStart.Y, EndPoint.Y);
            Color SelectionFillColor = Color.FromArgb(100, SelectionColor.R, SelectionColor.G, SelectionColor.B);
            using (SolidBrush SelectionFillBrush = new SolidBrush(SelectionFillColor)) {
                e.Graphics.FillRectangle(SelectionFillBrush, PBX.X, PBY.X, PBX.Y, PBY.Y);
            }
            using (SolidBrush SelectionBorderBrush = new SolidBrush(SelectionColor)) {
                using (Pen SelectionBorderPen = new Pen(SelectionBorderBrush)) {
                    e.Graphics.DrawRectangle(SelectionBorderPen, PBX.X, PBY.X, PBX.Y, PBY.Y);
                }
            }
        }
        #endregion

        #region Render Other
        public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius) {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0) {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        private void RenderStretchToll(PaintEventArgs e, Point StartPoint, Point EndPoint) {
            int TICK = 5;
            Point PBX = SelectionBounds(SelectionStart.X, EndPoint.X);
            Point PBY = SelectionBounds(SelectionStart.Y, EndPoint.Y);
            int SX = PBX.X;
            int SY = PBY.X;
            int SW = PBX.Y;
            int SH = PBY.Y;
            Color SCOL = Color.FromArgb(100, SelectionColor.R, SelectionColor.G, SelectionColor.B);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SelectionColor), 1), SX, SY, SX, SY + SH);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SelectionColor), 1), SX - TICK, SY, SX + TICK, SY);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SelectionColor), 1), SX - TICK, SY + SH, SX + TICK, SY + SH);
            string VAL = System.Convert.ToString(AbsMovement());
            Size STRSZ = new Size((int)e.Graphics.MeasureString(VAL, Font).Width, (int)e.Graphics.MeasureString(VAL, Font).Height);
            Point MIDPNT = new Point(SX + TICK, (int)((double)SY + ((double)SH / (double)2) - ((double)STRSZ.Height / (double)2)));
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(MIDPNT, STRSZ));
            e.Graphics.DrawString(VAL, Font, new SolidBrush(ForeColor), MIDPNT);
        }
        #endregion 
        #region Render Header
        private void RenderHeader(PaintEventArgs e, int HeaderHeight) {
            using (SolidBrush HeaderBackBrush = new SolidBrush(ColumnColor)) {
                e.Graphics.FillRectangle(HeaderBackBrush, 0, 0, this.Width, HeaderHeight);
            }
            int Xpos = LineTextOffset;
            int Inc = 0;
            using (SolidBrush HeaderTextBrush = new SolidBrush(_ColumnForeColor)) {
                using (StringFormat FormatFlags = StringFormat.GenericTypographic) {
                    FormatFlags.Trimming = StringTrimming.EllipsisCharacter;
                    for (int i = 0; i < columns.Count; i++) {
                        if (columns[i].Visible) {
                            if (columns[i].ColumnAlignment != ColumnTextAlignment.None) {
                                if (columns[i].ColumnAlignment == ColumnTextAlignment.Left) { FormatFlags.Alignment = StringAlignment.Near; }
                                else if (columns[i].ColumnAlignment == ColumnTextAlignment.Center) { FormatFlags.Alignment = StringAlignment.Center; }
                                else if (columns[i].ColumnAlignment == ColumnTextAlignment.Right) { FormatFlags.Alignment = StringAlignment.Far; }
                                e.Graphics.DrawString(columns[i].Text, Font, HeaderTextBrush, new Rectangle(Xpos + Offset, 2, columns[i].Width - (2 * Offset), (int)GenericLine_Height), FormatFlags);
                            }
                            if (Inc != 0) {
                                using (SolidBrush LineBrush = new SolidBrush(_ColumnLineColor)) {
                                    using (Pen LinePed = new Pen(LineBrush)) {
                                        e.Graphics.DrawLine(LinePed, new Point(Xpos, 0), new Point(Xpos, HeaderHeight));
                                    }
                                }
                                if (_ShowGrid) {
                                    using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                                        using (Pen LinePed = new Pen(LineBrush)) {
                                            e.Graphics.DrawLine(LinePed, new Point(Xpos, HeaderHeight), new Point(Xpos, Height));
                                        }
                                    }
                                }
                            }
                            Xpos += columns[i].Width;
                            Inc++;
                        }
                    }
                }
            }
            using (SolidBrush LineBrush = new SolidBrush(_ColumnLineColor)) {
                using (Pen LinePed = new Pen(LineBrush)) {
                    e.Graphics.DrawLine(LinePed, new Point(Xpos, 0), new Point(Xpos, HeaderHeight));
                }
            }
            if (_ShowGrid) {
                using (SolidBrush LineBrush = new SolidBrush(_GridlineColor)) {
                    using (Pen LinePed = new Pen(LineBrush)) {
                        e.Graphics.DrawLine(LinePed, new Point(Xpos, HeaderHeight), new Point(Xpos, Height));
                    }
                }
            }
        }
        #endregion 
        #region Render Scrollbars
        bool ShowVertScroll = true;
        bool ShowHorzScroll = true;
        Rectangle HorizontalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle HorizontalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF HorizontalScrollThumb = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBar = new Rectangle(0, 0, 0, 0);
        Rectangle VerticalScrollBounds = new Rectangle(0, 0, 0, 0);
        RectangleF VerticalScrollThumb = new Rectangle(0, 0, 0, 0);
        int ScrollBarButtonSize = 0;
        private void RenderScrollBar(PaintEventArgs e) {
            Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
            if (ShowVertScroll == true) {
                VerticalScrollBar = new Rectangle(Width - ScrollSize, LineHeaderHeight, ScrollSize, Height - LineHeaderHeight);

                if (ShowHorzScroll == true) { VerticalScrollBar.Height -= ScrollSize; }

                using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, VerticalScrollBar);
                }
                RenderVerticalBar(e);
            }
            if (ShowHorzScroll == true) {
                HorizontalScrollBar = new Rectangle(0, Height - ScrollSize, Width, ScrollSize);
                if (ShowVertScroll == true) { HorizontalScrollBar.Width -= ScrollSize; }
                using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, HorizontalScrollBar);
                }
                RenderHorizontalBar(e);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(HorizontalScrollBar.X, HorizontalScrollBar.Y), new Point(HorizontalScrollBar.Width, HorizontalScrollBar.Y));
                    }
                }
            }
            if ((ShowVertScroll == true) && (ShowHorzScroll == true)) {
                Rectangle Spacer = new Rectangle(Width - ScrollSize, Height - ScrollSize, ScrollSize, ScrollSize);
                using (SolidBrush HeaderBackBrush = new SolidBrush(BorderLineColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, Spacer);
                }
            }
        }
        private void RenderVerticalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(VerticalScrollBar, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                ScrollBarButtonSize = ScrollSize;
                VerticalScrollBounds = new Rectangle(VerticalScrollBar.X, VerticalScrollBar.Y + ScrollBarButtonSize, VerticalScrollBar.Width, VerticalScrollBar.Height - (2 * ScrollBarButtonSize));
                if (CurrentItems.Count > 1) {
                    float ViewableItems = ((float)(MaximumVerticalItems) / 2.0f) / (float)CurrentItems.Count;
                    if (CurrentItems.Count < MaximumVerticalItems) {
                        ViewableItems = 1;
                    }
                    float ThumbHeight = ViewableItems * VerticalScrollBounds.Height;
                    if (ThumbHeight < ScrollBarButtonSize * 2) {
                        ThumbHeight = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)(CurrentItems.Count-1)) + VerticalScrollBounds.Y;// + ScrollSize;
                    VerticalScrollThumb = new RectangleF(VerticalScrollBounds.X, ScrollBounds, VerticalScrollBar.Width, ThumbHeight);
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollBounds);
                }
                Rectangle Button = new Rectangle(VerticalScrollBar.X, LineHeaderHeight, ScrollBarButtonSize, ScrollBarButtonSize);
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y + Button.Height), new Point(Button.X + Button.Width, Button.Y + Button.Height));
                        Button.Y = VerticalScrollBar.Height + LineHeaderHeight - Button.Height;
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y), new Point(Button.X + Button.Width, Button.Y));
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(VerticalScrollBar.X, VerticalScrollBar.Y), new Point(VerticalScrollBar.X, VerticalScrollBar.Y + VerticalScrollBar.Height));
                    }
                }
            }
        }
        private void RenderHorizontalBar(PaintEventArgs e) {
            using (LinearGradientBrush HeaderForeBrush = new LinearGradientBrush(Bounds, _ScrollBarNorth, _ScrollBarSouth, 90.0f)) {
                //e.Graphics.FillRectangle(HeaderForeBrush, VerticalBar);
                ScrollBarButtonSize = ScrollSize;
                HorizontalScrollBounds = new Rectangle(HorizontalScrollBar.X + ScrollBarButtonSize, HorizontalScrollBar.Y, HorizontalScrollBar.Width - (2 * ScrollBarButtonSize), HorizontalScrollBar.Height);
                //if (View == DIR_VIEW.ListLine) {
                if (columns.Count > 0) {
                    float WidthOverCurrent = 1;
                    if (ColumnsTotalWidth > 0) {
                        WidthOverCurrent = ColumnsTotalWidth;
                    }
                    float ViewableLines = (float)Width / (WidthOverCurrent);
                    float ThumbWidth = ViewableLines * HorizontalScrollBounds.Width;
                    if (ThumbWidth < ScrollBarButtonSize * 2) {
                        ThumbWidth = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (HorizontalScrollBounds.Width - ThumbWidth) * ((float)_HorScroll / 100.0f) + HorizontalScrollBounds.X;
                    HorizontalScrollThumb = new RectangleF(ScrollBounds, HorizontalScrollBounds.Y, ThumbWidth, HorizontalScrollBar.Height);
                    e.Graphics.FillRectangle(HeaderForeBrush, HorizontalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, HorizontalScrollBounds);
                }
                // }
                //Buttons
                Rectangle Button = new Rectangle(0, HorizontalScrollBar.Y, ScrollBarButtonSize, ScrollBarButtonSize);
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X + Button.Width, Button.Y), new Point(Button.X + Button.Width, Button.Y + Button.Height));
                        Button.X = HorizontalScrollBar.Width - Button.Width;
                        e.Graphics.FillRectangle(HeaderForeBrush, Button);
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(Button.X, Button.Y), new Point(Button.X, Button.Y + Button.Height));
                    }
                }
            }
        }
        #endregion 
        #region Scroll Handling
        private System.Timers.Timer ScrollOutofBounds;
        private void SetMaxScroll() {
            _VerScrollMax = CurrentItems.Count + 1;
        }
        private int MeasureColumns() {
            int CurrentMax = 0;
            foreach (Column Col in columns) {
                if (Col.Visible == true) { CurrentMax += Col.Width; }
            }
            return CurrentMax;
        }
        private int MeasureColumnsExclusive() {
            int CurrentMax = 0;
            int i = 0;
            foreach (Column Col in columns) {
                if (i != _SpanColumn) {
                    if (Col.Visible == true) { CurrentMax += Col.Width; }
                }
                i++;
            }
            return CurrentMax;
        }
        private void ScrollOutofBounds_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            // theweh
            if ((VerScroll < VerScrollMax) || (VerScroll > 0)) {
                VerScroll += ScrollOutofBoundsDelta.Y;
            }
        }
        #endregion
        #region Position Handling
        private int GetVerticalScrollFromCursor(int MousePositionY, float ThumbPosition) {
            return (int)((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition) * (CurrentItems.Count-1)) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
        }
        //HorScroll = (MouseX - hBarX - ThumbPos) * 
        private int GetHorizontalScrollFromCursor(int MousePositionX, float ThumbPosition) {
            return (int)((float)((MousePositionX - HorizontalScrollBounds.X - ThumbPosition) * 100.0f) / (HorizontalScrollBounds.Width - HorizontalScrollThumb.Width));
        }
        #endregion
        #region Selection Handling
        private Point SELTEST = new Point(0, 0);
        private bool InSelection = false;
        private Point SelectionDelta = new Point(0, 0);
        private Point SelectionStart = new Point(0, 0);
        private Point selectionEnd = new Point(0, 0);
        private Point SelectionEnd {
            get { return selectionEnd; }
            set {
                selectionEnd = value;
                SelectionDelta = new Point(value.X - SelectionStart.X, value.Y - SelectionStart.Y);
            }
        }
        private int SelectedItemstart = -1;
        private int ListLinePoint(int PositionY, PointLineCalcuation CalculationType = PointLineCalcuation.LineToPositiion) {
            if (CalculationType == PointLineCalcuation.LineToPositiion) {
                return (GenericLine_Height * PositionY) + LineHeaderHeight;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLine) {
                return (int)Math.Floor((float)(PositionY - LineHeaderHeight) / (float)GenericLine_Height) + VerScroll;
            }
            else if (CalculationType == PointLineCalcuation.PositionToLineWithoutScroll) {
                return (int)((float)(PositionY - LineHeaderHeight) / (float)GenericLine_Height);
            }
            else if (CalculationType == PointLineCalcuation.LineToPositiionScrollFactored) {
                return (GenericLine_Height * (PositionY - VerScroll)) + LineHeaderHeight;
            }
            else {
                return 0;
            }
        }
        private Point TilePoint(Point Position, PointLineCalcuation CalculationType = PointLineCalcuation.LineToPositiion) {
            if (CalculationType == PointLineCalcuation.LineToPositiion) {
                return new Point(Position.X * TileWidth, (int)((float)GenericLine_Height * TilePadding) * Position.Y);
            }
            else if (CalculationType == PointLineCalcuation.PositionToLine) {
                float TileHeight = (float)GenericLine_Height * TilePadding;
                int XComponent = (int)Math.Floor((float)Position.X / TileWidth);
                int YComponent = TilesPerLine * (int)Math.Floor((float)Position.Y / TileHeight);
                return new Point(XComponent + YComponent + VerScroll, 0);
            }
            else if (CalculationType == PointLineCalcuation.PositionToLineWithoutScroll) {
                float TileHeight = (float)GenericLine_Height * TilePadding;
                int XComponent = (int)((float)Position.X / TileWidth);
                int YComponent = (int)(TilesPerLine * (float)Position.Y / TileHeight);
                return new Point(XComponent + YComponent, 0);
            }
            else if (CalculationType == PointLineCalcuation.LineToPositiionScrollFactored) {
                int AbsoluteLine = Position.X;
                float Line = (float)AbsoluteLine - VerScroll;
                float TileHeight = (float)GenericLine_Height * TilePadding;
                int TempY = (int)Math.Floor(Line / (float)TilesPerLine);

                int XComponent = (int)((Line - (Math.Floor(Line / (float)TilesPerLine)) * (float)TilesPerLine) * (float)TileWidth);
                int YComponent = (int)TempY * (int)TileHeight;

                return new Point(XComponent, YComponent);
            }
            else {
                return Point.Empty;//;
            }
        }
        private Point SelectionBounds(int Starting, int Ending) {
            Point PNT = new Point(0, 0);
            if (Starting < Ending)
                PNT = new Point(Starting, Ending - Starting);
            else if (Starting > Ending)
                PNT = new Point(Ending, Starting - Ending);
            else if (Starting == Ending)
                PNT = new Point(0, 0);
            return PNT;
        }
        private int AbsMovement() {
            int Start = ListLinePoint((SelectionBounds(SelectionStart.Y, SELTEST.Y).X), PointLineCalcuation.PositionToLine);
            int Endl = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X + SelectionBounds(SelectionStart.Y, SELTEST.Y).Y, PointLineCalcuation.PositionToLine);
            int DIFF = Endl - Start;
            return DIFF;
        }
        int FirstSelection = -1;
        private bool IsVaildSelectionBox() {
            bool Ret = false;
            if (SelectionBounds(SelectionStart.Y, SELTEST.Y).Y + 1 > GenericLine_Height) { Ret = true; }
            return Ret;
        }
        private void CursorClickSelect(Point MSPOS) {
            int SelectInt = -1;
            SelectInt = ListLinePoint(MSPOS.Y, PointLineCalcuation.PositionToLine);
            if (CtrlKey == true) {
                try {
                    if (CurrentItems[SelectInt].Selected == true) { CurrentItems[SelectInt].Selected = false; }
                    else { CurrentItems[SelectInt].Selected = true; }
                }
                catch { }
            }
            else if (ShiftKey == true) {
                if (FirstSelection == -1) {
                    FirstSelection = SelectInt;
                }
                ClearSelected();
                if (SelectInt > FirstSelection) {
                    for (int i = FirstSelection; i <= SelectInt; i++) {
                        if ((i >= 0) && (i < CurrentItems.Count)) { CurrentItems[i].Selected = true; }
                    }
                }
                else if (SelectInt < FirstSelection) {
                    for (int i = SelectInt; i <= FirstSelection; i++) {
                        if ((i >= 0) && (i < CurrentItems.Count)) { CurrentItems[i].Selected = true; }
                    }
                }
                else { if ((SelectInt >= 0) && (SelectInt < CurrentItems.Count)) { CurrentItems[SelectInt].Selected = true; } }
            }
            else if (CursorOutofBounds == false) {
                ClearSelected();
                if ((SelectInt >= 0) && (SelectInt < CurrentItems.Count)) {
                    CurrentItems[SelectInt].Selected = true;
                    FirstSelection = SelectInt;
                }
            }
        }
        private void ClearSelected() {
            foreach (ListItem itm in CurrentItems) {
                if (itm.Selected == true)
                    itm.Selected = false;
            }
        }
        private int ClampTileSelection(int Position) {
            if (Position <= 0) {
                return 0;
            }
            else if (Position >= TilesPerLine * TileWidth) {
                return (TilesPerLine * TileWidth) - 3;
            }
            else {
                return Position;
            }
        }
        private bool CursorMoveSelect() {
            int Start = -1;
            int Endl = -1;
            if (CursorOutofBounds == false) {
                ClearSelected();
            }
            Start = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X, PointLineCalcuation.PositionToLine);
            Endl = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X + SelectionBounds(SelectionStart.Y, SELTEST.Y).Y, PointLineCalcuation.PositionToLine);
            if (Endl > CurrentItems.Count - 1) { Endl = CurrentItems.Count - 1; }
            if (CurrentItems.Count > 1) {
                for (int i = Start; i <= Endl; i++) {
                    if ((i >= 0) && (i < CurrentItems.Count)) { CurrentItems[i].Selected = true; }
                }
            }
            return true;
        }
        private void SelectValuesList(Point MSPOS, bool LockOnMouseMove = false) {
            // Invalidate()
            try {
                bool Result = IsVaildSelectionBox();
                if (Result == true) {
                    Result = CursorMoveSelect();
                }
                if (Result == false) {
                    if (LockOnMouseMove == false) {
                        CursorClickSelect(MSPOS);
                    }
                }
            }
            catch { }
            Invalidate();
        }
        private bool HitCheckBox(Point MouseLocation) {
            int SelectedLine = -1;
            SelectedLine = ListLinePoint(MouseLocation.Y, PointLineCalcuation.PositionToLine);
            int SelectedColumn = GetColumn(MouseLocation);
            if (MouseLocation.Y > LineHeaderHeight) {
                if ((columns.Count > 0) && (SelectedColumn > -1) && (SelectedColumn < columns.Count)) {
                    if (columns[SelectedColumn].DisplayType == ColumnDisplayType.Checkbox) {
                        if ((SelectedLine < CurrentItems.Count) && (SelectedLine > -1)) {
                            Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                            if (GetCheckBoxRectangle(CurrentItem.X, CurrentItem.Y, columns[SelectedColumn].Width).Contains(MouseLocation)) {
                                CheckChange(SelectedLine, SelectedColumn);
                                return true;
                            }
                        }
                    }
                    else if (columns[SelectedColumn].DisplayType == ColumnDisplayType.CheckboxWithText) {
                        if ((SelectedLine < CurrentItems.Count) && (SelectedLine > -1)) {
                            Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                            if (GetCheckBoxRectangle(CurrentItem.X, CurrentItem.Y, 0).Contains(MouseLocation)) {
                                CheckChange(SelectedLine, SelectedColumn);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool HitDropDown(Point MouseLocation) {
            int SelectedLine = -1;
            SelectedLine = ListLinePoint(MouseLocation.Y, PointLineCalcuation.PositionToLine);
            int SelectedColumn = GetColumn(MouseLocation);
            if (MouseLocation.Y > LineHeaderHeight) {
                if ((columns.Count > 0) && (SelectedColumn > -1) && (SelectedColumn < columns.Count)) {
                    if (columns[SelectedColumn].DisplayType == ColumnDisplayType.DropDown) {
                        if ((SelectedLine < CurrentItems.Count) && (SelectedLine > -1)) {
                            if ((Math.Abs(SelectionDelta.Y) < GenericLine_Height)) {
                                Rectangle CurrentItem = GetItemRectangle(SelectedLine, SelectedColumn);
                                try {
                                    Point HitLocation = new Point(CurrentItem.X, CurrentItem.Y);

                                    Point BoxLocation = PointToScreen(HitLocation);
                                    if (HitLocation.X < 0) {
                                        BoxLocation = PointToScreen(new Point(0, HitLocation.Y));
                                    }
                                    Size TempSize = CurrentItem.Size;
                                    if (CurrentItem.X+ CurrentItem.Width >= Width) {
                                        int Diff = Width - (CurrentItem.X + TempSize.Width);
                                        if (ShowVertScroll == true) {
                                            TempSize = new Size(CurrentItem.Width - Diff - VerticalScrollBar.Width, CurrentItem.Height);
                                        }
                                        else {
                                            TempSize = new Size(CurrentItem.Width - Diff, CurrentItem.Height);
                                        }
                                    }
                                    //new Point(CurrentItem.X + PointToScreen(this.Location).X, CurrentItem.Y + PointToScreen(this.Location).Y - GenericLine_Height);
                                    DropDownClicked?.Invoke(this, new DropDownClickedEventArgs(HitLocation, BoxLocation, TempSize, SelectedColumn, SelectedLine, CurrentItems[SelectedLine]));
                                }
                                catch { }
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        private void CheckChange(int Line, int Column) {
            if (Column == 0) {
                CurrentItems[Line].Checked = !CurrentItems[Line].Checked;
                ItemCheckedChanged?.Invoke(this, new ItemCheckedChangeEventArgs(Column, Line, CurrentItems[Line], CurrentItems[Line].Checked));
            }
            else if (Column > 0) {
                int CurrentSubItem = Column - 1;
                if (CurrentSubItem < CurrentItems[Line].SubItems.Count) {
                    CurrentItems[Line].SubItems[CurrentSubItem].Checked = !CurrentItems[Line].SubItems[CurrentSubItem].Checked;
                    ItemCheckedChanged?.Invoke(this, new ItemCheckedChangeEventArgs(Column, Line, CurrentItems[Line], CurrentItems[Line].SubItems[CurrentSubItem].Checked));
                }
            }
        }
        private Rectangle GetItemRectangle(int Line, int Column) {
            int LineY = ListLinePoint(Line, PointLineCalcuation.LineToPositiionScrollFactored);
            int ColumnEnd = LineTextOffset;//0;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    if (i == Column) {
                        return new Rectangle(ColumnEnd, LineY, columns[i].Width, (int)GenericLine_Height);
                    }
                    ColumnEnd += columns[i].Width;
                }
            }
            return new Rectangle(-1, -1, 0, 0);
        }
        private int GetColumn(Point MouseLocation) {
            int SelectedColumn = -1;
            int ColumnStart = 0;
            int ColumnEnd = LineTextOffset;
            for (int i = 0; i < columns.Count; i++) {
                if (columns[i].Visible) {
                    ColumnEnd += columns[i].Width;
                    ColumnStart = ColumnEnd - columns[i].Width;
                    if ((MouseLocation.X >= ColumnStart) && (MouseLocation.X < ColumnEnd)) {
                        SelectedColumn = i;
                        break;
                    }
                }
            }
            return SelectedColumn;
        }
        #endregion
        #region Selection Support
        public void LineClearSelection(bool Render = true) {
            for (int i = 0; i <= CurrentItems.Count - 1; i++) {
                CurrentItems[i].Selected = false;
            }
            if (Render == true) {
                Invalidate();
            }
        }
        public void LineSingleSelect(int Index) {
            if (Index < CurrentItems.Count) {
                CurrentItems[Index].Selected = true;
                Invalidate();
            }
        }
        public void CentreLine(int Line, bool SelectLine = true, bool ClearSelection = false) {
            if (Line < 0) { return; }
            if (CurrentItems.Count < MaximumVerticalItems) {
                VerScroll = 0;
            }
            else {
                //int First = GetFirstSelected();
                if (Line == VerScroll) {
                    VerScroll = Line;
                }
                else if (Line >= (VerScroll + MaximumVerticalItems)) {
                    VerScroll = Line - MaximumVerticalItems;
                }
                else if (Line < VerScroll) {
                    VerScroll = Line;
                }
                else if ((Line >= VerScroll) && (Line <= (VerScroll + MaximumVerticalItems))) {

                }
            }
            if (ClearSelection == true)
                LineClearSelection(false);
            if (SelectLine == true)
                LineSingleSelect(Line);
        }
        #endregion
        #region Mouse Events
        bool InScrollBounds = false;
        bool ScrollStart = false;
        ScrollArea ScrollHit = ScrollArea.None;
        float ThumbDelta = 0;
        enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        bool HitHeader = false;
        int HitStart = 0;
        int SelectedColumn = -1;
        int OldWidth = 0;
        bool IgnoreLines = false;
        Point ScrollOutofBoundsDelta = new Point(0, 0);
        private bool CursorOutofBounds = false;
        private void LineInterface_MouseDown(object? sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                MouseIsDown = true;
                if (e.Y < LineHeaderHeight) {
                    int Xpos = LineTextOffset;
                    for (int i = 0; i < columns.Count; i++) {
                        if (columns[i].Visible) {
                            Xpos += columns[i].Width;
                            if (columns[i].FixedWidth == false) {
                                if ((e.X > Xpos - 5) && (e.X < Xpos + 5)) {
                                    HitHeader = true;
                                    OldWidth = columns[i].Width;
                                    SelectedColumn = i;
                                    break;
                                }
                            }
                        }
                    }
                    IgnoreLines = true;
                    HitStart = e.X;
                }
                else if ((ShowVertScroll == true) && (e.X >= Width - ScrollSize)) {
                    ScrollHit = ScrollArea.Vertical;
                    if (ScrollStart == false) {
                        ThumbDelta = e.Y - VerticalScrollThumb.Y;
                        if (ThumbDelta < 0) {
                            ThumbDelta = 0;
                        }
                        else if (ThumbDelta > VerticalScrollThumb.Height + VerticalScrollThumb.Height) {
                            ThumbDelta = VerticalScrollThumb.Height;
                        }
                        ScrollStart = true;
                    }
                    InScrollBounds = true;
                }
                else if ((ShowHorzScroll == true) && (e.Y > Height - ScrollSize)) {
                    ScrollHit = ScrollArea.Horizontal;
                    if (ScrollStart == false) {
                        ThumbDelta = e.X - HorizontalScrollThumb.X;
                        if (ThumbDelta < 0) {
                            ThumbDelta = 0;
                        }
                        else if (ThumbDelta > HorizontalScrollThumb.X + HorizontalScrollThumb.Width) {
                            ThumbDelta = HorizontalScrollThumb.Width;
                        }
                        ScrollStart = true;
                    }
                    InScrollBounds = true;
                }
                else {
                    ScrollHit = ScrollArea.None;
                    InScrollBounds = false;

                    if (CtrlKey == false & ShiftKey == false) {
                        ClearSelected();
                    }
                    if (InSelection == false) {
                        SelectionStart = new Point(e.Location.X, e.Location.Y);
                        SelectionEnd = new Point(e.Location.X, e.Location.Y);
                        SelectedItemstart = ListLinePoint(e.Location.Y, PointLineCalcuation.PositionToLine);
                        InSelection = true;
                    }

                    SelectedItems();
                }
            }
            Invalidate();
        }
        Point CurrentMousePosition = new Point(-1, -1);
        private void LineInterface_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e) {
            CurrentMousePosition = e.Location;
            if ((e.Y < LineHeaderHeight) && (HitHeader == false)) {
                int Xpos = LineTextOffset;
                this.Cursor = DefaultCursor;
                for (int i = 0; i < columns.Count; i++) {
                    if (columns[i].Visible) {
                        Xpos += columns[i].Width;
                        if (columns[i].FixedWidth == false) {
                            if ((e.X > Xpos - 5) && (e.X < Xpos + 5)) {
                                this.Cursor = Cursors.SizeWE;
                                break;
                            }
                        }
                    }
                }
            }
            else {
                if (HitHeader == false) {
                    this.Cursor = DefaultCursor;
                }

            }
            if (HitHeader == true) {
                if ((SelectedColumn < columns.Count) && (columns.Count > 0)) {
                    columns[SelectedColumn].Width = OldWidth + e.X - HitStart;
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Vertical)) {
                if (CurrentItems.Count > 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, ThumbDelta);
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Horizontal)) {
                if (columns.Count > 0) {
                    HorScroll = GetHorizontalScrollFromCursor(e.X, ThumbDelta);
                    Invalidate();
                }
            }
            else {
                if (e.Location.Y > this.Height) {
                    ScrollOutofBoundsDelta.Y = (int)((float)(e.Location.Y - this.Height) / (float)(ScrollItems * 10));
                    CursorOutofBounds = true;
                    ScrollOutofBounds.Enabled = true;
                }
                else if (e.Location.Y < 0) {
                    ScrollOutofBoundsDelta.Y = (int)((float)(e.Location.Y) / (float)(ScrollItems * 10));
                    ScrollOutofBounds.Enabled = true;
                    CursorOutofBounds = true;
                }
                else {
                    CursorOutofBounds = false;
                    ScrollOutofBounds.Enabled = false;
                }

                SELTEST = new Point(e.Location.X, e.Location.Y);
                if (InSelection == true) {
                    SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedItemstart, PointLineCalcuation.LineToPositiionScrollFactored));
                    SelectionEnd = new Point(e.Location.X, e.Location.Y);
                    SelectValuesList(e.Location, true);
                }
                SelectedItems();
                Invalidate();
            }
        }
        bool MouseIsDown = false;
        private void LineInterface_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e) {
            // timer.Stop()
            MouseIsDown = false;
            if (e.Button == MouseButtons.Left) {
                if (InScrollBounds == false) {
                    if (IgnoreLines == false) {
                        InSelection = false;
                        SelectedItemstart = -1;
                        Invalidate();
                        SelectedItems();
                    }
                }
            }
            HitHeader = false;
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            ScrollOutofBounds.Enabled = false;
            IgnoreLines = false;
            SelectedItems();
            Invalidate();
        }
        private void LineInterface_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e) {
            if (IgnoreLines == true) { return; }
            if ((InScrollBounds == true) && (e.X >= Width - ScrollSize)) {
                InScrollBounds = true;
                float ThumbDeltaTest = e.Y - VerticalScrollThumb.Y;
                if (ThumbDeltaTest < 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, 0);
                    Invalidate();
                }
                else if (ThumbDeltaTest > VerticalScrollThumb.Height) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, 0);
                    Invalidate();
                }
            }
            else if ((InScrollBounds == true) && (e.Y >= Height - ScrollSize)) {
                InScrollBounds = true;
                if (ShowHorzScroll == true) {
                    float ThumbDeltaTest = e.X - HorizontalScrollThumb.X;
                    if (ThumbDeltaTest < 0) {
                        HorScroll = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                    else if (ThumbDeltaTest > HorizontalScrollThumb.Width) {
                        HorScroll = GetHorizontalScrollFromCursor(e.X, 0);
                        Invalidate();
                    }
                }
            }
            if (InScrollBounds == false) {
                ListItemClickEvent(e.Y, e.Button);
                if (e.Button == MouseButtons.Left) {

                    bool HitSomething = false;
                    HitSomething = HitCheckBox(e.Location);
                    if (e.Location.Equals(CurrentMousePosition)) {
                        if (HitDropDown(e.Location) == true) {

                            HitSomething = true;
                        }
                    }
                    if (HitSomething == false) {
                        SelectValuesList(e.Location);
                    }
                }
            }

        }
        private void ListItemClickEvent(int VerPos, MouseButtons MouseBtn) {
            int TempCurrentLine = ListLinePoint(VerPos, PointLineCalcuation.PositionToLine);
            int TempY = ListLinePoint(VerPos, PointLineCalcuation.LineToPositiionScrollFactored);
            Rectangle LineRectangle = new Rectangle(0, TempY, Width, GenericLine_Height);
            if (TempCurrentLine < 0) { return; }
            if (CurrentItems.Count == 0) { return; }
            if (TempCurrentLine >= CurrentItems.Count) { return; }
            switch (MouseBtn) {
                case MouseButtons.Left:
                    ItemClicked?.Invoke(this, CurrentItems[TempCurrentLine], TempCurrentLine, LineRectangle); return;
                case MouseButtons.Middle:
                    ItemMiddleClicked?.Invoke(this, CurrentItems[TempCurrentLine], TempCurrentLine, LineRectangle); return;
                case MouseButtons.Right:
                    ItemRightClicked?.Invoke(this, CurrentItems[TempCurrentLine], TempCurrentLine, LineRectangle); return;
                default: return;
            }
        }
        private void LineInterface_MouseEnter(object? sender, System.EventArgs e) {
        }
        private void LineInterface_MouseHover(object? sender, System.EventArgs e) {
        }
        private void LineInterface_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e) {
            if (CtrlKey == false && ShiftKey == false) {
                int D = e.Delta;
                int DC = ScrollItems * (int)Math.Abs((double)D / (double)120);
                if (D > 0) {
                    if (VerScroll > 0)
                        VerScroll -= DC;
                }
                else if (VerScroll < VerScrollMax)
                    VerScroll += Math.Abs(DC);
            }
        }
        #endregion
        #region Key Events
        private const int WM_KEYDOWN = 0x100;
        private bool ShiftKey = false;
        private bool CtrlKey = false;
        protected override bool ProcessKeyPreview(ref System.Windows.Forms.Message m) {
            bool Processed = false; // Set to true, if we have "consumed" this keystroke
            switch (m.Msg) // This Select can be longer - it's an example
            {
                case WM_KEYDOWN: {
                        switch (m.WParam.ToInt32()) // WParam it is a virtual key code, in this case
                        {
                            case (int)Keys.Escape: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.Down: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.Up: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.ShiftKey: {
                                    Processed = true;
                                    break;
                                }
                            default:
                                return false;
                        }
                        break;
                    }
                default:
                    return false;
            }
            if (!Processed) {
                Processed = base.ProcessKeyPreview(ref m);
            }

            return Processed;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Down) {
                SelectedItems();
                int inst = 0;
                if (SelectedIndex >= CurrentItems.Count - 1)
                    inst = CurrentItems.Count - 1;
                else
                    inst = SelectedIndex + 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                    Focus();
                }
                else
                    CentreLine(inst, true, true);
                Invalidate();
                return true;
            }
            if (keyData == Keys.Up) {
                SelectedItems();
                int inst = 0;
                if (SelectedIndex <= 0)
                    inst = 0;
                else
                    inst = SelectedIndex - 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                }
                else
                    CentreLine(inst, true, true);
                Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ConsoleInterface_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            ShiftKey = e.Shift;
            CtrlKey = e.Control;
            // If e.KeyCode = Keys.Back Then
            // Try
            // CommandString = CommandString.Remove(CommandString.Length - 1, 1)
            // RefreshLongLine = True
            // Catch
            // End Try
            // ElseIf e.KeyCode = Keys.Enter Then
            // Try

            // RaiseEvent CommandEntered()
            // Catch
            // End Try
            // Else

            // End If
            if (CtrlKey == false && ShiftKey == false) {
                if (e.KeyCode == Keys.Up) {
                }
            }
            Invalidate();
        }
        private void LineInterface_KeyPress(object? sender, System.Windows.Forms.KeyPressEventArgs e) {
            //CheckModifierKeys(e);
        }
        private void LineInterface_KeyUp(object? sender, System.Windows.Forms.KeyEventArgs e) {
            ShiftKey = e.Shift;
            CtrlKey = e.Control;
        }
        private void LineEditingInterface_KeyDown(object? sender, KeyEventArgs e) {
            ShiftKey = e.Shift;
            CtrlKey = e.Control;
        }
        #endregion
        #region Control Events
        private void LineInterface_Load(object? sender, EventArgs e) {

        }
        private void LineEditingInterface_Resize(object? sender, EventArgs e) {
            Invalidate();
        }
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // LineInterface
            // 
            this.Name = "LineInterface";
            this.Size = new System.Drawing.Size(362, 346);
            this.Load += new System.EventHandler(this.LineInterface_Load);
            this.ResumeLayout(false);
        }
        #endregion
        //public void CentreLine(int Line, bool SelectLine = true, bool ClearSelection = false) {
        //    VerScroll = Line;
        //    if (ClearSelection == true)
        //        LineClearSelection(false);
        //    if (SelectLine == true)
        //        LineSingleSelect(Line);
        //}

        private enum PointLineCalcuation {
            LineToPositiion = 0x00,
            PositionToLine = 0x01,
            PositionToLineWithoutScroll = 0x02,
            LineToPositiionScrollFactored = 0x03
        }
    }
    public class Column {
        private int countOffset;
        [System.ComponentModel.Category("Data")]
        public int CountOffset { get => countOffset; set => countOffset = value; }
        private string text;
        [System.ComponentModel.Category("Appearance")]
        public string Text { get => text; set => text = value; }
        private bool fixedWidth;
        [System.ComponentModel.Category("Layout")]
        public bool FixedWidth { get => fixedWidth; set => fixedWidth = value; }
        [System.ComponentModel.Category("Layout")]
        private int width = 20;
        public int Width {
            get { return width; }
            set {
                if (value < 20) {
                    width = 20;
                }
                else {
                    width = value;
                }
            }
        }
        private bool dropdownRight = false;
        [System.ComponentModel.Category("Appearance")]
        public bool DropDownRight {
            get { return dropdownRight; }
            set { dropdownRight = value; }
        }
        private bool dropdownVisible = true;
        [System.ComponentModel.Category("Appearance")]
        public bool DropDownVisible {
            get { return dropdownVisible; }
            set { dropdownVisible = value; }
        }
        private bool visible = true;
        ColumnDisplayType displayType = ColumnDisplayType.Text;
        [System.ComponentModel.Category("Appearance")]
        public ColumnDisplayType DisplayType {
            get { return displayType; }
            set {
                displayType = value;
            }
        }
        ItemTextAlignment itemAlignment = ItemTextAlignment.Left;
        [System.ComponentModel.Category("Appearance")]
        public ItemTextAlignment ItemAlignment {
            get { return itemAlignment; }
            set {
                itemAlignment = value;
            }
        }
        ColumnTextAlignment columnAlignment = ColumnTextAlignment.Left;
        [System.ComponentModel.Category("Appearance")]
        public ColumnTextAlignment ColumnAlignment {
            get { return columnAlignment; }
            set {
                columnAlignment = value;
            }
        }
        bool useItemForeColor = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseItemForeColor {
            get { return useItemForeColor; }
            set {
                useItemForeColor = value;
            }
        }
        bool useItemBackColor = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseItemBackColor {
            get { return useItemBackColor; }
            set {
                useItemBackColor = value;
            }
        }
        public Column() {
            this.text = "Column";
        }
        public Column(string text) {
            this.text = text;
        }
        public Column(string text, int Width) {
            this.text = text;
            this.width = Width;
        }
        [System.ComponentModel.Category("Appearance")]
        public bool Visible { get => visible; set => visible = value; }

        public override string? ToString() {
            return text;
        }
    }
    public enum ColumnDisplayType {
        Text = 0x00,
        Checkbox = 0x01,
        CheckboxWithText = 0x02,
        ProgressBar = 0x04,
        DropDown = 0x06,
        LineCount = 0xFF
    }
    public enum ItemTextAlignment {
        None = 0x00,
        Left = 0x01,
        Center = 0x02,
        Right = 0x03
    }
    public enum ColumnTextAlignment {
        None = 0x00,
        Left = 0x01,
        Center = 0x02,
        Right = 0x03
    }
    public enum MarkerStyleType {
        Highlight = 0x00,
        Underline = 0x01,
        Pointer = 0x02,
        PointerWithUnderline = 0x03,
        PointerWithBox = 0x04
    }
    public class ListObject {
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
        private object? tag;
        [System.ComponentModel.Category("Data")]
        public object? Tag { get => tag; set => tag = value; }
        private int numValue;
        [System.ComponentModel.Category("Data")]
        public int Value { get => numValue; set => numValue = value; }
        [System.ComponentModel.Category("Appearance")]
        public bool Checked {
            get { return ischecked; }
            set {
                ischeckedPrevious = ischecked;
                ischecked = value;
                if (ischecked != ischeckedPrevious) { ischeckedChanged = true; }
                else { ischeckedChanged = false; }
            }
        }
        public bool CheckedChanged {
            get { return ischeckedChanged; }
        }
        public void ResetChangedChanged() {
            ischeckedChanged = false;
        }
        public override string? ToString() {
            return text;
        }

        private bool ischeckedChanged = false;
        private bool ischeckedPrevious = false;
        private bool ischecked = false;

    }
    public class ListItem : ListObject {
        private bool selected = false;
        [System.ComponentModel.Category("Control")]
        public bool Selected { get => selected; set => selected = value; }

        private List<ListSubItem> subItems = new List<ListSubItem>();
        [System.ComponentModel.Category("List Items")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ListSubItem> SubItems {
            get { return subItems; }
        }
        private ListObject GetItem(uint Subindex) {
            if (Subindex == 0) {
                return this;
            }
            else {
                if ((Subindex-1) < subItems.Count) {
                    return subItems[(int)Subindex-1];
                }
                else {
                    return new ListSubItem("Out of Bounds");
                }
            }
        }
        public ListObject this[int index] {
            get {
                uint Temp = (uint)index;
                if (index < 0) { Temp = 0; }
                return GetItem(Temp); 
            }
        }
        public ListItem() {

        }
        public ListItem(string Text) {
            this.Text = Text;
        }
    }
    public class ListSubItem : ListObject {
        public ListSubItem() {
        }
        public ListSubItem(string Text) {
            this.Text = Text;
        }
        public ListSubItem(bool Checked) {
            this.Checked = Checked;
        }
    }
    public class ItemCheckedChangeEventArgs : EventArgs {
        ListItem? parentItem = null;
        public ListItem? ParentItem {
            get { return parentItem; }
        }
        int column = -1;
        public int Column {
            get { return column; }
        }
        int item = -1;
        public int Item {
            get { return item; }
        }
        bool checkedState;
        public bool Checked {
            get { return checkedState; }
        }
        public ItemCheckedChangeEventArgs(int Column, int Item, ListItem? ItemValue, bool Checked) {
            this.column = Column;
            this.item = Item;
            this.parentItem = ItemValue;
            this.checkedState = Checked;
        }
    }
    public class DropDownClickedEventArgs : EventArgs {
        ListItem? parentItem = null;
        public ListItem? ParentItem {
            get { return parentItem; }
        }
        int column = -1;
        public int Column {
            get { return column; }
        }
        int item = -1;
        public int Item {
            get { return item; }
        }
        private Point location;
        private Point screenLocation;
        public Point ScreenLocation {
            get { return screenLocation; }
        }
        public Point Location {
            get { return location; }
        }
        Size itemsize;
        public Size ItemSize {
            get { return itemsize; }
        }
        public DropDownClickedEventArgs(Point HitPoint, Point AbsolutePosition, Size ItemSize, int Column, int Item, ListItem? ItemValue) {
            this.column = Column;
            this.item = Item;
            this.parentItem = ItemValue;
            this.itemsize = ItemSize;
            location = HitPoint;
            screenLocation = AbsolutePosition;
        }
    }
}
