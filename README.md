# 👻 Silent Soul

### First-Person Horror Obstacle Course Game

**Group 3 – Mid-Term Group Project**

Cambodia Academy of Digital Technology (CADT)
Institute of Digital Technology
Department of Computer Sciences

**Course:** Fundamentals of Game Development
**Lecturer:** Va Hongly
**TA:** Lim Chanvina

---

## 📌 Project Information

| Item                 | Details                             |
| -------------------- | ----------------------------------- |
| Project Name         | Silent Soul                         |
| Project Type         | First-Person Horror Obstacle        |
| Game Engine          | Unity 6                             |
| Unity Version        | `6000.3.8f1`                        |
| Programming Language | C#                                  |
| Platform             | Windows                             |
| Main Scene           | `Assets/Scenes/Main.unity`          |

**Git Repository:** [Game-MidProject](https://github.com/ylyheng/Game-MidProject.git)

---

## 📖 Table of Contents

1. [Game Overview](#-game-overview)
2. [Game Objective](#-game-objective)
3. [Game Rules](#-game-rules)
4. [Controls](#-controls)
5. [Game Features](#-game-features)
6. [Technical Implementation](#-technical-implementation)
7. [Installation and Setup](#-installation-and-setup)
8. [Project Structure](#-project-structure)
9. [Team Members and Contributions](#-team-members-and-contributions)
10. [Assets and Credits](#-assets-and-credits)
11. [License](#-license)

---

## 🎮 Game Overview

**Silent Soul** is a first-person horror obstacle game where players explore a dark and terrifying environment using only a lighter to guide their way.

The main objective is to discover three hidden artifacts and return each one to its corresponding pocket (altar).

However, collecting an artifact awakens haunted dolls that begin hunting the player. These dolls follow a unique **Weeping Angel-style mechanic**: they freeze when the player looks directly at them but move and chase the player whenever they are not being watched.

Players must carefully explore the environment, avoid the dolls, collect all three artifacts, and survive to complete the game.

---

## 🎯 Game Objective

To complete the game, players must:

1. Explore the dark environment using a lighter.
2. Find and collect all three hidden artifacts.
3. Avoid the haunted dolls while carrying artifacts.
4. Return each artifact to its matching pocket (altar).
5. Insert all three artifacts to unlock the ending.

**Victory Condition:** Successfully fill all three pockets (3/3). The end credits will automatically play.

---

## 📜 Game Rules

### 🏆 Winning

- Collect all three artifacts.
- Insert each artifact into its matching pocket.
- Once all three pockets are filled, the game is completed and the end credits begin.

### 💀 Losing

- A doll catches the player when it gets within approximately 1.2 meters.
- The game automatically reloads the scene approximately 1.5 seconds after the player is caught.

### 👹 Doll Behavior

- Dolls freeze when the player is looking directly at them.
- Dolls chase the player when they are not being watched.
- Collecting artifacts activates the dolls.
- Additional dolls spawn as the player progresses:
  - Level 1: 0 dolls.
  - Level 2: 1 doll.
  - Level 3: 3 dolls.

### 🗝️ Artifact and Pocket Rules

- Each pocket only accepts its corresponding artifact.
- Attempting to insert the wrong artifact displays a message such as `Required [Item]`.
- All three pockets must be filled to win.

### 🔦 Lighter

- The player uses a lighter to illuminate the dark environment.
- The lighter can be toggled on or off whenever needed.

---

## 🎮 Controls

| Key / Input  | Action                               |
| ------------ | ------------------------------------ |
| `W`          | Move Forward                         |
| `A`          | Move Left                            |
| `S`          | Move Backward                        |
| `D`          | Move Right                           |
| `Mouse`      | Look Around                          |
| `Left Shift` | Run                                  |
| `Left Ctrl`  | Sit / Crouch                         |
| `F`          | Toggle Lighter                       |
| `E`          | Interact / Pick Up / Insert Artifact |
| `Esc`        | Unlock Mouse Cursor                  |
| `Left Click` | Lock Mouse Cursor Again              |

**Note:** Sitting makes the player move more slowly and prevents running.

---

## ✨ Game Features

### 🏚️ Environment and Level Design

- Dark and creepy horror environment.
- Three hidden artifacts and their matching pockets.
- Atmospheric lighting and flickering lights.
- Exploration-based obstacle gameplay.

### 🏃 Player Mechanics

- Rigidbody-based first-person movement.
- Walking, running, and crouching.
- Mouse-controlled camera movement.
- Toggleable lighter.
- Raycast-based interaction system.
- Item pickup and inventory system.

### 👹 Enemy AI

- Weeping Angel-style doll behavior.
- Dolls freeze when observed.
- Dolls chase the player when not observed.
- NavMeshAgent-based pathfinding.
- Increasing enemy difficulty through multiple spawning stages.

### 🎒 Inventory and Interaction

- Pick up and carry artifacts.
- Display inventory information.
- Insert artifacts into matching pockets.
- Show interaction prompts and required-item messages.

### 🔊 Audio and Visual Effects

- Footstep sound effects.
- Doll movement sounds.
- Player death sound.
- Artifact insertion sound.
- Background soundtrack.
- Flickering-light effects.
- Scrolling end credits.

### 🏁 Game Progression

- GameManager tracks inserted artifacts.
- EnemySpawner controls enemy spawning.
- Automatic restart after player death.
- End credits after successful completion.

---

## ⚙️ Technical Implementation

The project was developed using **Unity 6 (6000.3.8f1)** and **C#**.

### Technologies and Systems

| Technology     | Purpose                          |
| -------------- | -------------------------------- |
| Unity 6        | Game development engine          |
| C#             | Gameplay programming             |
| Rigidbody      | Player physics and movement      |
| Collider       | Collision detection              |
| Trigger Events | Interaction and detection        |
| NavMeshAgent   | Enemy navigation and pathfinding |
| Raycasting     | Object interaction               |
| TextMesh Pro   | UI text rendering                |

### Main Scripts

The following scripts implement the main gameplay systems.

| Script                      | Responsibility                                    |
| --------------------------- | ------------------------------------------------- |
| `PlayerController.cs`       | Rigidbody movement: walking, running, and sitting |
| `CameraFollow.cs`           | First-person camera and mouse look                |
| `PlayerInteractor.cs`       | Detects interactable objects                      |
| `Interactable.cs`           | Base interaction behavior                         |
| `PickupItem.cs`             | Handles artifact pickup                           |
| `Inventory.cs`              | Manages collected items                           |
| `InventoryUi.cs`            | Displays inventory information                    |
| `ItemPocket.cs`             | Handles artifact insertion                        |
| `HoldObjectRightHand.cs`    | Displays held objects and activates dolls         |
| `HandMovement.cs`           | Controls hand movement                            |
| `Lighter.cs`                | Controls lighter on/off functionality             |
| `GameManager.cs`            | Tracks progress and win conditions                |
| `Enemy/DollChaser.cs`       | Doll AI, chasing, freezing, and player detection  |
| `Enemy/EnemySpawner.cs`     | Spawns dolls based on game progression            |
| `UI/EndCreditController.cs` | Controls end credits                              |
| `ENV/FlickeringLight.cs`    | Creates flickering-light effects                  |
| `FootStepAudio.cs`          | Controls player footsteps                         |
| `Soundrack.cs`              | Manages background soundtrack                     |

---

## 🚀 Installation and Setup

There are two ways to play or run the project.

### Option A: Play the Built Game (No Unity Required)

1. Download the game: 
   [**Download Silent Soul v1.0**](https://github.com/ylyheng/Game-MidProject/releases/download/v1.0/Game.zip)

2. Extract the downloaded ZIP file.
3. Open the extracted folder.
4. Run `Game-MidProject.exe`.
5. Make sure all required files and folders remain together.

> **Important:** Do not run the executable directly from inside the ZIP file.

### Option B: Run the Project Using Unity Editor

**Requirements:**

- Unity Hub.
- Unity Editor `6000.3.8f1`.
- Git.

**Step 1: Clone the repository**

Open a terminal and execute:

```bash
git clone https://github.com/ylyheng/Game-MidProject.git
```

**Step 2: Open the project in Unity Hub**

1. Launch Unity Hub.
2. Select **Add → Add project from disk**.
3. Choose the cloned `Game-MidProject` folder.
4. Open the project using Unity `6000.3.8f1`.

**Step 3: Open the Main Scene**

Navigate to:

```text
Assets/Scenes/Main.unity
```

**Step 4: Play the Game**

1. Open `Main.unity`.
2. Press the **Play** button in Unity.
3. Use the controls described above to start playing.

---

## 📁 Project Structure

```text
Game-MidProject/
│
├── Assets/
│   ├── Scenes/
│   │   └── Main.unity
│   │
│   ├── Scripts/
│   │   ├── PlayerController.cs
│   │   ├── CameraFollow.cs
│   │   ├── PlayerInteractor.cs
│   │   ├── Interactable.cs
│   │   ├── PickupItem.cs
│   │   ├── Inventory.cs
│   │   ├── InventoryUi.cs
│   │   ├── ItemPocket.cs
│   │   ├── Lighter.cs
│   │   ├── GameManager.cs
│   │   ├── Enemy/
│   │   ├── UI/
│   │   └── ENV/
│   │
│   └── Resources/
│       ├── Artifact
│       ├── Cobwebs
│       ├── Lamp
│       ├── Locker_HQ
│       ├── Skeleton
│       ├── Swords
│       └── Tree
│
├── Packages/
├── ProjectSettings/
└── README.md
```

---

## 👥 Team Members and Contributions

**Group 3**

| No. | Name            | Contribution                |
| --- | --------------- | --------------------------- |
| 1   | San Chanthai    | Player controls and physics |
| 2   | Y Lyheng        | Map and level design        |
| 3   | Sopheap Souchhy | Map and level design        |
| 4   | THAL Sotarith   | Game logic                  |

**Team Collaboration:** Git history reflects the contributions and commits made by individual team members.

---

## 🎨 Assets and Credits

### Assets

- 3D models and sound effects were obtained from free resources.
- Additional game assets are included in the `Assets/Resources` folder.
- TextMesh Pro is used for in-game text and UI rendering.

### AI Assistance

- AI tools were used to assist in writing and organizing this README documentation and catch bug during development.

---

## 📄 License

This project is developed for **educational purposes only** as part of the Fundamentals of Game Development course at the Cambodia Academy of Digital Technology (CADT).

---
