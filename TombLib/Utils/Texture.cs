﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombLib.Utils
{
    public abstract class Texture : ICloneable
    {
        public static ImageC UnloadedPlaceholder { get; } = ImageC.Black;

        // Do not change the image with this methode
        public ImageC Image { get; protected set; } = UnloadedPlaceholder;

        public virtual bool ReplaceMagentaWithTransparency => false;

        public abstract Texture Clone();

        object ICloneable.Clone()
        {
            return Clone();
        }

        public bool IsUnavailable => Image == UnloadedPlaceholder;
        public bool IsAvailable => Image != UnloadedPlaceholder;
    }

    public class TextureInvisible : Texture
    {
        public static Texture Instance { get; } = new TextureInvisible();

        private TextureInvisible()
        {
            Image = ImageC.Transparent;
        }

        public override Texture Clone()
        {
            return Instance;
        }
    }

    public enum BlendMode : ushort
    {
        Normal = 0,
        Additive = 2,
        NoZTest = 4,
        Subtract = 5,
        Wireframe = 6,
        Exclude = 8,
        Screen = 9,
        Lighten = 10,
        AlphaTest = 100
        // By using FLEP, more BlendMode mode indices can be assigned meaning.
        // We probably want support for those in the future.
    }

    public enum BumpMapMode : ushort
    {
        None = 0,
        Level1 = 1,
        Level2 = 2
    }

    public struct TextureArea
    {
        public static readonly TextureArea None;

        public Texture Texture;
        public Vector2 TexCoord0; // No array for those because:
        public Vector2 TexCoord1; //    - Cache locality
        public Vector2 TexCoord2; //    - No array bounds checks
        public Vector2 TexCoord3; //    - 'Clone', 'GetHashCode' and so on work by default
        public BlendMode BlendMode;
        public BumpMapMode BumpMode;
        public bool DoubleSided;

        public static bool operator ==(TextureArea first, TextureArea second)
        {
            return
                (first.Texture == second.Texture) &&
                (first.TexCoord0.Equals(second.TexCoord0)) &&
                (first.TexCoord1.Equals(second.TexCoord1)) &&
                (first.TexCoord2.Equals(second.TexCoord2)) &&
                (first.TexCoord3.Equals(second.TexCoord3)) &&
                (first.BlendMode == second.BlendMode) &&
                (first.BumpMode == second.BumpMode) &&
                (first.DoubleSided == second.DoubleSided);
        }

        public static bool operator !=(TextureArea first, TextureArea second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return this == (TextureArea)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool TextureIsUnavailable => (Texture == null) || (Texture.IsUnavailable);

        public bool TextureIsInvisble => (Texture == null) || (Texture == TextureInvisible.Instance) || (Texture.IsUnavailable);

        public bool TextureIsRectangle => ((TexCoord0 + TexCoord2).Length() == (TexCoord1 + TexCoord3).Length());

        public int AllocIndex => (int)BumpMode;

        public IEnumerable<KeyValuePair<int, Vector2>> TexCoords
        {
            get
            {
                yield return new KeyValuePair<int, Vector2>(0, TexCoord0);
                yield return new KeyValuePair<int, Vector2>(1, TexCoord1);
                yield return new KeyValuePair<int, Vector2>(2, TexCoord2);
                yield return new KeyValuePair<int, Vector2>(3, TexCoord3);
            }
        }

        public Vector2 GetTexCoord(int index)
        {
            switch (index)
            {
                case 0:
                    return TexCoord0;
                case 1:
                    return TexCoord1;
                case 2:
                    return TexCoord2;
                case 3:
                    return TexCoord3;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetTexCoord(int index, Vector2 value)
        {
            switch (index)
            {
                case 0:
                    TexCoord0 = value;
                    break;
                case 1:
                    TexCoord1 = value;
                    break;
                case 2:
                    TexCoord2 = value;
                    break;
                case 3:
                    TexCoord3 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static float CalculateArea(Vector2 texCoord0, Vector2 texCoord1)
        {
            return (texCoord1.X - texCoord0.X) * (texCoord1.Y + texCoord0.Y);
        }

        public float TriangleArea
        {
            get
            {
                return (CalculateArea(TexCoord0, TexCoord1) +
                    CalculateArea(TexCoord1, TexCoord2) +
                    CalculateArea(TexCoord2, TexCoord0)) * 0.5f;
            }
        }

        public float QuadArea
        {
            get
            {
                return (CalculateArea(TexCoord0, TexCoord1) +
                    CalculateArea(TexCoord1, TexCoord2) +
                    CalculateArea(TexCoord2, TexCoord3) +
                    CalculateArea(TexCoord3, TexCoord0)) * 0.5f;
            }
        }
    }
}
