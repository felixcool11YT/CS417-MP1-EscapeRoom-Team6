# Placement Guide: Bedroom Decon Theme Kit

## The fast path: one drag

`Prefabs/RoomDressing.prefab` is the whole dressed room in one object.
Drag it into your scene at position (0, 0, 0) with no rotation. Every prop
lands where it belongs, because the layout was designed against your actual
room geometry (floor x in [-5, 5], z in [-3, 3], divider doorway at x = 0).

The hierarchy it creates:

```
RoomDressing
  Bedroom
    Bed, Nightstand, TableLamp, Rug, Desk, Chair, Newspaper,
    Candle, Locker, LockerDoor, PosterSafety,
    CeilingFixture_Bedroom
  DeconZone
    DeconArch, PipeRun_Upper, PipeRun_Lower, Vent, PosterWarning,
    PosterChart, Dispenser,
    CeilingFixture_Decon, HazardStrip_N_2.7, ... (12 strips)
```

RoomDressing is a flat prefab: every prop carries its own mesh and
materials, so there are no prefab-to-prefab links to break on import.
If anything still looks wrong, fall back to dragging
the individual prefabs and use the position table below plus
`Preview/layout_map.png` (top-down map of every prop).

## Position table (local coords inside RoomDressing)

Bedroom:
- Bed (-3.2, 0, -1.85), headboard against the back wall
- Nightstand (-1.95, 0, -2.55); TableLamp sits on it at y 0.625
- Rug (-3.2, 0.002, -0.35)
- Desk (-4.55, 0, 1.5) rotated 90 deg, against the left wall
- Chair (-3.85, 0, 1.5) facing the desk
- Newspaper (-4.55, 0.765, 1.35),
  Candle (-4.75, 0.765, 1.75) - on the desk. Note: the keycard is NOT
  placed in the room. It is your room's authorization item, dispensed
  at the Dispenser when the puzzle hits 3/3.
- Locker (-0.85, 0, -2.7); LockerDoor (-1.13, 0, -2.71), hinge on the
  left edge, closed. Implement it LOCKED until the keypad code 2019 is
  entered; rotate the door on Y to swing open for the reveal.
- Keypad (-0.25, 1.4, -2.88) on the back wall next to the locker.
- Crate (-3.2, 0, -1.85) under the bed. Place YOUR radiation filter
  inside it (the crate is open-topped; the magenta glow leaking out is
  the signifier).
- PosterSafety (-4.88, 1.7, -0.9) on the left wall
- CeilingFixture_Bedroom (-2.5, 3.0, -0.5)

DeconZone:
- DeconArch (1.35, 0, 0) rotated 90 deg - walk through it along x after
  the divider doorway
- PipeRun_Upper (2.5, 2.3, -2.82) and PipeRun_Lower (2.5, 2.02, -2.82)
  along the back wall; Vent (0.9, 2.0, -2.84)
- PosterWarning (4.95, 1.7, -1.2) on the right wall
- PosterChart (0.16, 1.6, -1.6) rotated 90 deg, on the divider wall facing
  into the decon zone. A filter identification chart: color squares plus
  names, so the player learns the matching before finding anything.
- Dispenser (4.3, 1.5, -2.87) on the back wall near the sockets. This is
  where the keycard is issued at 3/3. The status light is red until
  calibration completes; swap `MAT_StatusRed` for `MAT_StatusGreen` and
  spawn the `Keycard` prefab at the slot.
- CeilingFixture_Decon (2.6, 3.0, -1.0)
- 12 HazardStrip pieces: a rectangle around your socket/filter work area
  plus two marking the divider doorway threshold

Your existing filters, sockets, and Puzzle wiring are untouched. The
layout leaves your socket positions alone.

## The prefabs (if you place by hand)

- `Bed`, `Nightstand`, `TableLamp`, `Desk`, `Chair` - bedroom furniture
- `Locker` - body. `LockerDoor` is separate: its origin is at the hinge
  edge, so place it at the hinge position on the locker body and rotate on Y
  to swing open for the locker reveal.
- `DeconArch` - decon zone archway, hazard stripes on the beam
- `ChemCanister` - chemical filter, green signifier band plus hazard label
- `ParticulateFilter` - blue filter face
- `Keycard`, `Candle` (emissive flame), `Newspaper`
- `Rug`, `PipeRun`, `Vent`, `HazardStrip`
- `PosterSafety`, `PosterWarning` - thin wall posters using the two sign
  materials
- `PosterChart` - filter identification chart (blue/green/magenta squares
  with names). Teaches the matching; hang it where the player will study it.
- `Dispenser` - wall-mounted authorization terminal with a card slot and a
  status light (red `MAT_StatusRed`, swap to green `MAT_StatusGreen` at
  3/3). The `Keycard` prefab is the item it issues; spawn it at the slot.
