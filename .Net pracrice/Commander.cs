using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Net_pracrice
{
    public class Commander
    {
        DirectoryContent left, right,current,cutBlock;
        public Commander()
        {
            cutBlock = null;
            left = new DirectoryContent(0, 0, (Console.WindowWidth - 5) / 2, Console.WindowHeight-2);
            left.setPath("D:\\");
            right = new DirectoryContent((Console.WindowWidth - 5) / 2+3 , 0, Console.WindowWidth / 2, Console.WindowHeight-2);
            right.setPath("C:\\");
            current = left;
            Console.CursorVisible = false;
            Window.parent = this;
            drawCommands();
            current.active();
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            work();
        }
        public void drawCommands()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("    F1 - copy | F2 - cut | F3 - paste | F4 - root | F5 - list of disks | F6 - properties | F7 - rename | F8 - find | F9 - new folder   ");
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public void redraw()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            current.active();
            if (left == current)
                right.deActive();
            else
                left.deActive();
            drawCommands();
            
        }
        public string buffer;
        public bool isCut, isFile;
        public void work()
        {
            ConsoleKeyInfo key;
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            while(true)
            {
                key=Console.ReadKey();
                switch(key.Key)
                {
                    case ConsoleKey.Escape:
                        {
                            Console.SetCursorPosition(0, Console.WindowHeight - 1);
                            return;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            if (current == left) break;
                            current.deActive();
                            current = left;
                            current.active();
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (current == right) break;
                            current.deActive();
                            current = right;
                            current.active();
                            break;
                        }
                    case ConsoleKey.F1:
                        {
                            isCut = false;
                            buffer = current.getCurrent();
                            isFile = !current.isFolder();
                            break;
                        }
                    case ConsoleKey.F2:
                        {
                            isCut = true;
                            cutBlock = current;
                            buffer = current.getCurrent();
                            isFile = !current.isFolder();
                            break;
                        }
                    case ConsoleKey.F3:
                        {
                            copyCut();
                            break;
                        }
                    default: current.keyProc(key); break;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(0, Console.WindowHeight-1);
            }
        }
        private string getOnlyName(string name)
        {
            int index=name.LastIndexOf('.');
            if (index != -1) return name.Substring(0, index);
            return name;
        }
        private bool safeFileCopy(FileInfo file, string path)
        {
            bool success = false;
            string tPath;
            if (path.Length > 3)
                tPath = path + '\\' + file.Name;
            else
                tPath = path + file.Name;
            int tryIndex = 0;
            while (!success)
            {
                try
                {
                    file.CopyTo(tPath);
                    return true;
                }
                catch (IOException)
                {
                    tryIndex++;
                    if (path.Length > 3)
                        tPath = path + '\\';
                    else
                        tPath = path;
                    tPath += getOnlyName(file.Name) +'('+tryIndex.ToString()+')' + file.Extension;
                }
            }
            return false;
        }
        private void copyDir(DirectoryInfo dir, string path)
        {
            DirectoryInfo[] dirs=dir.GetDirectories();
            string tPath=path;
            if (path.Length > 3) tPath += "\\";
            Directory.CreateDirectory(tPath);
            for (int i = 0; i < dirs.Length; i++)
            {
                Directory.CreateDirectory(tPath  + "\\" + dirs[i].Name);
                copyDir(dirs[i], tPath + "\\" + dirs[i].Name);
            }
            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)      
                safeFileCopy(files[i], path);
        }
        private void safeFileMove(FileInfo file, string path)
        { 
        
        }
        private void safeDirMove(DirectoryInfo dir, string path)
        {
            try
            {
                copyDir(dir, path);
            }
            catch
            {
                Window.alert("Problems with moving folder");
                return;
            }
            try
            {
                dir.Delete(true);
            }
            catch
            {
                Window.alert("Can not delete sourse folder");
            }
        }
        private void copyCut()
        {
            if (buffer != null && buffer.Length > 3)
            {
                string path = current.getPath();
                if (isFile)
                {
                    FileInfo file = new FileInfo(buffer);
                    if (isCut)
                    {
                        safeFileMove(file, path);
                        if (current != cutBlock) cutBlock.setPath(cutBlock.getPath());
                    }
                    else safeFileCopy(file, path);


                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(buffer);
                    if (isCut)
                    {
                        if (path.Length > 3) path += '\\' + dir.Name;
                        else path += dir.Name;
                        safeDirMove(dir, path);
                        if (current != cutBlock) cutBlock.setPath(cutBlock.getPath());
                    }
                    else
                    {
                   
                        if (path.Length > 3) path += '\\' + dir.Name;
                        else path += dir.Name;
                        try
                        {
                            copyDir(dir, path);
                        }
                        catch
                        {
                            Window.alert("Problems with copying folder");
                        }
                    }
                }
                current.setPath(current.getPath());
            }
        }

    }
}
