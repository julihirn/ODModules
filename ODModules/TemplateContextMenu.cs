using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ODModules {
    public partial class TemplateContextMenu : UserControl {

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;


       // [DllImport("user32")]
       // public static extern int SetParent
       //  (IntPtr hWndChild, IntPtr hWndNewParent);
       //
       // [DllImport("user32")]
       // public static extern int ShowWindow
       //  (IntPtr hWnd, int nCmdShow);


        public TemplateContextMenu() {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);
        }
        protected override bool ProcessDialogKey(Keys keyData) {// Alt+F4 is to closing
            if ((keyData & Keys.Alt) == Keys.Alt)
                if ((keyData & Keys.F4) == Keys.F4) {
                    this.Parent.Hide();
                    return true;
                }

            if ((keyData & Keys.Enter) == Keys.Enter) {
                if (this.ActiveControl.GetType() == typeof(Button)) {
                    Button Btn = (Button)this.ActiveControl;
                    //Btn.PerformClick();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }
        //protected override CreateParams CreateParams {
        //    get {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}
        private void TemplateContextMenu_Load(object sender, EventArgs e) {

        }
        //protected override bool ShowWithoutActivation {
        //    get { return true; }
        //}
       // [DllImport("user32.dll")]
       // static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
       // public new void Show(Control c) {
       //     if (c == null) throw new ArgumentNullException();
       //     _mControl = c;
       //     if (this.Handle == IntPtr.Zero) base.CreateControl();
       //     SetParent(base.Handle, IntPtr.Zero);
       //     ShowWindow(base.Handle, 1);
       // }
        private Control _mControl;
        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams ret = base.CreateParams;
        //        ret.Style = (int)WS_CHILD;
        //        ret.ExStyle |= (int)WS_EX_NOACTIVATE |
        //           (int)WS_EX_TOOLWINDOW;
        //        return ret;
        //    }
        //}
        //protected override void WndProc(ref Message m) {
        //    if (m.Msg == 0x86)  //WM_NCACTIVATE
        //    {
        //        if (m.WParam != IntPtr.Zero) //activate
        //        {
        //            SendMessage(_mControl.Handle, 0x86, (IntPtr)1, IntPtr.Zero);
        //        }
        //        this.DefWndProc(ref m);
        //        return;
        //    }
        //    base.WndProc(ref m);
        //}
        //public new void Show() {
        //    if (this.Handle == IntPtr.Zero) base.CreateControl();
        //
        //    //SetParent(base.Handle, IntPtr.Zero);
        //    //ShowWindow(base.Handle, 1);
        //}
    }
}
