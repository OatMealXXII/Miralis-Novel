# IMPLEMENTATION\_GUIDE

This guide details how to configure a Unity Project to run the **Miranova Engine (VSNL)**.

> **Prerequisite**: Ensure all C# scripts are in `Assets/Miralis/Scripts`.

***

## 1. Project Settings & Layers

Before building the scene, define the sorting logic for 2D elements.

* [ ] **Open:** `Edit` -> `Project Settings` -> `Tags and Layers`.
* [ ] **Sorting Layers** (Order from Top to Bottom):
  * `Default` (Base)
  * `Background` (BG Images)
  * `Video` (Movie Textures)
  * `Character` (Sprites / Live2D)
  * `VFX` (Particle Systems)
  * `UI` (Canvas elements, Dialogue Box)
* [ ] **Tags**:
  * Add Tag: `SpawnRoot` (For dynamic object spawning).

***

## 2. The Hierarchy Structure (Bootstrap Scene)

Create a new Scene named `Boot.unity`. Set up the hierarchy exactly as follows:

### 2.1 The Engine Root

* [ ] Create an **Empty GameObject** named `[MiranovaEngine]`.
* [ ] **Add Component**: `Engine.cs`.
* [ ] **Important**: To allow Inspector configuration, attach the Service components manually so `Engine` finds them instead of creating empty ones.
  * Add `UIManager`
  * Add `CharacterManager`
  * Add `AudioManager`
  * Add `BackgroundManager`

### 2.2 The UI Canvas structure

* [ ] Create a **UI -> Canvas** named `MainCanvas`.
  * Render Mode: `Screen Space - Overlay`.
  * Canvas Scaler: `Scale With Screen Size` (1920 x 1080).
  * Sorting Layer: `UI`.
* [ ] Create Child: **Panel** named `BackgroundPanel` (Remove Image source, set Color alpha to 0).
  * _This acts as a container if you aren't using Sprite Renderers for BG._
* [ ] Create Child: **Panel** named `DialoguePanel` (Anchor: Bottom Stretch).
  * Add `Image` (Black, Alpha 0.8) for the background text box.
  * Add Child: `Text (TMP)` named **SpeakerName**.
  * Add Child: `Text (TMP)` named **DialogueLine**.
* [ ] Create Child: **Panel** named `ChoicePanel` (Anchor: Center Middle).
  * Add `VerticalLayoutGroup` (Spacing 10, Alignment Center).
  * Add Child: **Button** named `ChoiceButton_Prefab` (Make this a Prefab in `Assets/Prefabs/UI`, then delete from scene).

***

## 3. Prefab Setup & Inspector Wiring

Now link the scene objects to the `[MiranovaEngine]` components.

### 3.1 Configure UIManager

Select `[MiranovaEngine]`. Locate the **UIManager** component.

* [ ] **Dialogue Panel**: Drag `MainCanvas/DialoguePanel`.
* [ ] **Speaker Text**: Drag `MainCanvas/DialoguePanel/SpeakerName`.
* [ ] **Dialogue Text**: Drag `MainCanvas/DialoguePanel/DialogueLine`.
* [ ] **Choice Panel**: Drag `MainCanvas/ChoicePanel`.
* [ ] **Choice Container**: Drag `MainCanvas/ChoicePanel` (same object).
* [ ] **Choice Button Prefab**: Drag the `ChoiceButton` prefab from your Project folder.

### 3.2 Configure CharacterManager

Select `[MiranovaEngine]`. Locate the **CharacterManager** component.

* [ ] **Character Root**: Create an empty child under `MainCanvas` named `CharacterRoot` and drag it here. This ensures sprites appear in the UI canvas stack.
* [ ] **Metadata DB**: Drag your `CharacterMetadata.asset` (Create via `Right Click -> Create -> VSNL -> CharacterMetadata` if available, or leave empty for dynamic mode).

### 3.3 Audio Setup

* [ ] **Music Folder**: Ensure you have `Assets/Miralis/Resources/Audio/BGM` for auto-loading.

***

## 4. Addressables Setup (Step-by-Step)

This system allows for efficient asset loading. It requires specific setup steps.

{% stepper %}
{% step %}
### Installation

* Go to **Window** -> **Package Manager**.
* Select **Packages: Unity Registry** (Top Left).
* Search for `Addressables`.
* Click **Install**.
{% endstep %}

{% step %}
### Initialization

* Go to **Window** -> **Asset Management** -> **Addressables** -> **Groups**.
* A window will pop up. If you see a button **"Create Addressables Settings"**, click it.
* You will now see a `Default Local Group`.
{% endstep %}

{% step %}
### Create Groups (Optional but Recommended)

* Right-click in the empty space of the Groups window -> **Create New Group** -> **Packed Assets**.
* Name it `Backgrounds`.
* Repeat for `Audio` and `Characters`.
{% endstep %}

