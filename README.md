# Kids Life Simulator (Unity 3D)

A complete, ready-to-run Unity 3D mini game where the player completes daily tasks, earns brownie points, and keeps mom's anger low.

## Project Structure

```
Assets/
├── Materials/
│   ├── Friend.mat
│   ├── Ground.mat
│   ├── PlayArea.mat
│   ├── Sink.mat
│   ├── Stove.mat
│   └── Toy.mat
├── Scenes/
│   └── Home.unity
└── Scripts/
    ├── GameManager.cs
    ├── InteractiveObject.cs
    ├── PlayerController.cs
    ├── TaskManager.cs
    └── UIManager.cs
```

## Open and Play

1. Open **Unity Hub**.
2. Click **Add project from disk** and choose this repository folder.
3. Open the project with **Unity 2022.3 LTS** (or newer compatible LTS).
4. In the Project window, open `Assets/Scenes/Home.unity`.
5. Click **Play**.

## Controls

- **W/A/S/D**: Move
- **Mouse**: Look around
- **Space**: Jump
- **E**: Interact with nearby task objects
- **Esc**: Unlock mouse cursor

## Gameplay Loop

- Complete tasks in order:
  1. Morning Routine (Sink)
  2. Room Cleaning (Toy)
  3. Help Mom Cook (Stove)
  4. Help Friend (Friend)
  5. Play Responsibly (PlayArea)
- Correct task completion gives brownie points.
- Wrong interaction increases mom's anger.
- Anger meter colors:
  - Green: calm
  - Yellow: warning
  - Red: danger
- At 100% anger, the game ends.
