using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handlers;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace ODModules {
    // <DefaultEvent("CommandEntered")> _
    public class LineEditingInterface : System.Windows.Forms.UserControl {
        private const int WM_KEYDOWN = 0x100;
        //private bool mwe = false;
        //private int MessageBlockSize = 0;
        public int ScrollMax = 100;
        public decimal ScrollPercent = 1;
        public bool ScrollEdit = false;
        public List<LIL> Lines = new List<LIL>();
        public List<LSEL> Highlights = new List<LSEL>();
        //private int LineSelect = -1;
        //private string CommandString = "";
        public string Command = "";
        //private bool ShowCursor = true;

        private bool RefreshLongLine = false;
        private int Yscroll = 0;
        private bool ShiftKey = false;
        private bool CtrlKey = false;

        private Point SELTEST = new Point(0, 0);
        private bool InSelection = false;
        //private Point ShiftSelect = new Point(-1, -1);
        //private bool TogSelect = false;
        private int sz_geny2 = 0;
        private int sz_tilex = 0;
        private int MaxTilesWide = 0;

        //public event CommandEnteredEventHandler? CommandEntered;

        //public delegate void CommandEnteredEventHandler();

        public event ValueChangedEventHandler? ValueChanged;

        public delegate void ValueChangedEventHandler();

        public LineEditingInterface() {
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
            // EXAMPLE C CODE


            RefreshLongLine = true;
        }





        #region Highlighters
        public void HighlightNew(string Input, Color Highlighter, bool Enable = true) {
            LSEL item = new LSEL(Input, Highlighter, Enable);
            Highlights.Add(item);
            Invalidate();
        }
        public void HighlightEdit(int Index, string Input, Color Highlighter, bool Enable) {
            if ((Index >= 0) && (Index <= Highlights.Count - 1)) {
                Highlights[Index].SearchString = Input;
                Highlights[Index].Highlight = Highlighter;
                Highlights[Index].Enable = Enable;
                Invalidate();
            }
        }
        public void HighlightRemove(int Index) {
            if (Index >= 0 && Index <= Highlights.Count - 1) {
                try {
                    Highlights.RemoveAt(Index);
                    Invalidate();
                }
                catch { }
            }
        }
        public void HighlightRemoveAll() {
            try {
                Highlights.Clear();
                Invalidate();
            }
            catch { }
        }
        #endregion
        #region Commands
        public void LineNew(string Input, bool Selected = false) {
            LIL item = new LIL();
            item.Line = Input;
            item.Selected = Selected;
            Lines.Add(item);
            RefreshLongLine = true;
            if (Selected == true) {
                SelectedLines();
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemove(int Index) {
            if (Index >= 0 || Index <= Lines.Count - 1) {
                Lines.RemoveAt(Index);
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveRange(int Start, int EndLine) {
            for (int i = EndLine; i >= Start; i--) {
                if (Start >= 0 || EndLine <= Lines.Count - 1) {
                    Lines.RemoveAt(i);
                }
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveAll() {
            RefreshLongLine = true;
            Lines.Clear();
            Invalidate();
        }
        public void LineCopy() {
            int Sellines = SelectedLines();
            int cnt = 0;
            string AppendString = "";
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    if (cnt < Sellines) {
                        AppendString += Lines[i].Line + Handlers.Constants.NewLineEnv;
                        cnt += 1;
                    }
                    else {
                        AppendString += Lines[i].Line;
                    }
                }
            }
            if (AppendString != null) {
                if (AppendString != "") {
                    Clipboard.SetText(AppendString);
                }
            }
        }
        public void LineCut() {
            int Sellines = SelectedLines();
            int cnt = 0;
            string AppendString = "";
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    if (cnt < Sellines) {
                        AppendString += Lines[i].Line + Handlers.Constants.NewLineEnv;
                        cnt += 1;
                    }
                    else {
                        AppendString += Lines[i].Line;
                    }
                }
            }
            try {
                Clipboard.SetText(AppendString);
            }
            catch { }
            LineRemoveSelected();
        }
        public void LinePaste(bool SelectPasted = false) {
            try {
                string PAST = Clipboard.GetText();
                char[] separators = new char[] { (char)0x0A, (char)0x0D };
                string[] SS = PAST.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                int SELCNT = SelectedLines();
                if (SelectPasted == true) {
                    ClearSelected();
                }
                if (SELCNT == 0) {
                    for (int i = 0; i <= SS.Count() - 1; i++) {
                        LineNew(SS[i], SelectPasted);
                    }
                }
                else {
                    int FL = IndexCount;
                    int LL = FL + SS.Count() - 1;
                    int I = 0;
                    for (int J = FL; J <= LL; J++) {
                        LineInsert(SS[I], J + 1, SelectPasted, false);
                        I += 1;
                    }
                }
            }
            catch { }
            Invalidate();
        }
        public void LineReplaceAll(string OldString, string NewString, bool SelectedOnly = false) {
            try {
                int SELCNT = SelectedLines();
                if (SELCNT == 0) {
                    for (int i = 0; i <= Lines.Count - 1; i++) {
                        Lines[i].Line = Lines[i].Line.Replace(OldString, NewString);
                    }
                }
                else {
                    for (int I = 0; I <= Lines.Count - 1; I++) {
                        if (SelectedOnly == false) {
                            Lines[I].Line = Lines[I].Line.Replace(OldString, NewString);
                        }
                        else if (Lines[I].Selected == true) {
                            Lines[I].Line = Lines[I].Line.Replace(OldString, NewString);
                        }
                    }
                }
            }
            catch { }
            Invalidate();
        }
        public void LinePasteInsert() {
            try {
                string PAST = Clipboard.GetText();
                char[] separators = new char[] { (char)0x10, (char)0x13 };
                string[] SS = PAST.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                int SELTOT = SelectedLines();
                if (SELTOT == 0) {
                    for (int i = 0; i <= SS.Count() - 1; i++) {
                        LineNew(SS[i]);
                    }
                }
                else {
                    int SELCNT = 1;
                    int INSCNT = 0;
                    int EXTCNT = 0;
                    int KPOS = 0;
                    for (int k = 0; k <= Lines.Count - 1; k++) {
                        if (SELCNT > SELTOT) {
                            KPOS = k;
                            break;
                        }
                        else if (Lines[k].Selected == true) {
                            Lines[k].Line += " " + SS[INSCNT];
                            SELCNT += 1;
                            INSCNT += 1;
                        }
                    }
                    for (int l = INSCNT; l <= SS.Count() - 1; l++) {
                        LineInsert(SS[l], KPOS + EXTCNT, false, false);
                        EXTCNT += 1;
                    }
                }
            }
            catch { }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveSelected() {
            int sellines = SelectedLines();
            while (sellines > 0) {
                int i;
                for (i = 0; i <= Lines.Count - 1; i++) {
                    if (i <= Lines.Count - 1) {
                        LIL sKey = Lines.ElementAt(i);
                        if (sKey.Selected == true) {
                            Lines.Remove(sKey);
                        }
                    }
                }
                sellines = SelectedLines();
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveSelectedWithoutRedraw() {
            int sellines = SelectedLines();
            while (sellines > 0) {
                int i;
                for (i = 0; i <= Lines.Count - 1; i++) {
                    if (i <= Lines.Count - 1) {
                        LIL sKey = Lines.ElementAt(i);
                        if (sKey.Selected == true) {
                            Lines.Remove(sKey);
                        }
                    }
                }
                sellines = SelectedLines();
            }
        }
        public void LineRemoveBlanks() {
            int cnt = 0;
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (i <= Lines.Count - 1) {
                    LIL sKey = Lines.ElementAt(i);
                    string LIN = sKey.Line.TrimStart();
                    if (LIN == "" || LIN == " ") {
                        cnt += 1;
                    }
                }
            }
            while (cnt > 0) {
                int i;
                for (i = 0; i <= Lines.Count - 1; i++) {
                    if (i <= Lines.Count - 1) {
                        LIL sKey = Lines.ElementAt(i);
                        string LIN = sKey.Line.TrimStart();
                        if (LIN == "" || LIN == " ") {
                            Lines.Remove(sKey);
                            cnt -= 1;
                        }
                    }
                }
            }
            SelectedLines();
            Invalidate();
        }
        public void LineSelectAll() {
            for (int i = 0; i <= Lines.Count - 1; i++) {
                Lines[i].Selected = true;
            }
            Invalidate();
        }
        public void LineSelectInvert() {
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    Lines[i].Selected = false;
                }
                else {
                    Lines[i].Selected = true;
                }
            }
            Invalidate();
        }
        public void LineSelectSimilar() {
            string CHKSTR = "";
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    CHKSTR = Lines[i].Line;
                    break;
                }
            }
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Line.ToLower().Trim().Contains(CHKSTR.ToLower().Trim())) {
                    Lines[i].Selected = true;
                }
            }
            Invalidate();
        }
        public void LineSelectMatching() {
            string CHKSTR = "";
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    CHKSTR = Lines[i].Line;
                    break;
                }
            }
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Line.ToLower().Trim() == CHKSTR.ToLower().Trim()) {
                    Lines[i].Selected = true;
                }
            }
            Invalidate();
        }
        public void LineMove(DIR_TYPE Direction) {
            if (Direction == DIR_TYPE.UP) {
                for (int i = 0; i <= Lines.Count - 1; i++) {
                    if (Lines[i].Selected == true) {
                        if (i > 0) {
                            LIL old_val = Lines[i - 1];
                            LIL new_val = Lines[i];
                            Lines[i - 1] = new_val;
                            Lines[i] = old_val;
                        }
                    }
                }
            }
            else if (Direction == DIR_TYPE.DOWN) {
                for (int i = Lines.Count - 1; i >= 0; i += -1) {
                    if (Lines[i].Selected == true) {
                        if (i < Lines.Count - 1) {
                            LIL old_val = Lines[i + 1];
                            LIL new_val = Lines[i];
                            Lines[i] = old_val;
                            Lines[i + 1] = new_val;
                        }
                    }
                }
            }
            Invalidate();
        }
        public void LineRotate(DIR_TYPE Direction) {
            List<string> temp = new List<string>();
            List<int> post = new List<int>();
            int i;
            int j;
            for (i = Lines.Count - 1; i >= 0; i += -1) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true) {
                    temp.Add(Lines[i].Line);
                    post.Add(i);
                }
            }
            if (Direction == DIR_TYPE.UP) {
                for (j = 0; j <= post.Count - 1; j++) {
                    int inc = j - 1;
                    if (inc >= 0) {
                        if (Lines[post[j]].Selected == true) {
                            Lines[post[j]].Line = temp[inc];
                        }
                    }
                    else {
                        Lines[post[j]].Line = temp[post.Count - 1];
                    }
                }
            }
            else if (Direction == DIR_TYPE.DOWN) {
                for (j = 0; j <= post.Count - 1; j++) {
                    int inc = j + 1;
                    if (inc <= post.Count - 1) {
                        if (Lines[post[j]].Selected == true) {
                            Lines[post[j]].Line = temp[inc];
                        }
                    }
                    else {
                        Lines[post[j]].Line = temp[0];
                    }
                }
            }
            Invalidate();
        }
        public void LineClearSelection(bool Render = true) {
            for (int i = 0; i <= Lines.Count - 1; i++) {
                Lines[i].Selected = false;
            }
            if (Render == true) {
                Invalidate();
            }
        }
        public void LineModifyPattern(List<Patterning> Pattern) {
            if (Pattern.Count > 0) {
                int Count = 0;
                int PatternLength = Pattern.Count;
                for (int i = 0; i <= Lines.Count - 1; i++) {
                    if (Lines[i].Selected == true) {
                        int Index = Count % PatternLength;
                        try {
                            string StringBefore = Pattern[Index].StringBefore;
                            string StringAfter = Pattern[Index].StringAfter;
                            string StringMiddle = Pattern[Index].StringMiddle;
                            if (Pattern[Index].ApplyBefore == false) {
                                StringBefore = "";
                            }
                            if (Pattern[Index].ApplyAfter == false) {
                                StringAfter = "";
                            }
                            if (Pattern[Index].ApplyMiddle == false) {
                                StringMiddle = Lines[i].Line;
                            }
                            Lines[i].Line = StringBefore + StringMiddle + StringAfter;
                        }
                        catch { }
                        Count += 1;
                    }
                }
                Invalidate();
            }
        }
        public void LineSingleSelect(int Index) {
            Lines[Index].Selected = true;
            Invalidate();
        }
        public void LineSingleSelectExclusive(int Index) {
            for (int i = 0; i < Lines.Count; i++) {
                Lines[Index].Selected = false;
                if (i == Index) {
                    Lines[Index].Selected = true;
                    _SelectionCount = 1;
                    _IndexCount = i;
                }
            }
            _LineCount = Lines.Count;
            ValueChanged?.Invoke();
            Invalidate();
        }
        public void LineSortRegex(string SortOnExpression, bool Ascending = true) {
            try {
                int SELCNT = SelectedLines();
                if (SELCNT != 0) {
                    List<string> LinesList = LineGetSelected();
                    //bool BreakAll = false;
                    for (int i = 0; i <= LinesList.Count - 1; i++) {
                        for (int j = 0; j <= LinesList.Count - 1; j++) {
                            Match St1 = Regex.Match(LinesList[i], SortOnExpression);
                            string ComSt1 = LinesList[i];
                            string ComSt2 = LinesList[j];
                            if (St1.Success == true)
                                ComSt1 = St1.Value;
                            Match St2 = Regex.Match(LinesList[j], SortOnExpression);
                            if (St2.Success == true)
                                ComSt2 = St2.Value;
                            TextPosition Txp = LineTextDifference(ComSt1, ComSt2);
                            string OldStr = LinesList[i];
                            string OldStr1 = LinesList[j];
                            if (Txp == TextPosition.Higher | Txp == TextPosition.Lower) {
                                if (Ascending == false) {
                                    if (i > j) {
                                        if (Txp == TextPosition.Higher) {
                                            LinesList[i] = OldStr1;
                                            LinesList[j] = OldStr;
                                        }
                                    }
                                    else if (i < j) {
                                        if (Txp == TextPosition.Lower) {
                                            LinesList[i] = OldStr1;
                                            LinesList[j] = OldStr;
                                        }
                                    }
                                }
                                else if (i < j) {
                                    if (Txp == TextPosition.Higher) {
                                        LinesList[i] = OldStr1;
                                        LinesList[j] = OldStr;
                                    }
                                }
                                else if (i > j) {
                                    if (Txp == TextPosition.Lower) {
                                        LinesList[i] = OldStr1;
                                        LinesList[j] = OldStr;
                                    }
                                }
                            }
                        }
                    }
                    int k = 0;
                    for (int i = 0; i <= Lines.Count - 1; i++) {
                        if (Lines[i].Selected == true) {
                            Lines[i].Line = LinesList[k];
                            k += 1;
                        }
                    }
                }
                else {
                    List<string> LinesList = LineGetAll();
                    for (int i = 0; i <= LinesList.Count - 1; i++) {
                        for (int j = 0; j <= LinesList.Count - 1; j++) {
                            Match St1 = Regex.Match(LinesList[i], SortOnExpression);
                            string ComSt1 = LinesList[i];
                            string ComSt2 = LinesList[j];
                            if (St1.Success == true)
                                ComSt1 = St1.Value;
                            Match St2 = Regex.Match(LinesList[j], SortOnExpression);
                            if (St2.Success == true)
                                ComSt2 = St2.Value;
                            TextPosition Txp = LineTextDifference(ComSt1, ComSt2);
                            string OldStr = LinesList[i];
                            string OldStr1 = LinesList[j];
                            if (Txp == TextPosition.Higher | Txp == TextPosition.Lower) {
                                if (Ascending == false) {
                                    if (i > j) {
                                        if (Txp == TextPosition.Higher) {
                                            LinesList[i] = OldStr1;
                                            LinesList[j] = OldStr;
                                        }
                                    }
                                    else if (i < j) {
                                        if (Txp == TextPosition.Lower) {
                                            LinesList[i] = OldStr1;
                                            LinesList[j] = OldStr;
                                        }
                                    }
                                }
                                else if (i < j) {
                                    if (Txp == TextPosition.Higher) {
                                        LinesList[i] = OldStr1;
                                        LinesList[j] = OldStr;
                                    }
                                }
                                else if (i > j) {
                                    if (Txp == TextPosition.Lower) {
                                        LinesList[i] = OldStr1;
                                        LinesList[j] = OldStr;
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 0; i <= Lines.Count - 1; i++) {
                        Lines[i].Line = LinesList[i];
                    }
                }
            }
            catch { }
            Invalidate();
        }
        public TextPosition LineTextDifference(string CompareAgainst, string CompareTo) {
            if (CompareAgainst.Length > CompareTo.Length) {
                return TextPosition.Lower; // First is of lower order
            }
            else if (CompareAgainst.Length < CompareTo.Length) {
                return TextPosition.Higher; // First is of higher order
            }
            else if (CompareAgainst.Length == 0) {
                return TextPosition.Same;
            }
            else
                for (int i = 0; i <= CompareAgainst.Length - 1; i++) {
                    byte[] ComByte = Encoding.UTF8.GetBytes(CompareAgainst[i].ToString().ToCharArray());
                    byte[] AgsByte = Encoding.UTF8.GetBytes(CompareTo[i].ToString().ToCharArray());
                    int ComInt = BitConverter.ToInt32(SpanBytes(ComByte), 0);
                    int AgsInt = BitConverter.ToInt32(SpanBytes(AgsByte), 0);
                    if (ComInt > AgsInt) {
                        return TextPosition.Higher;
                    }
                    else if (ComInt < AgsInt) {
                        return TextPosition.Lower;
                    }
                }
            return TextPosition.Same;
        }
        public List<string> LineGetSelected(bool Inverted = false) {
            List<string> OutputList = new List<string>();
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Inverted == false) {
                    if (Lines[i].Selected == true) {
                        OutputList.Add(Lines[i].Line);
                    }
                }
                else if (Lines[i].Selected == false) {
                    OutputList.Add(Lines[i].Line);
                }
            }
            return OutputList;
        }
        public List<string> LineGetAll() {
            List<string> OutputList = new List<string>();
            for (int i = 0; i <= Lines.Count - 1; i++) {
                OutputList.Add(Lines[i].Line);
            }
            return OutputList;
        }
        public void LineInsertBlock(List<string> InputLines) {
            try {
                int SELCNT = SelectedLines();
                if (SELCNT == 0) {
                    for (int i = 0; i <= InputLines.Count - 1; i++)
                        LineNew(InputLines[i]);
                }
                else {
                    int FL = IndexCount;
                    int LL = FL + InputLines.Count - 1;
                    int I = 0;
                    for (int J = FL; J <= LL; J++) {
                        LineInsert(InputLines[I], J + 1, false, false);
                        I += 1;
                    }
                }
            }
            catch { }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineInsertBlank() {
            int SELCNT = SelectedLines();
            if (SELCNT == 0)
                LineNew("");
            else {
                int POS = 0;
                for (int I = 0; I <= SELCNT - 1; I++) {
                    for (int J = POS; J <= Lines.Count - 1; J++) {
                        if (Lines[J].Selected == true) {
                            POS = J + 1;
                            LineInsert("", J + 1, false, false);
                            break;
                        }
                    }
                }
                Invalidate();
            }
        }
        public void LineInsertAppend(string Input) {
            int SELCNT = SelectedLines();
            if (SELCNT == 0)
                LineNew(Input);
            else {
                int SELCURR = 0;
                for (int i = 0; i <= Lines.Count - 1; i++) {
                    if (Lines[i].Selected == true) {
                        Lines[i].Line += Input;
                        SELCURR += 1;
                    }
                    if (SELCURR > SELCNT)
                        break;
                }
                RefreshLongLine = true;
                Invalidate();
            }
        }
        public void LineInsertSelection(string Input) {
            int SELCNT = SelectedLines();
            if (SELCNT == 0)
                LineNew(Input);
            else {
                int POS = 0;
                for (int I = 0; I <= SELCNT - 1; I++) {
                    for (int J = POS; J <= Lines.Count - 1; J++) {
                        if (Lines[J].Selected == true) {
                            POS = J + 1;
                            LineInsert(Input, J + 1, false, false);
                            break;
                        }
                    }
                }
                RefreshLongLine = true;
                Invalidate();
            }
        }
        public void LineInsert(string Input, int Postion, bool Selected = false, bool Render = true) {
            int CORPOS = 0;
            if (Postion < 0)
                CORPOS = 0;
            else if (CORPOS > Lines.Count - 1)
                CORPOS = Lines.Count - 1;
            else
                CORPOS = Postion;
            LIL ITX = new LIL();
            ITX.Line = Input;
            ITX.Selected = Selected;
            Lines.Insert(CORPOS, ITX);
            RefreshLongLine = true;
            if (Render == true) {
                Invalidate();
            }
        }
        public void LineInsertFirst(string Input) {
            int START = 0;
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (Lines[i].Selected == true) {
                    START = i;
                    break;
                }
            }
            LineClearSelection(false);
            LineInsert(Input, START + 1, true, false);
            CentreLine(START + 1);
            Invalidate();
        }
        public void LineEditSelected(string Input) {
            int i;
            for (i = 0; i <= Lines.Count - 1; i++) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true)
                    Lines[i].Line = Input;
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineIndentSelected(DIR_INDENT Indention) {
            int i;
            string TabString = StringHandler.Space(5);
            for (i = 0; i <= Lines.Count - 1; i++) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true) {
                    if (Indention == DIR_INDENT.Increase)
                        Lines[i].Line = TabString + Lines[i].Line;
                    else if (Indention == DIR_INDENT.Decrease) {
                        string j = Lines[i].Line;
                        try {
                            if (j.StartsWith(TabString)) {
                                string n = j.Remove(0, 5);
                                Lines[i].Line = n;
                            }
                        }
                        catch {
                        }
                    }
                    else if (Indention == DIR_INDENT.RemoveAll) {
                        char sChars = ' ';
                        string j = Lines[i].Line;
                        string n = j.TrimStart(sChars).Trim();
                        Lines[i].Line = n;
                    }
                    else if (Indention == DIR_INDENT.TrimAll) {
                        char sChars = ' ';
                        string j = Lines[i].Line;
                        string n = j.TrimEnd(sChars);
                        Lines[i].Line = n;
                    }
                }
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineTrim(char TrimCharacter = ' ') {
            int i;
            for (i = 0; i <= Lines.Count - 1; i++) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true) {
                    string j = Lines[i].Line;
                    int Marker = -1;
                    for (int k = 0; i < j.Length; k++) {
                        if (j[k] == TrimCharacter) {
                            Marker = k;
                            break;
                        }
                    }
                    string n = j;
                    if (Marker != -1) {
                        n = j.Remove(Marker, j.Length - Marker);
                    }
                    Lines[i].Line = n;
                }
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineChangeCase(CasesLine CaseType) {
            int i;
            for (i = 0; i <= Lines.Count - 1; i++) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true) {
                    if (CaseType == CasesLine.UPPER)
                        Lines[i].Line = Lines[i].Line.ToUpper();
                    else if (CaseType == CasesLine.LOWER)
                        Lines[i].Line = Lines[i].Line.ToLower();
                    else if (CaseType == CasesLine.SENTANCE)
                        Lines[i].Line = StringHandler.ToSentenceCase(Lines[i].Line);
                    else if (CaseType == CasesLine.SENTANCE)
                        Lines[i].Line = CamelCase(Lines[1].Line);
                }
            }
            Invalidate();
        }
        public void LineDuplicateLast() {
            string LastString = Lines[Lines.Count - 1].Line;
            LIL item = new LIL();
            item.Line = LastString;
            Lines.Add(item);
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveLast() {
            try {
                Lines.RemoveAt(Lines.Count - 1);
            }
            catch {
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineRemoveDuplicates(bool CheckSubString = false, char SplitChar = ',', int Index = 0) {
            //int cnt = 0;
            List<int> Indexes = new List<int>();
            var PreviousValue = "";
            for (int i = 0; i <= Lines.Count - 1; i++) {
                string CurrentValue = "";
                if (i > 0) {
                    if (CheckSubString == true) {
                        STR_MVSSF Spilts = StringHandler.STR_MVSS(Lines[i].Line, SplitChar);
                        if ((Index < Spilts.Count)) {
                            CurrentValue = Spilts.Value[Index];
                            if ((PreviousValue == CurrentValue))
                                Indexes.Add(i);
                        }
                    }
                    else if ((PreviousValue == Lines[i].Line))
                        Indexes.Add(i);
                }
                if (CheckSubString == true) {
                    if (i > 0)
                        PreviousValue = CurrentValue;
                    else {
                        STR_MVSSF Spilts = StringHandler.STR_MVSS(Lines[i].Line, SplitChar);
                        if ((Index < Spilts.Count))
                            PreviousValue = Spilts.Value[Index];
                    }
                }
                else
                    PreviousValue = Lines[i].Line;
            }
            for (int i = Indexes.Count - 1; i >= 0; i += -1)
                Lines.RemoveAt(Indexes[i]);
            SelectedLines();
            Invalidate();
        }

        public void LineDuplicateDown() {
            for (int i = Lines.Count - 1; i >= 0; i += -1) {
                if (Lines[i].Selected == true) {
                    if (i < Lines.Count - 1) {
                        string new_val = Lines[i].Line;
                        LIL item = new LIL();
                        item.Line = Lines[i].Line;
                        Lines.Insert(i + 1, item);
                    }
                    else {
                        LIL item = new LIL();
                        item.Line = Lines[i].Line;
                        Lines.Add(item);
                    }
                }
            }
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineFlipSelected() {
            List<string> temp = new List<string>();
            List<int> post = new List<int>();
            int i;
            int j;
            for (i = Lines.Count - 1; i >= 0; i += -1) {
                LIL sKey = Lines.ElementAt(i);
                if (sKey.Selected == true) {
                    temp.Add(Lines[i].Line);
                    post.Add(i);
                }
            }
            for (j = 0; j <= post.Count - 1; j++) {
                if (Lines[post[j]].Selected == true) {
                    int inv = post.Count - 1 - j;
                    Lines[post[j]].Line = temp[inv];
                }
            }
            Invalidate();
        }
        public void LineSwapLines(int Firstline, int SecondLine) {
            int FL = 0;
            int LL = 0;
            if (Firstline >= 0 && Firstline <= Lines.Count - 1)
                FL = Firstline;
            if (SecondLine >= 0 && SecondLine <= Lines.Count - 1)
                FL = Firstline;
            string STR1 = Lines[FL].Line;
            string STR2 = Lines[LL].Line;
            Lines[FL].Line = STR2;
            Lines[LL].Line = STR1;
        }
        public void LineMerge(char Delimiter = ' ') {
            int sellines = SelectedLines();
            int FirstSelected = -1;
            for (int i = 0; i <= Lines.Count - 1; i++) {
                if (i <= Lines.Count - 1) {
                    LIL sKey = Lines.ElementAt(i);
                    if (sKey.Selected == true) {
                        if (FirstSelected == -1) {
                            FirstSelected = i;
                            Lines[i].Selected = false;
                        }
                        else
                            Lines[FirstSelected].Line += Delimiter + Lines[i].Line;
                    }
                }
            }
            LineRemoveSelectedWithoutRedraw();
            if (FirstSelected <= Lines.Count - 1)
                Lines[FirstSelected].Selected = true;
            RefreshLongLine = true;
            Invalidate();
        }
        public void LineSplit(char Delimiter = ' ') {
            int sellines = SelectedLines();
            int cs = 0;
            int cl = 0;
            if (sellines > 0) {
                for (cs = 0; cs <= sellines; cs++) {
                    for (int l = cl; l <= Lines.Count - 1; l++) {
                        if (l <= Lines.Count - 1) {
                            LIL sKey = Lines.ElementAt(l);
                            if (sKey.Selected == true) {
                                STR_MVSSFA SS = STR_MVSSA(sKey.Line.TrimStart(' '), Delimiter);
                                if (SS.Count <= 1) {
                                    Lines[l].Line = SS.Value[0];
                                    cl = l + 1;
                                }
                                else {
                                    for (int m = 0; m <= SS.Count - 1; m++) {
                                        if (m == 0)
                                            Lines[l].Line = SS.Value[0];
                                        else
                                            LineInsert(SS.Value[m], (l + m), true, false);
                                    }
                                    cl = SS.Count + l;

                                }
                                cs += 1;
                                break;
                            }
                        }
                    }
                }
            }
            RefreshLongLine = true;
            Invalidate();
        }
        #endregion
        #region Support Functions and Methods
        public int SelectedLines() {
            try {

                int cnt = 0;
                bool bol = false;
                for (int i = 0; i <= Lines.Count - 1; i++) {
                    if (Lines[i].Selected == true) {
                        cnt += 1;
                        if (bol == false) {
                            _IndexCount = i;
                            _CurrentString = Lines[i].Line;
                            bol = true;
                        }
                    }
                }
                _SelectionCount = cnt;
                _LineCount = Lines.Count;
                ValueChanged?.Invoke();
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
        private string CamelCase(string InputSentence) {
            string oString = "";
            int iLength = InputSentence.Length;
            int P = 1;
            string iChar;
            bool UpperFlag = false;
            InputSentence = InputSentence.ToUpper(); // convert to lower case string 
            while (P <= iLength) {
                iChar = InputSentence.Substring(P, 1);//Strings.Mid(InputSentence, P, 1);
                if (P == 1) {
                    UpperFlag = true;// change first character to upper case
                }
                if (iChar == " " | iChar == "-")
                    UpperFlag = true;
                else if (UpperFlag == true) {
                    iChar = iChar.ToLower();
                    UpperFlag = false;
                }
                oString += iChar;
                P += 1;
            }
            return (oString);
        }
        private void ClearSelected() {
            foreach (LIL itm in Lines) {
                if (itm.Selected == true)
                    itm.Selected = false;
            }
        }
        private string FormatCounter(int Count, int LeadingZeros = 5) {
            string CurrentString = Count.ToString();
            string Output = CurrentString;
            if (LeadingZeros <= CurrentString.Length) {
                return Output;
            }
            else {
                if (LeadingZeros > 0) {
                    int diff = LeadingZeros - CurrentString.Length;
                    Output = "";
                    for (int I = 0; I < diff; I++) {
                        Output += "0";
                    }
                    Output += CurrentString;
                    return Output;
                }
                else {
                    return Output;
                }
            }
        }
        private int GetFirstSelected() {
            for (int i = 0; i < Lines.Count; i++) {
                if (Lines[i].Selected == true) { return i; }
            }
            return -1;
        }
        #endregion
        #region Properties
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
        private Color _ColumnColor;
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
        private int _LineCountOffset;
        [System.ComponentModel.Category("Appearance")]
        public int LineCountOffset {
            get {
                return _LineCountOffset;
            }
            set {
                _LineCountOffset = value;
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
        private bool _ShowLineCount;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowLineCount {
            get {
                return _ShowLineCount;
            }
            set {
                _ShowLineCount = value;
                Invalidate();
            }
        }
        private bool _ShowGridLines;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowGridLines {
            get {
                return _ShowGridLines;
            }
            set {
                _ShowGridLines = value;
                Invalidate();
            }
        }
        private bool _ShowSyntaxHighlighting;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowSyntaxHighlighting {
            get {
                return _ShowSyntaxHighlighting;
            }
            set {
                _ShowSyntaxHighlighting = value;
                Invalidate();
            }
        }
        private bool _ShowHighlightColors;
        [System.ComponentModel.Category("Show/Hide")]
        public bool ShowHighlightColors {
            get {
                return _ShowHighlightColors;
            }
            set {
                _ShowHighlightColors = value;
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
        private int _VerScroll;
        [System.ComponentModel.Category("Scrolling")]
        public int VerScroll {
            get {
                return _VerScroll;
            }
            set {
                if (value < 0)
                    _VerScroll = 0;
                else if (value > Lines.Count - 1)
                    if (Lines.Count <= 0) { _VerScroll = 0; }
                    else {
                        _VerScroll = Lines.Count - 1;
                    }

                else
                    _VerScroll = value;

                if (InSelection == true) {
                    if (Tool == DIR_EDIT.SelectionTool) {
                        if (View == DIR_VIEW.ListLine) {
                            SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedLineStart, PointLineCalcuation.LineToPositiionScrollFactored));
                        }
                        else {
                            SelectionStart = TilePoint(new Point(SelectedLineStart, 0), PointLineCalcuation.LineToPositiionScrollFactored);
                        }
                        SelectValuesList(SELTEST);
                    }
                }
                Invalidate();
            }
        }
        private void InvokeScrollCheck() {
            if (_VerScroll > Lines.Count - 1) {
                if (Lines.Count <= 0) { _VerScroll = 0; }
                else { _VerScroll = Lines.Count - 1; }
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
        public int IndexCount {
            get {
                return _IndexCount;
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
                if (View == DIR_VIEW.ListLine) {
                    if (LongLine + LineCounterOffset > Width) {
                        if (value < 0)
                            _HorScroll = 0;
                        else if (value > (LongLine + LineCounterOffset))
                            _HorScroll = LongLine + LineCounterOffset;
                        else
                            _HorScroll = value;
                    }
                    else { _HorScroll = 0; }
                }
                else {
                    _HorScroll = 0;
                }
                Invalidate();
            }
        }
        [System.ComponentModel.Category("Scrolling")]
        private int scrollLines = 3;
        public int ScrollLines {
            get {
                if (View == DIR_VIEW.ListLine) {
                    return scrollLines;
                }
                else {
                    return TilesPerLine;
                }
            }
            set {
                scrollLines = value;
            }
        }
        private DIR_VIEW _View;
        [System.ComponentModel.Category("Appearance")]
        public DIR_VIEW View {
            get {
                return _View;
            }
            set {
                _View = value;
                if (value != DIR_VIEW.ListLine) {
                    ShowHorzScroll = false;
                    VerScroll = RoundToNearest(VerScroll, TilesPerLine);
                }
                else {
                    ShowHorzScroll = true;
                }
                Invalidate();
            }
        }
        private DIR_EDIT _Tool;
        [System.ComponentModel.Category("Control")]
        public DIR_EDIT Tool {
            get {
                return _Tool;
            }
            set {
                _Tool = value;
                Invalidate();
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
        protected override void OnPaint(PaintEventArgs e) {
            if (View == DIR_VIEW.ListLine)
                RenderList(e);
            else if (View == DIR_VIEW.Tiles)
                RenderTiles(e);
            else if (View == DIR_VIEW.ZoomOut)
                RenderTiles(e, true);
        }
        private void RenderTiles(PaintEventArgs e, bool ZoomOut = false) {
            try {
                RenderSetup(e);

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                int CurrentLine = VerScroll;
                for (int y = 0; y < TilesPerPage + 1; y++) {
                    int LineHeight = (int)((float)GenericLine_Height * TilePadding);
                    for (int x = 0; x < TilesPerLine; x++) {
                        Rectangle LineBounds = new Rectangle(TilePoint(new Point(x, y)), new Size(TileWidth, LineHeight));
                        if (CurrentLine < Lines.Count) {
                            RenderTile(e, CurrentLine, LineBounds, ZoomOut);
                        }
                        else {
                            break;
                        }
                        CurrentLine++;
                    }
                    if (ShowGridLines == true) {
                        using (SolidBrush GridLineBrush = new SolidBrush(GridlineColor)) {
                            using (Pen GridLinePen = new Pen(GridLineBrush)) {
                                Point StartPoint = new Point(0, LineHeight * y);
                                Point EndPoint = new Point(Width, LineHeight * y);
                                e.Graphics.DrawLine(GridLinePen, StartPoint, EndPoint);
                            }
                        }
                    }
                }
                if (ShowGridLines == true) {
                    for (int x = 0; x < TilesPerLine; x++) {
                        using (SolidBrush GridLineBrush = new SolidBrush(GridlineColor)) {
                            using (Pen GridLinePen = new Pen(GridLineBrush)) {
                                Point StartPoint = new Point(x * TileWidth, 0);
                                Point EndPoint = new Point(x * TileWidth, Height);
                                e.Graphics.DrawLine(GridLinePen, StartPoint, EndPoint);
                            }
                        }
                    }
                }
                if (InSelection == true && ShiftKey == false && CtrlKey == false) {
                    if (Tool == DIR_EDIT.SelectionTool) {
                        RenderSelectionRectangle(e, new Point(SelectionStart.X, ListLinePoint(SelectedLineStart)), SelectionEnd);
                    }
                    else {
                        RenderStretchToll(e, SelectionStart, SelectionEnd);
                    }
                }
                RenderScrollBar(e);
            }
            catch { }
        }
        private Color AlphaDark(Color Input, int Alpha) {
            double AlphaReduce = (255 - Alpha) / (double)255;
            int AR = (int)Math.Floor((double)Input.R * AlphaReduce);
            int AG = (int)Math.Floor((double)Input.G * AlphaReduce);
            int AB = (int)Math.Floor((double)Input.B * AlphaReduce);
            return Color.FromArgb(AR, AG, AB);
        }

        int LineCounterOffset = 0;
        int Offset = 5;
        int LineTextOffset = 0;
        int LongLine = 0;
        int Xscroll = 0;
        int LineHeaderHeight = 0;
        int GenericLine_Height = 0;
        int ScrollSize = 10;
        int MaximumVerticalLines = 100;
        int TileWidth = 10;
        float TilePadding = 3;
        int TilesPerLine = 10;
        int TilesPerPage = 10;
        private Point SelectionStart = new Point(0, 0);
        private Point SelectionEnd = new Point(0, 0);
        private int SelectedLineStart = -1;
        int DocumentHeight = 10;
        bool RunOnStartUpOnly = true;
        private void RenderSetup(PaintEventArgs e) {
            if (RunOnStartUpOnly == true) {
                try {
                    if (DefaultFont != null) {
                        using (System.Drawing.Font GenericSize = DefaultFont) {
                            ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
                        }
                    }
                    else {
                        using (System.Drawing.Font GenericSize = this.Font) {
                            ScrollSize = (int)e.Graphics.MeasureString("W", GenericSize).Width;
                        }
                    }
                }
                catch { }

            }
            int sz_genx = (int)e.Graphics.MeasureString("0", Font).Width - 2;
            GenericLine_Height = (int)e.Graphics.MeasureString("0", Font).Height;

            int HorizontalScrollHeight = 0;
            if (ShowHorzScroll == true) { HorizontalScrollHeight = ScrollSize; }

            MaximumVerticalLines = (int)Math.Floor((this.Height - LineHeaderHeight - HorizontalScrollHeight) / (double)GenericLine_Height);
            int LineOffset = 2;
            LineHeaderHeight = (int)GenericLine_Height + (LineOffset * 3);
            LineCounterOffset = (int)e.Graphics.MeasureString("0000000", Font).Width;
            InvokeScrollCheck();
            if (RefreshLongLine == true) {
                LongLine = LongestString(e);
                RefreshLongLine = false;
            }
            if (ShowLineCount == false) {
                LineCounterOffset = 0;
                LineHeaderHeight = 0;
            }
            if (View != DIR_VIEW.ListLine) {
                LineHeaderHeight = 0;
            }
            DocumentHeight = this.Height - LineHeaderHeight - HorizontalScrollHeight;
            // if ((LongLine * sz_genx) > this.Width + Offset) {
            if (LongLine > this.Width + Offset) {
                Xscroll = (int)HorScroll;// (int)((double)HorScroll / (double)100) * LongLine;// (LongLine * sz_genx);
            }
            else {
                Xscroll = 0;
            }
            LineTextOffset = LineCounterOffset + Offset - Xscroll;


            SetMaxScroll();
            if (View == DIR_VIEW.ZoomOut) {
                TileWidth = (int)e.Graphics.MeasureString("000000", Font).Width;
                TilePadding = 1;//1.5f;
            }
            else {
                TileWidth = (int)e.Graphics.MeasureString("W", Font).Width * 12;
                TilePadding = 3;
            }
            TilesPerLine = (int)Math.Floor((float)Width / (float)TileWidth);
            TilesPerPage = (int)Math.Floor(((float)Height) / ((float)GenericLine_Height * TilePadding));
            RunOnStartUpOnly = false;
        }
        private void RenderList(PaintEventArgs e) {
            try {
                RenderSetup(e);
                int CurrentStartingLine = 0;

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                int CurrentLine = VerScroll;
                // if( VerScroll < 0) { VerScroll = 0; }
                if (CurrentStartingLine < Lines.Count) {
                    for (int Line = CurrentStartingLine; Line < MaximumVerticalLines + 1; Line++) {
                        if (CurrentLine <= Lines.Count - 1) {
                            Rectangle LineBounds = new Rectangle(0, ListLinePoint(Line), Width, GenericLine_Height);
                            RenderLine(e, CurrentLine, LineBounds);
                            CurrentLine++;
                        }
                    }
                }


                ////// --------BOX HIGHLIGHT-----------
                if (InSelection == true && ShiftKey == false && CtrlKey == false) {
                    if (Tool == DIR_EDIT.SelectionTool) {
                        RenderSelectionRectangle(e, new Point(SelectionStart.X, ListLinePoint(SelectedLineStart)), SelectionEnd);
                    }
                    else {
                        RenderStretchToll(e, SelectionStart, SelectionEnd);
                    }
                }
                RenderHeader(e, LineHeaderHeight);
                if (ShowLineCount == true) {
                    Rectangle LineCountShadow = new Rectangle(LineCounterOffset, 0, 5, DocumentHeight + LineHeaderHeight);
                    using (LinearGradientBrush LineCounterBrush = new LinearGradientBrush(LineCountShadow, Color.FromArgb(100, 0, 0, 0), Color.Transparent, 0.0f)) {
                        e.Graphics.FillRectangle(LineCounterBrush, LineCountShadow);
                    }
                }
                RenderScrollBar(e);
                //// --------SHADOW-----------
                //int SSZ = 8;
                //Rectangle STP = new Rectangle(0, 0, Width, SSZ);
                //LinearGradientBrush STB = new LinearGradientBrush(STP, ShadowColor, Color.Transparent, 90);
                //Rectangle SBP = new Rectangle(0, Height - SSZ, Width, SSZ);
                //LinearGradientBrush SBB = new LinearGradientBrush(SBP, ShadowColor, Color.Transparent, 270);
                //// -------------------------
                //g.FillRectangle(STB, STP);
                //g.FillRectangle(SBB, SBP);
                //g = e.Graphics;
                //g.ResetClip();
                //e.Graphics.DrawImage(bmp, 0, 0, ClientSize.Width, ClientSize.Height);
            }
            // EditScroll()
            catch { }
            //  GC.Collect();
        }
        #region Element Rendering
        private void RenderLine(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            int LinePositionY = (int)(((float)BoundingRectangle.Height - (float)e.Graphics.MeasureString("0", Font).Height) / 2.0f) + BoundingRectangle.Y;
            RenderLineColouring(e, CurrentLine, BoundingRectangle);
            if (ShowGridLines == true) {
                using (SolidBrush GridLineBrush = new SolidBrush(GridlineColor)) {
                    using (Pen GridLinePen = new Pen(GridLineBrush)) {
                        Point StartPoint = new Point(BoundingRectangle.X, BoundingRectangle.Y + BoundingRectangle.Height);
                        Point EndPoint = new Point(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y + BoundingRectangle.Height);
                        e.Graphics.DrawLine(GridLinePen, StartPoint, EndPoint);
                    }
                }
            }
            using (SolidBrush TxtBrush = new SolidBrush(ForeColor)) {



                e.Graphics.DrawString(Lines[CurrentLine].Line, Font, TxtBrush, new Point(LineTextOffset, LinePositionY));
                if (ShowLineCount == true) {
                    RenderLineCounterColouring(e, CurrentLine, BoundingRectangle);
                    e.Graphics.DrawString(FormatCounter(CurrentLine + _LineCountOffset), Font, TxtBrush, new Point(Offset, LinePositionY));
                }

            }
        }
        private void RenderTile(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle, bool Compact = false) {

            RenderLineColouring(e, CurrentLine, BoundingRectangle);

            int LinePositionY = 0;
            int UnitLineHeight = (int)e.Graphics.MeasureString("0", Font).Height;
            if (Compact == true) {
                LinePositionY = (int)(((float)BoundingRectangle.Height - (float)UnitLineHeight) / 2.0f) + BoundingRectangle.Y;
            }
            else {
                LinePositionY = (int)(((float)BoundingRectangle.Height - (float)(UnitLineHeight * 2)) / 2.0f) + BoundingRectangle.Y;
            }
            using (SolidBrush TxtBrush = new SolidBrush(ForeColor)) {
                e.Graphics.DrawString(FormatCounter(CurrentLine + _LineCountOffset), Font, TxtBrush, new Point(BoundingRectangle.X, LinePositionY));
                if (Compact == false) {
                    using (StringFormat Formatting = new StringFormat(StringFormatFlags.NoWrap)) {
                        Formatting.Trimming = StringTrimming.EllipsisCharacter;
                        e.Graphics.DrawString(Lines[CurrentLine].Line, Font, TxtBrush, new Rectangle(BoundingRectangle.X, LinePositionY + UnitLineHeight, BoundingRectangle.Width, GenericLine_Height), Formatting);
                    }

                }
            }
        }
        private void RenderLineCounterColouring(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle) {
            Rectangle SmallerRectangle = new Rectangle(0, BoundingRectangle.Y, LineCounterOffset, BoundingRectangle.Height);
            if ((ShowRowColors == true)) {
                if (CurrentLine % 2 == 0) {
                    using (SolidBrush AlternLineColor = new SolidBrush(RowColor)) {
                        e.Graphics.FillRectangle(AlternLineColor, SmallerRectangle);
                    }
                }
                else {
                    using (SolidBrush AlternLineColor = new SolidBrush(BackColor)) {
                        e.Graphics.FillRectangle(AlternLineColor, SmallerRectangle);
                    }
                }
            }
            else {
                using (SolidBrush AlternLineColor = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(AlternLineColor, SmallerRectangle);
                }
            }
            if (ShowHighlightColors == true) {
                string CurrentLineString = Lines[CurrentLine].Line.ToLower().TrimStart(' ');
                for (int I = 0; I <= Highlights.Count - 1; I++) {
                    if (CurrentLineString.Contains(Highlights[I].SearchString.ToLower()) == true) {
                        Color HighlightColor = RenderHandler.DeterministicDarkenColor(Highlights[I].Highlight, BackColor, 128);
                        using (SolidBrush SelectedLineLight = new SolidBrush(Color.FromArgb(255, HighlightColor.R, HighlightColor.G, HighlightColor.B))) {
                            //Rectangle SmallHighlight = new Rectangle(0, BoundingRectangle.Y, LineTextOffset, BoundingRectangle.Height);
                            e.Graphics.FillRectangle(SelectedLineLight, SmallerRectangle);
                        }
                    }
                }
            }
            if (Lines[CurrentLine].Selected == true) {
                using (SolidBrush SelectedLineLight = new SolidBrush(Color.FromArgb(100, SelectedColor.R, SelectedColor.G, SelectedColor.B))) {
                    e.Graphics.FillRectangle(SelectedLineLight, SmallerRectangle);
                }
            }
        }
        private void RenderLineColouring(PaintEventArgs e, int CurrentLine, Rectangle BoundingRectangle, bool OffsetSelectBounds = true) {
            if ((ShowRowColors == true)) {
                if (CurrentLine % 2 == 0) {
                    using (SolidBrush AlternLineColor = new SolidBrush(RowColor)) {
                        e.Graphics.FillRectangle(AlternLineColor, BoundingRectangle);
                    }
                }
            }
            if (ShowHighlightColors == true) {
                string CurrentLineString = Lines[CurrentLine].Line.ToLower().TrimStart(' ');
                for (int I = 0; I <= Highlights.Count - 1; I++) {
                    if (CurrentLineString.Contains(Highlights[I].SearchString.ToLower()) == true) {
                        Color HighlightColor = RenderHandler.DeterministicDarkenColor(Highlights[I].Highlight, BackColor, 128);
                        Color FadedHighligher = Color.FromArgb(highlightalpha, HighlightColor.R, HighlightColor.G, HighlightColor.B);
                        using (SolidBrush Highlighter = new SolidBrush(FadedHighligher)) {
                            e.Graphics.FillRectangle(Highlighter, BoundingRectangle);
                        }
                        //break;
                    }
                }
            }
            if (Lines[CurrentLine].Selected == true) {
                using (SolidBrush SelectedLine = new SolidBrush(SelectedColor)) {
                    e.Graphics.FillRectangle(SelectedLine, BoundingRectangle);
                }
            }
        }
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
        private void RenderHeader(PaintEventArgs e, int HeaderHeight) {
            if (ShowLineCount == true) {
                using (SolidBrush HeaderBackBrush = new SolidBrush(ColumnColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, 0, 0, this.Width, HeaderHeight);
                }
                using (SolidBrush HeaderTextBrush = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString("Line", Font, HeaderTextBrush, new Point(Offset, 2));
                    e.Graphics.DrawString("Data", Font, HeaderTextBrush, new Point(LineCounterOffset + 3, 2));
                }
                Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
                using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(0, HeaderHeight), new Point(Width, HeaderHeight));
                    }
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
        private void RenderScrollBar(PaintEventArgs e) {
            Color BorderLineColor = RenderHandler.DeterministicDarkenColor(BackColor, BackColor, 100);
            if (ShowVertScroll == true) {
                VerticalScrollBar = new Rectangle(Width - ScrollSize, LineHeaderHeight, ScrollSize, Height - LineHeaderHeight);

                if (ShowHorzScroll == true) { VerticalScrollBar.Height -= ScrollSize; }

                using (SolidBrush HeaderBackBrush = new SolidBrush(BackColor)) {
                    e.Graphics.FillRectangle(HeaderBackBrush, VerticalScrollBar);
                }
                RenderVerticalBar(e);
                //using (SolidBrush ScrollBarBorderBrush = new SolidBrush(BorderLineColor)) {
                //    using (Pen ScrollBarBorderPen = new Pen(ScrollBarBorderBrush)) {
                //        //e.Graphics.DrawLine(ScrollBarBorderPen, new Point(VerticalScrollBar.X, VerticalScrollBar.Y), new Point(VerticalScrollBar.X, VerticalScrollBar.Height + ScrollSize));
                //        e.Graphics.DrawLine(ScrollBarBorderPen, new Point(VerticalScrollBar.X, VerticalScrollBar.Y), new Point(VerticalScrollBar.X, VerticalScrollBar.Y + VerticalScrollBar.Height));
                //    }
                //}
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
                //e.Graphics.FillRectangle(HeaderForeBrush, VerticalBar);
                ScrollBarButtonSize = ScrollSize;
                VerticalScrollBounds = new Rectangle(VerticalScrollBar.X, VerticalScrollBar.Y + ScrollBarButtonSize, VerticalScrollBar.Width, VerticalScrollBar.Height - (2 * ScrollBarButtonSize));
                //if (View == DIR_VIEW.ListLine) {
                if (Lines.Count > 0) {
                    float ViewableLines = ((float)MaximumVerticalLines / 2.0f) / (float)Lines.Count;
                    float ThumbHeight = ViewableLines * VerticalScrollBounds.Height;
                    if (ThumbHeight < ScrollBarButtonSize * 2) {
                        ThumbHeight = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (VerticalScrollBounds.Height - ThumbHeight) * ((float)VerScroll / (float)Lines.Count) + VerticalScrollBounds.Y;// + ScrollSize;
                    VerticalScrollThumb = new RectangleF(VerticalScrollBounds.X, ScrollBounds, VerticalScrollBar.Width, ThumbHeight);
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollThumb);
                }
                else {
                    e.Graphics.FillRectangle(HeaderForeBrush, VerticalScrollBounds);
                }
                // }
                //Buttons
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
                if (Lines.Count > 0) {
                    float LongLines = 1;
                    if (LongLine > 0) {
                        LongLines = LongLine;
                    }
                    float ViewableLines = (float)Width / (LongLines + LineCounterOffset);
                    float ThumbWidth = ViewableLines * HorizontalScrollBounds.Width;
                    if (ThumbWidth < ScrollBarButtonSize * 2) {
                        ThumbWidth = ScrollBarButtonSize * 2;
                    }
                    float ScrollBounds = (HorizontalScrollBounds.Width - ThumbWidth) * ((float)HorScroll / LongLines) + HorizontalScrollBounds.X;// + ScrollSize;
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
        bool InScrollBounds = false;
        bool ScrollStart = false;
        ScrollArea ScrollHit = ScrollArea.None;
        float ThumbDelta = 0;

        private enum ScrollArea {
            None = 0x00,
            Vertical = 0x01,
            Horizontal = 0x02
        }
        private int GetVerticalScrollFromCursor(int MousePositionY, float ThumbPosition) {
            if (View == DIR_VIEW.ListLine) {
                return (int)((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition) * Lines.Count) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
            }
            else {
                int PagesPerDocument = (int)Math.Ceiling((float)Lines.Count / (float)(TilesPerLine));
                float ScrollPercentage = ((float)((MousePositionY - VerticalScrollBounds.Y - ThumbPosition)) / (VerticalScrollBounds.Height - VerticalScrollThumb.Height));
                return (int)Math.Floor(ScrollPercentage * PagesPerDocument) * TilesPerLine;
            }
        }
        private int GetHorizontalScrollFromCursor(int MousePositionX, float ThumbPosition) {
            if (View == DIR_VIEW.ListLine) {
                return (int)((float)((MousePositionX - HorizontalScrollBounds.X - ThumbPosition) * (LongLine + LineCounterOffset)) / (HorizontalScrollBounds.Width - HorizontalScrollThumb.Width));
            }
            return 0;
        }
        private System.Timers.Timer ScrollOutofBounds;
        #endregion
        #region Selection Handling
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
            if (View == DIR_VIEW.ListLine) {
                bool Ret = false;
                if (SelectionBounds(SelectionStart.Y, SELTEST.Y).Y + 1 > GenericLine_Height) { Ret = true; }
                return Ret;
            }
            else {
                bool Ret = false;
                if (SelectionBounds(SelectionStart.Y, SELTEST.Y).Y + 1 >= (float)(GenericLine_Height * TilePadding)) {
                    Ret = true;
                }
                if (SelectionBounds(SelectionStart.X, SELTEST.X).Y + 1 >= TileWidth) {
                    Ret = true;
                }
                return Ret;
            }
        }
        private void CursorClickSelect(Point MSPOS) {
            int SelectInt = -1;
            if (View == DIR_VIEW.ListLine) {
                SelectInt = ListLinePoint(MSPOS.Y, PointLineCalcuation.PositionToLine);
            }
            else {
                SelectInt = TilePoint(MSPOS, PointLineCalcuation.PositionToLine).X;
            }
            if (CtrlKey == true) {
                try {
                    if (Lines[SelectInt].Selected == true) { Lines[SelectInt].Selected = false; }
                    else { Lines[SelectInt].Selected = true; }
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
                        if ((i >= 0) && (i < Lines.Count)) { Lines[i].Selected = true; }
                    }
                }
                else if (SelectInt < FirstSelection) {
                    for (int i = SelectInt; i <= FirstSelection; i++) {
                        if ((i >= 0) && (i < Lines.Count)) { Lines[i].Selected = true; }
                    }
                }
                else { if ((SelectInt >= 0) && (SelectInt < Lines.Count)) { Lines[SelectInt].Selected = true; } }
            }
            else if (CursorOutofBounds == false) {
                ClearSelected();
                if ((SelectInt >= 0) && (SelectInt < Lines.Count)) {
                    Lines[SelectInt].Selected = true;
                    FirstSelection = SelectInt;
                }
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
            if (View == DIR_VIEW.ListLine) {
                if (CursorOutofBounds == false) {
                    ClearSelected();
                }
                Start = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X, PointLineCalcuation.PositionToLine);
                Endl = ListLinePoint(SelectionBounds(SelectionStart.Y, SELTEST.Y).X + SelectionBounds(SelectionStart.Y, SELTEST.Y).Y, PointLineCalcuation.PositionToLine);
                if (Endl > Lines.Count - 1) { Endl = Lines.Count - 1; }
                if (Lines.Count > 1) {
                    for (int i = Start; i <= Endl; i++) {
                        if ((i >= 0) && (i < Lines.Count)) { Lines[i].Selected = true; }
                    }
                }
                return true;
            }
            else {
                int TempStart = SelectedLineStart;
                int TempStartColumnEnd = TilePoint(new Point(ClampTileSelection(SelectionEnd.X), SelectionStart.Y), PointLineCalcuation.PositionToLine).X;
                int TempEnd = TilePoint(new Point(ClampTileSelection(SelectionEnd.X), SelectionEnd.Y), PointLineCalcuation.PositionToLine).X;
                int Diff = 0;
                Debug.Print(TempStart.ToString() + " " + TempStartColumnEnd.ToString() + " " + TempEnd.ToString());
                if (TempStart <= TempEnd) {
                    if (TempStart <= TempStartColumnEnd) {
                        Diff = TempStartColumnEnd - TempStart;
                        Start = TempStart;
                        Endl = TempEnd;
                    }
                    else {
                        Diff = TempStart - TempStartColumnEnd - 1;
                        Start = TempStartColumnEnd;
                        Endl = TempEnd + Diff;
                    }
                }
                else {
                    if (TempStart <= TempStartColumnEnd) {
                        Diff = TempStartColumnEnd - TempStart;
                        Start = TempEnd - Diff;//-1;
                        Endl = TempStart;// - TilesPerLine;// + Diff;
                        if (Diff <= 0) {
                            Endl -= 2;
                        }
                    }
                    else {
                        Diff = TempStart - TempStartColumnEnd - 1;
                        Start = TempEnd;
                        Endl = TempStart - TilesPerLine;// - 1;
                    }
                }
                int CurrentSelection = Start;
                int LineY = (int)Math.Floor((float)((Endl - Diff) - Start) / (float)TilesPerLine);
                if ((LineY <= 0) && (Diff <= 0)) {
                    return false;
                }
                else {
                    if (CursorOutofBounds == false) {
                        ClearSelected();
                    }
                }
                for (int i = 0; i <= Diff; i++) {
                    for (int j = 0; j <= LineY; j++) {
                        CurrentSelection = Start + i + (TilesPerLine * j);
                        if ((CurrentSelection >= 0) && (Lines.Count > 0) && (CurrentSelection < Lines.Count)) {
                            Lines[CurrentSelection].Selected = true;
                        }
                    }
                }
                return true;
            }
        }
        private void SelectValuesList(Point MSPOS, bool LockOnMouseMove = false) {
            // Invalidate()
            try {
                bool Result = IsVaildSelectionBox();
                if (Result == true) {
                    Result = CursorMoveSelect();
                }
                //Debug.Print(Result.ToString());
                if (Result == false) {
                    if (LockOnMouseMove == false) {
                        CursorClickSelect(MSPOS);
                    }
                }
            }
            catch { }
            Invalidate();
        }
        #endregion
        private STR_MVSSFA STR_MVSSA(string value, char chr) {
            string rechr = chr.ToString();
            string fullval = value;
            string[] splitval = fullval.Split(rechr.ToCharArray());
            string[] cnt = fullval.Split(chr);
            var cp = new STR_MVSSFA();
            for (int i = 0; i <= cnt.Count() - 1; i++)
                cp.Value.Add(splitval[i]);
            cp.Count = cnt.Count();
            return cp;
        }
        #region Scroll Handling
        private void SetMaxScroll() {
            _VerScrollMax = Lines.Count + 1;
        }
        private int LongestString(PaintEventArgs e) {
            int CurrentMax = 0;
            int CompareMax = 0;
            int i = 0;
            int line = Lines.Count;
            foreach (LIL itm in Lines) {
                CompareMax = itm.Line.Length;
                if (CompareMax > CurrentMax) {
                    CurrentMax = CompareMax;
                    line = i;
                }
                i++;

            }
            if (line < Lines.Count) {
                CurrentMax = (int)e.Graphics.MeasureString(Lines[line].Line, Font).Width + 10;
            }
            return CurrentMax;
        }
        private void ScrollOutofBounds_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            // theweh
            if ((VerScroll < VerScrollMax) || (VerScroll > 0)) {
                if (View == DIR_VIEW.ListLine) {
                    VerScroll += ScrollOutofBoundsDelta.Y;
                }
                else {
                    VerScroll += RoundToNearest(ScrollOutofBoundsDelta.Y, TilesPerLine);
                }
            }
        }
        int RoundToNearest(int n, int x) {
            return (int)(Math.Round((float)n / (float)x) * (float)x);
        }
        #endregion
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
        private void LineInterface_MouseDown(object? sender, MouseEventArgs e) {
            // timer.Start()  
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if ((ShowVertScroll == true) && (e.X >= Width - ScrollSize)) {
                    ScrollHit = ScrollArea.Vertical;
                    if (ScrollStart == false) {
                        ThumbDelta = e.Y - VerticalScrollThumb.Y;
                        if (ThumbDelta < 0) {
                            ThumbDelta = 0;
                        }
                        else if (ThumbDelta > VerticalScrollThumb.Y + VerticalScrollThumb.Height) {
                            ThumbDelta = VerticalScrollThumb.Height;
                        }
                        ScrollStart = true;
                    }
                    InScrollBounds = true;
                }
                else if ((ShowHorzScroll == true) && (e.Y > Height - ScrollSize)) {
                    if (View == DIR_VIEW.ListLine) {
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
                }
                else {
                    ScrollHit = ScrollArea.None;
                    InScrollBounds = false;

                    if (Tool == DIR_EDIT.SelectionTool) {
                        if (CtrlKey == false & ShiftKey == false)
                            ClearSelected();
                    }
                    // If View = DIR_VIEW.ListLine Then
                    if (InSelection == false) {


                        if (View == DIR_VIEW.ListLine) {
                            SelectionStart = new Point(e.Location.X, e.Location.Y);
                            SelectedLineStart = ListLinePoint(e.Location.Y, PointLineCalcuation.PositionToLine);
                        }
                        else {
                            SelectionStart = new Point(ClampTileSelection(e.Location.X), e.Location.Y);
                            SelectedLineStart = TilePoint(new Point(ClampTileSelection(e.Location.X), e.Location.Y), PointLineCalcuation.PositionToLine).X;
                        }
                        InSelection = true;
                    }

                    SelectedLines();
                }
            }
        }

        Point ScrollOutofBoundsDelta = new Point(0, 0);
        private void LineInterface_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e) {

            if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Vertical)) {
                //if (View == DIR_VIEW.ListLine) {
                if (Lines.Count > 0) {
                    VerScroll = GetVerticalScrollFromCursor(e.Y, ThumbDelta);
                    Invalidate();
                }
                // }
            }
            else if ((InScrollBounds == true) && (ScrollHit == ScrollArea.Horizontal)) {
                //if (View == DIR_VIEW.ListLine) {
                if (Lines.Count > 0) {
                    HorScroll = GetHorizontalScrollFromCursor(e.X, ThumbDelta);
                    Invalidate();
                }
                // }
            }
            else {
                if (e.Location.Y > this.Height) {
                    if (View != DIR_VIEW.ListLine) {
                        ScrollOutofBoundsDelta.Y = (int)(Math.Ceiling((float)(e.Location.Y - this.Height) / 100.0f)) * ScrollLines;
                    }
                    else {
                        ScrollOutofBoundsDelta.Y = (int)(Math.Ceiling((float)(e.Location.Y - this.Height) / 10.0f)) * ScrollLines;
                    }

                    //if (VerScroll < VerScrollMax)
                    //    VerScroll += 1;
                    CursorOutofBounds = true;
                    ScrollOutofBounds.Enabled = true;
                }
                else if (e.Location.Y < 0) {
                    if (View != DIR_VIEW.ListLine) {
                        ScrollOutofBoundsDelta.Y = (int)(Math.Ceiling((float)(e.Location.Y) / 100.0f)) * ScrollLines;
                    }
                    else {
                        ScrollOutofBoundsDelta.Y = (int)(Math.Ceiling((float)(e.Location.Y) / 10.0f)) * ScrollLines;
                    }
                    //if (VerScroll > 0)
                    //    VerScroll -= 1;
                    ScrollOutofBounds.Enabled = true;
                    CursorOutofBounds = true;
                }
                else {
                    CursorOutofBounds = false;
                    ScrollOutofBounds.Enabled = false;
                }

                SELTEST = new Point(e.Location.X, e.Location.Y);
                if (InSelection == true) {
                    if (View == DIR_VIEW.ListLine) {
                        SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedLineStart, PointLineCalcuation.LineToPositiionScrollFactored));
                    }
                    else {
                        SelectionStart = TilePoint(new Point(SelectedLineStart, 0), PointLineCalcuation.LineToPositiionScrollFactored);
                    }
                    SelectionEnd = new Point(e.Location.X, e.Location.Y);
                    if (Tool == DIR_EDIT.SelectionTool) {
                        SelectValuesList(e.Location, true);
                        //if (SelectionBounds(SelectionStart.Y, SELTEST.Y).Y + 1 > GenericLine_Height) {
                        //    CursorSelect();
                        //
                        //}
                    }
                    else { Invalidate(); }
                }
                //if (View == DIR_VIEW.ListLine) {
                //    SELTEST = new Point(e.Location.X, e.Location.Y);
                //    if (InSelection == true) {
                //        SelectionStart = new Point(SelectionStart.X, ListLinePoint(SelectedLineStart, PointLineCalcuation.LineToPositiionScrollFactored));
                //        SelectionEnd = new Point(e.Location.X, e.Location.Y);
                //        if (Tool == DIR_EDIT.SelectionTool) {
                //            SelectValuesList(e.Location);
                //        }
                //        else { Invalidate(); }
                //    }
                //}
                //else {
                //    SELTEST = new Point(e.Location.X, e.Location.Y);
                //    if (InSelection == true) {
                //        SelectionEnd = new Point(e.Location.X, e.Location.Y);
                //        SelectValuesTile(e.Location);
                //    }
                //}
                SelectedLines();
            }
        }
        private void LineInterface_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e) {
            // timer.Stop()
            if (e.Button == MouseButtons.Left) {
                if (InScrollBounds == false) {
                    if (Tool == DIR_EDIT.MoveTool)
                        MoveValuesList();
                    InSelection = false;
                    SelectedLineStart = -1;
                    Invalidate();
                    SelectedLines();
                }
            }
            ScrollHit = ScrollArea.None;
            InScrollBounds = false;
            ScrollStart = false;
            ScrollOutofBounds.Enabled = false;
        }
        private void LineInterface_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e) {
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
                if (View == DIR_VIEW.ListLine) {
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
                if (e.Button == MouseButtons.Left) {
                    //if (View == DIR_VIEW.ListLine) {
                    if (Tool == DIR_EDIT.SelectionTool) {
                        SelectValuesList(e.Location);
                        //CursorClickSelect(e.Location);
                    }


                }
            }

        }
        private bool CursorOutofBounds = false;
        private void LineInterface_MouseEnter(object sender, System.EventArgs e) {
        }
        private void LineInterface_MouseHover(object sender, System.EventArgs e) {
        }
        private void GETSELECTTILE(Rectangle Input) {
            //int OFFSET = 10;
            int MAXH = MaxTilesWide;
            // Dim MAXT As Integer = Math.Floor(MaxTilesWide / sz_tilex)
            int TILW = sz_tilex;
            int TILH = sz_geny2;
            int FIRST = Yscroll;
            int CORX = Input.X; // - 10
            int CORY = Input.Y; // - 10
            int CORW = Input.Width; // - 10
            int CORH = Input.Height; // - 10

            int DISX = (int)Math.Floor((double)CORW / (double)TILW);
            int DISY = (int)Math.Floor((double)CORH / (double)TILH);
            int PNTX = (int)Math.Floor((double)CORX / (double)TILW);
            int PNTY = (int)Math.Floor((double)CORY / (double)TILH);
            if (DISX > (MAXH - 1))
                DISX = MAXH;
            int STR1 = PNTX + (PNTY * MAXH) + FIRST;
            try {
                for (int J = 0; J <= DISY; J++) {
                    int STR2 = PNTX + ((PNTY + J) * MAXH) + FIRST;
                    for (int I = 0; I <= DISX; I++) {
                        int CAL = I + STR2;
                        if (CAL >= 0 && CAL <= Lines.Count - 1)
                            Lines[CAL].Selected = true;
                    }
                }
            }
            catch {
            }
            Invalidate();
        }
        private int GETSELECTSINGLE(Point Input) {
            int MAXH = MaxTilesWide;
            // Dim MAXT As Integer = Math.Floor(MaxTilesWide / sz_tilex)
            int TILW = sz_tilex;
            int TILH = sz_geny2;
            int FIRST = Yscroll;
            int CORX = Input.X; // - 10
            int CORY = Input.Y; // - 10
            int PNTX = (int)Math.Floor((double)CORX / (double)TILW);
            int PNTY = (int)Math.Floor((double)CORY / (double)TILH);
            int STR1 = PNTX + (PNTY * MAXH) + FIRST;
            return STR1;
        }
        private void MoveValuesList() {
            int DIFF = AbsMovement();
            if (SelectionEnd.Y > SelectionStart.Y) {
                for (int i = 0; i <= DIFF - 1; i++)
                    LineMove(DIR_TYPE.DOWN);
            }
            else if (SelectionEnd.Y < SelectionStart.Y) {
                for (int i = 0; i <= DIFF - 1; i++)
                    LineMove(DIR_TYPE.UP);
            }
        }

        private void SelectValuesTile(Point MSPOS) {
            try {
                if (InSelection == true) {
                    Point PBX = SelectionBounds(SelectionStart.X, SelectionEnd.X);
                    Point PBY = SelectionBounds(SelectionStart.Y, SelectionEnd.Y);
                    int SX = PBX.X;
                    int SY = PBY.X;
                    int SW = PBX.Y;
                    int SH = PBY.Y;
                    Rectangle RECT = new Rectangle(SX, SY, SW, SH);
                    GETSELECTTILE(RECT);
                }
                else {
                }
            }
            catch {
            }
            Invalidate();
        }
        private void LineInterface_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e) {
            if (CtrlKey == false && ShiftKey == false) {
                int D = e.Delta;
                int DC = ScrollLines * (int)Math.Abs((double)D / (double)120);
                if (D > 0) {
                    if (VerScroll > 0)
                        VerScroll -= DC;
                }
                else if (VerScroll < VerScrollMax)
                    VerScroll += Math.Abs(DC);
            }
        }

        private void LineEditingInterface_Resize(object? sender, EventArgs e) {
            if (Width > LongLine + LineCounterOffset) {
                HorScroll = 0;
            }
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

                            case (int)Keys.Left: {
                                    Processed = true;
                                    break;
                                }

                            case (int)Keys.Right: {
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
            if (keyData == Keys.Left) {
                if (View != DIR_VIEW.ListLine) {
                    int First = GetFirstSelected() - 1;
                    if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                    else if (First < 0) { First = 0; }
                    CentreLine(First, true, true);
                    Invalidate();
                    return true;
                }
            }
            else if (keyData == Keys.Right) {
                if (View != DIR_VIEW.ListLine) {
                    int First = GetFirstSelected() + 1;
                    if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                    else if (First < 0) { First = 0; }
                    CentreLine(First, true, true);
                    Invalidate();
                    return true;
                }
            }
            if (keyData == Keys.Down) {
                SelectedLines();
                int inst = 0;
                if (IndexCount >= Lines.Count - 1)
                    inst = Lines.Count - 1;
                else
                    inst = IndexCount + 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                    Focus();
                }
                else {
                    if (View == DIR_VIEW.ListLine) {
                        int First = GetFirstSelected() + 1;
                        if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                        else if (First < 0) { First = 0; }
                        CentreLine(First, true, true);
                    }
                    else {
                        int First = GetFirstSelected() + TilesPerLine;
                        if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                        else if (First < 0) { First = 0; }
                        else {
                            CentreLine(First, true, true);
                        }

                    }
                }


                Invalidate();
                return true;
            }
            else if (keyData == Keys.Up) {
                SelectedLines();
                int inst = 0;
                if (IndexCount <= 0)
                    inst = 0;
                else
                    inst = IndexCount - 1;
                if (ShiftKey == true) {
                    Focus();
                    CentreLine(inst, true, false);
                }
                else {
                    if (View == DIR_VIEW.ListLine) {
                        int First = GetFirstSelected() - 1;
                        if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                        else if (First < 0) { First = 0; }
                        CentreLine(First, true, true);
                    }
                    else {
                        int First = GetFirstSelected() - TilesPerLine;
                        if (First >= Lines.Count - 1) { First = Lines.Count - 1; }
                        else if (First < 0) { First = 0; }
                        else {
                            CentreLine(First, true, true);
                        }

                    }

                }
                Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void CentreLine(int Line, bool SelectLine = true, bool ClearSelection = false) {
            if (Line < 0) { return; }
            //VerScroll = Line;
            if (View == DIR_VIEW.ListLine) {
                if (Lines.Count < MaximumVerticalLines) {
                    VerScroll = 0;
                }
                else {
                    //int First = GetFirstSelected();
                    if (Line == VerScroll) {
                        VerScroll = Line;
                    }
                    else if (Line >= (VerScroll + MaximumVerticalLines)) {
                        VerScroll = Line - MaximumVerticalLines;
                    }
                    else if (Line < VerScroll) {
                        VerScroll = Line;
                    }
                    else if ((Line >= VerScroll) && (Line <= (VerScroll + MaximumVerticalLines))) {

                    }
                }
            }
            else {
                if (Line == VerScroll) {
                    VerScroll = RoundToNearest(Line, TilesPerLine);
                }
                else if (Line >= (VerScroll + (TilesPerPage * TilesPerLine))) {
                    VerScroll = RoundToNearest(Line - (TilesPerPage * TilesPerLine), TilesPerLine);
                    if ((VerScroll + (TilesPerPage * TilesPerLine)) - 1 < Line) { VerScroll += TilesPerLine; }
                }
                else if (Line < TilesPerLine) {
                    VerScroll = 0;
                }
                else if (Line < VerScroll) {
                    VerScroll = RoundToNearest(Line, TilesPerLine);
                    if (VerScroll > Line) { VerScroll -= TilesPerLine; }
                }
                else if ((Line >= VerScroll) && (Line <= (VerScroll + MaximumVerticalLines))) {

                }
            }
            if (ClearSelection == true)
                LineClearSelection(false);
            if (SelectLine == true)
                LineSingleSelect(Line);
        }

        private void LineInterface_Load(object? sender, EventArgs e) {

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
        private enum PointLineCalcuation {
            LineToPositiion = 0x00,
            PositionToLine = 0x01,
            PositionToLineWithoutScroll = 0x02,
            LineToPositiionScrollFactored = 0x03
        }
    }
    #region Enums and Classes
    public class LIL {
        public string Line = "";
        public bool Selected = false;
    }
    public class LSEL {
        public string SearchString;
        public bool Enable = false;
        public Color Highlight;
        public LSEL(string searchString, Color highlight, bool enable = true) {
            SearchString = searchString;
            Enable = enable;
            Highlight = highlight;
        }
    }
    public class Patterning {
        public bool ApplyBefore;
        public bool ApplyAfter;

        public bool ApplyMiddle;

        public string StringMiddle;


        public string StringBefore;
        public string StringAfter;

        public Patterning(bool applyBefore, bool applyMiddle, bool applyAfter, string stringBefore, string stringMiddle, string stringAfter) {
            this.ApplyBefore = applyBefore;
            this.ApplyMiddle = applyMiddle;
            this.ApplyAfter = applyAfter;
            this.StringBefore = stringBefore;
            this.StringMiddle = stringMiddle;
            this.StringAfter = stringAfter;
        }
    }

    internal struct HL_LNE {
        public SolidBrush BR = new SolidBrush(Color.Black);
        public bool EN = false;
        public HL_LNE() {
        }
    }
    public enum DIR_TYPE {
        /// <summary>
        ///     ''' Move Up
        ///     ''' </summary>
        UP = 0,
        /// <summary>
        ///     ''' Move Down
        ///     ''' </summary>
        DOWN = 1
    }
    public enum DIR_VIEW {
        /// <summary>
        ///     ''' View As a list
        ///     ''' </summary>
        ListLine = 0,
        /// <summary>
        ///     ''' View as tiles
        ///     ''' </summary>
        Tiles = 1,
        /// <summary>
        ///     ''' View Numbers only
        ///     ''' </summary>
        ZoomOut = 2
    }
    public enum DIR_EDIT {
        SelectionTool = 0,
        MoveTool = 1,
        DuplicateTool = 2
    }
    public enum TextPosition {
        Same = 0,
        Higher = 1,
        Lower = 2
    }
    public enum DIR_INDENT {
        Increase = 0,
        Decrease = 1,
        RemoveAll = 2,
        TrimAll = 3
    }
    public enum SYN_HIGHTYPE {
        CommentLine = 0,
        Header = 1,
        Command = 2
    }
    public enum CasesLine {
        UPPER = 0,
        LOWER = 1,
        SENTANCE = 2,
        CAMEL = 3
    }
    public class STR_MVSSFA {
        public List<string> Value = new List<string>();
        public int Count;
    }
    public class STR_MVIA {
        public List<string> Values = new List<string>();
        public List<int> CutPostion = new List<int>();
    }
    #endregion
}
