# MILK -- MonoGame Intuitive Library Kit

MILK is a simple ECS engine based on MonoGame, with a focus on ease of use.

### Usage

- Example [Tiny Town](https://github.com/rik-cross/tiny-town) RPG game repository.
- Documentation coming soon.

### Design Principles

MonoGameECS is a minimal, beginner-friendly engine that prioritises ease of use. This means:

* Minimal setup, e.g. users do not need to register components.
* Intuitive classes, methods and parameters.
* Interaction only with tangible objects, e.g. `entity.AddComponent(component)` rather than `componentManager.AddComponentToEntity(entity, component)`.
* Maximise default object values, e.g. `myImage = new Image()` should be possible (creating a 'blank' placeholder image).
* No 'reinventing the wheel', using existing MonoGame functionality wherever possible.
* Clear, fully commented code. This repo should serve as a guide to simple ECS development.

### Changelog
- [CHANGELOG](CHANGELOG.md)
- [ROADMAP](ROADMAP.md)

### Contributing
- Contributions meeting the above design principles are very welcome!
- Add feedback, requests and bugs as [issues](https://github.com/rik-cross/monogame-milk/issues).

### Licence
MonoGameECS is shared under the MIT licence. Code can be adapted and shared without attribution. See [LICENSE](LICENSE) for more information.
