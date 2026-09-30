# MP1c integration evidence

Team 6: Daryl Okeke, Felix Romero (frome4), Robert (RC, rc75).

The intended experience is a focused escape from a failing post-apocalyptic bunker. The three rooms restore clearance, and the shared exit station prepares the player and the bunker for departure. Clearing security and preparing for the outside are separate, explicitly labelled steps.

This is an implementation and recording checklist, not a submitted self-evaluation. Award claims must match the final headset footage. Automated checks do not establish headset usability or the final grade.

## Combined Lock Mechanic Challenge — target 2 points

All three room rewards open the inner authorization door into a separate airlock. The departure station sits along the east wall before the stairs. The exterior hatch remains locked until one gated sequence reuses each contributor's mechanic:

| Contributor | Learned mechanic | Final application |
| --- | --- | --- |
| Robert / RC | Pack the requested supply types into a designated container | Pack the dedicated EXIT first-aid kit and EXIT radio in their marked slots |
| Daryl | Apply the ordered particulate, chemical, radiation filtration sequence | Select the fixed cartridges in that order; an incorrect input resets progress |
| Felix | Configure electrical breakers from a circuit clue and test the configuration | FILTERS on, HATCH on, AUX off, then TEST |

Inactive stages cannot be operated early. All three stages release the exterior hatch, which still requires its push interaction. The ending requires departure completion and an open hatch; the timer continues until the player reaches the outside trigger. The note from R distinguishes inner-door access, airlock preparation, and actual escape. The final step retains the set-and-test pattern while reducing the number of breakers. R's handwritten note describes the systems that still need power and the damaged auxiliary line, leaving the player to infer the switch states.

Record: demonstrate 3/3 authorization opening the inner door without victory, enter the airlock, pack both items, reject one incorrect filter selection, complete the sequence, reject a wrong breaker configuration, then complete it and open the released hatch. Identify all three contributors' mechanics in narration.

## Relevant Feedback — target 2 points

The design intent is focused attention. At the departure station, only the current stage is enabled; its amber indicator becomes green when complete. Status text explains the next action, filter lamps retain sequence progress, and brief spatial sounds distinguish accepted and rejected inputs. Hover highlighting identifies selectable controls. Feedback is attached to the action rather than a continuous global alarm.

Room feedback includes the supply count and reward-box opening, filter progress and keycard dispenser light, generator valve indicators and reduced leaks, breaker sounds, terminal acceptance/rejection, and the powered generator. Short confirmation sounds remain local. The quiet ambient drone supports the bunker mood without being the primary instruction channel.

Record: contrast an incorrect and correct input, show the changed text/light, and explain which object should receive attention next. Confirm the cues are audible without drowning out clues on the headset.

## Distinct Lock Signifiers — target 2 points

| Key/control | Intended lock | Disambiguation |
| --- | --- | --- |
| Newspaper date | Bedroom locker keypad | Nearby date clue and numeric keypad |
| Blue particulate / green chemical / magenta radiation filters | Matching bedroom filter sockets | Color, filter name, numbered calibration display, exclusive interaction layers |
| Keycard | KEYCARD authorization socket | Flat card shape and labelled socket; FinalKeycard layer |
| Small generator power cell | POWER CELL authorization socket | Capsule shape and explicit socket label; separate from the large generator core |
| Brass key | BRASS KEY authorization socket | Brass key shape and label; its own interaction layer |
| Storage first aid, radio, pills, flare gun | Storage kit | A handwritten clue describes four survival needs; matching slot labels and the supply count confirm acceptance |
| EXIT first aid and EXIT radio | Departure kit's corresponding slots | Dedicated EXIT tags, matching amber shelf/slot labels, and physical separation beyond the inner door; each exit slot has its own interaction layer |
| Large generator core | Generator installation port | Core/port shape and GENERATOR CORE label; cabinet unlock precedes access |
| Sealant | Leaking repair zones | Repair tool, water effects, visible patches |
| Final fixed cartridges | Purge control buttons | Visibly secured behind guard bars, with protruding labelled push buttons below; instruction reads SECURED FILTERS / USE BUTTONS BELOW |
| Final FILTERS / HATCH / AUX switches | Exit power circuit | Named circuit labels, ON/OFF marks, and TEST |

The keypad's visible labels are bound to their button values and positioned over the matching colliders. The authorization panel has a catch shelf. Loose grabbable props use velocity tracking and continuous collision detection, so walls resist movement while an item is held. Objects that fall below the level return to their original position; the brass key returns to the authorization shelf. Recovery releases a held item before returning it and preserves puzzle progress. The runtime keycard receives the same protection. The storage box has a catch volume above its open tray and accepts overlapping supplies on both trigger entry and continued overlap. The generator port accepts an unlocked core while held and secures it immediately. Accepted clearances stay secured. The small power cell seats horizontally through its ring.

Record: show each authorization object beside its matching label, compare the storage supplies to the EXIT-tagged duplicates, and show that a wrong item is not accepted in an exit socket. Have a new player match the objects without verbal hints. Similar geometry still needs this visual usability check; layer masks alone are not proof of a clear signifier.

## Predictable Mass Collisions — target 2 points

Most loose props use non-default masses. Examples in kilograms:

