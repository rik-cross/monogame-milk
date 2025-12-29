# Getting Started

### Simplest use

Create a new MonoGame project, ensuring that you are using at least version 3.8.4. Add the milk library to your project:

```
dotnet add package monogame-milk
```

The code below is the simplest code that will create a new game. It will just show the default, placeholder scene, but is useful for testing that everything is installed and working:

```
using milk.Core;

using var game = new Game();
game.Run();
```

### Basic usage

To create a basic single-scene game, firstly create a new scene subclass:

```
using Microsoft.Xna.Framework;
using milk.Core;

public class MyScene : Scene
{
    public override void Init()
    {
        BackgroundColor = Color.CornflowerBlue;
    }
}
```

You can then create a new game with this class using the following code:

```
using milk.Core;

using var game = new Game();
game.Init = () => { game.SetScene(new MyScene()); };
game.Run();
```

### 'Tiny Town' Example

See the [Tiny Town](https://github.com/rik-cross/tiny-town) GitHub repo for an in-development RPG made with milk.