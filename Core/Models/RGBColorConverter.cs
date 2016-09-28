using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;

namespace StatTag.Core.Models
{
    /// <summary>
    /// static class with rgb color retrieving logic: Microsoft.Office.Interop.Word.WdColor -> System.Drawing.Color
    /// Taken from: http://pastebin.com/96L6aCEH
    /// Other references on how this works:
    /// http://www.wordarticles.com/Articles/Colours/2007.php#SettingColours   [This is a very good, detailed article]
    /// http://stackoverflow.com/questions/25560277/how-to-determine-the-font-color-of-the-active-document-opened-using-word-office
    /// http://stackoverflow.com/questions/16189590/office-2007-and-higher-interop-retrieve-rgb-color
    /// </summary>
    public static class RgbColorRetriever
    {
        #region Constants

        // first byte of WdColor determines its format type
        private static readonly int
            RGB = 0x00,
            Automatic = 0xFF,
            System = 0x80,
            ThemeLow = 0xD0,
            ThemeHigh = 0xDF;

        //structure to store HSL (hue, saturation, lightness) color
        private struct HSL
        {
            public double H, S, L;
        }

        #endregion

        /// <summary>
        /// Get RGB-color from WdColor
        /// </summary>
        /// <param name="wdColor">source color</param>
        /// <param name="doc">document, where this color from (for appropriate color theme)</param>
        public static Color GetRGBColor(Word.WdColor wdColor, Word.Document doc)
        {
            // separate 1st byte (the most significant) and 3 others to different vars
            int color = ((int)wdColor) & ((int)0xFFFFFF);
            int colorType = (int)(((uint)wdColor) >> 24);

            if (colorType == RGB)
            {
                // simple color in OLE format (it's just a BGR - blue, green, red) 
                // let's use standard color translator from system.drawing
                return ColorTranslator.FromOle(color);
            }
            else if (colorType == Automatic)
            {
                // standard contrast color. In my case I was needed color. But I don't know the proper way to understand which one (black or white) I need to choose.
                return Color.Black;
            }
            else if (colorType == System)
            {
                // In ActiveX controls in documents, and in VBA (for UserForm controls, for example) special values for system colours 
                // (for some reason lost in the mists of time these are also called OLE Colors) ranging from 0x80000000 to 0x80000018. 
                // I used system dll function to retrieve system color and then used standard color translator
                int sysColor = GetSysColor(color);
                return ColorTranslator.FromOle(sysColor);
            }
            else if (colorType >= ThemeLow && colorType <= ThemeHigh)
            {
                // color based on doc's color theme
                return GetThemedColor(colorType, color, doc);
            }

            throw new Exception("Unknown color type");
        }

        private static Color GetThemedColor(int colorType, int color, Word.Document doc)
        {
            // color based on theme is base color + tint or shade
            double tintAndShade = 0;
            // base color index is least siginficant 4 bits from colorType
            int colorThemeIndex = colorType & 0xF;

            // 2nd most significant byte is always 0
            // 3rd byte - shade, 4th - tint. One of them must be 0xFF and shouldn't be used
            // it means that always is used one of them and other is 0xFF
            int darkness = (color & 0x00FF00) >> 8;
            int lightness = color & 0x0000FF;

            if (darkness != 0xFF)
                tintAndShade = -1 + darkness / 255.0;
            else
                tintAndShade = 1.0 - lightness / 255.0;
            // so: 
            //      tintAndShade < 0 => shade base color by |tintAndShade| * 100%
            //      tintAndShade > 0 => tint base color |tintAndShade| * 100%

            return GetThemedColor(colorThemeIndex, tintAndShade, doc);
        }

        private static Color GetThemedColor(int colorThemeIndex, double tintAndShade, Word.Document doc)
        {
            // translate from wdThemeColorIndex to MsoThemeColorSchemeIndex
            Microsoft.Office.Core.MsoThemeColorSchemeIndex colorSchemeIndex = ThemeIndexToSchemeIndex(colorThemeIndex);
            // get color scheme by this index and take its RGB property, but this RGB still OLE RGB - i.e. BGR -> need to convert it to real RGB, i.e. use ColorTranslator.FromOle() and ToArgb after
            int colorSchemeRGB = ColorTranslator.FromOle(doc.DocumentTheme.ThemeColorScheme.Colors(colorSchemeIndex).RGB).ToArgb();

            // do RGB -> HSL translation to apply tint/shade
            HSL colorSchemeHSL = RGBtoHSL(colorSchemeRGB);

            // apply it
            if (tintAndShade > 0)
                colorSchemeHSL.L += (1 - colorSchemeHSL.L) * tintAndShade;
            else
                colorSchemeHSL.L *= 1 - Math.Abs(tintAndShade);

            // do backward HSL -> RGB translation
            int tintedAndShadedRGB = HSLtoRGB(colorSchemeHSL);

            return Color.FromArgb(tintedAndShadedRGB);
        }

