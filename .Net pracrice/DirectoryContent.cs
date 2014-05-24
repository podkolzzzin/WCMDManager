using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net_pracrice;
using System.Collections;
using System.IO;
namespace Net_pracrice
{
    class DirectoryContent
    {
        private int _x, _y, _w, _h;
        private string _path;
        private static int _counter = 0;
        private bool isActive;
        private ArrayList children;
        private int curY,maxY,numFolders,numFiles;
        private ConsoleColor textColor;
        private EditControl _edit,_searchEdit;
        public string[] dirs,files;
        public string copy;
        public bool isCopyDir,isCut;
        public DirectoryContent(int x, int y, int width, int height)
        {
            _searchEdit = null;
            isCopyDir = false;
            copy = null;
            isCut = false;
            this._x = x;
            this._y = y;
            this._w = width;
            this._h = height;
            children = new ArrayList();
            _edit = new EditControl(x, y, width, height); 
            _counter++;
            textColor = ConsoleColor.Gray;
            isActive = false;
            _path = "";
            curY = 0;
            maxY = 0;
            Console.SetCursorPosition(1, Console.WindowHeight - 1);
        }
        public void active()
        {
            isActive = true;
            textColor = ConsoleColor.White;
            _edit.active();
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(_x + 1, _y + 1);
            Console.Write(Func.strWithLength(_path, _w - 2));
            Border.drawHorizontal(_x + 1, _y + 2, _w - 2, textColor);
        }
        public void deActive()
        {
            isActive = false;
            textColor = ConsoleColor.Gray;
            _edit.deActive();
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(_x + 1, _y + 1);
            Console.Write(Func.strWithLength(_path, _w - 2));
            Border.drawHorizontal(_x + 1, _y + 2, _w - 2, textColor);
        }
        public void setPath(string path)
        {
            _path = path;
            update(true);
        }
       
