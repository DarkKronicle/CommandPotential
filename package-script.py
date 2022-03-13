import subprocess
import zipfile
import inquirer
from tempfile import mkstemp
import shutil
import os
import re

# https://stackoverflow.com/questions/39086/search-and-replace-a-line-in-a-file-in-python
def replace(file_path, pattern, subst):

    pattern = re.compile(pattern)   

    # Create temp file
    fh, abs_path = mkstemp()

    with os.fdopen(fh, 'w') as new_file:
        with open(file_path) as old_file:
            for line in old_file:
                new_file.write(re.sub(pattern, subst, line))

    # Copy the file permissions from the old file to the new file
    shutil.copymode(file_path, abs_path)

    # Remove original file
    os.remove(file_path)

    # Move new file
    shutil.move(abs_path, file_path)


def main():

    subprocess.run(['dotnet', 'build', 'RoR2Mods.sln'])

    q = [
        inquirer.Text('version', "What is the version?"),
        inquirer.Text('changelog', "What changed?")
    ]
    answers = inquirer.prompt(q)
    version = answers['version']
    changelog = answers['changelog']

    replace("manifest.json", r'"version_number": ".+",', '"version_number": "{}",'.format(version))
    replace("CommandPotential/CommandPotential.cs", r'public const string PluginVersion = ".+";', 'public const string PluginVersion = "{}";'.format(version))
    if changelog is not None and changelog != '':
        replace("README.md", "## Changelog", "## Changelog\n\n**{0}**\n\n{1}".format(version, changelog))
    shutil.copy("manifest.json", "package/manifest.json")
    shutil.copy("README.md", "package/README.md")
    shutil.copy("CommandPotential/bin/Debug/netstandard2.0/CommandPotential.dll", "package/CommandPotential.dll")

    compressed = ['README.md', 'CommandPotential.dll', 'icon.png', 'manifest.json']

    zipf = zipfile.ZipFile('package/build-{0}.zip'.format(version), 'w', zipfile.ZIP_DEFLATED)
    for f in compressed:
        zipf.write('package/' + f, f)
    zipf.close()
    print('Done!')


if __name__ == '__main__':
    main();
