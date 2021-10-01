# Making Maps

Sets of prefabs and tilesets are avaliable to quickly build your maps.

The general style of s&box maps consists of realistic maps with more cartoony gameplays.

Unfortunately with how the physics engine works in order for collisions to be detected properly walls need to be a seperate entity. This is done in the basic tilesets by creating a world mesh with no collision and a seperate brush entity `minigolf_wall` on top which we can control the reflectivity multiplier on.

## Entities

These should be in the prefabs for you already, but a quick rundown of the entities required in maps:

* `minigolf_hole_spawn` - a point entity where the ball spawns on the hole, the hole name and par
* `minigolf_hole_goal` - a brush entity that triggers goal on a hole
* `minigolf_hole_bounds` - a brush entity that contains the bounds
* `minigolf_wall` - walls are reflected, you should make these nodraw and use world geo blah blah