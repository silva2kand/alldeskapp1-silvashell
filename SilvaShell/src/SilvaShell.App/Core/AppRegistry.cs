using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SilvaShell.App.Core;

public static class AppRegistry
{
    public static IReadOnlyList<AppModule> Load(string path)
    {
        if (!File.Exists(path))
            return new List<AppModule>();

        var json = File.ReadAllText(path);
        var modules = JsonSerializer.Deserialize<List<AppModule>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return modules ?? new List<AppModule>();
    }
}