        public void update(bool pathChange=false)
        {
            if(curY<children.Count && curY>=0)
                ((ConsoleElement)children[curY]).deActive();
            _edit.clear();
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(_x + 1, _y + 1);
            Console.Write(Func.strWithLength(_path,_w-2));
            Border.drawHorizontal(_x + 1, _y + 2, _w - 2, textColor);
            Console.SetCursorPosition(_x + 1, _y);
            dirs = Directory.GetDirectories(_path);
            files = Directory.GetFiles(_path);
            numFolders = dirs.Length;

            maxY = dirs.Length + files.Length;


            for (int i = 0; i < dirs.Length; i++)
            {
                _edit.addChild(Func.getDirStr(dirs[i], _w - 2));
            }
            
            for (int i = 0; i < files.Length; i++)
            {
                _edit.addChild(Func.getFileStr(files[i], _w - 2));
            }
            if (maxY > 0)
                _edit.activate(0);
            curY = 0;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
        }
        private char prevChar;
        private bool findDirs = true;
        public void tryToFind(char c)
        {
            if (c == prevChar)
            {
                findDirs = !findDirs;
            }
            int pos;
            bool fileIndex = false;
            if (findDirs)
            {
                pos = tryToFindDir(c);
                if (pos == -1)
                { 
                    pos = tryToFindFile(c);
                    fileIndex = true;
                }
            }
            else
            {
                fileIndex = true;
                pos = tryToFindFile(c);
                if (pos == -1)
                {
                    fileIndex = false;
                    pos = tryToFindDir(c); 
                }
            }
            if (pos == -1) return;
            int index;
            if (fileIndex) index = pos + dirs.Count();
            else index = pos;
            _edit.deActivate(curY);
            _edit.show(index);
            
            curY = index;
            prevChar = c;
        }
        public int tryToFindDir(char c)
        {
            DirectoryInfo di;
            for (int i = 0; i < dirs.Length; i++)
            {
                di = new DirectoryInfo(dirs[i]);
                if (Char.ToLower(di.Name[0]) == c) return i;
            }
            return -1;
        }
        public int tryToFindFile(char c)
        {
            FileInfo fi;
            for (int i = 0; i < files.Length; i++)
            {
                fi = new FileInfo(files[i]);
                if (Char.ToLower(fi.Name[0]) == c) return i;
            }
            return -1;
        }
        public void findKeyProc(ArrayList files,ArrayList dirs)
        {
            if (_searchEdit != null) _searchEdit.destruct();
            _searchEdit = new EditControl(15, 15, Console.WindowWidth - 30, Console.WindowHeight - 30);
            

            _searchEdit.active();
            foreach (DirectoryInfo file in dirs)
                _searchEdit.addChild(Func.getDirStr(file.FullName, Console.WindowWidth - 32));
            foreach (FileInfo file in files)
                _searchEdit.addChild(Func.getFileStr(file.FullName, Console.WindowWidth - 32));


            Console.SetCursorPosition(16, 16);
            Console.Write(Func.strWithLength("Search results",Console.WindowWidth-32));
            Console.SetCursorPosition(16, 17);
            Console.Write(new string(' ',Console.WindowWidth-32));
            Console.SetCursorPosition(15, Console.WindowHeight - 15);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(Func.strWithLength("F12 - open parent filder | ESC - close",Console.WindowWidth-30));
            int curY=0;
            _searchEdit.activate(curY);
            ConsoleKeyInfo key;
            bool isEscape = false;
            while (true)
            {
                key=Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        {
                            if (curY < dirs.Count) setPath(((DirectoryInfo)dirs[curY]).FullName);
                            else System.Diagnostics.Process.Start(((FileInfo)files[curY - dirs.Count]).Directory.FullName);
                            isEscape = true;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            isEscape = true;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (curY - 1 >= 0)
                            {
                                _searchEdit.deActivate(curY);
                                curY--;
                                _searchEdit.activate(curY);
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (curY + 1 < dirs.Count + files.Count)
                            {
                                _searchEdit.deActivate(curY);
                                curY++;
                                _searchEdit.activate(curY);
                            }
                            break;
                        }
                    case ConsoleKey.F12:
                        {
                            if (curY < dirs.Count) setPath(((DirectoryInfo)dirs[curY]).Parent.FullName);
                            else setPath(((FileInfo)files[curY-dirs.Count]).Directory.FullName);
                            isEscape = true;
                            break;
                        }
                }
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                if (isEscape) break;
            }

            _searchEdit.destruct();
            _searchEdit = null;

        }
        public void keyProc(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    {
                        activateNext();
                        break;
                    }
                case ConsoleKey.UpArrow:
                    {
                        activatePrev();
                        break; 
                    }
                case ConsoleKey.Enter:
                    {
                        execute();
                        break;
                    }
                case ConsoleKey.Backspace:
                    {
                        upperFolder();
                        break;
                    }
                case ConsoleKey.Delete:
                    {
                        if (Window.confirm("Are you sure?"))
                        {
                            try
                            {
                                delete();
                            }
                            catch (System.IO.IOException)
                            {
                                Window.alert("Access denied");
                            }
                        }
                        break;
                    }
                case ConsoleKey.F5:
                    {
                        logicalDriveSelectWindow();
                        break;
                    }
                case ConsoleKey.F4:
                    { 
                        DirectoryInfo dir=new DirectoryInfo(_path);
                        setPath(dir.Root.Name);
                        break;
                    }
                case ConsoleKey.F6:
                    {
                        propertiesBox();
                        break;
                    }
                case ConsoleKey.F7:
                    {
                        if (isFolder())
                        {
                            DirectoryInfo dir = new DirectoryInfo(dirs[curY]);
                            try
                            {
                                string tPath = dir.Parent.FullName;
                                if (dir.Parent.FullName.Length > 3) tPath += '\\';
                                dir.MoveTo(tPath+Window.prompt("Rename folder " + dir.Name, "Name", 65));
                                setPath(_path);
                            }
                            catch
                            {
                                Window.alert("Can not rename folder");
                            }
                        }
                        break;
                    }
                case ConsoleKey.F8:
                    {
                        string pattern = Window.prompt("Find", "Search word ", 65);
                        if (pattern.Length == 0) break;
                        ArrayList fileRez=Func.findFiles(new DirectoryInfo(_path),pattern);
                        ArrayList dirRez = Func.findDirectories(new DirectoryInfo(_path), pattern);


                        findKeyProc(fileRez,dirRez);
                        break;
                        
                       
                    }
                case ConsoleKey.F9:
                    {
                        string rez = Window.prompt("New folder", "Folder name", 60);
                        string tPath=_path;
                        if (_path.Length > 3) tPath += '\\';
                        try
                        {
                            tPath += rez;
                            Directory.CreateDirectory(tPath);
                            setPath(_path);
                        }
                        catch
                        {
                            Window.alert("Can not create directory");
                        }
                        
                        break;
                    }
                default:
                    {
                        char c=(Char.ToLower(key.KeyChar));
                        if((c>='a' && c<='z') || (c>='а' && c<='я'))
                        {
                            tryToFind(c);
                        }
                        break;
                    }
            }
        }
        private void propertiesBox()
        {
            ArrayList mas=new ArrayList();
            if (isFolder())
            {
                DirectoryInfo dir=new DirectoryInfo(dirs[curY]);
                mas.Add(Func.strWithLength("Name: ",20)+Func.strWithLength(dir.Name,65));
                mas.Add(Func.strWithLength("Root directory: ",20)+Func.strWithLength(dir.Root.Name,65));
                mas.Add(Func.strWithLength("Parent directory: ",20)+Func.strWithLength(dir.Parent.FullName,65));
                mas.Add(Func.strWithLength("Last read time: ", 20) + Func.strWithLength(dir.LastAccessTime.ToString(), 65));
                mas.Add(Func.strWithLength("Last write time: ",20) + Func.strWithLength(dir.LastWriteTime.ToString(),65));
                DirStatistics t = Func.getDirStatistics(dir);
                mas.Add(Func.strWithLength("Size: ",20)+ Func.strWithLength(Func.fileSizeStr(t.size),65));
                mas.Add(Func.strWithLength("Files: ",20) + Func.strWithLength(t.numFiles.ToString(),65));
                mas.Add(Func.strWithLength("Folders: ",20) + Func.strWithLength(t.numDirs.ToString(),65));
            }
            else
            {
                FileInfo dir=new FileInfo(files[curY-dirs.Length]);
                mas.Add(Func.strWithLength("Name: ",20)+Func.strWithLength(dir.Name,65));
                mas.Add(Func.strWithLength("Parent directory: ",20)+Func.strWithLength(dir.DirectoryName,65));
                mas.Add(Func.strWithLength("Root directory: ",20)+Func.strWithLength(dir.Directory.Root.Name,65));
                mas.Add(Func.strWithLength("Is read only: ",20)+Func.strWithLength(dir.IsReadOnly.ToString(),65));
                mas.Add(Func.strWithLength("Last read time: ", 20) + Func.strWithLength(dir.LastAccessTime.ToString(), 65));
                mas.Add(Func.strWithLength("Last write time: ",20) + Func.strWithLength(dir.LastWriteTime.ToString(),65));
                mas.Add(Func.strWithLength("Size: ", 20) + Func.strWithLength(Func.fileSizeStr(dir.Length), 65));
            }
            Window.multiAlert(mas,65+20+4);
        }
        private void delete()
        {
            if (isFolder())
            {
                try
                {
                    new DirectoryInfo(dirs[curY]).Delete(true);
                }
                catch
                {
                    Window.alert("Access denied");
                }
            }
            else
            {
                try
                {
                    new FileInfo(files[curY - dirs.Length]).Delete();
                }
                catch
                {
                    Window.alert("Access denied");
                }
            }
            setPath(_path);
        }
        public bool isFolder()
        {
            if (curY >= numFolders) return false;
            return true;
        }
        public string getCurrent()
        {
            if (isFolder()) return dirs[curY];
            else return files[curY - dirs.Length];
        }
        public string getPath()
        {
            return _path;
        }
        public void upperFolder()
        {
            DirectoryInfo t=Directory.GetParent(_path);
            if (t == null)
            {
                logicalDriveSelectWindow();
                return;
            }
            setPath(t.FullName);
        }
        public void logicalDriveSelectWindow()
        {
            string[] drives = Directory.GetLogicalDrives();
            int pos=Window.selectiveWindow(drives);
            if (pos == -1) return;
            string savePath = _path;
            try
            {
                setPath(drives[pos]);
            }
            catch(IOException)
            {
                setPath(savePath);
                Window.alert("Device is not ready");
            }
        }
        public void execute()
        {
            if (isFolder())
            {
                goInto();
            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(files[curY - numFolders]);
                }
                catch { }
            }
        }
        public void goInto()
        {
            string tPath=_path;
            try
            {
                setPath(dirs[curY]);
            }
            catch (System.UnauthorizedAccessException)
            {
                Window.alert("Access denied");
                setPath(tPath);
            }
        }
        public void activateNext()
        {
            if (curY + 1 < maxY)
            {
                _edit.deActivate(curY);
                curY++;
                _edit.activate(curY);
            }
        }
        public void activatePrev()
        {
            if (curY - 1 >= 0)
            {
                _edit.deActivate(curY);
                curY--;
                _edit.activate(curY);
            }
        }
    }
}
