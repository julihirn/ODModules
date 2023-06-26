using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {
    public class MenuStrip : System.Windows.Forms.MenuStrip {
        public MenuStrip() {
            Renderer = new MenuStripColorTable();
        }
        private Color ItemSelectedBackColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedBackColorNorth {
            get { return ItemSelectedBackColor; }
            set {
                ItemSelectedBackColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).ItemSelectedBackColorNorth = value;
                }
                Invalidate();
            }
        }
        private Color ItemSelectedBackColor2 = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedBackColorSouth {
            get { return ItemSelectedBackColor2; }
            set {
                ItemSelectedBackColor2 = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).ItemSelectedBackColorSouth = value;
                }
                Invalidate();
            }
        }
        private Color stripItemSelectedBackColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color StripItemSelectedBackColorNorth {
            get { return stripItemSelectedBackColor; }
            set {
                stripItemSelectedBackColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).StripItemSelectedBackColorNorth = value;
                }
                Invalidate();
            }
        }
        private Color stripItemSelectedBackColor2 = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color StripItemSelectedBackColorSouth {
            get { return stripItemSelectedBackColor2; }
            set {
                stripItemSelectedBackColor2 = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).StripItemSelectedBackColorSouth = value;
                }
                Invalidate();
            }
        }
        private bool useNorthFadeIn = false;
        [System.ComponentModel.Category("Appearance")]
        public bool UseNorthFadeIn {
            get { return useNorthFadeIn; }
            set {
                useNorthFadeIn = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).UseNorthFadeIn = value;
                }
                Invalidate();
            }
        }
        private Color backColorFadeIn = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorNorthFadeIn {
            get { return backColorFadeIn; }
            set {
                backColorFadeIn = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).BackColorNorthFadeIn = value;
                }
                Invalidate();
            }
        }
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private Color BackColor = Color.DodgerBlue;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorNorth {
            get { return BackColor; }
            set {
                BackColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).BackColorNorth = value;
                }
                Invalidate();
            }
        }
        private Color BackColor2 = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color BackColorSouth {
            get { return BackColor2; }
            set {
                BackColor2 = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).BackColorSouth = value;
                }
                Invalidate();
            }
        }
        private Color menuBackColor = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorNorth {
            get { return menuBackColor; }
            set {
                menuBackColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).MenuBackColorNorth = value;
                }
                Invalidate();
            }
        }
        private Color menuBackColor2 = Color.DodgerBlue;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBackColorSouth {
            get { return menuBackColor2; }
            set {
                menuBackColor2 = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).MenuBackColorSouth = value;
                }
                Invalidate();
            }
        }
        private Color itemSelectedForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemSelectedForeColor {
            get { return itemSelectedForeColor; }
            set {
                itemSelectedForeColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).ItemSelectedForeColor = value;
                }
                Invalidate();
            }
        }
        private Color itemForeColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color ItemForeColor {
            get { return itemForeColor; }
            set {
                itemForeColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).ItemForeColor = value;
                }
                Invalidate();
            }
        }
        private Color menuBorderColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuBorderColor {
            get { return menuBorderColor; }
            set {
                menuBorderColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).MenuBorderColor = value;
                }
                Invalidate();
            }
        }
        private Color menuSeparatorColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuSeparatorColor {
            get { return menuSeparatorColor; }
            set {
                menuSeparatorColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).MenuSeparatorColor = value;
                }
                Invalidate();
            }
        }
        private Color menuSymbolColor = Color.WhiteSmoke;
        [System.ComponentModel.Category("Appearance")]
        public Color MenuSymbolColor {
            get { return menuSymbolColor; }
            set {
                menuSymbolColor = value;
                if (Renderer.GetType() == typeof(MenuStripColorTable)) {
                    ((MenuStripColorTable)Renderer).MenuSymbolColor = value;
                }
                Invalidate();
            }
        }
        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e) {
            //Debug.Print(e.Item.ToString());
            foreach (object Tsmi in this.Items) {
                if (Tsmi.GetType() == typeof(ToolStripMenuItem)) {
                    ((ToolStripMenuItem)Tsmi).DropDown.DropShadowEnabled = false;
                }
            }
            if (e.Item.GetType() == typeof(ToolStripMenuItem)) {

                // e.Item.DisplaySt //).DropShadowEnabled = false;
            }
            base.OnItemAdded(e);
        }
    }

}
