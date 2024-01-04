using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {

    partial class TemplateContextMenuHost {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            this.DoubleBuffered = true;
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
                if (TemplateControl != null) {
                    System.Windows.Forms.Control _content = TemplateControl;
                    TemplateControl = null;
                    _content.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
