var kernel = new StandardKernel();
kernel.Load<Protogame2DIoCModule>();
kernel.Load<ProtogameAssetIoCModule>();
kernel.Load<ProtogameEventsIoCModule>(); // Load the events module
kernel.Load<MyProjectModule>(); // This was added in the previous tutorial
AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, args);