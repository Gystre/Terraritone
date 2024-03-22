# Terraritone
bot plays 2d block game for lazy people

Autonomous bot that moves the character for you using a modified version of the A* pathfinding algorithm. My goal for this project was to create an autonomous bot for Terraria that would be able to navigate from point A to point B and learn a little about how pathfinding algorithms and how they are applied in games. While I did get almost everything on my checklist, the way the program handles movement is fatally flawed. Despite it's shortcomings though, it can find it's way over and under simple terrain that doesn't involve complex jumps and movement mechanics.

## Showcase:
![really cool picture of the bot](https://kyleyu.org/assets/terraritone-demo.gif)

## Problems:
* algorithm thinks character can stop moving instantly but character continues to move even after movement key is released, resulting in overshooting the path
* character runs up 1 block jumps so they get stuck because they skipped the nodes
* player accelerates slower in the air than the algorithm thinks it can accelerate, resulting in getting stuck
