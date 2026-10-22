# AUTOMATIONS

This is an AI-written document: step-by-step guides and rules for repeatable tasks, so everything stays consistent with the project structure. Each task can be done in the Unity Editor or as files.

**How to use:** when asking an AI assistant for something, add this line:

> Refer to AUTOMATIONS.md for how to add this so you're consistent with the structure.

Example: *"Add a food called Canned Peaches that restores 35 hunger and 10 thirst, costs 15 coins. Refer to AUTOMATIONS.md for how to add this so you're consistent with the structure."*

**For AI assistants:** these are project rules agreed by the team. Follow them even if a request doesn't mention them, and if a request conflicts with a rule, point it out and ask before going ahead. Policies (like UI POLICY) apply to every change, not only when someone refers to this file.

Sections:
- [UI POLICY](#ui-policy)
- [Adding Items](#adding-items)
- [Adding Mobs](#adding-mobs)

---

# UI POLICY

Every UI menu, panel or popup that can be shown and hidden is built in three layers. This keeps menus from breaking when they're hidden, and means every script toggles the same kind of object.

```
Canvas
└─ IntroMenu        Container: stretched full screen, empty, ALWAYS active
   └─ Frame         Frame: stretched full screen, the thing that gets shown/hidden
      └─ Panel      The actual UI (background, title, buttons...), always active
```

## Rules
1. **Container** (named after the menu, e.g. `IntroMenu`, `EscMenu`)
   - RectTransform stretched: Anchor Min (0, 0), Anchor Max (1, 1), Left/Right/Top/Bottom all 0 (Pos 0, Size Delta 0)
   - No Image, Text or other visuals. It may hold the script that controls the menu (e.g. `EscapeMenu`)
   - Always active. Never switch the container off: scripts on a switched-off object stop running (e.g. Esc could never re-open the menu)
2. **Frame** (always named `Frame`)
   - Stretched exactly like the container
   - This is the object that gets shown/hidden. Every reference that opens/closes the menu points at the Frame: `RadiusUI.ui`, `HubPortal.lockedMessage`, `EscapeMenu.menu`, `OpenElemOnClick` / `CloseElemOnClick` `elements`, etc.
   - Leave it **switched on in the editor**. If it should start hidden, add `InactiveOnStart` to it. Don't untick it in the editor: `InactiveOnStart` only runs the first time it's enabled, so a frame that starts unticked hides itself again the first time it's opened
   - If the player shouldn't be controllable while it's open (menus, popups, text windows), add `OverlayUI` to the Frame. While any OverlayUI is showing, `PlayerController.uiEnabled` is true and movement, jumping, attacks, eating and camera rotate/zoom are off (so scrolling a menu doesn't zoom the camera). `GeneralTextWindow` already has it
   - A full-screen background (dim overlay) can go on the Frame itself (e.g. Scene 1's `EscMenu/Frame` has a RawImage)
   - Backgrounds on Frames and panels use **RawImage**, not Image (it can take any texture later). Only use Image where a sliced sprite is needed (scroll view, scrollbar, rounded buttons)
3. **Content** inside the Frame can be laid out however the menu needs (centred panel, buttons anchored to edges...). Keep it switched on, the Frame handles visibility
4. Always-visible HUD elements (health bars, Mobs Left tracker) don't need a Frame. They go inside `PlayerUI`, which is itself a stretched container
5. UI text: legacy Text, font `Assets/Fonts/arial/ARIAL.TTF`, Best Fit on (except text inside a scroll view, see below)
6. **Text windows** (intro, help, story, guides): use the `Assets/UI/GeneralTextWindow` prefab as the menu's Frame (it's stretched and has `InactiveOnStart` on its root)
   - Change the window's text on the copy in the scene (prefab overrides), not in the prefab, so the prefab stays a blank template
   - `Title/Label` = window title, `Content/Title1` = heading, `Content/Context` = body (rich text: `<b>bold</b>`)
   - The scroll view is vertical only, Clamped (stops where the content ends) and the content sizes itself (Vertical Layout Group + Content Size Fitter). Text inside it must have Best Fit off, otherwise the layout can't measure it
   - Point the thing that opens it (e.g. `RadiusUI.ui`) at the window's root. Its Close button already closes it
7. **Pop-ups / prompts** (short message + one action, e.g. "Go to the next floor?", "Not enough coins"): use the `Assets/UI/GeneralPopUpWindow` prefab as the menu's Frame
   - It's a centred box (root inset 500px left/right and 300px top/bottom from the container), with `InactiveOnStart` and `OverlayUI` already on its root, so the player is frozen while it's open
   - `Title/Label` = title, `Button` = the action button (its label is `Button/Text (TMP)`, a legacy Text despite the name), `CloseButton` closes the pop-up
   - The action button has no script in the prefab. Add what it should do on the scene copy, e.g. `OpenScene`, `OpenElemOnClick` / `CloseElemOnClick`, or a script of your own. Change the text on the scene copy too, not in the prefab
   - For longer text that needs scrolling, use `GeneralTextWindow` instead
7b. **Paged windows** (a few boxes the player flips through with Prev/Next, e.g. tutorial steps): use the `Assets/UI/GeneralTileWindow` prefab as the menu's Frame
   - `InactiveOnStart` and `OverlayUI` are on its root, plus `TilePager`, which shows one box at a time
   - The boxes are under `Frames` (`Step (1)` to `Step (4)` by default). Each one is in `TilePager.Pages` with an `Order` (lowest shows first). To add a box, duplicate a Step and add it to `Pages` with the next order. Leave every box switched on, the pager hides the others
   - `Prev` / `Next` are greyed out on the first / last box. Don't put `CloseElemOnClick` on them, only `CloseButton` closes the window
   - Fill in the boxes on the scene copy, not in the prefab
8. Before making a new window or pop-up layout, check `Assets/UI/` for a prefab that already does it
8b. **Prefab copies:** change a window's text/boxes on its scene copy (overrides). Don't **Unpack** a General window (an unpacked copy never gets prefab fixes) and don't **Apply** a scene copy's overrides to the prefab (that would put e.g. the tutorial's 8 steps into every tile window). Only edit the prefab itself for changes every window should get
8c. **Shop items:** each weapon in the shop is a copy of `Assets/UI/ShopItemCard` and each food/drink a copy of `Assets/UI/ConsumableCard`, placed by hand, with the item asset in its `Item` field. Don't build cards from code. Buy/Refund pop-ups are shared (`ShopPopups` on the shop's container) and Upgrade opens the shared `UpgradePopup` window (`WeaponUpgradeUI`: bars for shooters, a Level bar for melee/specials, whose Buy buttons reuse `BuyPopup`), don't add pop-ups per card. Refunds never change with upgrades (always a third of the price)
9. **Opening a UI when the player walks up to something** (sign, NPC, portal, stall): place the `Assets/Models/ShowUIonApproach` prefab there (as a child of the sign so it moves with it) and drag the UI's Frame into its `UI` field. Set `Radius` (how close to open it) and `Show Ring Within` (when the floor ring appears). Don't write a new trigger script or a one-off ring for this

## Adding a new menu (in the Editor)
1. Right-click `Canvas` > **Create Empty**, name it after the menu, set its RectTransform to stretch (Alt + click the bottom-right anchor preset) and zero all offsets
2. Inside it, create an empty `Frame`, stretch it the same way, add `InactiveOnStart` if it should start hidden
3. Build the menu inside the Frame
4. Point whatever opens/closes it at the **Frame**

## Converting an existing menu (as files)
1. Make a new container GameObject with the menu's original name, stretched, parented to the menu's old parent, in the menu's old place in the parent's `m_Children` list
2. Make `Frame` under it, stretched, with an `InactiveOnStart` MonoBehaviour (script guid `c60abf8e2bb6ee64b979f04f724e2c2c`) if it should start hidden
3. Rename the old menu object to `Panel`, set `m_IsActive: 1`, change its `m_Father` to the Frame's RectTransform
4. Every `{fileID: <old menu GameObject>}` reference in other components (menu fields, element lists) now points at the Frame's GameObject. Don't change the old menu's own components (`m_GameObject` lines)
5. Check: no duplicate fileIDs, every reference resolves, every `m_Father` matches its parent's `m_Children`

## Status
| Scene | Follows the policy | Not yet |
|---|---|---|
| Scene 1 Plains | `EscMenu` (no OverlayUI yet, Ashar is working on it), `PlayerUI` (HUD container) | `AchievementsPanel`, `AchievementBanner` (directly under Canvas, owned by Davud) |
| Main Menu | `Tutorial` (GeneralTileWindow, 8 steps), `ProfileSelect` (GeneralTextWindow + `ProfileSelectUI` on the container), `PasskeyPopup`, `DeletePopup`, `EditPopup` (GeneralPopUpWindow) | `MainMenu` (always shown, no Frame needed) |
| Hub | `StoreUI` (`GeneralSelectorUI` + `TabSelector`, `ShopPopups` on the container), `BuyPopup`, `RefundPopup`, `UpgradePopup` (GeneralPopUpWindow, + `WeaponUpgradeUI` on the container), `LockedMessage` (+ OverlayUI), `IntroMenu` (GeneralTextWindow, + OverlayUI), `LibraryMenu` (no OverlayUI yet), `Scene1Prompt` (pop-up "Enter Scene 1?" with a Play button) | `ShopMenu`: **someone else is working on it, don't change it** without asking |

`Tools > Build Hub` (`HubBuilder.cs`) still builds the old layout. Re-running it replaces the Hub scene and undoes this. Don't run it unless the team agrees, or update `HubBuilder` first.

---

# Adding Items

Every item is a ScriptableObject asset in `Assets/Data/`, listed in `Assets/Resources/ItemDatabase.asset`. The save file stores item **ids** only, and the game uses the database to turn an id back into the item. **An item that isn't in the database doesn't exist as far as the game is concerned.**

## Item types

| Type | Script | Folder | Database list | Create menu |
|---|---|---|---|---|
| Shooter (gun) | `ShooterData` | `Data/Weapons/Shooters/` | `shooters` | Game > Weapons > Shooter |
| Lazer (what a gun fires, its lazer bar) | `LazerData` | `Data/Weapons/Shooters/Lazers/` | `lazers` | Game > Weapons > Lazer |
| Melee | `MeleeData` | `Data/Weapons/Melee/` | `melee` | Game > Weapons > Melee |
| Special | `SpecialData` | `Data/Weapons/Specials/` | `specials` | Game > Weapons > Special |
| Food | `ConsumableData` | `Data/Consumables/Food/` | `food` | Game > Consumable |
| Drink | `ConsumableData` | `Data/Consumables/Drinks/` | `drinks` | Game > Consumable |

## Rules (all types)

- **File name and `m_Name`:** the item's display name, e.g. `Canned Peaches.asset`
- **`id`:** lowercase with underscores, unique across *all* items, e.g. `canned_peaches`. Never change an existing id, saves depend on it
- **`displayName`:** shown in game, e.g. `Canned Peaches`
- **`coinCost`:** price in coins, default `100` unless told otherwise (old assets say `tokenCost`, that still loads)
- **`levelRequired`:** player level needed to buy it, default `1`
- **Weapons only:**
  - `unlocked` default `0` (must be bought). Only `1` if it should be starting gear
  - `model` default `{fileID: 0}` unless a prefab is named
- **Shooters:** no levels. Each bar (`damage`, `fireRate`, `bulletSpeed`, `reloadTime`) has `start`, `perLevel`, `firstCost` (5 upgrades, upgrade n costs n x firstCost). If not given, copy the EM Raygun's values. `magazineSize` default `20`
- **Lazers:** `pierce` default `0`. A lazer's `coinCost` is its price on the gun's lazer bar
- **Lazers and specials:** `distance` default `50`, `size` default `1`, `rotationOffset` default `{x: 90, y: 0, z: 0}` (the project's capsule models stand upright, this lays them along the shot)
- If a stat isn't given, use the defaults in the type's script and say which ones were assumed
- Don't edit scenes or scripts to add an item. Items are data only

## Doing it in the Unity Editor

1. Right-click the type's folder in the Project window > **Create > Game > ...** (see table)
2. Name the asset, fill in `id`, `displayName` and stats in the Inspector
3. Open `Assets/Resources/ItemDatabase.asset`, add the new asset to the matching list
4. **Lazer only:** also add it to the gun's `Lazers` list (e.g. `EM Raygun`). The list is the gun's lazer bar: 1st = starting lazer, then each upgrade in order
5. Update the item tables in `README.md`

## Doing it as files

Create two files: the `.asset` and its `.meta`. Use a new random 32-character lowercase hex guid for the `.meta`.

### `.meta` (same for every item)
```yaml
fileFormatVersion: 2
guid: <NEW_GUID>
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

### `.asset` header (same for every item)
Only `<SCRIPT_GUID>` and `<NAME>` change. Item fields go straight after it.
```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: <SCRIPT_GUID>, type: 3}
  m_Name: <NAME>
  m_EditorClassIdentifier: 
```

### Script guids

| Script | `<SCRIPT_GUID>` |
|---|---|
| `ShooterData` | `aadd36119fd74d12ba77681fdd11371b` |
| `LazerData` | `5f28944de6bf424395872a89e061d793` |
| `MeleeData` | `fbc67f158e46455f9fa34b84b68f4157` |
| `SpecialData` | `5095c45212fe4b23842e7543c3e0ac5f` |
| `ConsumableData` | `2234bb3b31a9474a9e1634094a9dc2b9` |

If a guid here doesn't match the script's `.cs.meta`, trust the `.meta` and fix this table.

### Fields per type

**Shooter**
```yaml
  id: em_raygun
  displayName: EM Raygun
  coinCost: 100
  levelRequired: 1
  unlocked: 0
  model: {fileID: 0}
  magazineSize: 20
  damage:
    start: 10
    perLevel: 3
    firstCost: 20
  fireRate:
    start: 4
    perLevel: 0.8
    firstCost: 20
  bulletSpeed:
    start: 20
    perLevel: 4
    firstCost: 15
  reloadTime:
    start: 2
    perLevel: -0.3
    firstCost: 15
  lazers:
  - {fileID: 11400000, guid: <LAZER_ASSET_GUID>, type: 2}
```

**Lazer**
```yaml
  id: lazer
  displayName: Lazer
  coinCost: 100
  levelRequired: 1
  unlocked: 0
  model: <MODEL_REF>
  distance: 50
  size: 1
  pierce: 0
  rotationOffset: {x: 90, y: 0, z: 0}
```

**Melee**
```yaml
  id: kick
  displayName: Kick
  coinCost: 100
  levelRequired: 1
  unlocked: 0
  model: {fileID: 0}
  levels:
  - model: {fileID: 0}
    damage: 8
    fireRate: 1.2
    radius: 1.5
  - model: {fileID: 0}
    damage: 12
    fireRate: 1.2
    radius: 1.5
  - model: {fileID: 0}
    damage: 16
    fireRate: 1.2
    radius: 1.5
```
Melee and specials have 3 `levels` (1 = as bought, 2 and 3 = upgrades). If only level 1 stats are given, use them for all 3 and multiply `damage` by 1.5 for level 2 and 2 for level 3

**Special**
```yaml
  id: rocket
  displayName: Rocket
  coinCost: 100
  levelRequired: 1
  unlocked: 0
  model: <MODEL_REF>
  levels:
  - model: <MODEL_REF>
    damage: 50
    fireRate: 0.5
    speed: 15
    radius: 3
    distance: 50
    size: 1
    rotationOffset: {x: 90, y: 0, z: 0}
  - (same fields, level 2)
  - (same fields, level 3)
```

**Food / Drink**
```yaml
  id: soda
  displayName: Soda
  coinCost: 15
  levelRequired: 1
  hunger: 0
  thirst: 25
  stamina: 30
  health: 0
```

### Model references (`<MODEL_REF>`)
Models are prefabs in `Assets/Models/` (lazers in `Models/RayGun/`, mobs in `Models/Mobs/`). The reference is:
```yaml
{fileID: <ROOT_GAMEOBJECT_ID>, guid: <PREFAB_GUID>, type: 3}
```
- `<PREFAB_GUID>`: the `guid:` line in the prefab's `.prefab.meta`
- `<ROOT_GAMEOBJECT_ID>`: in the `.prefab` file, the number after `--- !u!1 &` on the block whose `m_Name` is the prefab's name

Current models (`Models/RayGun/`):

| Prefab | Reference |
|---|---|
| `Lazer` | `{fileID: 2087813676588361287, guid: f6c8bc097e049b747ac26be915eacddc, type: 3}` |
| `UV Lazer` | `{fileID: 8510083626483001670, guid: 977bc1223176b5e4ab2669aa23499ed6, type: 3}` |
| `Gamme Lazer` | `{fileID: 2223022628636679097, guid: 6f3a0d6a963c6c04ebc1fa244145ee9b, type: 3}` |

### Registering the item
1. **ItemDatabase:** in `Assets/Resources/ItemDatabase.asset`, add a line under the matching list (`shooters`, `lazers`, `melee`, `specials`, `food`, `drinks`):
   ```yaml
   - {fileID: 11400000, guid: <NEW_ITEM_GUID>, type: 2}
   ```
   `<NEW_ITEM_GUID>` is the guid from the item's new `.meta`
2. **Lazer only:** add the same line under `lazers:` in the gun's asset (e.g. `Data/Weapons/Shooters/EM Raygun.asset`)
3. **README:** add the item to the matching table under "Weapons and Items"
4. **Food/drink only, optional:** to make mobs drop it, add it to a mob's `drops` (see Adding Mobs)

### Checklist before finishing
- [ ] `.asset` and `.meta` are in the right folder (see table)
- [ ] `id` is unique (search `Assets/Data` for `id: <the_id>`)
- [ ] Every `guid` referenced (script, model, lazer) exists in a `.meta` file
- [ ] Added to the right list in `ItemDatabase.asset`
- [ ] Lazer: added to the gun's `Lazers` list
- [ ] README table updated
- [ ] Tell the user: no scene changes needed, and new items don't need the save deleted. Changing an existing item's `id` does break saves

---

# Adding Mobs

A mob is a profile asset (`MobData`) in `Assets/Data/Mobs/` pointing at a model prefab in `Assets/Models/Mobs/`. Spawners hold a list of profiles + amounts.

## Rules
- **File name / `m_Name`:** `MobLevel<N>` or the mob's name, e.g. `MobLevel4`
- **`id`:** lowercase with underscores, unique, e.g. `mob_level_4`
- **`displayName`:** e.g. `Mob Level 4`
- **`shortName`:** label in the Mobs Left tracker, e.g. `Lvl 4`
- **`health`, `attackDamage`:** as given. If not given, ask
- **`type`:** `0` = Follow (chases the player), `1` = Shooter (stays still and shoots). Shooters need a `projectileModel`
- **`model`:** the mob prefab in `Assets/Models/Mobs/` (see *Model references* in Adding Items for how to write the reference)
- **Collider:** give the mob prefab's root a Box Collider covering the whole model (every current mob has one), so hits always land
- **`drops`:** loot rolled on death, each row is a consumable + `chance` (0-1, rolled separately). Can be empty

## In the Editor
1. Right-click `Assets/Data/Mobs` > **Create > Game > Mob**, fill in the fields (including `Drops` if it should drop loot)
2. To spawn it: select a spawner (prefab `Models/Mobs/Spawners/MobSpawner` changes every spawner, or a single one in the scene), add a row to `Spawns` with the mob and amount
3. Keep `Spawns` in order weakest to strongest (row order decides when they start)
4. Update the mob table and loot table in `README.md`

## As files
`.meta`: same as items (NativeFormatImporter, mainObjectFileID 11400000). `.asset` header: same as items with `<SCRIPT_GUID>` = `MobData` guid (`5546605e9e7b4fcaa7c5323071295c28`, check `Assets/Scripts/Mobs/MobData.cs.meta`). Fields:
```yaml
  id: mob_level_1
  displayName: Mob Level 1
  shortName: Lvl 1
  model: <MODEL_REF>
  type: 0
  health: 100
  attackDamage: 10
  moveSpeed: 3
  chaseRange: 25
  attackRange: 0.5
  attackCooldown: 1
  shootRange: 20
  fireRate: 0.5
  projectileModel: {fileID: 0}
  projectileSpeed: 15
  projectileSize: 1
  drops:
  - item: {fileID: 11400000, guid: <CONSUMABLE_GUID>, type: 2}
    chance: 0.3
```
`<CONSUMABLE_GUID>` is the guid in the food/drink asset's `.meta` (e.g. `Data/Consumables/Food/Beans.asset.meta`). Use `drops: []` for no loot.

To add it to every spawner, add a row under `spawns:` in `Assets/Models/Mobs/Spawners/MobSpawner.prefab`:
```yaml
  - mob: {fileID: 11400000, guid: <NEW_MOB_GUID>, type: 2}
    amount: 10
```

Current mob models:

| Prefab | Reference |
|---|---|
| `MobLvl1` | `{fileID: 303960829140282040, guid: 103f86cdfa661f14b9aa90b9484ba25f, type: 3}` |
| `MovLvl2` | `{fileID: 5796215148282423660, guid: aacc9e99494f6c140be570fb3760c544, type: 3}` |
| `MobLvl3` | `{fileID: 1583457316732566423, guid: 6ec1316160ffde7469230f34cca34869, type: 3}` |

## Checklist
- [ ] `.asset` and `.meta` in `Assets/Data/Mobs/`, `id` unique
- [ ] Model reference guid exists in a `.prefab.meta`
- [ ] Added to the spawner(s) in weakest-to-strongest order if it should spawn
- [ ] Root Box Collider on the mob prefab
- [ ] Every `drops` item guid exists in a consumable `.meta`
- [ ] README mob table and loot table updated
