# Making Maps

Sets of prefabs and tilesets are avaliable to quickly build your maps.

The general style of s&box maps consists of realistic maps with more cartoony gameplays, but you can do whatever you want.

## Tilesets

There is a basic tileset included with the game, this can be used to quickly block out your maps, each tile is 256 x 256 ( due to current limitations ), 128 x 128 or 192 x 192 can work nicely too on thinner sections.

When working with tilesets you can always enable/disable/collapse specific or all tiles in your map to add more detail.

## Surfaces

When creating maps there are some predefined surface types you can use, these define different physics behaviours as well as sounds and should be set on the Hammer material property.

* `minigolf.grass` - default grass
* `minigolf.sand` - drastically slows the ball down
* `minigolf.ice` - completely removes linear friction

## Wall Physics

Unfortunately with how the physics engine works in order for collisions to be detected properly walls need to be a seperate entity. This is done in the basic tilesets by creating a world mesh with no collision and a seperate brush entity `minigolf_wall` on top which we can control the reflectivity multiplier on.

## Entities

These should be in the prefabs for you already, but a quick rundown of the entities required in maps:

* `minigolf_hole_spawn` - a point entity where the ball spawns on the hole, the hole name and par
* `minigolf_hole_goal` - a brush entity that triggers goal on a hole
* `minigolf_hole_bounds` - a brush entity that contains the bounds
* `minigolf_wall` - walls are reflected, you should make these nodraw and use world geo blah blah