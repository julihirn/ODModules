using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ODModules {
    public class StatusMenu : System.Windows.Forms.StatusStrip {
        public StatusMenu() {
            Renderer = new MenuStripColorTable();
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
    }
}
