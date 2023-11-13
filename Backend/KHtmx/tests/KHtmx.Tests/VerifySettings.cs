using System.Runtime.CompilerServices;
using DiffEngine;

namespace KHtmx.Tests;

public static class VerifySettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Use Rider as Diff
        DiffTools.UseOrder(DiffTool.Rider);
    }
}
