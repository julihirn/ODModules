using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace ODModules {
    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DefaultProperty("Color")]
    [DefaultEvent("ColorChanged")]
    [ToolboxBitmap(typeof(ColorWheel), "ColorWheelToolboxBitmap.bmp")]
    [ToolboxItem(true)]
    public class ColorWheel : Control {

        #region Private Fields

        private static readonly object _eventAlphaChanged = new object();

        private static readonly object _eventColorChanged = new object();

        private static readonly object _eventColorStepChanged = new object();

        private static readonly object _eventDisplayLightnessChanged = new object();

        private static readonly object _eventHslColorChanged = new object();

        private static readonly object _eventLargeChangeChanged = new object();

        private static readonly object _eventLightnessChanged = new object();

        private static readonly object _eventLineColorChanged = new object();

        private static readonly object _eventSecondarySelectionSizeChanged = new object();

        private static readonly object _eventSelectionSizeChanged = new object();

        private static readonly object _eventShowAngleArrowChanged = new object();

        private static readonly object _eventShowCenterLinesChanged = new object();

        private static readonly object _eventShowSaturationRingChanged = new object();

        private static readonly object _eventSmallChangeChanged = new object();

        private double _alpha;

        private Point[] _arrowHead;

        private Brush _brush;

        private PointF _centerPoint;

        private Color _color;

        private Color[] _colors;

        private int _colorStep;

        private bool _displayLightness;

        private HslColor _hslColor;

        private bool _isDragging;

        private int _largeChange;

        private double _lightness;

        private Color _lineColor;

        private Pen _linePen;

        private bool _lockUpdates;

        private PointF[] _points;

        private float _radius;

        private HslColor[] _secondaryColors;

        private int _secondarySelectionSize;

        private Image _selectionGlyph;

        private int _selectionSize;

        private bool _showAngleArrow;

        private bool _showCenterLines;

        private bool _showSaturationRing;

        private int _smallChange;

        private int _updateCount;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWheel"/> class.
        /// </summary>
        public ColorWheel() {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            _color = Color.Black;
            _hslColor = new HslColor(_color) {
                L = 0.5
            };
            _colorStep = 4;
            _selectionSize = 10;
            _smallChange = 1;
            _largeChange = 5;
            _lightness = 0.5;
            _alpha = 1;
            _lineColor = Color.DimGray;
            _secondaryColors = ColorWheel.GetEmptyColorArray();
            _secondarySelectionSize = 8;

            this.CreateLinePen();
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when the Alpha property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler AlphaChanged {
            add => this.Events.AddHandler(_eventAlphaChanged, value);
            remove => this.Events.RemoveHandler(_eventAlphaChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler ColorChanged {
            add => this.Events.AddHandler(_eventColorChanged, value);
            remove => this.Events.RemoveHandler(_eventColorChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler ColorStepChanged {
            add => this.Events.AddHandler(_eventColorStepChanged, value);
            remove => this.Events.RemoveHandler(_eventColorStepChanged, value);
        }

        /// <summary>
        /// Occurs when the DisplayLightness property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler DisplayLightnessChanged {
            add => this.Events.AddHandler(_eventDisplayLightnessChanged, value);
            remove => this.Events.RemoveHandler(_eventDisplayLightnessChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler HslColorChanged {
            add => this.Events.AddHandler(_eventHslColorChanged, value);
            remove => this.Events.RemoveHandler(_eventHslColorChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler LargeChangeChanged {
            add => this.Events.AddHandler(_eventLargeChangeChanged, value);
            remove => this.Events.RemoveHandler(_eventLargeChangeChanged, value);
        }

        /// <summary>
        /// Occurs when the Lightness property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler LightnessChanged {
            add => this.Events.AddHandler(_eventLightnessChanged, value);
            remove => this.Events.RemoveHandler(_eventLightnessChanged, value);
        }

        /// <summary>
        /// Occurs when the LineColor property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler LineColorChanged {
            add => this.Events.AddHandler(_eventLineColorChanged, value);
            remove => this.Events.RemoveHandler(_eventLineColorChanged, value);
        }

        /// <summary>
        /// Occurs when the SecondarySelectionSize property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler SecondarySelectionSizeChanged {
            add => this.Events.AddHandler(_eventSecondarySelectionSizeChanged, value);
            remove => this.Events.RemoveHandler(_eventSecondarySelectionSizeChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler SelectionSizeChanged {
            add => this.Events.AddHandler(_eventSelectionSizeChanged, value);
            remove => this.Events.RemoveHandler(_eventSelectionSizeChanged, value);
        }

        /// <summary>
        /// Occurs when the ShowAngleArrow property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler ShowAngleArrowChanged {
            add => this.Events.AddHandler(_eventShowAngleArrowChanged, value);
            remove => this.Events.RemoveHandler(_eventShowAngleArrowChanged, value);
        }

        /// <summary>
        /// Occurs when the ShowCenterLines property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler ShowCenterLinesChanged {
            add => this.Events.AddHandler(_eventShowCenterLinesChanged, value);
            remove => this.Events.RemoveHandler(_eventShowCenterLinesChanged, value);
        }

        /// <summary>
        /// Occurs when the ShowSaturationRing property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler ShowSaturationRingChanged {
            add => this.Events.AddHandler(_eventShowSaturationRingChanged, value);
            remove => this.Events.RemoveHandler(_eventShowSaturationRingChanged, value);
        }

        [Category("Property Changed")]
        public event EventHandler SmallChangeChanged {
            add => this.Events.AddHandler(_eventSmallChangeChanged, value);
            remove => this.Events.RemoveHandler(_eventSmallChangeChanged, value);
        }

        #endregion Public Events

        #region Public Properties

        [Category("Behavior")]
        [DefaultValue(1)]
        public double Alpha {
            get => _alpha;
            set {
                if (value < 0 || value > 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 1.");
                }

                if (Math.Abs(_alpha - value) > double.Epsilon) {
                    _alpha = value;

                    this.OnAlphaChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the component color.
        /// </summary>
        /// <value>The component color.</value>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        public virtual Color Color {
            get => _color;
            set {
                if (_color != value) {
                    _color = value;

                    this.OnColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the increment for rendering the color wheel.
        /// </summary>
        /// <value>The color step.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">Value must be between 1 and 359</exception>
        [Category("Appearance")]
        [DefaultValue(4)]
        public virtual int ColorStep {
            get => _colorStep;
            set {
                if (value < 1 || value > 359) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 1 and 359");
                }

                if (_colorStep != value) {
                    _colorStep = value;

                    this.OnColorStepChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DisplayLightness {
            get => _displayLightness;
            set {
                if (_displayLightness != value) {
                    _displayLightness = value;

                    this.OnDisplayLightnessChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font {
            get => base.Font;
            set => base.Font = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        /// <summary>
        /// Gets or sets the component color.
        /// </summary>
        /// <value>The component color.</value>
        [Category("Appearance")]
        [DefaultValue(typeof(HslColor), "0, 0, 0")]
        [Browsable(false) /* disable editing until I write a proper type convertor */]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual HslColor HslColor {
            get => _hslColor;
            set {
                if (_hslColor != value) {
                    _hslColor = value;

                    this.OnHslColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Color"/> property when the wheel selection is moved a large distance.
        /// </summary>
        /// <value>A numeric value. The default value is 5.</value>
        [Category("Behavior")]
        [DefaultValue(5)]
        public virtual int LargeChange {
            get => _largeChange;
            set {
                if (_largeChange != value) {
                    _largeChange = value;

                    this.OnLargeChangeChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(0.5)]
        public double Lightness {
            get => _lightness;
            set {
                if (value < 0 || value > 1) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 1.");
                }

                if (Math.Abs(_lightness - value) > double.Epsilon) {
                    _lightness = value;

                    _hslColor = new HslColor(_hslColor.H, _hslColor.S, value);

                    this.OnLightnessChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color LineColor {
            get => _lineColor;
            set {
                if (_lineColor != value) {
                    _lineColor = value;

                    this.CreateLinePen();

                    this.OnLineColorChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HslColor[] SecondaryColors {
            get => _secondaryColors;
            set {
                _secondaryColors = value ?? ColorWheel.GetEmptyColorArray();

                this.Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(8)]
        public int SecondarySelectionSize {
            get => _secondarySelectionSize;
            set {
                if (_secondarySelectionSize != value) {
                    _secondarySelectionSize = value;

                    this.OnSecondarySelectionSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the selection handle.
        /// </summary>
        /// <value>The size of the selection handle.</value>
        [Category("Appearance")]
        [DefaultValue(10)]
        public virtual int SelectionSize {
            get => _selectionSize;
            set {
                if (_selectionSize != value) {
                    _selectionSize = value;

                    this.OnSelectionSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowAngleArrow {
            get => _showAngleArrow;
            set {
                if (_showAngleArrow != value) {
                    _showAngleArrow = value;

                    this.OnShowAngleArrowChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowCenterLines {
            get => _showCenterLines;
            set {
                if (_showCenterLines != value) {
                    _showCenterLines = value;

                    this.CreateLinePen();

                    this.OnShowCenterLinesChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowSaturationRing {
            get => _showSaturationRing;
            set {
                if (_showSaturationRing != value) {
                    _showSaturationRing = value;

                    this.OnShowSaturationRingChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Color"/> property when the wheel selection is moved a small distance.
        /// </summary>
        /// <value>A numeric value. The default value is 1.</value>
        [Category("Behavior")]
        [DefaultValue(1)]
        public virtual int SmallChange {
            get => _smallChange;
            set {
                if (_smallChange != value) {
                    _smallChange = value;

                    this.OnSmallChangeChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        ///   Gets a value indicating whether painting of the control is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if painting of the control is allowed; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool AllowPainting => _updateCount == 0;

        [Obsolete("Do not use. This property will be removed in a future update.")]
        protected Color[] Colors {
            get => _colors;
            set => _colors = value;
        }

        [Obsolete("Do not use. This property will be removed in a future update.")]
        protected bool LockUpdates {
            get => _lockUpdates;
            set => _lockUpdates = value;
        }

        [Obsolete("Do not use. This property will be removed in a future update.")]
        protected PointF[] Points {
            get => _points;
            set => _points = value;
        }

        [Obsolete("Do not use. This property will be removed in a future update.")]
        protected Image SelectionGlyph {
            get => _selectionGlyph;
            set => _selectionGlyph = value;
        }

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        ///   Disables any redrawing of the image box
        /// </summary>
        public virtual void BeginUpdate() {
            _updateCount++;
        }

        /// <summary>
        ///   Enables the redrawing of the image box
        /// </summary>
        public virtual void EndUpdate() {
            if (_updateCount > 0) {
                _updateCount--;
            }

            if (this.AllowPainting) {
                this.Invalidate();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Calculates wheel attributes.
        /// </summary>
        protected virtual void CalculateWheel() {
            PointF[] points;
            Color[] colors;
            Size size;

            size = this.ClientSize;

            // Only define the points if the control is above a minimum size, otherwise if it's too small, you get an "out of memory" exceptions (of all things) when creating the brush
            if (size.Width > 16 && size.Height > 16 && _colorStep > 0) {
                int count;
                int w;
                int h;
                double l;
                double angle;

                count = 360 / _colorStep;
                points = new PointF[count];
                colors = new Color[count];
                angle = 0;

                w = size.Width;
                h = size.Height;
                l = _displayLightness
                  ? _lightness
                  : 0.5;

                _centerPoint = new PointF(w / 2.0F, h / 2.0F);
                _radius = this.GetRadius(_centerPoint);

                for (int i = 0; i < count; i++) {
                    double angleR;
                    PointF location;

                    angleR = angle * (Math.PI / 180);
                    location = this.GetColorLocation(angleR, _radius);

                    points[i] = location;
                    colors[i] = HslColor.HslToRgb(angle, 1, l);

                    angle += _colorStep;
                }
            }
            else {
                points = null;
                colors = null;
            }

            _points = points;
            _colors = colors;
        }

        /// <summary>
        /// Creates the gradient brush used to paint the wheel.
        /// </summary>
        protected virtual Brush CreateGradientBrush() {
            Brush result;

            if (_points != null && _points.Length != 0 && _points.Length == _colors.Length) {
                result = new PathGradientBrush(_points, WrapMode.Clamp) {
                    CenterPoint = _centerPoint,
                    CenterColor = Color.White,
                    SurroundColors = _colors
                };
            }
            else {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Creates the selection glyph.
        /// </summary>
        protected virtual Image CreateSelectionGlyph() => null;

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control" /> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _linePen?.Dispose();
                _linePen = null;

                this.DisposeOfWheelBrush();
                this.DisposeOfSelectionGlyph();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the point within the wheel representing the source color.
        /// </summary>
        /// <param name="color">The color.</param>
        protected PointF GetColorLocation(Color color) {
            return this.GetColorLocation(new HslColor(color));
        }

        /// <summary>
        /// Gets the point within the wheel representing the source color.
        /// </summary>
        /// <param name="color">The color.</param>
        protected virtual PointF GetColorLocation(HslColor color) {
            double angle;
            double radius;

            angle = this.GetHueAngle(color.H);
            radius = _radius * color.S;

            return this.GetColorLocation(angle, radius);
        }

        protected PointF GetColorLocation(double angleR, double radius) {
            Padding padding;
            double x;
            double y;

            padding = this.Padding;
            x = padding.Left + _centerPoint.X + Math.Cos(angleR) * radius;
            y = padding.Top + _centerPoint.Y - Math.Sin(angleR) * radius;

            return new PointF((float)x, (float)y);
        }

        protected float GetRadius(PointF centerPoint) {
            Padding padding;
            int offset;

            padding = this.Padding;
            offset = _showAngleArrow
              ? _selectionSize
              : _selectionSize / 2;

            return Math.Min(centerPoint.X, centerPoint.Y) - (Math.Max(padding.Horizontal, padding.Vertical) + offset);
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values.</param>
        /// <returns>true if the specified key is a regular input key; otherwise, false.</returns>
        protected override bool IsInputKey(Keys keyData) {
            bool result;

            if ((keyData & Keys.Left) == Keys.Left || (keyData & Keys.Up) == Keys.Up || (keyData & Keys.Down) == Keys.Down ||
                (keyData & Keys.Right) == Keys.Right || (keyData & Keys.PageUp) == Keys.PageUp ||
                (keyData & Keys.PageDown) == Keys.PageDown || (keyData & Keys.Home) == Keys.Home || (keyData & Keys.End) == Keys.End) {
                result = true;
            }
            else {
                result = base.IsInputKey(keyData);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified point is within the bounds of the color wheel.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the specified point is within the bounds of the color wheel; otherwise, <c>false</c>.</returns>
        protected bool IsPointInWheel(Point point) {
            PointF normalized;

            // http://my.safaribooksonline.com/book/programming/csharp/9780672331985/graphics-with-windows-forms-and-gdiplus/ch17lev1sec21

            normalized = new PointF(point.X - _centerPoint.X, point.Y - _centerPoint.Y);

            return normalized.X * normalized.X + normalized.Y * normalized.Y <= _radius * _radius;
        }

        /// <summary>
        /// Raises the <see cref="AlphaChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnAlphaChanged(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventAlphaChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ColorChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnColorChanged(EventArgs e) {
            EventHandler handler;

            if (!_lockUpdates) {
                this.HslColor = new HslColor(_color);
            }

            this.Refresh();

            handler = (EventHandler)this.Events[_eventColorChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ColorStepChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnColorStepChanged(EventArgs e) {
            EventHandler handler;

            this.RefreshWheel();

            handler = (EventHandler)this.Events[_eventColorStepChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="DisplayLightnessChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDisplayLightnessChanged(EventArgs e) {
            EventHandler handler;

            this.RefreshWheel();

            handler = (EventHandler)this.Events[_eventDisplayLightnessChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="HslColorChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnHslColorChanged(EventArgs e) {
            EventHandler handler;

            if (!_lockUpdates) {
                this.SetRgbColor(_hslColor);
            }

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventHslColorChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e) {
            if (this.IsNavigationKey(e.KeyCode)) {
                HslColor color;
                double hue;
                double saturation;
                int step;

                e.Handled = true;

                color = _hslColor;
                hue = color.H;
                saturation = color.S;

                step = e.Shift
                  ? _largeChange
                  : _smallChange;

                switch (e.KeyCode) {
                    case Keys.Right:
                        hue += step;
                        break;

                    case Keys.Up:
                        saturation += step / 100F;
                        break;

                    case Keys.Left:
                        hue -= step;
                        break;

                    case Keys.Down:
                        saturation -= step / 100F;
                        break;

                    case Keys.PageUp:
                        hue += _largeChange;
                        break;

                    case Keys.PageDown:
                        hue -= _largeChange;
                        break;

                    case Keys.Home:
                        saturation = 1;
                        break;

                    case Keys.End:
                        saturation = 0;
                        break;
                }

                if (hue >= 360) {
                    hue = 0;
                }
                else if (hue < 0) {
                    hue = 359;
                }

                if (saturation > 1) {
                    saturation = 1;
                }
                else if (saturation < 0) {
                    saturation = 0;
                }

                if (Math.Abs(hue - color.H) > double.Epsilon || Math.Abs(saturation - color.S) > double.Epsilon) {
                    color.H = hue;
                    color.S = saturation;

                    // As the Color and HslColor properties update each other, need to temporarily disable this and manually set both
                    // otherwise the wheel "sticks" due to imprecise conversion from RGB to HSL
                    _lockUpdates = true;
                    this.SetRgbColor(color);
                    this.HslColor = color;
                    _lockUpdates = false;
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="LargeChangeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnLargeChangeChanged(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventLargeChangeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="LightnessChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnLightnessChanged(EventArgs e) {
            EventHandler handler;

            if (_displayLightness) {
                this.RefreshWheel();
            }

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventLightnessChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="LineColorChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnLineColorChanged(EventArgs e) {
            EventHandler handler;

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventLineColorChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (!this.Focused && this.TabStop) {
                this.Focus();
            }

            if (e.Button == MouseButtons.Left && this.IsPointInWheel(e.Location)) {
                _isDragging = true;
                this.SetColor(e.Location);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (_isDragging) {
                this.SetColor(e.Location);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data. </param>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            _isDragging = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.PaddingChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnPaddingChanged(EventArgs e) {
            base.OnPaddingChanged(e);

            this.RefreshWheel();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (this.AllowPainting) {
                Control parent;
                Graphics g;

                this.OnPaintBackground(e); // HACK: Easiest way of supporting things like BackgroundImage, BackgroundImageLayout etc

                // if the parent is using a transparent color, it's likely to be something like a TabPage in a tab control
                // so we'll draw the parent background instead, to avoid having an ugly solid color
                parent = this.Parent;
                if (this.BackgroundImage == null && parent != null && (this.BackColor == parent.BackColor || parent.BackColor.A != 255)) {
                    ButtonRenderer.DrawParentBackground(e.Graphics, this.DisplayRectangle, this);
                }

                g = e.Graphics;

                if (_brush != null) {
                    g.FillEllipse(_brush, this.ClientRectangle);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                // HACK: smooth out the edge of the wheel.
                // https://github.com/cyotek/Cyotek.Windows.Forms.ColorPicker/issues/1 - the linked source doesn't do this hack yet draws with a smoother edge
                using (Pen pen = this.CreateSmoothingPen()) {
                    g.DrawEllipse(pen, new RectangleF(_centerPoint.X - _radius, _centerPoint.Y - _radius, _radius * 2, _radius * 2));
                }

                this.PaintCenterLines(g);

                if (!_color.IsEmpty) {
                    this.PaintSaturationRing(g);
                    this.PaintArrowHead(g);
                    this.PaintCustomColors(e);
                    this.PaintCurrentColor(e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Resize" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            this.RefreshWheel();
        }

        /// <summary>
        /// Raises the <see cref="SecondarySelectionSizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSecondarySelectionSizeChanged(EventArgs e) {
            EventHandler handler;

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventSecondarySelectionSizeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="SelectionSizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionSizeChanged(EventArgs e) {
            EventHandler handler;

            this.DisposeOfSelectionGlyph();

            this.RefreshWheel();

            handler = (EventHandler)this.Events[_eventSelectionSizeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ShowAngleArrowChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnShowAngleArrowChanged(EventArgs e) {
            EventHandler handler;

            this.RefreshWheel();

            handler = (EventHandler)this.Events[_eventShowAngleArrowChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ShowCenterLinesChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnShowCenterLinesChanged(EventArgs e) {
            EventHandler handler;

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventShowCenterLinesChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ShowSaturationRingChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnShowSaturationRingChanged(EventArgs e) {
            EventHandler handler;

            this.Invalidate();

            handler = (EventHandler)this.Events[_eventShowSaturationRingChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="SmallChangeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSmallChangeChanged(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventSmallChangeChanged];

            handler?.Invoke(this, e);
        }

        protected void PaintColor(PaintEventArgs e, HslColor color) {
            this.PaintColor(e, color, false);
        }

        protected virtual void PaintColor(PaintEventArgs e, HslColor color, bool includeFocus) {
            PointF location;

            location = this.GetColorLocation(color);

            if (!float.IsNaN(location.X) && !float.IsNaN(location.Y)) {
                int size;
                int x;
                int y;

                size = includeFocus
                  ? _selectionSize
                  : _secondarySelectionSize;

                x = (int)location.X - size / 2;
                y = (int)location.Y - size / 2;

                if (_selectionGlyph == null) {
                    using (Brush brush = new SolidBrush(color.ToRgbColor())) {
                        e.Graphics.FillEllipse(brush, x, y, size, size);
                    }

                    e.Graphics.DrawEllipse(_linePen, x, y, size, size);
                }
                else {
                    e.Graphics.DrawImage(_selectionGlyph, x, y);
                }

                if (this.Focused && includeFocus) {
                    NativeMethods.DrawFocusRectangle(e.Graphics, new Rectangle(x - 2, y - 2, size + 5, size + 5));
                }
            }
        }

        protected virtual void PaintCurrentColor(PaintEventArgs e) {
            this.PaintColor(e, _hslColor, true);
        }

        protected virtual void SetColor(Point point) {
            double dx;
            double dy;
            double angle;
            double distance;
            double saturation;
            Padding padding;
            HslColor newColor;

            padding = this.Padding;
            dx = Math.Abs(point.X - _centerPoint.X - padding.Left);
            dy = Math.Abs(point.Y - _centerPoint.Y - padding.Top);
            angle = Math.Atan(dy / dx) / Math.PI * 180;
            distance = Math.Pow(Math.Pow(dx, 2) + Math.Pow(dy, 2), 0.5);
            saturation = distance / _radius;

            if (point.X < _centerPoint.X) {
                angle = 180 - angle;
            }

            if (point.Y > _centerPoint.Y) {
                angle = 360 - angle;
            }

            newColor = new HslColor(angle, saturation, _lightness);

            if (_hslColor != newColor) {
                _lockUpdates = true;
                this.HslColor = newColor;
                this.SetRgbColor(_hslColor);
                _lockUpdates = false;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static HslColor[] GetEmptyColorArray() {

            return Array.Empty<HslColor>();
        }

        private void CreateLinePen() {
            _linePen?.Dispose();

            _linePen = new Pen(_lineColor);
        }

        private Pen CreateSmoothingPen() {
            Color color;

            color = this.BackColor;

            if (this.Parent is TabPage) {
                // HACK: Should probably try to get the
                // actual theme colour but perhaps Window
                // or White will do fine
                color = SystemColors.Window;
            }

            return new Pen(color, 2) {
                Alignment = PenAlignment.Outset
            };
        }

        private void DefineArrowHead() {
            _arrowHead = new[]
            {
        new Point(0,0),
        new Point(_selectionSize,0),
        new Point(0,_selectionSize)
      };
        }

        private void DisposeOfSelectionGlyph() {
            if (_selectionGlyph != null) {
                _selectionGlyph.Dispose();
                _selectionGlyph = null;
            }
        }

        private void DisposeOfWheelBrush() {
            if (_brush != null) {
                _brush.Dispose();
                _brush = null;
            }
        }

        private double GetHueAngle(double hue) {
            return hue * Math.PI / 180;
        }

        private bool IsNavigationKey(Keys keyCode) {
            return keyCode == Keys.Up
                   || keyCode == Keys.Down
                   || keyCode == Keys.Left
                   || keyCode == Keys.Right
                   || keyCode == Keys.PageUp
                   || keyCode == Keys.PageDown
                   || keyCode == Keys.Home
                   || keyCode == Keys.End;
        }

        private void PaintArrowHead(Graphics g) {
            if (_showAngleArrow) {
                PointF head;

                head = this.GetColorLocation(this.GetHueAngle(_hslColor.H), _radius);

                g.TranslateTransform(head.X, head.Y);
                g.RotateTransform(-(float)(_hslColor.H + 45));
                g.FillPolygon(Brushes.White, _arrowHead);
                g.DrawPolygon(_linePen, _arrowHead);

                g.ResetTransform();
            }
        }

        private void PaintCenterLine(Graphics g, HslColor color, bool fullRadius) {
            PointF start;

            start = fullRadius
              ? this.GetColorLocation(this.GetHueAngle(color.H), _radius)
              : this.GetColorLocation(color);

            g.DrawLine(_linePen, start, _centerPoint);
        }

        private void PaintCenterLines(Graphics g) {
            if (_showCenterLines) {
                if (!_hslColor.IsEmpty) {
                    this.PaintCenterLine(g, _hslColor, _showAngleArrow);
                }

                if (_secondaryColors != null && _secondaryColors.Length > 0) {
                    for (int i = 0; i < _secondaryColors.Length; i++) {
                        this.PaintCenterLine(g, _secondaryColors[i], false);
                    }
                }
            }
        }

        private void PaintCustomColors(PaintEventArgs e) {
            if (_secondaryColors != null && _secondaryColors.Length > 0) {
                for (int i = 0; i < _secondaryColors.Length; i++) {
                    this.PaintColor(e, _secondaryColors[i], false);
                }
            }
        }

        private void PaintSaturationRing(Graphics g) {
            if (_showSaturationRing) {
                float radius;

                radius = (float)(_radius * _hslColor.S);

                using (Pen pen = new Pen(HslColor.HslToRgb(0, 0, _hslColor.S))) {
                    g.DrawEllipse(pen, new RectangleF(_centerPoint.X - radius, _centerPoint.Y - radius, radius * 2, radius * 2));
                }
            }
        }

        /// <summary>
        /// Refreshes the wheel attributes and then repaints the control
        /// </summary>
        private void RefreshWheel() {
            this.CalculateWheel();

            this.DisposeOfWheelBrush();
            _brush = this.CreateGradientBrush();

            if (_selectionGlyph == null) {
                _selectionGlyph = this.CreateSelectionGlyph();
            }

            this.DefineArrowHead();

            this.Invalidate();
        }

        private void SetRgbColor(HslColor hslColor) {
            this.Color = hslColor.ToRgbColor(Convert.ToInt32(_alpha * 255));
        }

        #endregion Private Methods
    }

    [Serializable]
    public struct HslColor {
        #region Public Fields

        public static readonly HslColor Empty;

        #endregion Public Fields

        #region Private Fields

        private int _alpha;

        private double _hue;

        private bool _isEmpty;

        private double _lightness;

        private double _saturation;

        #endregion Private Fields

        #region Public Constructors

        static HslColor() {
            Empty = new HslColor {
                IsEmpty = true
            };
        }

        public HslColor(double hue, double saturation, double lightness)
          : this(255, hue, saturation, lightness) { }

        public HslColor(int alpha, double hue, double saturation, double lightness) {
            _hue = Math.Min(359, hue);
            _saturation = Math.Min(1, saturation);
            _lightness = Math.Min(1, lightness);
            _alpha = alpha;
            _isEmpty = false;
        }

        public HslColor(Color color) {
            _alpha = color.A;
            _hue = color.GetHue();
            _saturation = color.GetSaturation();
            _lightness = color.GetBrightness();
            _isEmpty = false;
        }

        #endregion Public Constructors

        #region Public Properties

        public int A {
            get { return _alpha; }
            set { _alpha = Math.Min(0, Math.Max(255, value)); }
        }

        public double H {
            get { return _hue; }
            set {
                _hue = value;

                if (_hue > 359) {
                    _hue = 0;
                }
                if (_hue < 0) {
                    _hue = 359;
                }
            }
        }

        public bool IsEmpty {
            get { return _isEmpty; }
            internal set { _isEmpty = value; }
        }

        public double L {
            get { return _lightness; }
            set { _lightness = Math.Min(1, Math.Max(0, value)); }
        }

        public double S {
            get { return _saturation; }
            set { _saturation = Math.Min(1, Math.Max(0, value)); }
        }

        #endregion Public Properties

        #region Public Methods

        public static implicit operator Color(HslColor color) {
            return color.ToRgbColor();
        }

        public static implicit operator HslColor(Color color) {
            return new HslColor(color);
        }

        public static bool operator !=(HslColor a, HslColor b) {
            return !(a == b);
        }

        public static bool operator ==(HslColor a, HslColor b) {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return a.H == b.H && a.L == b.L && a.S == b.S && a.A == b.A;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj) {
            bool result;

            if (obj is HslColor) {
                HslColor color;

                color = (HslColor)obj;
                result = this == color;
            }
            else {
                result = false;
            }

            return result;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public Color ToRgbColor() {
            return this.ToRgbColor(this.A);
        }

        public Color ToRgbColor(int alpha) {
            return HslColor.HslToRgb(alpha, _hue, _saturation, _lightness);
        }

        public override string ToString() {
            StringBuilder builder;

            builder = new StringBuilder();
            builder.Append(this.GetType().Name);
            builder.Append(" [");
            builder.Append("H=");
            builder.Append(this.H);
            builder.Append(", S=");
            builder.Append(this.S);
            builder.Append(", L=");
            builder.Append(this.L);
            builder.Append("]");

            return builder.ToString();
        }

        #endregion Public Methods

        #region Internal Methods

        internal static Color HslToRgb(double h, double s, double l) {
            return HslColor.HslToRgb(255, h, s, l);
        }

        internal static Color HslToRgb(int alpha, double h, double s, double l) {
            byte r;
            byte g;
            byte b;

            // https://www.programmingalgorithms.com/algorithm/hsl-to-rgb

            if (Math.Abs(s) < double.Epsilon) {
                r = g = b = Convert.ToByte(l * 255F);
            }
            else {
                double v1;
                double v2;
                double hue;

                hue = h / 360;

                v2 = l < 0.5
                  ? l * (1 + s)
                  : l + s - l * s;
                v1 = 2 * l - v2;

                r = HslColor.Clamp(255 * HslColor.HueToRgb(v1, v2, hue + 1.0f / 3));
                g = HslColor.Clamp(255 * HslColor.HueToRgb(v1, v2, hue));
                b = HslColor.Clamp(255 * HslColor.HueToRgb(v1, v2, hue - 1.0f / 3));
            }

            return Color.FromArgb(alpha, r, g, b);
        }

        #endregion Internal Methods

        #region Private Methods

        private static byte Clamp(double v) {
            if (v < 0) {
                v = 0;
            }

            if (v > 255) {
                v = 255;
            }

            return (byte)Math.Round(v);
        }

        private static double HueToRgb(double v1, double v2, double vH) {
            double result;

            if (vH < 0) {
                vH++;
            }

            if (vH > 1) {
                vH--;
            }

            if (6 * vH < 1) {
                result = v1 + (v2 - v1) * 6 * vH;
            }
            else if (2 * vH < 1) {
                result = v2;
            }
            else if (3 * vH < 2) {
                result = v1 + (v2 - v1) * (2.0f / 3 - vH) * 6;
            }
            else {
                result = v1;
            }

            return result;
        }

        #endregion Private Methods
    }

    internal static class NativeMethods {
        #region Public Fields

        /// <summary>
        ///   Logical pixels inch in X
        /// </summary>
        public const int LOGPIXELSX = 88;

        /// <summary>
        ///   Logical pixels inch in Y
        /// </summary>
        public const int LOGPIXELSY = 90;

        public const int R2_NOT = 6;

        public const int WH_KEYBOARD_LL = 13;

        public const int WH_MOUSE_LL = 14;

        public const int WM_KEYDOWN = 0x0100;

        public const int WM_LBUTTONDOWN = 0x0201;

        public const int WM_MOUSEMOVE = 0x0200;

        public const int WM_NCLBUTTONDOWN = 0x00A1;

        #endregion Public Fields

        #region Private Fields

        private const string _gdi32DllName = "gdi32.dll";

        private const string _user32DllName = "user32.dll";

        #endregion Private Fields

        #region Public Methods

        [DllImport(_user32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(_user32DllName)]
        public static extern bool DrawFocusRect(IntPtr hDC, [In] ref RECT lprc);

        public static void DrawFocusRectangle(Graphics g, Rectangle rectangle) {
            NativeMethods.DrawFocusRectangle(g, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }

        public static void DrawFocusRectangle(Graphics g, int x, int y, int w, int h) {
            NativeMethods.RECT rect;

            rect = new NativeMethods.RECT(x, y, x + w, y + h);

            // The Win32 API DrawFocusRect draws using an inverted brush and so works on black,
            // whereas ControlPaint.DrawFocusRect decidedly does not
            NativeMethods.DrawFocusRect(g.GetHdc(), ref rect);
            g.ReleaseHdc();
        }

        [DllImport(_user32DllName, EntryPoint = "GetDC", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        public static Point GetDesktopDpi() {
            IntPtr hWnd;
            IntPtr hDC;
            int dpix;
            int dpiy;

            hWnd = GetDesktopWindow();
            hDC = GetDC(hWnd);

            try {
                dpix = GetDeviceCaps(hDC, LOGPIXELSX);
                dpiy = GetDeviceCaps(hDC, LOGPIXELSY);
            }
            finally {
                ReleaseDC(hWnd, hDC);
            }

            return new Point(dpix, dpiy);
        }

        [DllImport(_user32DllName)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(_gdi32DllName)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(_gdi32DllName, EntryPoint = "LineTo", CallingConvention = CallingConvention.StdCall)]
        public static extern bool LineTo(IntPtr hdc, int x, int y);

        [DllImport(_gdi32DllName, EntryPoint = "MoveToEx", CallingConvention = CallingConvention.StdCall)]
        public static extern bool MoveToEx(IntPtr hdc, int x, int y, IntPtr lpPoint);

        [DllImport(_user32DllName, EntryPoint = "ReleaseDC", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(_gdi32DllName, EntryPoint = "SetROP2", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetROP2(IntPtr hdc, int fnDrawMode);

        [DllImport(_user32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(_user32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(_user32DllName, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        #endregion Public Methods

        #region Public Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;

            public int top;

            public int right;

            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        #endregion Public Structs
    }

}

