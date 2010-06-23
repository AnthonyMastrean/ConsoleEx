// Enumerations used for setting console window colors.
using System;

namespace Microsoft.GotDotNet
{
	/// <summary>
	/// Color definitions for the console foreground.
	/// </summary>
	public enum ConsoleForeground
	{	
		Black      = 0x00,
		Navy       = 0x01,
		DarkGreen  = 0x02,
		Aquamarine = 0x03,
		Maroon     = 0x04,
		Purple     = 0x05,
		Olive      = 0x06,
		LightGray  = 0x07,
		DarkGray   = 0x08,
		Blue       = 0x09,
		Green      = 0x0A,
		Cyan       = 0x0B,
		Red        = 0x0C,
		Magenta    = 0x0D,
		Yellow     = 0x0E,
		White      = 0x0F
	}


	/// <summary>
	/// Color definitions for the console background.
	/// </summary>
	public enum ConsoleBackground
	{	
		Black      = 0x00,
		Navy       = 0x10,
		DarkGreen  = 0x20,
		Aquamarine = 0x30,
		Maroon     = 0x40,
		Purple     = 0x50,
		Olive      = 0x60,
		LightGray  = 0x70,
		DarkGray   = 0x80,
		Blue       = 0x90,
		Green      = 0xA0,
		Cyan       = 0xB0,
		Red        = 0xC0,
		Magenta    = 0xD0,
		Yellow     = 0xE0,
		White      = 0xF0
	}


	/// <summary>
	/// Border styles used for drawing rectangles on the console output window.
	/// </summary>
	public enum BorderStyle
	{
		None,
		Text,
		LineSingle,
		LineDouble
	}


	[Flags]
	public enum InputModeFlags
	{
		ENABLE_PROCESSED_INPUT = 0x01,
		ENABLE_LINE_INPUT      = 0x02,
		ENABLE_ECHO_INPUT      = 0x04,
		ENABLE_WINDOW_INPUT    = 0x08,
		ENABLE_MOUSE_INPUT     = 0x10
	}

	[Flags]
	public enum OutputModeFlags
	{
		ENABLE_PROCESSED_OUTPUT   = 0x01,
		ENABLE_WRAP_AT_EOL_OUTPUT = 0x02
	}
}