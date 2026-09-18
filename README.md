# CS 417 MP1b: Team 6 Escape Room

Shared Unity project for Daryl, Felix, and RC. The project is seeded from Daryl's working MP1a OpenXR/Quest project so everyone starts with the same Unity, URP, XR, and Android configuration.

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

The old MP1a assets remain as the working XR baseline and reference. All new MP1b work belongs under `Assets/EscapeRoom/`.

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
- `Assets/EscapeRoom/Scenes/EscapeRoom_Main.unity`
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

Each independent room reveals one themed authorization item. The player can solve rooms in any order, carry all three items to the central blast door, and place them into three distinct matching sockets. The final door remains closed through `2/3` and opens only at `3/3`.

The full audited design, rubric mapping, and individual task lists were sent to the team separately in `MP1B_TEAM_PLAN.md`.
