using Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ODModules {
    public class SplashControl : UserControl {
        public SplashControl() {
            DoubleBuffered = true;
            Title = new SplashScreenText("Title", true, Font);
            Subtitle = new SplashScreenText("Subtitle", true, Font);
            Description = new SplashScreenText("Description", false, Font);
            Status = new SplashScreenText("Status Text...", false, Font);
            ApplicationLogo = new SplashScreenLogo();
        }
        SplashScreenImagePostion backgroundImagePosition = SplashScreenImagePostion.TopRight;
        [System.ComponentModel.Category("Appearance")]
        public SplashScreenImagePostion BackgroundImagePosition {
            get { return backgroundImagePosition; }
            set {
                backgroundImagePosition = value;
                Invalidate();
            }
        }
        float backgroundImageScaling = 1;
        [System.ComponentModel.Category("Appearance")]
        public float BackgroundImageScaling {
            get { return backgroundImageScaling; }
            set {
                backgroundImageScaling = value;
                Invalidate();
            }
        }
        Color insetShadowColor = Color.Black;
        [System.ComponentModel.Category("Appearance")]
        public Color InsetShadowColor {
            get {
                return insetShadowColor;
            }
            set {
                insetShadowColor = value;
                Invalidate();
            }
        }
        Color borderNorthColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderNorthColor {
            get {
                return borderNorthColor;
            }
            set {
                borderNorthColor = value;
                Invalidate();
            }
        }
        Color borderSouthColor = Color.DimGray;
        [System.ComponentModel.Category("Appearance")]
        public Color BorderSouthColor {
            get {
                return borderSouthColor;
            }
            set {
                borderSouthColor = value;
                Invalidate();
            }
        }
        Color backgroundNorthColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color BackgroundNorthColor {
            get {
                return backgroundNorthColor;
            }
            set {
                backgroundNorthColor = value;
                Invalidate();
            }
        }
        Color backgroundSouthColor = Color.White;
        [System.ComponentModel.Category("Appearance")]
        public Color BackgroundSouthColor {
            get {
                return backgroundSouthColor;
            }
            set {
                backgroundSouthColor = value;
                Invalidate();
            }
        }
        bool useDimmedBackground = true;
        [System.ComponentModel.Category("Appearance")]
        public bool UseDimmedBackground {
            get {
                return useDimmedBackground;
            }
            set {
                useDimmedBackground = value;
                Invalidate();
            }
        }
        bool drawInsetBorder = true;
        [System.ComponentModel.Category("Appearance")]
        public bool DrawInsetBorderShadow {
            get {
                return drawInsetBorder;
            }
            set {
                drawInsetBorder = value;
                Invalidate();
            }
        }
        int borderThickness = 10;
        [System.ComponentModel.Category("Appearance")]
        public int BorderThickness {
            get {
                return borderThickness;
            }
            set {
                borderThickness = value;
                Invalidate();
            }
        }
        SplashScreenText? applicationTitle;
        [System.ComponentModel.Category("Display")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public SplashScreenText? Title {
            get {
                return applicationTitle;
            }
            set {
                if (applicationTitle != null) {
                    applicationTitle.PropertyChanged -= delegate { this.Invalidate(); };
                }
                applicationTitle = value;
                if (applicationTitle != null) {
                    applicationTitle.PropertyChanged += delegate { this.Invalidate(); };
                }
                Invalidate();
            }
        }
        SplashScreenText? applicationSubTitle;
        [System.ComponentModel.Category("Display")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public SplashScreenText? Subtitle {
            get {
                return applicationSubTitle;
            }
            set {
                if (applicationSubTitle != null) {
                    applicationSubTitle.PropertyChanged -= delegate { this.Invalidate(); };
                }
                applicationSubTitle = value;
                if (applicationSubTitle != null) {
                    applicationSubTitle.PropertyChanged += delegate { this.Invalidate(); };
                }
                Invalidate();

            }
        }
        SplashScreenText? applicationDescription;
        [System.ComponentModel.Category("Display")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public SplashScreenText? Description {
            get {
                return applicationDescription;
            }
            set {
                if (applicationDescription != null) {
                    applicationDescription.PropertyChanged -= delegate { this.Invalidate(); };
                }
                applicationDescription = value;
                if (applicationDescription != null) {
                    applicationDescription.PropertyChanged += delegate { this.Invalidate(); };
                }
                Invalidate();
            }
        }
        SplashScreenText? applicationStatus;
        [System.ComponentModel.Category("Display")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public SplashScreenText? Status {
            get {
                return applicationStatus;
            }
            set {
                if (applicationStatus != null) {
                    applicationStatus.PropertyChanged -= delegate { this.Invalidate(); };
                }
                applicationStatus = value;
                if (applicationStatus != null) {
                    applicationStatus.PropertyChanged += delegate { this.Invalidate(); };
                }
                Invalidate();
            }
        }

        SplashScreenLogo? logo;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [System.ComponentModel.Category("Display")]
        public SplashScreenLogo? ApplicationLogo {
            get {
                return logo;
            }
            set {
                if (logo != null) {
                    logo.PropertyChanged -= delegate { this.Invalidate(); };
                }
                logo = value;
                if (logo != null) {
                    logo.PropertyChanged += delegate { this.Invalidate(); };
                }
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            DrawBackground(e);
            DrawTitles(e);
            DrawInsetShadow(e);
        }
        private void DrawTitles(PaintEventArgs e) {
            int VerticalOffset = 0;

            VerticalOffset += GetLogoSize(e, logo).Height;
            VerticalOffset += GetTitleSize(e, applicationTitle).Height + (int)GetTitleOffset(applicationTitle);
            VerticalOffset += GetTitleSize(e, applicationSubTitle).Height + (int)GetTitleOffset(applicationSubTitle);
            VerticalOffset += GetTitleSize(e, applicationDescription).Height + (int)GetTitleOffset(applicationDescription);
            VerticalOffset += GetTitleSize(e, applicationStatus, true).Height + (int)GetTitleOffset(applicationStatus);

            int RunningVerticalPosition = (int)((float)(Height - VerticalOffset) / 2.0f);
            RunningVerticalPosition += DrawLogo(e, logo, new Point(0, RunningVerticalPosition));
            RunningVerticalPosition += DrawTitle(e, applicationTitle, new Point(0, RunningVerticalPosition));
            RunningVerticalPosition += DrawTitle(e, applicationSubTitle, new Point(0, RunningVerticalPosition));
            RunningVerticalPosition += DrawTitle(e, applicationDescription, new Point(0, RunningVerticalPosition));
            RunningVerticalPosition += DrawTitle(e, applicationStatus, new Point(0, RunningVerticalPosition), true);
        }
        private int DrawTitle(PaintEventArgs e, SplashScreenText? TitleSection, Point TextPosition, bool UseDefaultOnZero = false) {
            if (TitleSection == null) { return 0; }
            if (TitleSection.Text == null) { return 0; }
            //if (TitleSection.Text.Length == 0) { return 0; }
            if (TitleSection.Font == null) { return 0; }
            if (TitleSection.Visible == false) { return 0; }
            Color North = TitleSection.NorthForeColor;
            Color South = TitleSection.SouthForeColor;
            if (TitleSection.UseParentColor) {
                //if (borderThickness > 0) {
                North = borderNorthColor;
                South = borderSouthColor;
                //}
            }
            int InsetOffset = 0;
            int InsetThickness = 0;
            if (borderThickness > 0) {
                InsetOffset = borderThickness;
                InsetThickness = (borderThickness * 2);
            }
            Rectangle FillRectangle = new Rectangle(0, 0, Width, Height);
            int VerticalOffset = (int)GetTitleOffset(TitleSection);
            Size TextSize = GetTitleSize(e, TitleSection, UseDefaultOnZero);
            if (TextSize.Height == 0) {
                if (UseDefaultOnZero == false) {
                    return 0;
                }
            }
            int XOffset = 0;
            int XOffsetTrim = 0;
            if (TitleSection.TextJustification == SplashScreenTextJustification.Left) {
                XOffset = TitleSection.HorizontalOffset;
                XOffsetTrim = 2 * XOffset;
            }
            else if (TitleSection.TextJustification == SplashScreenTextJustification.Right) {
                XOffset = TitleSection.HorizontalOffset;
                XOffsetTrim = 2 * XOffset;
            }
            if (TitleSection.Text.Length > 0) {
                Rectangle TextRectangle = new Rectangle(InsetOffset + TextPosition.X + XOffset, TextPosition.Y + VerticalOffset, Width - InsetThickness - XOffsetTrim, TextSize.Height);
                using (LinearGradientBrush TextBrush = new LinearGradientBrush(FillRectangle, North, South, 45.0f)) {
                    using (StringFormat TitleFormat = new StringFormat()) {
                        TitleFormat.LineAlignment = StringAlignment.Center;
                        if (TitleSection.TextJustification == SplashScreenTextJustification.Left) {
                            TitleFormat.Alignment = StringAlignment.Near;
                        }
                        else if (TitleSection.TextJustification == SplashScreenTextJustification.Center) {
                            TitleFormat.Alignment = StringAlignment.Center;
                        }
                        else if (TitleSection.TextJustification == SplashScreenTextJustification.Right) {
                            TitleFormat.Alignment = StringAlignment.Far;
                        }
                        e.Graphics.DrawString(TitleSection.Text, TitleSection.Font, TextBrush, TextRectangle, TitleFormat);
                    }
                }
            }
            return TextSize.Height + VerticalOffset;
        }
        private int DrawLogo(PaintEventArgs e, SplashScreenLogo? LogoSection, Point LogoPosition) {
            if (LogoSection == null) { return 0; }
            if (LogoSection.Visible == false) { return 0; }
            if (LogoSection.Image == null) { return 0; }
            Size LogoSize = GetLogoSize(e, LogoSection);
            Point LogoPoint = new Point(LogoPosition.X + LogoSection.HorizontalOffset, LogoPosition.Y);
            Rectangle LogoRectangle = new Rectangle(LogoPoint, LogoSize);
            e.Graphics.DrawImage(LogoSection.Image, LogoRectangle);
            return LogoSize.Height;
        }
        private Size GetLogoSize(PaintEventArgs e, SplashScreenLogo? LogoSection) {
            if (LogoSection != null) {
                if (LogoSection.Visible == false) {
                    return new Size(0, 0);
                }
                if (LogoSection.Image != null) {
                    Size ImageSize = LogoSection.Image.Size;
                    if ((ImageSize.Width > 0) && (ImageSize.Height > 0)) {
                        float Ratio = (float)ImageSize.Width / (float)ImageSize.Height;
                        return new Size((int)(LogoSection.Height * Ratio), LogoSection.Height);
                    }
                }
            }
            return new Size(0, 0);
        }
        private Size GetTitleSize(PaintEventArgs e, SplashScreenText? TitleSection, bool UseDefaultOnZero = false) {
            if (TitleSection != null) {
                if (TitleSection.Visible == false) {
                    return new Size(0, 0);
                }
                if (TitleSection.Font != null) {
                    if (TitleSection.Text != null) {
                        if (TitleSection.Text.Length > 0) {
                            return new Size((int)e.Graphics.MeasureString(TitleSection.Text, TitleSection.Font).Width, (int)e.Graphics.MeasureString(TitleSection.Text, TitleSection.Font).Height);
                        }
                        else {
                            if (UseDefaultOnZero) {
                                SizeF UnitSize = e.Graphics.MeasureString("W", TitleSection.Font);
                                return new Size((int)UnitSize.Width, (int)UnitSize.Height);
                            }
                        }
                    }
                }
            }
            return new Size(0, 0);
        }
        private uint GetTitleOffset(SplashScreenText? TitleSection) {
            if (TitleSection != null) {
                if (TitleSection.Visible == false) {
                    return 0;
                }
                return TitleSection.VerticalOffset;
            }
            return 0;
        }
        private void DrawInsetShadow(PaintEventArgs e) {
            if (drawInsetBorder) {
                //if (borderThichkness <= 0) {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                int ShadowSize = 5;
                int BorderOffset = 0;
                if (borderThickness > 0) {
                    BorderOffset = borderThickness;
                }
                int BorderThicknessDouble = 2 * BorderOffset;
                Rectangle InsetLeft = new Rectangle(BorderOffset - 1, BorderOffset, ShadowSize, Height - BorderThicknessDouble);
                Rectangle InsetRight = new Rectangle(Width - ShadowSize - BorderOffset, BorderOffset, ShadowSize, Height - BorderThicknessDouble);
                Rectangle InsetTop = new Rectangle(BorderOffset - 1, BorderOffset - 1, Width - BorderThicknessDouble, ShadowSize);
                Rectangle InsetBottom = new Rectangle(BorderOffset, Height - ShadowSize - BorderOffset, Width - BorderThicknessDouble, ShadowSize);
                using (LinearGradientBrush ShadowLeft = new LinearGradientBrush(InsetLeft, insetShadowColor, Color.Transparent, 0.0f)) {
                    e.Graphics.FillRectangle(ShadowLeft, InsetLeft);
                }
                using (LinearGradientBrush ShadowRight = new LinearGradientBrush(InsetRight, insetShadowColor, Color.Transparent, 180.0f)) {
                    e.Graphics.FillRectangle(ShadowRight, InsetRight);
                }
                using (LinearGradientBrush ShadowTop = new LinearGradientBrush(InsetTop, insetShadowColor, Color.Transparent, 90.0f)) {
                    e.Graphics.FillRectangle(ShadowTop, InsetTop);
                }
                using (LinearGradientBrush ShadowBottom = new LinearGradientBrush(InsetBottom, insetShadowColor, Color.Transparent, 270.0f)) {
                    e.Graphics.FillRectangle(ShadowBottom, InsetBottom);
                }
            }
        }
        private void DrawBackground(PaintEventArgs e) {
            if (borderThickness <= 0) {
                using (LinearGradientBrush FillBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), backgroundNorthColor, backgroundSouthColor, 45.0f)) {
                    e.Graphics.FillRectangle(FillBrush, new Rectangle(0, 0, Width, Height));
                }
            }
            else {
                using (LinearGradientBrush FillBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), borderNorthColor, borderSouthColor, 45.0f)) {
                    e.Graphics.FillRectangle(FillBrush, new Rectangle(0, 0, Width, Height));
                }
                Rectangle Inset = new Rectangle(borderThickness, borderThickness, Width - (2 * borderThickness), Height - (2 * borderThickness));
                Color North = backgroundNorthColor;
                Color South = backgroundSouthColor;

                if (useDimmedBackground) {
                    North = RenderHandler.BlackDarkenColor(borderNorthColor, 128);
                    South = RenderHandler.BlackDarkenColor(borderSouthColor, 128);
                }
                using (LinearGradientBrush FillBrush = new LinearGradientBrush(Inset, North, South, 45.0f)) {
                    e.Graphics.FillRectangle(FillBrush, Inset);
                }
            }
            DrawBackgroundImage(e);
        }
        private void DrawBackgroundImage(PaintEventArgs e) {
            if (BackgroundImage != null) {
                Size ImageSize = BackgroundImage.Size;
                if ((ImageSize.Width > 0) && (ImageSize.Height > 0)) {
                    float Ratio = (float)ImageSize.Width / (float)ImageSize.Height;
                    Size ScaledSize = new Size((int)(BackgroundImage.Height * Ratio * backgroundImageScaling), (int)(BackgroundImage.Height * backgroundImageScaling));
                    Rectangle BackgroundRectanagle = new Rectangle(new Point(0, 0), ScaledSize);
                    if (backgroundImagePosition == SplashScreenImagePostion.TopRight) {
                        BackgroundRectanagle = new Rectangle(new Point(Width - ScaledSize.Width, 0), ScaledSize);
                    }
                    else if (backgroundImagePosition == SplashScreenImagePostion.BottomRight) {
                        BackgroundRectanagle = new Rectangle(new Point(Width - ScaledSize.Width, Height - ScaledSize.Height), ScaledSize);
                    }
                    else if (backgroundImagePosition == SplashScreenImagePostion.BottomLeft) {
                        BackgroundRectanagle = new Rectangle(new Point(0, Height - ScaledSize.Height), ScaledSize);
                    }
                    e.Graphics.DrawImage(BackgroundImage, BackgroundRectanagle);
                }

            }
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // SplashControl
            // 
            this.Name = "SplashControl";
            this.Load += new System.EventHandler(this.SplashControl_Load);
            this.ResumeLayout(false);

        }

        private void SplashControl_Load(object? sender, EventArgs e) {

        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplashScreenLogo {
        public delegate void UpdateHandler(object sender);
        public event UpdateHandler? PropertyChanged;
        Image? embeddedimage = null;
        [System.ComponentModel.Category("Appearance")]
        [NotifyParentProperty(true)]
        public Image? Image {
            get {
                return embeddedimage;
            }
            set {
                embeddedimage = value;
                PropertyChanged?.Invoke(this);
            }
        }
        int imageHeight = 10;
        [System.ComponentModel.Category("Appearance")]
        public int Height {
            get {
                return imageHeight;
            }
            set {
                if (imageHeight >= 5) {
                    imageHeight = value;
                }
                else {
                    imageHeight = 10;
                }
                PropertyChanged?.Invoke(this);
            }
        }
        int horizontalOffset = 0;
        [System.ComponentModel.Category("Appearance")]
        public int HorizontalOffset {
            get {
                return horizontalOffset;
            }
            set {
                horizontalOffset = value;
                PropertyChanged?.Invoke(this);
            }
        }
        bool visible = true;
        [System.ComponentModel.Category("Appearance")]
        [NotifyParentProperty(true)]
        public bool Visible {
            get { return visible; }
            set {
                visible = value;
                PropertyChanged?.Invoke(this);
            }
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplashScreenText : ExpandableObjectConverter {
        public delegate void UpdateHandler(object sender);
        public event UpdateHandler? PropertyChanged;

        int horizontalOffset = 0;
        [System.ComponentModel.Category("Appearance")]
        public int HorizontalOffset {
            get {
                return horizontalOffset;
            }
            set {
                horizontalOffset = value;
                PropertyChanged?.Invoke(this);
            }
        }
        string text = "";
        [System.ComponentModel.Category("Appearance")]
        [NotifyParentProperty(true)]
        public string Text {
            get { return text; }
            set {
                text = value;
                PropertyChanged?.Invoke(this);
            }
        }
        Font? textFont = null;
        [System.ComponentModel.Category("Appearance")]
        [NotifyParentProperty(true)]
        public Font? Font {
            get { return textFont; }
            set {
                textFont = value;
                PropertyChanged?.Invoke(this);
            }

        }
        bool visible = true;
        [System.ComponentModel.Category("Appearance")]
        [NotifyParentProperty(true)]
        public bool Visible {
            get { return visible; }
            set {
                visible = value;
                PropertyChanged?.Invoke(this);
            }
        }


        Color northForeColor = Color.Black;
        [NotifyParentProperty(true)]
        [System.ComponentModel.Category("Appearance")]
        public Color NorthForeColor {
            get {
                return northForeColor;
            }
            set {
                northForeColor = value;
                PropertyChanged?.Invoke(this);
            }
        }
        Color southForeColor = Color.Black;
        [NotifyParentProperty(true)]
        [System.ComponentModel.Category("Appearance")]
        public Color SouthForeColor {
            get {
                return southForeColor;
            }
            set {
                southForeColor = value;
                PropertyChanged?.Invoke(this);
            }
        }
        bool useParentColor = false;
        [NotifyParentProperty(true)]
        [System.ComponentModel.Category("Appearance")]
        public bool UseParentColor {
            get { return useParentColor; }
            set {
                useParentColor = value;
                PropertyChanged?.Invoke(this);
            }
        }
        SplashScreenTextJustification textJustification = SplashScreenTextJustification.Left;
        public SplashScreenTextJustification TextJustification {
            get { return textJustification; }
            set {
                textJustification = value;
                PropertyChanged?.Invoke(this);
            }

        }
        uint verticalOffset = 0;
        public uint VerticalOffset {
            get { return verticalOffset; }
            set {
                verticalOffset = value;
                PropertyChanged?.Invoke(this);
            }
        }


        public SplashScreenText(string text, bool visible, Font font) {
            this.text = text;
            Visible = visible;
            Font = font;
        }

        public override bool Equals(object? obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return Text;
        }
    }
    public enum SplashScreenTextJustification {
        Left = 0x00,
        Center = 0x01,
        Right = 0x02
    }
    public enum SplashScreenImagePostion {
        TopLeft = 0x00,
        TopRight = 0x01,
        BottomLeft = 0x02,
        BottomRight = 0x03
    }
}
