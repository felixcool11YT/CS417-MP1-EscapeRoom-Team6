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
    Keycard, Candle, Locker, LockerDoor, PosterSafety,
    CeilingFixture_Bedroom
  DeconZone
    DeconArch, PipeRun_Upper, PipeRun_Lower, Vent, PosterWarning,
    CeilingFixture_Decon, HazardStrip_N_2.7, ... (12 strips)
```

If the nested prefabs ever fail to link on import, fall back to dragging
the individual prefabs and use the position table below plus
`Preview/layout_map.png` (top-down map of every prop).

## Position table (local coords inside RoomDressing)

Bedroom:
- Bed (-3.2, 0, -1.85), headboard against the back wall
- Nightstand (-1.95, 0, -2.55); TableLamp sits on it at y 0.625
- Rug (-3.2, 0.002, -0.35)
- Desk (-4.55, 0, 1.5) rotated 90 deg, against the left wall
- Chair (-3.85, 0, 1.5) facing the desk
- Newspaper (-4.55, 0.765, 1.35), Keycard (-4.35, 0.767, 1.7),
  Candle (-4.75, 0.765, 1.75) - all on the desk
- Locker (-0.85, 0, -2.7); LockerDoor (-1.13, 0, -2.71), hinge on the
  left edge, closed. Rotate on Y to swing open for the reveal.
- PosterSafety (-4.88, 1.7, -0.9) on the left wall
- CeilingFixture_Bedroom (-2.5, 3.0, -0.5)

DeconZone:
- DeconArch (1.35, 0, 0) rotated 90 deg - walk through it along x after
  the divider doorway
- PipeRun_Upper (2.5, 2.3, -2.82) and PipeRun_Lower (2.5, 2.02, -2.82)
  along the back wall; Vent (0.9, 2.0, -2.84)
- PosterWarning (4.95, 1.7, -1.2) on the right wall
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
- `CeilingFixture` - ceiling mount with an emissive panel AND a real
  Point Light (intensity 20, range 10, warm color). Two are placed in
  RoomDressing. Tweak the intensity to taste; delete the Light component
  if you want to do lighting yourself.

## Signifier color scheme (keep consistent with your sockets)

- Particulate filter: blue (`MAT_FilterParticulate` on the filter face)
- Chemical filter: green (`MAT_FilterChemical` on the canister body)
- Radiation filter: magenta, emissive. Use a sphere with
  `MAT_FilterRadiation` so it glows in the dark decon zone.

## Newspaper front page

The newspaper now uses `MAT_PaperFront` on its top face: a front page with
a placeholder headline ("CONTAMINATION SCARE SHUTS WING"). The body text is
lorem-style placeholder. Give me your real headline and clue text and I
will regenerate the texture in minutes.

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
