# LOG

# Folder Structure
All project content lives in `Assets/`:

- `Animations/` - animation clips and controllers (empty for now)
- `Audio/` - music and sound effects (empty for now)
- `Data/` - game data assets (ScriptableObjects)
  - `Consumables/Food/` - `Beans`
  - `Mobs/` - mob profiles (`MobLevel1`, `MobLevel2`, `MobLevel3`)
  - `Consumables/Drinks/` - `WaterBottle`, `Soda`
  - `Weapons/Shooters/` - `EM Raygun`
    - `Lazers/` - `Lazer`, `UV Lazer`, `Gamme Lazer` (ammo/upgrades for the EM Raygun)
  - `Weapons/Melee/` - `Punch`, `Kick`, `Bat`
  - `Weapons/Specials/` - `Rocket`, `Grenade`
- `Fonts/` - Arial, used by all UI text
- `Images/` - 2D images (`pointer.png` - aim arrow, `circleborder.png` - door ring and prompt)
- `Materials/Palette/` - ready-to-use materials: `Colours` (23 flat colours), `Metals`, `Glow` (emissive), `Glass` (see-through), `Surfaces` (wood, stone, brick, ice, lava...)
- `Materials/` - materials for scene objects. `Level/` has the level materials (Floor, Grass, Path, SpawnPad, TreeTrunk, TreeLeaves, Door, HexFloor, HexWall). Others: (`Baseplate`, `Bullet`, `Gamme Lazer`, `MapWalls`, `Player`, `UV Lazer`, `Wall`)
- `Models/` - 3D models and prefabs
  - `RayGun/` - `Lazer`, `UV Lazer`, `Gamme Lazer`
  - `Mobs/` - mob models (`MobLvl1`, `MovLvl2`, `MobLvl3`)
    - `Spawners/` - `MobSpawner` prefab
- `Resources/` - `ItemDatabase.asset`, the list of every item in the game
- `Scenes/` - all game scenes (`Hub`, `Main Menu`, `Scene 1 Plains`)
- `Scripts/` - all C# code
  - `Achievements/` - achievement list, progress tracking, panel and popup
  - `Editor/` - editor-only tools (not part of the game)
  - `Generic/` - small reusable scripts that can go on any object
  - `Items/` - item base class, consumables, item database
  - `Loot/` - mob drops, pickups and floating text
  - `Mobs/` - mob profiles, mobs, spawners
  - `Player/` - player profile, survival stats, combat, inventory
  - `Save/` - saving and loading to JSON, the shared player profile
  - `UI/` - buttons, menus and the player HUD
  - `Weapons/` - weapon types and projectiles
  - `World/` - things in the level the player interacts with (doors, hub portals and menu zones)
- `Textures/` - textures used by materials (empty for now)
- `UI/` - UI prefabs
  - `MobHealthBar` - health bar shown above mobs
  - `GeneralTextWindow` - standard full-screen text window: Title bar, scrolling content (heading + body text), Close button. Use it as a menu's `Frame` (see UI POLICY in `AUTOMATIONS.md`)

# Build Settings
- Unity version: 2023.1.22f1
- Scenes:
  - 0: `Hub` game starts here
  - 1: `Main Menu` old menu
  - 2: `Scene 1 Plains`
- Any new scene must be added here or it can't be loaded by name
- Pick the platform (Windows), click **Build** choose an output folder
- Run the `.exe` from that folder
- Note: the Exit button only closes the app in a build, in the editor it just stops Play mode

# Scenes 
- Hub (main menu)
- Main Menu Scene 
- Scene 1: Plains

Scene Descriptions:

