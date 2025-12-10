# ARCHITECTURE

{% stepper %}
{% step %}
### High-Level Architecture

VSNL uses a modular, Service-Oriented Architecture (SOA) primarily built around the **Service Locator** pattern.

#### Core Loop

1. **Engine (Bootstrapper):** The `Engine` MonoBehaviour persists across scenes (`DontDestroyOnLoad`). It initializes all services in a specific order.
2. **ScriptPlayer:** Reads `.vsnl` text files line-by-line using the `ScriptParser`.
3. **CommandFactory:** Parses command lines (e.g., `@jump`) and instantiates the corresponding `IVSNLCommand`.
4. **Services:** Commands delegate logic to decoupled services (e.g., `AudioManager`, `CharacterManager`).

#### Service Locator Pattern

Services are accessed via `Engine.Instance`.

```csharp
// Example access
var audio = Engine.Instance.GetService<AudioManager>();
audio.PlayBGM("Theme");
```

Extensions should inject dependencies by retrieving them from the Engine rather than creating hard singletons.
{% endstep %}

{% step %}
### How to Add a New Command

Creating custom commands is the primary way to extend VSNL.

{% stepper %}
{% step %}
#### Implement the Interface

Create a new script in `Assets/Scripts/Commands/Concrete`.

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;
using VSNLEngine.Core; // For Services

namespace VSNL.Commands.Concrete
{
    public class Command_Shake : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // 1. Parse Args (e.g., "5")
            if (!float.TryParse(args, out float duration)) duration = 1.0f;

            // 2. Logic (Direct logic or Service call)
            var camera = Camera.main;
            var originalPos = camera.transform.position;
            
            float elapsed = 0;
            while(elapsed < duration) 
            {
                float x = Random.Range(-0.1f, 0.1f);
                float y = Random.Range(-0.1f, 0.1f);
                camera.transform.position = originalPos + new Vector3(x, y, 0);
                
                elapsed += Time.deltaTime;
                await UniTask.Yield(); // Wait frame
            }
            camera.transform.position = originalPos;
        }
    }
}
```
{% endstep %}

{% step %}
#### Register the Command

Open `Assets/Scripts/Commands/CommandFactory.cs` and add it to the registry.

```csharp
private static readonly Dictionary<string, Type> _commandRegistry = new Dictionary<string, Type>()
{
    { "wait", typeof(Command_Wait) },
    // ...
    { "shake", typeof(Command_Shake) } // Register here!
};
```
{% endstep %}

{% step %}
#### Use it in Script

Now you can use it in any `.vsnl` file:

```vsnl
"The ground trembles..."
@shake 2.0
```
{% endstep %}
{% endstepper %}
{% endstep %}

{% step %}
### Save System Internals

The save system uses `Newtonsoft.Json` (or `JsonUtility` depending on version) to serialize the state.

#### GameState Object

The state is captured in `VSNL.State.GameState`:

* `LineIndex` (int): Where the reader is.
* `ScriptFileName` (string): Current story chapter.
* `Variables` (Dictionary): User flags (`$score`, `$met_alice`).
* `ActiveCharacters` (List): Who is on screen and their positions.

#### Adding Data to Saves

If you add a new feature (e.g., a Quest System), you must:

1. Add a field to `GameState` class.
2. Ensure your Manager reads this field in `RestoreState` (in `Engine.cs` or the Manager's own restore method).

#### Checkpoints

The engine supports a "Checkpoint" concept via the `@save` command (planned v1.1), which dumps the current `GameState` to a Slot file (e.g., `save_01.json`).
{% endstep %}
{% endstepper %}
