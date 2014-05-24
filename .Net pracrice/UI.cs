using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Net_pracrice
{
    static public class Border
    {
        public const char topleft = (char)9556;
        public const char bottomleft = (char)9562;
        public const char topright = (char)9559;
        public const char bottomright = (char)9565;
        public const char vert = (char)9553;
        public const char horiz = (char)9552;
        public static void drawHorizontal(int x1,int y1,int length,ConsoleColor color=ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x1, y1);
            Console.Write(new string(Border.horiz,length));
        }
        public static void drawVertical(int x1,int y1,int length,ConsoleColor color=ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < length; i++)
            {
                Console.SetCursorPosition(x1, y1 + i);
                Console.Write(Border.vert);
            }
        }
        public static void DrawBorder(int _x,int _y,int _w,int _h,ConsoleColor color=ConsoleColor.Gray)
        {
            //9556 9552 9559 9565 9553 9562 9559
            Console.ForegroundColor = color;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(_x, _y);
            Console.Write((char)Border.topleft);
            Console.Write(new string(Border.horiz, _w - 2));
            Console.Write(Border.topright);
            for (int i = 1; i < _h - 1; i++)
            {
                Console.SetCursorPosition(_x, _y + i);
                Console.Write(Border.vert);
                Console.SetCursorPosition(_x + _w - 1, _y + i);
                Console.Write(Border.vert);
            }
            Console.SetCursorPosition(_x, _y + _h - 1);
            Console.Write(Border.bottomleft);
            Console.Write(new string(Border.horiz, _w - 2));
            Console.Write(Border.bottomright);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
        }
    }
    class UI
    {
        public static void drawRect(int x, int y, int width, int height, System.ConsoleColor color = ConsoleColor.White)
        {
            string str = new string(' ', width);
            Console.BackgroundColor = color;
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(str);
            }
        }
    }
    enum ConsoleElementStyle
    {
        CENTER, LEFT, RIGHT
    }
    class ConsoleElement
    {
        private int x, y, w, h;
        private string text, rez;
        private ConsoleColor _textColor;
        private ConsoleColor _bgColor;
        private bool wasDrawn = false,isActive=false;
        public ConsoleElement(int x, int y, int w, int h, string text, ConsoleElementStyle style = ConsoleElementStyle.CENTER,ConsoleColor textColor=ConsoleColor.White,ConsoleColor bgColor=ConsoleColor.DarkGray)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this._bgColor = bgColor;
            this._textColor = textColor;
            this.text = text;
            if (style == ConsoleElementStyle.CENTER)
            {
                rez = new string(' ', (this.w - this.text.Length) / 2);
                rez = rez + text + rez;
            }
            else if (style == ConsoleElementStyle.RIGHT)
            {
                rez = new string(' ', this.w - this.text.Length);
                rez += text;
            }
            else
            {
                rez = new string(' ', this.w - this.text.Length);
                rez = text + rez;
            }


            //this.deActive();
        }
        public void active()
        {
            wasDrawn = true;
            isActive = true;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = _bgColor;
            Console.BackgroundColor = _textColor;
            Console.Write(rez);
        }
        public void deActive()
        {
            wasDrawn = true;
            isActive = false;
            Console.ForegroundColor = _textColor;
            Console.BackgroundColor = _bgColor;
            Console.SetCursorPosition(x, y);
            Console.Write(rez);
        }
        public void setTextColor(ConsoleColor color)
        {
            _textColor = color;
            if (wasDrawn && isActive) active();
            else if (wasDrawn) deActive();
        }
        public void destruct()
        {
            if (wasDrawn)
            {
                Console.BackgroundColor = _bgColor;
                Console.SetCursorPosition(x, y);
                Console.Write(new string(' ', rez.Length));
            }
        }

        internal void active(int index)
        {
            throw new NotImplementedException();
        }
    }
    struct DirStatistics
    {
        public Int64 size;
        public int numFiles;
        public int numDirs;
        public static DirStatistics operator +(DirStatistics n,DirStatistics tt)
        {
            DirStatistics t;
            t.size = tt.size + n.size;
            t.numDirs = tt.numDirs + n.numDirs;
            t.numFiles = tt.numFiles + n.numFiles;
            return t;
        }

    };
    class Func
    {
        public static string strWithLength(string str, int length)
        {
            string rez;
            if (str.Length > length)
                rez = str.Substring(0, length - 5)+"[...]";
            else
                rez=str+(new string(' ',length-str.Length));
            return rez;
        }
        public static string getDirStr(string dirName,int length)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirName);
            return strWithLength(di.Name, length - 20) + "  " + strWithLength("<dir>", 8) + (new string(' ', 10));
        }
        public static string fileSizeStr(Int64 length)
        {
            if (length < 10240) return length.ToString() + " Byte";
            else if (length < 1024 * 1024 * 10) return (length / 1024).ToString() + " KB";
            else if (length < (Int64)1024 * 1024 * 1024 * 10) return (length / 1024 / 1024).ToString() + " MB";
            else if (length < (Int64)1024 * 1024 * 1024 * 1024 * 10) return (length / 1024 / 1024 / 1024).ToString() + " GB";
            else  return (length / 1024 / 1024 / 1024 / 1024).ToString() + " TB";

        }
        public static string getFileStr(string fileName, int length)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(fileName);

            string tName = strWithLength(fi.Name, length - 20) + "  ";
            if (fi.Extension.Length > 1)
               tName += strWithLength('<' + fi.Extension.Substring(1) + '>', 8);
            else
                tName += strWithLength(" ", 8);
            tName += fileSizeStr(fi.Length);
            return tName;
        }
        public static DirStatistics getDirStatistics(System.IO.DirectoryInfo dir)
        {
            System.IO.FileInfo[] files =null;
            try
            {
                files = dir.GetFiles();
            }
            catch
            { };
            DirStatistics rez;
            rez.size = 0;
            rez.numFiles = 0;
            rez.numDirs = 0;
            
            if (files != null)
            {
                rez.numFiles = files.Length;
                for (int i = 0; i < files.Length; i++)
                    rez.size += files[i].Length;
            }
            System.IO.DirectoryInfo[] dirs=null;
            try
            {
                dirs = dir.GetDirectories();
            }
            catch { };
            if (dirs != null)
            {
                rez.numDirs = dirs.Length;
                for (int i = 0; i < dirs.Length; i++)
                    rez += getDirStatistics(dirs[i]);
            }
            return rez;
        }
        public static ArrayList findFiles(System.IO.DirectoryInfo dir, string pattern, ArrayList  rez=null)
        {
            if (rez == null) rez = new ArrayList();
            pattern = pattern.ToLower();
            System.IO.FileInfo[] files=null;
            try
            {
                files = dir.GetFiles();
            }
            catch { };
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.ToLower().IndexOf(pattern) != -1) rez.Add(files[i]);
                }
            }
            System.IO.DirectoryInfo[] dirs=null;
            try
            {
                dirs = dir.GetDirectories();
            }
            catch { };
            if (dirs != null)
            {
                for (int i = 0; i < dirs.Length; i++)
                    findFiles(dirs[i], pattern, rez);
            }
            return rez;
        }
        public static ArrayList findDirectories(System.IO.DirectoryInfo dir, string pattern, ArrayList rez = null)
        {
            if (rez == null) rez = new ArrayList();
            pattern = pattern.ToLower();
            System.IO.DirectoryInfo[] dirs = null;
            try
            {
                dirs = dir.GetDirectories();
            }
            catch { };
            if (dirs != null)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (dirs[i].Name.ToLower().IndexOf(pattern) != -1) rez.Add(dirs[i]);
                    findDirectories(dirs[i], pattern, rez);
                }
            }
            return rez;            
        }
    }
    class Window
    {
        public static Commander parent=null;
        public static void alert(string message)
        {
            string rez;
            if (message.Length > 5 * Console.WindowWidth / 6) rez = Func.strWithLength(message, 5 * Console.WindowWidth / 6);
            else if (message.Length < 25) rez = Func.strWithLength(message, 25);
            else rez = message;
            int height = 7;
            int width = rez.Length + 4;
            int x = (Console.WindowWidth - (width)) / 2;
            int y = (Console.WindowHeight - height) / 2;
            
            
            Border.DrawBorder(x, y, width, height);
            UI.drawRect(x + 1, y + 1, width-2 , height-2,ConsoleColor.DarkGray);
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write(rez);
            new ConsoleElement(x  +  2, y + 4, width - 4 , 1, " OK ").active();
            ConsoleKeyInfo key;
            while (true)
            {
                key=Console.ReadKey();
                if (key.Key == ConsoleKey.Enter) break;
            }
            if (parent != null) parent.redraw();
        }
        public static bool confirm(string message)
        {
            string rez;
            if (message.Length > 5 * Console.WindowWidth / 6) rez = Func.strWithLength(message, 5 * Console.WindowWidth / 6);
            else if (message.Length < 25) rez = Func.strWithLength(message, 25);
            else rez = message;
            int height = 7;
            int width = rez.Length + 4;
            int x = (Console.WindowWidth - (width)) / 2;
            int y = (Console.WindowHeight - height) / 2;


            Border.DrawBorder(x, y, width, height);
            UI.drawRect(x + 1, y + 1, width - 2, height - 2, ConsoleColor.DarkGray);
            Console.SetCursorPosition(x + 2, y + 2);
            Console.Write(rez);
            ConsoleElement yes = new ConsoleElement(x + 2, y + 4, (width - 4) / 2, 1, "YES");
            ConsoleElement no = new ConsoleElement(x + 2 + (width - 4) / 2, y + 4, (width - 4) / 2, 1, "NO");
            ConsoleKeyInfo key;
            yes.active();
            no.deActive();
            bool isYes = true;
            while (true)
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    isYes = true;
                    yes.active();
                    no.deActive();
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    isYes = false;
                    no.active();
                    yes.deActive();
                }
            }
            if (parent != null) parent.redraw();
            return isYes;
        }
        public static int selectiveWindow(string[] messages)
        {
            ArrayList selections=new ArrayList();
            ConsoleElement tElem;
            int h = messages.Length + 4;
            int w = 44;
            int x = (Console.WindowWidth - w) / 2;
            int y = (Console.WindowHeight - h) / 2;
            Border.DrawBorder(x, y, w, h);
            UI.drawRect(x + 1, y + 1, w - 2, h - 2,ConsoleColor.DarkGray);
            x += 2;
            y += 2;
            w -= 4;
            for(int i=0;i<messages.Length;i++)
            {
                tElem = new ConsoleElement(x, y + i, w, h, messages[i], ConsoleElementStyle.LEFT);
                selections.Add(tElem);
                if (i == 0) tElem.active();
                else tElem.deActive();
            }
            int active = 0;
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow)
                {
                    ((ConsoleElement)selections[active]).deActive();
                    active++;
                    active %= messages.Length;
                    ((ConsoleElement)selections[active]).active();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    ((ConsoleElement)selections[active]).deActive();
                    active--;
                    if (active < 0) active = messages.Length - 1;
                    ((ConsoleElement)selections[active]).active();
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    parent.redraw();
                    return active;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    parent.redraw();
                    return -1;
                }
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
            }
        }
        public static int selectiveWindow(ArrayList messages)
        {
            ArrayList selections = new ArrayList();
            ConsoleElement tElem;
            int h = messages.Count + 4;
            int w = 85;
            int x = (Console.WindowWidth - w) / 2;
            int y = (Console.WindowHeight - h) / 2;
            Border.DrawBorder(x, y, w, h);
            UI.drawRect(x + 1, y + 1, w - 2, h - 2, ConsoleColor.DarkGray);
            x += 2;
            y += 2;
            w -= 4;
            for (int i = 0; i < messages.Count; i++)
            {
                tElem = new ConsoleElement(x, y + i, w, h, (string)messages[i], ConsoleElementStyle.LEFT);
                selections.Add(tElem);
                if (i == 0) tElem.active();
                else tElem.deActive();
            }
            int active = 0;
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow)
                {
                    ((ConsoleElement)selections[active]).deActive();
                    active++;
                    active %= messages.Count;
                    ((ConsoleElement)selections[active]).active();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    ((ConsoleElement)selections[active]).deActive();
                    active--;
                    if (active < 0) active = messages.Count - 1;
                    ((ConsoleElement)selections[active]).active();
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    parent.redraw();
                    return active;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    parent.redraw();
                    return -1;
                }
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
            }
        }
        public static void multiAlert(ArrayList messages,int _width)
        {
            int height = messages.Count + 6;
            int width = _width;
            int x = (Console.WindowWidth - width) / 2;
            int y = (Console.WindowHeight - height) / 2;
            Border.DrawBorder(x, y, width, height);
            UI.drawRect(x +1, y + 1, width - 2, height - 2, ConsoleColor.DarkGray);
            x += 2;
            y += 2;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < messages.Count; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(messages[i]);
            }

            new ConsoleElement(x, y + height - 4, width - 4, 1, " OK ").active();
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter) break;
            }
            if (parent != null) parent.redraw();            
        }
        public static string prompt(string caption, string name,int maxLength)
        {
            int height = 9;
            int width = maxLength+2+name.Length;
            int x = (Console.WindowWidth - (width)) / 2;
            int y = (Console.WindowHeight - height) / 2;


            Border.DrawBorder(x, y, width, height);
            UI.drawRect(x + 1, y + 1, width - 2, height - 2, ConsoleColor.DarkGray);
            Console.SetCursorPosition(x + 2, y + 2);
            x += 2;
            y += 2;
            Console.Write(caption);
            new ConsoleElement(x + 2, y + 4, width - 4, 1, " OK ").active();
            Console.SetCursorPosition(x + 2, y + 1);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(name + ": ");

            Console.CursorVisible = true;
            
            string rez= Console.ReadLine();           
            if (parent != null) parent.redraw();
            Console.CursorVisible = false;
            return rez;
        }
    }
}
