# CS 417 MP1c: Bunker 6

Shared Unity project by Daryl Okeke, Felix Romero (frome4), and Robert (RC, rc75). The project is seeded from Daryl's working MP1a OpenXR/Quest project so everyone starts with the same Unity, URP, XR, and Android configuration.

## Setup

- Unity: `6000.5.6f1`
- Render pipeline: URP
- XR: OpenXR `1.17.1` and XR Interaction Toolkit `3.5.1`
- Target: Meta Quest / Android ARM64

After cloning:

1. Run `git lfs install` and `git lfs pull`.
2. Open the repository folder through Unity Hub with Unity `6000.5.6f1`.
3. Let packages import fully before editing.
4. Confirm the Console has no compile errors.
5. Create a feature branch before making room changes.

The old MP1a assets remain as the working XR baseline and reference. All integrated work belongs under `Assets/EscapeRoom/`.

## Ownership Boundaries

Do not edit another person's prefab, room folder, or sandbox scene. Do not reorganize/delete existing shared XR assets.

### Daryl

- `Assets/EscapeRoom/Rooms/Daryl_BedroomDecon/`
- `Assets/EscapeRoom/Shared/PlayerInput/`
- `Assets/EscapeRoom/Shared/SceneFlow/`
- `Assets/EscapeRoom/Shared/GameFlow/`
- `Assets/EscapeRoom/Shared/FinalDoor/System/`
- `Assets/EscapeRoom/Shared/UI/`
- `Assets/EscapeRoom/Scenes/Start.unity`
- `Assets/EscapeRoom/Scenes/MP1B_EscapeRoom.unity`
- Shared XR/Input changes, integration, final builds, and Quest testing

### RC

- `Assets/EscapeRoom/Rooms/RC_SupplyStorage/`
- `Assets/EscapeRoom/Shared/StartContent/`
- `Assets/EscapeRoom/Shared/Collectibles/`
- `Assets/EscapeRoom/Scenes/Sandboxes/RC_SupplyStorage.unity`

RC owns the start-screen content prefab, but Daryl owns `Start.unity` and its scene-loading logic.

### Felix

- `Assets/EscapeRoom/Rooms/Felix_GeneratorComms/`
- `Assets/EscapeRoom/Shared/FinalDoor/Art/`
- `Assets/EscapeRoom/Scenes/Sandboxes/Felix_GeneratorComms.unity`

Felix's final-door prefab is presentation-only. Daryl owns socket, progress, door-opening, and win-condition logic.

### Coordinated folders

- `Assets/EscapeRoom/Shared/Materials/` requires coordination before editing.
- Each room owner may instead keep room-specific materials, scripts, models, audio, and prefabs inside their own room folder.
- Each person creates only their own named sandbox scene under `Assets/EscapeRoom/Scenes/Sandboxes/`.

## Room Handoff Contract

Each room must eventually provide:

- One root prefab at position `(0,0,0)`, rotation `(0,0,0)`, scale `(1,1,1)` in its sandbox
- A documented entrance anchor
- A self-contained puzzle that works in its sandbox
- One completion event that fires exactly once
- One final authorization item revealed by solving the puzzle
- Its own colliders, lights, props, clues, and local feedback

A room must not contain an XR Origin, EventSystem, global timer/scoreboard/manager, hard-coded reference to another room, dependency on another room's completion, or direct control of the final door.

## Branches and Collaboration

Suggested branches:

- `feature/daryl-bedroom-decon`
- `feature/daryl-integration`
- `feature/rc-supply-storage`
- `feature/felix-generator-comms`

Workflow:

1. Pull `main` before starting.
2. Work only in your owned files on your feature branch.
3. Preserve every Unity `.meta` file.
4. Commit one coherent change at a time.
5. Push the branch and open a pull request.
6. Have at least one teammate review before merging.
7. Demonstrate a room working in its sandbox before integration.

Do not commit `Library`, `Temp`, `Logs`, `UserSettings`, or generated builds. Record imported asset sources and licenses as assets are added.

## Game Contract

Each independent room reveals one themed authorization item. The player can solve rooms in any order, carry all three items to the central blast door, and place them into three distinct matching sockets. At `3/3`, the clearances enable the departure station; the blast door remains closed. Players then pack the dedicated EXIT first-aid kit and radio, select the fixed filter controls in particulate–chemical–radiation order, and configure FILTERS and DOOR on with AUX off before pressing TEST. Completing all three stages opens the route to the stairwell and hatch.

The full audited design, rubric mapping, and individual task lists were sent to the team separately in `MP1B_TEAM_PLAN.md`.


## Play and build

Open `Assets/EscapeRoom/Scenes/Start.unity`. WAKE UP loads the canonical `MP1B_EscapeRoom` scene; its filename is retained to preserve existing references. The desktop XR simulator is available in the editor and is excluded from Android builds.

For Quest, install Android Build Support, use IL2CPP and ARM64, and build the enabled Start and MP1B scenes as an APK. Minimum Android API is 32. `MP1cBuildTools.WireAndTest` runs the departure integration check and then builds these scenes; `RoomIntegrationCheck.Run` first checks the menu transition, all three room chains, the mass/signifier constraints, the ending, and restart. Reports and APK output are written to `../the_room_info/MP1c_Quest_Test/` outside the source repository. `SceneAudit.Run` writes a read-only inventory of scene references, text, audio, and rigidbodies.

Automated checks cover game state, reward gating, interaction wiring, and controlled physics. A Quest walkthrough is still required for reach, text readability, audio balance, locomotion, and the stairs/hatch. See [integration evidence and test checklist](MP1C_INTEGRATION.md) for the rubric mapping and recording plan. Third-party sources are listed in [asset attributions](ATTRIBUTIONS.md) and the room asset documentation.
