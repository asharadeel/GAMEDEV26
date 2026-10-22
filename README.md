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
    - `Lazers/` - `Lazer`, `UV Lazer`, `Gamme Lazer` (the EM Raygun's lazer bar: starting lazer, then 2 upgrades)
  - `Weapons/Melee/` - `Punch`, `Kick`, `Bat`
  - `Weapons/Specials/` - `Rocket`, `Grenade` (not in the game any more: abilities replaced specials, kept in case they come back)
  - `Abilities/` - `Fire`, `Ice`, `Lightning` (each holds its skill tree branch)
- `Fonts/` - Arial, used by all UI text
- `Images/` - 2D images (`pointer.png` - aim arrow, `circleborder.png` - door ring and prompt)
- `Materials/Palette/` - ready-to-use materials: `Colours` (23 flat colours), `Metals`, `Glow` (emissive), `Glass` (see-through), `Surfaces` (wood, stone, brick, ice, lava...)
- `Materials/` - materials for scene objects. `Level/` has the level materials (Floor, Grass, Path, SpawnPad, TreeTrunk, TreeLeaves, Door, HexFloor, HexWall). Others: (`Baseplate`, `Bullet`, `Gamme Lazer`, `MapWalls`, `Player`, `UV Lazer`, `Wall`)
- `Models/` - 3D models and prefabs
  - `ShowUIonApproach` - drop next to anything (e.g. a sign) and set its `UI`: opens that UI when the player walks close, with a ring and a blue marker disc on the floor
  - `RayGun/` - `Lazer`, `UV Lazer`, `Gamme Lazer`
  - `Mobs/` - mob models (`MobLvl1`, `MovLvl2`, `MobLvl3`)
  - `Abilities/` - `Fireball`, `FrostNova`, `LightningBolt` (what the abilities look like)
    - `Spawners/` - `MobSpawner` prefab
- `Resources/` - `ItemDatabase.asset`, the list of every item in the game
- `Scenes/` - all game scenes (`Hub`, `Main Menu`, `Scene 1 Plains`)
- `Scripts/` - all C# code
  - `Achievements/` - achievement list, progress tracking, panel and popup
  - `Editor/` - editor-only tools (not part of the game)
  - `Generic/` - small reusable scripts that can go on any object
  - `Hub/` - scripts only used in the Hub
  - `Items/` - item base class, consumables, item database
  - `Loot/` - mob drops, pickups and floating text
  - `Abilities/` - abilities, using them, and the skill tree
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
  - `AbilityCard` - one ability in the Hub's Inventory (Name, "On Q"/"On E", Equip Q and Equip E buttons + `AbilityCard`)
  - `SkillNode` - one circle in the skill tree, made by `SkillTreeUI`
  - `InventoryCard` - one weapon in the Hub's Inventory (Name, Equip/Unequip button + `InventoryCard`). Made from `ShopItemCard`'s look. Place one per weapon by hand in the inventory grid and drag the weapon asset into `Item`
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
- Left side: Shop, right side: Inventory (it used to be the Library), east wall: Skill Tree. Stand on the ring and it opens the thing you are standing on
- `IntroMenu`, `StoreUI`, `InventoryMenu`, `SkillTreeMenu` are under `Canvas`
- `InventoryMenu`: a `GeneralSelectorUI` copy titled "Inventory" with 3 tabs like the shop: Shooters (EM Raygun), Melee (Punch, Kick, Bat), Abilities (Fire, Ice, Lightning). Buttons 4 and 5 are switched off. Each tab's `Viewport/Content` has a heading, a line of text and an `Items` grid (`Grid Layout Group` 300 x 400 cells + `InventoryGrid`). The weapon tabs have `InventoryCard`s (only owned weapons show, Equip / Unequip), the Abilities tab has `AbilityCard`s (Equip Q / Equip E)
- `Skill Tree` stall (east wall, x 14.8, with `SkillTreeLight`): same as the other stalls (sign + `ShowUIonApproach`), opens `SkillTreeMenu`: a full-screen `GeneralPopUpWindow` copy (`SkillTreeWindow`, action button off) with a coins line and 3 columns, `Branch (Fire)`, `Branch (Ice)`, `Branch (Lightning)`, each a `Header` and a `Tree` area. `SkillTreeUI` on the container draws the nodes and lines into the `Tree` areas
- `StoreUI > GeneralSelectorUI`: the shop window, 5 section buttons that each show their own frame (`TabSelector`). Tabs in order: Shooters (EM Raygun), Melee (Punch, Kick, Bat) as `ShopItemCard`s, Specials (button switched off, abilities replaced specials), Food (Beans), Drinks (Water Bottle, Soda) as `ConsumableCard`s, each in the section's `Items` grid. The Shooters tab's frame is still called `Weapons`
- `BuyPopup`, `RefundPopup`: the shop's pop-ups (GeneralPopUpWindow), run by `ShopPopups` on `StoreUI`
- `UpgradePopup`: the weapon upgrade window (a GeneralPopUpWindow stretched full screen, same size as the shop window), run by `WeaponUpgradeUI` on the container. Opened by Upgrade on any weapon's card. Shooters get one bar per upgrade (Lazer, Damage, Fire Rate, Bullet Speed, Reload Time), melee and specials just a Level bar (2 blocks: levels 2 and 3). Bars are under `Frame/Bars`: label, blocks that fill left to right, "now → next" value and a Buy button that opens `BuyPopup`. Sits right after `StoreUI` (over the shop, under `BuyPopup`). Opening it hides the shop, closing it brings the shop back on the same tab. Built by **Tools > Build Weapon Upgrades** (it replaced the old "Upgrade to level n" pop-up)
- The shop window's title says "Shop"
- `Scene1Prompt`: pop-up "Enter Scene 1?" (from `GeneralPopUpWindow`), Play loads Scene 1 Plains
- `IntroMenu` uses the `GeneralTextWindow` prefab as its Frame: welcome title, how to play, controls, mobs and health
- `IntroMenu`, `InventoryMenu` and `LockedMessage` follow the UI POLICY (container > `Frame` > `Panel`, see `AUTOMATIONS.md`). `ShopMenu` doesn't yet, it's being worked on separately
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
- Esc menu (`Canvas > EscMenu`): `Frame` (dim overlay) > `Panel`, styled like the pop-up windows: dark box with a "PAUSED" title bar, a grey RETURN TO MENU button (loads the Hub) and a dark CLOSE bar along the bottom
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
| 1 / 2 | Select shooter / melee |
| Q / E | Use the ability on that key (picked in the Hub's Inventory) |
| R | Reload the shooter (it also reloads by itself when empty) |
| 4-9, 0 | Eat / drink from that inventory slot |
| I | Open / close inventory |
| J | Open / close achievements |
| E / Click prompt | Use a door when in range (E opens the door instead of the ability) |
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
  - Level, Coins (new profiles start with 150; still called `tokens` in the save file so old saves keep their balance), Speed, and Health, Stamina, Hunger, Thirst (all 0-100)
  - `Owned Items` - everything the player has bought
  - `Current Inventory` - what they have on them right now: shooter, lazer, melee, the abilities on Q and E (`abilityQ` / `abilityE`, Fire and Ice to start) and the food/drink slots (`special` is no longer used).
  - `Skill Nodes` - the skill tree nodes bought, e.g. `"fire.2"`.
  -  Only owned items can be equipped
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
  - The shooter fires its `magazineSize` shots, then reloads for its reload time (R reloads early). Damage, fire rate, bullet speed and reload time come from the gun's upgrade bars, the look, size and pierce from its lazer. `AmmoLabel()` gives "12 / 20" / "Reloading..." for the HUD
  - 1/2 select shooter/melee, R reloads the shooter. `AimDirection` and `ShotOrigin()` are shared with `PlayerAbilities`
  - `Aim Arrow` - flat sprite on the floor that points at the mouse (`Arrow Distance`, `Arrow Height`, `Arrow Spin`)
  - While melee is selected and the equipped melee is Punch, a capsule `Fist` sits in front of the player pointing where they aim and jabs forward on each punch (`Fist Colour`)
  - Right-click the component for **Unlock All Weapons (testing)**
- `PlayerInventory.cs` - goes on the Player
  - I opens/closes `Inventory Panel`
  - 10 slots: 1 shooter, 2 melee, the Q / E abilities, 4-9 and 0 food/drink
  - Pressing 4-9 or 0 eats/drinks that slot
  - `Slot Labels` - one Text per slot, filled in while the panel is open (selected weapon shows yellow)

## Scripts/Weapons/
All weapons are assets (see Weapons and Items below).
- `WeaponData.cs` - shared by all weapon gear: `unlocked` (starts owned), `model`
- `WeaponData.cs` - base for all weapons: `unlocked`, `model`, and the prices for melee/specials: upgrade to level 2 = half the `coinCost`, level 3 = 1.5x (rounded up), refund = a third of the `coinCost` (rounded up, upgrades aren't refunded). Max level 3
- `ShooterData.cs` - slot 1 gun (EM Raygun). No levels: 5 upgrade bars (`ShooterStat`: Lazer, Damage, FireRate, BulletSpeed, ReloadTime), each bought on its own. `Damage`, `Fire Rate`, `Bullet Speed`, `Reload Time` each have `start` (value with 0 upgrades), `perLevel` (added per upgrade, negative for reload) and `firstCost` (upgrade n costs n x firstCost), 5 upgrades each. The `Lazers` list is the lazer bar: 1st = starting lazer, each next one costs its own `coinCost`. `magazineSize` = shots before reloading
- `LazerData.cs` - what a gun fires: `model`, `distance` (50), `size` (bigger = easier to hit), `pierce` (how many enemies it goes through), `rotationOffset`. `PerkText` describes it for the shop ("Bigger shots, goes through 2 enemies")
- `MeleeData.cs` - slot 2, hits everything within `radius` in front of the player. 3 `Levels`, each with `model`, `damage`, `fireRate`, `radius`
- `SpecialData.cs` - not used any more (abilities replaced specials). Was slot 3, fires a projectile that explodes. 3 `Levels`, each with `model`, `damage`, `fireRate`, `speed`, `radius`, `distance` (50), `size`, `rotationOffset`
- `Damage.cs` - shared helpers: `Damage.Hit` (one target) and `Damage.HitArea` (everything in a radius, each mob once)
- `Projectile.cs` - `Projectile.Spawn(...)` fires a model as a shot (used by the player and shooter mobs)
  - Player shots hit everything except the player. Enemy shots hit only the player and pass through mobs
  - Flies straight, disappears on hit or after travelling its `distance`
  - Calls `TakeDamage(float)` on whatever it hits (mobs lose health), specials hit everything in their radius

## Scripts/Items/
- `ItemData.cs` - base for every item: `id` (used in the save), `displayName`, `coinCost` (100, assets with the old `tokenCost` still load), `levelRequired` (1)
- `ConsumableData.cs` - food/drink: restores `hunger`, `thirst`, `stamina`, `health`
- `ItemDatabase.cs` - every item in the game, split into lists: shooters, lazers, melee, specials (empty now), food, drinks, abilities
  - The save only stores ids (e.g. `"lazer"`), the game uses this to turn an id back into the item asset
  - The asset must be at `Assets/Resources/ItemDatabase.asset`

## Scripts/Save/
- `SaveSystem.cs` - saves/loads any `[System.Serializable]` class as JSON
  - `SaveSystem.Save("name", data)`, `SaveSystem.Load("name", fallback)`, `SaveSystem.Exists("name")`, `SaveSystem.Delete("name")`
- `ProfileManager.cs` - the current player profile, usable from any script in any scene
  - `ProfileManager.Profile.Coins += 50;`
  - 3 slots (`profile_1.json` to `profile_3.json`). The Main Menu picks one with `LoadSlot`. Playing a scene directly (testing) uses the old `profile.json` instead, `HasProfile` is false then
  - `CreateProfile`, `EditProfile` (name + colour), `DeleteSlot` (also deletes its achievements), `PeekSlot` (read a slot without switching)
  - Loads the profile the first time it's used, saves when the game closes or the scene changes
  - `TryBuyWeapon`, `TryBuyConsumable`, `TryBuyInventorySlot` (check coins/level/space)
  - `GetLevel`, `CanUpgrade` / `TryUpgrade`, `CanRefund` / `TryRefund` (not starting items, not while equipped), `IsEquipped`, `Equip` / `Unequip` (Unequip empties the slot)
  - Shooters: `GetLevel(shooter, stat)` (0-5), `GetStat(shooter, stat)` (the value with upgrades), `CanUpgrade(shooter, stat)` / `TryUpgrade(shooter, stat)`
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
- `AbilityCard.cs` - on the `AbilityCard` prefab. Drag an ability into `Item`. Shows its name, "On Q" / "On E" (fixed height like `InventoryCard`) and Equip Q / Equip E buttons. Equipping moves it if it was on the other key, pressing its own key's button again unequips it
- `InventoryCard.cs` - on the `InventoryCard` prefab. Drag a weapon asset into `Item`. Shows its name, type (Shooter / Melee / Special) and a green "Equipped" label (`Equipped Tag`, at a fixed height so it lines up across cards), with one Equip / Unequip button. Equipping replaces whatever was in that slot, Unequip leaves the slot empty
- `InventoryGrid.cs` - on the grid holding the `InventoryCard`s. Each time the window opens it hides the cards for weapons the player doesn't own (so bought weapons appear, refunded ones disappear)
- `ShopItemCard.cs` - on the `ShopItemCard` prefab. Drag a weapon asset into `Item`. Shows "<name>" and "Level <n>" (shooters just the name), plus Buy (not owned) or Equip + Upgrade + Refund (owned). Upgrade opens `WeaponUpgradeUI`, Refund is hidden for starting items and greyed out while equipped
- `ConsumableCard.cs` - on the `ConsumableCard` prefab. Drag a food/drink asset into `Item`. Shows its name, Buy, Refund and "Amount owned: <n>" (how many are in the inventory slots). Buying puts one in the first free slot. Refund sells one back for half its price (rounded up), greyed out when you have none
- `ShopPopups.cs` - on the Hub's `StoreUI` container. Runs the shared `BuyPopup` ("Buy <item>", button "Buy: <cost>", also used for every upgrade: "Buy Damage (10 → 13)" / "Buy UV Lazer" / "Buy Punch Level 2") and `RefundPopup` ("Refund <item>: <refund>"). Shows the player's coins, or why it didn't work. Buying food/drink keeps the pop-up open so you can buy several
- `WeaponUpgradeUI.cs` - on the Hub's `UpgradePopup` container. `Open(weapon)` shows "Upgrade <weapon>", the player's coins and its bars: shooters one `UpgradeBar` per `ShooterStat`, melee/specials only the 1st bar as "Level" with what the next level changes (e.g. "Level 1 → 2: Damage 5 → 7.5"), the other rows hide. Buy opens `BuyPopup` for that bar's next upgrade, MAX when full. `Shop` (the shop window) is hidden with a CanvasGroup while it's open, not switched off, so it comes back on the same tab however the window is closed. Blocks past a bar's max hide. `Filled` / `Empty` set the block colours
- `CurrencyChangeButton.cs` - on a Button: `Mode` Add / Subtract and `Amount` of coins. Subtract doesn't go through without enough coins
- `TabSelector.cs` - a row of buttons that each open their own frame. `Tabs` = list of `Button` + `Frame`. Clicking a button shows its frame and hides the rest, the open tab's button is greyed out. Opens on the first tab each time the window opens. On the Hub's `StoreUI/GeneralSelectorUI` (5 buttons under `SectionSelector`, 5 frames under `Sections`)
- `TilePager.cs` - shows one box (page) at a time instead of a scroll view. `Pages` = list of `Order` + `Content` (the box/Frame), shown lowest order first. `Left Button` / `Right Button` flip pages and are greyed out when there's no page that way. Optional `Page Label` shows e.g. "2 / 5". Goes back to page 1 each time the window opens. Already set up on the `Assets/UI/GeneralTileWindow` prefab (4 steps)
- `PlayerHUD.cs` - goes on PlayerUI, keeps the Health/Stamina/Hunger/Thirst sliders in sync, shows the shooter's ammo in `Ammo Text` (Scene 1: `PlayerUI/Frame/AmmoCounter`, bottom right, blank when another weapon is selected) and the Q / E cooldowns in `Ability Text` (`PlayerUI/Frame/AbilityCooldowns`, above the ammo) and fills the objectives list from `Objectives`. H hides/shows the whole player UI (`Hide Key`)

## Scripts/Abilities/
- `AbilityData.cs` - an ability (Create > Game > Ability): `type` (Fire / Ice / Lightning), `colour`, `model`, base `damage`, `radius`, `cooldown`, `extra` (Ice: slow seconds, Lightning: chains) and the skill tree `nodes`
- `PlayerAbilities.cs` - on Scene 1's Player. Q / E use the equipped abilities, each with its own cooldown. Stats are the ability's plus its unlocked nodes. `SlotLabel(slot)` gives "Q  Fire  Ready" for the HUD
- `SkillTreeUI.cs` - on the Hub's `SkillTreeMenu` container. Each time the window opens it shows every branch: unlocked nodes in the ability's colour, buyable ones white with a +, locked ones dark grey. Lines light up when the node above is unlocked. Clicking a buyable node opens `BuyPopup`
- `SkillNodeUI.cs` - on the `SkillNode` prefab (`Assets/UI/SkillNode`): the circle, its icon and the title under it

## Scripts/Mobs/
- `MobData.cs` - a mob profile asset: `id`, `displayName`, `model`, `type` (Follow / Shooter), `health`, `attackDamage`, plus Follow settings (`moveSpeed`, `chaseRange`, `attackRange`, `attackCooldown`) and Shooter settings (`shootRange`, `fireRate`, `projectileModel`, `projectileSpeed`, `projectileSize`), and `drops` (loot: consumable + chance 0-1). Create > Game > Mob
- `Mob.cs` - base for every mob: health, health bar, dies at 0 (drops its loot and counts the kill for achievements). `Slow(multiplier, seconds)` slows it (Ice), `FollowMob` and `BossMob` multiply their speed by `SpeedMultiplier`, helpers for finding the player
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
- `ReadWritePlayerProfile.cs` - was the Library's script (not in any scene since the Library became the Inventory, kept in case it's wanted again). Writes the saved profile out as text: level, coins, health/stamina/hunger/thirst, equipped gear, owned weapons, inventory, and every achievement with progress (e.g. `[ ] Slayer - Kill 10 mobs (7/10)`)
  - Put it on a window's Frame (e.g. a `GeneralTextWindow`), drag `Content/Title1` into `Title Text` and `Content/Context` into `Body Text`
  - Refreshes every time the window opens
  - Title = player name, body = stats. If no profile was picked on the Main Menu (e.g. testing the Hub directly) it shows "Profile not loaded" / "Profile has not generated... something gone wrong :/"

## Scripts/World/
- `PortalPiece.cs` - spins and bobs the portal piece the boss drops (its `ShowUIonApproach` opens the pop-up). Walking onto it sets `HasPortalPiece` on the profile and saves
- `PortalSlot.cs` - on the Hub's `Level2Portal`. If the profile has the portal piece, walking up opens `PlacePiecePrompt` instead of the locked pop-up. Place turns the slot and the portal purple and saves `Portal2Open`. There's no Level 2 yet, so the open portal does nothing. Right-click the component for **Give Portal Piece (testing)**
- `RadiusUI.cs` (was `MenuZone`) - shows a UI when the player walks close, hides it when they walk away
  - `UI` - what to show, normally a menu's `Frame`. `Radius` - how close to open it. `Show Ring Within` - when the floor ring appears
  - The ring always lies flat and matches the radius, even under a rotated or scaled sign
  - Use it through the `Models/ShowUIonApproach` prefab (RadiusUI + a red floor ring + a flat blue marker disc half the ring's width showing where to stand). In the Hub, the Shop, IntroWall, ExitWall and Inventory signs each have one, and so do both portals (3.5 in front, opening `Scene1Prompt` / `Scene2Prompt`)
- `HubPortal.cs` - goes on a portal with a trigger collider
  - Walking in loads `Scene Name` (must be in Build Settings)
  - If `Locked`, shows `Locked Message` for `Show Message For` seconds instead
- `WallHider.cs` - goes on `CaveWalls` in the hub. Hides the walls between the camera and the player so they don't block the view (they still have colliders and shadows)
- `DoorInteract.cs` - goes on a door. `DoorInteract.PlayerNearDoor` is true while the player can use one (then E opens the door, not the E ability)
  - When the player is within `Radius` of `Interact Point`: shows the on-screen `Prompt` (circle with "E" / "Click") above the door
  - Press `Key` (E) or click the prompt to move the player to `Destination`
  - `Radius Ring` - circle on the floor showing the radius, visible within `Show Ring Within`
  - The end-room door sends the player to `HexRoom/DoorSpawn`

## Scripts/Generic/
- `FaceCamera.cs` - keeps a label turned towards the camera (used for the names above the portals and stalls). With `Face Player Within` above 0, while the player is that close it smoothly turns to point straight at the camera (which follows the player), so it turns as they walk around it and always reads square-on; it turns back when they leave (`Turn Speed`). The SHOP, INVENTORY and SKILL TREE signs use 10
- `InactiveOnStart.cs` - turns its object off when the scene starts. Leave the object switched on in the editor, otherwise this won't run until it's first enabled

## Scripts/Editor/
- `ReloadChangedScenes.cs` - reloads the open scene when its file changes on disk (a `git pull`, or edited outside Unity), including after a recompile or leaving Play mode, when Unity would otherwise keep showing its own old copy. Asks first if the scene has unsaved changes. Nothing to set up. Editor only
- `WeaponUpgradeBuilder.cs` - adds **Tools > Build Weapon Upgrades**. Opens the Hub, sets the shop title to "Shop" and builds `UpgradePopup` (removing the old one), then opens Scene 1 Plains and adds `AmmoCounter` (wired into `PlayerHUD`). Saves both. Running it again replaces them. Editor only
- `PortalSlotBuilder.cs` - adds **Tools > Add Portal Slots**. With the Hub open, puts a square slot on each portal's inner pillar (Level 1 purple, Level 2 dark grey), adds `PortalSlot` to `Level2Portal` and the `PlacePiecePrompt` pop-up (`GeneralPopUpWindow`). Editor only
- `BossRoomBuilder.cs` - adds **Tools > Build Boss Room**. With Scene 1 Plains open, builds the boss, pillars, portal piece, `PortalPieceMenu` and the `PrototypeSkip` pad, and makes `Data/Mobs/Boss.asset` and `Models/Boss/Boulder.prefab` if they don't exist yet. Editor only
- `HubBuilder.cs` - adds **Tools > Build Hub**. Builds the hub scene out of basic shapes, saves it as `Assets/Scenes/Hub.unity` and puts it first in Build Settings. Editor only, not part of the game

# Save Data
- Saved as JSON in Unity's persistent data folder, not in `Assets/` (Assets is read-only in a build)
- Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\GAMEDEV26\`
  - `profile_1.json` to `profile_3.json` - the 3 player profiles (name, passkey, ball colour, created, stats, coins (saved as `tokens`), owned items, item levels, shooter upgrades, skill tree nodes, current inventory incl. abilities on Q/E)
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

Upgrades (melee and specials): 3 levels, bought on the Level bar in the upgrade window. Level 1 comes with the item, level 2 costs half its price, level 3 costs 1.5x its price (100-coin item: 50, then 150). Refund = a third of the price (34), whatever the level, and the level goes back to 1.

Shooters have upgrade bars instead, each bought on its own in the shop (Upgrade on the card). Refunding a shooter resets its bars. The refund is always a third of the gun's price: upgrades never change it and aren't refunded. EM Raygun (magazine: 20 shots):

| Bar | Start | Per upgrade | Max (5 upgrades) | Upgrade costs |
|---|---|---|---|---|
| Damage | 10 | +3 | 25 | 20, 40, 60, 80, 100 |
| Fire Rate (shots/sec) | 4 | +0.8 | 8 | 20, 40, 60, 80, 100 |
| Bullet Speed | 20 | +4 | 40 | 15, 30, 45, 60, 75 |
| Reload Time (sec) | 2 | -0.3 | 0.5 | 15, 30, 45, 60, 75 |

| Lazer bar | Cost | Perk | Model |
|---|---|---|---|
| Lazer | starting | - | `Lazer` |
| UV Lazer | 100 | Bigger shots (size 1.5) | `UV Lazer` |
| Gamme Lazer | 200 | Bigger shots, goes through 2 enemies | `Gamme Lazer` |

Melee and specials: level 2 = 1.5x damage, level 3 = 2x damage, everything else the same (change per level in the asset).

| Melee | Damage (Lv 1 / 2 / 3) | Attacks/sec | Radius |
|---|---|---|---|
| Punch | 5 / 7.5 / 10 | 2 | 1.2 |
| Kick | 8 / 12 / 16 | 1.2 | 1.5 |
| Bat | 15 / 22.5 / 30 | 1 | 2 |

Rocket and Grenade (specials) are out of the game: abilities replaced them. Their assets are kept in `Data/Weapons/Specials/`.

# Abilities and Skill Tree
Every player has Fire, Ice and Lightning for free at their base stats. Put one on Q and one on E in the Hub's Inventory (Abilities tab). The Skill Tree stall upgrades them with coins.

| Ability | What it does | Damage | Radius | Cooldown | Extra |
|---|---|---|---|---|---|
| Fire | Fireball towards the mouse, explodes on what it hits | 25 | Blast 3 | 6s | - |
| Ice | Frost nova around you, hurts and slows mobs (half speed) | 10 | 5 | 8s | Slow 2s |
| Lightning | Hits the nearest mob in range, jumps to more (half the range each jump). Nothing in range = no cooldown | 20 | Range 10 | 5s | Chains 2 |

Skill tree: each ability's branch starts at the ability (free) and splits into two paths of 3 nodes. A node can only be bought once the node below it is unlocked. Costs 40, 70, 120 along each path.

| Ability | Left path | Right path |
|---|---|---|
| Fire | Hotter Flames +10 Damage, Searing +10 Damage, Inferno +20 Damage | Wider Blast +1 Blast, Quick Cast -1s Cooldown, Firestorm +1.5 Blast |
| Ice | Chill +1s Slow, Frostbite +10 Damage, Deep Freeze +2s Slow | Wide Frost +1.5 Radius, Cold Snap -1.5s Cooldown, Blizzard +2 Radius |
| Lightning | Charged +8 Damage, High Voltage +8 Damage, Overcharge +14 Damage | Arc +1 Chain, Quick Strike -1s Cooldown, Storm +2 Chains |

New node? Add it to the ability asset's `Nodes` list (`title`, `stat`, `amount`, `cost`, `parent` = index of the node below it or -1 for the ability, `column` -1/0/1, `row` 1-3). The tree draws itself from that list.

| Consumable | Restores | Coin Cost |
|---|---|---|
| Beans | Hunger +40 | 10 |
| Water Bottle | Thirst +50 | 10 |
| Soda | Thirst +25, Stamina +30 | 15 |

Every item has a `coinCost` and `levelRequired`. Weapons and lazers default to 100 coins and level 1. All lazers travel 50 units.
