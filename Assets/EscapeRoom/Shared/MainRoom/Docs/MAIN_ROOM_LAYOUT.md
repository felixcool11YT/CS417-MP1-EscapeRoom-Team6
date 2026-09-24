# Main Room Hub Layout

The reusable room asset is `Prefabs/MainRoomHub.prefab`. Test it in
`Assets/EscapeRoom/Scenes/Sandboxes/Daryl_MainRoom.unity`.

## Connection anchors

- `Entrance_Daryl`: west doorway
- `Entrance_RC`: east doorway
- `Entrance_Felix`: north doorway
- `FinalDoorAnchor`: south blast-door opening
- `PlayerStart`: sandbox-only starting position facing the room center

These are empty transforms under `IntegrationAnchors`. They define connection points without
making any room depend on another teammate's hierarchy or scripts.

## Room contents

- Realistic imported sofa, television, and gamepad in the lounge area
- Imported nine-foot pool table and two arcade cabinets in the games area
- Imported capsule blast door under `FinalBlastDoorCandidate`
- Static box colliders on the room shell and imported set pieces
- Separate lounge, games, and final-door lights

The blast door is a visual candidate only. Final key sockets, shared progress logic, and opening
animation should be added during integration after the team confirms the final door design.
