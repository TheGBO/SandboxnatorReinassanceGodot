### In spite of the fact that using obsidian markdown gives more control over organization, I still want to keep a todo list here. But a small short term one, not an actual planning.
---
## Gameplay aspects:
- [ ] - Fix the sound system of entity placement, sounds should NOT be intrinsecal(intrinsincal?) to the instantiation of a block, this leads to new players to get ear-"abused" whenever they join a world full of buildings that have sounds for their placement.
- [ ] - Currently, RGBall is a monolithic magic dye that paints anything that can be painted, this should not persist in newer versions, instead, each colour should be its own item, for instance "Red paint bucket", "Magenta paint bucket"[...] instead of a single all-in-one.
- [ ] - Implement an inventory system. The idea I had was not to have an inventory system as a grid like minecraft of terraria does, instead, the inventory system would be merely a list of limited size, like 32 item stacks. A stack would be just a data type containg : Item id, Item amount. the max amount a stack could hold would be 128 items (if stackable) and 1 item (if non stackable). There would not be a fixed hotbar like minecraft, instead, a hotbar would be a "bookmarks system" from existing items in the inventory. This inventory approach, besides being mobile-friendly and easier to code right away, would also make client side inventory organization trivial, the graphical interface would have built-in buttons to "sort by name", "sort by id", "sort by quantity"... The inventory GUI display could look like a "grocery list" (column and 32 rows) or a "spreadsheet" (4 columns and 8 rows). This item on my checklist is massive because it's brainstorming besides just a mere goal. I still have to find a way to deal with a "creative mode(currently the default)", I was thinking of making a "cherrypick menu": one where you could select any Item available in the whole game item registry and send it to your inventory.
---
## Technical aspects:
- [x] - Have an organized way to synchronize data via godot's multiplayersynchronizer.
- [ ] - Server authoritative movement with client-side prediction. This is nuts, I don't know if I'm able to implement this.
- [ ] - A way to serialize every single aspect of the world state, that is: buildings, player positions, other entities... This is to allow to do the follwing things:
    - Save worlds to disk
    - Share world between players, maybe?
