# Professor Trayaurus (prototype)

ECS657U Coursework 1, group Survival_1: Ali Habib, Ashar Adeel, Elias El Harrak Guiou-Guiou, Davud Jabiyev.

A 3D top-down roleplaying game where you start in a cave step through a portal into the the first level/world fight your way through a maze full of mobs while keeping your hunger and thirst up then beat the boss to win a portal piece that opens the next level.

**Play in the browser:** [WebGL build](ADD-GITHUB-PAGES-LINK)


## How to play
1. **Main Menu:** make or pick a profile then press Play, tutorial button explains basics.
2. **Hub:** walk around the cave. Stand on a ring to open the specific feature. Walk into the Level 1 portal to start.
3. **The Plains:** follow the path into the maze. Mobs spawn as you get close. Eat and drink to stop your health draining.
4. **Boss:** the end room door (E) takes you to the boss arena. If the 6 purple pillars aren't destroyed, the boss only takes 20% damage. Break every pillar to drop its shield and then hit it before the pillars grow back. The boss has different attacks for different ranges.
5. **Win:** pick up the portal piece, go back to the hub and place it in the Level 2 portal. Level 2 isn't built yet, so the portal opens but doesnt do anything.

## Controls
| Key | Action |
|---|---|
| WASD | Move |
| Shift | Sprint (uses stamina) |
| Space | Jump |
| Mouse | Aim |
| Left click / X | Attack |
| Right click + drag | Rotate camera |
| Scroll | Zoom |
| 1 / 2 | Shooter / melee |
| R | Reload |
| Q / E | Abilities (picked in the Hub inventory) |
| 4-9, 0 | Eat / drink from that slot |
| E | Use a door when close |
| I / J / H | Inventory / achievements / hide UI |
| Esc | Menu |

Controls use the Input System package, all set up in `Assets/Scripts/Input/GameInput.cs`. A gamepad works for the main actions.

## Running the project
- Unity **2023.1.22f1**. Open `Assets/Scenes/Main Menu.unity` and press Play (Build Settings order: Main Menu, Hub, Scene 1 Plains).
- Clone with Git rather than downloading a ZIP. GitHub's ZIP leaves out the Git LFS files.
- Only Unity built-in assets are used: primitives, the built-in font and built-in UI sprites.

## Project structure
- `Assets/Scenes`: Main Menu, Hub, Scene 1 Plains
- `Assets/Scripts`: grouped by system (Player, Weapons, Mobs, Abilities, Items, Loot, Save, UI, World, Input, Editor)
- `Assets/Data`: ScriptableObjects for items, weapons, mobs and abilities
- `Assets/Models`, `Assets/UI`, `Assets/Materials`: prefabs, reusable UI windows and the shared material palette

## Generative AI use
Some code was generated with AI it is declared at the top of each script or here howevr all the owners of the code can explain how it works.

| Files | Tool | How it was used | Owner |
|---|---|---|---|
| `Input/GameInput.cs`, `Editor/RingLine.cs`, `Editor/BossRoomBuilder.cs`, `Editor/PortalSlotBuilder.cs`, `Mobs/BossMob.cs`, `Mobs/BossPillar.cs`, `Mobs/Shockwave.cs`, `World/PortalPiece.cs`, `World/PortalSlot.cs`, `World/HubPortal.cs`, `World/WallHider.cs`, `World/RadiusUI.cs` (started as MenuZone), `Generic/FaceCamera.cs` | Claude (Anthropic) | Helped debug code and give examples of code which completed similair tasks as a blueprint | Elias El Harrak Guiou-Guiou |
| Input System changes in `PlayerController`, `PlayerCombat`, `PlayerInventory`, `PlayerHUD`, `EscapeMenu`, `DoorInteract`, `AchievementPanel`, `PlayerAbilities`; punch fist in `PlayerCombat`; portal fields in `PlayerProfile`; Max Alive in `MobSpawner` | Claude (Anthropic) | Edits to existing scripts to check for errors and used to optimise movement mechanics and features | Elias El Harrak Guiou-Guiou |


