# Config1.SettingsIni
Settings reader for ini files

## info
A settings.ini file or alternative filepath which can be specified when constructing  
```
ISettingsIni ini = new SettingsIni("./app.ini");
```
The file it self can contain comments, a line starting with #, but should start with a subject.  
A subject line is marked with brackets the following lines can be keys or key=value pairs.  
```
[subject]
key1=value1
key2=1
```

To get a value try the following
```
string value = ini.GetValue("subject", "key1");
int value = ini.GetValueAs<int>("subject", "key2");
```

## mapping to objects
If you have an options object you can map a subject to it

```
public class AppOptions
{
    public string Key1 {get;set;}
    public int Key2 {get;set;}
}

AppOptions options = ini.GetSubjectAs<AppOptions>("subject");
```
