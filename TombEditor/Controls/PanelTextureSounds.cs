﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TombEditor.Geometry;

namespace TombEditor.Controls
{
    public partial class PanelTextureSounds : PictureBox
    {
        private Editor _editor;
        public FormTextureSounds ContainerForm { get; set; }

        private Font _font = new Font("Segoe UI", 8, FontStyle.Bold);
        private Pen _penSelectedTexture = new Pen(Brushes.Red, 3);

        public short SelectedX { get; set; }
        public short SelectedY { get; set; }
        public short Page { get; set; }
        public bool IsTextureSelected { get; set; }


        public PanelTextureSounds()
        {}
       
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            SelectedX = (short)(Math.Floor(e.X / 64.0f) * 64.0f);
            SelectedY = (short)(Math.Floor(e.Y / 64.0f) * 64.0f);
            Page = (short)Math.Floor(SelectedY / 256.0f);
            SelectedY -= (short)(Page * 256);

            IsTextureSelected = true;

            ContainerForm.SelectTexture();

            Invalidate();
        }

        private int GetTextureSound(int x, int y, int page)
        {
            if (_editor == null || _editor.Level == null) return -1;

            for (int i = 0; i < _editor.Level.TextureSounds.Count; i++)
            {
                TextureSound txtSound = _editor.Level.TextureSounds[i];

                if (txtSound.X == x && txtSound.Y == y && txtSound.Page == page) return i;
            }

            return -1;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (_editor == null && Editor.Instance == null)
                return;
            else
                _editor = Editor.Instance;

            Graphics g = pe.Graphics;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < Height / 64; y++)
                {
                    int page = (int)Math.Floor(y / 4.0f);
                    int xc = x * 64;
                    int yc = (y - 4 * page) * 64;

                    int txtId = GetTextureSound(xc, yc, page);

                    string soundName = "";
                    Brush color = Brushes.Green;

                    if (txtId == -1)
                    {
                        soundName = "Stone";
                        color = Brushes.Brown;
                    }
                    else
                    {
                        TextureSounds sound = _editor.Level.TextureSounds[txtId].Sound;

                        switch (sound)
                        {
                            case TextureSounds.Concrete:
                                soundName = "Concrete";
                                color = Brushes.Brown;
                                break;

                            case TextureSounds.Grass:
                                color = Brushes.Green;
                                soundName = "Grass";
                                break;

                            case TextureSounds.Gravel:
                                color = Brushes.Gray;
                                soundName = "Gravel";
                                break;

                            case TextureSounds.Ice:
                                color = Brushes.Blue;
                                soundName = "Ice";
                                break;

                            case TextureSounds.Marble:
                                color = Brushes.Brown;
                                soundName = "Marble";
                                break;

                            case TextureSounds.Metal:
                                color = Brushes.Gray;
                                soundName = "Metal";
                                break;

                            case TextureSounds.Mud:
                                color = Brushes.Brown;
                                soundName = "Mud";
                                break;

                            case TextureSounds.OldMetal:
                                color = Brushes.LightCoral;
                                soundName = "OldMetal";
                                break;

                            case TextureSounds.OldWood:
                                color = Brushes.BurlyWood;
                                soundName = "OldWood";
                                break;

                            case TextureSounds.Sand:
                                color = Brushes.SandyBrown;
                                soundName = "Sand";
                                break;

                            case TextureSounds.Snow:
                                color = Brushes.Blue;
                                soundName = "Snow";
                                break;

                            case TextureSounds.Stone:
                                color = Brushes.Brown;
                                soundName = "Stone";
                                break;

                            case TextureSounds.Water:
                                color = Brushes.Blue;
                                soundName = "Water";
                                break;

                            case TextureSounds.Wood:
                                color = Brushes.BurlyWood;
                                soundName = "Wood";
                                break;
                        }
                    }

                    g.FillRectangle(color, new Rectangle(xc, page * 256 + yc + 48, 64, 16));
                    g.DrawString(soundName, _font, Brushes.White, new Point(xc + 4, page * 256 + yc + 49));
                }
            }

            g.DrawLine(Pens.White, new Point(64, 0), new Point(64, Height));
            g.DrawLine(Pens.White, new Point(128, 0), new Point(128, Height));
            g.DrawLine(Pens.White, new Point(192, 0), new Point(192, Height));

            for (int i = 1; i < Height / 64; i++)
            {
                g.DrawLine(Pens.White, new Point(0, 64 * i), new Point(255, 64 * i));
            }

            if (IsTextureSelected)
            {
                g.DrawRectangle(_penSelectedTexture, new Rectangle(SelectedX, Page * 256 + SelectedY, 64, 64));
            }
        }
    }
}
