# LOG

# Folder Structure
All project content lives in `Assets/`:

- `Animations/` - animation clips and controllers (empty for now)
- `Audio/` - music and sound effects (empty for now)
- `Data/` - game data assets (ScriptableObjects)
  - `Consumables/Food/` - `Beans`
  - `Consumables/Drinks/` - `WaterBottle`, `Soda`
  - `Weapons/Shooters/` - `EM Raygun`
    - `Lazers/` - `Lazer`, `UV Lazer`, `Gamme Lazer` (ammo/upgrades for the EM Raygun)
  - `Weapons/Melee/` - `Punch`, `Kick`, `Bat`
  - `Weapons/Specials/` - `Rocket`, `Grenade`
- `Fonts/` - Arial, used by all UI text
- `Images/` - 2D images (`pointer.png` - aim arrow)
- `Materials/` - materials for scene objects (`Baseplate`, `Bullet`, `Gamme Lazer`, `MapWalls`, `Player`, `UV Lazer`, `Wall`)
- `Models/` - 3D models and prefabs (`Lazer`, `UV Lazer`, `Gamme Lazer`)
- `Resources/` - `ItemDatabase.asset`, the list of every item in the game
- `Scenes/` - all game scenes
- `Scripts/` - all C# code
  - `Generic/` - small reusable scripts that can go on any object
  - `Items/` - item base class, consumables, item database
  - `Player/` - player profile, survival stats, combat, inventory
  - `Save/` - saving and loading to JSON, the shared player profile
  - `UI/` - buttons, menus and the player HUD
  - `Weapons/` - weapon types and projectiles
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

Scene 1 Plains UI (`Canvas > PlayerUI`):
- Top right: Controls button, opens a panel listing all controls
- Left: Objectives list (edit it on `PlayerHUD`)
- Bottom: Health, Stamina, Hunger, Thirst bars across the full width
- Above the bars: Inventory panel (press I)
- Esc menu (`Canvas > EscMenu`)

# Controls
| Key | Action |
|---|---|
| WASD | Move |
| Shift | Sprint (uses stamina) |
| Space | Jump |
| Mouse | Aim (360 degrees) |
| Left Click / X | Attack with selected weapon |
| 1 / 2 / 3 | Select shooter / melee / special |
| Q | Cycle weapons |
| 4-9, 0 | Eat / drink from that inventory slot |
| I | Open / close inventory |
| H | Hide / show the player UI |
| Esc | Menu |

# Scripts
All scripts are in `Assets/Scripts/`.

## Scripts/
- `PlayerController.cs` - goes on the Player
  - WASD moves the player using its Rigidbody, so walls block it
  - Space jumps (only when on the ground), Shift sprints while there's stamina
  - Move speed comes from the player profile, `Sprint Multiplier` and `Jump Force` are set here
  - `Camera Settings` controls the camera (Camera, Pitch, Yaw, Roll, Distance, Orthographic Size). The camera follows the player and always uses these values, change them here not on the camera
- `DayNight.cs` - goes on GameController
  - Rotates the Directional Light (sun) for a day/night cycle and fades it out at night
  - `Sun` is the light, `Day Length` is seconds per day, `Time Of Day` is where the day starts (0 midnight, 0.25 sunrise, 0.5 noon, 0.75 sunset), `Max Intensity` is noon brightness
- `EscapeMenu.cs` - goes on an always-on object (EscMenu)
  - Pressing Esc toggles the `Menu` object on/off
  - Put `InactiveOnStart` on the menu if it should start hidden

