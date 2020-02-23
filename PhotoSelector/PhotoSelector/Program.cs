using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PhotoSelector
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Properties.Settings.Default.Upgrade();

            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    if (Properties.Settings.Default.HoldingFolder == "" || !(Directory.Exists(Properties.Settings.Default.HoldingFolder)))
                    {
                        SetHoldingFolder();
                    }

                    if (File.Exists(Properties.Settings.Default.HoldingFolder + "\\" + Path.GetFileName(args[0])))
                    {
                        MessageBox.Show("A photo with that filename is already in the selected holding folder.", "Duplicate Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    bool done = AddPhotoToHoldingFolder(Properties.Settings.Default.HoldingFolder, args[0]);

                    if (!done)
                    {
                        MessageBox.Show("Photo NOT added!", "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The file cannot be found or no longer exists.", "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (args.Length == 0)
            {
                SetHoldingFolder();
            }
            else
            {
                MessageBox.Show("Invalid application arguments.", "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(
        string lpFileName,
        string lpExistingFileName,
        IntPtr lpSecurityAttributes
        );

        static bool AddPhotoToHoldingFolder(string holdingFolderPath, string filePath)
        {
            return CreateHardLink(holdingFolderPath + "\\" + Path.GetFileName(filePath), filePath, IntPtr.Zero);
        }

        static void SetHoldingFolder()
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                Description = "Select a new holding folder...",
                ShowNewFolderButton = true
            };

            DialogResult result = folder.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.HoldingFolder = folder.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
