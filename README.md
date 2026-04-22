# Tower Simulator 3 Voice Trainer

A lightweight, native Windows application designed to train your local Windows Speech Recognition (SAPI 5) profile with custom aviation and ATC vocabulary for **Tower Simulator 3**. 

Since modern versions of Windows (like Windows 11) hide the legacy speech training menus, this tool uses direct COM hooks to launch the hidden Microsoft Voice Training Wizard, feeding it your specific simulator phrases.

## Features
* **Native Integration:** Hooks directly into `SAPI.SpSharedRecognizer`—no third-party voice engines required.
* **Custom Vocabulary:** Train the engine on specific airline callsigns, taxiways, and ATC phraseology that default Windows struggles with.

---

## Prerequisites

To build and run this application from the source code, you will need the Microsoft .NET SDK installed on your machine.
* Download the **[.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)** (or .NET 6.0/7.0).

---

## How to Build and Run

1. **Download or Clone** this repository to your local machine.
2. Open your **Command Prompt** (`cmd`) or PowerShell.
3. Navigate to the folder containing `TowerSimulator3VoiceTrainer.csproj`:
   `cd path\to\TowerSimulator3VoiceTrainer`
4. **To run the app directly:**
   `dotnet run`
5. **To compile a standalone executable** (so you don't need to use the command line next time):
   `dotnet publish -c Release -r win-x64 --self-contained false`
   *You can find your compiled `.exe` inside the `bin\Release\net8.0-windows\win-x64\publish` folder.*

---

## How to Use

### 1. Prepare your Dictionary Files
The Windows Speech API has a legacy memory limitation: **it will crash if you feed it more than 40 custom phrases at once.** You must create standard `.txt` files with **one phrase per line**, keeping each file under 40 lines. (Example files like `01_Alphabet.txt`, `02_Ground.txt`, etc., are highly recommended).

*Example format:*
Pushback Approved Facing
Taxi to terminal
Hold short of taxiway
Contact ground

There are sample Dictionary files for Tower Simulator 3 included in the repo.

### 2. Train the Engine
1. Launch the **TowerVoiceTrainer** application.
2. Ensure your **Windows Speech Recognition** is turned on and your microphone is active.
3. Click **Browse...** and select one of the `.txt` files.
4. Click **Launch SAPI Voice Training**.
5. The native Microsoft Speech Wizard will appear. Read the phrases out loud as prompted to bake them into your Windows acoustic profile.
6. Repeat the process for the remaining `.txt` files.

---

## Troubleshooting

**Error: "Speech training is not configured properly..."**
* **Cause:** You tried to load a text file with too many lines, or your text file contains special characters like `[ ]`, `/`, or `-`.
* **Fix:** Ensure your text file has fewer than 40 lines and contains only standard phonetic words (no punctuation).

---

## License
This tool is open-source and free to use for the flight simulation community.