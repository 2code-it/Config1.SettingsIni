using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config1.SettingsIni
{
	internal class FileSystem : IFileSystem
	{
		public string PathGetFullPath(string path)
			=> Path.GetFullPath(path, AppDomain.CurrentDomain.BaseDirectory);

		public string[] FileReadAllLines(string filePath)
			=> File.ReadAllLines(filePath);
	}
}
