using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace TZ.FolderLinkMove
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btn_MoveFolder_Click(object sender, RoutedEventArgs e)
        {
            string sourceDire = txt_SourceDirPath.Text;
            if (string.IsNullOrEmpty(sourceDire))
            {
                MessageBox.Show("请选择源文件夹！");
                return;
            }
            string targetDir = txt_TargetDirPath.Text;
            if (string.IsNullOrEmpty(targetDir))
            {
                MessageBox.Show("请选择目标文件夹！");
                return;
            }

            if (!Directory.Exists(sourceDire))
            {
                MessageBox.Show("源文件夹不存在！");
                return;
            }
            if (!Directory.Exists(targetDir))
            {
                MessageBox.Show("目标文件夹不存在！");
                return;
            }

            var targetDirInfo = new DirectoryInfo(targetDir);
            var targetDirs = targetDirInfo.GetDirectories();
            if (targetDirs.Length > 0)
            {
                MessageBox.Show("目标文件夹有其他文件夹，不能复制！");
                return;
            }
            var targetFiles = targetDirInfo.GetFiles();
            if (targetFiles.Length > 0)
            {
                MessageBox.Show("目标文件夹有其他文件，不能复制！");
                return;
            }

            var msgBoxResult = MessageBox.Show("请确保转移的文件夹内没有程序及服务运行，文件没有被专用", "是否转移文件夹？", MessageBoxButton.YesNo);
            if (msgBoxResult == MessageBoxResult.No)
            {
                return;
            }
            var type = cbx_Type.SelectedIndex == 0 ? "d" : "j";
            this.loadingBar.IsOpen = true;
            var copyFileResult = await Task.Run(() =>
            {
                try
                {
                    CopyDireToDire(sourceDire, targetDir);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                    //throw e;
                }
            });
            if (!copyFileResult)
            {
                if (Directory.Exists(targetDir))
                {
                    var delDestDirResult = await Task.Run(() => {
                        try
                        {
                            DelectDir(targetDir);
                            return true;
                        }
                        catch (Exception e)
                        {
                            return false;
                            //throw e;
                        }
                    });
                }
                this.loadingBar.IsOpen = false;
                MessageBox.Show("复制文件报错");
                return;
            }

            var sourceDireInfo = new DirectoryInfo(sourceDire);
            var delSourceFileResult = await Task.Run(() =>
            {
                try
                {
                    sourceDireInfo.Delete(true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
            if (!delSourceFileResult)
            {
                this.loadingBar.IsOpen = false;
                MessageBox.Show("删除源文件夹报错");
                return;
            }
            var commadStr = $"mklink /{type} \"{sourceDire}\" \"{targetDir}\"";
            try
            {
                var respStr= await Task.Run(() =>
                {
                    string response = CMD.Execute(commadStr,
                        (sender, e) =>
                        {
                            MessageBox.Show(e.Data);
                        },
                        (sender, e) =>
                        {
                            MessageBox.Show(e.Data);
                        }
                        );
                    return response;
                });
            }
            catch (Exception ex)
            {
                this.loadingBar.IsOpen = false;
                MessageBox.Show("连接文件夹报错");
                return;
            }
            this.loadingBar.IsOpen = false;
            MessageBox.Show("转移成功");
        }


        private void btn_SourceDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               txt_SourceDirPath.Text= openFileDialog.SelectedPath;
            }
        }

        private void btn_TargetDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               txt_TargetDirPath.Text= openFileDialog.SelectedPath;
            }
        }

        /// <summary>
       /// 将一个文件夹下的所有文件复制到另一个文件夹
       /// </summary>
       /// <param name="srcDir">源文件夹路径</param>
       /// <param name="destDire">目标文件夹路径</param>
        private void CopyDireToDire(string srcDir, string destDire)
        {
            //复制文件
            var sourceDireInfo = new DirectoryInfo(srcDir);
            //var aaa=sourceDireInfo.getfil
            FileInfo[] sourceFiles = sourceDireInfo.GetFiles();
            foreach (FileInfo fileInfo in sourceFiles)
            {
                string sourceFilePath = fileInfo.FullName;
                string destFilePath = sourceFilePath.Replace(srcDir, destDire);
                File.Copy(sourceFilePath, destFilePath, true);
            }
            //复制文件夹
            DirectoryInfo[] direInfos = sourceDireInfo.GetDirectories();
            foreach (DirectoryInfo dInfo in direInfos)
            {
                string sourceDirePath = dInfo.FullName;
                string destDirePath = sourceDirePath.Replace(srcDir, destDire);
                Directory.CreateDirectory(destDirePath);
                CopyDireToDire(sourceDirePath, destDirePath);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        private void DelectDir(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo fileSystemInfo in fileinfo)
            {
                if (fileSystemInfo is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(fileSystemInfo.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(fileSystemInfo.FullName);
                }
            }
        }

    }
}
