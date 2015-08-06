using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;

using Microsoft.Xna.Framework;

using MineLib.Android.WrapperInstances;

using MineLib.Core.Wrappers;

using MineLib.PCL.Graphics;

using PCLStorage;


namespace MineLib.Android
{
    [Activity(Label = "MineLib.Andoid", MainLauncher = true, Icon = "@drawable/icon", AlwaysRetainTaskState = true, LaunchMode = LaunchMode.SingleInstance, 
        ScreenOrientation = ScreenOrientation.SensorLandscape, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public sealed class MainActivity : AndroidGameActivity
    {
        Action OnBackButtonPressed;

        public MainActivity()
        {
            AppDomainWrapper.Instance = new AppDomainWrapperInstance();
            FileSystemWrapper.Instance = new FileSystemWrapperInstance();
            NetworkTCPWrapper.Instance = new NetworkTCPWrapperInstance();
            ThreadWrapper.Instance = new ThreadWrapperInstance();

            var assetsContent = ParseAssetsFolder(Application.Context.Assets.List("Content"), "Content");
            MoveContentToExternalStorage(assetsContent);
        }


        private static void MoveContentToExternalStorage(IEnumerable<string> files)
        {
            foreach (var file in files)
                using (var stream = Application.Context.Assets.Open(Path.Combine("Content", file)))
                {
                    var currentFolder = FileSystemWrapper.ContentFolder;

                    var directories = file.Split(Path.DirectorySeparatorChar);
                    if (directories.Length > 1)
                        for (int i = 0; i < directories.Length - 1; i++)
                            currentFolder = currentFolder.CreateFolderAsync(directories[i], CreationCollisionOption.OpenIfExists).Result;
                        
                    var createdFile = currentFolder.CreateFileAsync(Path.GetFileName(file), CreationCollisionOption.ReplaceExisting).Result;
                    using (var createdFileStream = createdFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).Result)
                        stream.CopyTo(createdFileStream);
                }
        }
        
        private static IEnumerable<string> ParseAssetsFolder(IEnumerable<string> entries, string path)
        {
            var returnFiles = new List<string>();

            foreach (var entry in entries)
            {
                if (Path.HasExtension(entry))
                    returnFiles.Add(entry);
                
                if (!Path.HasExtension(entry))
                    foreach (var file in ParseAssetsFolder(Application.Context.Assets.List(Path.Combine(path, entry)), Path.Combine(path, entry)))
                        returnFiles.Add(Path.Combine(entry, file));
            }

            return returnFiles;
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var t = this.ObbDir;

            var game = new Client(PlatformCode, true);
            InputWrapper.Instance = new InputWrapperInstance(this, (View)game.Services.GetService(typeof(View)), ref OnBackButtonPressed);
            SetContentView((View)game.Services.GetService(typeof(View)));
            game.Run();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            if(OnBackButtonPressed != null)
                OnBackButtonPressed();
        }

        private static void PlatformCode(Game game) { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

