using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PdfSharpCore.Drawing
{
    public static class XColorConverter
    {
        static ColorSpaceConverter  _converter;

        public static void UseConverter(ColorSpaceConverter converter)
        {
            _converter = converter;
        }

        /// <summary>
        /// NOTE: Relies on ImageSharp default LinearRgb WorkingSpace. You can specify particular workingspaces to get more control over conversion involving Rgb.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="destSpace"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
        public static XColor ConvertTo(this XColor x, XColorSpace destSpace)
        {
            if (x.ColorSpace == destSpace)
                return x;

            if (_converter == null)
                throw new InvalidOperationException($"You must assign a color converter before using {nameof(XColorConverter)}");

            if(x.ColorSpace == XColorSpace.Rgb)
            {
                if(destSpace == XColorSpace.Cmyk)
                {
                    var c = _converter.ToCmyk(new LinearRgb(x.R, x.G, x.B));
                    return XColor.FromCmyk(c.C, c.M, c.Y, c.K);
                }
                else
                {
                    var c = _converter.ToHsl(new LinearRgb(x.R, x.G, x.B));
                    return XColor.FromGrayScale(c.L);
                }
            }
            else if (x.ColorSpace == XColorSpace.Cmyk)
            {
                if (destSpace == XColorSpace.Rgb)
                {
                    var c = _converter.ToRgb(new Cmyk((float)x.C, (float)x.M, (float)x.Y, (float)x.K));
                    return XColor.FromArgb((int)(c.R * 256), (int)(c.G * 256), (int)(c.B * 256));
                }
                else
                {
                    var c = _converter.ToHsl(new Cmyk((float)x.C, (float)x.M, (float)x.Y, (float)x.K));
                    return XColor.FromGrayScale(c.L);
                }
            }
            else if (x.ColorSpace == XColorSpace.GrayScale)
            {
                if (destSpace == XColorSpace.Rgb)
                    return XColor.FromArgb((int)(x.GS * 256), (int)(x.GS * 256), (int)(x.GS * 256));
                else
                    return XColor.FromCmyk(0, 0, 0, x.GS);
            }
            else
                throw new InvalidProgramException("1128: Unimplemented color space.");
        }
    }
}
