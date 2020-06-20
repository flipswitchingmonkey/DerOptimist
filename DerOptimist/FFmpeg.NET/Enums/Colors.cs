using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FFmpeg.NET.Enums
{
    public static class Colors
    {
        public static ColorCollection Presets;

        static Colors()
        {
            Presets = new ColorCollection();
            Presets.Add(new ColorEntry("AliceBlue", "0xF0F8FF"));
            Presets.Add(new ColorEntry("AntiqueWhite", "0xFAEBD7"));
            Presets.Add(new ColorEntry("Aqua", "0x00FFFF"));
            Presets.Add(new ColorEntry("Aquamarine", "0x7FFFD4"));
            Presets.Add(new ColorEntry("Azure", "0xF0FFFF"));
            Presets.Add(new ColorEntry("Beige", "0xF5F5DC"));
            Presets.Add(new ColorEntry("Bisque", "0xFFE4C4"));
            Presets.Add(new ColorEntry("Black", "0x000000"));
            Presets.Add(new ColorEntry("BlanchedAlmond", "0xFFEBCD"));
            Presets.Add(new ColorEntry("Blue", "0x0000FF"));
            Presets.Add(new ColorEntry("BlueViolet", "0x8A2BE2"));
            Presets.Add(new ColorEntry("Brown", "0xA52A2A"));
            Presets.Add(new ColorEntry("BurlyWood", "0xDEB887"));
            Presets.Add(new ColorEntry("CadetBlue", "0x5F9EA0"));
            Presets.Add(new ColorEntry("Chartreuse", "0x7FFF00"));
            Presets.Add(new ColorEntry("Chocolate", "0xD2691E"));
            Presets.Add(new ColorEntry("Coral", "0xFF7F50"));
            Presets.Add(new ColorEntry("CornflowerBlue", "0x6495ED"));
            Presets.Add(new ColorEntry("Cornsilk", "0xFFF8DC"));
            Presets.Add(new ColorEntry("Crimson", "0xDC143C"));
            Presets.Add(new ColorEntry("Cyan", "0x00FFFF"));
            Presets.Add(new ColorEntry("DarkBlue", "0x00008B"));
            Presets.Add(new ColorEntry("DarkCyan", "0x008B8B"));
            Presets.Add(new ColorEntry("DarkGoldenRod", "0xB8860B"));
            Presets.Add(new ColorEntry("DarkGray", "0xA9A9A9"));
            Presets.Add(new ColorEntry("DarkGreen", "0x006400"));
            Presets.Add(new ColorEntry("DarkKhaki", "0xBDB76B"));
            Presets.Add(new ColorEntry("DarkMagenta", "0x8B008B"));
            Presets.Add(new ColorEntry("DarkOliveGreen", "0x556B2F"));
            Presets.Add(new ColorEntry("Darkorange", "0xFF8C00"));
            Presets.Add(new ColorEntry("DarkOrchid", "0x9932CC"));
            Presets.Add(new ColorEntry("DarkRed", "0x8B0000"));
            Presets.Add(new ColorEntry("DarkSalmon", "0xE9967A"));
            Presets.Add(new ColorEntry("DarkSeaGreen", "0x8FBC8F"));
            Presets.Add(new ColorEntry("DarkSlateBlue", "0x483D8B"));
            Presets.Add(new ColorEntry("DarkSlateGray", "0x2F4F4F"));
            Presets.Add(new ColorEntry("DarkTurquoise", "0x00CED1"));
            Presets.Add(new ColorEntry("DarkViolet", "0x9400D3"));
            Presets.Add(new ColorEntry("DeepPink", "0xFF1493"));
            Presets.Add(new ColorEntry("DeepSkyBlue", "0x00BFFF"));
            Presets.Add(new ColorEntry("DimGray", "0x696969"));
            Presets.Add(new ColorEntry("DodgerBlue", "0x1E90FF"));
            Presets.Add(new ColorEntry("FireBrick", "0xB22222"));
            Presets.Add(new ColorEntry("FloralWhite", "0xFFFAF0"));
            Presets.Add(new ColorEntry("ForestGreen", "0x228B22"));
            Presets.Add(new ColorEntry("Fuchsia", "0xFF00FF"));
            Presets.Add(new ColorEntry("Gainsboro", "0xDCDCDC"));
            Presets.Add(new ColorEntry("GhostWhite", "0xF8F8FF"));
            Presets.Add(new ColorEntry("Gold", "0xFFD700"));
            Presets.Add(new ColorEntry("GoldenRod", "0xDAA520"));
            Presets.Add(new ColorEntry("Gray", "0x808080"));
            Presets.Add(new ColorEntry("Green", "0x008000"));
            Presets.Add(new ColorEntry("GreenYellow", "0xADFF2F"));
            Presets.Add(new ColorEntry("HoneyDew", "0xF0FFF0"));
            Presets.Add(new ColorEntry("HotPink", "0xFF69B4"));
            Presets.Add(new ColorEntry("IndianRed", "0xCD5C5C"));
            Presets.Add(new ColorEntry("Indigo", "0x4B0082"));
            Presets.Add(new ColorEntry("Ivory", "0xFFFFF0"));
            Presets.Add(new ColorEntry("Khaki", "0xF0E68C"));
            Presets.Add(new ColorEntry("Lavender", "0xE6E6FA"));
            Presets.Add(new ColorEntry("LavenderBlush", "0xFFF0F5"));
            Presets.Add(new ColorEntry("LawnGreen", "0x7CFC00"));
            Presets.Add(new ColorEntry("LemonChiffon", "0xFFFACD"));
            Presets.Add(new ColorEntry("LightBlue", "0xADD8E6"));
            Presets.Add(new ColorEntry("LightCoral", "0xF08080"));
            Presets.Add(new ColorEntry("LightCyan", "0xE0FFFF"));
            Presets.Add(new ColorEntry("LightGoldenRodYellow", "0xFAFAD2"));
            Presets.Add(new ColorEntry("LightGreen", "0x90EE90"));
            Presets.Add(new ColorEntry("LightGrey", "0xD3D3D3"));
            Presets.Add(new ColorEntry("LightPink", "0xFFB6C1"));
            Presets.Add(new ColorEntry("LightSalmon", "0xFFA07A"));
            Presets.Add(new ColorEntry("LightSeaGreen", "0x20B2AA"));
            Presets.Add(new ColorEntry("LightSkyBlue", "0x87CEFA"));
            Presets.Add(new ColorEntry("LightSlateGray", "0x778899"));
            Presets.Add(new ColorEntry("LightSteelBlue", "0xB0C4DE"));
            Presets.Add(new ColorEntry("LightYellow", "0xFFFFE0"));
            Presets.Add(new ColorEntry("Lime", "0x00FF00"));
            Presets.Add(new ColorEntry("LimeGreen", "0x32CD32"));
            Presets.Add(new ColorEntry("Linen", "0xFAF0E6"));
            Presets.Add(new ColorEntry("Magenta", "0xFF00FF"));
            Presets.Add(new ColorEntry("Maroon", "0x800000"));
            Presets.Add(new ColorEntry("MediumAquaMarine", "0x66CDAA"));
            Presets.Add(new ColorEntry("MediumBlue", "0x0000CD"));
            Presets.Add(new ColorEntry("MediumOrchid", "0xBA55D3"));
            Presets.Add(new ColorEntry("MediumPurple", "0x9370D8"));
            Presets.Add(new ColorEntry("MediumSeaGreen", "0x3CB371"));
            Presets.Add(new ColorEntry("MediumSlateBlue", "0x7B68EE"));
            Presets.Add(new ColorEntry("MediumSpringGreen", "0x00FA9A"));
            Presets.Add(new ColorEntry("MediumTurquoise", "0x48D1CC"));
            Presets.Add(new ColorEntry("MediumVioletRed", "0xC71585"));
            Presets.Add(new ColorEntry("MidnightBlue", "0x191970"));
            Presets.Add(new ColorEntry("MintCream", "0xF5FFFA"));
            Presets.Add(new ColorEntry("MistyRose", "0xFFE4E1"));
            Presets.Add(new ColorEntry("Moccasin", "0xFFE4B5"));
            Presets.Add(new ColorEntry("NavajoWhite", "0xFFDEAD"));
            Presets.Add(new ColorEntry("Navy", "0x000080"));
            Presets.Add(new ColorEntry("OldLace", "0xFDF5E6"));
            Presets.Add(new ColorEntry("Olive", "0x808000"));
            Presets.Add(new ColorEntry("OliveDrab", "0x6B8E23"));
            Presets.Add(new ColorEntry("Orange", "0xFFA500"));
            Presets.Add(new ColorEntry("OrangeRed", "0xFF4500"));
            Presets.Add(new ColorEntry("Orchid", "0xDA70D6"));
            Presets.Add(new ColorEntry("PaleGoldenRod", "0xEEE8AA"));
            Presets.Add(new ColorEntry("PaleGreen", "0x98FB98"));
            Presets.Add(new ColorEntry("PaleTurquoise", "0xAFEEEE"));
            Presets.Add(new ColorEntry("PaleVioletRed", "0xD87093"));
            Presets.Add(new ColorEntry("PapayaWhip", "0xFFEFD5"));
            Presets.Add(new ColorEntry("PeachPuff", "0xFFDAB9"));
            Presets.Add(new ColorEntry("Peru", "0xCD853F"));
            Presets.Add(new ColorEntry("Pink", "0xFFC0CB"));
            Presets.Add(new ColorEntry("Plum", "0xDDA0DD"));
            Presets.Add(new ColorEntry("PowderBlue", "0xB0E0E6"));
            Presets.Add(new ColorEntry("Purple", "0x800080"));
            Presets.Add(new ColorEntry("Red", "0xFF0000"));
            Presets.Add(new ColorEntry("RosyBrown", "0xBC8F8F"));
            Presets.Add(new ColorEntry("RoyalBlue", "0x4169E1"));
            Presets.Add(new ColorEntry("SaddleBrown", "0x8B4513"));
            Presets.Add(new ColorEntry("Salmon", "0xFA8072"));
            Presets.Add(new ColorEntry("SandyBrown", "0xF4A460"));
            Presets.Add(new ColorEntry("SeaGreen", "0x2E8B57"));
            Presets.Add(new ColorEntry("SeaShell", "0xFFF5EE"));
            Presets.Add(new ColorEntry("Sienna", "0xA0522D"));
            Presets.Add(new ColorEntry("Silver", "0xC0C0C0"));
            Presets.Add(new ColorEntry("SkyBlue", "0x87CEEB"));
            Presets.Add(new ColorEntry("SlateBlue", "0x6A5ACD"));
            Presets.Add(new ColorEntry("SlateGray", "0x708090"));
            Presets.Add(new ColorEntry("Snow", "0xFFFAFA"));
            Presets.Add(new ColorEntry("SpringGreen", "0x00FF7F"));
            Presets.Add(new ColorEntry("SteelBlue", "0x4682B4"));
            Presets.Add(new ColorEntry("Tan", "0xD2B48C"));
            Presets.Add(new ColorEntry("Teal", "0x008080"));
            Presets.Add(new ColorEntry("Thistle", "0xD8BFD8"));
            Presets.Add(new ColorEntry("Tomato", "0xFF6347"));
            Presets.Add(new ColorEntry("Turquoise", "0x40E0D0"));
            Presets.Add(new ColorEntry("Violet", "0xEE82EE"));
            Presets.Add(new ColorEntry("Wheat", "0xF5DEB3"));
            Presets.Add(new ColorEntry("White", "0xFFFFFF"));
            Presets.Add(new ColorEntry("WhiteSmoke", "0xF5F5F5"));
            Presets.Add(new ColorEntry("Yellow", "0xFFFF00"));
            Presets.Add(new ColorEntry("YellowGreen", "0x9ACD32"));
        }

    }
    public class ColorCollection : KeyedCollection<string, ColorEntry>
    {
        protected override string GetKeyForItem(ColorEntry item)
        {
            // In this example, the key is the part number.
            return item.Name;
        }
    }

    public class ColorEntry
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public ColorEntry(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
