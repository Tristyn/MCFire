using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlockPalette
{
    /// <summary>
    /// gets the average colors of a block, outputs values as argb colors.
    /// Auto-Generates the colors in a switch statement.
    /// icons retrieved from http://minecraft.gamepedia.com/index.php?title=Category:Block_icons&filefrom=Acacia_Leaves.png&fileuntil=Zombie_Head.png#mw-category-media using DownloadThemAll
    /// 04/05/2014
    /// </summary>
    class Program
    {
        static void Main()
        {
            var path = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources//Icons")).AbsolutePath;
            // build id list
            List<IdColor> blockIds = new List<IdColor>();
            List<Color> colors = new List<Color>();
            // build ids
            foreach (var file in new DirectoryInfo(path).EnumerateFiles("*.png"))
            {
                var bitmap = new Bitmap(file.FullName);
                var color = GetDominantColor(bitmap);
                colors.Add(color);

                //parse string
                foreach (var id in file.Name.Replace(".png", "").Split(','))
                {
                    var split = id.Split('-');

                    switch (split.Length)
                    {
                        case 1:
                            blockIds.Add(new IdColor(int.Parse(split[0]), null, color));
                            break;
                        case 2:
                            blockIds.Add(new IdColor(int.Parse(split[0]), byte.Parse(split[1]), color));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Console.WriteLine(color);
            }

            // order ids by BlockIds
            var orderedById = (from id in blockIds
                               orderby id.BlockId
                               group new DataColor(id.Data, id.Color) by id.BlockId into dataColorGroups
                               select new IdDataColors(dataColorGroups.Key, dataColorGroups)).ToList();

            foreach (var idGroup in orderedById)
            {
                // order ascending data, nulls at back
                var nullsAtBack = idGroup.DataColors.OrderBy(color => color.Data == null ? byte.MaxValue : color.Data.Value).ToList();

                // remove duplicate nulls
                if (nullsAtBack.Last().Data == null)
                    while (true)
                    {
                        if (nullsAtBack.Count - 2 <= 0) break;
                        if (nullsAtBack[nullsAtBack.Count - 2].Data != null) break;

                        //duplicate null, remove it.
                        nullsAtBack.RemoveAt(nullsAtBack.Count - 2);
                    }

                idGroup.DataColors = nullsAtBack;
            }


            var sb = new StringBuilder();
            sb.AppendLine("// Auto-Generated via BlockPalette project.");
            sb.AppendLine("switch (id)");
            sb.AppendLine("{");

            for (int i = 0; i < orderedById.Count; i++)
            {
                var dataGroup = orderedById[i];
                var dataColors = dataGroup.DataColors;
                if (dataColors.Count == 1)
                {
                    sb.AppendLine(string.Format("case {0}:", dataGroup.BlockId));
                    ColorToString(dataColors.First().Color, sb);
                    continue;
                }

                sb.AppendLine(string.Format("case {0}:", dataGroup.BlockId));
                sb.AppendLine("switch (data)");
                sb.AppendLine("{");
                var defaulted = false;
                for (var j = 0; j < dataColors.Count; j++)
                {
                    var dataColor = dataColors[j];

                    defaulted = BuildCaseReturnStatement(dataColor, sb);
                }
                sb.AppendLine("}");
                if (defaulted)
                    sb.AppendLine("break;");
            }
            sb.AppendLine("}");
            ColorToString(Color.Purple, sb);

            var outputPath = Path.GetTempPath() + Guid.NewGuid() + ".txt";
            File.WriteAllText(outputPath, sb.ToString());
            Process.Start(outputPath);
            Console.ReadLine();
        }

        static bool BuildCaseReturnStatement(DataColor color, StringBuilder sb)
        {
            var defauling = CaseToString(color, sb);
            ColorToString(color.Color, sb);
            return defauling;
        }

        static bool CaseToString(DataColor color, StringBuilder sb)
        {
            var defaulting = color.Data == null;
            sb.AppendLine(defaulting ? "default:" : string.Format("case {0}:", color.Data));
            return !defaulting;
        }

        static void ColorToString(Color color, StringBuilder sb)
        {
            sb.AppendLine(string.Format("return new Color({0}, {1}, {2}, {3});", color.R, color.G, color.B, color.A));
        }

        public static Color GetDominantColor(Bitmap bmp)
        {

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);
                    if (clr.A == 0)
                        continue;
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }
    }

    struct IdColor
    {
        public int BlockId { get; private set; }
        public byte? Data { get; private set; }
        public Color Color { get; private set; }

        public IdColor(int blockId, byte? data, Color color)
            : this()
        {
            BlockId = blockId;
            Data = data;
            Color = color;
        }
    }

    class IdDataColors
    {
        public int BlockId { get; private set; }
        public List<DataColor> DataColors { get; set; }

        public IdDataColors(int blockiD, IEnumerable<DataColor> colors)
        {
            BlockId = blockiD;
            DataColors = colors.ToList();
        }
    }

    class DataColor
    {
        public byte? Data { get; private set; }
        public Color Color { get; private set; }

        public DataColor(byte? data, Color color)
        {
            Data = data;
            Color = color;
        }
    }
}
