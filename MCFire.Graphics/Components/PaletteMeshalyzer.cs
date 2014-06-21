using System.ComponentModel.Composition;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Extensions;
using SharpDX;
using Substrate.Core;

namespace MCFire.Graphics.Components
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IMeshalyzer))]
    public class PaletteMeshalyzer : LightMeshalyzer
    {
        // TODO: average color is calculated per face, mods can override colors (dictionary)
        protected override Color GetVertexColor(IChunk chunk, LocalBlockPosition blockPosition, LocalBlockPosition airPosition)
        {
            var block = chunk.Blocks.GetBlock(blockPosition);
            var color = Palette(block.ID, block.Data);
            var lum = base.GetVertexColor(chunk, blockPosition, airPosition);
            return color * lum;
        }

        // TODO: move this to BlockPalette
        static Color Palette(int id, int data)
        {
            // Auto-Generated via BlockPalette project.
            switch (id)
            {
                case 1:
                    switch (data)
                    {
                        case 1:
                            return new Color(95, 71, 61, 255);
                        case 2:
                            return new Color(100, 72, 61, 255);
                        case 3:
                            return new Color(112, 112, 114, 255);
                        case 4:
                            return new Color(114, 114, 116, 255);
                        case 5:
                            return new Color(82, 82, 82, 255);
                        case 6:
                            return new Color(83, 83, 84, 255);
                        default:
                            return new Color(80, 80, 80, 255);
                    }
                case 2:
                    return new Color(69, 74, 45, 255);
                case 3:
                    switch (data)
                    {
                        case 1:
                            return new Color(85, 61, 42, 255);
                        case 2:
                            return new Color(69, 49, 28, 255);
                        default:
                            return new Color(85, 61, 42, 255);
                    }
                case 4:
                    return new Color(78, 78, 78, 255);
                case 5:
                    switch (data)
                    {
                        case 1:
                            return new Color(66, 49, 29, 255);
                        case 2:
                            return new Color(125, 114, 78, 255);
                        case 3:
                            return new Color(98, 70, 49, 255);
                        case 4:
                            return new Color(106, 57, 31, 255);
                        case 5:
                            return new Color(38, 24, 11, 255);
                        default:
                            return new Color(98, 79, 49, 255);
                    }
                case 6:
                    switch (data)
                    {
                        case 0:
                            return new Color(67, 110, 18, 255);
                        case 1:
                            return new Color(51, 58, 33, 255);
                        case 2:
                            return new Color(118, 150, 84, 255);
                        case 3:
                            return new Color(48, 86, 18, 255);
                        case 4:
                            return new Color(114, 115, 20, 255);
                        case 5:
                            return new Color(56, 86, 28, 255);
                        case 8:
                            return new Color(67, 110, 18, 255);
                        case 9:
                            return new Color(51, 58, 33, 255);
                        case 10:
                            return new Color(118, 150, 84, 255);
                        case 11:
                            return new Color(48, 86, 18, 255);
                        case 12:
                            return new Color(114, 115, 20, 255);
                        case 13:
                            return new Color(56, 86, 28, 255);
                    }
                    break;
                case 7:
                    return new Color(53, 53, 53, 255);
                case 8:
                    return new Color(31, 45, 159, 255);
                case 9:
                    return new Color(31, 45, 159, 255);
                case 10:
                    return new Color(137, 63, 14, 255);
                case 11:
                    return new Color(137, 63, 14, 255);
                case 12:
                    switch (data)
                    {
                        case 0:
                            return new Color(140, 135, 102, 255);
                        case 1:
                            return new Color(108, 56, 21, 255);
                    }
                    break;
                case 13:
                    return new Color(81, 79, 78, 255);
                case 14:
                    return new Color(91, 89, 80, 255);
                case 15:
                    return new Color(86, 83, 81, 255);
                case 16:
                    return new Color(74, 74, 74, 255);
                case 17:
                    switch (data)
                    {
                        case 0:
                            return new Color(78, 63, 38, 255);
                        case 1:
                            return new Color(43, 31, 16, 255);
                        case 2:
                            return new Color(123, 118, 105, 255);
                        case 3:
                            return new Color(71, 55, 28, 255);
                        case 4:
                            return new Color(78, 63, 38, 255);
                        case 5:
                            return new Color(43, 31, 16, 255);
                        case 6:
                            return new Color(123, 118, 105, 255);
                        case 7:
                            return new Color(71, 55, 28, 255);
                        case 8:
                            return new Color(78, 63, 38, 255);
                        case 9:
                            return new Color(43, 31, 16, 255);
                        case 10:
                            return new Color(123, 118, 105, 255);
                        case 11:
                            return new Color(71, 55, 28, 255);
                        case 12:
                            return new Color(78, 63, 38, 255);
                        case 13:
                            return new Color(43, 31, 16, 255);
                        case 14:
                            return new Color(123, 118, 105, 255);
                        case 15:
                            return new Color(71, 55, 28, 255);
                    }
                    break;
                case 18:
                    switch (data)
                    {
                        case 0:
                            return new Color(23, 59, 7, 255);
                        case 1:
                            return new Color(29, 46, 29, 255);
                        case 2:
                            return new Color(41, 54, 27, 255);
                        case 3:
                            return new Color(12, 59, 1, 255);
                    }
                    break;
                case 19:
                    return new Color(117, 117, 36, 255);
                case 20:
                    return new Color(141, 156, 158, 255);
                case 21:
                    return new Color(65, 72, 86, 255);
                case 22:
                    return new Color(24, 42, 87, 255);
                case 23:
                    return new Color(67, 67, 67, 255);
                case 25:
                    return new Color(63, 42, 31, 255);
                case 26:
                    return new Color(111, 50, 45, 255);
                case 27:
                    return new Color(131, 107, 72, 255);
                case 28:
                    return new Color(120, 101, 89, 255);
                case 29:
                    return new Color(77, 76, 61, 255);
                case 30:
                    return new Color(220, 220, 220, 255);
                case 31:
                    return new Color(56, 86, 48, 255);
                case 32:
                    return new Color(123, 79, 25, 255);
                case 33:
                    return new Color(80, 72, 59, 255);
                case 34:
                    return new Color(80, 72, 59, 255);
                case 35:
                    switch (data)
                    {
                        case 0:
                            return new Color(142, 142, 142, 255);
                        case 1:
                            return new Color(150, 81, 34, 255);
                        case 2:
                            return new Color(143, 56, 150, 255);
                        case 5:
                            return new Color(37, 120, 30, 255);
                        case 6:
                            return new Color(139, 83, 98, 255);
                        case 7:
                            return new Color(42, 42, 42, 255);
                        case 8:
                            return new Color(101, 105, 105, 255);
                        case 9:
                            return new Color(24, 74, 95, 255);
                        case 10:
                            return new Color(82, 34, 125, 255);
                        case 11:
                            return new Color(24, 32, 98, 255);
                        case 12:
                            return new Color(54, 32, 17, 255);
                        case 13:
                            return new Color(35, 49, 15, 255);
                        case 14:
                            return new Color(104, 28, 25, 255);
                        case 15:
                            return new Color(16, 14, 14, 255);
                    }
                    break;
                case 37:
                    return new Color(60, 90, 0, 255);
                case 38:
                    switch (data)
                    {
                        case 0:
                            return new Color(100, 57, 4, 255);
                        case 1:
                            return new Color(37, 152, 138, 255);
                        case 2:
                            return new Color(177, 141, 211, 255);
                        case 3:
                            return new Color(162, 191, 138, 255);
                        case 4:
                            return new Color(103, 135, 38, 255);
                        case 5:
                            return new Color(95, 134, 32, 255);
                        case 6:
                            return new Color(94, 153, 65, 255);
                        case 7:
                            return new Color(101, 150, 73, 255);
                        case 8:
                            return new Color(176, 197, 139, 255);
                        default:
                            return new Color(74, 23, 7, 255);
                    }
                case 39:
                    return new Color(138, 105, 83, 255);
                case 40:
                    return new Color(194, 53, 56, 255);
                case 41:
                    return new Color(159, 150, 49, 255);
                case 42:
                    return new Color(139, 139, 139, 255);
                case 43:
                    switch (data)
                    {
                        case 0:
                            return new Color(104, 104, 104, 255);
                        case 2:
                            return new Color(98, 79, 49, 255);
                        case 3:
                            return new Color(78, 78, 78, 255);
                        case 4:
                            return new Color(91, 62, 54, 255);
                        case 5:
                            return new Color(78, 78, 78, 255);
                        case 6:
                            return new Color(28, 14, 16, 255);
                        case 7:
                            return new Color(148, 146, 142, 255);
                        case 8:
                            return new Color(100, 100, 100, 255);
                        case 9:
                            return new Color(138, 133, 100, 255);
                    }
                    break;
                case 44:
                    switch (data)
                    {
                        case 0:
                            return new Color(113, 113, 113, 255);
                        case 1:
                            return new Color(153, 147, 110, 255);
                        case 2:
                            return new Color(110, 89, 55, 255);
                        case 3:
                            return new Color(86, 86, 86, 255);
                        case 4:
                            return new Color(103, 70, 61, 255);
                        case 5:
                            return new Color(85, 85, 85, 255);
                        case 6:
                            return new Color(30, 15, 18, 255);
                        case 7:
                            return new Color(165, 163, 158, 255);
                    }
                    break;
                case 45:
                    return new Color(91, 62, 54, 255);
                case 46:
                    return new Color(98, 51, 38, 255);
                case 47:
                    return new Color(82, 67, 42, 255);
                case 48:
                    return new Color(66, 77, 66, 255);
                case 49:
                    return new Color(12, 11, 18, 255);
                case 50:
                    return new Color(81, 67, 34, 255);
                case 51:
                    return new Color(201, 118, 20, 255);
                case 52:
                    return new Color(42, 44, 39, 255);
                case 54:
                    return new Color(77, 56, 25, 255);
                case 55:
                    return new Color(53, 0, 0, 255);
                case 56:
                    return new Color(81, 87, 89, 255);
                case 57:
                    return new Color(60, 137, 133, 255);
                case 58:
                    return new Color(70, 52, 33, 255);
                case 59:
                    return new Color(86, 101, 7, 255);
                case 60:
                    return new Color(81, 56, 36, 255);
                case 61:
                    return new Color(59, 59, 59, 255);
                case 62:
                    return new Color(69, 64, 60, 255);
                case 63:
                    return new Color(92, 75, 45, 255);
                case 64:
                    return new Color(81, 61, 30, 255);
                case 65:
                    return new Color(121, 95, 52, 255);
                case 66:
                    switch (data)
                    {
                        case 0:
                            return new Color(120, 108, 88, 255);
                        case 1:
                            return new Color(115, 103, 83, 255);
                        case 2:
                            return new Color(115, 103, 83, 255);
                        case 3:
                            return new Color(115, 103, 83, 255);
                        case 4:
                            return new Color(120, 108, 88, 255);
                        case 5:
                            return new Color(120, 108, 88, 255);
                        case 6:
                            return new Color(114, 102, 82, 255);
                        case 7:
                            return new Color(114, 102, 82, 255);
                        case 8:
                            return new Color(114, 102, 82, 255);
                        case 9:
                            return new Color(114, 102, 82, 255);
                    }
                    break;
                case 67:
                    return new Color(81, 81, 81, 255);
                case 68:
                    return new Color(99, 81, 48, 255);
                case 69:
                    return new Color(79, 77, 75, 255);
                case 70:
                    return new Color(108, 108, 108, 255);
                case 71:
                    return new Color(102, 102, 102, 255);
                case 72:
                    return new Color(139, 113, 69, 255);
                case 73:
                    return new Color(84, 69, 69, 255);
                case 74:
                    return new Color(107, 81, 81, 255);
                case 75:
                    return new Color(49, 29, 18, 255);
                case 76:
                    return new Color(85, 36, 17, 255);
                case 77:
                    return new Color(83, 83, 83, 255);
                case 78:
                    return new Color(200, 208, 208, 255);
                case 79:
                    return new Color(80, 111, 163, 255);
                case 80:
                    return new Color(153, 161, 161, 255);
                case 81:
                    return new Color(8, 63, 15, 255);
                case 82:
                    return new Color(101, 105, 113, 255);
                case 83:
                    return new Color(103, 152, 57, 255);
                case 84:
                    return new Color(64, 44, 33, 255);
                case 85:
                    return new Color(87, 71, 43, 255);
                case 86:
                    return new Color(112, 66, 11, 255);
                case 87:
                    return new Color(71, 34, 33, 255);
                case 88:
                    return new Color(53, 40, 32, 255);
                case 89:
                    return new Color(91, 75, 44, 255);
                case 90:
                    return new Color(57, 30, 96, 255);
                case 91:
                    return new Color(119, 77, 14, 255);
                case 92:
                    return new Color(145, 122, 114, 255);
                case 93:
                    return new Color(108, 102, 101, 255);
                case 94:
                    return new Color(121, 101, 98, 255);
                case 95:
                    switch (data)
                    {
                        case 0:
                            return new Color(160, 160, 160, 255);
                        case 1:
                            return new Color(135, 80, 32, 255);
                        case 2:
                            return new Color(111, 48, 135, 255);
                        case 3:
                            return new Color(63, 96, 135, 255);
                        case 4:
                            return new Color(146, 146, 32, 255);
                        case 5:
                            return new Color(80, 128, 15, 255);
                        case 6:
                            return new Color(151, 80, 103, 255);
                        case 7:
                            return new Color(48, 48, 48, 255);
                        case 8:
                            return new Color(96, 96, 96, 255);
                        case 9:
                            return new Color(48, 80, 96, 255);
                        case 10:
                            return new Color(80, 39, 111, 255);
                        case 11:
                            return new Color(32, 48, 111, 255);
                        case 12:
                            return new Color(63, 48, 32, 255);
                        case 13:
                            return new Color(63, 80, 32, 255);
                        case 14:
                            return new Color(96, 32, 32, 255);
                        case 15:
                            return new Color(15, 15, 15, 255);
                    }
                    break;
                case 96:
                    return new Color(96, 70, 33, 255);
                case 97:
                    switch (data)
                    {
                        case 0:
                            return new Color(80, 80, 80, 255);
                        case 1:
                            return new Color(78, 78, 78, 255);
                        case 2:
                            return new Color(78, 78, 78, 255);
                        case 3:
                            return new Color(73, 76, 67, 255);
                        case 4:
                            return new Color(75, 75, 75, 255);
                        case 5:
                            return new Color(76, 76, 76, 255);
                    }
                    break;
                case 98:
                    switch (data)
                    {
                        case 0:
                            return new Color(78, 78, 78, 255);
                        case 1:
                            return new Color(73, 76, 67, 255);
                        case 2:
                            return new Color(75, 75, 75, 255);
                        case 3:
                            return new Color(76, 76, 76, 255);
                    }
                    break;
                case 99:
                    return new Color(90, 68, 53, 255);
                case 100:
                    return new Color(116, 23, 22, 255);
                case 101:
                    return new Color(69, 68, 67, 255);
                case 102:
                    return new Color(118, 132, 135, 255);
                case 103:
                    return new Color(98, 101, 24, 255);
                case 104:
                    return new Color(122, 181, 78, 255);
                case 105:
                    return new Color(122, 181, 78, 255);
                case 106:
                    return new Color(31, 79, 10, 255);
                case 107:
                    return new Color(83, 67, 41, 255);
                case 108:
                    return new Color(97, 66, 57, 255);
                case 109:
                    return new Color(80, 80, 80, 255);
                case 110:
                    return new Color(72, 59, 55, 255);
                case 111:
                    return new Color(17, 86, 24, 255);
                case 112:
                    return new Color(28, 14, 16, 255);
                case 113:
                    return new Color(23, 11, 14, 255);
                case 114:
                    return new Color(29, 14, 17, 255);
                case 115:
                    return new Color(111, 18, 17, 255);
                case 116:
                    return new Color(77, 72, 66, 255);
                case 117:
                    return new Color(70, 66, 59, 255);
                case 118:
                    return new Color(37, 37, 37, 255);
                case 119:
                    return new Color(11, 11, 12, 255);
                case 120:
                    return new Color(73, 86, 69, 255);
                case 121:
                    return new Color(141, 143, 105, 255);
                case 122:
                    return new Color(7, 5, 10, 255);
                case 123:
                    return new Color(43, 26, 16, 255);
                case 124:
                    return new Color(75, 61, 40, 255);
                case 125:
                    switch (data)
                    {
                        case 0:
                            return new Color(110, 89, 55, 255);
                        case 1:
                            return new Color(72, 54, 32, 255);
                        case 2:
                            return new Color(137, 125, 86, 255);
                        case 3:
                            return new Color(108, 77, 54, 255);
                        case 4:
                            return new Color(104, 56, 31, 255);
                        case 5:
                            return new Color(37, 24, 11, 255);
                    }
                    break;
                case 126:
                    switch (data)
                    {
                        case 0:
                            return new Color(110, 89, 55, 255);
                        case 1:
                            return new Color(72, 54, 32, 255);
                        case 2:
                            return new Color(137, 125, 86, 255);
                        case 3:
                            return new Color(108, 77, 54, 255);
                        case 4:
                            return new Color(104, 56, 31, 255);
                        case 5:
                            return new Color(37, 24, 11, 255);
                    }
                    break;
                case 127:
                    return new Color(89, 48, 18, 255);
                case 128:
                    return new Color(143, 138, 104, 255);
                case 129:
                    return new Color(70, 82, 74, 255);
                case 130:
                    return new Color(22, 32, 31, 255);
                case 131:
                    return new Color(99, 85, 61, 255);
                case 132:
                    return new Color(127, 127, 127, 255);
                case 133:
                    return new Color(52, 139, 75, 255);
                case 134:
                    return new Color(68, 51, 30, 255);
                case 135:
                    return new Color(129, 118, 81, 255);
                case 136:
                    return new Color(101, 73, 51, 255);
                case 137:
                    return new Color(112, 86, 69, 255);
                case 138:
                    return new Color(128, 160, 161, 255);
                case 139:
                    switch (data)
                    {
                        case 0:
                            return new Color(76, 76, 76, 255);
                        case 1:
                            return new Color(63, 76, 63, 255);
                    }
                    break;
                case 140:
                    return new Color(85, 50, 40, 255);
                case 141:
                    return new Color(19, 128, 2, 255);
                case 142:
                    return new Color(32, 170, 35, 255);
                case 143:
                    return new Color(102, 83, 51, 255);
                case 144:
                    return new Color(112, 112, 112, 255);
                case 145:
                    return new Color(48, 47, 47, 255);
                case 146:
                    return new Color(77, 56, 25, 255);
                case 147:
                    return new Color(216, 207, 70, 255);
                case 148:
                    return new Color(193, 193, 193, 255);
                case 149:
                    return new Color(111, 103, 101, 255);
                case 150:
                    return new Color(123, 101, 97, 255);
                case 151:
                    return new Color(69, 61, 48, 255);
                case 152:
                    return new Color(110, 17, 5, 255);
                case 153:
                    return new Color(80, 53, 50, 255);
                case 154:
                    return new Color(41, 41, 41, 255);
                case 155:
                    switch (data)
                    {
                        case 1:
                            return new Color(148, 146, 140, 255);
                        case 2:
                            return new Color(148, 146, 141, 255);
                        case 3:
                            return new Color(148, 146, 141, 255);
                        case 4:
                            return new Color(148, 146, 141, 255);
                        default:
                            return new Color(148, 146, 142, 255);
                    }
                case 156:
                    return new Color(156, 154, 149, 255);
                case 157:
                    return new Color(104, 83, 71, 255);
                case 158:
                    return new Color(66, 66, 66, 255);
                case 159:
                    switch (data)
                    {
                        case 0:
                            return new Color(132, 112, 102, 255);
                        case 1:
                            return new Color(102, 53, 23, 255);
                        case 2:
                            return new Color(94, 55, 68, 255);
                        case 3:
                            return new Color(71, 68, 87, 255);
                        case 4:
                            return new Color(118, 84, 22, 255);
                        case 5:
                            return new Color(65, 74, 33, 255);
                        case 6:
                            return new Color(102, 49, 49, 255);
                        case 7:
                            return new Color(36, 26, 22, 255);
                        case 8:
                            return new Color(85, 67, 61, 255);
                        case 9:
                            return new Color(54, 57, 57, 255);
                        case 10:
                            return new Color(74, 44, 54, 255);
                        case 11:
                            return new Color(47, 37, 57, 255);
                        case 12:
                            return new Color(48, 32, 22, 255);
                        case 13:
                            return new Color(48, 52, 26, 255);
                        case 14:
                            return new Color(90, 38, 29, 255);
                        case 15:
                            return new Color(23, 14, 10, 255);
                    }
                    break;
                case 160:
                    switch (data)
                    {
                        case 0:
                            return new Color(156, 156, 156, 255);
                        case 1:
                            return new Color(132, 78, 31, 255);
                        case 2:
                            return new Color(109, 46, 132, 255);
                        case 3:
                            return new Color(62, 93, 132, 255);
                        case 4:
                            return new Color(143, 143, 31, 255);
                        case 5:
                            return new Color(78, 125, 15, 255);
                        case 6:
                            return new Color(148, 78, 100, 255);
                        case 7:
                            return new Color(46, 46, 46, 255);
                        case 8:
                            return new Color(93, 93, 93, 255);
                        case 9:
                            return new Color(46, 78, 93, 255);
                        case 10:
                            return new Color(78, 38, 109, 255);
                        case 11:
                            return new Color(31, 46, 109, 255);
                        case 12:
                            return new Color(62, 46, 31, 255);
                        case 13:
                            return new Color(62, 78, 31, 255);
                        case 14:
                            return new Color(93, 31, 31, 255);
                        case 15:
                            return new Color(15, 15, 15, 255);
                    }
                    break;
                case 161:
                    switch (data)
                    {
                        case 0:
                            return new Color(45, 42, 18, 255);
                        case 1:
                            return new Color(10, 31, 4, 255);
                    }
                    break;
                case 162:
                    switch (data)
                    {
                        case 0:
                            return new Color(78, 59, 49, 255);
                        case 1:
                            return new Color(39, 30, 19, 255);
                        case 4:
                            return new Color(78, 59, 49, 255);
                        case 5:
                            return new Color(39, 30, 19, 255);
                        case 8:
                            return new Color(78, 59, 49, 255);
                        case 9:
                            return new Color(39, 30, 19, 255);
                        case 12:
                            return new Color(78, 59, 49, 255);
                        case 13:
                            return new Color(39, 30, 19, 255);
                    }
                    break;
                case 163:
                    return new Color(109, 59, 32, 255);
                case 164:
                    return new Color(39, 25, 11, 255);
                case 165:
                    return new Color(83, 138, 69, 255);
                case 166:
                    return new Color(210, 25, 25, 255);
                case 167:
                    return new Color(137, 137, 137, 255);
                case 170:
                    return new Color(103, 80, 11, 255);
                case 171:
                    switch (data)
                    {
                        case 0:
                            return new Color(188, 188, 188, 255);
                        case 1:
                            return new Color(186, 106, 53, 255);
                        case 2:
                            return new Color(153, 68, 161, 255);
                        case 3:
                            return new Color(90, 117, 171, 255);
                        case 4:
                            return new Color(152, 143, 34, 255);
                        case 5:
                            return new Color(56, 148, 48, 255);
                        case 6:
                            return new Color(177, 112, 130, 255);
                        case 7:
                            return new Color(54, 54, 54, 255);
                        case 8:
                            return new Color(131, 137, 137, 255);
                        case 9:
                            return new Color(39, 94, 117, 255);
                        case 10:
                            return new Color(107, 52, 155, 255);
                        case 11:
                            return new Color(39, 48, 120, 255);
                        case 12:
                            return new Color(67, 42, 26, 255);
                        case 13:
                            return new Color(44, 60, 23, 255);
                        case 14:
                            return new Color(128, 43, 41, 255);
                        case 15:
                            return new Color(21, 18, 18, 255);
                    }
                    break;
                case 172:
                    return new Color(95, 59, 42, 255);
                case 173:
                    return new Color(11, 11, 11, 255);
                case 174:
                    return new Color(105, 124, 157, 255);
                case 175:
                    switch (data)
                    {
                        case 0:
                            return new Color(140, 149, 39, 255);
                        case 1:
                            return new Color(145, 148, 136, 255);
                        case 4:
                            return new Color(86, 63, 4, 255);
                        case 5:
                            return new Color(91, 109, 98, 255);
                    }
                    break;
            }
            return new Color(128, 0, 128, 255);

        }
    }
}
