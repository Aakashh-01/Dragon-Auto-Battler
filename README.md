# 🐉 Dragon Auto Battler

A strategic 3D auto-battler built in Unity where dragons fight automatically using AI state machines.

## 🎮 Features
* **Smart AI:** Dragons patrol, chase, and engage enemies automatically.
* **Combat System:**
    * **Auto-Attacks:** Range-based biting and clawing.
    * **Abilities:** Fire Breath (AoE), Tail Whip (Knockback), and Fly Attack (Ultimate).
* **Player Control:**
    * **Passive Mode:** Click ground to move/retreat (AI ignores enemies).
    * **Aggressive Mode:** Click enemy to chase and attack.
* **Visuals:** HDR Bloom, Emission effects, and floating damage numbers.

## 🕹️ Controls
* **Left Click (Ground):** Move / Retreat.
* **Left Click (Enemy):** Attack.
* **ESC:** Quit Game.

## 🛠️ Tech Stack
* **Engine:** Unity 6 (2022+)
* **Language:** C#
* **Architecture:** Finite State Machine (Idle -> Move -> Combat -> Cooldown).
* **Optimization:** Cached Components, Object Pooling for VFX, 2D Audio Spatialization.

## 🚀 How to Play
1.  Download the **Build.zip** from the Google Drive link below.
2.  Extract and run `DragonFight.exe`.
3.  Defeat the Red Dragon to win!
