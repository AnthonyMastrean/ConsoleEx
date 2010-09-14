using System;

namespace Microsoft.GotDotNet
{
	public class ConsoleExTest
	{
		[STAThread]
		public static void Main(string[] args)
		{
			ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Black);
			ConsoleEx.Clear();
			ConsoleEx.Title = "ConsoleEx Class Demo Application";

			ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Maroon);
			ConsoleEx.DrawRectangle(BorderStyle.LineSingle, 1, 1, 77, 22, true);
			ConsoleEx.WriteAt(20, 11, "The cursor has been temporarily disabled.");
			ConsoleEx.WriteAt(24, 13, "Press the 'c' key to continue...");
		    ConsoleEx.CursorVisible = false;
		    while (ConsoleEx.ReadChar() != 'c')
		    {
		        /* do nothing */
		    }

		    ConsoleEx.TextColor(ConsoleForeground.Yellow, ConsoleBackground.Aquamarine);
			ConsoleEx.Clear();
			ConsoleEx.CursorHeight = 100;
			ConsoleEx.CursorVisible = true;
			Console.WriteLine("The cursor is now switched on and has been sized to 100%");
			Console.WriteLine();
			Console.Write("Please enter a new window title: ");
			ConsoleEx.Title = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine("Window title now set to: " + ConsoleEx.Title);
			ConsoleEx.CursorHeight = 25;	// small
			Console.WriteLine("Press Enter to continue...");
			Console.ReadLine();

			ConsoleEx.Clear();
			ConsoleEx.TextColor(ConsoleForeground.Black, ConsoleBackground.Red);
			ConsoleEx.DrawRectangle(BorderStyle.None, 0, 0, 39, 11, true);
			ConsoleEx.TextColor(ConsoleForeground.Black, ConsoleBackground.Green);
			ConsoleEx.DrawRectangle(BorderStyle.None, 40, 0, 39, 11, true);
			ConsoleEx.TextColor(ConsoleForeground.Black, ConsoleBackground.Blue);
			ConsoleEx.DrawRectangle(BorderStyle.None, 0, 12, 39, 11, true);
			ConsoleEx.TextColor(ConsoleForeground.Black, ConsoleBackground.Yellow);
			ConsoleEx.DrawRectangle(BorderStyle.None, 40, 12, 39, 11, true);
			ConsoleEx.TextColor(ConsoleForeground.White, ConsoleBackground.Black);
		    ConsoleEx.Move(10, 10);
			var x = ConsoleEx.CursorX;
			var y = ConsoleEx.CursorY;
			Console.Write("({0},{1})", x, y);
			ConsoleEx.Move(4, 1);
			x = ConsoleEx.CursorX;
			y = ConsoleEx.CursorY;
			Console.Write("({0},{1})", x, y);
			ConsoleEx.ReadChar();
			ConsoleEx.Clear();
		}
	}
}