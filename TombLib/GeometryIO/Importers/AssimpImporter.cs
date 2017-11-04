﻿using Assimp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombLib.Utils;

namespace TombLib.GeometryIO.Importers
{
    public class AssimpImporter : BaseGeometryImporter
    {
        public AssimpImporter(IOGeometrySettings settings)
            : base (settings)
        {

        }

        public override IOModel ImportFromFile(string filename)
        {
            // Use Assimp.NET for importing model
            AssimpContext context = new AssimpContext();
            Scene scene = context.ImportFile(filename, PostProcessPreset.TargetRealTimeMaximumQuality);

            var newModel = new IOModel();
            var textures = new Dictionary<int, IOTexture>();

            // Create the list of textures to load
            for (int i = 0; i < scene.Materials.Count; i++)
            {
                var mat = scene.Materials[i];

                var diffusePath = (mat.HasTextureDiffuse ? mat.TextureDiffuse.FilePath : null);
                if (diffusePath == null || diffusePath == "") continue;

                var found = false;
                for (var j = 0; j < textures.Count; j++)
                    if (textures.ElementAt(j).Value.Name == diffusePath)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    textures.Add(i, new IOTexture(diffusePath, ImageC.FromFile(diffusePath)));
            }

            foreach (var text in textures)
                newModel.Textures.Add(text.Value.Name);

            var lastBaseVertex = 0;
            var minVertex = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxVertex = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            
            // Loop for each mesh loaded in scene
            foreach (var mesh in scene.Meshes)
            {
                var minVertexMesh = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var maxVertexMesh = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                var newMesh = new IOMesh();

                if (!textures.ContainsKey(mesh.MaterialIndex)) continue;
                var faceTexture = textures[mesh.MaterialIndex];
                var hasTexCoords = mesh.HasTextureCoords(0);
                var hasColors = mesh.HasVertexColors(0);

                newMesh.Texture = faceTexture.Name;

                // Source data
                var positions = mesh.Vertices;
                var texCoords = mesh.TextureCoordinateChannels[0];
                var colors = mesh.VertexColorChannels[0]; 

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    // Create position
                    var position = new Vector3(positions[i].X, positions[i].Y, positions[i].Z) * _settings.Scale;
                    position = ApplyAxesTransforms(position);
                    newMesh.Positions.Add(position);

                    // Create UV
                    if (hasTexCoords)
                    {
                        var currentUV = new Vector2(texCoords[i].X, texCoords[i].Y);
                        currentUV = ApplyUVTransform(currentUV, faceTexture.Width, faceTexture.Height);
                        newMesh.UV.Add(currentUV);
                    }

                    // Create colors
                    if (hasColors)
                    {
                        var color = new Vector4(colors[i].R, colors[i].G, colors[i].B, colors[i].A);
                        newMesh.Colors.Add(color);
                    }

                    // Track min & max vertex for bounding box
                    if (position.X <= minVertexMesh.X && position.Y <= minVertexMesh.Y && position.Z <= minVertexMesh.Z)
                        minVertexMesh = position;

                    if (position.X >= maxVertexMesh.X && position.Y >= maxVertexMesh.Y && position.Z >= maxVertexMesh.Z)
                        maxVertexMesh = position;

                    if (position.X <= minVertex.X && position.Y <= minVertex.Y && position.Z <= minVertex.Z)
                        minVertex = position;

                    if (position.X >= maxVertex.X && position.Y >= maxVertex.Y && position.Z >= maxVertex.Z)
                        maxVertex = position;
                }

                // Add polygons
                foreach (var face in mesh.Faces)
                {
                    if (face.IndexCount == 3)
                    {
                        var poly = new IOPolygon(IOPolygonShape.Triangle);

                        poly.Indices.Add(lastBaseVertex + face.Indices[0]);
                        poly.Indices.Add(lastBaseVertex + face.Indices[1]);
                        poly.Indices.Add(lastBaseVertex + face.Indices[2]);

                        if (hasTexCoords)
                        {
                            poly.UV.Add(newMesh.UV[face.Indices[0]]);
                            poly.UV.Add(newMesh.UV[face.Indices[1]]);
                            poly.UV.Add(newMesh.UV[face.Indices[2]]);
                        }

                        if (hasColors)
                        {
                            poly.Colors.Add(newMesh.Colors[face.Indices[0]]);
                            poly.Colors.Add(newMesh.Colors[face.Indices[1]]);
                            poly.Colors.Add(newMesh.Colors[face.Indices[2]]);
                        }

                        newMesh.Polygons.Add(poly);
                    }
                    else
                    {
                        var poly = new IOPolygon(IOPolygonShape.Quad);

                        poly.Indices.Add(lastBaseVertex + face.Indices[0]);
                        poly.Indices.Add(lastBaseVertex + face.Indices[1]);
                        poly.Indices.Add(lastBaseVertex + face.Indices[2]);
                        poly.Indices.Add(lastBaseVertex + face.Indices[3]);

                        poly.UV.Add(newMesh.UV[face.Indices[0]]);
                        poly.UV.Add(newMesh.UV[face.Indices[1]]);
                        poly.UV.Add(newMesh.UV[face.Indices[2]]);
                        poly.UV.Add(newMesh.UV[face.Indices[3]]);

                        poly.Colors.Add(newMesh.Colors[face.Indices[0]]);
                        poly.Colors.Add(newMesh.Colors[face.Indices[1]]);
                        poly.Colors.Add(newMesh.Colors[face.Indices[2]]);
                        poly.Colors.Add(newMesh.Colors[face.Indices[3]]);

                        newMesh.Polygons.Add(poly);
                    }
                }

                // Set the bounding box
                newMesh.BoundingBox = new BoundingBox(minVertex, maxVertex);

                // Calculate bounding sphere
                var centreMesh = (minVertexMesh + maxVertexMesh) / 2.0f;
                var radiusMesh = (maxVertexMesh - centreMesh).Length();
                newMesh.BoundingSphere = new BoundingSphere(centreMesh, radiusMesh);

                newModel.Meshes.Add(newMesh);
            }

            // Set the model's bounding box
            newModel.BoundingBox = new BoundingBox(minVertex, maxVertex);

            // Calculate model's bounding sphere
            var centre = (minVertex + maxVertex) / 2.0f;
            var radius = (maxVertex - centre).Length();
            newModel.BoundingSphere = new BoundingSphere(centre, radius);

            return newModel;
        }
    }
}
