Silent Soul

Group 3 Mid-Term Group Project (Playable Obstacle Course) Cambodia Academy of Digital Technology Institute of Digital Technology Department of Computer Sciences Course: Fundamentals of Game Development Lecturer: Va Hongly

Quick Links
Item	Link
Git repository	https://github.com/ylyheng/Game-MidProject.git'

All links are public (no permission request needed) and will not expire.

1. Game Overview

Silent Soul is a first-person horror obstacle game. The player explores a dark, creepy map holding only a lighter, searching for three artifacts. Each artifact must be carried back and placed into its matching pocket (altar). But carrying an artifact wakes up the haunted dolls, which chase you whenever you are not looking at them.

2. Objective

Find all 3 artifacts, pick each one up, and insert each into its pocket. When all 3 pockets are filled, the game is completed and the end credits play.

3. Rules
Win: Insert all 3 artifacts into their pockets (3/3). The end credits then roll.
Lose: A doll catches you (gets within about 1.2 m).
Restart: After you are caught, the scene reloads automatically after about 1.5 seconds, so you start over.
Dolls ("weeping angel" style):
A doll freezes while you are looking at it.
It chases you while you are not looking.
Picking up artifacts activates the dolls. More dolls spawn at later stages (2 dolls, then 3 dolls).
Pockets only accept the item they require. The prompt reads "Required [item]".
Your lighter lights the dark. Use it to find your way.
4. Controls
Input	Action
W / A / S / D	Move (relative to where you look)
Mouse	Look around
Left Shift	Run
Left Ctrl	Sit / crouch (slower, cannot run)
F	Toggle lighter
E	Interact: pick up an item or insert it into a pocket
Esc	Unlock the mouse cursor
Left click (in the game window)	Lock the cursor again
5. Game Features
Game design: A dark map with three artifacts to find, matching pockets, and chasing dolls as the main obstacle. The challenge grows as dolls spawn in numbers (level 2 and level 3).
Player controls: Rigidbody-based first-person movement with acceleration, walk/run/sit speeds, mouse look, lighter, and an interaction raycast.
Physics: Rigidbody player movement, colliders (walls and floor), trigger and collision events, and NavMeshAgent pathfinding for the dolls.
Game logic: GameManager counts the inserted pockets (win condition). A doll catching the player triggers the fail condition and a scene reload (restart).
UI / feedback: Interaction prompts ("Pick up", "Required"), an inventory UI, sound effects (footsteps, doll movement, kill sound, insert sound, soundtrack), a flickering-light atmosphere and scrolling end credits.
6. Technical Details
Unity version: 6000.3.8f1 (Unity 6)
Language: C#
Platform tested: Windows
Main scene: Assets/Scenes/Main.unity
Key scripts (Assets/Scripts):
Script	Purpose
PlayerController.cs	Rigidbody movement: walk, run, sit
CameraFollow.cs (FirstPersonCamera)	Mouse look, sit camera, cursor lock
PlayerInteractor.cs / Interactable.cs	Look-at interaction with the E key
PickupItem.cs / Inventory.cs / InventoryUi.cs	Picking up items and the inventory
ItemPocket.cs	Insert artifacts into pockets
HoldObjectRightHand.cs / HandMovement.cs	Held item in hand; activates dolls
Lighter.cs	Lighter toggle (F)
GameManager.cs	Win condition (3 pockets) and game events
Enemy/DollChaser.cs	Doll AI: freezes when seen, chases when unseen, kills and reloads
Enemy/EnemySpawner.cs	Spawns dolls per level
UI/EndCreditController.cs	End credits after winning
ENV/FlickeringLight.cs, FootStepAudio.cs, Soundrack.cs	Atmosphere and audio
7. Setup & Play Instructions
Option A Run the playable build (no Unity needed)
Download the ZIP: [BUILD LINK]
Extract the entire ZIP (do not run the .exe from inside the ZIP).
Run [SilentSoul].exe. Keep the _Data folder and the other files next to it.
Play using the controls above.
Option B Open in the Unity Editor
Install Unity 6000.3.8f1 via Unity Hub.
Clone the repository:
bash
git clone https://github.com/ylyheng/Game-MidProject.git
In Unity Hub choose Add Add project from disk and select the cloned folder.
Open Assets/Scenes/Main.unity.
Press Play.
8. Project Structure
Game-MidProject/
|-- Assets/
|   |-- Scenes/        (Main.unity)
|   |-- Scripts/       (Player, Enemy, UI, ENV)
|   `-- Resources/     (artifact, Cobwebs, lamp, Locker_HQ, Skeleton, Swords, tree)
|-- Packages/
|-- ProjectSettings/
`-- README.md
9. Team Members & Contributions
Name	Contribution
San Chanthai	Player controls and physics
Y Lyheng	Map / level design
Sopheap Souchhy	Map / level design
THAL Sotarith	Game logic

Git history reflects each member's commits.

10. Assets, Tutorials & AI Credits

Assets

Models and sounds from free resources (see the Assets/Resources folders).
TextMesh Pro (Unity built-in).

AI assistance

AI was used to help write this README.
11. License

For educational purposes only.
