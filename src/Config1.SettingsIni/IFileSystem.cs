namespace Config1.SettingsIni
{
	internal interface IFileSystem
	{
		string PathGetFullPath(string path);
		string[] FileReadAllLines(string filePath);
	}
}