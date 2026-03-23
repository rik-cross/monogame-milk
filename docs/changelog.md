# Changelog

<br />

![v0.1 - Regular milk](./images/milk.png)
#### v0.1 - Regular milk

- Top-level game class, with 'manager' classes
- Basic ECS architecture
- Scenes, including:
    - Menus including buttons, with the option to provide a custom draw method
    - Tiled tilemaps, including textures and scene colliders
    - Scene transitions
    - Scene renderables: Image and Text
    - Scene cameras
    - Scene animators which process 'lerp' functions
- Systems / components, including:
    - Sprite system and components, including animated sprites
    - Physics system (swept AABB), using transform and collider components
    - A trigger system, using trigger components with collision enter, collide and exit callbacks
    - An inventory system to store and manage entities
    - A collection system and 'collectable' component
    - An emote system, with emotes that appear above entities
- In-game logger

Coming soon to v0.1 (in no particular order):
- Scene:
    - Dialogue
    - Point of interest markers
    - Map querying (e.g. tile beneath entity)
- Systems / components:
    - Crafting
- Managers
    - Sound and music
