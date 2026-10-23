# Professor Trayarous

A top-down survival shooter made in Unity for GAMEDEV26. Fight through levels of mobs, keep your hunger and thirst up, beat the boss and collect portal shards. Between runs, you upgrade and gear up in the Hub.

## Group Members

- Ashar Adeel 
- Ali Habib
- Davud Jabiyev 
- Elias El Harrak Guiuo-Guiuo

## How to Run

1. Install **Unity 2023.1.22f1** through Unity Hub. Other versions may change or break the project.
2. Clone the repo:
   ```
   git clone https://github.com/asharadeel/GAMEDEV26.git
   ```
3. In Unity Hub, click **Add > Add project from disk** and pick the cloned folder.
4. Open `Assets/Scenes/Main Menu.unity` and press **Play**.
5. Click **Play**, make a profile, and you'll start in the Hub.

To build the game, use **File > Build Settings**. The scenes are already in order: Main Menu, Hub, Scene 1 Plains.

## How to Play

- **Main Menu:** make or pick one of 3 save profiles. The Tutorial explains the basics.
- **Hub:** buy weapons, food and armour in the Shop. Equip gear and pack food in the Inventory. Craft armour at the Blacksmith and upgrade abilities in the Skill Tree. Walk into the Level 1 portal to start a run.
- **Scene 1 Plains:** get through the maze, kill mobs for coins, food and ores, then beat the boss to get a portal shard. If you die you keep what you picked up, and you can go back to the Hub or restart the level.

### Controls

- **WASD** - Move
- **Shift** - Sprint
- **Space** - Jump
- **Mouse** - Aim
- **Left Click** - Attack
- **Right Click + drag / Scroll** - Rotate / zoom camera
- **1 / 2** - Shooter / melee
- **Q / E** - Abilities
- **R** - Reload
- **4-9** - Eat / drink from the backpack
- **I / J** - Inventory / achievements
- **F** - Flashlight
- **H** - Hide UI
- **Esc** - Menu

## Project Docs

- `PROJECT_NOTES.md` covers how everything in the project works: folders, scenes, every script and the item and mob tables.
- `AUTOMATIONS.md` has step-by-step guides for repeating work we've already done once, such as adding items, mobs and menus, so new content is quick to add and stays consistent.

The game uses only Unity's built-in assets and the Input System package.

## AI Declaration

We used **Claude** (by Anthropic), mostly through **Claude Code**, to help build this project. The game's design, features and final decisions were ours. We tested and reviewed what the AI produced, and kept or changed it as needed.

**What we used it for:**
- **Documentation:** `PROJECT_NOTES.md` and `AUTOMATIONS.md` are AI-written, and the AI kept them up to date as we changed the project.
  - `PROJECT_NOTES.md` explains how the project works: the folder layout, what is in each scene, what every script does and how to set it up, the controls, and the tables of items, weapons, mobs, loot, abilities and achievements. It's how we kept up with each other's changes.
  - `AUTOMATIONS.md` is there to save time on repetitive work. We did the hard part ourselves: building the objects, scripts and prefabs. After that, filling in new content (new items, weapons, food, mobs, menus) is tedious, so this file gives the AI step-by-step guides for repeating tasks we have already done once. It also sets the rules for doing them (for example the UI policy and which prefabs to use) and a checklist for each, so new content comes out the same as what we built.
- **Scene and prefab setup:** building and wiring UI in the scene and prefab files, for example menus, pop-ups and inventory tabs.
- **Code help:** writing and fixing scripts in our own code style.
  - Scripts written with Claude are marked in their header (`ai use: ...`) along with the group member who owns them, for example the move to the Input System package (`GameInput.cs`) and `RingLine.cs`.
- **Fixing problems:** debugging, merge conflicts and cleaning up.
