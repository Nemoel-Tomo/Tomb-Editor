﻿using DarkUI.Forms;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TombLib.Utils;
using System.ComponentModel;
using TombLib.LevelData;

namespace TombEditor.Forms
{
    public partial class FormRoomProperties : DarkForm
    {
        private class RoomPropertyRow
        {
            public bool Replace { get; set; }
            public string DisplayName { get; set; }
            public string Name { get; set; }
        }

        private readonly List<RoomPropertyRow> _rows;
        private readonly Editor _editor;

        public FormRoomProperties(Editor editor)
        {
            InitializeComponent();
            _editor = editor;

            // Set window property handlers
            Configuration.LoadWindowProperties(this, _editor.Configuration);
            FormClosing += new FormClosingEventHandler((s, e) => Configuration.SaveWindowProperties(this, _editor.Configuration));

            // Populate property list
            _rows = new List<RoomPropertyRow>();
            var names = TypeDescriptor.GetProperties(typeof(RoomProperties))
                                      .Cast<PropertyDescriptor>()
                                      .Select(p => new KeyValuePair<string, string>(p.Name, p.DisplayName));
            foreach (var n in names)
                if (!_rows.Any(r => r.DisplayName == n.Value))
                     _rows.Add(new RoomPropertyRow() { Replace = false, Name = n.Key, DisplayName = n.Value });

            dgvPropertyList.DataSource = new BindingList<RoomPropertyRow>(_rows);
        }

        private void butOk_Click(object sender, EventArgs e)
        {
            if (_rows.All(r => !r.Replace) || _editor.SelectedRooms.Count <= 1)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            var undoList = new List<UndoRedoInstance>();
            var propInfo = typeof(RoomProperties).GetProperties();

            var curr = _editor.SelectedRoom;
            foreach (var r in _editor.SelectedRooms.Skip(1))
            {
                // Add this room to undo list
                undoList.Add(new RoomPropertyUndoInstance(_editor.UndoManager, r));

                // Clone current property so we don't reference same reference-type objects (e.g. room tags)
                var newProp = curr.Properties.Clone();

                // Scan through all collected properties and copy selected ones based on "DisplayName" attribute
                foreach (var row in _rows)
                {
                    if (!row.Replace) continue; // Don't copy properties not chosen to be replaced
                    foreach (var prop in propInfo)
                    {
                        var attribValue = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault();
                        if (attribValue is DisplayNameAttribute)
                        {
                            var displayName = (attribValue as DisplayNameAttribute).DisplayName;
                            if (row.DisplayName == displayName)
                            {
                                prop.SetValue(r.Properties, prop.GetValue(newProp));

                                // HACK: We need to rebuild lighting for rooms with changed AmbientLight property.
                                if (prop.Name == "AmbientLight")
                                    r.RebuildLighting(_editor.Configuration.Rendering3D_HighQualityLightPreview);
                            }
                        }
                    }
                }
                _editor.RoomPropertiesChange(r);
			}
			
            _editor.UndoManager.Push(undoList);
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
