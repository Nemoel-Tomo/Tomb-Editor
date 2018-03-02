﻿using SharpDX.Direct3D;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombLib.Graphics
{
    public class DeviceManager
    {
        // to be removed
        public static DeviceManager DefaultDeviceManager = new DeviceManager();

        public GraphicsDevice Device { get; set; }
        public Dictionary<string, Texture2D> Textures { get; } = new Dictionary<string, Texture2D>();
        public Dictionary<string, Effect> Effects { get; } = new Dictionary<string, Effect>();
        public SpriteFont Font { get; set; }

        public DeviceManager()
        {
            Device = GraphicsDevice.New(DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.None, FeatureLevel.Level_10_0);

            string resourcePath = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);

            // Load effects
            IEnumerable<string> effectFiles = Directory.EnumerateFiles(resourcePath + "\\Editor\\Shaders", "*.fx");
            foreach (string fileName in effectFiles)
            {
                string effectName = Path.GetFileNameWithoutExtension(fileName);
                Effects.Add(effectName, LoadEffect(fileName));
            }

            // Load images
            IEnumerable<string> textureFiles = Directory.EnumerateFiles(resourcePath + "\\Editor\\Textures", "*.png");
            foreach (string fileName in textureFiles)
            {
                string textureName = Path.GetFileNameWithoutExtension(fileName);
                Textures.Add(textureName, TextureLoad.Load(Device, fileName));
            }

            // Load default font
            SpriteFontData fontData = SpriteFontData.Load(ResourcesC.ResourcesC.font);
            fontData.DefaultCharacter = '\n'; // Don't crash on uncommon Unicode values
            Font = SpriteFont.New(Device, fontData);
        }

        private Effect LoadEffect(string fileName)
        {
            EffectCompilerResult result = EffectCompiler.CompileFromFile(fileName);

            if (result.HasErrors)
            {
                string errors = "";
                foreach (var err in result.Logger.Messages)
                    errors += err + Environment.NewLine;
                throw new Exception("Could not compile effect '" + fileName + "'" + Environment.NewLine + errors);
            }

            return new Effect(Device, result.EffectData);
        }
    }
}
