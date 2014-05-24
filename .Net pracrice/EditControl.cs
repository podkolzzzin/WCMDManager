using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Net_pracrice
{
    class EditControl
    {
        private int _x, _y, _w, _h;
        private ConsoleColor textColor;
        private bool isActive;
        private ArrayList children;
        private ArrayList texts;
        private int currentMin,currentMax;
        public EditControl(int x, int y, int width, int height)
        {
            isActive = false;
            children = new ArrayList();
            texts = new ArrayList();
            _x = x;
            _y = y;
            _w = width;
            _h = height;
            currentMin=0;
            currentMax=1;
            textColor = ConsoleColor.Gray;
            Border.DrawBorder(_x, _y, _w, _h);
        }
        public void active()
        {
            isActive = true;
            textColor = ConsoleColor.White;
            Border.DrawBorder(_x, _y, _w, _h,textColor);
            updateColor();
        }
        public void deActive()
        {
            isActive = false;
            textColor = ConsoleColor.Gray;
            Border.DrawBorder(_x, _y, _w, _h, textColor);
            updateColor();
        }
        public void show(int index)
        {
            if (index < currentMax && index >= currentMin)
                activate(index);
            else
            {
                if (index < currentMin)
                { 
                    currentMin = index;
                    currentMax = currentMin + _h - 4;
                }
                if (index >= currentMax)
                { 
                    currentMax = index;
                    currentMin = currentMax - _h + 4;
                }
                showTexts();
                activate(index);
            }
        }
        public void activate(int index)
        {
            int i=index - currentMin;
            if (index >= currentMin && index < currentMax)
            {
                ((ConsoleElement)children[i]).active(); 
            }
            else
            {
                if (index >= currentMax)
                    down(index);
                else up(index);
            }
        }
        public void up(int index)
        {
            int delta = index - currentMin ;
            currentMax += delta;
            currentMin += delta;
            showTexts();
            activate(currentMin);

        }
        public void down(int index)
        {
            int delta=currentMax - index + 1;
            currentMax += delta;
            currentMin += delta;
            showTexts();
            activate(currentMax - 1);     
        }
        public void showTexts()
        { 
            _clear();
            int saveMin = currentMin;
            int saveMax = currentMax;
            for (int i = currentMin; i < saveMax; i++)
            {
                addChild((string)texts[i]);
            }
            currentMax = saveMax;
        }
        public void deActivate(int index)
        {
            if(index>=currentMin && index<currentMax)
                ((ConsoleElement)children[index - currentMin]).deActive();
        }
        private void _clear()
        {
            for (int i = 0; i < children.Count; i++)
            {
                ((ConsoleElement)children[i]).destruct();
            }
            
            children.Clear();            
        }
        public void clear()
        {
            _clear();
            currentMax = currentMin = 0;
            texts.Clear();
        }
        private void updateColor()
        {
            for (int i = 0; i < children.Count; i++)
            {
                ((ConsoleElement)children[i]).setTextColor(textColor);
            }
        }
        public void addChild(string text)
        {         
            if (children.Count < _h - 4)
            {
                ConsoleElement t = new ConsoleElement(_x + 1, _y + children.Count + 3, _w - 2, 1, text, ConsoleElementStyle.LEFT, textColor);
                t.deActive();
                children.Add(t);
                currentMax++;
            }
            texts.Add(text);
        }
        public void destruct()
        {
            Window.parent.redraw();
        }
    }
}
