using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenTkPlay
{
   class Flag
   {
      private RectangleF _bounds;

      public Flag(RectangleF bounds)
      {
         UpdateBounds(bounds);
      }

      public Flag UpdateBounds(RectangleF bounds)
      {
         _bounds = bounds;

         return this;
      }

      public void Draw()
      {
         RectangleF bounds = new RectangleF(_bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);

         float coordWidth = (bounds.Width / 2f);
         float coordHeight = (bounds.Height / 2f);

         float poleWidth = 1;
         float poleHeight = 20;
         float flagWidth = 9;
         float flagHeight = 9;
         float totalWidth = (poleWidth + flagWidth);
         float totalHeight = poleHeight;

         GL.PushMatrix();

         GL.Translate(bounds.X + coordWidth + ((_bounds.Width - totalWidth) / 2f), bounds.Y + coordHeight + ((_bounds.Height - totalHeight) / 2f), 0);

         { // Pole
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.FromArgb(255, 106, 106, 106));
            GL.Vertex2(-coordWidth, -coordHeight);
            GL.Vertex2(-coordWidth, -coordHeight + poleHeight);
            GL.End();
         }

         { // Flag
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.DarkRed);
            GL.Vertex2(-coordWidth + 1, -coordHeight + 1);
            GL.Vertex2(-coordWidth + 1, -coordHeight + 1 + flagHeight);
            GL.Vertex2(-coordWidth + 1 + flagWidth, -coordHeight + 1 + (flagHeight / 2f));
            GL.End();
         }

         GL.PopMatrix();
      }
   }
}
