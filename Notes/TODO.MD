# CURRENT
- [x] Setup client to open window with SILK.NET
- [x] Get server working with an interactible console maybe?
- [x] Have client and server connect to each other
- [x] Setup some math and time classes in core
- [x] Setup event system (EventBus, KeyEvents, MouseEvents, WindowEvents)
- [x] Setup input system in core
- [x] Setup camera in core
- [x] Setup basic FPS mouse look and WASD movement in client
- [x] Setup basic Player class in core
- [x] Server tracks connected players as Player objects
- [x] Fix private properties in KeyEvents and WindowEvents
- [x] Have everything use new math/time classes in core
- [x] Hook time/framerate into client as well as server
- [x] Setup asset folder structure
- [x] Setup basic file I/O and asset loading in core
- [x] Have shaders load from files instead of hardcoded strings
- [x] Render a 3D cube and replace the triangle
- [x] Send player position to server each tick
- [x] Server broadcast player positions to all connected clients
- [ ] Refactor renderer to be modular instead of one gaint render method

# Later
- [ ] Add version checking to server for server and client to ensure correct version is in use
- [ ] Setup multithreading
- [ ] Have client launch a background instance of the server so singleplayer/multiplayer behave the same
- [ ] Setup Skybox/Basic env (expand later)
- [ ] FPS player controller with gravity and collision
- [ ] Basic world/chunk system
- [x] ImGui debug overlay (FPS, position, server stats)

# Future
- [ ] Abstract renderer for multi API backends i.e OpenGL, Vulkan, etc
- [ ] Add optional steam networking
- [ ] ECS (Entity Component System) for world objects/Maybe GOCS (Game Object Component System)
- [ ] Mod API surface and Lua scripting via NLua
- [ ] Map editor
- [ ] Mod manager
