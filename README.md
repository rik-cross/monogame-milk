## MonoGameECS

An ECS engine, based on MonoGame, with a focus on ease of use.

#### Usage

`dotnet install MonoGameECS`

Alternatively, you are free to download, use and modify the code in this repo.

```
using MonoGameECS;

using var game = new Game();
game.Init = () => { game.SetScene(new MyScene()); };
game.Run();
```

#### Features

* An overall `Game` class
* ECS (Entities, Component and Systems)
  * Components: `TransformComponent`
  * Systems: `GraphicsSystem`
* A `Scene` that runs systems over a set of entities
* Scene `Camera`s, with zooming and tracking
* Scene `Transition`s and easing functions
* Scene renderables: `Text`, `Image`s

#### Versions and Roadmap

###### Version 0.1

* ECS implementation
* Scenes and cameras
* Scene transitions and easing functions
* Scene renderables
* Transform component and graphics system

#### Design Principles

MonoGameECS is a minimal, beginner-friendly engine that prioritises ease of use. This means:

* Minimal setup, e.g. users do not need to register components.
* Interaction only with tangible objects, e.g. `entity.AddComponent(component)` rather than `componentManager.AddComponentToEntity(entity, component)`.
* Maximise default object values, e.g. `myImage = new Image()` should be possible (creating a 'blank' placeholder image).
* Clear, fully commented code. This repo should serve as a guide to simple ECS development.

#### Contributing

Contributions meeting the above design principles are very welcome!

#### Licence

MonoGameECS is shared under the MIT licence. Code can be adapted and shared without attribution. See [LICENSE.md](LICENSE.md) for more information.
