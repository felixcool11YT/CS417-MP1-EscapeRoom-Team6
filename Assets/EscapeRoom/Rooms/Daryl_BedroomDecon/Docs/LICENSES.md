# Asset Licenses

## Original room-kit assets

The models, textures, and materials directly inside this room's `Models`,
`Textures`, and `Materials` folders were generated procedurally on 2026-09-22.
They are original works released as CC0 (public domain).

Generated with:
- `/tmp/build_pr/make_models.py` (OBJ meshes)
- `/tmp/build_pr/make_textures.py` (PNG textures)
- `/tmp/build_pr/make_unity.py` (materials and meta files)

Note: Felix's `ImportedBedroomProps` textures in his own room folder come from
assorted web images with unclear licensing. This folder intentionally does not
reuse any of them.

## Poly Haven replacement models

The following 1K glTF assets are stored under `ThirdParty/PolyHaven`. Poly Haven
publishes its assets under CC0: https://polyhaven.com/license

- **Old Bed Frame**, Luca B: https://polyhaven.com/a/old_bed_frame
- **Metal Office Desk**, Ulan Cabanilla: https://polyhaven.com/a/metal_office_desk
- **School Chair 01**, Ethan Place: https://polyhaven.com/a/SchoolChair_01
- **Painted Wooden Nightstand**, Kirill Sannikov:
  https://polyhaven.com/a/painted_wooden_nightstand
- **Old Military Crate**, Poly Haven: https://polyhaven.com/a/old_military_crate
- **Industrial Caged Sconce**, Ulan Cabanilla:
  https://polyhaven.com/a/industrial_caged_sconce
- **Modular Industrial Pipes 01**, Jorge Camacho:
  https://polyhaven.com/a/modular_industrial_pipes_01
- **Vintage Day Bed**, Aron Łyczek:
  https://polyhaven.com/a/vintage_day_bed
- **Barrel 01**, Jorge Camacho:
  https://polyhaven.com/a/Barrel_01
- **Wet Floor Sign 01**, Fran Calvente:
  https://polyhaven.com/a/WetFloorSign_01
- **Medical Box**, Ulan Cabanilla:
  https://polyhaven.com/a/medical_box
- **Korean Fire Extinguisher 01**, UM JOORIN:
  https://polyhaven.com/a/korean_fire_extinguisher_01
- **Old Gas Mask**, Michał Wiśniewski:
  https://polyhaven.com/a/old_gas_mask
- **Small LPG Tank**, Ulan Cabanilla:
  https://polyhaven.com/a/small_lpg_tank
- **Modular Airduct Circular 01**, Riley Queen:
  https://polyhaven.com/a/modular_airduct_circular_01

Attribution is not required by CC0, but the sources are retained here for
project documentation and course submission transparency.

## Sketchfab replacement models

The following downloaded glTF assets are stored under `ThirdParty/Sketchfab`.
Each original `license.txt` file is retained beside its model and textures.

- **PB178 Wix Air Filter Low**, Makovetkyi Volodymyr, licensed under
  [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/):
  https://sketchfab.com/3d-models/pb178-wix-air-filter-low-529082da8d0a40db9279455602baaf14
- **Radioactive Explosive gernade**, Zohaib1, licensed under
  [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/):
  https://sketchfab.com/3d-models/radioactive-explosive-gernade-53edc60a9fa74bbe929441d4169109ad

Both models were imported as glTF assets, scaled to fit the existing gameplay
colliders, and used as visual replacements. Their interaction logic remains on
the original Unity GameObjects.

The downloaded **Russian GP-5 Gas Mask - (Filter Damaged)** was not added to
the project because its creator's model description explicitly excludes use in
school assignments. The chemical-filter visual instead uses the CC0 Small LPG
Tank listed above.
