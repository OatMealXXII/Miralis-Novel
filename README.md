# README

![AI Assisted](https://img.shields.io/badge/AI-Assisted-blueviolet?style=for-the-badge\&logo=googlegemini) ![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg?style=flat-square) ![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg?style=flat-square) ![License](https://img.shields.io/badge/license-MIT-green.svg?style=flat-square)

VSNL Engine is a production-ready, script-driven Visual Novel framework for Unity. Designed for performance and flexibility, it offers a robust alternative to industry standards like Naninovel while remaining 100% free and open-source.

## ✨ Key Features

* **Human-Readable Scripting**: Write your story in `.vsnl` files—a syntax simple enough for writers but powerful enough for complex logic.
* **Async/Await Architecture**: Built on modern C# `UniTask` to ensure zero-lag resource loading and smooth asynchronous command execution.
* **Modular Architecture**: Uses a strict **Service Locator** pattern, decoupling subsystems like Audio, UI, and State Management for easy extension.
* **Advanced Rendering**: Native support for **Live2D** and **Spine** character controllers, alongside standard 2D Sprites.
* **Essential VN Systems**: Includes Save/Load persistence, Infinite Backlog, Auto-Read, and Skip modes out of the box.

## 🚀 Quick Start (5 Minutes)

{% hint style="info" %}
Prerequisites:

* Unity 2022.3 LTS or newer.
* (Optional) Live2D Cubism SDK for Live2D support.
{% endhint %}

### Installation

{% stepper %}
{% step %}
### Open Package Manager

Open the **Package Manager** in Unity.
{% endstep %}

{% step %}
### Add via Git URL

Add package via git URL:

```
https://github.com/OatMealXXII/Miralis-Novel.git
```
{% endstep %}

{% step %}
### Or download .unitypackage

Or download the latest `.unitypackage` from the Releases page:

https://github.com/OatMealXXII/Miralis-Novel/releases
{% endstep %}
{% endstepper %}

### Hello World

Create a file named `Start.vsnl` in your `Resources/Scripts` folder:

{% code title="Start.vsnl" %}
```vsnl
@bg "School_Gate"
@bgm "Morning_Breeze"

# Define a character for specific styling
char "Elysia"

# Simple Dialogue
"Elysia"
"Hi there! Welcome to the VSNL Engine."

# Display Character Sprite
@char "Elysia:Happy:Center"
"It's super easy to write dialogue!"

# Choices
choice "What do you think?"
    "It's amazing!" -> Label_Amazing
    "I need more features." -> Label_Feedback

[Label_Amazing]
"Glad you like it! Let's build something epic."
@stop

[Label_Feedback]
"We're open source! Feel free to contribute."
@stop
```
{% endcode %}

## 🗺️ Roadmap

* [x] **Core Script Interpreter** (Parsing, Logic, Flow Control)
* [x] **Resource System** (Async Loading, Addressables Support)
* [x] **Visual Node Editor** (GraphView Integration)
* [x] **Save & Load System** (JSON Serialization)
* [x] **Live2D Integration**
* [x] **Localization System**
* [ ] 3D Character Rendering
* [ ] Console Porting Tools

## 🤝 Community & License

Have questions or want to contribute? Join our Discord Server:

**Sorry there're no Discord server for now if you found bugs just sent issues to me thank you.**

VSNL Engine is licensed under the **MIT License**. You are free to use it for commercial and non-commercial projects.
