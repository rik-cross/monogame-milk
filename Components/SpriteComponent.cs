using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameECS;

public class SpriteComponent : Component
{

    public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private string currentState;
    private string previousState;

    // constructor is just passing a single image
    public SpriteComponent(Texture2D texture = null)
    {
        if (texture != null)
        {
            AddSprite(
                new Sprite(new List<Texture2D>() { texture }),
                state: "default"
            );
        }
    }

    public void AddSprite(Sprite sprite, string state = "default")
    {
        sprites[state] = sprite;
    }

    public bool HasSpriteForState(string state)
    {
        return sprites.ContainsKey(state) == true && sprites[state] != null;
    }

    public Sprite GetSpriteForState(string state)
    {
        return sprites[state];
    }

    //public Texture2D GetCurrentTexture(string state = "default")
    //{   
    //}

    //public void AddTexture(Texture2D texture, string state = "default", ...)
    //{
    //}

    //public void AddTextures(List<Texture2D> textures, string state = "default", ...) {  
    //}

}