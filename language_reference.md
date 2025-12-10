# LANGUAGE\_REFERENCE

{% stepper %}
{% step %}
### Narrative Commands

#### Implicit Dialogue

Display text in the dialogue box. Just write the text!

* Syntax: "Speaker Name" (Optional) followed by "Dialogue Text" on the next line.

Example:

{% code title="example.vsnl" %}
```vsnl
"Elysia"
"Hello there!"
```
{% endcode %}

#### @char

Displays a character sprite or model.

* Description: Shows a character at a specific position with an emotion.
* Syntax: `@char Name:Emotion:Position`

Parameters:

| Parameter  | Description                                                               |
| ---------- | ------------------------------------------------------------------------- |
| `Name`     | Character ID (must match filename or Metadata).                           |
| `Emotion`  | (Optional) Sprite/Animation state (Default: "Default").                   |
| `Position` | (Optional) Screen position "Left", "Center", "Right" (Default: "Center"). |

Example:

{% code title="example.vsnl" %}
```vsnl
@char "Elysia:Happy:Left"
```
{% endcode %}

#### @bg

Sets the background image.

* Description: Crossfades to a new background image.
* Syntax: `@bg "ImageName"`

Parameters:

| Parameter   | Description                                  |
| ----------- | -------------------------------------------- |
| `ImageName` | Name of the file in `Resources/Backgrounds`. |

Example:

{% code title="example.vsnl" %}
```vsnl
@bg "School_Sunset"
```
{% endcode %}

#### @despawn (Hide)

Removes a character or object.

* Description: Despawns a character or visual effect.
* Syntax: `@despawn "TargetName"`

Example:

{% code title="example.vsnl" %}
```vsnl
@despawn "Elysia"
```
{% endcode %}
{% endstep %}

{% step %}
### Audio Commands

#### @bgm

Plays background music.

* Description: Loops a music track with auto-crossfade.
* Syntax: `@bgm "TrackName" [FadeDuration]`

Parameters:

| Parameter      | Description                                |
| -------------- | ------------------------------------------ |
| `TrackName`    | File in `Resources/Audio/BGM`.             |
| `FadeDuration` | (Optional) Seconds to fade (Default: 1.0). |

Example:

{% code title="example.vsnl" %}
```vsnl
@bgm "Battle_Theme" 2.0
```
{% endcode %}

#### @sfx

Plays a sound effect.

* Description: Plays a one-shot sound.
* Syntax: `@sfx "ClipName"`

Example:

{% code title="example.vsnl" %}
```vsnl
@sfx "Explosion"
```
{% endcode %}

#### @stop\_all\_audio

* Note: Not implemented directly. Use specific stop logic if needed or `@bgm "None"`.
{% endstep %}

{% step %}
### Flow Control

#### @goto

Jumps to a label.

* Description: Moves execution to `[LabelName]`.
* Syntax: `@goto LabelName`

Example:

{% code title="example.vsnl" %}
```vsnl
@goto Chapter2
```
{% endcode %}

#### @choice

Starts a choice menu.

* Description: Displays buttons. Engine waits for user input.
* Syntax:

{% code title="example.vsnl" %}
```vsnl
@choice "Prompt Text"
    "Option A" -> LabelA
    "Option B" -> LabelB
```
{% endcode %}

* Example: See Quick Start Guide.

#### @stop

Stops execution.

* Description: Halts the script player. Useful after choices or at the end of a chapter.
* Syntax: `@stop`

#### @wait

Pauses execution.

* Description: Waits for X seconds.
* Syntax: `@wait Duration`

Example:

{% code title="example.vsnl" %}
```vsnl
@wait 1.5
```
{% endcode %}
{% endstep %}

{% step %}
### Logic & Variables

#### @set / @let

Sets a variable provided by `VariableManager`.

* Description: Assigns a value (`string`, `float`, `bool`) to a variable.
* Syntax: `@set $VariableName = Value`

Example:

{% code title="example.vsnl" %}
```vsnl
@set $affinity = 10
```
{% endcode %}

#### @if

Conditional logic.

* Description: Executes the next block only if the expression is true.
* Syntax: `@if Expression` ... `@endif`
* Supported Operators: `==`, `!=`, `>`, `<`, `&&`, `||`

Example:

{% code title="example.vsnl" %}
```vsnl
@if $affinity > 5
    "She smiles at you."
@endif
```
{% endcode %}
{% endstep %}

{% step %}
### Advanced / System

#### @spawn

Spawns a prefab (VFX/Live2D).

* Description: Instantiates a Prefab from `Resources/Prefabs`.
* Syntax: `@spawn "PrefabName" x:0 y:0`

Parameters:

| Parameter    | Description                                                                               |
| ------------ | ----------------------------------------------------------------------------------------- |
| `PrefabName` | Name of the prefab.                                                                       |
| `x / y`      | (Optional) Screen coordinates (normalized 0-1 or pixels depending on implementing logic). |

Example:

{% code title="example.vsnl" %}
```vsnl
@spawn "RainEffect"
```
{% endcode %}

#### @motion (Live2D)

Plays an animation on a character.

* Description: Triggers a Live2D motion or Animator state.
* Syntax: `@motion "CharacterName" "MotionName"`

Example:

{% code title="example.vsnl" %}
```vsnl
@motion "Elysia" "Attack"
```
{% endcode %}

#### @minigame

Loads a minigame scene.

* Description: Loads a scene additively and waits for it to close.
* Syntax: `@minigame "SceneName" win_var:VariableName`

Example:

{% code title="example.vsnl" %}
```vsnl
@minigame "Poker" win_var:$poker_result
```
{% endcode %}

{% hint style="info" %}
This reference covers VSNL v1.0. Future updates (v1.1) will include `@camera`, `@voice`, and `@save` commands.
{% endhint %}
{% endstep %}
{% endstepper %}
