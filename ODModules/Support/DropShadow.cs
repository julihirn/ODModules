using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ODModules.Support {
    public class DropShadow {
        #region Shadowing

        #region Fields

        private bool _isAeroEnabled = false;
        private bool _isDraggingEnabled = false;
        private const int WM_NCHITTEST = 0x84;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int CS_DBLCLKS = 0x8;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        #endregion

        #region Structures

        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct MARGINS {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        #endregion
        public enum DWM_WINDOW_CORNER_PREFERENCE {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }
        public enum DWMWINDOWATTRIBUTE {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }
        #region Methods

        #region Public

        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);

        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsCompositionEnabled() {
            if (Environment.OSVersion.Version.Major < 6) return false;

            bool enabled;
            DwmIsCompositionEnabled(out enabled);

            return enabled;
        }

        #endregion

        #region Private

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
         );

        private bool CheckIfAeroIsEnabled() {
            if (Environment.OSVersion.Version.Major >= 6) {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);

                return (enabled == 1) ? true : false;
            }
            return false;
        }

        #endregion

        #region Overrides

        public void ApplyShadows(Form form) {
            var v = 2;

            DwmSetWindowAttribute(form.Handle, 2, ref v, 4);

            MARGINS margins = new MARGINS() {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };

            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }
        public void ApplyShadows(ToolStripDropDown form) {
            var v = 2;

            DwmSetWindowAttribute(form.Handle, 2, ref v, 4);
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL; // DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND
            DwmSetWindowAttribute(form.Handle, attribute, ref preference, sizeof(uint));
            MARGINS margins = new MARGINS() {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };

            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }
        public void ApplyShadows(ContextMenu form) {
            var v = 2;

            DwmSetWindowAttribute(form.Handle, 2, ref v, 4);

            MARGINS margins = new MARGINS() {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };

            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }
        public void ApplyShadows(MenuStrip form) {
            var v = 2;

            DwmSetWindowAttribute(form.Handle, 2, ref v, 4);

            MARGINS margins = new MARGINS() {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };

            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }

        #endregion

        #endregion

        #endregion
    }
}
