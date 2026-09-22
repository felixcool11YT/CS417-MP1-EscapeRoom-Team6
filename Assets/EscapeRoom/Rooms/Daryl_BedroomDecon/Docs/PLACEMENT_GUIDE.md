# Placement Guide: Bedroom Decon Theme Kit

Drag the models from `Models/` into your scene, then assign the materials from
`Materials/` per the table below. All models are in meters, Y-up, origin at
floor center (except the locker door, see below).

## Signifier color scheme (keep consistent with your sockets)

- Particulate filter: blue. Model `PROP_ParticulateFilter`, material
  `MAT_FilterParticulate` on the FilterFace part.
- Chemical filter: green. Model `PROP_ChemCanister`, material
  `MAT_FilterChemical` on the Body. Put `MAT_CanisterLabel` (hazard stripes)
  on the Label part.
- Radiation filter: magenta, emissive. Use a sphere with
  `MAT_FilterRadiation` so it glows in the dark decon zone.

## Room shell (matches the concrete/brick direction from the team chat)

- Walls: `MAT_Wall_ConcreteDark`, one accent wall `MAT_Wall_BrickDark`
- Ceiling: `MAT_Ceiling_Concrete`
- Floor: `MAT_Floor_Tiles`
- Decon zone trim and arch: `MAT_MetalPanels` on posts, `MAT_HazardStripes`
  on the arch beam (`PROP_DeconArch`)
- Signs: quads with `MAT_SignDecon` over the decon entrance and
  `MAT_SignWarning` near the filters

## Furniture material assignments

- `PROP_Bed`: Frame/Headboard/Legs use `MAT_Wood`, Mattress uses
  `MAT_Fabric`, Blanket uses `MAT_Fabric`
- `PROP_Nightstand`: `MAT_Wood`, knobs `MAT_MetalPanels`
- `PROP_TableLamp`: Base/Stem `MAT_MetalPanels`, Shade `MAT_Fabric`,
  Bulb gets an emissive material or a real point light
- `PROP_Desk`, `PROP_Chair`: `MAT_Wood`
- `PROP_Locker` + `PROP_LockerDoor`: `MAT_LockerMetal`. The door model's
  origin is at its hinge edge: place the door object at the hinge position on
  the locker body and rotate it on Y to swing open for the locker reveal.
- `PROP_Candle`: Wax/Dish use `MAT_CandleWax`, Flame uses `MAT_Flame`
  (emissive). One instance only, it is a hero prop.
- `PROP_Newspaper`: `MAT_Paper`
- `PROP_Keycard`: `MAT_Keycard`

## Performance notes

- Every model is under 500 verts. Textures are 256 or 512 px. Safe for Quest.
- The radiation filter and candle flame use emissive materials, no extra
  lights needed.
