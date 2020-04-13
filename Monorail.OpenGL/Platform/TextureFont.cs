using BmFont;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Monorail.Platform
{
    public class TextureFont
    {
        public Texture2D FontTexture;

        public Font Font;

        // This dictionary lookups might be a bit slow on some systems
        // TODO use an array instead?
        public Dictionary<int, FontChar> CharLookup = new Dictionary<int, FontChar>();

        // TODO Support Kerning Pairs

        public static TextureFont CreateFromFile(string filename)
        {
            // TODO: Convert to binary format at some stage for improved loadtimes
            Font font = null;
            Texture2D fontTexture = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Font));
            var fi = new FileInfo(filename);
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                font = (Font)serializer.Deserialize(stream);

                var texture = font.Info.Face;
                var pages = font.Pages.Count();
                if (pages != 1)
                {
                    throw new NotImplementedException("TODO Support multiple pages!");
                }

                var dir = fi.DirectoryName;
                fontTexture = Texture2D.CreateFromFile(dir + "/" + font.Pages[0].File);
            }

            var rv = new TextureFont
            {
                Font = font,
                FontTexture = fontTexture
            };

            // Construct char dictionary
            for (int i = 0; i < font.Chars.Length; i++)
            {
                rv.CharLookup.Add(font.Chars[i].Id, font.Chars[i]);
            }

            return rv;
        }
    }
}
