# Placement Guide: Bedroom Decon Theme Kit

Drag the prefabs from `Prefabs/` into your scene and position them. Every
prefab comes fully assembled: meshes with materials already wired to the
correct parts, plus a MeshCollider (convex on the small grabbable items).
You only handle placement and your XR/logic wiring. Nothing here touches
your scripts, sockets, or scene.

OBJ is just the raw 3D model file format underneath. A prefab is Unity's
pre-assembled object: mesh plus materials plus components, ready to drop in.

## The prefabs

- `Bed`, `Nightstand`, `TableLamp`, `Desk`, `Chair` - bedroom furniture
- `Locker` - body. `LockerDoor` is separate: its origin is at the hinge
  edge, so place it at the hinge position on the locker body and rotate on Y
  to swing open for the locker reveal.
- `DeconArch` - decon zone archway, hazard stripes on the beam
- `ChemCanister` - chemical filter, green signifier band plus hazard label
- `ParticulateFilter` - blue filter face
- `Keycard`, `Candle` (emissive flame), `Newspaper`

## Signifier color scheme (keep consistent with your sockets)

- Particulate filter: blue (`MAT_FilterParticulate` on the filter face)
- Chemical filter: green (`MAT_FilterChemical` on the canister body)
- Radiation filter: magenta, emissive. Use a sphere with
  `MAT_FilterRadiation` so it glows in the dark decon zone.

## Room shell (matches the concrete/brick direction from the team chat)

- Walls: `MAT_Wall_ConcreteDark`, one accent wall `MAT_Wall_BrickDark`
- Ceiling: `MAT_Ceiling_Concrete`
- Floor: `MAT_Floor_Tiles`
- Signs: quads with `MAT_SignDecon` over the decon entrance and
  `MAT_SignWarning` near the filters

## Your part (placement + logic)

1. Merge this PR into `feature/daryl-bedroom-decon`.
2. Drag each prefab into the scene where it belongs.
3. Add your XR components (XRGrabInteractable, sockets, interaction layers)
   to the prefab instances as needed. Colliders are already on them.
4. Verify in Play Mode on Quest. These prefabs were authored blind from the
   URP template, so check materials and collider behavior before trusting.

## Performance notes

- Every model is under 500 verts. Textures are 256 or 512 px. Safe for Quest.
- The radiation filter and candle flame use emissive materials, no extra
  lights needed.
