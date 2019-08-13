using DarkUI.Forms;
using System;
using System.IO;
using System.Windows.Forms;
using TombIDE.Shared;

namespace TombIDE.ProjectMaster
{
	public partial class SettingsSpecialFunctions : UserControl
	{
		private IDE _ide;

		public SettingsSpecialFunctions()
		{
			InitializeComponent();
		}

		public void Initialize(IDE ide)
		{
			_ide = ide;
		}

		private void button_DeleteLogs_Click(object sender, EventArgs e)
		{
			try
			{
				string[] files = Directory.GetFiles(_ide.Project.ProjectPath);

				bool wereFilesDeleted = false;

				foreach (string file in files)
				{
					string fileName = Path.GetFileName(file);

					if (fileName == "db_patches_crash.bin" ||
						fileName == "DETECTED CRASH.txt" ||
						fileName == "LastExtraction.lst" ||
						(fileName.StartsWith("Last_Crash_") && (fileName.EndsWith(".txt") || fileName.EndsWith(".mem"))) ||
						fileName.EndsWith("_warm_up_log.txt"))
					{
						File.Delete(file);
						wereFilesDeleted = true;
					}
				}

				if (wereFilesDeleted)
					DarkMessageBox.Show(this, "Successfully deleted all log files\n" +
					"and error dumps from the project folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					DarkMessageBox.Show(this, "No log files or error dumps were found.", "Information",
						MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				DarkMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