Hub:
- The main menu, a round cave you walk around in
- Player starts in the middle
- Back wall: intro text written on the wall
- Left portal: Level 1
- Right portal: Level 2 locked
- Left side: Shop, right side: Settings. Stand on the ring and it opens the thing you are standing on
- `IntroMenu`, `ShopMenu` and `SettingsMenu` are under `Canvas` and are empty
- `IntroMenu` uses the `GeneralTextWindow` prefab as its Frame: welcome title, how to play, controls, mobs and health
- `IntroMenu`, `SettingsMenu` and `LockedMessage` follow the UI POLICY (container > `Frame` > `Panel`, see `AUTOMATIONS.md`). `ShopMenu` doesn't yet, it's being worked on separately
- The Plains Esc menu `ReturnToMenu` button now loads the Hub

Main Menu:
- Starter UI for entering game, just main menu

Scene 1 Plains
- A basic plains scene has been made with a player (sphere object), a basic structure of walls to give it a building look, and walls around the map. Baseplate is 200x200. super basic map with colliders and movment script

Scene 1 Plains layout (200x200 grey floor, 4 walls around it):
- `Garden` (front/south): grass, trees, bushes, `SpawnPad` with `PlayerSpawn`, path to the building
- `HelperArea` (west of the spawn pad): 16x16 square with 4 low wooden walls and a doorway facing the spawn, `HelperSpot` in the middle. Empty for now, for helper stuff later
- `Building`: 75x75 maze entered from the front, `EndRoom` at the back with the next-floor door (`DoorPlaceholder` + `DoorInteract`), press E or click the prompt to go to the HexRoom
- `MobSpawners`: 3 across the middle of the maze, 3 in the end room
- `HexRoom` (east of the map, not walkable to): dark hexagonal room 75 across, `DoorSpawn` just inside its door, where the next-floor door will send the player

Scene 1 Plains UI (`Canvas > PlayerUI`):
- Top centre: Mobs Left tracker (Lvl 1 x n, Lvl 2 x m, Lvl 3 x p)
- Top right: Controls button, opens a panel listing all controls
- Left: Objectives list (edit it on `PlayerHUD`)
- Bottom: Health, Stamina, Hunger, Thirst bars across the full width
- Above the bars: Inventory panel (press I)
- Esc menu (`Canvas > EscMenu`)
- Achievements (`Canvas > AchievementUI`): `AchievementsPanel` in the centre (press J), `AchievementBanner` at the top centre when one unlocks

# Controls
| Key | Action |
|---|---|
| WASD | Move |
| Shift | Sprint (uses stamina) |
| Space | Jump |
| Mouse | Aim (360 degrees) |
| Right Click + drag | Rotate camera (360 degrees) |
| Scroll | Zoom in / out |
| Left Click / X | Attack with selected weapon |
| 1 / 2 / 3 | Select shooter / melee / special |
| Q | Cycle weapons |
| 4-9, 0 | Eat / drink from that inventory slot |
| I | Open / close inventory |
| J | Open / close achievements |
| E / Click prompt | Use a door when in range |
| H | Hide / show the player UI |
| Esc | Menu |

# Scripts
All scripts are in `Assets/Scripts/`.

