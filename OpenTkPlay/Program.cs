namespace OpenTkPlay
{
   public class Program
   {
      private Program() { }

      static void Main(string[] args)
      {
         using (new Game()) { }
      }
   }
}
