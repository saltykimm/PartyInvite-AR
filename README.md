# PartyInvite-AR (Birthday Surprise)

An interactive Augmented Reality (AR) experience built with **Unity** and **Vuforia Engine** to create a magical birthday surprise invitation.

## 🚀 Features

- **AR Image Target Tracking**: Uses Vuforia Engine to detect and stabilize targets.
- **Interactive Elements**:
  - Tap on the screen/cards to trigger animations, play sound, and load scenes.
  - Interactive presents, glasses, and cookies with custom animations.
  - Balloon Spawner: Tap buttons to spawn colorful balloons dynamically.
  - Fireworks system for celebratory visual feedback.
- **Scene Navigation**: Clean home page with scene loading and application quitting logic.

## 🛠️ Tech Stack & Dependencies

- **Engine**: Unity (2022+ recommended)
- **AR Library**: [Vuforia Engine 11.4.4](https://developer.vuforia.com/) (included locally as a `.tgz` package)
- **Text Rendering**: TextMesh Pro

## 📦 Getting Started

### Prerequisites

- Unity Hub and Unity Editor (compatible with version used in `ProjectSettings/ProjectVersion.txt`).
- Git & Git LFS installed on your machine.

### Installation

1. Clone this repository:
   ```bash
   git clone <your-repository-url>
   ```
2. Open **Unity Hub**.
3. Click **Add** -> **Add project from disk**.
4. Select the `BirthdaySuprise` folder.
5. Let Unity resolve and import the local Vuforia package (`Packages/com.ptc.vuforia.engine-11.4.4.tgz`).

## 📁 Repository Structure

- `Assets/`: Scenes, Prefabs, Shaders, and C# Scripts containing all the game logic.
- `Packages/`: Project dependency definitions and local Vuforia package.
- `ProjectSettings/`: Unity configuration settings.
