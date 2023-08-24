using System.Reflection;

namespace Config1.SettingsIni
{
	public class SettingsIni : ISettingsIni
	{
		public SettingsIni() : this(_defaultFilePath) { }
		public SettingsIni(string filePath) : this(filePath, new FileSystem()) { }
		internal SettingsIni(string filePath, IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
			_filePath = filePath;
			Reload();
		}

		private Dictionary<string, Dictionary<string, string?>> _settings = default!;
		private const string _defaultFilePath = "./settings.ini";
		private readonly IFileSystem _fileSystem;
		private string _filePath;


		public void Reload()
		{
			var settings = GetSettingsFromFile(_fileSystem.PathGetFullPath(_filePath));
			if (settings is null) throw new InvalidOperationException("Invalid settings file");
			_settings = settings;
		}

		public string[] GetSubjects()
		{
			return _settings.Keys.ToArray();
		}

		public string[] GetKeys(string subject)
		{
			if (!SubjectExists(subject)) throw new InvalidOperationException($"Subject '{subject}' does not exist");
			return _settings[subject].Keys.ToArray();
		}

		public bool SubjectExists(string subject)
		{
			return _settings.ContainsKey(subject);
		}

		public bool KeyExists(string subject, string key)
		{
			return SubjectExists(subject) ? _settings[subject].ContainsKey(key) : false;
		}

		public string GetValue(string subject, string key)
		{
			return KeyExists(subject, key) ? _settings[subject][key]! : string.Empty;
		}

		public string[] GetValues(string subject)
		{
			return GetKeys(subject).Select(x => GetValue(subject, x)).ToArray();
		}

		public T[] GetValuesAs<T>(string subject)
		{
			return GetKeys(subject).Select(x => GetValueAs<T>(subject, x)).ToArray();
		}

		public T GetValueAs<T>(string subject, string key)
		{
			return (T)GetValueAs(subject, key, typeof(T));
		}

		public object GetValueAs(string subject, string key, Type type)
		{
			string value = GetValue(subject, key);
			if (type == typeof(string)) return value;
			if (value == string.Empty) return Activator.CreateInstance(type)!;
			return Convert.ChangeType(value, type);
		}

		public T GetSubjectAs<T>(string? subject = null) where T : new()
		{
			if (subject is null)
			{
				subject = GetSubjects().Where(x => x.Equals(typeof(T).Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
			}
			if (subject is null) throw new InvalidOperationException($"Subject not found for '{typeof(T).Name}'");

			string[] keys = GetKeys(subject);
			T newObject = Activator.CreateInstance<T>();
			PropertyInfo[] properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();

			foreach (PropertyInfo property in properties)
			{
				string? key = keys.Where(x => x.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
				if (key is null) continue;
				object value = GetValueAs(subject, key, property.PropertyType);
				property.SetValue(newObject, value);
			}
			return newObject;
		}

		private Dictionary<string, Dictionary<string, string?>>? GetSettingsFromFile(string filePath)
		{
			string[] lines = _fileSystem.FileReadAllLines(filePath)
			    .Where(x => x != string.Empty && !x.StartsWith('#')).ToArray();

			if (lines.Length == 0 || lines[0][0] != '[') return null;
			
			int i = 1;
			return _settings = lines.Select(line => (line[0] == '[' ? ++i : i, line))
			    .GroupBy(x => x.Item1)
			    .ToDictionary(
				   a => a.First().Item2.Trim(new[] { '[', ']' }),
				   a => a.Skip(1).Select(x => x.Item2.Split('=', 2)).ToDictionary(
					  b => b[0],
					  b => b.Length > 1 ? b[1] : null
			    ));
		}
	}
}