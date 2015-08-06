using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Android.App;

using PCLStorage;

namespace MineLib.Android.WrapperInstances
{
    public class FileSystemWrapperInstance : Core.Wrappers.IFileSystem
    {
        public IFolder ProtocolsFolder { get; private set; }

        public IFolder ContentFolder { get; private set; }

        public IFolder SettingsFolder { get; private set; }

        public IFolder LogFolder { get; private set; }


        public IFile GetFileFromPath(string path)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
        
        public IFolder GetFolderFromPath(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public FileSystemWrapperInstance()
        {
            var baseDirectory = Application.Context.GetExternalFilesDir(null).ParentFile.AbsolutePath;

            var protocolsPath = Path.Combine(baseDirectory, "Protocols");
            var contentPath = Path.Combine(baseDirectory, "Content");
            var settingsPath = Path.Combine(baseDirectory, "Settings");
            var logsPath = Path.Combine(baseDirectory, "Logs");

            if (!Directory.Exists(protocolsPath))
                Directory.CreateDirectory(protocolsPath);

            if (!Directory.Exists(contentPath))
                Directory.CreateDirectory(contentPath);

            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            ProtocolsFolder = new FileSystemFolder(protocolsPath);
            ContentFolder   = new FileSystemFolder(contentPath);
            SettingsFolder  = new FileSystemFolder(settingsPath);
            LogFolder       = new FileSystemFolder(logsPath);
        }
    }
}
