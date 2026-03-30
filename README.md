# SaferAmongUs

SaferAmongUs is a **proof-of-concept (PoC) client-side moderation and safety layer for Among Us**, built to demonstrate how chat abuse, dating/sexual solicitation, and unsafe lobby behaviors can be mitigated directly at the game level.

This project is **not intended as a cheat, exploit, or gameplay modification**, but as a **technical demonstration for Innersloth** of how moderation, reporting, and lobby-safety features could be integrated more deeply and effectively than simple word filters.

---

## Purpose

Public Among Us lobbies are frequently abused for:

* Dating and sexual solicitation ("hlo u girl", "how u get horny", etc.)
* Grooming-style behavior
* Spam and harassment
* Unsafe interactions in spaces used by minors

SaferAmongUs explores **systemic, structural mitigation**, rather than reactive moderation alone.

The long-term goal is to:

* Reduce exposure to inappropriate content **before it reaches other players**
* Provide a foundation for **LM-based detection and external reporting**
* Demonstrate feasibility with minimal performance impact

---

## Scope

This repository currently contains:

* Harmony patches targeting core chat, lobby, and moderation systems
* Rule-based detection hooks (BlockedWords + custom DaterCheck)
* UI and flow restrictions to reduce abuse vectors

Planned (not yet implemented):

* Language-model–based chat classification
* Context-aware detection (multi-message, multi-user)
* External reporting pipeline (outside lobby lifecycle)
* Human-review escalation for high-risk cases

---

## Key Features

### 1. Chat Interception & Blocking

Chat messages are intercepted **before broadcast**.

Messages are blocked when:

* They match existing `BlockedWords`
* They match custom dating/sexual solicitation heuristics (`DaterCheck`)

If the sender is the local player, a warning is displayed instead of silently failing.

This prevents:

* Normalization of sexual messages
* Other players being exposed at all

---

### 2. Extended Word Filtering

The default `BlockedWords.ContainsWord` logic is extended to:

* Include contextual dating/solicitation detection
* Apply the same logic consistently across:

  * Chat
  * Player names

This avoids trivial bypasses (e.g., usernames used as bait).

---

### 3. Lobby Visibility Control (PoC)

Lobby visibility is controlled at game creation time to demonstrate risk surface reduction through upfront configuration, rather than post-creation enforcement:

* Lobby visibility (Public / Private) is selected during the Create Game flow
* cannot be changed after the lobby is created
* A custom Public/Private toggle is injected into the Create Game UI
* Lobby visibility is enforced programmatically once the lobby starts

This shows how **UI-level friction** can drastically reduce abuse without heavy moderation.

---

### 4. Moderation Flow Adjustments

The built-in Ban/Kick system is modified to:

* Disable instant bans
* Force vote-based kicks instead
* Reduce impulsive or abusive moderation

This aligns moderation with **collective confirmation** rather than unilateral action.

---

### 5. Player Count Adjustment

As part of the PoC, lobby configuration constraints are adjusted:

* Minimum lobby size enforced at 7 players (max 15)
* 4–6 player lobbies, which are frequently abused and significantly harder to moderate, are no longer creatable

This demonstrates how safety constraints can coexist with normal gameplay configuration.

---

## Architecture Overview

* **Harmony**: Runtime patching of Among Us methods
* **Client-side interception**: No server modification required for PoC
* **Modular detection**: `DaterCheck` is intentionally isolated to allow replacement with:

  * Local ML models
  * Remote classification APIs

---

## Why Language Models

Rule-based filters alone are insufficient:

* Easy to evade
* High false positives
* No understanding of context or intent

Future iterations will integrate:

* Lightweight text classifiers (on-device or remote)
* Multi-message context windows
* Confidence-based escalation

The intent is **classification and reporting**, not censorship.

---

## Ethical & Safety Considerations

This project is explicitly designed with the assumption that:

* Among Us has a significant minor player base
* Sexual solicitation in public lobbies is unacceptable
* Prevention is preferable to punishment

No data collection or reporting is implemented in this PoC.

Any future reporting system **must**:

* Minimize data retention
* Avoid storing raw identifiers unnecessarily
* Comply with regional privacy laws

---

## Disclaimer

This repository:

* Is a **technical proof-of-concept**
* Is **not affiliated with or endorsed by Innersloth**
* Should not be used in live environments without authorization

All patches are provided for demonstration and discussion purposes only.

---

## Next Steps

Planned improvements:

* Replace `DaterCheck` heuristics with LM-based intent classification
* Add severity scoring and confidence thresholds
* Implement external, opt-in reporting outside lobby lifecycle
* Add telemetry for false-positive analysis (locally only)

---

## Build
### Prerequisites
Download the 6.x .Net SDK, or the 9.x or newer .Net SDK (both work, 9.x+ has support for older sdks) 
[6.x Installation Page](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
[9.x Installation Page](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
Git is also required as it 'clones' or copies the the source code of any git repo hub (like github) onto your PC for you to modify in any way shape or fourm (if the codes license allows you), [Git's Download link](https://git-scm.com/install/)

### Building
Clone the repo (or source code's release website) by running ```git clone https://github.com/Sonic3Modder/SaferAmongUs-Buildable``` on your PC by searching for command prompt on your windows menu and copying and pasting the command there, there will be some fancy output, nothing to worry about, its just debug info to see how its downloading. I assume that the linux users know how to do this. Now you need to type ```cd SaferAmongUS-Buildable``` and run ```dotnet build``` this will build it, once its build it should be in bin/net6.0 and the .dll should be in there. Thats it

---

## Contact

This project was created to start a **technical discussion**, not to bypass systems.

If you are part of Innersloth or interested in moderation tooling, feel free to reach out at `matchducking@proton.me` for a deeper technical breakdown.
