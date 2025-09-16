### Naming Conventions
- ```PascalCase``` Classes, properties, methods, files and folder names, enums, structs.
- ```camelCase``` Private fields, method arguments and local variables.
- For game registry IDs, both camelCase and PascalCase are acceptable as long as it's consistent within the registry dictionary, if one command is camelCase, all should be, if one item is pascalCase, all should be.
- ```snake_case``` files that have a prefix/suffix such as ui_hover.mp3, ui_interact.mp3, PlayerModel_0
### DTO Conventions : 
- DTO (Data transfer objects) in Sandboxnator are of two types:
    - Binary (MessagePackObject)
        - **Uses**: Network packets, compact data, save files...
    - Resource (Godot Resources)
        - **Uses**: Human readable, customizable and flexible assets
        - Must be easy to use in the godot editor.
    - Rule of thumb: 
        - If it’s player/game state → Binary DTO.
        - If it’s content/configuration → Resource.

### Resource naming conventions for GameContent/Items
- Example:
    - **(.tres) ItemData** (the indexing resource): BlackCube.tres
    - **(.tscn) Model Scene** (the model held in hand): ItemBlackCube.tscn
    - **(.tscn) Placed** (the the build in the world): PlacedBlackCube.tscn

# (W.I.P.)
    - networking conventions
    - component system conventions
    - chat command conventions
    