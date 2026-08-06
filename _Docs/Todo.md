# Short-term TODO list

---

## Gameplay aspects

- [x] - **Fix entity placement sounds:** Decouple audio from block instantiation so joining players don't get their ears abused by all existing world blocks play placement sounds when loading.
- [ ] - **Split RGBall into individual dyes:** Replace the monolithic RGBall item with individual color items (e.g., "Red Paint Bucket", "Magenta Paint Bucket").
  - **Helper for registering dyes:** It would also be nice to add a custom way to add dyes via `GameRegistries` so each dye would receive their respective items, procedurally generated textures and colors.

- [ ] - **Implement inventory system:**
  - **Structure:** 32-slot list of item stacks (Data: `ItemID`, `Amount`). Max stack: 128 (stackable), 1 (non-stackable).
  - **Hotbar:** Acts as a "bookmarks" system referencing inventory slots rather than a fixed grid.
  - **UI/UX:** Grocery-list layout (1x32) or spreadsheet layout (4x8) with built-in sorting (Name, ID, Quantity). Mobile-friendly.
  - **Creative Mode:** Add a item picker menu to select any item from the registry and inject it into the inventory. Works similarly to that old minecraft too many items mod.

---

## Technical aspects

- [x] - Have an organized way to synchronize data via Godot's `MultiplayerSynchronizer`.
- [ ] - Implement full world state serialization (buildings, players, entities, items) to support saving and sharing world files.
- [ ] - Implement the ability for items to store their own custom data that goes beyond just its ID.
- [ ] - Build a custom tool to allow for easier creation of PlacingItems, currently, you need to define three different assets in order to add a single new building item to the game:
  - `PlaceableItemData` : `ItemData` : `Resource` - the item that will be in the registry.
  - `_BasePlaced` : `PackedScene` - the actual in-world placed entity
  - `PlacingItem` : `BaseItem` - The model and item that will be held by the player's hand.

---

## UI aspects

- [ ] - Finish the settings menu.
- [ ] - Add a persistent "recent chat messages" overlay so opening the full chat isn't required to read messages.
