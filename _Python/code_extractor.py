#!/usr/bin/python
import os

root_dir = "../"

output_file = "DebugDoc.txt"


def override_outfile(target: str = output_file):
    with open(target, "w", encoding="utf-8") as outfile:
        outfile.write("")


# generates the "debug documentation" .txt file that contains every single script in the game
# extension is the target format (like .tres, .tscn, .cs, .gd)
def output_by_extension(ext: str, target: str = output_file):
    with open(target, "a", encoding="utf-8") as outfile:
        outfile.write("//::===== Debug Documentation. ===== \n")
        for dir_path, dir_names, file_names in os.walk(root_dir):
            for file_name in file_names:
                if file_name.endswith(f".{ext}"):
                    file_path: str = os.path.join(dir_path, file_name)
                    with open(file_path, "r", encoding="utf-8") as infile:
                        outfile.write(f"//::{file_path.split('../')[1]}\n")
                        outfile.write(infile.read())
                        outfile.write("\n---")
                        outfile.write("\n\n")
    print(f"All .{ext} files have been written to {output_file}")


if __name__ == "__main__":
    override_outfile()
    output_by_extension("cs")
    output_by_extension("tscn")
    #output_by_extension("tres")
