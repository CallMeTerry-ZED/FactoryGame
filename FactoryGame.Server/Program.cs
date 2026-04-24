using FactoryGame.Core;
using FactoryGame.Core.Log;
using FactoryGame.Server;

Logger.Initialize("Logs/Server");
Logger.Info("Server process started.");

var server = new Server();
server.Start();

Logger.Info("Server shut down.");
Logger.Shutdown();