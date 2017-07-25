﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using TombEditor.Geometry;

namespace TombEditor.Compilers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_color
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_color4
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_vertex
    {
        public short X;
        public short Y;
        public short Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_face4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Vertices;
        public short Texture;
        public short LightingEffect;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_face3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] Vertices;
        public short Texture;
        public short LightingEffect;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room_info
    {
        public int X;
        public int Z;
        public int YBottom;
        public int YTop;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room_portal
    {
        public ushort AdjoiningRoom;
        public tr_vertex Normal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public tr_vertex[] Vertices;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room_sector
    {
        public ushort FloorDataIndex;
        public short BoxIndex;
        public byte RoomBelow;
        public sbyte Floor;
        public byte RoomAbove;
        public sbyte Ceiling;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr4_room_light
    {
        public int X;
        public int Y;
        public int Z;
        public tr_color Color;
        public byte LightType;
        public ushort Intensity;
        public float In;
        public float Out;
        public float Length;
        public float CutOff;
        public float DirectionX;
        public float DirectionY;
        public float DirectionZ;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room_vertex
    {
        public tr_vertex Vertex;
        public short Lighting1;
        public ushort Attributes;
        public short Lighting2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room_staticmesh
    {
        public uint X;
        public uint Y;
        public uint Z;
        public ushort Rotation;
        public ushort Intensity1;
        public ushort Intensity2;
        public ushort ObjectID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_room
    {
        public tr_room_info Info;
        public uint NumDataWords;
        public ushort NumVertices;
        public tr_room_vertex[] Vertices;
        public ushort NumRectangles;
        public tr_face4[] Rectangles;
        public ushort NumTriangles;
        public tr_face3[] Triangles;
        public ushort NumSprites;
        public ushort NumPortals;
        public tr_room_portal[] Portals;
        public ushort NumZSectors;
        public ushort NumXSectors;
        public tr_room_sector[] Sectors;
        public ushort AmbientIntensity1;
        public ushort AmbientIntensity2;
        public short LightMode;
        public ushort NumLights;
        public tr4_room_light[] Lights;
        public ushort NumStaticMeshes;
        public tr_room_staticmesh[] StaticMeshes;
        public short AlternateRoom;
        public short Flags;
        public byte WaterScheme;
        public byte ReverbInfo;
        public byte AlternateGroup;

        // Helper data
        public tr_sector_aux[,] AuxSectors;
        public TextureSounds[,] TextureSounds;
        public List<int> ReachableRooms;
        public bool Visited;
        public bool Flipped;
        public short FlippedRoom;
        public short BaseRoom;
        public int OriginalRoomId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_mesh
    {
        public tr_vertex Centre;
        public int Radius;
        public short NumVertices;
        public tr_vertex[] Vertices;
        public short NumNormals;
        public tr_vertex[] Normals;
        public short[] Lights;
        public short NumTexturedRectangles;
        public tr_face4[] TexturedRectangles;
        public short NumTexturedTriangles;
        public tr_face3[] TexturedTriangles;
        public short NumColoredRectangles;
        public tr_face4[] ColoredRectangles;
        public short NumColoredTriangles;
        public tr_face3[] ColoredTriangles;
        public int MeshSize;
        public int MeshPointer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_staticmesh
    {
        public uint ObjectID;
        public ushort Mesh;
        public tr_bounding_box VisibilityBox;
        public tr_bounding_box CollisionBox;
        public ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_animatedTextures_set
    {
        public short NumTextures;
        public short[] Textures;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_bounding_box
    {
        public short X1;
        public short X2;
        public short Y1;
        public short Y2;
        public short Z1;
        public short Z2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_moveable
    {
        public uint ObjectID;
        public ushort NumMeshes;
        public ushort StartingMesh;
        public uint MeshTree;
        public uint FrameOffset;
        public ushort Animation;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_item
    {
        public short ObjectID;
        public short Room;
        public int X;
        public int Y;
        public int Z;
        public short Angle;
        public short Intensity1;
        public short Intensity2;
        public ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_sprite_texture
    {
        public ushort Tile;
        public byte X;
        public byte Y;
        public ushort Width;
        public ushort Height;
        public short LeftSide;
        public short TopSide;
        public short RightSide;
        public short BottomSide;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_sprite_sequence
    {
        public int ObjectID;
        public short NegativeLength;
        public short Offset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_animation
    {
        public uint FrameOffset;
        public byte FrameRate;
        public byte FrameSize;
        public ushort StateID;
        public int Speed;
        public int Accel;
        public int SpeedLateral;
        public int AccelLateral;
        public ushort FrameStart;
        public ushort FrameEnd;
        public ushort NextAnimation;
        public ushort NextFrame;
        public ushort NumStateChanges;
        public ushort StateChangeOffset;
        public ushort NumAnimCommands;
        public ushort AnimCommand;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_state_change
    {
        public ushort StateID;
        public ushort NumAnimDispatches;
        public ushort AnimDispatch;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_anim_dispatch
    {
        public short Low;
        public short High;
        public short NextAnimation;
        public short NextFrame;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_box
    {
        public byte Zmin;
        public byte Zmax;
        public byte Xmin;
        public byte Xmax;
        public short TrueFloor;
        public short OverlapIndex;
    }

    public struct tr_box_aux
    {
        public byte Zmin;
        public byte Zmax;
        public byte Xmin;
        public byte Xmax;
        public short TrueFloor;
        public short Clicks;
        public byte RoomBelow;
        public short OverlapIndex;
        public int ZoneID;
        public bool Border;
        public byte NumXSectors;
        public byte NumZSectors;
        public bool IsolatedBox;
        public bool NotWalkableBox;
        public bool Monkey;
        public bool Jump;
        public short Room;
        public short AlternateRoom;
        public bool Portal;
        public bool FlipMap;
    }

    public struct tr_overlap_aux
    {
        public int MainBox;
        public int Box;
        public bool Skeleton;
        public bool Monkey;
        public bool EndOfList;
        public bool IsEdge;
    }

    public struct tr_sector_aux
    {
        public bool Wall;
        public bool SoftSlope;
        public bool HardSlope;
        public bool Monkey;
        public bool Box;
        public bool BorderWall;
        public bool Portal;
        public bool IsFloorSolid;
        public bool NotWalkableFloor;
        public int WallPortal;
        public int FloorPortal;
        public int CeilingPortal;
        public int BoxIndex;
        public short LowestFloor;
        public int RoomAbove;
        public int RoomBelow;
        public short MeanFloorHeight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_zone
    {
        public ushort GroundZone1_Normal;
        public ushort GroundZone2_Normal;
        public ushort GroundZone3_Normal;
        public ushort GroundZone4_Normal;
        public ushort FlyZone_Normal;
        public ushort GroundZone1_Alternate;
        public ushort GroundZone2_Alternate;
        public ushort GroundZone3_Alternate;
        public ushort GroundZone4_Alternate;
        public ushort FlyZone_Alternate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_sound_source
    {
        public int X;
        public int Y;
        public int Z;
        public ushort SoundID;
        public ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_sound_details
    {
        public short Sample;
        public short Volume;
        public short Unknown1;
        public short Unknown2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_object_texture_vert
    {
        public byte Xpixel;
        public byte Xcoordinate;
        public byte Ypixel;
        public byte Ycoordinate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_object_texture
    {
        public ushort Attributes;
        public ushort Tile;
        public ushort Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public tr_object_texture_vert[] Vertices;
        public uint Unknown1;
        public uint Unknown2;
        public uint Xsize;
        public uint Ysize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_camera
    {
        public int X;
        public int Y;
        public int Z;
        public short Room;
        public ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_animatedTextures
    {
        public short NumTextureID;
        public short[] TextureID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr_ai_item
    {
        public ushort ObjectID;
        public ushort Room;
        public int X;
        public int Y;
        public int Z;
        public ushort OCB;
        public ushort Flags;
        public int Angle;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tr4_flyby_camera
    {
        public int X;
        public int Y;
        public int Z;
        public int DirectionX;
        public int DirectionY;
        public int DirectionZ;

        public byte Sequence;
        public byte Index;

        public ushort FOV;
        public short Roll;
        public ushort Timer;
        public ushort Speed;
        public ushort Flags;

        public int Room;
    }
}
