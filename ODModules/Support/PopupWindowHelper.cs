using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules.Support {
    public class PopupWindowHelperMessageFilter : IMessageFilter {
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_NCLBUTTONDOWN = 0x0A1;
        private const int WM_NCRBUTTONDOWN = 0x0A4;
        private const int WM_NCMBUTTONDOWN = 0x0A7;


        /// <summary>
        /// The popup form
        /// </summary>
        private Form ?popup = null;

        private Control ?TextBox = null;
        public PopupWindowHelperMessageFilter(Form popupW, Control textbox) {
            popup = popupW;
            TextBox = textbox;
            // MessageBox.Show(popup.Bounds.ToString());
            // MessageBox.Show(textbox.Bounds.ToString());
        }
        public Form ?Popup {
            get {
                return this.popup;
            }
            set {
                this.popup = value;
            }
        }
        public bool PreFilterMessage(ref Message m) {
            if (this.popup != null) {
                switch (m.Msg) {
                    case WM_LBUTTONDOWN:
                    case WM_RBUTTONDOWN:
                    case WM_MBUTTONDOWN:
                    case WM_NCLBUTTONDOWN:
                    case WM_NCRBUTTONDOWN:
                    case WM_NCMBUTTONDOWN:
                        OnMouseDown();
                        break;
                }
            }
            return false;
        }
        private void OnMouseDown() {
            // Get the cursor location
            Point cursorPos = Cursor.Position;
            // Check if it is within the popup form or textbox
            if (!popup.Bounds.Contains(cursorPos)) {
                if (!TextBox.Bounds.Contains(TextBox.Parent.PointToClient(cursorPos))) {
                    Application.RemoveMessageFilter(this);
                    popup.Close();
                }
            }

        }


    }
}
