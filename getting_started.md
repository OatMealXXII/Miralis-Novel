# GETTING\_STARTED

Welcome to the **VSNL Engine**! This guide will take you from an empty Unity project to running your very first interactive Visual Novel scene in under 15 minutes.

***

## Chapter 1: Installation & Setup

Before writing any dialogue, we need to set up the engine in Unity.

{% stepper %}
{% step %}
### Fresh Start

1. Open **Unity Hub**.
2. Create a **New Project** using the **2D Core** Template (Unity 2022.3 LTS or newer recommended).
3. Name it `MyFirstVN` and click **Create**.
{% endstep %}

{% step %}
### Installing VSNL

Once Unity opens:

1. Open the **Package Manager** (`Window` -> `Package Manager`).
2. Click the **+** (Plus) icon in the top-left corner.
3. Select **Add package from git URL...**.
4. Paste the repository URL (e.g., [https://github.com/OatMealXXII/Miralis-Novel](https://github.com/OatMealXXII/Miralis-Novel.git)) and click **Add**.

{% hint style="info" %}
If you downloaded the `.unitypackage` instead, simply drag and drop it into your `Assets` folder to import.
{% endhint %}
{% endstep %}

{% step %}
### Folder Structure

VSNL relies on Unity's **Resources** system to load assets easily. Let's create the folders we need.

1. In the **Project Window**, right-click the `Assets` folder -> `Create` -> `Folder`. Name it `Resources`.
2. Inside `Resources`, create the following sub-folders:
   * `Scripts` (Your .vsnl story files go here)
   * `Backgrounds` (Scene images)
   * `Characters` (Sprites and Live2D prefabs)
   * `Music` (BGM files)
{% endstep %}
{% endstepper %}

***

## Chapter 2: Your First Script

Now the fun part: writing the story!

{% stepper %}
{% step %}
### The Script File

1. Right-click inside `Assets/Resources/Scripts`.
2. Select `Create` -> `VSNL Script` (or simple Text File named `Chapter1.vsnl` if using standard assets).
3. Name the file `Chapter1`.
{% endstep %}

{% step %}
### Writing Code

Open `Chapter1.vsnl` in your favorite text editor (VS Code is recommended) and paste this:

{% code title="Chapter1.vsnl" %}
```vsnl
# Chapter 1: The Morning Coffee

# Set the mood
@bg "CoffeeShop_Interior"
@bgm "Morning_Jazz"

# Introduce a character named 'Barista'
char "Barista"

"Barista"
"Good morning! The usual?"

# The player character (Narrator) thinks this
"I nod silently, still waking up."
```
{% endcode %}

{% hint style="info" %}
You need to put an image named `CoffeeShop_Interior` into `Resources/Backgrounds` for the background to show up. If it's missing, you'll get a warning but the game won't crash!
{% endhint %}
{% endstep %}

{% step %}
### Running It

1. Open the **VSNL Engine** scene or drag the `Engine` prefab into your scene.
2. Press **Play** in Unity.
3. The engine will automatically look for a default script. If you see the debug console, type `@run Chapter1` (or configure the Engine snippet to auto-load it).
{% endstep %}
{% endstepper %}

***

## Chapter 3: The Choice System

Visual Novels are all about choices. Let's make the player choose their drink.

Append this to your `Chapter1.vsnl` file:

{% code title="Choice example" %}
```vsnl
"Barista"
"So, what will it be today?"

@choice "Select your drink:"
    "Black Coffee (Bitter)" -> Label_Black
    "Latte (Sweet)" -> Label_Latte

# The game effectively pauses here until a choice is clicked.

[Label_Black]
"I'll take it black today."
"Barista"
"Bold choice."
@goto Label_End

[Label_Latte]
"A caramel latte, please."
"Barista"
"Coming right up! Extra foam."
@goto Label_End

[Label_End]
"She turns around to start the machine."
@stop
```
{% endcode %}

### Breakdown

* `@choice`: Starts the selection menu.
* `-> LabelName`: Tells the engine where to jump if clicked.
* `[LabelName]`: A destination marker.
* `@goto`: Jumps to a specific label to merge branches back together (so you don't repeat the ending).

***

## Chapter 4: Debugging

Something went wrong? Use the **Console**!

### 4.1 Common Errors

<details>

<summary>Click to expand common errors</summary>

* **"Label not found":** You tried to `@goto` a label that doesn't exist. Check for typos!
* **"Script file not found":** Did you put the file in `Resources/Scripts`? Is the text asset recognized by Unity?

</details>

### 4.2 In-Game Console

Press `~` (Tilde) while playing to open the **VSNL Debug Console**.

* Type `log` to see the dialogue history.
* Type `var $score` to see current variable values.
* Type `goto Label_End` to fast-forward testing.

***

**Congratulations!** You've built your first VSNL scene.\
For advanced features like **Live2D animation** or **Mini-Games**, check out the `ADVANCED_GUIDE.md`.
