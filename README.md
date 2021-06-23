# Cinematic-tool-demo

A camerawork demo that can be applied in other projects to see how the camera will roughly work before actually start working on the camera movement.

## Features

### Waypoint Menus
- Waypoint generation/deletion : Can generate, and delete the waypoint via UI and mouse click on the map.

- Waypoint gen distance settings : Set the default distance from the surface where new node will be generated.

- Waypoint movement : Enables the direction controller. User can click and drag to move the waypoint.

![NodeGenDel](https://github.com/Picbridge/Cinematic-tool-demo/blob/main/GIF/WaypointMenu.gif)

### Camera control menus
- Simulate/Stop : This button lets the camera start and follow the waypoints. 
User can press once again to pause the simulation. Cannot be clicked while generate waypoint status.

- Reset : This button deletes all the waypoints. Cannot be clicked while generate waypoint status.

- Avoidance distance : This slider controls the distance of auto generated avoidance nodes from the obstacle. This feature won’t be updated automatically, so that the user can differ the value each time. The position of the avoidance nodes will be updated in conditions of: waypoint generation, waypoint movement.

- Cam speed : This slider controls the speed of the camera when going through the waypoints.

![CamMenu](https://github.com/Picbridge/Cinematic-tool-demo/blob/main/GIF/CamMenu.gif)

### Camera look at menus
- Set objects to look : This button enable/disables the setting of featured objects. While enabled, the user can click on obstacles to set them as “featured objects”. Camera will focus on this featured object when passing by. The object will turn to yellow, and the bound where camera will start focus on will be shown. Object needs mesh renderer.
 
- Look from distance : This slider controls the radius of the bound from where camera will start to focus on.

- Cam steer speed : This slider controls the camera rotating speed

- Cam orbit radius : This slider controls the distance between object and the camera while camera is within the distance of observation.

- Orbit speed : This slider controls the speed while orbiting around the object.

![LookAtMenu](https://github.com/Picbridge/Cinematic-tool-demo/blob/main/GIF/LookAtMenu.gif)


### Sources

Test map 1: https://assetstore.unity.com/packages/3d/environments/landscapes/p-w-temple-edition-33637

Statue in test map 1: https://assetstore.unity.com/packages/3d/environments/fantasy/angel-statue-27594

Test map 2: https://assetstore.unity.com/packages/3d/environments/3d-free-modular-kit-85732


