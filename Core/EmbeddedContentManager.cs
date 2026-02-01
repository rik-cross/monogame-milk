//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace milk.Core;

internal class EmbeddedContentManager : ContentManager
{
    private readonly Assembly _assembly;
    private readonly string _resourcePrefix;

    internal EmbeddedContentManager(IServiceProvider serviceProvider, string resourcePrefix) 
        : base(serviceProvider)
    {
        _assembly = Assembly.GetExecutingAssembly();
        _resourcePrefix = resourcePrefix;
    }

    protected override Stream OpenStream(string assetName)
    {
        string resourcePath = $"{_resourcePrefix}.{assetName.Replace('/', '.')}.xnb";
        var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
            throw new ContentLoadException($"Could not find embedded resource: {resourcePath}");

        return stream;
    }
}