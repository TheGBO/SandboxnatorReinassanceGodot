### In spite of the fact that using obsidian markdown gives more control over organization, I still want to keep a todo list here. But a small short term one, not an actual planning.
---
## Gameplay aspects:
- [ ] - Fix the sound system of entity placement, sounds should NOT be intrinsecal(intrinsincal?) to the instantiation of a block, this leads to new players to get ear-"abused" whenever they join a world full of buildings that have sounds for their placement.
- [ ] - Currently, RGBall is a monolithic magic dye that paints anything that can be painted, this should not persist in newer versions, instead, each colour should be its own item, for instance "Red paint bucket", "Magenta paint bucket"[...] instead of a single all-in-one.
---
## Technical aspects:
- [ ] - Have an organized way to synchronize data via godot's multiplayersynchronizer.
- [ ] - Server authoritative movement with client-side prediction. This is nuts, I don't know if I'm able to implement this.
- [ ] - A way to serialize every single aspect of the world state, that is: buildings, player positions, other entities... This is to allow to do the follwing things:
    - Save worlds to disk
    - Share world between players, maybe?
