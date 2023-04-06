# The Nine Second Rule

[![Build project](https://github.com/notangoose/The-Nine-Second-Rule/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/notangoose/The-Nine-Second-Rule/actions/workflows/main.yml)

## Description

_The Nine Second Rule_ is a 2D platformer game made in Unity.

## Downloads (<https://nightly.link>)

- [Linux 64-bit](https://nightly.link/notangoose/The-Nine-Second-Rule/workflows/main/main/Build-StandaloneLinux64.zip)
- [macOS Universal](https://nightly.link/notangoose/The-Nine-Second-Rule/workflows/main/main/Build-StandaloneOSX.zip)
- [Windows](https://nightly.link/notangoose/The-Nine-Second-Rule/workflows/main/main/Build-StandaloneWindows.zip)
- [Windows 64-bit](https://nightly.link/notangoose/The-Nine-Second-Rule/workflows/main/main/Build-StandaloneWindows64.zip)

## Usage

### Controls

There is currently no information about controls in the game, so here you go:

| Control       | Keys           | Description                                 |
| ------------- | -------------- | ------------------------------------------- |
| (Double) Jump | W, Up Arrow    | Travel across large gaps and scale walls    |
| Fallthrough   | S, Down Arrow  | Fall through one way platforms              |
| Move Left     | A, Left Arrow  | Traverse the level and hold walls           |
| Move Right    | D, Right Arrow | Traverse the level and hold walls           |
| Select Level  | Escape         | Play another level or quit the game         |
| Reset         | R              | Respawn the player at the starting position |
| Dash          | Space          | Run faster than ever before                 |

### Game Elements

- **Platform**
  - Horizontal
  - Can vary in size
  - Both sides have collision
  - Always supports you :D
- **One Way Platform**
  - Like a **Platform**
  - Fall through it with the _Fallthrough_ key
- **Wall**
  - Vertical
  - Can vary in size
  - Both sides have collision
  - Can be scaled easily by moving towards it and jumping
- **Invisible Wall**
  - Like a **Wall** but is invisible
  - Currently only used in _Level Select_
  - Cannot be climbed
- **Spike**
  - Triangular
  - Don't touch!
- **Turret**
  - Shoots projectiles straight at regular intervals
- **Moving Platform**
  - Also like a **Platform**
  - Moves
- **Finish**
  - Circular
  - Get here to win!
