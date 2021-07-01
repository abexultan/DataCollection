# DataCollection
Allows collection of data for the game Cities: Skylines

## Installation:
* Open this project in [Visual Studio](https://visualstudio.microsoft.com/vs/) (while installing make sure .NET components are checked as well as .NET 3.5 framework)
* Follow steps 2 and 3 from method 2 of this [tutorial](https://community.simtropolis.com/forums/topic/73404-modding-tutorial-0-your-first-mod/) (dependencies and build script)
* Build the mod by pressing CTRL-SHIFT-B
* In your game in `Content Manager` -> `Mods` DataCollection modification should appear, make sure it is checked
* In visual settings disable `Depth of field` and `Grain` effects
* Make sure that your game is running in your screen resolution and highest settings possible (for a clearer images)
## Camera positions capturing:
* Load the game with mod installed in your city of choice
* While in freecamera (lower right corner button) fly over your city, stop at the position and orientation you thinks is appropriate and press `Delete` key on your keyboard
    * Current position and orientation will be written in the `.txt` file called `campos` which we will be using later
* Repeat the previous step at different locations (preferably ~40-50 locations are enough)
## Data collection:
* After caputring camera positions, open this project in Visual Studio and in `DataCollectionLogic.cs` change the field `camposfile` with the full path (`C:\Users\*YOURUSERNAME*\AppData\Local\Colossal Order\Cities_Skylines\ModConfig\DataCollection\GameSession-YYYYMMDDHHmmss`) to `campos.txt` file you have previously collected
* Build the mod (CTRL-SHIFT-B)
* Now, when loading, game will create an array of positions you have previously collected and you can automate data collection process
* Install [AutoHotkey](https://www.autohotkey.com/)
* Edit the script `cities_skylines` (Right-click -> Edit script) and change the number of loop iterations with the number of camera positions you collected multiplied by 50
* Launch the script
* Open the game, load the same city you have used for camera positions capturing, switch to Freecamera, launch the gametime (press Spacebar on your keyboard) and press CTRL-P, this should launch the Autohotkey script which will press your keyboard keys automatically, collecting the data
* After finishing, go to `C:\Users\*YOURUSERNAME*\AppData\Local\Colossal Order\Cities_Skylines\ModConfig\DataCollection\GameSession-YYYYMMDDHHmmss` and find `images` and `labels`
* You can validate your data by trying out the .py script from this repository
