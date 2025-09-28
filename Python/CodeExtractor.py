#generates the "debug documentation" .txt file that contains every single script in the game
import os

root_dir = "../"

output_file = "Docs/AllScripts.txt"

with open(output_file, "w", encoding="utf-8") as outfile:
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for filename in filenames:
            if filename.endswith(".cs"): 
                file_path = os.path.join(dirpath, filename)
                with open(file_path, "r", encoding="utf-8") as infile:
                    outfile.write(f"// ===== {file_path} =====\n\n")
                    outfile.write(infile.read())
                    outfile.write("\n\n")

print(f"All .cs files have been written to {output_file}")
