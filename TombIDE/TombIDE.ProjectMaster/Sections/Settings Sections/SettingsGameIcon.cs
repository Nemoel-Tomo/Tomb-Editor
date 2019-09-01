﻿using DarkUI.Forms;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TombIDE.Shared;
using TombLib.LevelData;

namespace TombIDE.ProjectMaster
{
	public partial class SettingsGameIcon : UserControl
	{
		private IDE _ide;

		#region Initialization

		public SettingsGameIcon()
		{
			InitializeComponent();
		}

		public void Initialize(IDE ide)
		{
			_ide = ide;

			radioButton_Dark.Checked = !_ide.Configuration.LightModePreviewEnabled;
			radioButton_Light.Checked = _ide.Configuration.LightModePreviewEnabled;

			string tempExeFilePath = Path.Combine(Path.GetTempPath(), "tomb_temp.exe");

			using (File.Create(tempExeFilePath))
			{
				Bitmap defaultExeIcon = IconExtractor.GetIconFrom(tempExeFilePath, IconSize.Large, false).ToBitmap();
				Bitmap gameExeIcon = IconExtractor.GetIconFrom(Path.Combine(_ide.Project.ProjectPath, _ide.Project.GetExeFileName()), IconSize.Large, false).ToBitmap();

				byte[] defaultExeStream = ImageHandling.GetBitmapStream(defaultExeIcon);
				byte[] gameExeStream = ImageHandling.GetBitmapStream(gameExeIcon);

				if (gameExeStream.SequenceEqual(defaultExeStream) && File.Exists(Path.Combine(_ide.Project.ProjectPath, "launch.exe")))
					UpdateIcons();
				else
				{
					label_Unavailable.Visible = true;

					button_Change.Enabled = false;
					button_Reset.Enabled = false;
				}
			}

			File.Delete(tempExeFilePath);
		}

		#endregion Initialization

		#region Events

		private void radioButton_Dark_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton_Dark.Checked)
			{
				panel_PreviewBackground.BackColor = Color.FromArgb(48, 48, 48);

				_ide.Configuration.LightModePreviewEnabled = false;
				_ide.Configuration.Save();
			}
		}

		private void radioButton_Light_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton_Light.Checked)
			{
				panel_PreviewBackground.BackColor = Color.White;

				_ide.Configuration.LightModePreviewEnabled = true;
				_ide.Configuration.Save();
			}
		}

		private void button_Change_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Title = "Choose the .ico file you want to inject into your game's .exe file";
				dialog.Filter = "Icon Files|*.ico";

				if (dialog.ShowDialog(this) == DialogResult.OK)
					ApplyIconToExe(dialog.FileName);
			}
		}

		private void button_Reset_Click(object sender, EventArgs e)
		{
			DialogResult result = DarkMessageBox.Show(this, "Are you sure you want to restore the default icon?", "Are you sure?",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				string icoFilePath = string.Empty;

				if (_ide.Project.GameVersion == GameVersion.TR4 || _ide.Project.GameVersion == GameVersion.TRNG)
				{
					if (_ide.Project.GameVersion == GameVersion.TRNG && File.Exists(Path.Combine(_ide.Project.ProjectPath, "flep.exe")))
						icoFilePath = Path.Combine(SharedMethods.GetProgramDirectory(), @"Templates\TOMB4\Defaults", "FLEP.ico");
					else
						icoFilePath = Path.Combine(SharedMethods.GetProgramDirectory(), @"Templates\TOMB4\Defaults", _ide.Project.GameVersion + ".ico");
				}
				else if (_ide.Project.GameVersion == GameVersion.TR5Main)
					icoFilePath = Path.Combine(SharedMethods.GetProgramDirectory(), @"Templates\TOMB5\Defaults", _ide.Project.GameVersion + ".ico");

				ApplyIconToExe(icoFilePath);
			}
		}

		#endregion Events

		#region Methods

		private void ApplyIconToExe(string iconPath)
		{
			try
			{
				string launchFilePath = Path.Combine(_ide.Project.ProjectPath, "launch.exe");
				IconInjector.InjectIcon(launchFilePath, iconPath);

				UpdateIcons();
				UpdateWindowsIconCache();
			}
			catch (Exception ex)
			{
				DarkMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateIcons() // This method is trash I know, but I couldn't find a better one
		{
			string launchFilePath = Path.Combine(_ide.Project.ProjectPath, "launch.exe");

			// Generate a random string to create a temporary .exe file.
			// We will extract the icon from the .exe copy because Windows is caching icons which doesn't allow us to easily extract
			// icons larger than 32x32 px.
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string randomString = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

			// Create the temporary .exe file
			string tempFilePath = launchFilePath + "." + randomString + ".exe";
			File.Copy(launchFilePath, tempFilePath);

			Bitmap ico_256 = ImageHandling.CropBitmapWhitespace(IconExtractor.GetIconFrom(tempFilePath, IconSize.Jumbo, false).ToBitmap());

			// Windows doesn't seem to have a name for 128x128 px icons, therefore we must resize the Jumbo one
			Bitmap resized_256 = (Bitmap)ImageHandling.ResizeImage(ico_256, 128, 128);
			Bitmap ico_128 = ImageHandling.CropBitmapWhitespace(resized_256);

			Bitmap ico_48 = ImageHandling.CropBitmapWhitespace(IconExtractor.GetIconFrom(tempFilePath, IconSize.ExtraLarge, false).ToBitmap());
			Bitmap ico_16 = ImageHandling.CropBitmapWhitespace(IconExtractor.GetIconFrom(tempFilePath, IconSize.Small, false).ToBitmap());

			panel_256.BackgroundImage = ico_256;

			panel_128.BackgroundImage = (ico_256.Width > 128 && ico_256.Height > 128) ? ico_128 : ico_256;

			panel_48.BackgroundImage = ico_48;
			panel_16.BackgroundImage = ico_16;

			// Now delete the temporary .exe file
			File.Delete(tempFilePath);
		}

		private void UpdateWindowsIconCache()
		{
			ProcessStartInfo info = new ProcessStartInfo
			{
				FileName = @"C:\Windows\SysNative\ie4uinit.exe",
				Arguments = IsWindows10() ? "-show" : "-ClearIconCache"
			};

			Process.Start(info);
		}

		private bool IsWindows10()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			string productName = (string)key.GetValue("ProductName");
			return productName.StartsWith("Windows 10");
		}

		#endregion Methods
	}
}
