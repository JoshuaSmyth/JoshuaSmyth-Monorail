Monorail
Game Framework from Maglev Studios

  Credits
  - Joshua Smyth

  Code From Various Sources:
  - FNA
  - Monogame

  - SDL and SDL_Image Wrappers by Ethan Lee				(https://github.com/flibitijibibo/SDL2-CS)
  - OpenAL Wrappers by Ethan Lee						(https://github.com/flibitijibibo/OpenAL-CS)
  - OpenGL Wrappers and Math Library from opengl4csharp (https://github.com/giawa/opengl4csharp)


Goals:
 Simple Framework offering minimal abstractions over SDL/OpenGL and OpenAL
 Focus on easy-to-use code first API using features required by the Treasure Hunters Engine

Theretically everything should be crossplatform, but focusing on windows for now.

You would want to use this framework when you want:
	- To use C#
	- To use SDL, OpenGL and OpenAL.
	- Code First development rather than interface driven development.
	- Use a Game Framework rather than Game Engine.
	- Build your own engine on top of lightly abstracted functionality.
	- Crossplatform Development
	- Crossplatform Deployment (Desktop, Mobile)

Game Engine Vs Game Framework When to Use Which?
https://www.youtube.com/watch?v=G4cEXWosFtA

INCLUDE LOTS OF SAMPLES!!!

 Inspirations
  - XNA and it's decendents (FNA/Monogame)
  - Cocos2D
  - Love
  - LibGDX
  - LWJGL
  - Marmalade
  - Phaser.io
  - Threes.js
  - SFML
  - Raylib (C++)
  - Allegro
  - Haxe

THE BASICS!
Standard Features you'd expect
  - Sprite Batch
  - Bitmap Font Rendering
  - Graphics Device
    - Statistics for draw calls / texture swaps / buffer binds etc...
  - Simple Audio Playback
    - (Wav/Ogg with DSP effects such as Reverb, Low Pass Filter etc...)
  - Standard Shaders and custom shader support

SPECIAL FEATURES!
These make the framework worthwhile to use!
  - Coroutines (including coroutine inspection)
  - Extendable Ingame debug console display
  - Simple Service Registry
  - Asset Management Registry (with hotloader, runtime content processor, local directory or network or pakfile loading)
  - Code Profiler (InGameDisplay or Websocket Server Support)
  - Common Math Library (Including random numbers, intersection tests)
  - Simple Debug Primitive Rendering
  - Post Effects Pipeline
  - Tweening
  - RenderQueueCommands / Material System (Maybe this is TreasureHunters Engine or optional includes)
  - Instanced rendering
  - Multithreaded Job System (No dependacy Support)
  - Game/App Config
  - 2D and 3D Cameras
		- 2D
		- 3D Perspective / Orthographic / DebugFly
  - 3D Asset Loader
		- Collada
		- Obj
		- MRM	(Monorail Model - Fast Binary Format)

PLUGINS (To be implemented Later)
 - Skinned Animation
 - Spine Support
 - Particle System
 - Input Recording and Playback

BRING YOUR OWN (This is what we won't be implementing at least not at this stage, or it will be in an engine build ontop of Monorail)
  - UI Framework
  - Scene Management
  - Physics
  - Collision Detection
  - AI
  - Pathfinding
  - Networking
  - XML/JSON/Ini Readers


  Technical Details
  
  Threads
   - 0. AppMain		(Processes and buffers input and OS events and captures any crshes from any of the other threads)
   - 1. DebugThread (This helps keep the app responsive if Update locks up and provide some inspection tooling)

   while(frameRate)
	   - 2. Update
			UpdateCPU()
			UpdateRenderCPU()
	   - 3. SubmitGPU (Processes the renderqueue and submits instructions to the GPU - Note if we have not finished by the time we hit the next UpdateRenderCPU Update thread needs to wait before UpdateRenderCPU can be called)
   SyncThreads

   - 4. Audio [Fire and Forget]
   - 5. BackgroundLoader [WriteBack]


	
