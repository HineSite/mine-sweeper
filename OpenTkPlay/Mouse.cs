using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenTkPlay
{
   /********************************************************************************
    * 
    * GetAsyncKeyState
    *    If the function succeeds, the return value specifies whether the key was 
    *       pressed since the last call to GetAsyncKeyState, and whether the key is 
    *       currently up or down. If the most significant bit is set, the key is down, 
    *       and if the least significant bit is set, the key was pressed after the 
    *       previous call to GetAsyncKeyState. The return value is zero if a window 
    *       in another thread or process currently has the keyboard focus.
    *    
    ********************************************************************************/


   public class MouseEventArgs
   {
      public Point ScreenPos;
      public PointF WindowPos;
      public PointF NormalizedPos;
   }

   public static class Mouse
   {
      public delegate void OnLeftUpEventHandler(MouseEventArgs mouseEventArgs);
      public delegate void OnRightUpEventHandler(MouseEventArgs mouseEventArgs);

      private static bool _leftDown = false;
      private static bool _rightDown = false;
      private static Hashtable _events = new Hashtable();

      public static int LeftCount = 0;

      public static event OnLeftUpEventHandler OnLeftUp
      {
         add
         {
            _events["OnLeftUp"] = (OnLeftUpEventHandler)_events["OnLeftUp"] + value;
            LeftCount++;
         }

         remove
         {
            _events["OnLeftUp"] = (OnLeftUpEventHandler)_events["OnLeftUp"] - value;
            LeftCount--;
         }
      }

      public static event OnRightUpEventHandler OnRightUp
      {
         add
         {
            _events["OnRightUp"] = (OnRightUpEventHandler)_events["OnRightUp"] + value;
         }

         remove
         {
            _events["OnRightUp"] = (OnRightUpEventHandler)_events["OnRightUp"] - value;
         }
      }

      private enum MouseButtons : ushort
      {
         LEFT = 0x01,
         RIGHT = 0x02,
         MIDDLE = 0x04,
         X1 = 0x05,
         X2 = 0x06
      }

      [DllImport("user32.dll")]
      public static extern bool GetCursorPos(out Point lpPoint);

      [DllImport("user32.dll")]
      public static extern short GetAsyncKeyState(UInt16 virtualKeyCode);

      public static void Update(RectangleF windowBounds)
      {
         MouseEventArgs eventArgs = new MouseEventArgs();
         GetCursorPos(out eventArgs.ScreenPos);

         short leftState = GetAsyncKeyState((ushort)MouseButtons.LEFT);
         short rightState = GetAsyncKeyState((ushort)MouseButtons.RIGHT);

         if (!windowBounds.IntersectsWith(eventArgs.ScreenPos))
         {
            if (leftState == 0)
               _leftDown = false;

            if (rightState == 0)
               _rightDown = false;

            return;
         }

         eventArgs.WindowPos = new PointF(eventArgs.ScreenPos.X - windowBounds.X, eventArgs.ScreenPos.Y - windowBounds.Y);
         eventArgs.NormalizedPos = eventArgs.WindowPos.Normalize(0, windowBounds.Width, 0, windowBounds.Height);


         //if (leftState == short.MinValue)
         if (leftState != 0)
         {
            _leftDown = true;
         }
         else
         {
            if (_leftDown)
            {
               _leftDown = false;

               if (_events.ContainsKey("OnLeftUp"))
                  ((OnLeftUpEventHandler)_events["OnLeftUp"])(eventArgs);
            }
         }

         if (rightState == short.MinValue)
         {
            _rightDown = true;
         }
         else
         {
            if (_rightDown)
            {
               _rightDown = false;

               if (_events.ContainsKey("OnRightUp"))
                  ((OnRightUpEventHandler)_events["OnRightUp"])(eventArgs);
            }
         }
      }
   }
}
