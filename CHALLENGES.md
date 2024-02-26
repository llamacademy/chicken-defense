# Challenges
This repository is intended to be used for learning and education. There are some areas for improvement in this codebase (as with all software). The below are some areas you can improve or extend to solidify and better understand the concepts being taught here.
If you see some areas or have a good idea, feel free to open a PR to add the idea here!

Please do not commit and create PRs with your implementation of these challenges. The challenges are for your own learning and solutions will not be added to the repo.

## Easy(ish)
- Tweak the balancing of Llama cost versus damage
- Add a new enemy type
- Create a new procedural Click Shader. Be creative!
- Create a Shader for Unit Selection Decal that is more engaging
- Add death "juice" for enemies, chickens, and eggs
  - This could be sound effects, particle systems, animations, etc...
- Update the Spitting Llama to smoothly rotate to look at the
- Make the Fox jump animation look less wacky. Perhaps look at the direction of the NavMeshLink and rotate more realistically.
- Tweak the Minimap so Llamas can not go outside the bounds of the Minimap Camera
- Add Sound Effects for Llamas, snakes, and foxes. Things like slithering when moving, hoof stomps for llamas, spit sounds, etc...
- Show the Chicken Coop more clearly on the minimap. Perhaps a LineRenderer like the Camera Bounds. Use your imagination.

## Medium(ish)
- Change the targeting of the Spitting Llama to prefer foxes over snakes.
- Change the targeting of both Llamas to prefer enemies closer to the Chicken Coop instead of closest to them.
- Create a new Llama (perhaps a biting llama)
  - Use Animation Rigging to retarget the tip of the bite to the enemy being attacked!
- Create a UI to show the currently selected units
- Create a new "enemy" type
- Update difficulty scaling to also scale up/down number of starting chickens and eggs based on difficulty selected.

## Advanced(ish)
- Implement Jobs system for Egg targeting in AttackerBrain.cs
- Create a procedural jump animation for the Fox jumping over the fence
- Consider the design challenges with using the same base class for Enemies and Llamas. Reimplement them using different base classes.
- Add the ability for enemies to attack Llamas instead of only run away. This most likely requires significant rebalanacing.
