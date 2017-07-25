﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Runtime.InteropServices;
using SharpDX.Toolkit.Graphics;

namespace TombLib.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkinnedVertex
    {
        [VertexElementAttribute("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0)]
        public Vector4 Position;
        [VertexElementAttribute("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16)]
        public Vector2 UV;
        [VertexElementAttribute("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 24)]
        public Vector3 Normal;
        [VertexElementAttribute("TANGENT", 0, SharpDX.DXGI.Format.R32G32B32_Float, 36)]
        public Vector3 Tangent;
        [VertexElementAttribute("BINORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 48)]
        public Vector3 Binormal;
        [VertexElementAttribute("BONEWEIGTH", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 60)]
        public Vector4 BoneWeigths;
        [VertexElementAttribute("BLENDINDICES", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 76)]
        public Vector4 BoneIndices;
    }
}