## Scripts/
- `PlayerController.cs` - goes on the Player
  - WASD moves the player using its Rigidbody, so walls block it. Movement follows the camera's rotation (W is always up the screen)
  - Space jumps (only when on the ground), Shift sprints while there's stamina
  - Move speed comes from the player profile, `Sprint Multiplier` and `Jump Force` are set here
  - `uiEnabled` (static) - true while a menu with `OverlayUI` is open, all movement and camera controls stop
  - Right click + drag spins the camera (`Rotate Speed`), scroll zooms Orthographic Size between `Min Zoom` (8) and `Max Zoom` (20) in steps of `Zoom Step` (1), starting at `Start Zoom` (10)
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
  - `TakeDamage(amount)` - mobs and enemy shots call this to lower health
  - Hunger and thirst each drop by 1 every 5 seconds (`Hunger Drop Every`, `Thirst Drop Every`)
  - While hunger or thirst is at 25 or below (`Low Level`), health drops by 1 every 8 seconds (`Health Drop Every`)
  - Stamina drains while sprinting and refills otherwise
  - At 0 health: stats refill and the scene restarts
  - Eating/drinking counts towards achievements (items consumed, food eaten, drinks consumed)
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
- `Damage.cs` - shared helpers: `Damage.Hit` (one target) and `Damage.HitArea` (everything in a radius, each mob once)
- `Projectile.cs` - `Projectile.Spawn(...)` fires a model as a shot (used by the player and shooter mobs)
  - Player shots hit everything except the player. Enemy shots hit only the player and pass through mobs
  - Flies straight, disappears on hit or after travelling its `distance`
  - Calls `TakeDamage(float)` on whatever it hits (mobs lose health), specials hit everything in their radius

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
- `MobsLeftUI.cs` - on PlayerUI/MobsLeft (top centre): "Mobs Left" with each mob's `shortName` x how many are still to be killed across all spawners (not spawned yet + alive)
- `OverlayUI.cs` - goes on a menu's `Frame`. While it's showing, `PlayerController.uiEnabled` is true: no moving, jumping, attacking, weapon switching, eating, or camera rotate/zoom. Counts open overlays, so several can be open. On `GeneralTextWindow` (so the Hub intro) and the Hub's `LockedMessage/Frame`
- `PlayerHUD.cs` - goes on PlayerUI, keeps the Health/Stamina/Hunger/Thirst sliders in sync and fills the objectives list from `Objectives`. H hides/shows the whole player UI (`Hide Key`)

## Scripts/Mobs/
- `MobData.cs` - a mob profile asset: `id`, `displayName`, `model`, `type` (Follow / Shooter), `health`, `attackDamage`, plus Follow settings (`moveSpeed`, `chaseRange`, `attackRange`, `attackCooldown`) and Shooter settings (`shootRange`, `fireRate`, `projectileModel`, `projectileSpeed`, `projectileSize`), and `drops` (loot: consumable + chance 0-1). Create > Game > Mob
- `Mob.cs` - base for every mob: health, health bar, dies at 0 (drops its loot and counts the kill for achievements), helpers for finding the player
  - `FollowMob.cs` - wanders until the player is within `chaseRange`, then chases and hits them every `attackCooldown` seconds. Has a Rigidbody so walls block it
  - `ShooterMob.cs` - stays still, faces the player within `shootRange` and shoots `projectileModel` at them
  - The spawner adds FollowMob or ShooterMob based on the profile's `type`
- `MobHealthBar.cs` - on the `UI/MobHealthBar` prefab: slider + health number (e.g. `75 / 100`) above each mob, faces the camera
- Each mob prefab has a Box Collider on its root covering the whole model, so hits always land
- `MobSpawner.cs` - goes on a spawner (`Models/Mobs/Spawners/MobSpawner` prefab)
  - `Spawns` - rows of mob + amount, weakest first
  - Row 0 starts straight away and spawns every 5s, each next row starts 30s later and spawns 4s slower (row 1: after 30s every 9s, row 2: after 60s every 13s)
  - `Spawn Radius` - random spread, mobs are placed so they stand on the floor
  - `Activation Radius` (20) - nothing spawns until the player is this close, timings start from then (yellow circle in the Scene view)
  - `Health Bar Prefab` - put above each mob it spawns
  - Shows a red circle in the Scene view

| Mob | Type | Health | Attack Damage | Speed | Per spawner |
|---|---|---|---|---|---|
| Mob Level 1 | Follow | 100 | 10 / hit | 3.5 | 20 |
| Mob Level 2 | Follow | 125 | 15 / hit | 2.5 | 10 |
| Mob Level 3 | Shooter | 200 | 22 / shot | stays still | 5 |

Loot (each rolled separately when the mob dies):

| Mob | Beans | Water Bottle | Soda |
|---|---|---|---|
| Mob Level 1 | 30% | 25% | - |
| Mob Level 2 | 40% | 35% | 20% |
| Mob Level 3 | 55% | 45% | 35% |

