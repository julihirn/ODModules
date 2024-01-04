using ODModules.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {
    public partial class TemplateContextMenuHost : ToolStripDropDown, IMessageFilter {
        private Control? TemplateControl = null;
        private ToolStripControlHost Host;
        private bool Fade = true;


   
        public TemplateContextMenuHost(Control? popedControl, Form Parent) {
            InitializeComponent();
            this.parent = Parent;
            DropShadowEnabled = false;
            (new DropShadow()).ApplyShadows(this);
            SetStyle(ControlStyles.Selectable, false);

            if (popedControl == null)
                throw new ArgumentNullException("content");

            this.TemplateControl = popedControl;
            this.Fade = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;
            this.Host = new ToolStripControlHost(popedControl);


            Host.AutoSize = false;
            Padding = Margin = Host.Padding = Host.Margin = Padding.Empty;
            popedControl.Location = Point.Empty;
            this.Items.Add(Host);
            parent.Deactivate += TemplateContextMenuHost_Deactivate;

            popedControl.Disposed += delegate (object? sender, EventArgs e) {
                popedControl = null;
                Dispose(true);// this popup container will be disposed immediately after disposion of the contained control
            };
        }

        private void TemplateContextMenuHost_Deactivate(object? sender, EventArgs e) {
            parent.Activate();
        }

        private void Parent_LostFocus(object? sender, EventArgs e) {

        }

        protected override bool ProcessDialogKey(Keys keyData) {
            if ((keyData & Keys.Alt) == Keys.Alt)
                return false;

            return base.ProcessDialogKey(keyData);
        }
        public void Show(Control control) {
            if (control == null)
                throw new ArgumentNullException("control");

            Show(control, control.ClientRectangle);
        }
        public void ShowAndCentre(Control Hostcontrol) {
            if (Hostcontrol == null) { return; }
            if (TemplateControl == null) { return; }

            int XCentred = Hostcontrol.Location.X + ((Hostcontrol.Width - TemplateControl.Width) / 2);
            int YCentred = Hostcontrol.Location.Y + ((Hostcontrol.Height - TemplateControl.Height) / 2);
            Point p = new Point(XCentred, YCentred);
            p = Hostcontrol.PointToClient(p);
            Show(Hostcontrol, p, ToolStripDropDownDirection.BelowRight);
        }
        public void Show(Form f, Point p) {
            this.Show(f, new Rectangle(p, new Size(0, 0)));
        }
        private void Show(Control ParentControl, Rectangle Area) {
            if (ParentControl == null)
                throw new ArgumentNullException("control");
            Point location = ParentControl.PointToScreen(new Point(Area.Left, Area.Top + Area.Height));
            Rectangle screen = Screen.FromControl(ParentControl).WorkingArea;
            if (location.X + Size.Width > (screen.Left + screen.Width))
                location.X = (screen.Left + screen.Width) - Size.Width;
            if (location.Y + Size.Height > (screen.Top + screen.Height))
                location.Y -= Size.Height + Area.Height;
            location = ParentControl.PointToClient(location);
            Show(ParentControl, location, ToolStripDropDownDirection.BelowRight);
        }
        private const int AnimataionStep = 5;
        private const int TotalDuration = 100;
        private const int AnimationDuration = TotalDuration / AnimataionStep;
        protected override void SetVisibleCore(bool visible) {
            double opacity = Opacity;
            if (visible && Fade) Opacity = 0;
            base.SetVisibleCore(visible);
            if (!visible || !Fade) return;
            for (int i = 1; i <= AnimataionStep; i++) {
                if (i > 1) {
                    System.Threading.Thread.Sleep(AnimationDuration);
                }
                Opacity = opacity * (double)i / (double)AnimataionStep;
            }
            Opacity = opacity;
        }
        protected override void OnOpening(CancelEventArgs e) {
            if (TemplateControl == null) { return; }
            if (TemplateControl.IsDisposed || TemplateControl.Disposing) {
                e.Cancel = true;
                return;
            }
            base.OnOpening(e);
        }
        protected override void OnOpened(EventArgs e) {
            if (TemplateControl == null) { return; }
            TemplateControl.Focus();
            parent.ResumeLayout();
            base.OnOpened(e);
        }
        private const int WM_NCACTIVATE = 0x86;

       // [DllImport("user32.dll", CharSet = CharSet.Auto)]
       // public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
       // bool _activating = false;
       // protected override void WndProc(ref Message m) {
       //     // The popup needs to be activated for the user to interact with it,
       //     // but we want to keep the owner window's appearance the same.
       //     if ((m.Msg == WM_NCACTIVATE) && !_activating && (m.WParam != IntPtr.Zero)) {
       //         // The popup is being activated, ensure parent keeps activated appearance
       //         _activating = true;
       //         SendMessage(this.parent.Handle, WM_NCACTIVATE, (IntPtr)1, IntPtr.Zero);
       //         _activating = false;
       //         // Call base.WndProc here if you want the appearance of the popup to change
       //     }
       //     else {
       //         base.WndProc(ref m);
       //     }
       // }

        //protected override CreateParams CreateParams {
        //    get {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}

        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_NCLBUTTONDOWN = 0x0A1;
        private const int WM_NCRBUTTONDOWN = 0x0A4;
        private const int WM_NCMBUTTONDOWN = 0x0A7;
        Form parent;
        public Form HostParent {
            get { return parent; }
        }

        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,           // window handle
             int hWndInsertAfter,    // placement-order handle
             int X,          // horizontal position
             int Y,          // vertical position
             int cx,         // width
             int cy,         // height
             uint uFlags);       // window positioning flags

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void ShowInactiveTopmost(Control frm) {
        
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
      
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST, frm.Left, frm.Top, frm.Width, frm.Height, SWP_NOACTIVATE);
            //  frm.TopMost = false;
           // ;
        }
        public bool PreFilterMessage(ref Message m) {
            if (this != null) {
                switch (m.Msg) {
                    case WM_LBUTTONDOWN:
                    case WM_RBUTTONDOWN:
                    case WM_MBUTTONDOWN:
                    case WM_NCLBUTTONDOWN:
                    case WM_NCRBUTTONDOWN:
                    case WM_NCMBUTTONDOWN:
                        //OnMouseDown();
                        break;
                }
            }
            return false;
        }
    }
}
