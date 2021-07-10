﻿using System;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Forms;
using TombLib.LevelData;

namespace TombEditor.Forms
{
    public partial class FormCamera : DarkForm
    {
        private readonly CameraInstance _instance;
        private readonly Editor _editor = Editor.Instance;

        public FormCamera(CameraInstance instance)
        {
            InitializeComponent();

            _instance = instance;
            ckFixed.Checked = _instance.Fixed;
            nudMoveTimer.Value = _instance.MoveTimer;

            ckFixed.Enabled      = (instance.Room.Level.Settings.GameVersion >= TRVersion.Game.TR4);
            nudMoveTimer.Enabled = (instance.Room.Level.Settings.GameVersion <= TRVersion.Game.TR2);

            if (_editor.Level.Settings.GameVersion == TRVersion.Game.TombEngine)
            {
                tbLuaId.Text = _instance.LuaScriptId;
            }
        }

        private void butOk_Click(object sender, EventArgs e)
        {
            if (_editor.Level.Settings.GameVersion == TRVersion.Game.TombEngine)
            {
                foreach (var room in _editor.Level.Rooms.Where(r => r != null))
                    foreach (var instance in room.Objects)
                        if (instance is CameraInstance)
                        {
                            var cameraInstance = instance as CameraInstance;
                            if (cameraInstance != _instance && cameraInstance.LuaScriptId == tbLuaId.Text)
                            {
                                DarkMessageBox.Show(this, "The value of LUA Script ID is already taken by another camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
            }

            _instance.Fixed = ckFixed.Checked;
            _instance.MoveTimer = (byte)nudMoveTimer.Value;

            if (_editor.Level.Settings.GameVersion == TRVersion.Game.TombEngine)
            {
                _instance.LuaScriptId = tbLuaId.Text;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
