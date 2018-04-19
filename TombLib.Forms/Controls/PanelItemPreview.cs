﻿using DarkUI.Controls;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using TombLib.Graphics;
using TombLib.Wad;

namespace TombLib.Controls
{
    public abstract class PanelItemPreview : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IWadObject CurrentObject
        {
            get { return _currentObject; }
            set
            {
                if (_currentObject == value)
                    return;
                _currentObject = value;
                _fixedSoundInfoEditor.Visible = value is WadFixedSoundInfo;
                if (value is WadFixedSoundInfo)
                    _fixedSoundInfoEditor.SoundInfo = ((WadFixedSoundInfo)value).SoundInfo;
                Invalidate();
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ArcBallCamera Camera { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AnimationIndex { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int KeyFrameIndex { get; set; }

        private DarkScrollBar _animationScrollBar;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DarkScrollBar AnimationScrollBar
        {
            get { return _animationScrollBar; }
            set
            {
                if (_animationScrollBar != null)
                    _animationScrollBar.ValueChanged -= AnimationScrollBar_Value_ValueChanged;
                _animationScrollBar = value;
                value.ValueChanged += AnimationScrollBar_Value_ValueChanged;
            }
        }

        private GraphicsDevice _device;
        private DeviceManager _deviceManager;
        private SwapChainGraphicsPresenter _presenter;
        private RasterizerState _rasterizerWireframe;
        private float _lastX;
        private float _lastY;
        private SpriteBatch _spriteBatch;
        private Texture2D _spriteTexture;
        private WadTexture _spriteTextureData;
        private WadRenderer _wadRenderer;
        private SoundInfoEditor _fixedSoundInfoEditor;
        private IWadObject _currentObject = null;

        public PanelItemPreview()
        {
            // Init fixed sound info editor
            _fixedSoundInfoEditor = new SoundInfoEditor();
            _fixedSoundInfoEditor.Name = "_fixedSoundInfoEditor";
            _fixedSoundInfoEditor.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            Rectangle clientRectangle = ClientRectangle;
            clientRectangle.Inflate(new Size(-5, -5));
            _fixedSoundInfoEditor.Bounds = clientRectangle;
            _fixedSoundInfoEditor.Visible = false;
            _fixedSoundInfoEditor.ReadOnly = ReadOnly;
            _fixedSoundInfoEditor.SoundInfoChanged += delegate
            {
                if (ReadOnly)
                    return;
                if (CurrentObject is WadFixedSoundInfo)
                {
                    WadFixedSoundInfo fixedSoundInfo = (WadFixedSoundInfo)CurrentObject;
                    fixedSoundInfo.SoundInfo = _fixedSoundInfoEditor.SoundInfo;
                }
                ObjectWasModified(this, EventArgs.Empty);
            };
            Controls.Add(_fixedSoundInfoEditor);
        }


        public void InitializePanel(DeviceManager deviceManager)
        {
            // Reset scrollbar
            _device = deviceManager.Device;
            _deviceManager = deviceManager;
            _wadRenderer = new WadRenderer(deviceManager.Device, true);

            // Initialize the viewport, after the panel is added and sized on the form
            var pp = new PresentationParameters
            {
                BackBufferFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                BackBufferWidth = Width,
                BackBufferHeight = Height,
                DepthStencilFormat = DepthFormat.Depth24Stencil8,
                DeviceWindowHandle = this,
                IsFullScreen = false,
                MultiSampleCount = MSAALevel.None,
                PresentationInterval = PresentInterval.Immediate,
                RenderTargetUsage = SharpDX.DXGI.Usage.RenderTargetOutput | SharpDX.DXGI.Usage.BackBuffer,
                Flags = SharpDX.DXGI.SwapChainFlags.None
            };

            _presenter = new SwapChainGraphicsPresenter(_device, pp);
            ResetCamera();

            // Initialize the rasterizer state for wireframe drawing
            SharpDX.Direct3D11.RasterizerStateDescription renderStateDesc =
                new SharpDX.Direct3D11.RasterizerStateDescription
                {
                    CullMode = SharpDX.Direct3D11.CullMode.None,
                    DepthBias = 0,
                    DepthBiasClamp = 0,
                    FillMode = SharpDX.Direct3D11.FillMode.Wireframe,
                    IsAntialiasedLineEnabled = true,
                    IsDepthClipEnabled = true,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = true,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0
                };

            _rasterizerWireframe = RasterizerState.New(_device, renderStateDesc);

            _spriteBatch = new SpriteBatch(_device);
        }

        private void _fixedSoundInfoEditor_SoundInfoChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ResetCamera()
        {
            Camera = new ArcBallCamera(new Vector3(0.0f, 256.0f, 0.0f), 0, 0, -(float)Math.PI / 2, (float)Math.PI / 2, 2048.0f, 0, 1000000, FieldOfView * (float)(Math.PI / 180));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wadRenderer?.Dispose();
                _spriteBatch?.Dispose();
                _spriteTexture?.Dispose();
                _presenter?.Dispose();
                _rasterizerWireframe?.Dispose();
                _fixedSoundInfoEditor?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_device == null || _presenter == null)
                e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
            if (_fixedSoundInfoEditor.Visible)
                using (var brush = new SolidBrush(_fixedSoundInfoEditor.BackColor))
                    e.Graphics.FillRectangle(brush, ClientRectangle);

            // Don't paint the background
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw();
        }

        public void Draw()
        {
            if (_fixedSoundInfoEditor.Visible || _device == null || _presenter == null)
                return;

            _device.Presenter = _presenter;
            _device.SetViewports(new SharpDX.ViewportF(0, 0, ClientSize.Width, ClientSize.Height));
            _device.SetRenderTargets(_device.Presenter.DepthStencilBuffer, _device.Presenter.BackBuffer);
            _device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target,
                         BackgroundColor.ToSharpDX(),
                         1.0f,
                         0);
            _device.SetDepthStencilState(_device.DepthStencilStates.Default);
            _device.SetBlendState(_device.BlendStates.Opaque);
            _device.SetRasterizerState(_device.RasterizerStates.CullBack);

            Matrix4x4 viewProjection = Camera.GetViewProjectionMatrix(Width, Height);
            if (CurrentObject is WadMoveable)
            {
                AnimatedModel model = _wadRenderer.GetMoveable((WadMoveable)CurrentObject);
                // We don't need to rebuilt it everytime necessarily, but it's cheap to so and
                // simpler than trying to figure out when it may be necessary.
                model.UpdateAnimation(AnimationIndex, KeyFrameIndex);

                var effect = _deviceManager.Effects["Model"];

                effect.Parameters["Color"].SetValue(Vector4.One);
                effect.Parameters["Texture"].SetResource(_wadRenderer.Texture);
                effect.Parameters["TextureSampler"].SetResource(_device.SamplerStates.Default);

                // Build animation transforms
                var matrices = new List<Matrix4x4>();
                if (model.Animations.Count != 0)
                {
                    for (var b = 0; b < model.Meshes.Count; b++)
                        matrices.Add(model.AnimationTransforms[b]);
                }
                else
                {
                    foreach (var bone in model.Bones)
                        matrices.Add(bone.GlobalTransform);
                }

                for (int i = 0; i < model.Meshes.Count; i++)
                {
                    var mesh = model.Meshes[i];
                    if (mesh.Vertices.Count == 0)
                        continue;

                    _device.SetVertexBuffer(0, mesh.VertexBuffer);
                    _device.SetIndexBuffer(mesh.IndexBuffer, true);
                    _device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, mesh.VertexBuffer));

                    effect.Parameters["ModelViewProjection"].SetValue((matrices[i] * viewProjection).ToSharpDX());

                    effect.Techniques[0].Passes[0].Apply();

                    foreach (var submesh in mesh.Submeshes)
                        _device.DrawIndexed(PrimitiveType.TriangleList, submesh.Value.NumIndices, submesh.Value.MeshBaseIndex);
                }
            }
            else if (CurrentObject is WadStatic)
            {
                StaticModel model = _wadRenderer.GetStatic((WadStatic)CurrentObject);

                var effect = _deviceManager.Effects["StaticModel"];

                effect.Parameters["ModelViewProjection"].SetValue(viewProjection.ToSharpDX());
                effect.Parameters["Color"].SetValue(Vector4.One);
                effect.Parameters["Texture"].SetResource(_wadRenderer.Texture);
                effect.Parameters["TextureSampler"].SetResource(_device.SamplerStates.Default);

                for (int i = 0; i < model.Meshes.Count; i++)
                {
                    var mesh = model.Meshes[i];

                    _device.SetVertexBuffer(0, mesh.VertexBuffer);
                    _device.SetIndexBuffer(mesh.IndexBuffer, true);
                    _device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, mesh.VertexBuffer));

