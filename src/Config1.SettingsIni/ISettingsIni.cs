namespace Config1.SettingsIni
{
	public interface ISettingsIni
	{
		string[] GetKeys(string subject);
		T GetSubjectAs<T>(string? subject = null) where T : new();
		string[] GetSubjects();
		string GetValue(string subject, string key);
		object GetValueAs(string subject, string key, Type type);
		T GetValueAs<T>(string subject, string key);
		string[] GetValues(string subject);
		T[] GetValuesAs<T>(string subject);
		bool KeyExists(string subject, string key);
		void Reload();
		bool SubjectExists(string subject);
	}
}