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


### 🤖 AI Workflow & Usage Note
**Tools Used:** Gemini (Google AI)

**How it improved efficiency:**
* **Rapid Prototyping:** Used AI to generate the initial boilerplate code for the Finite State Machine (FSM) structure, saving hours of manual setup.
* **Debugging:** AI assisted in diagnosing complex Unity-specific issues, such as the "Animation Stun-Lock" bug and Audio Source conflicts, providing solutions like `CrossFade` vs `SetTrigger` to stabilize combat logic.
* **Optimization:** Leveraged AI suggestions to implement performance improvements, such as Caching `GetComponent` calls and using Object Pooling concepts for VFX.
* **Documentation:** Used AI to help structure this README and generate clean, commented code for better readability.
