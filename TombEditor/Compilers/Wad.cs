﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombLib.Wad;

namespace TombEditor.Compilers
{
    public partial class LevelCompilerTr4
    {
        private Dictionary<WadMesh, int> _tempMeshPointers;

        private void ConvertWadMeshes()
        {
            ReportProgress(11, "Converting WAD meshes to TR4 format");

            var wad = _level.Wad;

            _tempMeshPointers = new Dictionary<WadMesh, int>();
            _meshes = new tr_mesh[wad.Meshes.Count];

            ReportProgress(12, "    Number of meshes: " + wad.Meshes.Count);

            int currentMeshSize = 0;
            int totalMeshSize = 0;

            for (int i = 0; i < wad.Meshes.Count; i++)
            {
                var oldMesh = wad.Meshes.ElementAt(i).Value;

                currentMeshSize = 0;

                var newMesh = new tr_mesh
                {
                    Center = new tr_vertex
                    {
                        X = (short)oldMesh.BoundingSphere.Center.X,
                        Y = (short)-oldMesh.BoundingSphere.Center.Y,
                        Z = (short)oldMesh.BoundingSphere.Center.Z
                    },
                    Radius = (short)oldMesh.BoundingSphere.Radius
                };

                currentMeshSize += 10;

                newMesh.NumVertices = (short)oldMesh.VerticesPositions.Count;
                currentMeshSize += 2;

                newMesh.Vertices = new tr_vertex[oldMesh.VerticesPositions.Count];

                for (int j = 0; j < oldMesh.VerticesPositions.Count; j++)
                {
                    var vertex = oldMesh.VerticesPositions[j];
                    var newVertex = new tr_vertex
                    {
                        X = (short)vertex.X,
                        Y = (short)-vertex.Y,
                        Z = (short)vertex.Z
                    };

                    newMesh.Vertices[j] = newVertex;

                    currentMeshSize += 6;
                }

                newMesh.NumNormals = (short)(oldMesh.VerticesNormals.Count > 0 ? oldMesh.VerticesNormals.Count : -oldMesh.VerticesShades.Count);
                currentMeshSize += 2;

                if (oldMesh.VerticesNormals.Count > 0)
                {
                    newMesh.Normals = new tr_vertex[oldMesh.VerticesNormals.Count];

                    for (int j = 0; j < oldMesh.VerticesNormals.Count; j++)
                    {
                        var normal = oldMesh.VerticesNormals[j];
                        var newNormal = new tr_vertex
                        {
                            X = (short)normal.X,
                            Y = (short)-normal.Y,
                            Z = (short)normal.Z
                        };

                        newMesh.Normals[j] = newNormal;

                        currentMeshSize += 6;
                    }
                }
                else
                {
                    newMesh.Lights = new short[oldMesh.VerticesShades.Count];

                    for (int j = 0; j < oldMesh.VerticesShades.Count; j++)
                    { 
                        newMesh.Lights[j] = oldMesh.VerticesShades[j];

                        currentMeshSize += 2;
                    }
                }

                short numRectangles = 0;
                short numTriangles = 0;

                foreach (var poly in oldMesh.Polys)
                {
                    if (poly.Shape == WadPolygonShape.Rectangle)
                        numRectangles++;
                    else
                        numTriangles++;
                }

                newMesh.NumTexturedQuads = numRectangles;
                currentMeshSize += 2;

                newMesh.NumTexturedTriangles = numTriangles;
                currentMeshSize += 2;

                int lastRectangle = 0;
                int lastTriangle = 0;

                newMesh.TexturedQuads = new tr_face4[numRectangles];
                newMesh.TexturedTriangles = new tr_face3[numTriangles];

                foreach (var poly in oldMesh.Polys)
                {
                    if (poly.Shape == WadPolygonShape.Rectangle)
                    {
                        tr_face4 face = new tr_face4();

                        face.Vertices = new ushort[4];
                        face.Vertices[0] = (ushort)poly.Indices[0];
                        face.Vertices[1] = (ushort)poly.Indices[1];
                        face.Vertices[2] = (ushort)poly.Indices[2];
                        face.Vertices[3] = (ushort)poly.Indices[3];

                        var result = _objectTextureManager.AddTexture(_level.Wad.GetTextureArea(poly), false, false);
                        face.Texture = result.ObjectTextureIndex;

                        face.LightingEffect = (ushort)(poly.Transparent ? 0x01 : 0x00);
                        face.LightingEffect |= (ushort)(poly.ShineStrength << 1);

                        newMesh.TexturedQuads[lastRectangle] = face;

                        currentMeshSize += 12;
                        lastRectangle++;
                    }
                    else
                    {
                        tr_face3 face = new tr_face3();

                        face.Vertices = new ushort[3];
                        face.Vertices[0] = (ushort)poly.Indices[0];
                        face.Vertices[1] = (ushort)poly.Indices[1];
                        face.Vertices[2] = (ushort)poly.Indices[2];

                        var result = _objectTextureManager.AddTexture(_level.Wad.GetTextureArea(poly), true, false);
                        face.Texture = result.ObjectTextureIndex;

                        face.LightingEffect = (ushort)(poly.Transparent ? 0x01 : 0x00);
                        face.LightingEffect |= (ushort)(poly.ShineStrength << 1);

                        newMesh.TexturedTriangles[lastTriangle] = face;

                        currentMeshSize += 10;
                        lastTriangle++;
                    }
                }

                if (currentMeshSize % 4 != 0)
                {
                    currentMeshSize += 2;
                }

                newMesh.MeshSize = currentMeshSize;
                newMesh.MeshPointer = totalMeshSize;
                _tempMeshPointers.Add(oldMesh, totalMeshSize);

                totalMeshSize += currentMeshSize;

                _meshes[i] = newMesh;
            }
        }

