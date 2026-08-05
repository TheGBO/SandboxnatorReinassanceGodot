# Conventions for sandboxnator

## Naming conventions

- ```PascalCase``` Classes, properties, methods, files and folder names, enums, structs, registry entries (IDs).
- ```camelCase``` Private fields with the Export attribute, method arguments and local variables.
- ```_underscoreCamelCase``` Truly private fields without external references
- ```snake_case``` files that have a prefix/suffix such as ui_hover.mp3, ui_interact.mp3, PlayerModel_0

---

## Resource naming conventions for GameContent/Items

- Example:
  - **(.tres) ItemData** (the indexing resource): BlackCube.tres
  - **(.tscn) Model Scene** (the model held in hand): ItemBlackCube.tscn
  - **(.tscn) Placed** (the the build in the world): PlacedBlackCube.tscn

---

## Networking conventions

- When naming RPCs, they should have a sufix indicating the intention and direction of a comand.
  - `ServerBound`: Indicates that an RPC command and its information flow from client to server. The "intent" part is context dependent, for instance `ServerBoundRequestSomething` or `ServerBoundUseSomething`. The `ServerBound` prefix replaces its legacy counterpart `C2S_`. It is also mentioning that it is good practice, and most of the times crucial to mark the RPC attribute of ServerBound RPCs as `MultiplayerApi.RpcMode.AnyPeer, CallLocal = true`.
  - `ClientBound`: Indicates that an RPC command and its information flow from the server to the client. The "intent" also depends on context such as `ClientBoundConfirmSomething`, `ClientBoundReceiveSomething`, `ClientBoundSyncSomething` and `ClientBoundSetSomething`. the `ClientBound` prefix replaces the legacy `S2C_` prefix. The recommended RPC attribute is `MultiplayerApi.RpcMode.Authority`, for it is alway recommended to have the server as an authority, unless it comes to client authoritative systems such as Movement or Input.

---

## (W.I.P.)

    - DTO and serialization conventions
    - component system conventions
    - chat command conventions
    - Registry conventions
    - Item specific data conventions
    