## Scripts/Player/
- `PlayerProfile.cs` - the player's saved data (not a component)
  - Level, Tokens, Speed, and Health, Stamina, Hunger, Thirst (all 0-100)
  - `Owned Items` - everything the player has bought
  - `Current Inventory` - what they have on them right now: shooter, lazer, melee, special and the food/drink slots. Only owned items can be equipped
  - Selected weapon
  - Inventory slots (7 food/drink slots, keys 4-9 and 0)
  - Values have getters/setters that keep them in range (e.g. stamina can't go below 0 or above max)
  - To add a new stat: add a private `[SerializeField]` field and a public property
- `PlayerStats.cs` - goes on the Player
  - Hunger and thirst each drop by 1 every 5 seconds (`Hunger Drop Every`, `Thirst Drop Every`)
  - While hunger or thirst is at 25 or below (`Low Level`), health drops by 1 every 8 seconds (`Health Drop Every`)
  - Stamina drains while sprinting and refills otherwise
  - At 0 health: stats refill and the scene restarts
  - Drain/regen rates are set in the Inspector, live profile values are shown under `Profile` while playing
  - Right-click the component for **Save Now** / **Delete Save**
- `PlayerCombat.cs` - goes on the Player
  - Aims at the mouse, 360 degrees around the player
  - Left click or X attacks with the selected weapon, limited by its fire rate
  - 1/2/3 select shooter/melee/special, Q cycles
  - `Aim Arrow` - flat sprite on the floor that points at the mouse (`Arrow Distance`, `Arrow Height`, `Arrow Spin`)
  - Right-click the component for **Unlock All Weapons (testing)**
- `PlayerInventory.cs` - goes on the Player
  - I opens/closes `Inventory Panel`
  - 10 slots: 1 shooter, 2 melee, 3 special, 4-9 and 0 food/drink
  - Pressing 4-9 or 0 eats/drinks that slot
  - `Slot Labels` - one Text per slot, filled in while the panel is open (selected weapon shows yellow)

## Scripts/Weapons/
All weapons are assets (see Weapons and Items below).
- `WeaponData.cs` - shared by all weapon gear: `unlocked` (starts owned), `model`
- `ShooterData.cs` - slot 1 gun (EM Raygun). Has a `Lazers` list of the lazers it can fire, the lazer decides the shot
- `LazerData.cs` - a bullet/upgrade for a gun: `damage`, `fireRate` (shots/sec), `speed`, `distance` (50), `size`, `rotationOffset`
- `MeleeData.cs` - slot 2: `damage`, `fireRate`, hits everything within `radius` in front of the player
- `SpecialData.cs` - slot 3, fires a projectile that explodes: `damage`, `fireRate`, `speed`, `radius`, `distance` (50), `size`, `rotationOffset`
- `Projectile.cs` - added to each shot automatically when fired
  - Flies straight, disappears on hit or after travelling its `distance`
  - Calls `TakeDamage(float)` on whatever it hits (for enemies later), specials hit everything in their radius

## Scripts/Items/
- `ItemData.cs` - base for every item: `id` (used in the save), `displayName`, `tokenCost` (100), `levelRequired` (1)
- `ConsumableData.cs` - food/drink: restores `hunger`, `thirst`, `stamina`, `health`
- `ItemDatabase.cs` - every item in the game, split into lists: shooters, lazers, melee, specials, food, drinks
  - The save only stores ids (e.g. `"lazer"`), the game uses this to turn an id back into the item asset
  - The asset must be at `Assets/Resources/ItemDatabase.asset`

## Scripts/Save/
- `SaveSystem.cs` - saves/loads any `[System.Serializable]` class as JSON
  - `SaveSystem.Save("name", data)`, `SaveSystem.Load("name", fallback)`, `SaveSystem.Delete("name")`
- `ProfileManager.cs` - the one player profile, usable from any script in any scene
  - `ProfileManager.Profile.Tokens += 50;`
  - Loads `profile.json` the first time it's used, saves when the game closes or the scene changes
  - `TryBuyWeapon`, `TryBuyConsumable`, `TryBuyInventorySlot` (check tokens/level/space)
  - `Equip` (puts an owned item in the current inventory), `Owns`, `CanAfford`
  - `GetOwned<T>()` - every owned item of a type, e.g. `GetOwned<LazerData>()` for a pick-your-lazer menu

## Scripts/UI/
Button scripts go on a Button and hook into its click automatically (no need to use the On Click list).
- `OpenScene.cs` - loads a scene, picked from a dropdown of scenes in Build Settings
- `ExitGame.cs` - closes the game (stops Play mode in the editor)
- `OpenElemOnClick.cs` - turns ON every object in its `Elements` list
- `CloseElemOnClick.cs` - turns OFF every object in its `Elements` list
- `PlayerHUD.cs` - goes on PlayerUI, keeps the Health/Stamina/Hunger/Thirst sliders in sync and fills the objectives list from `Objectives`. H hides/shows the whole player UI (`Hide Key`)

## Scripts/Generic/
- `InactiveOnStart.cs` - turns its object off when the scene starts. Leave the object switched on in the editor, otherwise this won't run until it's first enabled

# Save Data
- Saved as JSON in Unity's persistent data folder, not in `Assets/` (Assets is read-only in a build)
- Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\GAMEDEV26\profile.json`
- Same location in the editor and in builds
- The save only stores item ids, so item stats/models can be changed without breaking saves
- To reset, delete the file or right-click **Player Stats > Delete Save** (do this after changing profile fields)

# Weapons and Items
- Every item is an asset in `Assets/Data/`
- Full step-by-step guide (for people and Claude): `AUTOMATIONS.md` > Adding Items
- To make a new one:
  - Right-click in Project > **Create > Game > Weapons > Shooter / Lazer / Melee / Special**, or **Create > Game > Consumable**
  - Give it a unique `id`, set its stats (and `model` for weapons)
  - Add it to the right list in `Assets/Resources/ItemDatabase.asset`, otherwise the game can't find it
  - New lazer? Also add it to the gun's `Lazers` list
- EM Raygun, Lazer and Punch are `unlocked` (every player starts with them), everything else must be bought

EM Raygun lazers (slot 1 fires the equipped lazer):

| Lazer | Damage | Shots/sec | Speed | Model |
|---|---|---|---|---|
| Lazer | 10 | 4 | 20 | `Lazer` |
| UV Lazer | 4 | 10 | 28 | `UV Lazer` |
| Gamme Lazer | 40 | 1 | 35 | `Gamme Lazer` |

| Melee | Damage | Attacks/sec | Radius |
|---|---|---|---|
| Punch | 5 | 2 | 1.2 |
| Kick | 8 | 1.2 | 1.5 |
| Bat | 15 | 1 | 2 |

| Special | Damage | Shots/sec | Speed | Explosion Radius |
|---|---|---|---|---|
| Rocket | 50 | 0.5 | 15 | 3 |
| Grenade | 35 | 0.8 | 8 | 5 |

| Consumable | Restores | Token Cost |
|---|---|---|
| Beans | Hunger +40 | 10 |
| Water Bottle | Thirst +50 | 10 |
| Soda | Thirst +25, Stamina +30 | 15 |

Every item has a `tokenCost` and `levelRequired`. Weapons and lazers default to 100 tokens and level 1. All lazers and specials travel 50 units.
