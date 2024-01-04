using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ODModules {
    [DefaultEvent("_TextChanged")]
    public partial class TextBox : UserControl {
        //Fields
        private Color borderColor = Color.MediumSlateBlue;
        private int borderSize = 2;
        private bool underlinedStyle = false;
        private Color borderFocusColor = Color.HotPink;
        private Color backFocusColor = Color.LightGray;
        private bool isFocused = false;
        [Browsable(true)]
        private bool IsFocused {
            get { return isFocused; }
            set {
                isFocused = value;
                textBox1.BackColor = isFocused == true ? backFocusColor : BackColor;
                this.Invalidate();
            }
        }
        //Constructor
        public TextBox() {
            InitializeComponent();
            DoubleBuffered = true;
            textBox1.Text = "";
        }

        //Events
        public event EventHandler _TextChanged;
        #region Appearance Properties
        //Properties
        [Category("Appearance")]
        public Color BorderColor {
            get { return borderColor; }
            set {
                borderColor = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        public int BorderSize {
            get { return borderSize; }
            set {
                borderSize = value;
                this.Invalidate();
            }
        }

        [Category("Appearance")]
        public bool UnderlinedStyle {
            get { return underlinedStyle; }
            set {
                underlinedStyle = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        public override Color ForeColor {
            get { return base.ForeColor; }
            set {
                base.ForeColor = value;
                textBox1.ForeColor = value;
            }
        }
        [Category("Appearance")]
        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                textBox1.Font = value;
                if (this.DesignMode)
                    UpdateControlHeight();
            }
        }
        [Category("Appearance")]
        [Browsable(true)]
        public override string Text {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
        [Category("Appearance")]
        public HorizontalAlignment TextAlign {
            get { return textBox1.TextAlign; }
            set { textBox1.TextAlign = value; }
        }
        [Category("Appearance")]
        public string PlaceholderText {
            get { return textBox1.PlaceholderText; }
            set { textBox1.PlaceholderText = value; }
        }
        [Category("Appearance")]
        public string[] Lines {
            get { return textBox1.Lines; }
            set { textBox1.Lines = value; }
        }
        [Category("Appearance")]
        public Color SelectedBorderColor {
            get { return borderFocusColor; }
            set { borderFocusColor = value; }
        }
        [Category("Appearance")]
        public Color SelectedBackColor {
            get { return backFocusColor; }
            set { backFocusColor = value; }
        }
        #endregion
        #region Behavior Properties
        [Category("Behavior")]
        public bool UseSystemPasswordChar {
            get { return textBox1.UseSystemPasswordChar; }
            set { textBox1.UseSystemPasswordChar = value; }
        }
        [Category("Behavior")]
        public char PasswordChar {
            get { return textBox1.PasswordChar; }
            set { textBox1.PasswordChar = value; }
        }
        [Category("Behavior")]
        public int MaxLength {
            get { return textBox1.MaxLength; }
            set { textBox1.MaxLength = value; }
        }
        [Category("Behavior")]
        public bool Multiline {
            get { return textBox1.Multiline; }
            set { textBox1.Multiline = value; }
        }
        [Category("Behavior")]
        public bool ReadOnly {
            get { return textBox1.ReadOnly; }
            set { textBox1.ReadOnly = value; }
        }
        [Category("Behavior")]
        public bool ShortcutsEnabled {
            get { return textBox1.ShortcutsEnabled; }
            set { textBox1.ShortcutsEnabled = value; }
        }
        [Category("Behavior")]
        public bool WordWrap {
            get { return textBox1.WordWrap; }
            set { textBox1.WordWrap = value; }
        }
        [Category("Appearance")]
        public override Color BackColor {
            get { return base.BackColor; }
            set {
                base.BackColor = value;
                textBox1.BackColor = value;
            }
        }
        #endregion

        [Category("Autocomplete")]
        public AutoCompleteStringCollection AutoCompleteCustomSource {
            get { return textBox1.AutoCompleteCustomSource; }
            set { textBox1.AutoCompleteCustomSource = value; }
        }
        [Category("Autocomplete")]
        public AutoCompleteSource AutoCompleteSource {
            get { return textBox1.AutoCompleteSource; }
            set { textBox1.AutoCompleteSource = value; }
        }
        [Category("Autocomplete")]
        public AutoCompleteMode AutoCompleteMode {
            get { return textBox1.AutoCompleteMode; }
            set { textBox1.AutoCompleteMode = value; }
        }
        //Overridden methods

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics graph = e.Graphics;
            Color TempBackColor = isFocused == true ? backFocusColor : BackColor;
            using (SolidBrush BackBr =  new SolidBrush(TempBackColor)) {
                e.Graphics.FillRectangle(BackBr, new Rectangle(0,0, Width, Height));
            }
            //Draw border
            using (Pen penBorder = new Pen(borderColor, borderSize)) {
                penBorder.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                if (isFocused) penBorder.Color = borderFocusColor;

                if (underlinedStyle) //Line Style
                    graph.DrawLine(penBorder, 0, this.Height - 1, this.Width, this.Height - 1);
                else //Normal Style
                    graph.DrawRectangle(penBorder, 0, 0, this.Width - 0.5F, this.Height - 0.5F);
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (this.DesignMode)
                UpdateControlHeight();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            UpdateControlHeight();
        }

        //Private methods
        private void UpdateControlHeight() {
            if (textBox1.Multiline == false) {
                int txtHeight = TextRenderer.MeasureText("Text", this.Font).Height + 1;
                textBox1.Multiline = true;
                textBox1.MinimumSize = new Size(0, txtHeight);
                textBox1.Multiline = false;

                this.Height = textBox1.Height + this.Padding.Top + this.Padding.Bottom;
            }
        }

        //TextBox events
        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (_TextChanged != null)
                _TextChanged.Invoke(sender, e);
        }

        private void textBox1_Click(object sender, EventArgs e) {
            this.OnClick(e);
        }

        private void textBox1_MouseEnter(object sender, EventArgs e) {
            this.OnMouseEnter(e);
        }

        private void textBox1_MouseLeave(object sender, EventArgs e) {
            this.OnMouseLeave(e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            this.OnKeyPress(e);
        }

        private void textBox1_Enter(object sender, EventArgs e) {
            IsFocused = true;
           
        }

        private void textBox1_Leave(object sender, EventArgs e) {
            IsFocused = false;
        }

        ///::::+
    }
}