| Object | Mass |
| --- | ---: |
| Pills | 0.10 |
| Tape | 0.15 |
| Brass key / spawned keycard | 0.20 |
| Particulate filter | 0.35 |
| Radio | 0.45 |
| Chemical filter | 0.55 |
| Radiation filter | 0.75 |
| First-aid kit | 0.90 |
| Chemical canister | 1.20 |
| Generator core | 1.40 |
| Medical storage box | 1.50 |

The loose-object extremes give **1.50 / 0.10 = 15:1**, below 20:1. Secured supplies become kinematic only after placement. Fixed scenery is static rather than an arbitrarily massive dynamic body. The room integration check inventories bodies and compares controlled impacts against light and heavy bodies using these mass values.

Record: show a light item and a heavier-looking item reacting to contact, then state the example masses and 15:1 ratio. Inspector values and an automated collision test supplement, but do not replace, visible gameplay evidence.

## Integrated Theme — target 2 points

Bedroom decontamination, emergency storage, generator restoration, clearances, and departure preparation serve the same bunker scenario. Reused metal surfaces, practical repair props, warning colors, local electrical and water sounds, notes from R, and the final hatch/wasteland reinforce it. The inner door admits the player to a separate preparation airlock. Its dedicated supplies and secured filtration controls prepare the player for the surface; completing the station releases the exterior hatch. Inner-door access gives a short confirmation, while the escape presentation occurs outside. Storage and maintenance clues use worn paper and handwritten text. The circuit clue beside the generator fuse cabinet describes two surviving paths through the grid; the table note directs players to that wall clue. The departure note uses the same paper and handwriting in its physical and enlarged reading views.

Record: begin with a brief room overview, connect the three room functions to the bunker, then end at the open hatch and wasteland. Explain why the final station exists in the story.

## Accurate Self Evaluation — target 1 additional point

The five base axes total 10 points. The sidequest depends on the team's submitted evaluation being within two points of the grader, so it cannot be verified in advance. Assign final claims after reviewing the actual recording and noting any remaining usability weaknesses.

## Automated verification

The September 30 interaction and clue revision passed 116 room/menu/ending/restart/signifier/physics checks, 18 departure-station checks, and 39 grab-safety checks. Scene inspection found no missing scripts or unresolved nonempty event targets. All three actual room rewards were passed through the final authorization sockets before exercising the departure sequence. The checks also exercise each visible keypad digit and the Clear/Enter buttons, verify label-to-collider alignment, drop the brass key at speed with recovery disabled, test its below-floor return separately, and verify the secured reward poses and horizontal power-cell axis. The revised checks confirm that authorization opens the inner door without victory, early hatch pushes and early exit-trigger entry are rejected, preparation releases the hatch without stopping the timer, the hatch must physically open before escape, and restart relocks everything.

In a separate physics scene, equal 0.5 kg projectiles at 4 m/s produced target speeds of about 3.49 m/s for a 0.10 kg body and 1.03 m/s for a 1.50 kg body. This checks the intended mass response under controlled contact, while the headset pass must still check hand-driven collisions in the rooms.

Grab-safety checks use an XR test interactor to hold the actual pills against a solid wall while its controller target moves through the wall. They also exercise the actual box and generator trigger volumes while items are held, verify that installed items stay seated after later controller movement, and test recovery without resetting packing progress.

These checks invoke puzzle and interaction APIs in Play Mode. They do not simulate a person's reach, sight lines, controller tracking, or traversal of the stairs.

## Headset acceptance pass

- Start the APK from its menu; confirm both controllers and locomotion work.
- Read the newspaper and R's note; verify text, X/read binding, close interaction, and comfortable UI depth.
- Check the bedroom keypad order, filter pickup/retry, audible feedback, and spawned keycard.
- Carry the pills into the open storage box while holding them; confirm they snap in once and remain there after moving the controller. Pack all four storage objects. Confirm the brass key cannot be taken before completion and stays reachable afterward.
- Hold a loose item against a wall; its body should stop even if the simulated controller passes through. If an item is lost below the level, find it at its original location (brass key: authorization shelf), with puzzle progress preserved.
- Follow the generator chain from leaks through valves, breakers, radio, terminal, core, and power-cell reward. Check that the core cannot be grabbed before the cabinet opens.
- Attempt incorrect final-socket matches and confirm authorization opens only the inner door, and finish the full departure sequence. Check that the hatch refuses pushes before preparation is complete.
- Walk the stairs, operate the hatch, and reach the ending without passing through geometry or becoming stuck.
- Check timer, optional collectibles, restart, and a loss/retry. Check floor-level objects can be retrieved seated or using the distance grab.

## Recording and submission

Keep the narrated group video under five minutes. Plan roughly 30 seconds for the bunker and room mechanics, two minutes for the combined challenge and failures, one minute for signifiers/feedback/masses, and the remainder for the exit. These are planning durations, not evidence timestamps.

After editing, record the actual timestamp for each exact rubric title in the Group Integration Summary. Include team number, all contributor names, the final itch.io and GitHub links, points claimed, and what the visible evidence demonstrates; export PDF. Do not invent timestamps or mark headset checks complete before running them.

Upload the tested APK to each required teammate itch.io page, list contributors, and include a cover, description, and the video or five gameplay screenshots. Submit the narrated video and summary PDF on Canvas. Individual contribution/ownership disclosures and any exclusive story claims belong in the appropriate individual contribution statements and must match the team's actual work.
