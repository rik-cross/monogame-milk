# Introduction

MonoGame Intuitive Library Toolkit (milk) is a minimal, beginner-friendly engine that prioritises ease of use. This means that the **long-term** goals for the project are:

- Minimal setup, e.g. users do not need to register components.
- Interaction only with tangible objects, e.g. `entity.AddComponent(component)` rather than `componentManager.AddComponentToEntity(entity, component)`.
- Maximise default object values, e.g. `myImage = new Image()` should be possible (creating a 'blank' placeholder image).
- Clear, fully commented code. This repository should serve as a guide to simple ECS development.

milk is a layer on top of MonoGame, that provides additional functionality. For example, milk allows you to easily create scenes, entities, cameras, maps, UI, etc.

milk is an ECS engine, which has the following structure:

- Entities are just collections of components
- Components are a way of grouping data
- Systems are the logic that act on the components

Entities are added to scenes, and then components are added to entities. Systems them process entities that have all of the required components.