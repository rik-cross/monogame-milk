using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System.IO;

namespace milk.Core;

internal class EmbeddedContentManager : ContentManager
{
    private readonly Assembly _assembly;
    private readonly string _resourcePrefix;

    internal EmbeddedContentManager(IServiceProvider serviceProvider, string resourcePrefix) 
        : base(serviceProvider)
    {
        _assembly = Assembly.GetExecutingAssembly();
        _resourcePrefix = resourcePrefix; // e.g., "MyEngine.Resources"
    }

    protected override Stream OpenStream(string assetName)
    {
        // Convert "Fonts/MainFont" to "MyEngine.Resources.Fonts.MainFont.xnb"
        string resourcePath = $"{_resourcePrefix}.{assetName.Replace('/', '.')}.xnb";
        var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
            throw new ContentLoadException($"Could not find embedded resource: {resourcePath}");

        return stream;
    }
}