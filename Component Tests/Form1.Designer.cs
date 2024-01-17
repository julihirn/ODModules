namespace Component_Tests {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            ODModules.Column column1 = new ODModules.Column();
            ODModules.Column column2 = new ODModules.Column();
            ODModules.ListItem listItem1 = new ODModules.ListItem();
            ODModules.ListSubItem listSubItem1 = new ODModules.ListSubItem();
            ODModules.ListItem listItem2 = new ODModules.ListItem();
            ODModules.ListSubItem listSubItem2 = new ODModules.ListSubItem();
            ODModules.ListItem listItem3 = new ODModules.ListItem();
            ODModules.ListSubItem listSubItem3 = new ODModules.ListSubItem();
            ODModules.ListItem listItem4 = new ODModules.ListItem();
            ODModules.ListSubItem listSubItem4 = new ODModules.ListSubItem();
            ODModules.ListItem listItem5 = new ODModules.ListItem();
            ODModules.ListSubItem listSubItem5 = new ODModules.ListSubItem();
            ODModules.ListItem listItem6 = new ODModules.ListItem();
            contextMenu1 = new ODModules.ContextMenu();
            itemAToolStripMenuItem = new ToolStripMenuItem();
            itemBToolStripMenuItem = new ToolStripMenuItem();
            itemCToolStripMenuItem = new ToolStripMenuItem();
            button1 = new Button();
            button2 = new Button();
            bitToggle1 = new ODModules.BitToggle();
            textBox1 = new TextBox();
            propertyGrid1 = new PropertyGrid();
            menuStrip1 = new ODModules.MenuStrip();
            testToolStripMenuItem = new ToolStripMenuItem();
            aaToolStripMenuItem = new ToolStripMenuItem();
            bbToolStripMenuItem = new ToolStripMenuItem();
            ccToolStripMenuItem = new ToolStripMenuItem();
            textBox2 = new ODModules.TextBox();
            listControl1 = new ODModules.ListControl();
            contextMenu1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenu1
            // 
            contextMenu1.ActionSymbolForeColor = Color.FromArgb(200, 200, 200);
            contextMenu1.BorderColor = Color.Black;
            contextMenu1.DropShadowEnabled = false;
            contextMenu1.ForeColor = Color.White;
            contextMenu1.ImageScalingSize = new Size(32, 32);
            contextMenu1.InsetShadowColor = Color.FromArgb(128, 0, 0, 0);
            contextMenu1.Items.AddRange(new ToolStripItem[] { itemAToolStripMenuItem, itemBToolStripMenuItem, itemCToolStripMenuItem });
            contextMenu1.MenuBackColorNorth = Color.DodgerBlue;
            contextMenu1.MenuBackColorSouth = Color.DodgerBlue;
            contextMenu1.MouseOverColor = Color.FromArgb(127, 0, 0, 0);
            contextMenu1.Name = "contextMenu1";
            contextMenu1.SeparatorColor = Color.FromArgb(200, 200, 200);
            contextMenu1.ShowInsetShadow = true;
            contextMenu1.ShowItemInsetShadow = true;
            contextMenu1.Size = new Size(110, 70);
            // 
            // itemAToolStripMenuItem
            // 
            itemAToolStripMenuItem.Name = "itemAToolStripMenuItem";
            itemAToolStripMenuItem.Size = new Size(109, 22);
            itemAToolStripMenuItem.Text = "Item A";
            // 
            // itemBToolStripMenuItem
            // 
            itemBToolStripMenuItem.Name = "itemBToolStripMenuItem";
            itemBToolStripMenuItem.Size = new Size(109, 22);
            itemBToolStripMenuItem.Text = "Item B";
            // 
            // itemCToolStripMenuItem
            // 
            itemCToolStripMenuItem.Name = "itemCToolStripMenuItem";
            itemCToolStripMenuItem.Size = new Size(109, 22);
            itemCToolStripMenuItem.Text = "Item C";
            // 
            // button1
            // 
            button1.Location = new Point(12, 187);
            button1.Margin = new Padding(2, 1, 2, 1);
            button1.Name = "button1";
            button1.Size = new Size(81, 22);
            button1.TabIndex = 5;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 211);
            button2.Margin = new Padding(2, 1, 2, 1);
            button2.Name = "button2";
            button2.Size = new Size(81, 22);
            button2.TabIndex = 6;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // bitToggle1
            // 
            bitToggle1.ActiveToggleForeColor = Color.Black;
            bitToggle1.Bits = 32;
            bitToggle1.InactiveToggleForeColor = Color.Silver;
            bitToggle1.Location = new Point(386, 149);
            bitToggle1.MouseDownForeColor = Color.WhiteSmoke;
            bitToggle1.MouseOverForeColor = Color.Blue;
            bitToggle1.Name = "bitToggle1";
            bitToggle1.Size = new Size(252, 157);
            bitToggle1.TabIndex = 7;
            bitToggle1.TogglerSize = ODModules.BitToggle.WordSize.QWord;
            bitToggle1.Value = "0";
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Top;
            textBox1.Location = new Point(0, 24);
            textBox1.Margin = new Padding(2, 1, 2, 1);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(386, 23);
            textBox1.TabIndex = 8;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Right;
            propertyGrid1.Location = new Point(386, 24);
            propertyGrid1.Margin = new Padding(2, 1, 2, 1);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.SelectedObject = bitToggle1;
            propertyGrid1.Size = new Size(218, 294);
            propertyGrid1.TabIndex = 9;
            propertyGrid1.Click += propertyGrid1_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColorNorth = Color.DodgerBlue;
            menuStrip1.BackColorNorthFadeIn = Color.DodgerBlue;
            menuStrip1.BackColorSouth = Color.DodgerBlue;
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.ItemForeColor = Color.Black;
            menuStrip1.Items.AddRange(new ToolStripItem[] { testToolStripMenuItem });
            menuStrip1.ItemSelectedBackColorNorth = Color.White;
            menuStrip1.ItemSelectedBackColorSouth = Color.White;
            menuStrip1.ItemSelectedForeColor = Color.Black;
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.MenuBackColorNorth = Color.DodgerBlue;
            menuStrip1.MenuBackColorSouth = Color.DodgerBlue;
            menuStrip1.MenuBorderColor = Color.WhiteSmoke;
            menuStrip1.MenuSeparatorColor = Color.WhiteSmoke;
            menuStrip1.MenuSymbolColor = Color.WhiteSmoke;
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(3, 1, 0, 1);
            menuStrip1.Size = new Size(604, 24);
            menuStrip1.StripItemSelectedBackColorNorth = Color.White;
            menuStrip1.StripItemSelectedBackColorSouth = Color.White;
            menuStrip1.TabIndex = 10;
            menuStrip1.Text = "menuStrip1";
            menuStrip1.UseNorthFadeIn = false;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aaToolStripMenuItem, bbToolStripMenuItem, ccToolStripMenuItem });
            testToolStripMenuItem.ForeColor = Color.Black;
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(39, 22);
            testToolStripMenuItem.Text = "Test";
            // 
            // aaToolStripMenuItem
            // 
            aaToolStripMenuItem.ForeColor = Color.Black;
            aaToolStripMenuItem.Name = "aaToolStripMenuItem";
            aaToolStripMenuItem.Size = new Size(88, 22);
            aaToolStripMenuItem.Text = "aa";
            // 
            // bbToolStripMenuItem
            // 
            bbToolStripMenuItem.ForeColor = Color.Black;
            bbToolStripMenuItem.Name = "bbToolStripMenuItem";
            bbToolStripMenuItem.Size = new Size(88, 22);
            bbToolStripMenuItem.Text = "bb";
            // 
            // ccToolStripMenuItem
            // 
            ccToolStripMenuItem.ForeColor = Color.Black;
            ccToolStripMenuItem.Name = "ccToolStripMenuItem";
            ccToolStripMenuItem.Size = new Size(88, 22);
            ccToolStripMenuItem.Text = "cc";
            // 
            // textBox2
            // 
            textBox2.AutoCompleteMode = AutoCompleteMode.None;
            textBox2.AutoCompleteSource = AutoCompleteSource.None;
            textBox2.BackColor = SystemColors.Window;
            textBox2.BorderColor = Color.MediumSlateBlue;
            textBox2.BorderSize = 2;
            textBox2.Dock = DockStyle.Bottom;
            textBox2.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            textBox2.ForeColor = Color.DimGray;
            textBox2.Lines = new string[] { "textBox2" };
            textBox2.Location = new Point(0, 287);
            textBox2.Margin = new Padding(4);
            textBox2.MaxLength = 32767;
            textBox2.Multiline = false;
            textBox2.Name = "textBox2";
            textBox2.Padding = new Padding(7);
            textBox2.PasswordChar = '\0';
            textBox2.PlaceholderText = "";
            textBox2.ReadOnly = false;
            textBox2.SelectedBackColor = Color.LightGray;
            textBox2.SelectedBorderColor = Color.HotPink;
            textBox2.ShortcutsEnabled = true;
            textBox2.Size = new Size(386, 31);
            textBox2.TabIndex = 12;
            textBox2.TextAlign = HorizontalAlignment.Left;
            textBox2.UnderlinedStyle = false;
            textBox2.UseSystemPasswordChar = false;
            textBox2.WordWrap = true;
            // 
            // listControl1
            // 
            listControl1.AllowColumnSpanning = false;
            listControl1.AllowMouseWheel = true;
            listControl1.ColumnColor = Color.LightGray;
            listControl1.ColumnForeColor = Color.Black;
            listControl1.ColumnLineColor = Color.DimGray;
            column1.ColumnAlignment = ODModules.ColumnTextAlignment.Left;
            column1.CountOffset = 0;
            column1.DisplayType = ODModules.ColumnDisplayType.Text;
            column1.DropDownRight = false;
            column1.DropDownVisible = true;
            column1.FixedWidth = false;
            column1.ItemAlignment = ODModules.ItemTextAlignment.Left;
            column1.Text = "Column";
            column1.UseItemBackColor = false;
            column1.UseItemForeColor = false;
            column1.Visible = true;
            column1.Width = 120;
            column2.ColumnAlignment = ODModules.ColumnTextAlignment.Left;
            column2.CountOffset = 0;
            column2.DisplayType = ODModules.ColumnDisplayType.DropDown;
            column2.DropDownRight = false;
            column2.DropDownVisible = true;
            column2.FixedWidth = false;
            column2.ItemAlignment = ODModules.ItemTextAlignment.Left;
            column2.Text = "Column";
            column2.UseItemBackColor = false;
            column2.UseItemForeColor = false;
            column2.Visible = true;
            column2.Width = 120;
            listControl1.Columns.Add(column1);
            listControl1.Columns.Add(column2);
            listControl1.Dock = DockStyle.Fill;
            listControl1.DropDownMouseDown = Color.DimGray;
            listControl1.DropDownMouseOver = Color.LightGray;
            listControl1.ExternalItems = null;
            listControl1.Filter = null;
            listControl1.FilterColumn = 0;
            listControl1.GridlineColor = Color.LightGray;
            listControl1.HighlightStrength = 128;
            listControl1.HorizontalScrollStep = 3;
            listControl1.HorScroll = new decimal(new int[] { 0, 0, 0, 0 });
            listItem1.BackColor = Color.Transparent;
            listItem1.Checked = false;
            listItem1.ForeColor = Color.Black;
            listItem1.Indentation = 0U;
            listItem1.LineBackColor = Color.Transparent;
            listItem1.LineForeColor = Color.Black;
            listItem1.Name = "";
            listItem1.Selected = false;
            listSubItem1.BackColor = Color.Transparent;
            listSubItem1.Checked = false;
            listSubItem1.ForeColor = Color.Black;
            listSubItem1.Indentation = 0U;
            listSubItem1.Name = "";
            listSubItem1.Tag = null;
            listSubItem1.Text = "e3r2";
            listSubItem1.Value = 0;
            listItem1.SubItems.Add(listSubItem1);
            listItem1.Tag = null;
            listItem1.Text = "weqdq";
            listItem1.UseLineBackColor = false;
            listItem1.UseLineForeColor = false;
            listItem1.Value = 0;
            listItem2.BackColor = Color.Transparent;
            listItem2.Checked = false;
            listItem2.ForeColor = Color.Black;
            listItem2.Indentation = 0U;
            listItem2.LineBackColor = Color.Transparent;
            listItem2.LineForeColor = Color.Black;
            listItem2.Name = "";
            listItem2.Selected = false;
            listSubItem2.BackColor = Color.Transparent;
            listSubItem2.Checked = false;
            listSubItem2.ForeColor = Color.Black;
            listSubItem2.Indentation = 0U;
            listSubItem2.Name = "";
            listSubItem2.Tag = null;
            listSubItem2.Text = "ertert";
            listSubItem2.Value = 0;
            listItem2.SubItems.Add(listSubItem2);
            listItem2.Tag = null;
            listItem2.Text = "ertetr";
            listItem2.UseLineBackColor = false;
            listItem2.UseLineForeColor = false;
            listItem2.Value = 0;
            listItem3.BackColor = Color.Transparent;
            listItem3.Checked = false;
            listItem3.ForeColor = Color.Black;
            listItem3.Indentation = 0U;
            listItem3.LineBackColor = Color.Transparent;
            listItem3.LineForeColor = Color.Black;
            listItem3.Name = "";
            listItem3.Selected = false;
            listSubItem3.BackColor = Color.Transparent;
            listSubItem3.Checked = false;
            listSubItem3.ForeColor = Color.Black;
            listSubItem3.Indentation = 0U;
            listSubItem3.Name = "";
            listSubItem3.Tag = null;
            listSubItem3.Text = "weg";
            listSubItem3.Value = 0;
            listItem3.SubItems.Add(listSubItem3);
            listItem3.Tag = null;
            listItem3.Text = "regeg";
            listItem3.UseLineBackColor = false;
            listItem3.UseLineForeColor = false;
            listItem3.Value = 0;
            listItem4.BackColor = Color.Transparent;
            listItem4.Checked = false;
            listItem4.ForeColor = Color.Black;
            listItem4.Indentation = 0U;
            listItem4.LineBackColor = Color.Transparent;
            listItem4.LineForeColor = Color.Black;
            listItem4.Name = "";
            listItem4.Selected = false;
            listSubItem4.BackColor = Color.Transparent;
            listSubItem4.Checked = false;
            listSubItem4.ForeColor = Color.Black;
            listSubItem4.Indentation = 0U;
            listSubItem4.Name = "";
            listSubItem4.Tag = null;
            listSubItem4.Text = "weggew";
            listSubItem4.Value = 0;
            listItem4.SubItems.Add(listSubItem4);
            listItem4.Tag = null;
            listItem4.Text = "ergerge";
            listItem4.UseLineBackColor = false;
            listItem4.UseLineForeColor = false;
            listItem4.Value = 0;
            listItem5.BackColor = Color.Transparent;
            listItem5.Checked = false;
            listItem5.ForeColor = Color.Black;
            listItem5.Indentation = 0U;
            listItem5.LineBackColor = Color.Transparent;
            listItem5.LineForeColor = Color.Black;
            listItem5.Name = "";
            listItem5.Selected = true;
            listSubItem5.BackColor = Color.Transparent;
            listSubItem5.Checked = false;
            listSubItem5.ForeColor = Color.Black;
            listSubItem5.Indentation = 0U;
            listSubItem5.Name = "";
            listSubItem5.Tag = null;
            listSubItem5.Text = "rhthrt";
            listSubItem5.Value = 0;
            listItem5.SubItems.Add(listSubItem5);
            listItem5.Tag = null;
            listItem5.Text = "ergwgwe";
            listItem5.UseLineBackColor = false;
            listItem5.UseLineForeColor = false;
            listItem5.Value = 0;
            listItem6.BackColor = Color.Transparent;
            listItem6.Checked = false;
            listItem6.ForeColor = Color.Black;
            listItem6.Indentation = 0U;
            listItem6.LineBackColor = Color.Transparent;
            listItem6.LineForeColor = Color.Black;
            listItem6.Name = "";
            listItem6.Selected = false;
            listItem6.Tag = null;
            listItem6.Text = "ewggwe";
            listItem6.UseLineBackColor = false;
            listItem6.UseLineForeColor = false;
            listItem6.Value = 0;
            listControl1.Items.Add(listItem1);
            listControl1.Items.Add(listItem2);
            listControl1.Items.Add(listItem3);
            listControl1.Items.Add(listItem4);
            listControl1.Items.Add(listItem5);
            listControl1.Items.Add(listItem6);
            listControl1.LineMarkerIndex = 0;
            listControl1.Location = new Point(0, 47);
            listControl1.MarkerBorderColor = Color.LimeGreen;
            listControl1.MarkerFillColor = Color.FromArgb(100, 50, 205, 50);
            listControl1.MarkerStyle = ODModules.MarkerStyleType.Highlight;
            listControl1.Name = "listControl1";
            listControl1.RowColor = Color.LightGray;
            listControl1.ScrollBarMouseDown = Color.FromArgb(64, 0, 0, 0);
            listControl1.ScrollBarNorth = Color.DarkTurquoise;
            listControl1.ScrollBarSouth = Color.DeepSkyBlue;
            listControl1.ScrollItems = 3;
            listControl1.SelectedColor = Color.SkyBlue;
            listControl1.SelectionColor = Color.Gray;
            listControl1.ShadowColor = Color.FromArgb(128, 0, 0, 0);
            listControl1.ShowGrid = true;
            listControl1.ShowItemIndentation = false;
            listControl1.ShowMarker = false;
            listControl1.ShowRowColors = false;
            listControl1.Size = new Size(386, 240);
            listControl1.SpanColumn = 0;
            listControl1.TabIndex = 13;
            listControl1.UseLocalList = true;
            listControl1.VerScroll = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(604, 318);
            ContextMenuStrip = contextMenu1;
            Controls.Add(listControl1);
            Controls.Add(textBox2);
            Controls.Add(bitToggle1);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(propertyGrid1);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            contextMenu1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private Button button2;
        private ODModules.ContextMenu contextMenu1;
        private ToolStripMenuItem itemAToolStripMenuItem;
        private ToolStripMenuItem itemBToolStripMenuItem;
        private ToolStripMenuItem itemCToolStripMenuItem;
        private ODModules.BitToggle bitToggle1;
        private TextBox textBox1;
        private PropertyGrid propertyGrid1;
        private ODModules.MenuStrip menuStrip1;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem aaToolStripMenuItem;
        private ToolStripMenuItem bbToolStripMenuItem;
        private ToolStripMenuItem ccToolStripMenuItem;
        private ODModules.TextBox textBox2;
        private ODModules.ListControl listControl1;
    }
}