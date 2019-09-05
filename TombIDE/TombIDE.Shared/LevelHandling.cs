﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using TombLib.LevelData;
using TombLib.LevelData.IO;
using TombLib.Projects;

namespace TombIDE.Shared
{
	public class LevelHandling
	{
		public static List<string> GenerateSectionMessages(ProjectLevel level, int ambientSoundID, bool horizon)
		{
			return new List<string>
			{
				"\n[Level]",
				"Name= " + level.Name,
				"Level= DATA\\" + level.Name.ToUpper().Replace(' ', '_') + ", " + ambientSoundID,
				"LoadCamera= 0, 0, 0, 0, 0, 0, 0",
				"Horizon= " + (horizon? "ENABLED" : "DISABLED")
			};
		}

		public static void UpdatePrj2GameSettings(string prj2FilePath, ProjectLevel destLevel, Project destProject)
		{
			Level level = Prj2Loader.LoadFromPrj2(prj2FilePath, null);

			string exeFilePath = Path.Combine(destProject.EnginePath, destProject.GetExeFileName());

			string dataFileName = destLevel.Name.Replace(' ', '_') + destProject.GetLevelFileExtension();
			string dataFilePath = Path.Combine(destProject.EnginePath, "data", dataFileName);

			string projectSamplesPath = Path.Combine(destProject.ProjectPath, "Sounds");

			level.Settings.LevelFilePath = prj2FilePath;

			level.Settings.GameDirectory = level.Settings.MakeRelative(destProject.EnginePath, VariableType.LevelDirectory);
			level.Settings.GameExecutableFilePath = level.Settings.MakeRelative(exeFilePath, VariableType.LevelDirectory);
			level.Settings.GameLevelFilePath = level.Settings.MakeRelative(dataFilePath, VariableType.LevelDirectory);
			level.Settings.GameVersion = destProject.GameVersion;

			level.Settings.SoundsCatalogs.Add(new ReferencedSoundsCatalog(level.Settings, projectSamplesPath));

			Prj2Writer.SaveToPrj2(prj2FilePath, level);
		}

		public static string RemoveIllegalNameSymbols(string levelName)
		{
			char[] illegalNameChars = { ';', '[', ']', '=', ',', '.', '!' };
			return illegalNameChars.Aggregate(levelName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}
	}
}
