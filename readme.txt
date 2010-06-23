ConsoleEx 1.03
~~~~~~~~~~~~~~

Written by Tim Sneath <tims@microsoft.com>

Visual Studio .NET allows you to write console applications, but the built-in class provides limited functionality for controlling the screen and cursor. For more sophisticated access, it is necessary to drop down to the system API calls provided in kernel32.dll.

This class is a helper class that provides supplemental functionality to that available in the System.Console class. It allows a developer to:
 - Move the text cursor location
 - Change the cursor size
 - Set the cursor to be invisible
 - Set foreground and background colors for the console
 - Clear the screen
 - Change the console window title
 - Draw rectangles and bounding boxes on the screen
 - Read input on a character-by-character basis

The class does not replace traditional functions supplied in the System.Console class such as WriteLine() and ReadLine(), but is intended to be use in conjunction with calls to that class.

This class is largely built using information from the Platform SDK. Documentation on the Console APIs within kernel32 can be found at the following location:
   http://msdn.microsoft.com/library/en-us/dllproc/base/character_mode_applications.asp
	
Several of the functions within this class are reworked and expanded versions of those found in KnowledgeBase articles Q319883 and Q319257.

I'm interested in your comments and suggestions for improvement. Please email me at the above address. 

Change Log
==========
v1.03 - Set and obtain current cursor location
      - Minor bug fix

v1.02 - Added GetChar() input function
      - Improved remote console support
      - Minor bug fixes

To Do
=====
 - Improve support for buffered console windows
 - Add additional drawing functions
 - Performance testing / profiling
