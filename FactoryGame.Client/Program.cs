using FactoryGame.Client;
using FactoryGame.Core.Log;

Logger.Initialize();
Logger.Info("Client starting...");

var window = new Window("FactoryGame - Client", 1280, 720, true);
window.Run();

Logger.Info("Client shut down.");
Logger.Shutdown();