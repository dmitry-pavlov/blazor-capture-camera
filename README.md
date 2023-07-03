# Capture Camera

Blazor Server app recording video from camera by capturing frames, storing them and converting to video file.

## Dependencies

- [Azure.Storage.Blobs](https://www.nuget.org/packages/Azure.Storage.Blobs/) - Azure Storage Blobs client library for .NET pointed to local storage. See [Use the Azurite emulator for local Azure Storage development](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)
- [AnimatedGif](https://github.com/mrousavy/AnimatedGif) - used to generate kinda video from frames captured from camera. See `VideoRecorder.cs` it can use some web services which can produce video file from images. I just went easiest cross platform way on web for this demo. 

## Getting Started 

- Open solution in Visual Studio from git https://github.com/dmitry-pavlov/blazor-capture-camera and rebuild
- Solution has Blazor Server project 'CaptureCamera' with 'CameraStreamer.razor' component wrapping up capturing from camera.
- Before running the app make sure you started [Azurite Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) and created in local storage DocumentContainer `recordings` (as configured in `appsettings.json`)
- Run project and try demo: 

https://github.com/dmitry-pavlov/blazor-capture-camera/assets/1560778/28fbe490-4288-43ed-a828-71cb4d66d801

