# Professor Trayaurus (prototype)

ECS657U Coursework 1, group Survival_1.

A 3D top-down roleplaying game. You start in a cave, step through a portal into the first level, and fight your way through a maze full of mobs while keeping your hunger and thirst up. Beat the boss to win a portal piece that opens the next level. Between runs, you upgrade and gear up in the Hub.

**Play in the browser:** [WebGL build](ADD-GITHUB-PAGES-LINK)

## Group Members

- Ali Habib
- Ashar Adeel
- Elias El Harrak Guiou-Guiou
- Davud Jabiyev

## How to Run

1. Install **Unity 2023.1.22f1** through Unity Hub. Other versions may change or break the project.
2. Clone the repo with Git. Don't download a ZIP, because GitHub's ZIP leaves out the Git LFS files.
   ```
   git clone https://github.com/asharadeel/GAMEDEV26.git
   ```
3. In Unity Hub, click **Add > Add project from disk** and pick the cloned folder.
4. Open `Assets/Scenes/Main Menu.unity` and press **Play**.
5. Click **Play**, make a profile, and you'll start in the Hub.

To build the game, use **File > Build Settings**. The scenes are already in order: Main Menu, Hub, Scene 1 Plains.

## How to Play

1. **Main Menu:** make or pick one of 3 save profiles, then press Play. The Tutorial explains the basics.
2. **Hub:** walk around the cave and stand on a ring to open that feature.
   - Buy weapons, food and armour in the Shop.
   - Equip gear and pack food into your backpack in the Inventory.
   - Craft armour at the Blacksmith and upgrade abilities in the Skill Tree.
   - Walk into the Level 1 portal to start.
3. **The Plains:** follow the path into the maze. Mobs spawn as you get close and drop coins, food and ores. Eat and drink to stop your health draining.
   - If you die, you keep what you picked up, and you can go back to the Hub or restart the level.
4. **Boss:** walk up to the end room door and press Enter on the pop-up to go to the boss arena.
   - While any of the 6 purple pillars stands, the boss only takes 20% damage.
   - Break every pillar to drop its shield, then hit it before the pillars grow back.
   - The boss has different attacks for different ranges.
5. **Win:** pick up the portal piece, go back to the Hub and place it in the Level 2 portal. Level 2 isn't built yet, so the portal opens but doesn't do anything.

## Controls

- **WASD** - Move
- **Shift** - Sprint (uses stamina)
- **Space** - Jump
- **Mouse** - Aim
- **Left Click / X** - Attack
- **Right Click + drag** - Rotate camera
- **Scroll** - Zoom
- **1 / 2** - Shooter / melee
- **R** - Reload
- **Q / E** - Abilities (picked in the Hub inventory)
- **4-9** - Eat / drink from that backpack slot (0 shows your armour)
- **E** - Use a door when close
- **I / J** - Inventory / achievements
- **F** - Flashlight
- **H** - Hide UI
- **Esc** - Menu

Controls use the Input System package, all set up in `Assets/Scripts/Input/GameInput.cs`. A gamepad works for the main actions.

## Project Structure

- `Assets/Scenes`: Main Menu, Hub, Scene 1 Plains
- `Assets/Scripts`: grouped by system (Player, Weapons, Mobs, Abilities, Items, Loot, Save, UI, World, Input, Editor)
- `Assets/Data`: ScriptableObjects for items, weapons, mobs and abilities
- `Assets/Models`, `Assets/UI`, `Assets/Materials`: prefabs, reusable UI windows and the shared material palette

Only Unity's built-in assets are used: primitives, the built-in font and built-in UI sprites.

## Project Docs

- `PROJECT_NOTES.md` covers how everything in the project works: folders, scenes, every script and the item and mob tables.
- `AUTOMATIONS.md` has step-by-step guides for repeating work we've already done once, such as adding items, mobs and menus, so new content is quick to add and stays consistent.

## AI Declaration

We used **Claude** (by Anthropic), mostly through **Claude Code**, to help build this project. The game's design, features and final decisions were ours. We tested and reviewed what the AI produced, and kept or changed it as needed. Some code was generated with AI. It is declared at the top of each script or in the table below, and the owner of each piece of code can explain how it works.

