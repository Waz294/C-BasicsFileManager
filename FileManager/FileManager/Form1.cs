using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {
        private string currDir = "";
        private List<string> commandsHistory = new List<string>();
        private string currStr = "";

        public Form1()
        {
            ExceptionHelper.ClearExceptionFolder();
            InitializeComponent();
            currDir = loadLastDir();

            if(currDir == "")
            {
                currDir = Directory.GetCurrentDirectory();
            }

            PopulateTreeView(currDir);
            RichTBVWriteLine($"Current directory: {currDir}");
            tbInput.Select();
        }
        private string loadLastDir()
        {
            var output = "";
            try
            {
                output = File.ReadAllText(Consts.lastDirFilepath);
            }
            catch(Exception e)
            {
                ExceptionHelper.WriteException(e, "loadLastDir");
            }

            return output;
        }
        private void PopulateTreeView(string currDir)
        {
            TreeNode rootNode;
            TreeNode backupNode = new TreeNode();

            if (treeView1.Nodes.Count > 0)
                backupNode = (TreeNode)treeView1.Nodes[0].Clone();

            try
            {
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                DirectoryInfo info = new DirectoryInfo(currDir);
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode, 0);

                treeView1.Nodes.Add(rootNode);

                foreach (FileInfo file in info.GetFiles())
                {
                    treeView1.Nodes[0].Nodes.Add(file.Name);
                }

            }
            catch(Exception e)
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(backupNode);
                throw new Exception("Incorrect path", e); 
            }
            finally
            {
                treeView1.ExpandAll();
                treeView1.EndUpdate();
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo, int level)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            try
            {
                foreach (DirectoryInfo subDir in subDirs)
                {
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ImageKey = "folder";
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0 && level < 1)
                    {
                        GetDirectories(subSubDirs, aNode, level + 1);
                    }
                    nodeToAddTo.Nodes.Add(aNode);

                    if (level < 1)
                    {
                        foreach (FileInfo file in subDir.GetFiles())
                        {
                            aNode.Nodes.Add(file.Name);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                ExceptionHelper.WriteException(e, "GetDirectories");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveCurrDir();
        }

        private void saveCurrDir()
        {
            File.WriteAllText(Consts.lastDirFilepath, currDir);
        }

        private void tbInput_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        commandsHistory.Add(tbInput.Text);
                        RichTBVWriteLine(tbInput.Text, true);

                        var input = tbInput.Text.Split(' ');
                        switch (input[0])
                        {
                            case "help":
                                {
                                    if (input.Length > 1)
                                    {
                                        OutputHelp(input[1]);
                                    }
                                    else
                                    {
                                        OutputHelp();
                                    }
                                    
                                    break;
                                }
                            case "cp":
                                {
                                    CopyFileOrDir(input[1], input[2]);
                                    break;
                                }
                            case "del":
                                {
                                    DeleteFileOrDir(input[1]);
                                    break;
                                }
                            case "info":
                                {
                                    OutputFileOrDirInfo(input[1]);
                                    break;
                                }
                            case "cd":
                                {
                                    moveCurrentDir(input[1]);
                                    break;
                                }
                            case "clr":
                                {
                                    richTextBox1.Clear();
                                    break;
                                }
                            default:
                                {
                                    RichTBVWriteLine("Unknown command");
                                    break;
                                }
                        }
                        tbInput.Text = "";
                        break;
                    }
                case Keys.Up:
                    {
                        var curr = commandsHistory.Find(x => x == currStr);
                        if(curr == tbInput.Text && commandsHistory.IndexOf(curr) != 0)
                        {
                            currStr = commandsHistory[commandsHistory.IndexOf(curr) - 1];
                            tbInput.Text = currStr;
                        }
                        else
                        {
                            currStr = commandsHistory[commandsHistory.Count - 1];
                            tbInput.Text = currStr;
                        }

                        break;
                    }
                case Keys.Down:
                    {
                        var curr = commandsHistory.Find(x => x == currStr);
                        if (curr == tbInput.Text && commandsHistory.IndexOf(curr) != (commandsHistory.Count - 1))
                        {
                            currStr = commandsHistory[commandsHistory.IndexOf(curr) + 1];
                            tbInput.Text = currStr;
                        }
                        else
                        {
                            currStr = commandsHistory[0];
                            tbInput.Text = currStr;
                        }

                        break;
                    }
            }
        }

        private void OutputHelp(string input = "")
        {
            if(input == "")
            {
                foreach(var item in Consts.CommandsDescriptions)
                {
                    RichTBVWriteLine("-" + item);
                }
            }
            else
            {
                if (Consts.CommandList.Contains(input))
                {
                    RichTBVWriteLine(Consts.CommandsDescriptions[Array.IndexOf(Consts.CommandList,input)]);
                }
            }
        }

        private void OutputFileOrDirInfo(string input)
        {
            var tmpStr = ManagerHelper.NormalizePath(input, currDir);
            try
            {
                FileAttributes attr = File.GetAttributes(tmpStr);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    DirectoryInfo di = new DirectoryInfo(tmpStr);
                    RichTBVWriteLine($"Directory Name: {di.Name}");
                    RichTBVWriteLine($"Directory path: {di.FullName}");
                    RichTBVWriteLine($"Creation time: {di.CreationTime}");
                    RichTBVWriteLine($"Last access time: {di.LastAccessTime}");
                    RichTBVWriteLine($"Last write time: {di.LastWriteTime}");
                }
                else
                {
                    FileInfo fi = new FileInfo(tmpStr);
                    if (fi.Exists)
                    {
                        RichTBVWriteLine($"File Name: {fi.Name}");
                        RichTBVWriteLine($"File path: {fi.FullName}");
                        RichTBVWriteLine($"File Extension: {fi.Extension}");
                        RichTBVWriteLine($"File Size in Bytes: {fi.Length}");
                        RichTBVWriteLine($"Is ReadOnly: {fi.IsReadOnly}");
                        RichTBVWriteLine($"Creation time: {fi.CreationTime}");
                        RichTBVWriteLine($"Last access time: {fi.LastAccessTime}");
                        RichTBVWriteLine($"Last write time: {fi.LastWriteTime}");
                    }
                }
                PopulateTreeView(currDir);
            }
            catch (FileNotFoundException e)
            {
                RichTBVWriteLine("Can't delete directory while it's open");
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
            catch (DirectoryNotFoundException e)
            {
                RichTBVWriteLine(e.Message);
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
        }

        private void CopyFileOrDir(string v1, string v2)
        {
            var src = ManagerHelper.NormalizePath(v1, currDir);
            var dest = ManagerHelper.NormalizePath(v2, currDir);
            try
            {
                FileAttributes attr = File.GetAttributes(src);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    ManagerHelper.DirectoryCopy(src, dest);                    
                }
                else
                {
                    File.Copy(src, dest);
                }

                PopulateTreeView(currDir);
            }
            catch (FileNotFoundException e)
            {
                RichTBVWriteLine(e.Message);
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
            catch (DirectoryNotFoundException e)
            {
                RichTBVWriteLine(e.Message);
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
        }

        private void DeleteFileOrDir(string input)
        {
            var tmpStr = ManagerHelper.NormalizePath(input, currDir);
            try
            {
                FileAttributes attr = File.GetAttributes(tmpStr);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    DirectoryInfo di = new DirectoryInfo(tmpStr);

                    di.Delete(true);
                }
                else
                {
                    File.Delete(tmpStr);
                }

                PopulateTreeView(currDir);
            }
            catch(FileNotFoundException e)
            {
                RichTBVWriteLine("Can't delete directory while it's open");
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
            catch (DirectoryNotFoundException e)
            {
                RichTBVWriteLine(e.Message);
                ExceptionHelper.WriteException(e, "DeleteFileOrDir");
            }
        }

        /// <summary>
        /// Меняет текущую директорию
        /// </summary>
        /// <param name="input">Путь к новой директории (абсолютный или относительный)</param>
        private void moveCurrentDir(string input)
        {
            try
            {
                var str = ManagerHelper.NormalizePath(input, currDir);
                PopulateTreeView(str);
                currDir = str;
            }
            catch(Exception e)
            {
                RichTBVWriteLine(e.Message);
                ExceptionHelper.WriteException(e, "moveCurrentDir");
            }
            finally
            {
                RichTBVWriteLine($"Current directory: {currDir}");
            }
        }

        private void RichTBVWriteLine(string input, bool userWrite = false)
        {
            if (userWrite)
            {
                input = ">" + input;
            }

            richTextBox1.Text += "\n" + input;
        }


    }
}