## Scripts/Loot/
- `LootDropper.cs` - `LootDropper.Drop(mobData, position)` rolls the mob's `drops` and spawns a Pickup for each success. Called by `Mob` when it dies
- `Pickup.cs` - a dropped consumable on the ground
  - `Pickup.Spawn(position, item)` makes one: a small cube, blue for drinks, orange for food, white otherwise
  - Spins and bobs, walk over it to pick it up. If the inventory is full it stays on the ground
  - Disappears after `Lifetime` (30s)
  - Counts towards achievements (picked up, or missed with a full inventory)
- `FloatingText.cs` - `FloatingText.Show(position, message)` shows a short message (e.g. "+ Beans") that floats up and fades

## Scripts/Achievements/
- `AchievementData.cs` - one achievement: `id`, `title`, `description`, the stat it tracks and the `goal`
  - Tracked stats (`AchievementStat`): MobsKilled, ItemsPickedUp, ItemsConsumed, DrinksConsumed, FoodEaten, DropsMissed
- `AchievementManager.cs` - holds every achievement and a counter per stat
  - Other scripts call `AchievementManager.Add(stat, 1)` when something happens, achievements unlock when their goal is reached
  - `OnUnlocked` event for UI, progress saved to `achievements.json`
- `AchievementPanel.cs` - on `Canvas/AchievementUI`. J opens a list of all achievements, e.g. `[ ] Slayer - Kill 10 mobs (7/10)`, `[x]` when unlocked
- `AchievementPopup.cs` - on `Canvas/AchievementUI`. Shows "Achievement unlocked: X" at the top for `Show Time` (3s)

| Achievement | Goal |
|---|---|
| First Blood / Slayer / Exterminator | Kill 1 / 10 / 50 mobs |
| Finders Keepers / Scavenger / Hoarder | Pick up 1 / 10 / 30 items |
| Snack Time / Well Fed | Eat or drink 1 / 20 items |
| Thirsty Work | Drink 5 drinks |
| Hungry Hunter | Eat 5 food items |
| Bag Full / Pack Mule | Walk over 1 / 5 pickups with a full inventory |

## Scripts/World/
- `MenuZone.cs` - goes on a spot in the hub (shop, settings, intro wall)
  - When the player walks within `Radius` the `Menu` opens, walking away closes it
  - `Radius Ring` - circle on the floor, visible within `Show Ring Within`
- `HubPortal.cs` - goes on a portal with a trigger collider
  - Walking in loads `Scene Name` (must be in Build Settings)
  - If `Locked`, shows `Locked Message` for `Show Message For` seconds instead
- `WallHider.cs` - goes on `CaveWalls` in the hub. Hides the walls between the camera and the player so they don't block the view (they still have colliders and shadows)
- `DoorInteract.cs` - goes on a door
  - When the player is within `Radius` of `Interact Point`: shows the on-screen `Prompt` (circle with "E" / "Click") above the door
  - Press `Key` (E) or click the prompt to move the player to `Destination`
  - `Radius Ring` - circle on the floor showing the radius, visible within `Show Ring Within`
  - The end-room door sends the player to `HexRoom/DoorSpawn`

## Scripts/Generic/
- `FaceCamera.cs` - keeps a label turned towards the camera (used for the names above the portals, shop and settings)
- `InactiveOnStart.cs` - turns its object off when the scene starts. Leave the object switched on in the editor, otherwise this won't run until it's first enabled

## Scripts/Editor/
- `HubBuilder.cs` - adds **Tools > Build Hub**. Builds the hub scene out of basic shapes, saves it as `Assets/Scenes/Hub.unity` and puts it first in Build Settings. Editor only, not part of the game

# Save Data
- Saved as JSON in Unity's persistent data folder, not in `Assets/` (Assets is read-only in a build)
- Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\GAMEDEV26\`
  - `profile.json` - player profile (stats, tokens, owned items, current inventory)
  - `achievements.json` - achievement progress and unlocks
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
