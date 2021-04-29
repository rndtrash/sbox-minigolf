minigolf bro!!

### Note on making maps

* A course consists of holes, each hole needs the following entities:
  * `minigolf_hole_info` - a point entity describing the hole
  * `minigolf_hole_spawn` - a point entity where the ball spawns on the hole
  * `minigolf_hole_goal` - a brush entity that triggers goal on a hole
  * `minigolf_hole_bounds` - a brush entity that contains the bounds
* There is a prefab for the hole goal.
* **Sides have to be a func_brush** to do collisions properly, this may change in the future.

### TODO

* [ ] Get a better map
* [ ] Determine holes info from the map entities and network this to clients.
* [ ] On powering show an arrow on the ball in 3D space.
* [ ] Lock input clientside when ball is moving, or maybe just have a networked CanShoot property?
* [ ] Add a flag to the hole, 2D element if it's too far away.
* [ ] Repurpose the standard kill feed to include when other players putt.
* [ ] Make a proper trailing particle
* [ ] Make a proper bounce particle instead of smoke..
* [ ] Find some cheer/applause/groan noises for hole in one -> under par.
* [ ] Add a lobby state where players have some time initially before the game starts, or if people join late.
* [ ] Spectating: for players not playing and players who hve finished their current hole.
* [ ] Map voting at the end of a course.
* [ ] Scoreboard that looks like a score card, show on tab and after every hole.
* [ ] Name tags on other peoples balls
* [ ] Customization options for your balls
* [ ] Music and more sounds