**What we used it for:**
- **Documentation:** `PROJECT_NOTES.md` and `AUTOMATIONS.md` are AI-written, and the AI kept them up to date as we changed the project.
  - `PROJECT_NOTES.md` explains how the project works: the folder layout, what is in each scene, what every script does and how to set it up, the controls, and the tables of items, weapons, mobs, loot, abilities and achievements. It's how we kept up with each other's changes.
  - `AUTOMATIONS.md` is there to save time on repetitive work. We did the hard part ourselves: building the objects, scripts and prefabs. After that, filling in new content (new items, weapons, food, mobs, menus) is tedious, so this file gives the AI step-by-step guides for repeating tasks we have already done once. It also sets the rules for doing them (for example the UI policy and which prefabs to use) and a checklist for each, so new content comes out the same as what we built.
- **Scene and prefab setup:** building and wiring UI in the scene and prefab files, for example menus, pop-ups and inventory tabs.
- **Code help:** debugging and assistance on the more complicated scripts. Scripts where AI was used are marked in their header (`ai use: ...`) along with the group member who owns them.
- **Fixing problems:** debugging, merge conflicts and cleaning up.

| Files | Tool | How it was used | Owner |
|---|---|---|---|
| `Input/GameInput.cs`, `Editor/RingLine.cs`, `Editor/BossRoomBuilder.cs`, `Editor/PortalSlotBuilder.cs`, `Mobs/BossMob.cs`, `Mobs/BossPillar.cs`, `Mobs/Shockwave.cs`, `World/PortalPiece.cs`, `World/PortalSlot.cs`, `World/HubPortal.cs`, `World/WallHider.cs`, `World/RadiusUI.cs` (started as MenuZone), `Generic/FaceCamera.cs` | Claude (Anthropic) | Helped debug code and gave examples of code doing similar tasks, used as a blueprint | Elias El Harrak Guiou-Guiou |
| Input System changes in `PlayerController`, `PlayerCombat`, `PlayerInventory`, `PlayerHUD`, `EscapeMenu`, `DoorInteract`, `AchievementPanel`, `PlayerAbilities`; punch fist in `PlayerCombat`; portal fields in `PlayerProfile`; Max Alive in `MobSpawner` | Claude (Anthropic) | Edits to existing scripts to check for errors and to optimise movement mechanics and features | Elias El Harrak Guiou-Guiou |
| Data files in `Assets/Data/` (weapons, lazers, food and drinks, ores, armour, mobs) and `Assets/Resources/ItemDatabase.asset` | Claude (Anthropic) | Ashar decided which items and mobs to add and gave every value (prices, damage, health, drop chances and so on). Claude made the asset files from those values and registered them in the item database, following `AUTOMATIONS.md` | Ashar Adeel |
| `Player/PlayerArmor.cs`, `UI/TilePager.cs` | Claude (Anthropic) | Debugging and assistance on the more complicated parts. The design and logic are Ashar's | Ashar Adeel |
| `UI/BackpackBox.cs`, `World/SessionTracker.cs`, `Loot/OreDropper.cs`, `Hub/HubProgress.cs` | Claude (Anthropic) | A little help when stuck or confused on parts of these. The design and logic are Ashar's | Ashar Adeel |
| Scenes and UI prefabs (Hub, Scene 1 Plains, Main Menu, `Assets/UI/` windows and cards) | Claude (Anthropic) | Building and wiring UI and scene objects to Ashar's instructions: menus, pop-ups, inventory tabs, the Esc menu, the death screen, lights and drop models | Ashar Adeel |
| `PROJECT_NOTES.md`, `AUTOMATIONS.md` | Claude (Anthropic) | AI-written docs, kept up to date as the project changed (see above) | Ashar Adeel |
| `Abilities/SkillTreeUI.cs`, skill tree nodes in `Data/Abilities/` (Fire, Ice, Lightning) | Claude (Anthropic) | Debugging and assistance cloning and repeating the skill tree data across the three branches. The core logic was made by Ali | Ali Habib |
