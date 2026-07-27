<table>
  <tr>
    <td align="center">
      <img width="800" height="600" alt="image" src="https://github.com/user-attachments/assets/d09818ab-79cf-4ee5-8004-40588093621b" />
    </td>
    <td align="center">
      <img width="800" height="600" alt="image" src="https://github.com/user-attachments/assets/f60908e8-ae67-41ad-ad74-61187f6b7a8a" />
    </td>
  </tr>
</table>

# Asteroids DragonECS Sample

This project is a small Asteroids-style sample built around [DragonECS](https://github.com/DCFApixels/DragonECS). It is meant as a practical sandbox for exploring the framework in a game-like context: entity templates, systems, modules, feature boundaries, injection, runtime data, pooling, events, and Unity-facing views.

The code intentionally leans a bit heavier than a minimal arcade game would need. The goal is not to make the shortest possible Asteroids implementation, but to show how different DragonECS pieces can be composed, separated, and reused across a project.

One of the main ideas here is modularity. The project experiments with splitting the code into reusable modules, a game-level composition layer, and gameplay features built on top of that foundation. Some parts may look more explicit than strictly necessary for a tiny game, but that explicitness helps make the data flow, system ownership, and feature boundaries easier to inspect and adjust.

Use this repository as a reference point, a playground, or a source of examples when trying DragonECS patterns in Unity. It is a demonstration of one possible direction for organizing a project, with room for iteration depending on the needs of a real game.

Special thanks to [GreatVV](https://github.com/GreatVV), whose Asteroids project was used as the starting point for this sample.

## Packages

The sample uses a small set of non-Unity packages and embedded plugins:

- [DragonECS](https://github.com/DCFApixels/DragonECS) - the core ECS framework used for worlds, entities, components, systems, modules, queries, and injection.
- [DragonECS-Unity](https://github.com/DCFApixels/DragonECS-Unity) - Unity integration helpers, including templates and Unity-facing ECS utilities.
- [DragonECS-AutoInjections](https://github.com/DCFApixels/DragonECS-AutoInjections) - automatic dependency injection support for ECS systems.
- [DragonECS-Graphs](https://github.com/DCFApixels/DragonECS-Graphs) - graph and relation utilities used for entity links and interaction flow between gameplay objects.
- [DebugX](https://github.com/DCFApixels/Unity-DebugX) - runtime/debug drawing helpers used for visualizing vectors, bounds, field size, and other development information.
- [Simple CRT Shader](https://github.com/yunoda-3DCG/Simple-CRT-Shader) - an embedded CRT post-processing shader/effect package, modified in this sample to better match the desired look and adapted for the URP rendering flow.