- `CeilingFixture` - ceiling mount with an emissive panel AND a real
  Point Light (intensity 20, range 10, warm color). Two are placed in
  RoomDressing. Tweak the intensity to taste; delete the Light component
  if you want to do lighting yourself.

## Signifier color scheme (keep consistent with your sockets)

- Particulate filter: blue (`MAT_FilterParticulate` on the filter face)
- Chemical filter: green (`MAT_FilterChemical` on the canister body)
- Radiation filter: magenta, emissive. Use a sphere with
  `MAT_FilterRadiation` so it glows in the dark decon zone.

## Newspaper front page (the clue)

The newspaper's top face (`MAT_PaperFront`) is a full front page at 512px
so it stays legible in the headset. It does three jobs:

1. World context: "CONTAMINATION SCARE SHUTS WING" with a short article
   about the lockdown.
2. The rule of the room, taught diegetically: "Decon calibration requires
   all three filter stages seated before the system will issue an
   authorization credential."
3. The clue: a red-pen circle around the paragraph about the 2019
   retrofit ("crews cached spare filtration units beneath crew bunks"),
   plus R.'s handwritten margin note apologizing for not waking you,
   explaining he tore the housings apart hunting for good filters, and
   pointing at the article. The note ends: "I'm going up top to see the
   sky. Don't hate me. -- R."

Mechanically: the player grabs the newspaper off the desk, brings it close,
sees the circle and the note, and checks under the bed. Keep the text as
is unless the story changes; the texture regenerates in minutes.

## Placing your objects (filters, sockets, keycard)

RoomDressing places every prop. These are yours to place because they
carry your XR components and socket logic. Exact coordinates:

- YOUR Filter_Particulate: on the desk at (-4.35, 0.85, 1.15). In plain
  view, the gimme.
- YOUR Filter_Chemical: inside the locker at (-0.85, 0.15, -2.7), on the
  locker floor. Revealed when the door swings open.
- YOUR Filter_Radiation: inside the crate at (-3.2, 0.11, -1.85). The
  crate is open-topped; the magenta glow leaking out is the signifier.
- Keycard spawn (use the PR's Keycard prefab): at the dispenser slot,
  (4.3, 1.38, -2.745). Spawn it there at 3/3 when you swap the status
  light to green.

Your sockets stay exactly where they are. Nothing in the layout overlaps
them.

## Puzzle flow v2 (your implementation)

1. Blue particulate filter in plain view on the desk -> blue socket. This
   teaches grab plus socket for free. The chart on the divider wall shows
   it is stage 1.
2. The locker is LOCKED. The keypad beside it on the back wall takes a
   4-digit code. The code is 2019, from the newspaper article ("during the
   2019 retrofit"). The player reads the paper for the code and absorbs
   the "beneath crew bunks" clue without realizing it yet. Inside the
   locker: the green chemical filter -> green socket as stage 2.
3. The player stalls at 2/3. The circled paragraph plus R.'s note point at
   the bed. Under it: a wooden crate with the magenta glow leaking out.
   Your radiation filter goes inside the crate. Stage 3 -> magenta socket.
4. The chart enforces order: sockets only accept filters in stage order
   1-2-3. A wrong-order seat gets rejected (buzz, filter pops back out).
   The chart says so on its face ("wrong order will be rejected").
5. At 3/3: the Dispenser status light goes green and the Keycard spawns
   at the slot. The keycard is your room's ONE authorization item. It
   travels via the team's persistent inventory to the FinalDoor scene,
   which you are building.

Difficulty audit: every step has a fair clue (visible filter, year in
plain text, crate under the only bed, order posted on the wall). Wrong
guesses fail soft. No pixel hunting. Estimated 15-25 minutes for a
first-time player.

## Room shell (matches the concrete/brick direction from the team chat)

- Walls: `MAT_Wall_ConcreteDark`, one accent wall `MAT_Wall_BrickDark`
- Ceiling: `MAT_Ceiling_Concrete`
- Floor: `MAT_Floor_Tiles`
- Signs: quads with `MAT_SignDecon` over the decon entrance and
  `MAT_SignWarning` near the filters

## Your part (placement + logic)

1. Merge this PR into `feature/daryl-bedroom-decon`.
2. Drag `RoomDressing` into the scene at (0, 0, 0).
3. Add your XR components (XRGrabInteractable, sockets, interaction layers)
   to the prefab instances as needed. Colliders are already on them.
4. Verify in Play Mode on Quest. These prefabs were authored blind from the
   URP template, so check materials and collider behavior before trusting.

## Performance notes

- Every model is under 500 verts. Textures are 256 or 512 px. Safe for Quest.
- The radiation filter and candle flame use emissive materials, no extra
  lights needed. The two ceiling fixtures each carry one Point Light.
- OBJ is just the raw 3D model file format underneath. A prefab is Unity's
  pre-assembled object: mesh plus materials plus components, ready to drop in.
