import os
import re

def cleanup_usings(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Extract namespace
    namespace_match = re.search(r'namespace\s+([^;{\s]+)', content)
    if not namespace_match:
        return content

    namespace = namespace_match.group(1)

    # Essential using directives that should never be removed
    essential_usings = {
        'Microsoft.Extensions.DependencyInjection',
        'Microsoft.Extensions.Configuration',
        'Microsoft.Extensions.Hosting',
        'Microsoft.AspNetCore.Builder',
        'System',
        'System.Threading.Tasks',
        'System.Collections.Generic',
        'System.Linq',
        'TMDbLib.Client',
        'Trailarr.Core.Configuration',
        'Trailarr.Core.Services',
        'Trailarr.Core.Models',
        'Microsoft.Extensions.Logging',
        'Microsoft.Extensions.Options',
        'System.IO',
        'System.Text.Json',
        'System.Threading',
        'YoutubeExplode',
        'YoutubeExplode.Videos.Streams'
    }

    # Remove using directives for the current namespace and keep essential ones
    lines = content.split('\n')
    cleaned_lines = []

    for line in lines:
        if line.strip().startswith('using'):
            # Keep essential using directives
            using_match = re.match(r'using\s+([^;]+);', line)
            if using_match:
                using_namespace = using_match.group(1)
                if using_namespace in essential_usings or not using_namespace.startswith(namespace):
                    cleaned_lines.append(line)
        else:
            cleaned_lines.append(line)

    return '\n'.join(cleaned_lines)

def process_directory(directory):
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                try:
                    cleaned_content = cleanup_usings(file_path)
                    with open(file_path, 'w', encoding='utf-8') as f:
                        f.write(cleaned_content)
                except Exception as e:
                    print(f"Error processing {file_path}: {e}")

if __name__ == '__main__':
    src_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'backend', 'Trailarr.Core'))
    process_directory(src_dir)
    print("Finished cleaning up using directives")
