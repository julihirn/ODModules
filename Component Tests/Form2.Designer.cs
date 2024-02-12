namespace Component_Tests {
    partial class Form2 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            bitScale1 = new ODModules.BitScale();
            SuspendLayout();
            // 
            // bitScale1
            // 
            bitScale1.DataFormat = ODModules.BitScale.BinaryFormat.Length4Bit;
            bitScale1.DownColor = Color.Empty;
            bitScale1.HoverColor = Color.Empty;
            bitScale1.InactiveForecolor = Color.Empty;
            bitScale1.IsSigned = false;
            bitScale1.Location = new Point(-4, 12);
            bitScale1.Name = "bitScale1";
            bitScale1.Size = new Size(303, 62);
            bitScale1.TabIndex = 0;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(296, 198);
            Controls.Add(bitScale1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
        }

        #endregion

        private ODModules.BitScale bitScale1;
    }
}