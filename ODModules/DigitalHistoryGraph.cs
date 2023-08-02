using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public class DigitalHistoryGraph : Control {
        List<bool> DigitalData = new List<bool>();
        #region Properties
        private Color graphColorSouth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color GraphColorSouth {
            get { return graphColorSouth; }
            set {
                graphColorSouth = value;
                Invalidate();
            }
        }
        private Color graphColorNorth = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color GraphColorNorth {
            get { return graphColorNorth; }
            set {
                graphColorNorth = value;
                Invalidate();
            }
        }
        private int datapoints;

        public DigitalHistoryGraph() {
            DoubleBuffered = true;
        }

        public int DataPoints {
            get { return datapoints; }
            set {
                if (value > 0) {
                    datapoints = value;
                    DigitalData.Clear();
                    for(int i=0;i< datapoints - 1; i++) {
                        DigitalData.Add(false);
                    }
                }
                else {
                    DigitalData.Clear();
                }
                Invalidate();
            }
        }
        public void PushData(bool Data) {
            if (DigitalData.Count > 1) {
                for(int i = 0; i < DigitalData.Count - 1; i++) {
                    DigitalData[i] = DigitalData[i+1];
                }
                DigitalData[DigitalData.Count - 1] = Data;
                Invalidate();
            }
        }
        #endregion
        protected override void OnPaint(PaintEventArgs e) {
            if (DigitalData.Count > 0) {
                int BarWidth = (int)((float)Width / (float)DigitalData.Count);
                Rectangle DrawingBounds = new Rectangle(0, 0, Width, Height);
                using (LinearGradientBrush BarBrush = new LinearGradientBrush(DrawingBounds, graphColorSouth, graphColorNorth, 90)) {
                    for (int i = 0; i < DigitalData.Count; i++) {
                        int BarHeight = 0;
                        if (DigitalData[i] == true) { 
                            BarHeight = Height; 
                        }
                        e.Graphics.FillRectangle(BarBrush, new Rectangle(i * BarWidth, Height- BarHeight, BarWidth, BarHeight));
                    }
                }

            }
        }

        protected override void OnResize(EventArgs e) {
            Invalidate();
        }
    }
}