        public void ConvertWad2DataToTr4()
        {
            var tempAnimations = new List<tr_animation>();
            var tempDispatches = new List<tr_anim_dispatch>();
            var tempStateChanges = new List<tr_state_change>();
            var tempAnimCommands = new List<ushort>();
            var tempMoveables = new List<tr_moveable>();
            var tempStaticMeshes = new List<tr_staticmesh>();
            var tempKeyFrames = new List<short>();
            var tempMeshPointers = new List<uint>();
            var tempMeshTrees = new List<int>();

            var wad = _level.Wad;

            int lastAnimation = 0;
            int lastKeyFrame = 0;
            int lastAnimCommand = 0;
            int lastStateChange = 0;
            int lastAnimDispatch = 0;
            int lastMeshTree = 0;
            int lastMeshPointer = 0;

            // First thing build frames
            var keyframesDictionary = new Dictionary<WadKeyFrame, uint>();

            int currentKeyFrameSize = 0;
            int totalKeyFrameSize = 0;
            int mmm = 0;

            for (int i = 0; i < wad.Moveables.Count; i++)
            {
                foreach (var animation in wad.Moveables.ElementAt(i).Value.Animations)
                {
                    animation.KeyFramesOffset = totalKeyFrameSize * 2;

                    // First I need to calculate the max frame size because I will need to pad later with 0x00
                    int maxKeyFrameSize = 0;
                    foreach (var keyframe in animation.KeyFrames)
                    {
                        currentKeyFrameSize = 9;

                        foreach (var angle in keyframe.Angles)
                        {
                            if (angle.Axis == WadKeyFrameRotationAxis.ThreeAxes)
                                currentKeyFrameSize += 2;
                            else
                                currentKeyFrameSize += 1;
                        }

                        if (currentKeyFrameSize > maxKeyFrameSize) maxKeyFrameSize = currentKeyFrameSize;
                    }

                    foreach (var keyframe in animation.KeyFrames)
                    {
                        currentKeyFrameSize = 0;
                        int baseFrame = tempKeyFrames.Count;

                        tempKeyFrames.Add((short)keyframe.BoundingBox.Minimum.X);
                        tempKeyFrames.Add((short)keyframe.BoundingBox.Maximum.X);
                        tempKeyFrames.Add((short)-keyframe.BoundingBox.Minimum.Y);
                        tempKeyFrames.Add((short)-keyframe.BoundingBox.Maximum.Y);
                        tempKeyFrames.Add((short)keyframe.BoundingBox.Minimum.Z);
                        tempKeyFrames.Add((short)keyframe.BoundingBox.Maximum.Z);

                        currentKeyFrameSize += 6;

                        tempKeyFrames.Add((short)keyframe.Offset.X);
                        tempKeyFrames.Add((short)-keyframe.Offset.Y);
                        tempKeyFrames.Add((short)keyframe.Offset.Z);

                        currentKeyFrameSize += 3;

                        foreach (var angle in keyframe.Angles)
                        {
                            //long rotation32 = 0;
                            short rotation16 = 0;
                            short rotX = 0;
                            short rotY = 0;
                            short rotZ = 0;

                            switch (angle.Axis)
                            {
                                case WadKeyFrameRotationAxis.ThreeAxes:
                                    rotation16 = (short)((angle.X << 4) | ((angle.Y & 0xfc0) >> 6));
                                    tempKeyFrames.Add(rotation16);

                                    rotation16= (short)(((angle.Y & 0x3f) << 10) | (angle.Z & 0x3ff));
                                    tempKeyFrames.Add(rotation16);

                                    currentKeyFrameSize += 2;

                                    break;

                                case WadKeyFrameRotationAxis.AxisX:
                                    rotation16 = unchecked((short)0x4000);
                                    rotX = (short)angle.X;
                                    rotation16 |= rotX;

                                    tempKeyFrames.Add(rotation16);
                                    //Console.WriteLine(rotation16.ToString("X"));

                                    currentKeyFrameSize += 1;

                                    break;

                                case WadKeyFrameRotationAxis.AxisY:
                                    rotation16 = unchecked((short)0x8000);
                                    rotY = (short)angle.Y;
                                    rotation16 |= rotY;

                                    tempKeyFrames.Add(rotation16);
                                    //Console.WriteLine(rotation16.ToString("X"));
                                    
                                    currentKeyFrameSize += 1;

                                    break;

                                case WadKeyFrameRotationAxis.AxisZ:
                                    rotation16 = unchecked((short)0xc000);
                                    rotZ = (short)angle.Z;
                                    rotation16 += rotZ;

                                    tempKeyFrames.Add(rotation16);
                                    //Console.WriteLine(rotation16.ToString("X"));
                                    
                                    currentKeyFrameSize += 1;

                                    break;
                            }
                        }

                        // Padding
                        if (currentKeyFrameSize < maxKeyFrameSize)
                        {
                            for (int p = 0; p < (maxKeyFrameSize - currentKeyFrameSize); p++)
                            {
                                tempKeyFrames.Add(0);                                
                            }

                            currentKeyFrameSize += maxKeyFrameSize - currentKeyFrameSize;
                        }

                        int endFrame = tempKeyFrames.Count;

                        if (mmm==0)
                        {
                            for (int jjj = baseFrame; jjj < endFrame; jjj++)
                                Console.WriteLine(tempKeyFrames[jjj].ToString("X"));
                            Console.WriteLine("----------------------------");
                        }

                        keyframesDictionary.Add(keyframe, (uint)totalKeyFrameSize);
                        totalKeyFrameSize += currentKeyFrameSize;
                    }

                    animation.KeyFramesSize = maxKeyFrameSize;

                    mmm++;
                }
            }

            for (int i=0;i<wad.Moveables.Count;i++)
            {
                var oldMoveable = wad.Moveables.ElementAt(i).Value;
                var newMoveable = new tr_moveable();

                newMoveable.Animation = (ushort)(oldMoveable.Animations.Count != 0 ? lastAnimation : -1);
                newMoveable.FrameOffset = (uint)lastKeyFrame;
                newMoveable.NumMeshes = (ushort)oldMoveable.Meshes.Count;
                newMoveable.ObjectID = oldMoveable.ObjectID;
                newMoveable.MeshTree = (uint)lastMeshTree;
                newMoveable.StartingMesh = (ushort)lastMeshPointer;

                int numAnimations = 0;
                int numStateChanges = 0;
                int numAnimCommands = 0;
                int numKeyFrames = 0;

                // Add animations
                foreach (var animation in oldMoveable.Animations)
                {
                    var newAnimation = new tr_animation();

                    // Setup the final animation
                    newAnimation.FrameOffset = (uint)animation.KeyFramesOffset;
                    newAnimation.FrameRate = animation.FrameDuration;
                    newAnimation.FrameSize = (byte)animation.KeyFramesSize;
                    newAnimation.Speed = animation.Speed;
                    newAnimation.Accel = animation.Acceleration;
                    newAnimation.SpeedLateral = animation.LateralSpeed;
                    newAnimation.AccelLateral = animation.LateralAcceleration;
                    newAnimation.FrameStart = (ushort)(animation.FrameStart);
                    newAnimation.FrameEnd = (ushort)(animation.FrameEnd);
                    newAnimation.AnimCommand = (ushort)(tempAnimCommands.Count);
                    newAnimation.StateChangeOffset = (ushort)(tempStateChanges.Count);
                    newAnimation.NumAnimCommands = (ushort)animation.AnimCommands.Count;
                    newAnimation.NumStateChanges = (ushort)animation.StateChanges.Count;
                    newAnimation.NextAnimation = (ushort)(animation.NextAnimation + lastAnimation);
                    newAnimation.NextFrame = (ushort)(animation.NextFrame);
                    newAnimation.StateID = (animation.StateId);

                    int index = oldMoveable.Animations.ReferenceIndexOf(animation);
                    if (index>0)
                    {
                        ushort frameBase = 0; // oldMoveable.Animations[oldMoveable.Animations.ReferenceIndexOf(animation)].FrameBase;
                        for (int k = 0; k < index; k++) frameBase += oldMoveable.Animations[k].RealNumberOfFrames;


                        newAnimation.FrameStart += frameBase;
                        newAnimation.FrameEnd += frameBase;
                    }

                    numKeyFrames += animation.KeyFrames.Count;

                    // Add anim commands
                    foreach (var command in animation.AnimCommands)
                    {
                        switch (command.Type)
                        {
                            case WadAnimCommandType.PositionReference:
                                tempAnimCommands.Add(0x01);

                                tempAnimCommands.Add(command.Parameter1);
                                tempAnimCommands.Add(command.Parameter2);
                                tempAnimCommands.Add(command.Parameter3);

                                break;

                            case WadAnimCommandType.JumpReference:
                                tempAnimCommands.Add(0x02);

                                tempAnimCommands.Add(command.Parameter1);
                                tempAnimCommands.Add(command.Parameter2);

                                break;

                            case WadAnimCommandType.EmptyHands:
                                tempAnimCommands.Add(0x03);

                                break;

                            case WadAnimCommandType.KillEntity:
                                tempAnimCommands.Add(0x04);

                                break;

                            case WadAnimCommandType.PlaySound:
                                tempAnimCommands.Add(0x05);

                                tempAnimCommands.Add(command.Parameter1);
                                tempAnimCommands.Add(command.Parameter2);

                                break;

                            case WadAnimCommandType.FlipEffect:
                                tempAnimCommands.Add(0x06);

                                tempAnimCommands.Add(command.Parameter1);
                                tempAnimCommands.Add(command.Parameter2);

                                break;
                        }
                    }

                    // Add state changes
                    foreach (var stateChange in animation.StateChanges)
                    {
                        var newStateChange = new tr_state_change();

                        newStateChange.AnimDispatch = (ushort)lastAnimDispatch;
                        newStateChange.StateID = stateChange.StateId;
                        newStateChange.NumAnimDispatches = (ushort)(stateChange.Dispatches.Count);

                        foreach (var dispatch in stateChange.Dispatches)
                        {
                            var newAnimDispatch = new tr_anim_dispatch();

                            newAnimDispatch.Low = (ushort)(dispatch.InFrame+newAnimation.FrameStart);
                            newAnimDispatch.High = (ushort)(dispatch.OutFrame + newAnimation.FrameStart);
                            newAnimDispatch.NextAnimation = (ushort)(dispatch.NextAnimation + lastAnimation);
                            newAnimDispatch.NextFrame = (ushort)(dispatch.NextFrame);

                            tempDispatches.Add(newAnimDispatch);
                        }

                        lastAnimDispatch += stateChange.Dispatches.Count;

                        tempStateChanges.Add(newStateChange);
                    }

                    tempAnimations.Add(newAnimation);

                    numAnimations++;
                    numAnimCommands += animation.AnimCommands.Count;
                    numStateChanges += animation.StateChanges.Count;
                }

                lastAnimation += numAnimations;
                lastKeyFrame += numKeyFrames;

                newMoveable.MeshTree = (uint)tempMeshTrees.Count;
                newMoveable.StartingMesh = (ushort)tempMeshPointers.Count;

                // Now build mesh pointers and mesh trees
                foreach (var meshTree in oldMoveable.Links)
                {
                    tempMeshTrees.Add((int)meshTree.Opcode);
                    tempMeshTrees.Add((int)meshTree.Offset.X);
                    tempMeshTrees.Add((int)-meshTree.Offset.Y);
                    tempMeshTrees.Add((int)meshTree.Offset.Z);
                }
                
                foreach (var mesh in oldMoveable.Meshes)
                {
                    tempMeshPointers.Add((uint)_tempMeshPointers[mesh]);
                }

                tempMoveables.Add(newMoveable);
            }

            // Adjust NextFrame of each Animation
            for (int i=0;i<tempAnimations.Count;i++)
            {
                var animation = tempAnimations[i];
                animation.NextFrame += tempAnimations[animation.NextAnimation].FrameStart;
                tempAnimations[i] = animation;
            }

            // Adjust NextFrame of each AnimDispatch
            for (int i = 0; i < tempDispatches.Count; i++)
            {
                var dispatch = tempDispatches[i];
                dispatch.NextFrame += tempAnimations[dispatch.NextAnimation].FrameStart;
                tempDispatches[i] = dispatch;
            }

            // Convert static meshes
            for (int i=0;i<wad.Statics.Count;i++)
            {
                var oldStaticMesh = wad.Statics.ElementAt(i).Value;
                var newStaticMesh = new tr_staticmesh();

                newStaticMesh.ObjectID = oldStaticMesh.ObjectID;

                newStaticMesh.CollisionBox = new tr_bounding_box
                {
                    X1 = (short)oldStaticMesh.CollisionBox.Minimum.X,
                    X2 = (short)oldStaticMesh.CollisionBox.Maximum.X,
                    Y1 = (short)-oldStaticMesh.CollisionBox.Minimum.Y,
                    Y2 = (short)-oldStaticMesh.CollisionBox.Maximum.Y,
                    Z1 = (short)oldStaticMesh.CollisionBox.Minimum.Z,
                    Z2 = (short)oldStaticMesh.CollisionBox.Maximum.Z
                };

                newStaticMesh.VisibilityBox = new tr_bounding_box
                {
                    X1 = (short)oldStaticMesh.VisibilityBox.Minimum.X,
                    X2 = (short)oldStaticMesh.VisibilityBox.Maximum.X,
                    Y1 = (short)-oldStaticMesh.VisibilityBox.Minimum.Y,
                    Y2 = (short)-oldStaticMesh.VisibilityBox.Maximum.Y,
                    Z1 = (short)oldStaticMesh.VisibilityBox.Minimum.Z,
                    Z2 = (short)oldStaticMesh.VisibilityBox.Maximum.Z
                };

                newStaticMesh.Flags = (ushort)oldStaticMesh.Flags;
               newStaticMesh.Mesh = (ushort)tempMeshPointers.Count;
                
                tempStaticMeshes.Add(newStaticMesh);

                tempMeshPointers.Add((uint)_tempMeshPointers[oldStaticMesh.Mesh]);
            }

            _animations = tempAnimations.ToArray();
            _meshPointers = tempMeshPointers.ToArray();
            _meshTrees = tempMeshTrees.ToArray();
            _stateChanges = tempStateChanges.ToArray();
            _animDispatches = tempDispatches.ToArray();
            _animCommands = tempAnimCommands.ToArray();
            _frames = tempKeyFrames.ToArray();
            _moveables = tempMoveables.ToArray();
            _staticMeshes = tempStaticMeshes.ToArray();

            if (File.Exists("Wad.txt")) File.Delete("Wad.txt");

            StreamWriter writer = new StreamWriter(File.OpenWrite("Wad.txt"));
            int n = 0;
            foreach (var anim in tempAnimations)
            {
                writer.WriteLine("Anim #" + n);
                writer.WriteLine("    KeyframeOffset: " + anim.FrameOffset);
                writer.WriteLine("    FrameRate: " + anim.FrameRate);
                writer.WriteLine("    KeyFrameSize: " + anim.FrameSize);
                writer.WriteLine("    FrameStart: " + anim.FrameStart);
                writer.WriteLine("    FrameEnd: " + anim.FrameEnd);
                writer.WriteLine("    StateChangeOffset: " + anim.StateChangeOffset);
                writer.WriteLine("    NumStateChanges: " + anim.NumStateChanges);
                writer.WriteLine("    AnimCommand: " + anim.AnimCommand);
                writer.WriteLine("    NumAnimCommands: " + anim.NumAnimCommands);
                writer.WriteLine("    NextAnimation: " + anim.NextAnimation);
                writer.WriteLine("    NextFrame: " + anim.NextFrame);
                writer.WriteLine("    StateID: " + anim.StateID);
                writer.WriteLine("    Speed: " + anim.Speed.ToString("X"));
                writer.WriteLine("    Accel: " + anim.Accel.ToString("X"));
                writer.WriteLine("    SpeedLateral: " + anim.SpeedLateral.ToString("X"));
                writer.WriteLine("    AccelLateral: " + anim.AccelLateral.ToString("X"));
                writer.WriteLine();

                n++;
            }

            n = 0;
            foreach (var dispatch in tempDispatches)
            {
                writer.WriteLine("AnimDispatch #" + n);
                writer.WriteLine("    In: " + dispatch.Low);
                writer.WriteLine("    Out: " + dispatch.High);
                writer.WriteLine("    NextAnimation: " + dispatch.NextAnimation);
                writer.WriteLine("    NextFrame: " + dispatch.NextFrame);
                writer.WriteLine();

                n++;
            }

            n = 0;
            for  (int jj=0;jj<tempMeshTrees.Count;jj+=4)
            {
                writer.WriteLine("MeshTree #" + jj);
                writer.WriteLine("    Op: " + tempMeshTrees[jj + 0]);
                writer.WriteLine("    X: " + tempMeshTrees[jj + 1]);
                writer.WriteLine("    Y: " + tempMeshTrees[jj + 2]);
                writer.WriteLine("    Z: " + tempMeshTrees[jj + 3]);
                writer.WriteLine();

                n++;
            }

            n = 0;
            for (int jj = 0; jj < tempMeshPointers.Count; jj++)
            {
                writer.WriteLine("MeshPointer #" + jj + ": " + tempMeshPointers[jj]);

                n++;
            }

            n = 0;
            foreach (var mesh in _meshes)
            {
                writer.WriteLine("Mesh #" + n);
                writer.WriteLine("    Vertices: " + mesh.NumVertices);
                writer.WriteLine("    Normals: " + mesh.NumNormals);
                writer.WriteLine("    Polygons: " + (mesh.NumTexturedQuads+mesh.NumTexturedTriangles));
                writer.WriteLine("    MeshPointer: " + mesh.MeshPointer);
                writer.WriteLine();

                n++;
            }

            n = 0;
            foreach (var mov in _moveables)
            {
                writer.WriteLine("Moveable #" + n);
                writer.WriteLine("    MeshTree: " + mov.MeshTree);
                writer.WriteLine("    MeshPointer: " + mov.StartingMesh);
                writer.WriteLine("    AnimationIndex: " + mov.Animation);
                writer.WriteLine("    NumMeshes: " + mov.NumMeshes);
                writer.WriteLine();

                n++;
            }

            n = 0;
            foreach (var sc in _stateChanges)
            {
                writer.WriteLine("StateChange #" + n);
                writer.WriteLine("    StateID: " + sc.StateID);
                writer.WriteLine("    NumAnimDispatches: " + sc.NumAnimDispatches);
                writer.WriteLine("    AnimDispatch: " + sc.AnimDispatch);
                writer.WriteLine();

                n++;
            }

            writer.Flush();
            writer.Close();
        }
    }
}
