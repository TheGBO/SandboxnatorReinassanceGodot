# Short-term TODO list

---

## Gameplay aspects

- [ ] - **Fix entity placement sounds:** Decouple audio from block instantiation so joining players don't get their ears abused by all existing world blocks play placement sounds when loading.
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
- [ ] - Implement server-authoritative movement with client-side prediction. (or maybe not)
- [ ] - Implement full world state serialization (buildings, players, entities, items) to support saving and sharing world files.
- [ ] - Implement the ability for items to store their own custom data that goes beyond just its ID.

---

## UI aspects

- [ ] - Finish the settings menu.
- [ ] - Add a persistent "recent chat messages" overlay so opening the full chat isn't required to read messages.