{% step %}
### Mark & Rename Assets

Example: you have an image `school_day.png` in your Project:

* Drag the file from your Project Window into the `Backgrounds` group in the Addressables window.
* The entry will have a long name like `Assets/Miralis/Resources/Backgrounds/school_day.png`.
* **Right-click the entry** -> **Rename**.
* Change it to just `school_day`.

_This is the "Address". The script command `@bg "school_day"` looks for this exact name._
{% endstep %}

{% step %}
### Build Content (Vital)

The engine won't see your files unless you build them.

* In the Addressables Groups window top bar:
  * Click **Build** -> **New Build** -> **Default Build Script**.
* Wait for it to finish.

Tip: Whenever you add new files or rename them, run a Build update.
{% endstep %}
{% endstepper %}

***

## 5. Final Code Adjustment (Crucial)

Since we manually added components to configure them, update `Engine.cs` to prevent duplicates.

Open `Assets/Miralis/Scripts/Core/Engine.cs` and find `InitializeEngineAsync`. Change calls like:

{% code title="Before - Engine.cs" %}
```csharp
var uiManager = gameObject.AddComponent<UIManager>();
```
{% endcode %}

To:

{% code title="After - Engine.cs" %}
```csharp
var uiManager = gameObject.GetComponent<UIManager>();
if (!uiManager) uiManager = gameObject.AddComponent<UIManager>();
```
{% endcode %}

Repeat this pattern for all Managers (Character, Audio, etc).

***

## 6. Logic Linking & Engine Configuration

How to configure the values in the `Engine` Inspector:

### 6.1 Start Script

* **Start Script**: Drag your `.vsnl` text file (e.g., `Start.vsnl`) from the Project window into this slot.
  * _If you leave this empty, the engine will try to load "Start" from Resources by default._
* **Auto Start**: Keep checked (`True`) to run the script immediately when the game begins.

### 6.2 Service Overrides (Optional)

You see a list of Managers like `Save Load Manager`, `UI Manager`, etc.

* **Automatic Mode (Recommended)**: If all these components are on the **SAME GameObject** as the Engine, you can **leave them as `None`**. The Engine will find them automatically.
* **Manual Mode**: You can drag the components from the Inspector into these slots if you want to be 100% sure they are linked.
  * _Example: Drag the `UIManager` title from the component list into the `Ui Manager` slot._

***

## 7. Troubleshooting & Inspector Fields

You might see some empty fields in the Inspector (like in your screenshot). Here is how to handle them:

### 7.1 Text Manager (Empty?)

If the **Text Manager** field in `UIManager` is `None`:

1. **Add Component**: Select `[MiranovaEngine]`. Add the `TextDisplayManager` script to it.
2. **Assign**: Drag the `[MiranovaEngine]` object itself into the **Text Manager** slot of `UIManager`.
3. **Configure TextDisplayManager**:
   * **Speaker Text**: Drag `MainCanvas/DialoguePanel/SpeakerName`.
   * **Dialogue Text**: Drag `MainCanvas/DialoguePanel/DialogueLine`.
   * _Note: This separates the "Typing Effect" logic from the main UI logic._

### 7.2 Addressables (Detailed)

If you want to use the Addressable system instead of Resources:

* Install: `Window` -> `Package Manager` -> `Unity Registry` -> Search `Addressables` -> `Install`.
* Initialize: `Window` -> `Asset Management` -> `Addressables` -> `Groups`. Click **Create Addressables Settings** if prompt appears.
* Add Assets: Drag your `.vsnl` files, Audio, and Images into the Group window.
* Simplify Names: The "Address" is the key. Rename `Assets/Miralis/Resources/Audio/Music.mp3` to just `Music`.
* Build Content: **Crucial Step!** In the Addressables window, click `Build` -> `New Build` -> `Default Build Script`.
  * _If you don't build, the engine won't find the files._

***

## 8. Missing Assets (FAQ)

<details>

<summary><strong>Q: "I don't have music or sound effects yet. Will the game crash?"</strong></summary>

**A: No.**

The engine is designed to handle missing files gracefully.

* If you write `@bgm "EpicTheme"` but the file is missing, the Console will log a **Warning**: `[AudioManager] Clip EpicTheme not found`.
* The game will **continue running** normally (just without sound).

Best Practice for Prototyping:

* Skip commands: Just don't write the `@bgm` command until you have the file.
* Use Placeholders:
  * Create a dummy file (e.g., recorded silence or a free sound).
  * Name it `placeholder.mp3`.
  * Use `@bgm "placeholder"` in your scripts.
  * Addressables: Just label the placeholder asset as `BattleTheme` temporarily. When you get the real file, replace it and keep the Addressable Name the same. You won't even need to touch the code!

</details>

***
