using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public partial class BitScale : UserControl {
        public BitScale() {
            InitializeComponent();
        }
        private Color hovercolor;
        [Category("Appearance")]
        public Color HoverColor {
            get { return hovercolor; }
            set { hovercolor = value; Invalidate(); }
        }
        private Color downcolor;
        [Category("Appearance")]
        public Color DownColor {
            get { return downcolor; }
            set { downcolor = value; Invalidate(); }
        }
        private Color inactiveForecolor;
        [Category("Appearance")]
        public Color InactiveForecolor {
            get { return inactiveForecolor; }
            set { inactiveForecolor = value; Invalidate(); }
        }
        private BinaryFormat dataFormat;
        [Category("Binary Format")]
        public BinaryFormat DataFormat {
            get { return dataFormat; }
            set {
                dataFormat = value;
                if ((value & BinaryFormat.Signed) == BinaryFormat.Signed) {
                    isSigned = true;
                }
                else { isSigned = false; }
                Invalidate();
            }
        }
        private bool isSigned;
        [Category("Binary Format")]
        public bool IsSigned {
            get { return isSigned; }
            set {
                if (((int)dataFormat & 0x01000) == 0x01000) {
                    isSigned = true;
                }
                else {
                    isSigned = value;
                    if (value == true) { dataFormat |= BinaryFormat.Signed; }
                    else { dataFormat &= ~BinaryFormat.Signed; }
                }
                Invalidate();
            }
        }
        private void BitScale_Load(object sender, EventArgs e) {

        }
        bool IsCompact = false;
        int UnitHeight = 10;
        int UnitWide = 12;//10
        protected override void OnPaint(PaintEventArgs e) {
            UnitHeight = (int)e.Graphics.MeasureString("W", Font).Height;
            UnitWide = (int)(e.Graphics.MeasureString("X", Font).Width * 2.5f);
            DrawBitScale(e, new Point(10, 10));
        }
        private void DrawBitScale(PaintEventArgs e, Point StartPoint) {
            using (StringFormat TxtFormt = new StringFormat()) {
                int XOffset = 0;
                TxtFormt.Alignment = StringAlignment.Center;
                for (int i = 0; i < 11; i++) {
                    string CurrentScaler = IntBitSizeToString(i);
                    Rectangle TxtBox = new Rectangle(StartPoint.X + XOffset, StartPoint.Y, UnitWide, UnitHeight);
                    using (SolidBrush TxtBrush = new SolidBrush(ForeColor)) {
                        e.Graphics.DrawString(CurrentScaler, Font, TxtBrush, TxtBox, TxtFormt);
                    }
                    XOffset += TxtBox.Width;
                }
            }

        }
        private string IntBitSizeToString(int i) {
            switch (i) {
                case 0: return "4";
                case 1: return "8";
                case 2: return "12";
                case 3: return "16";
                case 4: return "24";
                case 5: return "32";
                case 6: return "64";
                case 7: return "128";
                case 8: return "256";
                case 9: return "Float";
                case 10: return "Double";
                default: return "";
            }
        }
        public enum BinaryFormat {
            Length4Bit = 0x00000,
            Length8Bit = 0x00001,
            Length12Bit = 0x00002,
            Length16Bit = 0x00004,
            Length24Bit = 0x00008,
            Length32Bit = 0x00010,
            Length64Bit = 0x00020,
            Length128Bit = 0x00040,
            Length256Bit = 0x00080,
            Signed = 0x10000,
            Float = 0x11010,
            Double = 0x11020,
        }
    }
}
