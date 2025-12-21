# Introduction

MonoGameECS is a minimal, beginner-friendly engine that prioritises ease of use. This means that the long-term goals for the project are:

- Minimal setup, e.g. users do not need to register components.
- Interaction only with tangible objects, e.g. `entity.AddComponent(component)` rather than `componentManager.AddComponentToEntity(entity, component)`.
- Maximise default object values, e.g. `myImage = new Image()` should be possible (creating a 'blank' placeholder image).
- Clear, fully commented code. This repo should serve as a guide to simple ECS development.