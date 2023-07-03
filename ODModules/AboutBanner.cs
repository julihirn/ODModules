using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic.Logging;

namespace ODModules {
    public class AboutBanner : UserControl {
        Image? logo = null;
        [System.ComponentModel.Category("Branding")]
        public Image? Logo {
            get {
                return logo;
            }
            set {
                logo = value;
                Invalidate();
            }
        }
        string brandText = "";
        [System.ComponentModel.Category("Branding")]
        public string BrandText {
            get {
                return brandText;
            }
            set {
                brandText = value;
                Invalidate();
            }
        }
        public AboutBanner() {
            DoubleBuffered = true;
        }
        Font brandFont = DefaultFont;
        [System.ComponentModel.Category("Branding")]
        public Font BrandFont {
            get {
                return brandFont;
            }
            set {
                brandFont = value;
                Invalidate();
            }
        }
        string brandSecondaryText = "";
        [System.ComponentModel.Category("Branding")]
        public string BrandSecondaryText {
            get {
                return brandSecondaryText;
            }
            set {
                brandSecondaryText = value;
                Invalidate();
            }
        }
        Font brandSecondaryFont = DefaultFont;
        [System.ComponentModel.Category("Branding")]
        public Font BrandSecondaryFont {
            get {
                return brandSecondaryFont;
            }
            set {
                brandSecondaryFont = value;
                Invalidate();
            }
        }
        Color ruleColorLeft = Color.Blue;
        [System.ComponentModel.Category("Appearance")]
        public Color RuleColorEdge {
            get {
                return ruleColorLeft;
            }
            set {
                ruleColorLeft = value;
                Invalidate();
            }
        }
        Color ruleColorCenter = Color.Cyan;
        [System.ComponentModel.Category("Appearance")]
        public Color RuleColorCenter {
            get {
                return ruleColorCenter;
            }
            set {
                ruleColorCenter = value;
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            int BrandTextPadding = 0;
            int BrandTextLength = 0;
            int BrandTextSecondaryHeight = 0;
            bool HasText = false;
            bool HasLogo = false;
            int PrimaryTextHeight = 0;
            if (BrandText.Length > 0) {
                BrandTextLength = (int)e.Graphics.MeasureString(BrandText, brandFont).Width;
                BrandTextPadding = (int)(e.Graphics.MeasureString("W", brandFont).Width / 1.5f);
                PrimaryTextHeight = (int)e.Graphics.MeasureString("W", brandFont).Height;
                HasText = true;
            }
            if (brandSecondaryText.Length > 0) {
                BrandTextSecondaryHeight = (int)e.Graphics.MeasureString(brandSecondaryText, brandSecondaryFont).Height;
            }
            Size ImageSize = Size.Empty;
            if (logo != null) {
                ScaleSize(logo.Size, new Size(1, (int)((float)(PrimaryTextHeight + BrandTextSecondaryHeight) * 1.25f)), out ImageSize);
                HasLogo = true;
            }
            int XStart = (Width - (BrandTextLength + BrandTextPadding + ImageSize.Width)) / 2;
            int YLogoStart = (Height - ImageSize.Height) / 2;
            int YTextStart = (Height - (PrimaryTextHeight + BrandTextSecondaryHeight)) / 2;
            Rectangle LogoRectangle = new Rectangle(new Point(XStart, YLogoStart), ImageSize);
            DrawLogo(e, LogoRectangle);
            if (HasText == true) {
                using (SolidBrush TxtBr = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString(BrandText, brandFont, TxtBr, new Point(XStart + BrandTextPadding + ImageSize.Width, YTextStart));

                    if (brandSecondaryText.Length > 0) {
                        e.Graphics.DrawString(brandSecondaryText, brandSecondaryFont, TxtBr, new Point(XStart + BrandTextPadding + ImageSize.Width, YTextStart + PrimaryTextHeight));
                    }
                }
            }
            int BandWidth = (int)(6.0f * (e.Graphics.DpiX / 96.0f));
            Rectangle Band = new Rectangle(-1, Height - BandWidth, Width, BandWidth + 1);
            using (SolidBrush BandBr = new SolidBrush(RuleColorCenter)) {
                e.Graphics.FillRectangle(BandBr, Band);
            }
            Rectangle LeftBand = new Rectangle(Band.X, Band.Y, Band.Width / 2, Band.Height);
            Rectangle RightBand = new Rectangle(Band.X + Band.Width / 2, Band.Y, (Band.Width / 2)+1, Band.Height);
            using (LinearGradientBrush BrandBr = new LinearGradientBrush(LeftBand,RuleColorEdge,Color.Transparent,0.0f)) {
                LeftBand.Inflate(-1, 0);
                e.Graphics.FillRectangle(BrandBr, LeftBand);
            }
            using (LinearGradientBrush BrandBr = new LinearGradientBrush(RightBand, RuleColorEdge, Color.Transparent, 180.0f)) {
                RightBand = new Rectangle(RightBand.X + 1, RightBand.Y, RightBand.Width, RightBand.Height);
                e.Graphics.FillRectangle(BrandBr, RightBand);
            }
        }
        private void ScaleSize(Size Input, Size Bounds, out Size Output) {
            if (Input.Height <= 0) { Output = new Size(0, 0); }
            float Scale = (float)Bounds.Height / (float)Input.Height;//Bounds = Input * x
            Output = new Size((int)((float)Input.Width * Scale), (int)((float)Input.Height * Scale));
        }
        private void DrawLogo(PaintEventArgs e, Rectangle LogoBounds) {
            if (logo == null) { return; }
            if (LogoBounds.Height <= 0) { return; }
            if (LogoBounds.Width <= 0) { return; }
            try {
                e.Graphics.DrawImage(logo, LogoBounds);
            }
            catch { }
        }

        protected override void OnResize(EventArgs e) {
            Invalidate();
            base.OnResize(e);
        }
    }
}
