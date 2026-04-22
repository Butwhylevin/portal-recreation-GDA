# Portal Recreation (GDA)

**Portal Recreation** is a fan-made implementation of the iconic portal mechanics from Valve’s *Portal*, built with Unity. This project demonstrates core gameplay systems including portal placement, teleportation, collision handling, and render-to-texture portal visuals.

## Features

- **Portal Placement**: Shoot portals onto surfaces; each portal aligns its rotation to the wall’s normal for seamless integration.
- **Teleportation System**: When the player enters a portal, they are instantly moved to the paired portal’s location and rotation, preserving momentum and orientation.
- **Collision Management**: While inside a portal’s hitbox, collision with walls is disabled to prevent clipping. Temporary colliders around the portal maintain physical interaction, and normal collision resumes after exiting.
- **Portal Rendering**: A dedicated camera renders the view from the other portal onto a texture, which is applied to the portal material using shaders.
- **Perspective Correction**: Implements an oblique projection matrix to ensure the rendered view matches the player’s perspective through the portal.

## Controls

- **Movement**: `WASD` keys
- **Look Around**: Mouse
- **Jump**: `Spacebar`
- **Blue Portal**: `Left Mouse Button`
- **Orange Portal**: `Right Mouse Button`
- **Teleport**: Walk into any portal

## Behind the Scenes

This project evolved through several technical challenges:

1. **Portal Placement**  
   Simply moves the portal object and aligns it to the normal of the wall it’s on.

2. **Basic Teleportation**  
   When the player collides with a portal, their position and rotation are set to the other portal’s transform.  
   *Important*: After teleporting, further teleportation is blocked until the player exits the new portal’s collider, preventing infinite back‑and‑forth loops.

3. **Collision Problem**  
   *Problem*: The player still collides with the wall the portal is embedded in, causing jitter and clipping.  
   *Solution*: Disable player collision with all walls while inside a portal’s hitbox, enable temporary colliders around the portal, then re‑enable normal collision when leaving the portal’s hitbox.  
   (This approach is based on [daniel-ilett/portals-urp](https://github.com/daniel-ilett/portals-urp).)

4. **Portal Visuals**  
   To make portals look like portals, a camera renders the view from the other side onto the portal surface using shaders. The camera must move dynamically to match the player’s perspective.

5. **Oblique Projection Matrix**  
   *The hardest part* – getting the perspective correct required implementing an oblique projection matrix for the secondary camera. This ensures that the rendered texture aligns perfectly with the player’s view through the portal.

## License

This project is a fan creation and is not affiliated with or endorsed by Valve Corporation. *Portal* and related trademarks are property of Valve. The source code is provided for educational and non‑commercial use.

## Acknowledgments

- [daniel-ilett/portals-urp](https://github.com/daniel-ilett/portals-urp) for the collision handling inspiration.
- The Unity community for countless resources on shaders, projection matrices, and portal rendering techniques.
- Valve for creating the timeless *Portal* series.
