<br/>
<h1 align="center">
  <img width="50%" src="https://raw.githubusercontent.com/handsomematt/sbox-minigolf/master/ui/logo_golf.png">
</h1>

<h3 align="center">
	Multiplayer Minigolf for [Sandbox](https://sbox.facepunch.com/news)
</h3>
<br/>

# Making Maps

Anybody is welcome to create maps and upload them freely to the sbox devsite. Here's a quick rundown of the requirements for a map:

* A course consists of holes, each hole needs the following entities:
  * `minigolf_hole_spawn` - a point entity where the ball spawns on the hole, the hole name and par
  * `minigolf_hole_goal` - a brush entity that triggers goal on a hole
  * `minigolf_hole_bounds` - a brush entity that contains the bounds
* Use the provided prefabs for holes.
* A ball is 6x6x6 (3 radius).
* Use `minigolf_wall` for walls on a course.

# License

All Rights Reserved

Copyright (c) 2022 Matthew Stevens