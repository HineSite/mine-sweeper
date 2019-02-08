using System.Collections;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   class Button
   {
      public delegate void OnClickEventHandler();

      private Hashtable _events = new Hashtable();

      public event OnClickEventHandler OnClick
      {
         add
         {
            _events["OnClick"] = (OnClickEventHandler)_events["OnClick"] + value;
         }

         remove
         {
            _events["OnClick"] = (OnClickEventHandler)_events["OnClick"] - value;
         }
      }

      private string _text = "";
      private TextWriter _textWriter = null;
      private RectangleF _bounds;

      public Button(string text, RectangleF bounds)
      {
         _bounds = bounds;
         _textWriter = new TextWriter(8);

         UpdateText(text);

         Mouse.OnLeftUp += OnLeftUp;
      }

      public void UpdateText(string newText)
      {
         _text = newText;

         _textWriter.Clear();
         _textWriter.AddLine(_text);
         _textWriter.CreateTexture();
      }

      private void OnLeftUp(MouseEventArgs mouseEventArgs)
      {
         if (_bounds.IntersectsWith(mouseEventArgs.WindowPos))
         {
            if (_events.ContainsKey("OnClick"))
               ((OnClickEventHandler)_events["OnClick"])();
         }
      }

      public void Draw()
      {
         float widthHalf = (_bounds.Width / 2.0f);
         float heightHalf = (_bounds.Height / 2.0f);

         GL.PushMatrix();
         GL.Disable(EnableCap.LineSmooth);

         GL.Translate(_bounds.X + widthHalf, _bounds.Y + heightHalf, 0.0f);

         GL.Begin(PrimitiveType.Quads);
         GL.Color3(Color.LightGray);
         GL.Vertex2(-widthHalf, heightHalf);
         GL.Vertex2(-widthHalf, -heightHalf);
         GL.Vertex2(widthHalf, -heightHalf);
         GL.Vertex2(widthHalf, heightHalf);

         GL.Color3(Color.White);
         GL.Vertex2(-widthHalf + 1, heightHalf - 1);
         GL.Vertex2(-widthHalf + 1, -heightHalf + 1);
         GL.Vertex2(widthHalf - 1, -heightHalf + 1);
         GL.Vertex2(widthHalf - 1, heightHalf - 1);
         GL.End();


         _textWriter.Draw();

         GL.PopMatrix();
      }
   }
}
