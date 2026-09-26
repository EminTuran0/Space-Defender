# Space Defender

A 2D side-scrolling space shooter built with Unity 6 and the Universal Render Pipeline (2D).
You pilot a ship through an endlessly scrolling starfield, clearing waves of critters,
dodging asteroids, collecting power-ups and fighting a boss that shows up once you have
thinned the swarm enough.

## Gameplay

- **Endless scroll with a speed dial.** A single `worldSpeed` value in `GameManager` drives the
  parallax background, the spawn timers and enemy drift. Boosting raises it from `1` to `7`,
  so the whole world visibly accelerates while your energy drains.
- **Energy-gated boost.** Boost costs `0.5` energy per physics tick and needs at least `10`
  energy to engage; it regenerates automatically whenever you are not boosting.
- **Score-driven weapon upgrades.** Every 200 points promotes the phaser one level, up to
  level 5. Each level adds `+2` bullet speed, `+1` damage and `+1` simultaneous bullet, fanned
  out across a 12° spread. The level resets when a run ends.
- **Boss trigger.** Killing 16 critters spawns `Boss1` off the right edge: 100 HP, 20 contact
  damage, alternating between a straight `-6` charge and short randomised drifts.
- **Power-ups.** `Overcharge`, `Shield` and `Health` drop on a random 6–12 second timer.
  Shield absorbs a set number of hits, spawns a shield effect around the ship and counts down
  on its own HUD label.
- **Wave spawner.** `ObjectSpawner` walks a configurable list of waves (prefab, interval,
  objects per wave) and loops back to the first wave once the list is exhausted.

## Controls

| Action | Keyboard | Gamepad |
| --- | --- | --- |
| Move | `WASD` / Arrow keys | Left stick |
| Fire | `Right Shift` | `Fire1` |
| Boost (hold) | `Space` | `Fire2` |
| Pause | `Esc` or `P` | `Fire3` |

## Scenes

The build includes four scenes, in order:

1. `MainMenu` — start a new game or quit.
2. `Level1` — the playable level.
3. `Level 1 Complete` — shown after clearing the level.
4. `GameOver` — shown three seconds after the ship is destroyed.

The final score is persisted through `PlayerPrefs` under the key `FinalScore` and read back by
the `FinalScore` script on the end screens.

## Project layout

```
Assets/
  Scripts/
    GameManager.cs            score, pause, world speed, scene flow
    PlayerController.cs       movement, energy, boost, health
    UIController.cs           health and energy sliders
    AudioManager.cs           one-shot SFX through the Master mixer
    ObjectPooler.cs           reusable instance pool
    ObjectSpawner.cs          wave-based spawning
    ParallaxBackground.cs     scrolling backdrop
    Enemies/                  Critter1, Boss1
    Obstacles/                Asteroid, LostWhale
    PowerUps/                 spawner, pickup, mover, receiver, PowerUpType
    Weapons/                  PhaserWeapon, PhaserBullet, WeaponUpgradeManager
    Menus/                    MenuManager, MenuParallax
    Utils/                    FlashWhite, FloatInSpace, DestroyWhenAnimationFinished
  Scenes/                     MainMenu, Level1, Level 1 Complete, GameOver
  Art/, Animations/, Audio/, Prefabs/, Settings/
```

## Running it

1. Install **Unity 6000.2.15f1** (the exact version is pinned in
   `ProjectSettings/ProjectVersion.txt`). Other Unity 6 releases will offer to upgrade the
   project.
2. Open the project folder in Unity Hub — packages restore automatically from
   `Packages/manifest.json`.
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

## Built with

- Unity 6000.2.15f1
- Universal Render Pipeline 17.2.0 (2D renderer)
- Input System 1.16.0, TextMesh Pro, Unity UI
- [Shield Shader FREE](Assets/Shield%20Shader%20FREE) for the shield effect (see the asset's
  own documentation file for its terms)

## Notes

The project carries three names for historical reasons: the repository is `Space-Defender`, the
Unity solution is `GalaxyDefender`, and the player-facing product name in
`ProjectSettings` is still `SpaceQuest`.
