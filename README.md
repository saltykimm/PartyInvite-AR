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

## ⚙️Changes after Postpresentation

- **Locking mechanism**: imageTarget locks creating a stable scanning reducing jitter.
- **dressCode**: A new object added for a new UI to show the dressCode of the birthday
- **Buttons**: Instead of a visibilty UI, each object now has their own interactive buttoin for players to push alongside labels of what they are & colour coded to see if it could be interacted or not
- **UI redesign**: UI had been remade completely to attract attention and create a more dream-like experience to entice people
