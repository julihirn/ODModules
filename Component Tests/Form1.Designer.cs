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
            contextMenu1.Size = new Size(159, 118);
            // 
            // itemAToolStripMenuItem
            // 
            itemAToolStripMenuItem.Name = "itemAToolStripMenuItem";
            itemAToolStripMenuItem.Size = new Size(158, 38);
            itemAToolStripMenuItem.Text = "Item A";
            // 
            // itemBToolStripMenuItem
            // 
            itemBToolStripMenuItem.Name = "itemBToolStripMenuItem";
            itemBToolStripMenuItem.Size = new Size(158, 38);
            itemBToolStripMenuItem.Text = "Item B";
            // 
            // itemCToolStripMenuItem
            // 
            itemCToolStripMenuItem.Name = "itemCToolStripMenuItem";
            itemCToolStripMenuItem.Size = new Size(158, 38);
            itemCToolStripMenuItem.Text = "Item C";
            // 
            // button1
            // 
            button1.Location = new Point(23, 399);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 5;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(23, 451);
            button2.Name = "button2";
            button2.Size = new Size(150, 46);
            button2.TabIndex = 6;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // bitToggle1
            // 
            bitToggle1.ActiveToggleForeColor = Color.Black;
            bitToggle1.Bits = 32;
            bitToggle1.Dock = DockStyle.Fill;
            bitToggle1.InactiveToggleForeColor = Color.Silver;
            bitToggle1.Location = new Point(0, 81);
            bitToggle1.Margin = new Padding(6, 6, 6, 6);
            bitToggle1.MouseDownForeColor = Color.WhiteSmoke;
            bitToggle1.MouseOverForeColor = Color.Blue;
            bitToggle1.Name = "bitToggle1";
            bitToggle1.Size = new Size(519, 597);
            bitToggle1.TabIndex = 7;
            bitToggle1.Value = "0";
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Top;
            textBox1.Location = new Point(0, 42);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(519, 39);
            textBox1.TabIndex = 8;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Right;
            propertyGrid1.Location = new Point(519, 42);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.SelectedObject = bitToggle1;
            propertyGrid1.Size = new Size(405, 636);
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
            menuStrip1.Size = new Size(924, 42);
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
            testToolStripMenuItem.Size = new Size(76, 38);
            testToolStripMenuItem.Text = "Test";
            // 
            // aaToolStripMenuItem
            // 
            aaToolStripMenuItem.ForeColor = Color.Black;
            aaToolStripMenuItem.Name = "aaToolStripMenuItem";
            aaToolStripMenuItem.Size = new Size(359, 44);
            aaToolStripMenuItem.Text = "aa";
            // 
            // bbToolStripMenuItem
            // 
            bbToolStripMenuItem.ForeColor = Color.Black;
            bbToolStripMenuItem.Name = "bbToolStripMenuItem";
            bbToolStripMenuItem.Size = new Size(359, 44);
            bbToolStripMenuItem.Text = "bb";
            // 
            // ccToolStripMenuItem
            // 
            ccToolStripMenuItem.ForeColor = Color.Black;
            ccToolStripMenuItem.Name = "ccToolStripMenuItem";
            ccToolStripMenuItem.Size = new Size(359, 44);
            ccToolStripMenuItem.Text = "cc";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(924, 678);
            ContextMenuStrip = contextMenu1;
            Controls.Add(bitToggle1);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(propertyGrid1);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(6);
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
    }
}