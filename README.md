minigolf bro!!

### Note on making maps

* A course consists of holes, each hole needs the following entities:
  * `minigolf_hole_info` - a point entity describing the hole
  * `minigolf_hole_spawn` - a point entity where the ball spawns on the hole
  * `minigolf_hole_goal` - a brush entity that triggers goal on a hole
  * `minigolf_hole_bounds` - a brush entity that contains the bounds
* Use the provided prefabs for holes.
* A ball is 8x8x8 (4 radius).
* **Sides have to be a func_brush** to do collisions properly, use the following properties:
  * Solidity: Always Solid
  * Render to Cubemaps: Yes
  * Lightmap Static: Yes

### TODO

Not inclusive of everything, just so I don't forget.

* [ ] Add a flag to the hole, 2D element if it's too far away.
* [ ] Repurpose the standard kill feed to include when other players scoring.
* [ ] Spectating: for players not playing and players who hve finished their current hole.
* [ ] Map voting at the end of a course.
