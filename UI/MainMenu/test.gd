extends Node



func _ready() -> void:
	print(OS.get_user_data_dir())
	print(OS.get_executable_path())
	#TODO: Move to C# and change the game save path according to OS, if it's a desktop os. it shall be the executable path, otherwise, the "user://" thingy
	match OS.get_name():
		"Windows":
			print("Welcome to Windows!")
		"macOS":
			print("Welcome to macOS!")
		"Linux", "FreeBSD", "NetBSD", "OpenBSD", "BSD":
			print("Welcome to Linux/BSD!")
		"Android":
			print("Welcome to Android!")
		"iOS":
			print("Welcome to iOS!")
		"Web":
			print("Welcome to the Web!")
