// ConsoleEx V1.03 - Tim Sneath <tims@microsoft.com>

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.GotDotNet
{
	/// <summary>
	/// This class provides supplemental functionality to that available in the System.Console class. It 
	/// allows a developer to control the cursor location, size and visibility, to manipulate the color
	/// used for writing text on the screen, to read characters individually from the input buffer, to 
	/// manipulate the console window title, to clear the screen and to draw rectangles on the screen. It 
	/// does not replace traditional functions supplied in the System.Console class such as WriteLine() 
	/// and ReadLine(), but is intended to be use in conjunction with calls to that class.
	/// </summary>
	/// <remarks>
	/// This class is largely built using information from the Platform SDK. Documentation on the 
	/// Console APIs within kernel32 can be found at the following location:
	///       http://msdn.microsoft.com/library/en-us/dllproc/base/character_mode_applications.asp
	///
	/// Several of the functions within this class are reworked and expanded versions of those
	/// found in KnowledgeBase articles Q319883 and Q319257.
	/// </remarks>
	public class ConsoleEx
	{
		#region API and Structure Declarations

		// Standard structures used for interop with kernel32
		[StructLayout(LayoutKind.Sequential)]
			struct COORD
		{
			public short x;
			public short y;
		}

		[StructLayout(LayoutKind.Sequential)]
			struct SMALL_RECT
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}
		
		[StructLayout(LayoutKind.Sequential)]
			struct CONSOLE_SCREEN_BUFFER_INFO
		{
			public COORD dwSize;
			public COORD dwCursorPosition;
			public int wAttributes;
			public SMALL_RECT srWindow;
			public COORD dwMaximumWindowSize;
		}

		[StructLayout(LayoutKind.Sequential)]
			struct CONSOLE_CURSOR_INFO 
		{
			public int dwSize;  
			public bool bVisible;
		} 


		// External function declarations
		[DllImport("kernel32.dll", EntryPoint="GetStdHandle", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", EntryPoint="GetConsoleScreenBufferInfo", SetLastError=true, 
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput,
			ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

		[DllImport("kernel32.dll", EntryPoint="SetConsoleTextAttribute", SetLastError=true, 
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int SetConsoleTextAttribute(int hConsoleOutput,
			int wAttributes);

		[DllImport("kernel32.dll", EntryPoint="FillConsoleOutputCharacter", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int FillConsoleOutputCharacter(int hConsoleOutput, 
			byte cCharacter, int nLength, COORD dwWriteCoord, ref int lpNumberOfCharsWritten);

		[DllImport("kernel32.dll", EntryPoint="FillConsoleOutputAttribute", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int FillConsoleOutputAttribute(int hConsoleOutput,
			int wAttribute, int nLength, COORD dwWriteCoord, ref int lpNumberOfAttrsWritten);

		[DllImport("kernel32.dll", EntryPoint="SetConsoleCursorPosition", SetLastError=true, 
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int SetConsoleCursorPosition(int hConsoleOutput, 
			COORD dwCursorPosition);

		[DllImport("kernel32.dll", EntryPoint="SetConsoleTitle", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool SetConsoleTitle(string lpConsoleTitle);

		[DllImport("kernel32.dll", EntryPoint="GetConsoleTitle", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int GetConsoleTitle(StringBuilder lpConsoleTitle, 
			int nSize);

		[DllImport("kernel32.dll", EntryPoint="SetConsoleCursorInfo", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int SetConsoleCursorInfo(int hConsoleOutput, 
			ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);

		[DllImport("kernel32.dll", EntryPoint="GetConsoleCursorInfo", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int GetConsoleCursorInfo(int hConsoleOutput, 
			ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo);
		
		[DllImport("kernel32.dll", EntryPoint="ReadConsole", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool ReadConsole(int hConsoleInput,
			StringBuilder buf, int nNumberOfCharsToRead, ref int lpNumberOfCharsRead, int lpReserved);

		[DllImport("kernel32.dll", EntryPoint="SetConsoleMode", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool SetConsoleMode(int hConsoleHandle,
			int dwMode);

		[DllImport("kernel32.dll", EntryPoint="GetConsoleMode", SetLastError=true,
			 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool GetConsoleMode(int hConsoleHandle,
			ref int dwMode);

		#endregion

		// Const variables
		private const int  INVALID_HANDLE_VALUE    = -1;
		private const int  STD_INPUT_HANDLE        = -10;
		private const int  STD_OUTPUT_HANDLE       = -11;
		private const byte EMPTY                   = 32;
		private const int  TITLE_LENGTH            = 1024;

		// Internal variables
		private static int hConsoleOutput;	// handle to output buffer
		private static int hConsoleInput;	// handle to input buffer
		private static COORD ConsoleOutputLocation;
		private static CONSOLE_SCREEN_BUFFER_INFO ConsoleInfo;
		private static int OriginalConsolePen;
		private static int CurrentConsolePen;

		static ConsoleEx()
		{
			// Grab input and output buffer handles
			hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
			hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
			if (hConsoleOutput == INVALID_HANDLE_VALUE || hConsoleInput == INVALID_HANDLE_VALUE)
				throw new ApplicationException("Unable to obtain buffer handle during initialization.");

			// Get information about the console window characteristics.
			ConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
			ConsoleOutputLocation = new COORD();
			GetConsoleScreenBufferInfo(hConsoleOutput, ref ConsoleInfo);
			OriginalConsolePen = ConsoleInfo.wAttributes;

			// Disable wrapping at the end of a line (ENABLE_WRAP_AT_EOL_INPUT); this enables rectangles 
			// to be drawn that fill the screen without the window scrolling.
			SetConsoleMode(hConsoleOutput, 
				(int) OutputModeFlags.ENABLE_PROCESSED_OUTPUT);
		}

		private ConsoleEx()
		{
			throw new NotSupportedException("This object may not be instantiated. Use static methods instead.");
		}

		/// <summary>
		/// Sets or gets the console window title.
		/// </summary>
		public static string Title 
		{
			get
			{
				StringBuilder title = new StringBuilder(TITLE_LENGTH);
				int ret = GetConsoleTitle(title, TITLE_LENGTH);
				return title.ToString(0, ret);
			}
			set 
			{	
				if (value.Length < TITLE_LENGTH)
					SetConsoleTitle(value);
				else
					throw new ArgumentOutOfRangeException(
						"Title", 
						value, 
						"Console window title must be no more than " + TITLE_LENGTH +  " characters.");
			}
		}

		/// <summary>
		/// Gets or sets the X co-ordinate of the cursor.
		/// </summary>
		public static short CursorX
		{
			get
			{
				CONSOLE_SCREEN_BUFFER_INFO strConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
				GetConsoleScreenBufferInfo(hConsoleOutput, ref strConsoleInfo);
			
				return strConsoleInfo.dwCursorPosition.x;
			}
			set
			{
				// Range validation is done by Move()
				Move(value, CursorY);
			}
		}

		/// <summary>
		/// Gets or sets the Y co-ordinate of the cursor.
		/// </summary>
		public static short CursorY
		{
			get
			{
				CONSOLE_SCREEN_BUFFER_INFO strConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
				GetConsoleScreenBufferInfo(hConsoleOutput, ref strConsoleInfo);
			
				return strConsoleInfo.dwCursorPosition.y;
			}
			set
			{
				// Range validation is done by Move()
				Move(CursorX, value);
			}
		}

		/// <summary>
		/// Gets or sets the visibility of the cursor.
		/// </summary>
		public static bool CursorVisible
		{
			get
			{
				CONSOLE_CURSOR_INFO ConsoleCursorInfo = new CONSOLE_CURSOR_INFO();
				GetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);
				return ConsoleCursorInfo.bVisible;
			}
			set
			{
				// Obtain current cursor information, since the CONSOLE_CURSOR_INFO struct
				// also contains information on the shape of the cursor.
				CONSOLE_CURSOR_INFO ConsoleCursorInfo = new CONSOLE_CURSOR_INFO();
				GetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);

				ConsoleCursorInfo.bVisible = value;
				SetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);
			}
		}

		/// <summary>
		/// Gets or sets the height of the cursor, as a percentage of the overall character cell.
		/// The value must be a number between 1 and 100, otherwise an ArgumentOutOfRangeException
		/// will be thrown.
		/// </summary>
		public static int CursorHeight
		{
			get
			{
				CONSOLE_CURSOR_INFO ConsoleCursorInfo = new CONSOLE_CURSOR_INFO();
				GetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);
				return ConsoleCursorInfo.dwSize;
			}
			set
			{
				if (value >= 1 && value <= 100)
				{
					// Obtain current cursor information, since the CONSOLE_CURSOR_INFO struct
					// also contains information on the visibility of the cursor.
					CONSOLE_CURSOR_INFO ConsoleCursorInfo = new CONSOLE_CURSOR_INFO();
					GetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);

					ConsoleCursorInfo.dwSize = value;
					SetConsoleCursorInfo(hConsoleOutput, ref ConsoleCursorInfo);
				}
				else
					throw new ArgumentOutOfRangeException(
						"CursorHeight", 
						value, 
						"Cursor height must be a percentage of the character cell between 1 and 100.");
			}
		}

		/// <summary>
		/// Sets the console pen color to that specified.
		/// </summary>
		/// <param name="foreground">A foreground color specified from the 
		/// ConsoleForeground enumeration</param>
		/// <param name="background">A background color specified from the 
		/// ConsoleBackground enumeration</param>
		public static void TextColor(ConsoleForeground foreground, ConsoleBackground background)
		{
			CurrentConsolePen = (int)foreground + (int)background;
			SetConsoleTextAttribute(hConsoleOutput, CurrentConsolePen);
		}
	
		/// <summary>
		/// Resets the console pen color to the original default at the time 
		/// the class was originally initialised.
		/// </summary>
		public static void ResetColor()
		{
			SetConsoleTextAttribute(hConsoleOutput, OriginalConsolePen);
		}

		/// <summary>
		/// Clears screen.
		/// </summary>
		public static void Clear()
		{
			int hWrittenChars = 0;
			CONSOLE_SCREEN_BUFFER_INFO strConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();			
			COORD Home;		
			Home.x = Home.y = 0;
			
			if (GetConsoleScreenBufferInfo(hConsoleOutput, ref strConsoleInfo) == 0)
			{
				// If the device does not support GetConsoleScreenBufferInfo, then try just
				// writing ^L (ASCII control code for Form Feed) to the screen. This may 
				// work for some scenarios such as using telnet to access a remote console.
				Console.Write('\x0c');	// ^L
				return;
			}
			
			FillConsoleOutputCharacter(hConsoleOutput, EMPTY, 
				strConsoleInfo.dwSize.x * strConsoleInfo.dwSize.y, Home, ref hWrittenChars);
			FillConsoleOutputAttribute(hConsoleOutput, CurrentConsolePen, 
				strConsoleInfo.dwSize.x * strConsoleInfo.dwSize.y, Home, ref hWrittenChars);
			
			SetConsoleCursorPosition(hConsoleOutput, Home);
		}

		/// <summary>
		/// Moves the console cursor to the specified location on the screen.
		/// </summary>
		/// <param name="x">X co-ordinate for desired location (typically 0 to 79)</param>
		/// <param name="y">Y co-ordinate for desired location (typically 0 to 24)</param>
		public static void Move(int x, int y)
		{
			// Check that parameters specified are sane
			CONSOLE_SCREEN_BUFFER_INFO strConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
			GetConsoleScreenBufferInfo(hConsoleOutput, ref strConsoleInfo);

			if (x >= strConsoleInfo.dwSize.x ||  x < 0)
				throw new ArgumentOutOfRangeException("x", x, 
					"The co-ordinates specified must be within the dimensions of the window.");

			if (y >= strConsoleInfo.dwSize.y || y < 0)
				throw new ArgumentOutOfRangeException("y", y, 
					"The co-ordinates specified must be within the dimensions of the window.");

			COORD cursorLocation;
			cursorLocation.x = (short)x;
			cursorLocation.y = (short)y;

			SetConsoleCursorPosition(hConsoleOutput, cursorLocation);
		}

		/// <summary>
		/// Writes the specified text at the given location.
		/// </summary>
		/// <remarks>
		/// This is a wrapper function that provides a shorthand for moving to a location and 
		/// writing there. The cursor will be left in the position immediately succeeding the 
		/// rightmost character of the text written to the screen.
		/// </remarks>
		/// <param name="x">X co-ordinate for leftmost character of text</param>
		/// <param name="y">Y co-ordinate for location of text</param>
		/// <param name="text">String to be written to the screen</param>
		public static void WriteAt(int x, int y, string text)
		{
			// No need to validate x and y co-ordinates as they will be tested by Move()
			Move(x, y);
			Console.Write(text);
		}

		/// <summary>
		/// Draws a rectangle on the console window using either 7-bit ASCII characters or
		/// line drawing characters, depending on the style specified. If the dimensions of 
		/// the rectangle exceed the boundaries of the screen, an ArgumentOutOfRangeException
		/// will be thrown.
		/// </summary>
		/// <remarks>
		/// Note that the sides of the rectangle themselves are one character wide,
		/// so, for example, a width and height of 2 will result in a 3-by-3 character rectangle.
		/// This matches the behavior of the equivalent Windows Forms DrawRectangle() method.
		/// </remarks>
		/// <param name="style">A border style specified from the BorderStyle enumeration</param>
		/// <param name="x">X co-ordinate of upper left corner of rectangle</param>
		/// <param name="y">Y co-ordinate of upper left corner of rectangle</param>
		/// <param name="cx">Width of the rectangle</param>
		/// <param name="cy">Height of the rectangle</param>
		public static void DrawRectangle(BorderStyle style, int x, int y, int cx, int cy, bool fill)
		{
			if (style != BorderStyle.None)
			{
				// Set rectangle frame appropriately for the style chosen. Unicode 
				// characters represent horizontal and vertical lines, then NW, NE,
				// SW and SE corners of the rectangle in that order.
				string frame;
				switch(style)
				{
					case BorderStyle.LineSingle:
						frame = "\u2500\u2502\u250c\u2510\u2514\u2518";
						break;
					case BorderStyle.LineDouble:
						frame = "\u2550\u2551\u2554\u2557\u255A\u255D";
						break;
					default:
						frame = @"-|/\\/";
						break;
				}

				// Create top line of box
				StringBuilder line = new StringBuilder(cx+1);
				line.Append(frame[2]);
				for (int i=1; i < cx; i++) line.Append(frame[0]);
				line.Append(frame[3]);
				Move(x, y);
				Console.Write(line);

				// Create sides of box
				for (int i=1; i < cy; i++)
				{
					Move(x, y+i);
					Console.Write(frame[1]);
					Move(x+cx, y+i);
					Console.Write(frame[1]);
				}

				// Create bottom line of box
				line[0] = frame[4];
				line[cx] = frame[5];
				Move(x, y+cy);
				Console.Write(line);
			}
		
			// Fill rectangle with current console pen
			if (fill)
			{
				int hWrittenChars = 0;
				COORD c = new COORD();
				c.x = (short)x;
			
				for (int i=0; i<=cy; i++)
				{
					c.y = (short)(y + i);
					FillConsoleOutputAttribute(hConsoleOutput, CurrentConsolePen, 
						cx + 1, c, ref hWrittenChars);
				}
			}
		}

		/// <summary>
		/// Read a single character from the input buffer. Unlike Console.Read(), which 
		/// only reads from the buffer when the read operation has terminated (e.g. by
		/// pressing Enter), this method reads as soon as the character has been entered.
		/// </summary>
		/// <returns>The character read by the system</returns>
		public static char ReadChar()
		{
			// Temporarily disable character echo (ENABLE_ECHO_INPUT) and line input
			// (ENABLE_LINE_INPUT) during this operation
			SetConsoleMode(hConsoleInput, 
				(int) (InputModeFlags.ENABLE_PROCESSED_INPUT | 
				InputModeFlags.ENABLE_WINDOW_INPUT | 
				InputModeFlags.ENABLE_MOUSE_INPUT));

			int lpNumberOfCharsRead = 0;
			StringBuilder buf = new StringBuilder(1);

			bool success = ReadConsole(hConsoleInput, buf, 1, ref lpNumberOfCharsRead, 0);
			
			// Reenable character echo and line input
			SetConsoleMode(hConsoleInput, 
				(int) (InputModeFlags.ENABLE_PROCESSED_INPUT | 
				InputModeFlags.ENABLE_ECHO_INPUT |
				InputModeFlags.ENABLE_LINE_INPUT |
				InputModeFlags.ENABLE_WINDOW_INPUT | 
				InputModeFlags.ENABLE_MOUSE_INPUT));
			
			if (success)
				return Convert.ToChar(buf[0]);
		
			throw new ApplicationException("Attempt to call ReadConsole API failed.");
		}
	}
}