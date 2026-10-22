# READ_ME.MD

This is an AI-written document, kept up to date as the project changes so we can all understand each other's changes. Rules for how things should be built are in `AUTOMATIONS.md`.

# Folder Structure
All project content lives in `Assets/`:

- `Animations/` - animation clips and controllers (empty for now)
- `Audio/` - music and sound effects (empty for now)
- `Data/` - game data assets (ScriptableObjects)
  - `Consumables/Food/` - `Beans`
  - `Mobs/` - mob profiles (`MobLevel1`, `MobLevel2`, `MobLevel3`)
  - `Consumables/Drinks/` - `WaterBottle`, `Soda`
  - `Weapons/Shooters/` - `EM Raygun`
    - `Lazers/` - `Lazer`, `UV Lazer`, `Gamme Lazer` (the EM Raygun's level 1, 2 and 3)
  - `Weapons/Melee/` - `Punch`, `Kick`, `Bat`
  - `Weapons/Specials/` - `Rocket`, `Grenade`
- `Fonts/` - Arial, used by all UI text
- `Images/` - 2D images (`pointer.png` - aim arrow, `circleborder.png` - door ring and prompt)
- `Materials/Palette/` - ready-to-use materials: `Colours` (23 flat colours), `Metals`, `Glow` (emissive), `Glass` (see-through), `Surfaces` (wood, stone, brick, ice, lava...)
- `Materials/` - materials for scene objects. `Level/` has the level materials (Floor, Grass, Path, SpawnPad, TreeTrunk, TreeLeaves, Door, HexFloor, HexWall). Others: (`Baseplate`, `Bullet`, `Gamme Lazer`, `MapWalls`, `Player`, `UV Lazer`, `Wall`)
- `Models/` - 3D models and prefabs
  - `ShowUIonApproach` - drop next to anything (e.g. a sign) and set its `UI`: opens that UI when the player walks close, with a ring and a blue marker disc on the floor
  - `RayGun/` - `Lazer`, `UV Lazer`, `Gamme Lazer`
  - `Mobs/` - mob models (`MobLvl1`, `MovLvl2`, `MobLvl3`)
    - `Spawners/` - `MobSpawner` prefab
- `Resources/` - `ItemDatabase.asset`, the list of every item in the game
- `Scenes/` - all game scenes (`Hub`, `Main Menu`, `Scene 1 Plains`)
- `Scripts/` - all C# code
  - `Achievements/` - achievement list, progress tracking, panel and popup
  - `Editor/` - editor-only tools (not part of the game)
  - `Generic/` - small reusable scripts that can go on any object
  - `Hub/` - scripts only used in the Hub (Library)
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
  - `GeneralPopUpWindow` - standard pop-up: centred box with a title, one action button and a Close button. Already has `InactiveOnStart` and `OverlayUI`. Use it as a menu's `Frame` for prompts (see UI POLICY in `AUTOMATIONS.md`)
  - `GeneralTextWindow` - standard full-screen text window: Title bar, scrolling content (heading + body text), Close button. Use it as a menu's `Frame` (see UI POLICY in `AUTOMATIONS.md`)
  - `ShopItemCard` - one shop item (Name, Buy, Upgrade, Refund, Equip + `ShopItemCard`). Place one per item by hand in a shop section and drag the item asset into `Item`
  - `ConsumableCard` - one food/drink in the shop (Name, Buy, Refund, Quantity + `ConsumableCard`). Place one per item by hand and drag the item asset into `Item`
  - `GeneralTileWindow` - paged window: Title bar, boxes under `Frames` (`Step (1)`-`Step (4)`, each a Title + Content), Prev/Next and Close. `TilePager` shows one box at a time. Use it as a menu's `Frame` (see UI POLICY in `AUTOMATIONS.md`)
  - The window backgrounds (each prefab's root) are RawImages
  - Editing a copy in a scene only changes that copy (an override). Opening and editing the prefab itself, or **Overrides > Apply**, changes every copy. Unpacked copies never get prefab updates

# Build Settings
- Unity version: 2023.1.22f1
- Scenes:
  - 0: `Hub` game starts here (should probably become `Main Menu`, so a profile is picked first)
  - 1: `Main Menu` title screen, profile select and tutorial
  - 2: `Scene 1 Plains`
- Any new scene must be added here or it can't be loaded by name
- Pick the platform (Windows), click **Build** choose an output folder
- Run the `.exe` from that folder
- Note: the Exit button only closes the app in a build, in the editor it just stops Play mode

# Scenes 
- Hub (home base)
- Main Menu (title screen, profiles, tutorial)
- Scene 1: Plains

Scene Descriptions:

Hub:
- Home base, a round cave you walk around in
- Player starts in the middle
- Back wall: intro text written on the wall
- Left portal: Level 1
- Right portal: Level 2 locked
- Each portal has a square slot on its inner pillar (**Tools > Add Portal Slots**): Level 1's is purple (open), Level 2's is dark grey. After beating the Level 1 boss and picking up the portal piece, walking up to Level 2 asks to place it (`PlacePiecePrompt`), which turns the slot and the portal purple. Level 2 doesn't exist yet, so the open portal does nothing
- Left side: Shop, right side: Library. Stand on the ring and it opens the thing you are standing on
- `IntroMenu`, `StoreUI`, `LibraryMenu` are under `Canvas`
- `StoreUI > GeneralSelectorUI`: the shop window, 5 section buttons that each show their own frame (`TabSelector`). Tabs in order: Shooters (EM Raygun), Melee (Punch, Kick, Bat), Specials (Rocket, Grenade) as `ShopItemCard`s, Food (Beans), Drinks (Water Bottle, Soda) as `ConsumableCard`s, each in the section's `Items` grid. The Shooters tab's frame is still called `Weapons`
- `BuyPopup`, `UpgradePopup`, `RefundPopup`: the shop's pop-ups (GeneralPopUpWindow), run by `ShopPopups` on `StoreUI`
- `Scene1Prompt`: pop-up "Enter Scene 1?" (from `GeneralPopUpWindow`), Play loads Scene 1 Plains
- `IntroMenu` uses the `GeneralTextWindow` prefab as its Frame: welcome title, how to play, controls, mobs and health
- `IntroMenu`, `LibraryMenu` and `LockedMessage` follow the UI POLICY (container > `Frame` > `Panel`, see `AUTOMATIONS.md`). `ShopMenu` doesn't yet, it's being worked on separately
- The Plains Esc menu `ReturnToMenu` button now loads the Hub

Main Menu:
- Play, Tutorial and Exit
- Tutorial opens `Tutorial` (a `GeneralTileWindow`): 8 short steps flipped with Prev/Next - movement, camera, shooting, objective, boss, inventory, eating/drinking, the Hub. Steps 5-8 are added on the scene copy only, the prefab keeps 4
- Play opens `ProfileSelect` (a `GeneralTextWindow`): 3 profile cards on the left, `NewProfile` window on the right
  - Empty card: opens New Profile - name, optional passkey (toggle, 3-30 characters), ball colour
  - Filled card (name + creation date): plays and loads the Hub. If it has a passkey, `PasskeyPopup` asks for it first ("Username: ...")
  - Edit on a card: `EditPopup` (change name and ball colour). Delete: `DeletePopup` to confirm, then `PasskeyPopup` too if it has a passkey
  - All run by `ProfileSelectUI` on the `ProfileSelect` container

Scene 1 Plains
- A basic plains scene has been made with a player (sphere object), a basic structure of walls to give it a building look, and walls around the map. Baseplate is 200x200. super basic map with colliders and movment script

Scene 1 Plains layout (200x200 grey floor, 4 walls around it):
- `Garden` (front/south): grass, trees, bushes, `SpawnPad` with `PlayerSpawn`, path to the building
- `HelperArea` (west of the spawn pad): 16x16 square with 4 low wooden walls and a doorway facing the spawn, `HelperSpot` in the middle. Empty for now, for helper stuff later
- `Building`: 75x75 maze entered from the front, `EndRoom` at the back with the next-floor door (`DoorPlaceholder` + `DoorInteract`), press E or click the prompt to go to the HexRoom
- `MobSpawners`: 3 across the middle of the maze, 3 in the end room
- `HexRoom` (east of the map, not walkable to): dark hexagonal room 75 across, `DoorSpawn` just inside its door, where the next-floor door will send the player
- `HexRoom/BossArena`: the boss fight (added with **Tools > Build Boss Room**, running it again replaces it)
  - `Boss` (Plains Guardian) in the middle, wakes up when the player walks into the room
  - 6 purple `Pillar`s on the corners, joined to the boss by a beam. While any pillar stands the boss only takes 20% damage. Break all 6 (shoot or hit them) and the boss turns orange and takes full damage for 12 seconds, then the pillars grow back
  - Moves: jump slam (red ring shows where it lands, hurts everything inside), throws 3 boulders aimed ahead of the player (pillars block them), swings its club when the player is close
  - When it dies a `PortalPiece` appears in the middle. Walking onto it picks it up (saved on the profile as `HasPortalPiece`) and opens `PortalPieceMenu` (a `GeneralPopUpWindow`), both buttons load the Hub
- `PrototypeSkip` (yellow circle beside the path, before the maze): testing shortcut, stand in it and press E to go straight to `HexRoom/DoorSpawn`. Uses `DoorInteract`. Made by **Tools > Build Boss Room**, delete it before the final build

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
  - The ball takes the profile's colour (only when a profile was picked on the Main Menu)
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
  - Name, `Password Protected` + passkey (3-30 characters, `PlayerProfile.IsValidPasskey`), ball colour, creation date/time
  - Level, Tokens (new profiles start with 150), Speed, and Health, Stamina, Hunger, Thirst (all 0-100)
  - `Owned Items` - everything the player has bought
  - `Current Inventory` - what they have on them right now: shooter, lazer, melee, special and the food/drink slots. Only owned items can be equipped
  - Selected weapon
  - `Has Portal Piece` (picked up from the Level 1 boss) and `Portal 2 Open` (placed in the Hub)
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
  - While melee is selected and the equipped melee is Punch, a capsule `Fist` sits in front of the player pointing where they aim and jabs forward on each punch (`Fist Colour`)
  - Right-click the component for **Unlock All Weapons (testing)**
- `PlayerInventory.cs` - goes on the Player
  - I opens/closes `Inventory Panel`
  - 10 slots: 1 shooter, 2 melee, 3 special, 4-9 and 0 food/drink
  - Pressing 4-9 or 0 eats/drinks that slot
  - `Slot Labels` - one Text per slot, filled in while the panel is open (selected weapon shows yellow)

## Scripts/Weapons/
All weapons are assets (see Weapons and Items below).
- `WeaponData.cs` - shared by all weapon gear: `unlocked` (starts owned), `model`
- `WeaponData.cs` - base for all weapons: `unlocked`, `model`, and the prices: upgrade to level 2 = half the `tokenCost`, level 3 = 1.5x (rounded up), refund = a third of the `tokenCost` (rounded up, upgrades aren't refunded). Max level 3
- `ShooterData.cs` - slot 1 gun (EM Raygun). Its `Lazers` list is its levels: 1st lazer = level 1, 2nd = level 2, 3rd = level 3. The lazer decides the shot
- `LazerData.cs` - a bullet/upgrade for a gun: `damage`, `fireRate` (shots/sec), `speed`, `distance` (50), `size`, `rotationOffset`
- `MeleeData.cs` - slot 2, hits everything within `radius` in front of the player. 3 `Levels`, each with `model`, `damage`, `fireRate`, `radius`
- `SpecialData.cs` - slot 3, fires a projectile that explodes. 3 `Levels`, each with `model`, `damage`, `fireRate`, `speed`, `radius`, `distance` (50), `size`, `rotationOffset`
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
  - `SaveSystem.Save("name", data)`, `SaveSystem.Load("name", fallback)`, `SaveSystem.Exists("name")`, `SaveSystem.Delete("name")`
- `ProfileManager.cs` - the current player profile, usable from any script in any scene
  - `ProfileManager.Profile.Tokens += 50;`
  - 3 slots (`profile_1.json` to `profile_3.json`). The Main Menu picks one with `LoadSlot`. Playing a scene directly (testing) uses the old `profile.json` instead, `HasProfile` is false then
  - `CreateProfile`, `EditProfile` (name + colour), `DeleteSlot` (also deletes its achievements), `PeekSlot` (read a slot without switching)
  - Loads the profile the first time it's used, saves when the game closes or the scene changes
  - `TryBuyWeapon`, `TryBuyConsumable`, `TryBuyInventorySlot` (check tokens/level/space)
  - `GetLevel`, `CanUpgrade` / `TryUpgrade`, `CanRefund` / `TryRefund` (not starting items, not while equipped), `IsEquipped`
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
- `ProfileSelectUI.cs` - on the Main Menu's `ProfileSelect` container. Runs the profile cards, New Profile window and the Passkey/Delete/Edit pop-ups (see Main Menu above). `Play Scene` is where a picked profile goes (Hub)
- `ShopItemCard.cs` - on the `ShopItemCard` prefab. Drag a weapon asset into `Item`. Shows "<name>" and "Level <n>", plus Buy (not owned) or Equip + Upgrade + Refund (owned). Upgrade is hidden at level 3, Refund is hidden for starting items and greyed out while equipped
- `ConsumableCard.cs` - on the `ConsumableCard` prefab. Drag a food/drink asset into `Item`. Shows its name, Buy, Refund and "Amount owned: <n>" (how many are in the inventory slots). Buying puts one in the first free slot. Refund sells one back for half its price (rounded up), greyed out when you have none
- `ShopPopups.cs` - on the Hub's `StoreUI` container. Runs the shared `BuyPopup` ("Buy <item>", button "Buy: <cost>"), `UpgradePopup` ("Upgrade to level <n>", button "Cost: <cost>") and `RefundPopup` ("Refund <item>: <refund>"). Shows the player's tokens, or why it didn't work. Buying food/drink keeps the pop-up open so you can buy several
- `CurrencyChangeButton.cs` - on a Button: `Mode` Add / Subtract and `Amount` of tokens. Subtract doesn't go through without enough tokens
- `TabSelector.cs` - a row of buttons that each open their own frame. `Tabs` = list of `Button` + `Frame`. Clicking a button shows its frame and hides the rest, the open tab's button is greyed out. Opens on the first tab each time the window opens. On the Hub's `StoreUI/GeneralSelectorUI` (5 buttons under `SectionSelector`, 5 frames under `Sections`)
- `TilePager.cs` - shows one box (page) at a time instead of a scroll view. `Pages` = list of `Order` + `Content` (the box/Frame), shown lowest order first. `Left Button` / `Right Button` flip pages and are greyed out when there's no page that way. Optional `Page Label` shows e.g. "2 / 5". Goes back to page 1 each time the window opens. Already set up on the `Assets/UI/GeneralTileWindow` prefab (4 steps)
- `PlayerHUD.cs` - goes on PlayerUI, keeps the Health/Stamina/Hunger/Thirst sliders in sync and fills the objectives list from `Objectives`. H hides/shows the whole player UI (`Hide Key`)

## Scripts/Mobs/
- `MobData.cs` - a mob profile asset: `id`, `displayName`, `model`, `type` (Follow / Shooter), `health`, `attackDamage`, plus Follow settings (`moveSpeed`, `chaseRange`, `attackRange`, `attackCooldown`) and Shooter settings (`shootRange`, `fireRate`, `projectileModel`, `projectileSpeed`, `projectileSize`), and `drops` (loot: consumable + chance 0-1). Create > Game > Mob
- `Mob.cs` - base for every mob: health, health bar, dies at 0 (drops its loot and counts the kill for achievements), helpers for finding the player
  - `BossMob.cs` - the Plains boss (on `HexRoom/BossArena/Boss`, not spawned by a spawner). Walks at and keeps facing the player, picks a move after `Rest Between Moves`: club if close, otherwise jump slam or boulders. Takes `Shielded Damage` (0.2) of each hit while any pillar stands, full damage for `Weak Time` seconds once they're all broken. Shows `Portal Piece` when it dies. Profile: `Data/Mobs/Boss`
  - `BossPillar.cs` - a breakable pillar (`Max Health` 50) with a beam to the boss, shows a health bar (`Health Bar Prefab`) once it's hit, tells the boss when it breaks, `Rebuild()` grows it back
  - `Shockwave.cs` - the flat ring that spreads out when the boss lands
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
  - `Max Alive` (4) - most mobs from one spawner alive at the same time, it waits for one to die before spawning the next
  - `Health Bar Prefab` - put above each mob it spawns
  - Shows a red circle in the Scene view

| Mob | Type | Health | Attack Damage | Speed | Per spawner |
|---|---|---|---|---|---|
| Mob Level 1 | Follow | 100 | 10 / hit | 3.5 | 8 |
| Mob Level 2 | Follow | 125 | 15 / hit | 2.5 | 4 |
| Mob Level 3 | Shooter | 200 | 22 / shot | stays still | 2 |
| Plains Guardian (boss) | Boss | 1500 | 30 slam / 20 boulder / 25 club | 2.5 | 1 (HexRoom) |

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
  - `OnUnlocked` event for UI, progress saved per profile (`achievements_1.json` to `achievements_3.json`, `achievements.json` when testing a scene directly)
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

## Scripts/Hub/
- `ReadWritePlayerProfile.cs` - for the Library. Writes the saved profile out as text: level, tokens, health/stamina/hunger/thirst, equipped gear, owned weapons, inventory, and every achievement with progress (e.g. `[ ] Slayer - Kill 10 mobs (7/10)`)
  - Put it on the Library's Frame (e.g. a `GeneralTextWindow`), drag `Content/Title1` into `Title Text` and `Content/Context` into `Body Text`
  - Refreshes every time the Library opens
  - Title = player name, body = stats. If no profile was picked on the Main Menu (e.g. testing the Hub directly) it shows "Profile not loaded" / "Profile has not generated... something gone wrong :/"

## Scripts/World/
- `PortalPiece.cs` - spins and bobs the portal piece the boss drops (its `ShowUIonApproach` opens the pop-up). Walking onto it sets `HasPortalPiece` on the profile and saves
- `PortalSlot.cs` - on the Hub's `Level2Portal`. If the profile has the portal piece, walking up opens `PlacePiecePrompt` instead of the locked pop-up. Place turns the slot and the portal purple and saves `Portal2Open`. There's no Level 2 yet, so the open portal does nothing. Right-click the component for **Give Portal Piece (testing)**
- `RadiusUI.cs` (was `MenuZone`) - shows a UI when the player walks close, hides it when they walk away
  - `UI` - what to show, normally a menu's `Frame`. `Radius` - how close to open it. `Show Ring Within` - when the floor ring appears
  - The ring always lies flat and matches the radius, even under a rotated or scaled sign
  - Use it through the `Models/ShowUIonApproach` prefab (RadiusUI + floor ring + a flat blue marker disc half the ring's width showing where to stand). In the Hub, the Shop, IntroWall, ExitWall and Library signs each have one, and so do both portals (3.5 in front, opening `Scene1Prompt` / `Scene2Prompt`)
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
- `PortalSlotBuilder.cs` - adds **Tools > Add Portal Slots**. With the Hub open, puts a square slot on each portal's inner pillar (Level 1 purple, Level 2 dark grey), adds `PortalSlot` to `Level2Portal` and the `PlacePiecePrompt` pop-up (`GeneralPopUpWindow`). Editor only
- `BossRoomBuilder.cs` - adds **Tools > Build Boss Room**. With Scene 1 Plains open, builds the boss, pillars, portal piece, `PortalPieceMenu` and the `PrototypeSkip` pad, and makes `Data/Mobs/Boss.asset` and `Models/Boss/Boulder.prefab` if they don't exist yet. Editor only
- `HubBuilder.cs` - adds **Tools > Build Hub**. Builds the hub scene out of basic shapes, saves it as `Assets/Scenes/Hub.unity` and puts it first in Build Settings. Editor only, not part of the game

# Save Data
- Saved as JSON in Unity's persistent data folder, not in `Assets/` (Assets is read-only in a build)
- Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\GAMEDEV26\`
  - `profile_1.json` to `profile_3.json` - the 3 player profiles (name, passkey, ball colour, created, stats, tokens, owned items, item levels, current inventory)
  - `achievements_1.json` to `achievements_3.json` - each profile's achievement progress and unlocks
  - `profile.json` / `achievements.json` - used when a scene is played without picking a profile (testing)
  - The passkey is stored as plain text, it only stops someone clicking into the wrong profile
- Same location in the editor and in builds
- The save only stores item ids, so item stats/models can be changed without breaking saves
- To reset, delete the file or right-click **Player Stats > Delete Save** (resets progress, keeps the name, passkey and colour). Delete a whole profile from the Main Menu

# Weapons and Items
- Every item is an asset in `Assets/Data/`
- Full step-by-step guide: `AUTOMATIONS.md` > Adding Items
- To make a new one:
  - Right-click in Project > **Create > Game > Weapons > Shooter / Lazer / Melee / Special**, or **Create > Game > Consumable**
  - Give it a unique `id`, set its stats (and `model` for weapons)
  - Add it to the right list in `Assets/Resources/ItemDatabase.asset`, otherwise the game can't find it
  - New lazer? Also add it to the gun's `Lazers` list
- EM Raygun, Lazer and Punch are `unlocked` (every player starts with them), everything else must be bought

Upgrades: every weapon has 3 levels. Level 1 comes with the item, level 2 costs half its price, level 3 costs 1.5x its price (100-token item: 50, then 150). Refund = a third of the price (34), whatever the level, and the level goes back to 1.

EM Raygun levels (the lazer it fires):

| Level | Lazer | Damage | Shots/sec | Speed | Model |
|---|---|---|---|---|---|
| 1 | Lazer | 10 | 4 | 20 | `Lazer` |
| 2 | UV Lazer | 4 | 10 | 28 | `UV Lazer` |
| 3 | Gamme Lazer | 40 | 1 | 35 | `Gamme Lazer` |

Melee and specials: level 2 = 1.5x damage, level 3 = 2x damage, everything else the same (change per level in the asset).

| Melee | Damage (Lv 1 / 2 / 3) | Attacks/sec | Radius |
|---|---|---|---|
| Punch | 5 / 7.5 / 10 | 2 | 1.2 |
| Kick | 8 / 12 / 16 | 1.2 | 1.5 |
| Bat | 15 / 22.5 / 30 | 1 | 2 |

| Special | Damage (Lv 1 / 2 / 3) | Shots/sec | Speed | Explosion Radius |
|---|---|---|---|---|
| Rocket | 50 / 75 / 100 | 0.5 | 15 | 3 |
| Grenade | 35 / 52.5 / 70 | 0.8 | 8 | 5 |

| Consumable | Restores | Token Cost |
|---|---|---|
| Beans | Hunger +40 | 10 |
| Water Bottle | Thirst +50 | 10 |
| Soda | Thirst +25, Stamina +30 | 15 |

Every item has a `tokenCost` and `levelRequired`. Weapons and lazers default to 100 tokens and level 1. All lazers and specials travel 50 units.
