using System;
using System.IO;
using System.Text.RegularExpressions;
using BDMVision.Model.Log;
using WaftechLibraries.Log;

namespace BDMVision.Model.Job
{
	public class DeleteObsoleteFiles
	{
		/// <summary>
		/// Deletes all obsolete subdirectory and its files and folders 
		/// </summary>
		/// <param name="mainImageDirectory"></param>
		public void DeleteObsoleteImages(
			string mainImageDirectory,
			int NoOfDaysToKeepDirectory)
		{
			// Check if directory exists
			if (!Directory.Exists(mainImageDirectory)) return;

			DateTime dt_now = DateTime.Now;

			// 1. Get all subdirectories inside the main directory
			string[] subDirectories = Directory.GetDirectories(mainImageDirectory);

			// 2. filter subdirectories to be deleted
			Regex r = new Regex(@"\d{4}-\d{2}-\d{2}");

			foreach (string s in subDirectories)
			{
				Match m = r.Match(s);

				// if the directory Matched
				if (m.Success)
				{
					string matchedString = m.Captures[0].Value;
					VisionLogger.Log(LogType.Log, GetType().ToString(), "Matched Directory is " + matchedString);
					DateTime dt = DateTime.Parse(matchedString);
					TimeSpan diff = dt_now - dt;

					// if Obsolete
					if (diff.Days > NoOfDaysToKeepDirectory)
					{
						var dir = new DirectoryInfo(s);
						dir.Delete(true);
						VisionLogger.Log(
							LogType.Log,
							GetType().ToString(),
							string.Format("Directory Deleted: {0}", s));						
					}
				}
			}
		}

		public void DeleteObsoleteLogFiles(
			string mainLogDirectory,
			int NoofDaysToKeepFiles)
		{
			// Check if directory exists
			if (!Directory.Exists(mainLogDirectory)) return;

			DateTime dt_now = DateTime.Now;

			// 1. Get all files inside the main directory
			string[] logfiles = Directory.GetFiles(mainLogDirectory);
			string matchedPattern = @"Log\d{8}";

			// 2. filter files to be deleted
			Regex r = new Regex(matchedPattern);

			foreach(string s in logfiles)
			{
				Match m = r.Match(s);

				// if file matched
				if (m.Success)
				{
					string matchedString = m.Captures[0].Value;
					VisionLogger.Log(LogType.Log, GetType().ToString(), "Matched File is " + matchedString);

					// Split string into timedate
					string[] results = Regex.Split(matchedString, @"(?<=\D)(?=\d)");

					// modify results into time date format
					string convertedstring = DateTime.ParseExact(results[1], "yyyyMMdd", null).ToString("yyyy-MM-dd");

					DateTime dt = DateTime.Parse(convertedstring);
					TimeSpan diff = dt_now - dt;

					// if Obsolete
					if (diff.Days > NoofDaysToKeepFiles)
					{
						File.Delete(s);
						VisionLogger.Log(
							LogType.Log,
							GetType().ToString(),
							string.Format("File Deleted: {0}", s));
					}
				}
			}

		}
	}

	public static class FileManager
	{
		public static void CreateDirectory(string directoryPath)
		{
			Directory.CreateDirectory(directoryPath);
		}
		public static void Delete(string directoryPath)
		{
			System.IO.DirectoryInfo di = new DirectoryInfo(directoryPath);
			foreach (FileInfo file in di.GetFiles())
			{
				VisionLogger.Log(LogType.Log, "FileManager", file.Name + " deleted");
				file.Delete();
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				VisionLogger.Log(LogType.Log, "FileManager", dir.Name + " deleted");
				dir.Delete(true);
			}
		}
	}
}
