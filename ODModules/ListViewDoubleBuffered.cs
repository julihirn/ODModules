using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace ODModules {
    public class ListViewDoubleBuffered : System.Windows.Forms.ListView {
        public ListViewDoubleBuffered() {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
            OwnerDraw = true;
        }
        public Color columnHeaderBackColor = Color.LightGray;
        [Category("Appearance")]
        public Color ColumnHeaderBackColor {
            get { return columnHeaderBackColor; }
            set {
                columnHeaderBackColor = value;
                Invalidate();
            }
        }
        public Color columnHeaderForeColor = Color.LightGray;
        [Category("Appearance")]
        public Color ColumnHeaderForeColor {
            get { return columnHeaderForeColor; }
            set {
                columnHeaderForeColor = value;
                Invalidate();
            }
        }
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e) {
            using (SolidBrush Br = new SolidBrush(columnHeaderBackColor)) {
                e.Graphics.FillRectangle(Br, e.Bounds);
            }
            using (SolidBrush Br = new SolidBrush(columnHeaderForeColor)) {
                if (e.Header != null) {
                    using (StringFormat Sf = new StringFormat()) {
                        Sf.LineAlignment = StringAlignment.Center;
                        Sf.Alignment = StringAlignment.Near;
                        Sf.Trimming = StringTrimming.EllipsisCharacter;
                        e.Graphics.DrawString(e.Header.Text, Font, Br, e.Bounds, Sf);
                    }
                }
                
            }
            //e.DrawText();
        }

        protected override void OnNotifyMessage(Message m) {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14) {
                base.OnNotifyMessage(m);
            }
        }

    }
}
