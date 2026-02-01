//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace milk.Core;

public class EngineResourceManager : ContentManager
{
    public Texture2D ImgMilk { get; private set; }
    public SpriteFont FontSmall { get; private set; }
    public SpriteFont FontMedium { get; private set; }
    public SpriteFont FontLarge { get; private set; }
    
    private readonly Assembly _assembly;

    internal EngineResourceManager(IServiceProvider services) : base(services)
    {
       _assembly = typeof(EngineResourceManager).Assembly;
        RootDirectory = ""; 
    }

    protected override Stream OpenStream(string assetName)
    {
        string resourcePath = assetName.EndsWith(".xnb") ? assetName : assetName + ".xnb";
        
        var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
            throw new ContentLoadException($"Could not find embedded resource: {resourcePath}");

        return stream;
    }

    internal void LoadEngineContent()
    {
        try 
        {
            ImgMilk = this.Load<Texture2D>("milk.Assets.Images.milk");
            FontSmall = this.Load<SpriteFont>("milk.Assets.Fonts.Small");
            FontMedium = this.Load<SpriteFont>("milk.Assets.Fonts.Medium");
            FontLarge = this.Load<SpriteFont>("milk.Assets.Fonts.Large");
        }
        catch (Exception ex)
        {
            global::System.Console.WriteLine("Error loading embedded asset: " + ex.Message);
        }
    }
}