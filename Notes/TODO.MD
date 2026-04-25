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
- [ ] Have everything use new math/time classes in core
- [ ] Hook time/framerate into client as well as server
- [ ] Setup asset folder structure
- [ ] Setup basic file I/O and asset loading in core
- [ ] Have shaders load from files instead of hardcoded strings
- [ ] Render a 3D cube and replace the triangle
- [ ] Send player position to server each tick
- [ ] Server broadcast player positions to all connected clients

# Later
- [ ] Add version checking to server for server and client to ensure correct version is in use
- [ ] Setup multithreading
- [ ] Have client launch a background instance of the server so singleplayer/multiplayer behave the same
- [ ] Setup Skybox/Basic env (expand later)
- [ ] FPS player controller with gravity and collision
- [ ] Basic world/chunk system
- [ ] ImGui debug overlay (FPS, position, server stats)

# Future
- [ ] Abstract renderer for multi API backends i.e OpenGL, Vulkan, etc
- [ ] Add optional steam networking
- [ ] ECS (Entity Component System) for world objects
- [ ] Mod API surface and Lua scripting via NLua
- [ ] Map editor
- [ ] Mod manager
