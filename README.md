# Dead Tired 🔦

A 3D survival horror prototype inspired by Five Nights at Freddy's, built in Unity (C#). 

![DeadTired](https://github.com/user-attachments/assets/a87ee044-4750-448a-9034-562d94487931)

## 📌 Overview
**Dead Tired** is a resource-management horror game where the player must balance overlapping threats (audio, visual, and environmental) to survive. This repository contains the core PC build, featuring custom concurrent AI logic, modular gameplay systems, and dynamic fail-states. 

*(Note: A standalone VR port was also developed for Meta Quest, adapting these core systems for spatial interaction).*

* **Engine:** Unity 6000.0.53f 
* **Language:** C#
* **Role:** Lead Programmer & Level Designer

## ⚙️ Key Systems & Architecture
If you are reviewing my code, I highly recommend checking out these specific systems located in `Assets/Scripts/`:

* **`MonsterController3.cs`**: A concurrent AI manager that acts as an observer, dynamically adjusting threat timers based on player actions (e.g., bathroom light state) and triggering dynamic fail-states.
* **`SceneManager.cs`**: Handles seamless transitions, UI fading, and scene state persistence.
* **Modular Hazard Logic**: Scripts like `ShowerOnOff.cs` and `Chap3Window.cs` utilize decoupled coroutines with randomized intervals (`minOnTime` / `maxOnTime`) to overlap gameplay tension without causing logic conflicts.

## 🎮 Play the Game
Playable PC & VR build is available on [itch.io](https://laowang91.itch.io/dead-tired).

## 🚀 How to Run the Project Locally
1. Clone this repository.
2. Open Unity Hub and click `Add Project from Disk`.
3. Select the cloned folder.
4. Open the `MainMenu` scene located in `Assets/Scenes/` to begin.
