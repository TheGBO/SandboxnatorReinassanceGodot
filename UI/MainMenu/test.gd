extends Node



func _ready() -> void:
	print(OS.get_user_data_dir())
	print(OS.get_executable_path())
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
