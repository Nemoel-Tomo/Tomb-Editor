﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using TombLib.Wad.Catalog;

namespace TombLib.Wad
{
    public struct WadStaticId : IWadObjectId, IEquatable<WadStaticId>, IComparable<WadStaticId>, IComparable
    {
        public uint TypeId;

        public WadStaticId(uint objTypeId)
        {
            TypeId = objTypeId;
        }

        public int CompareTo(WadStaticId other) => TypeId.CompareTo(other.TypeId);
        public int CompareTo(object other) => CompareTo((WadStaticId)other);
        public static bool operator <(WadStaticId first, WadStaticId second) => (first.TypeId < second.TypeId);
        public static bool operator <=(WadStaticId first, WadStaticId second) => (first.TypeId <= second.TypeId);
        public static bool operator >(WadStaticId first, WadStaticId second) => (first.TypeId > second.TypeId);
        public static bool operator >=(WadStaticId first, WadStaticId second) => (first.TypeId >= second.TypeId);
        public static bool operator ==(WadStaticId first, WadStaticId second) => (first.TypeId == second.TypeId);
        public static bool operator !=(WadStaticId first, WadStaticId second) => !(first == second);
        public bool Equals(WadStaticId other) => this == other;
        public override bool Equals(object other) => (other is WadStaticId) && this == (WadStaticId)other;
        public override int GetHashCode() => unchecked((int)TypeId);

        public string ToString(WadGameVersion gameVersion)
        {
            return "(" + TypeId + ") " + TrCatalog.GetStaticName(gameVersion, TypeId);
        }
        public override string ToString() => "Uncertain game version - " + ToString(WadGameVersion.TR4_TRNG);
    }

    public class WadStatic : IWadObject, ICloneable
    {
        public WadStaticId Id { get; private set; }

        public WadStatic(WadStaticId id)
        {
            Id = id;
        }

        public WadMesh Mesh { get; set; }
        public short Flags { get; set; }
        public BoundingBox VisibilityBox { get; set; }
        public BoundingBox CollisionBox { get; set; }

        public string ToString(WadGameVersion gameVersion) => Id.ToString(gameVersion);
        public override string ToString() => "Uncertain game version - " + ToString(WadGameVersion.TR4_TRNG);

        public WadStatic Clone()
        {
            WadStatic clone = (WadStatic)MemberwiseClone();
            clone.Mesh = Mesh.Clone();
            return clone;
        }
        object ICloneable.Clone() => Clone();

        IWadObjectId IWadObject.Id => Id;
    }
}