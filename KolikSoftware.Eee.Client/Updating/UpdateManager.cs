using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using KolikSoftware.Eee.Service;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace KolikSoftware.Eee.Client.Updating
{
    public partial class UpdateManager : Component
    {
        object lockObj = new object();

        EeeDataSet.UpdateDataTable currentUpdates;
        EeeDataSet.UpdateDataTable pendingUpdates;
        int updatesToDownloadCount;

        static readonly string PathToUpdates;

        public enum UpdatesState
        {
            None,
            Downloadable,
            Installable
        }

        static UpdateManager()
        {
            PathToUpdates = Path.Combine(Application.StartupPath, "Updates");
        }

        public UpdateManager()
        {
            InitializeComponent();
        }

        public UpdateManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Perform()
        {
            if (!this.IsBusy)
            {
                if (this.State == UpdatesState.Downloadable)
                    ShowUpdates();
                else if (this.State == UpdatesState.Installable)
                    Install();
            }
        }

        void ShowUpdates()
        {
            MergePending();

            using (UpdateForm updateForm = new UpdateForm(this.currentUpdates))
            {
                updateForm.ShowDialog();

                if (updateForm.Install)
                {
                    this.busy = true;
                    this.updatesToDownloadCount = this.currentUpdates.Count;
                    PrepareDir();
                    DownloadNext();
                }
            }
        }

        void Install()
        {
            this.busy = true;
            this.installWorker.RunWorkerAsync();
        }

        public UpdatesState State
        {
            get
            {
                if (Directory.Exists(PathToUpdates))
                    return UpdatesState.Installable;
                else if ((this.currentUpdates != null && this.currentUpdates.Count > 0) || (this.pendingUpdates != null && this.pendingUpdates.Count > 0))
                    return UpdatesState.Downloadable;
                else
                    return UpdatesState.None;
            }
        }

        bool busy = false;

        public bool IsBusy
        {
            get
            {
                return this.busy;
            }
        }

        /// <summary>
        /// Merges pending updates into current, if any.
        /// </summary>
        void MergePending()
        {
            lock (this.lockObj)
            {
                if (this.currentUpdates != null)
                {
                    if (this.pendingUpdates != null)
                    {
                        this.currentUpdates.Merge(this.pendingUpdates);
                        this.pendingUpdates.Dispose();
                    }
                    this.pendingUpdates = null;
                }
                else
                {
                    this.currentUpdates = this.pendingUpdates;
                    this.pendingUpdates = null;
                }

            }
        }

        BackgroundServiceController serviceController;

        public BackgroundServiceController ServiceController
        {
            get
            {
                return this.serviceController;
            }
            set
            {
                if (this.serviceController != null)
                {
                    this.serviceController.UpdatesAvailable -= serviceController_UpdatesAvailable;
                    this.serviceController.DownloadFinished -= serviceController_DownloadFinished;
                    this.serviceController.DownloadFailed -= serviceController_DownloadFailed;
                }

                this.serviceController = value;

                this.serviceController.UpdatesAvailable += serviceController_UpdatesAvailable;
                this.serviceController.DownloadFinished += serviceController_DownloadFinished;
                this.serviceController.DownloadFailed += serviceController_DownloadFailed;
            }
        }

        void serviceController_DownloadFailed(object sender, BackgroundServiceController.DownloadFailedEventArgs e)
        {
            if (this.currentUpdates != null && this.currentUpdates.Count > 0 && e.Link == this.currentUpdates[0].Link)
            {
                this.busy = false;
                OnDownloadFailed(new DownloadFailedEventArgs(e.Link, e.Error));
                if (Directory.Exists(PathToUpdates))
                    Directory.Delete(PathToUpdates, true);
            }
        }

        void serviceController_DownloadFinished(object sender, BackgroundServiceController.DownloadFinishedEventArgs e)
        {
            if (this.currentUpdates != null && this.currentUpdates.Count > 0 && e.Link == this.currentUpdates[0].Link)
            {
                this.currentUpdates.RemoveUpdateRow(this.currentUpdates[0]);
                DownloadNext();
            }
        }

        void serviceController_UpdatesAvailable(object sender, BackgroundServiceController.UpdatesAvailableEventArgs e)
        {
            lock (this.lockObj)
            {
                if (this.pendingUpdates != null)
                {
                    this.pendingUpdates.Merge(e.Updates);
                }
                else
                {
                    this.pendingUpdates = (EeeDataSet.UpdateDataTable)e.Updates.Copy();
                }
            }
        }

        void DownloadNext()
        {
            if (this.currentUpdates.Count == 0)
            {
                FinishDownloads();
            }
            else
            {
                string path = GetDir(this.currentUpdates[0].UpdateID);

                OnDownloadStarted(new DownloadStartedEventArgs(this.updatesToDownloadCount - this.currentUpdates.Count + 1, this.updatesToDownloadCount));
                
                this.serviceController.DownloadFile(this.currentUpdates[0].Link, path);
            }
        }

        void FinishDownloads()
        {
            this.busy = false;
            OnDownloadAllFinished(DownloadAllFinishedEventArgs.Empty);
        }

        void PrepareDir()
        {
            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Updates");

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        string GetDir(int updateId)
        {
            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Updates");

            path = Path.Combine(path, updateId.ToString());

            Directory.CreateDirectory(path);

            return path;
        }

        #region Installing
        void installWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string rootDir = Application.StartupPath;
            string updatesDir = Path.Combine(rootDir, "Updates");

            if (Directory.Exists(updatesDir) == false)
                throw new ApplicationException("Sorry, no updates found.");

            try
            {
                string[] updateDirs = Directory.GetDirectories(updatesDir);

                SortedList<int, string> updateDirsSorted = new SortedList<int, string>();

                foreach (string updateDir in updateDirs)
                {
                    string dirName = Path.GetFileName(updateDir);
                    int updateId;
                    if (int.TryParse(dirName, out updateId))
                        updateDirsSorted.Add(updateId, updateDir);
                }

                int updatesTotal = updateDirsSorted.Count;
                int updateNo = 1;

                foreach (KeyValuePair<int, string> updateDirSorted in updateDirsSorted)
                {
                    this.installWorker.ReportProgress(updateNo, updatesTotal);

                    string[] zipFiles = Directory.GetFiles(updateDirSorted.Value, "*.zip");

                    foreach (string zipFile in zipFiles)
                    {
                        Unzip(zipFile);
                    }

                    Install(rootDir, updateDirSorted.Key, updateDirSorted.Value);

                    updateNo++;
                }
            }
            finally
            {
                if (Directory.Exists(updatesDir))
                    Directory.Delete(updatesDir, true);
            }
        }

        void installWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnInstallStarted(new InstallStartedEventArgs(e.ProgressPercentage, (int)e.UserState));
        }

        void installWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.busy = false;

            if (e.Error == null)
                OnInstallAllFinished(InstallAllFinishedEventArgs.Empty);
            else
                OnInstallFailed(new InstallFailedEventArgs(e.Error));
        }

        void Install(string rootDir, int updateId, string updateDir)
        {
            string[] setupFiles = Directory.GetFiles(updateDir, "*.msi");

            if (setupFiles != null && setupFiles.Length > 0)
            {
                foreach (string setupFile in setupFiles)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("msiexec.exe", "/i \"" + setupFile + "\" /quiet /passive /norestart");
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardError = true;
                    using (Process process = Process.Start(startInfo))
                    {
                        string errors = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        if (errors != null && errors.Length != 0)
                            throw new InstallationException("Installation failed: " + errors);
                    }

                    File.Delete(setupFile);
                }
            }

            try
            {
                PrepareFiles(updateDir, rootDir);
                MoveFiles(updateDir, rootDir);

                Commit(updateDir, updateId);
            }
            catch (Exception)
            {
                Rollback(rootDir);
                throw;
            }
        }

        void Rollback(string fromDir)
        {
            string[] subDirs = Directory.GetDirectories(fromDir);

            foreach (string subDir in subDirs)
            {
                string subDirName = Path.GetFileName(subDir);

                if (subDirName != "." && subDirName != "..")
                    Rollback(Path.Combine(fromDir, subDirName));
            }

            string[] files = Directory.GetFiles(fromDir, "*.upd");

            foreach (string file in files)
            {
                /// Remove .upd extension
                string destFile = file.Substring(0, file.Length - 4);

                if (File.Exists(destFile))
                    File.Delete(destFile);

                File.Move(file, destFile);
            }
        }

        void PrepareFiles(string fromDir, string toDir)
        {
            string[] subDirs = Directory.GetDirectories(fromDir);

            foreach (string subDir in subDirs)
            {
                string subDirName = Path.GetFileName(subDir);

                if (subDirName != "." && subDirName != "..")
                    PrepareFiles(Path.Combine(fromDir, subDirName), Path.Combine(toDir, subDirName));
            }

            if (Directory.Exists(toDir) == false)
                return;

            string[] files = Directory.GetFiles(fromDir, "*.*");

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                string destFile = Path.Combine(toDir, fileName);

                if (File.Exists(destFile))
                {
                    string updFile = destFile + ".upd";

                    if (File.Exists(updFile))
                        File.Delete(updFile);
                    
                    File.Move(destFile, updFile);
                }
            }
        }

        void MoveFiles(string fromDir, string toDir)
        {
            string[] subDirs = Directory.GetDirectories(fromDir);

            foreach (string subDir in subDirs)
            {
                string subDirName = Path.GetFileName(subDir);

                if (subDirName != "." && subDirName != "..")
                    MoveFiles(Path.Combine(fromDir, subDirName), Path.Combine(toDir, subDirName));
            }

            if (Directory.Exists(toDir) == false)
                Directory.CreateDirectory(toDir);

            string[] files = Directory.GetFiles(fromDir, "*.*");

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                string destFile = Path.Combine(toDir, fileName);

                File.Move(file, destFile);
            }
        }

        void Commit(string updateDir, int updateId)
        {
            Properties.Settings.Default.LatestUpdateNo = updateId;
        }

        void Unzip(string filePath)
        {
            FastZip zip = new FastZip();
            zip.ExtractZip(filePath, Path.GetDirectoryName(filePath), ".*");
            File.Delete(filePath);
        }
        #endregion

        #region Exception
        public class InstallationException : Exception
        {
            public InstallationException() { }
            public InstallationException(string message) : base(message) { }
            public InstallationException(string message, Exception inner) : base(message, inner) { }
        }
        #endregion

        #region Events
        public class DownloadStartedEventArgs : EventArgs
        {
            int updateNo;

            public int UpdateNo
            {
                get
                {
                    return this.updateNo;
                }
            }

            int updatesTotal;

            public int UpdatesTotal
            {
                get
                {
                    return this.updatesTotal;
                }
            }

            public DownloadStartedEventArgs(int updateNo, int updatesTotal)
            {
                this.updateNo = updateNo;
                this.updatesTotal = updatesTotal;
            }
        }

        public event EventHandler<DownloadStartedEventArgs> DownloadStarted;

        protected virtual void OnDownloadStarted(DownloadStartedEventArgs e)
        {
            EventHandler<DownloadStartedEventArgs> handler = DownloadStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class DownloadAllFinishedEventArgs : EventArgs
        {
            public static readonly new DownloadAllFinishedEventArgs Empty = new DownloadAllFinishedEventArgs();
        }

        public event EventHandler<DownloadAllFinishedEventArgs> DownloadAllFinished;

        protected virtual void OnDownloadAllFinished(DownloadAllFinishedEventArgs e)
        {
            EventHandler<DownloadAllFinishedEventArgs> handler = DownloadAllFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class DownloadFailedEventArgs : EventArgs
        {
            string link;

            public string Link
            {
                get
                {
                    return this.link;
                }
            }

            Exception error;

            public Exception Error
            {
                get
                {
                    return this.error;
                }
            }

            public DownloadFailedEventArgs(string link, Exception error)
            {
                this.link = link;
                this.error = error;
            }
        }

        public event EventHandler<DownloadFailedEventArgs> DownloadFailed;

        protected virtual void OnDownloadFailed(DownloadFailedEventArgs e)
        {
            EventHandler<DownloadFailedEventArgs> handler = DownloadFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class InstallStartedEventArgs : EventArgs
        {
            int updateNo;

            public int UpdateNo
            {
                get
                {
                    return this.updateNo;
                }
            }

            int updatesTotal;

            public int UpdatesTotal
            {
                get
                {
                    return this.updatesTotal;
                }
            }

            public InstallStartedEventArgs(int updateNo, int updatesTotal)
            {
                this.updateNo = updateNo;
                this.updatesTotal = updatesTotal;
            }
        }

        public event EventHandler<InstallStartedEventArgs> InstallStarted;

        protected virtual void OnInstallStarted(InstallStartedEventArgs e)
        {
            EventHandler<InstallStartedEventArgs> handler = InstallStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class InstallAllFinishedEventArgs : EventArgs
        {
            public static readonly new InstallAllFinishedEventArgs Empty = new InstallAllFinishedEventArgs();
        }

        public event EventHandler<InstallAllFinishedEventArgs> InstallAllFinished;

        protected virtual void OnInstallAllFinished(InstallAllFinishedEventArgs e)
        {
            EventHandler<InstallAllFinishedEventArgs> handler = InstallAllFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class InstallFailedEventArgs : EventArgs
        {
            Exception error;

            public Exception Error
            {
                get
                {
                    return this.error;
                }
            }

            public InstallFailedEventArgs(Exception error)
            {
                this.error = error;
            }
        }

        public event EventHandler<InstallFailedEventArgs> InstallFailed;

        protected virtual void OnInstallFailed(InstallFailedEventArgs e)
        {
            EventHandler<InstallFailedEventArgs> handler = InstallFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
