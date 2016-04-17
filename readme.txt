Greg Rozmarynowycz
COLONY
Interactive Media Development

Colony is an development simulation set on an alien planet where human have dispatached robotic drones to prepare the planet for
human habitation. The drones must gather resources (represented by orange minerals), bring them back to the Colony Hub, and when
enough resources are present, construct Habitation pods. When the colony is complete the simulation resets.

CONTROLS: Space bar to cycle cameras

The actions of each drone can be viewed through its first person camera, or mouse-controlled camera, with zoom, pan, and rotate
can be used to watch the progress of the entire colony (mouse based camera control: https://gist.github.com/JISyed/5017805).

Drones navigate the map with A* based pathfinding/following, with logic ensure it is virtually impossible for them to become
indefinitely stuck. If they remain in the same position for too long, they will re-calculate their path until they can move
again. The drone's also avoid obstacles using tangent-based obstacle avoidance, although collision/near-collision is somewhat
desirable to collect and drop off resources.

Drone's implement a task based AI system. If they don't have a task they will first attempt to construct a habitation pod. If there
are not enough resources in the Colony pool, then they will proceed to gather resources, and finally explore the map for further 
resource locations (not fully implemented). If there enough resources to build, they will attempt to find an open build site. If they
are unable to, this means the colony is complete and the simulation resets. Once they find an open build site, they claim it, locking
other drones out. Once they are close enough to the site, they will construct a pod.

If the drones have an active Gather task and mineral node, they will pathfind to the mineral node, and when close enough, gather
resources from it until their inventory is full. They will then drop off their inventory at the colony hub.

All models were created in Maya.