        private static int HSLtoRGB(HSL HSL)
        {
            // took from http://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
            double red, green, blue;

            if (HSL.S == 0)
                red = green = blue = HSL.L;
            else
            {
                double q = HSL.L < 0.5 ? HSL.L * (1 + HSL.S) : HSL.L + HSL.S - HSL.L * HSL.S;
                double p = 2 * HSL.L - q;

                red = Hue2RGB(p, q, HSL.H + 1.0 / 3);
                green = Hue2RGB(p, q, HSL.H);
                blue = Hue2RGB(p, q, HSL.H - 1.0 / 3);
            }

            int r = (int)(red * 255), g = (int)(green * 255), b = (int)(blue * 255);
            return (r << 16) + (g << 8) + b;
        }

        private static double Hue2RGB(double p, double q, double t)
        {
            // took from http://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion

            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        private static HSL RGBtoHSL(int RGB)
        {
            // took from http://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
            double red, green, blue;
            double max, min, diff;

            red = ((RGB & 0xFF0000) >> 16) / 255.0;
            green = ((RGB & 0x00FF00) >> 8) / 255.0;
            blue = (RGB & 0x0000FF) / 255.0;

            max = Math.Max(red, Math.Max(green, blue));
            min = Math.Min(red, Math.Min(green, blue));
            diff = max - min;

            HSL res;
            res.L = res.H = res.S = (max + min) / 2;
            if (max == min)
                res.S = res.H = 0;
            else
            {
                res.S = res.L < 0.5 ? diff / (max + min) : diff / (2 - max - min);

                if (red == max)
                    res.H = (green - blue) / diff - (blue > green ? 6 : 0);
                else if (green == max)
                    res.H = (blue - red) / diff + 2;
                else if (blue == max)
                    res.H = (red - green) / diff + 4;
                res.H /= 6;
            }

            return res;
        }

        private static Microsoft.Office.Core.MsoThemeColorSchemeIndex ThemeIndexToSchemeIndex(int colorThemeIndex)
        {
            // translation sheet from http://www.wordarticles.com/Articles/Colours/2007.php#UIConsiderations
            switch ((Word.WdThemeColorIndex)colorThemeIndex)
            {
                case Word.WdThemeColorIndex.wdThemeColorMainDark1:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeDark1;
                case Word.WdThemeColorIndex.wdThemeColorMainLight1:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeLight1;
                case Word.WdThemeColorIndex.wdThemeColorMainDark2:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeDark2;
                case Word.WdThemeColorIndex.wdThemeColorMainLight2:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeLight2;
                case Word.WdThemeColorIndex.wdThemeColorAccent1:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent1;
                case Word.WdThemeColorIndex.wdThemeColorAccent2:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent2;
                case Word.WdThemeColorIndex.wdThemeColorAccent3:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent3;
                case Word.WdThemeColorIndex.wdThemeColorAccent4:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent4;
                case Word.WdThemeColorIndex.wdThemeColorAccent5:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent5;
                case Word.WdThemeColorIndex.wdThemeColorAccent6:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeAccent6;
                case Word.WdThemeColorIndex.wdThemeColorHyperlink:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeHyperlink;
                case Word.WdThemeColorIndex.wdThemeColorHyperlinkFollowed:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeFollowedHyperlink;
                case Word.WdThemeColorIndex.wdThemeColorBackground1:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeLight1;
                case Word.WdThemeColorIndex.wdThemeColorText1:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeDark1;
                case Word.WdThemeColorIndex.wdThemeColorBackground2:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeLight2;
                case Word.WdThemeColorIndex.wdThemeColorText2:
                    return Microsoft.Office.Core.MsoThemeColorSchemeIndex.msoThemeDark2;
                default:
                    throw new Exception("Something is rotten in the state of Denmark...");
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetSysColor(int nIndex);
    }
}