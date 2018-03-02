﻿namespace TombEditor
{
    partial class FormSound
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new DarkUI.Controls.DarkLabel();
            this.tbSound = new DarkUI.Controls.DarkTextBox();
            this.cbBit5 = new DarkUI.Controls.DarkCheckBox();
            this.cbBit1 = new DarkUI.Controls.DarkCheckBox();
            this.cbBit2 = new DarkUI.Controls.DarkCheckBox();
            this.cbBit3 = new DarkUI.Controls.DarkCheckBox();
            this.cbBit4 = new DarkUI.Controls.DarkCheckBox();
            this.lstSounds = new DarkUI.Controls.DarkListView();
            this.butPlaySound = new DarkUI.Controls.DarkButton();
            this.butOK = new DarkUI.Controls.DarkButton();
            this.butCancel = new DarkUI.Controls.DarkButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label1.Location = new System.Drawing.Point(12, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Sound:";
            // 
            // tbSound
            // 
            this.tbSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSound.Location = new System.Drawing.Point(59, 412);
            this.tbSound.Name = "tbSound";
            this.tbSound.Size = new System.Drawing.Size(449, 22);
            this.tbSound.TabIndex = 55;
            this.tbSound.TextChanged += new System.EventHandler(this.tbSound_TextChanged);
            // 
            // cbBit5
            // 
            this.cbBit5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBit5.AutoSize = true;
            this.cbBit5.Location = new System.Drawing.Point(271, 448);
            this.cbBit5.Name = "cbBit5";
            this.cbBit5.Size = new System.Drawing.Size(49, 17);
            this.cbBit5.TabIndex = 56;
            this.cbBit5.Text = "Bit 5";
            // 
            // cbBit1
            // 
            this.cbBit1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBit1.AutoSize = true;
            this.cbBit1.Location = new System.Drawing.Point(59, 448);
            this.cbBit1.Name = "cbBit1";
            this.cbBit1.Size = new System.Drawing.Size(49, 17);
            this.cbBit1.TabIndex = 57;
            this.cbBit1.Text = "Bit 1";
            // 
            // cbBit2
            // 
            this.cbBit2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBit2.AutoSize = true;
            this.cbBit2.Location = new System.Drawing.Point(112, 448);
            this.cbBit2.Name = "cbBit2";
            this.cbBit2.Size = new System.Drawing.Size(49, 17);
            this.cbBit2.TabIndex = 58;
            this.cbBit2.Text = "Bit 2";
            // 
            // cbBit3
            // 
            this.cbBit3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBit3.AutoSize = true;
            this.cbBit3.Location = new System.Drawing.Point(165, 448);
            this.cbBit3.Name = "cbBit3";
            this.cbBit3.Size = new System.Drawing.Size(49, 17);
            this.cbBit3.TabIndex = 59;
            this.cbBit3.Text = "Bit 3";
            // 
            // cbBit4
            // 
            this.cbBit4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbBit4.AutoSize = true;
            this.cbBit4.Location = new System.Drawing.Point(218, 448);
            this.cbBit4.Name = "cbBit4";
            this.cbBit4.Size = new System.Drawing.Size(49, 17);
            this.cbBit4.TabIndex = 60;
            this.cbBit4.Text = "Bit 4";
            // 
            // lstSounds
            // 
            this.lstSounds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSounds.Location = new System.Drawing.Point(13, 13);
            this.lstSounds.Name = "lstSounds";
            this.lstSounds.Size = new System.Drawing.Size(593, 386);
            this.lstSounds.TabIndex = 61;
            this.lstSounds.Text = "darkListView1";
            this.lstSounds.SelectedIndicesChanged += new System.EventHandler(this.lstSounds_SelectedIndicesChanged);
            // 
            // butPlaySound
            // 
            this.butPlaySound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butPlaySound.Image = global::TombEditor.Properties.Resources.actions_play_16;
            this.butPlaySound.Location = new System.Drawing.Point(514, 410);
            this.butPlaySound.Name = "butPlaySound";
            this.butPlaySound.Size = new System.Drawing.Size(92, 23);
            this.butPlaySound.TabIndex = 62;
            this.butPlaySound.Text = "Play sound";
            this.butPlaySound.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butPlaySound.Click += new System.EventHandler(this.butPlay_Click);
            // 
            // butOK
            // 
            this.butOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.butOK.Location = new System.Drawing.Point(174, 495);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(130, 24);
            this.butOK.TabIndex = 63;
            this.butOK.Text = "Ok";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(310, 495);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(130, 24);
            this.butCancel.TabIndex = 64;
            this.butCancel.Text = "Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // FormSound
            // 
            this.AcceptButton = this.butOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(618, 531);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butPlaySound);
            this.Controls.Add(this.lstSounds);
            this.Controls.Add(this.cbBit4);
            this.Controls.Add(this.cbBit3);
            this.Controls.Add(this.cbBit2);
            this.Controls.Add(this.cbBit1);
            this.Controls.Add(this.cbBit5);
            this.Controls.Add(this.tbSound);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FormSound";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sound source";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkUI.Controls.DarkLabel label1;
        private DarkUI.Controls.DarkTextBox tbSound;
        private DarkUI.Controls.DarkCheckBox cbBit5;
        private DarkUI.Controls.DarkCheckBox cbBit1;
        private DarkUI.Controls.DarkCheckBox cbBit2;
        private DarkUI.Controls.DarkCheckBox cbBit3;
        private DarkUI.Controls.DarkCheckBox cbBit4;
        private DarkUI.Controls.DarkListView lstSounds;
        private DarkUI.Controls.DarkButton butPlaySound;
        private DarkUI.Controls.DarkButton butOK;
        private DarkUI.Controls.DarkButton butCancel;
    }
}