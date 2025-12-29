using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Reflection;

namespace milk.Core;

public class EngineResourceManager : ContentManager
{
    public Texture2D ImgMilk { get; private set; }
    public SpriteFont FontSmall { get; private set; }
    public SpriteFont FontMedium { get; private set; }
    public SpriteFont FontLarge { get; private set; }
    
    // Define the assembly field here
    private readonly Assembly _assembly;

    // Add this constructor to satisfy both ContentManager and your Game.cs call
    internal EngineResourceManager(IServiceProvider services) : base(services)
    {
       _assembly = typeof(EngineResourceManager).Assembly;
        // Set the root directory to empty because we are handling the pathing in OpenStream
        RootDirectory = ""; 
    }

    protected override Stream OpenStream(string assetName)
    {
        // assetName will be passed in as "milk.Assets.Images.placeholder"
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