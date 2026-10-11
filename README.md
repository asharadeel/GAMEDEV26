# LOG

# Folder Structure
All project content lives in `Assets/`:

- `Animations/` - animation clips and controllers (empty for now)
- `Audio/` - music and sound effects (empty for now)
- `Images/` - 2D images, e.g. UI art (empty for now)
- `Materials/` - materials for scene objects (`Baseplate`, `MapWalls`, `Player`, `Wall`)
- `Models/` - 3D models (empty for now)
- `Scenes/` - all game scenes
- `Scripts/` - all C# code
  - `Generic/` - small reusable scripts that can go on any object
  - `UI/` - scripts for buttons and menus
- `Textures/` - textures used by materials (empty for now)
- `UI/` - UI assets such as sprites and prefabs (empty for now)

# Build Settings
- Unity version: 2023.1.22f1
- Open **File > Build Settings**
- Scenes in build (order matters, index 0 loads first):
  - 0: `Main Menu`
  - 1: `Scene 1 Plains`
- Any new scene must be added here or it can't be loaded by name
- Pick the platform (Windows), click **Build**, choose an output folder
- Run the `.exe` from that folder
- Note: the Exit button only closes the app in a build, in the editor it just stops Play mode

# Scenes 
- Main Menu Scene 
- Scene 1: Plains

Scene Descriptions:

Main Menu:
- Starter UI for entering game, just main menu

Scene 1 Plains
- A basic plains scene has been made with a player (sphere object), a basic structure of walls to give it a building look, and walls around the map. Baseplate is 200x200. super basic map with colliders and movment script

# Scripts
All scripts are in `Assets/Scripts/`.

## Scripts/
- `PlayerController.cs` - goes on the Player
  - WASD / arrow keys move the player using its Rigidbody, so walls block it
  - `Speed` sets move speed
  - `Camera Settings` controls the camera (Camera, Pitch, Yaw, Roll, Distance, Orthographic Size). The camera follows the player and always uses these values, change them here not on the camera
- `DayNight.cs` - goes on GameController
  - Rotates the Directional Light (sun) for a day/night cycle and fades it out at night
  - `Sun` is the light, `Day Length` is seconds per day, `Time Of Day` is where the day starts (0 midnight, 0.25 sunrise, 0.5 noon, 0.75 sunset), `Max Intensity` is noon brightness
- `EscapeMenu.cs` - goes on an always-on object (EscMenu)
  - Pressing Esc toggles the `Menu` object on/off
  - Put `InactiveOnStart` on the menu if it should start hidden

## Scripts/UI/
All of these go on a Button and hook into its click automatically (no need to use the On Click list).
- `OpenScene.cs` - loads a scene, picked from a dropdown of scenes in Build Settings
- `ExitGame.cs` - closes the game (stops Play mode in the editor)
- `OpenElemOnClick.cs` - turns ON every object in its `Elements` list
- `CloseElemOnClick.cs` - turns OFF every object in its `Elements` list

## Scripts/Generic/
- `InactiveOnStart.cs` - turns its object off when the scene starts. Leave the object switched on in the editor, otherwise this won't run until it's first enabled
