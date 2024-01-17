using ODModules.ColorPicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODModules {
    public class HueColorSlider : ColorSlider {
        #region Constructors

        public HueColorSlider() {
            this.BarStyle = ColorBarStyle.Custom;
            this.Maximum = 359;
            this.CustomColors = new ColorCollection(Enumerable.Range(0, 359).Select(h => HslColor.HslToRgb(h, 1, 0.5)));
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ColorBarStyle BarStyle {
            get { return base.BarStyle; }
            set { base.BarStyle = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color Color1 {
            get { return base.Color1; }
            set { base.Color1 = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color Color2 {
            get { return base.Color2; }
            set { base.Color2 = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color Color3 {
            get { return base.Color3; }
            set { base.Color3 = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override float Maximum {
            get { return base.Maximum; }
            set { base.Maximum = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override float Minimum {
            get { return base.Minimum; }
            set { base.Minimum = value; }
        }

        public override float Value {
            get { return base.Value; }
            set { base.Value = (int)value; }
        }

        #endregion
    }
}
