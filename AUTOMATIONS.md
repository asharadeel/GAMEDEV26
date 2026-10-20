# AUTOMATIONS

Step-by-step guides for repeatable tasks in this project. They're written so Claude can do the task on its own and stay consistent with the project structure, and so group members know what Claude will do (or can do it by hand).

**How to use:** tell Claude what you want plus this line:

> Refer to AUTOMATIONS.md for how to add this so you're consistent with the structure.

Example: *"Add a food called Canned Peaches that restores 35 hunger and 10 thirst, costs 15 tokens. Refer to AUTOMATIONS.md for how to add this so you're consistent with the structure."*

Sections:
- [Adding Items](#adding-items)
- [Adding Mobs](#adding-mobs)

---

# Adding Items

Every item is a ScriptableObject asset in `Assets/Data/`, listed in `Assets/Resources/ItemDatabase.asset`. The save file stores item **ids** only, and the game uses the database to turn an id back into the item. **An item that isn't in the database doesn't exist as far as the game is concerned.**

## Item types

| Type | Script | Folder | Database list | Create menu |
|---|---|---|---|---|
| Shooter (gun) | `ShooterData` | `Data/Weapons/Shooters/` | `shooters` | Game > Weapons > Shooter |
| Lazer (ammo/upgrade for a gun) | `LazerData` | `Data/Weapons/Shooters/Lazers/` | `lazers` | Game > Weapons > Lazer |
| Melee | `MeleeData` | `Data/Weapons/Melee/` | `melee` | Game > Weapons > Melee |
| Special | `SpecialData` | `Data/Weapons/Specials/` | `specials` | Game > Weapons > Special |
| Food | `ConsumableData` | `Data/Consumables/Food/` | `food` | Game > Consumable |
| Drink | `ConsumableData` | `Data/Consumables/Drinks/` | `drinks` | Game > Consumable |

## Rules (all types)

- **File name and `m_Name`:** the item's display name, e.g. `Canned Peaches.asset`
- **`id`:** lowercase with underscores, unique across *all* items, e.g. `canned_peaches`. Never change an existing id, saves depend on it
- **`displayName`:** shown in game, e.g. `Canned Peaches`
- **`tokenCost`:** price in tokens, default `100` unless told otherwise
- **`levelRequired`:** player level needed to buy it, default `1`
- **Weapons only:**
  - `unlocked` default `0` (must be bought). Only `1` if it should be starting gear
  - `model` default `{fileID: 0}` unless a prefab is named
- **Lazers and specials:** `distance` default `50`, `size` default `1`, `rotationOffset` default `{x: 90, y: 0, z: 0}` (the project's capsule models stand upright, this lays them along the shot)
- If a stat isn't given, use the defaults in the type's script and say which ones were assumed
- Don't edit scenes or scripts to add an item. Items are data only

## Doing it in the Unity Editor (by hand)

1. Right-click the type's folder in the Project window > **Create > Game > ...** (see table)
2. Name the asset, fill in `id`, `displayName` and stats in the Inspector
3. Open `Assets/Resources/ItemDatabase.asset`, add the new asset to the matching list
4. **Lazer only:** also add it to the gun's `Lazers` list (e.g. `EM Raygun`)
5. Update the item tables in `README.md`

## Doing it as files (Claude)

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
  tokenCost: 100
  levelRequired: 1
  unlocked: 0
  model: {fileID: 0}
  lazers:
  - {fileID: 11400000, guid: <LAZER_ASSET_GUID>, type: 2}
```

**Lazer**
```yaml
  id: lazer
  displayName: Lazer
  tokenCost: 100
  levelRequired: 1
  unlocked: 0
  model: <MODEL_REF>
  damage: 10
  fireRate: 4
  speed: 20
  distance: 50
  size: 1
  rotationOffset: {x: 90, y: 0, z: 0}
```

**Melee**
```yaml
  id: kick
  displayName: Kick
  tokenCost: 100
  levelRequired: 1
  unlocked: 0
  model: {fileID: 0}
  damage: 8
  fireRate: 1.2
  radius: 1.5
```

**Special**
```yaml
  id: rocket
  displayName: Rocket
  tokenCost: 100
  levelRequired: 1
  unlocked: 0
  model: <MODEL_REF>
  damage: 50
  fireRate: 0.5
  speed: 15
  radius: 3
  distance: 50
  size: 1
  rotationOffset: {x: 90, y: 0, z: 0}
```

**Food / Drink**
```yaml
  id: soda
  displayName: Soda
  tokenCost: 15
  levelRequired: 1
  hunger: 0
  thirst: 25
  stamina: 30
  health: 0
```

### Model references (`<MODEL_REF>`)
Models are prefabs in `Assets/Models/`. The reference is:
```yaml
{fileID: <ROOT_GAMEOBJECT_ID>, guid: <PREFAB_GUID>, type: 3}
```
- `<PREFAB_GUID>`: the `guid:` line in the prefab's `.prefab.meta`
- `<ROOT_GAMEOBJECT_ID>`: in the `.prefab` file, the number after `--- !u!1 &` on the block whose `m_Name` is the prefab's name

Current models:

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
- **`health`, `attackDamage`:** as given. If not given, ask
- **`type`:** `0` = Follow (chases the player), `1` = Shooter (stays still and shoots). Shooters need a `projectileModel`
- **`model`:** the mob prefab in `Assets/Models/Mobs/` (see *Model references* in Adding Items for how to write the reference)
- Mobs don't need a collider on the root, colliders on child shapes are fine (damage reaches the root)

## By hand
1. Right-click `Assets/Data/Mobs` > **Create > Game > Mob**, fill in the fields
2. To spawn it: select a spawner (prefab `Models/Mobs/Spawners/MobSpawner` changes every spawner, or a single one in the scene), add a row to `Spawns` with the mob and amount
3. Keep `Spawns` in order weakest to strongest (row order decides when they start)
4. Update the mob table in `README.md`

## As files (Claude)
`.meta`: same as items (NativeFormatImporter, mainObjectFileID 11400000). `.asset` header: same as items with `<SCRIPT_GUID>` = `MobData` guid (`5546605e9e7b4fcaa7c5323071295c28`, check `Assets/Scripts/Mobs/MobData.cs.meta`). Fields:
```yaml
  id: mob_level_1
  displayName: Mob Level 1
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
```
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
- [ ] README mob table updated