                    effect.Parameters["ModelViewProjection"].SetValue(viewProjection.ToSharpDX());
                    effect.Techniques[0].Passes[0].Apply();

                    foreach (var submesh in mesh.Submeshes)
                        _device.DrawIndexed(PrimitiveType.TriangleList, submesh.Value.NumIndices, submesh.Value.MeshBaseIndex);
                }
            }
            else if (CurrentObject is WadSpriteSequence)
            {
                WadSpriteSequence spriteSequence = (WadSpriteSequence)CurrentObject;
                int spriteIndex = Math.Min(spriteSequence.Sprites.Count - 1, KeyFrameIndex);
                if (spriteIndex < spriteSequence.Sprites.Count)
                {
                    WadSprite sprite = spriteSequence.Sprites[spriteIndex];

                    // Load texture
                    if (_spriteTextureData != sprite.Texture)
                    {
                        _spriteTexture?.Dispose();
                        _spriteTexture = TextureLoad.Load(_device, sprite.Texture.Image);
                        _spriteTextureData = sprite.Texture;
                    }

                    // Draw
                    int x = (ClientSize.Width - _spriteTextureData.Image.Width) / 2;
                    int y = (ClientSize.Height - _spriteTextureData.Image.Height) / 2;
                    _spriteBatch.Begin(SpriteSortMode.Immediate, _device.BlendStates.AlphaBlend);
                    _spriteBatch.Draw(_spriteTexture, new SharpDX.DrawingRectangle(x, y, _spriteTextureData.Image.Width, _spriteTextureData.Image.Height), SharpDX.Color.White);
                    _spriteBatch.End();
                }
            }

            _device.Present();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_presenter != null)
            {
                _presenter.Resize(ClientSize.Width, ClientSize.Height, SharpDX.DXGI.Format.B8G8R8A8_UNorm);
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Make this control able to receive scroll and key board events...
            base.OnMouseEnter(e);
            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            Camera.Zoom(-e.Delta * NavigationSpeedMouseWheelZoom);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _lastX = e.X;
            _lastY = e.Y;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Right)
            {
                // Use height for X coordinate because the camera FOV per pixel is defined by the height.
                float deltaX = (e.X - _lastX) / Height;
                float deltaY = (e.Y - _lastY) / Height;

                _lastX = e.X;
                _lastY = e.Y;

                if ((ModifierKeys & Keys.Control) == Keys.Control)
                    Camera.Zoom(-deltaY * NavigationSpeedMouseZoom);
                else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                    Camera.MoveCameraPlane(new Vector3(-deltaX, -deltaY, 0) * NavigationSpeedMouseTranslate);
                else
                    Camera.Rotate(deltaX * NavigationSpeedMouseRotate,
                                  -deltaY * NavigationSpeedMouseRotate);
                Invalidate();
            }
        }

        private void AnimationScrollBar_Value_ValueChanged(object sender, ScrollValueEventArgs e)
        {
            KeyFrameIndex = AnimationScrollBar.Value;
            Invalidate();
        }

        public void UpdateAnimationScrollbar()
        {
            // Figure out scroll bar maximum
            int stateCount = -1;
            if (CurrentObject is WadMoveable)
            {
                if (AnimationIndex < ((WadMoveable)CurrentObject).Animations.Count)
                    stateCount = ((WadMoveable)CurrentObject).Animations[AnimationIndex].KeyFrames.Count;
            }
            else if (CurrentObject is WadSpriteSequence)
            {
                stateCount = ((WadSpriteSequence)CurrentObject).Sprites.Count;
            }

            // Setup scroll bar
            KeyFrameIndex = Math.Max(KeyFrameIndex, stateCount - 1);
            AnimationScrollBar.ViewSize = 1;
            AnimationScrollBar.Enabled = stateCount > 1;
            AnimationScrollBar.Minimum = 0;
            AnimationScrollBar.Maximum = Math.Max(1, stateCount);
            AnimationScrollBar.Value = KeyFrameIndex;
            AnimationScrollBar.Invalidate();
        }

        public abstract Vector4 BackgroundColor { get; }
        public abstract float FieldOfView { get; }
        public abstract float NavigationSpeedMouseWheelZoom { get; }
        public abstract float NavigationSpeedMouseZoom { get; }
        public abstract float NavigationSpeedMouseTranslate { get; }
        public abstract float NavigationSpeedMouseRotate { get; }
        public abstract bool ReadOnly { get; }
        public event EventHandler ObjectWasModified;
    }
}