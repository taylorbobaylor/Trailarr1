import os
import re
import codecs

def is_block_scoped_namespace(content):
    pattern = r'^\s*namespace\s+[\w\.]+\s*{.*?}[\r\n]*$'
    return bool(re.search(pattern, content, re.MULTILINE | re.DOTALL))

def get_indentation(line):
    return len(line) - len(line.lstrip())

def convert_to_file_scoped(file_path):
    try:
        with codecs.open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()
            print(f"Processing {file_path}")

        if re.search(r'^\s*namespace\s+[\w\.]+\s*;', content, re.MULTILINE):
            print(f"Skipping {file_path} - already file-scoped")
            return False

        if not is_block_scoped_namespace(content):
            print(f"Skipping {file_path} - not block-scoped")
            return False

        namespace_match = re.search(r'^\s*namespace\s+([\w\.]+)\s*{', content, re.MULTILINE)
        if not namespace_match:
            print(f"No namespace match found in {file_path}")
            return False

        namespace_name = namespace_match.group(1)
        print(f"Found namespace: {namespace_name}")

        lines = content.splitlines()

        namespace_line = None
        brace_count = 0
        namespace_end_line = None

        for i, line in enumerate(lines):
            if namespace_match.group(0) in line:
                namespace_line = i
                brace_count = 1
                print(f"Found namespace declaration at line {i + 1}")
                continue

            if namespace_line is not None:
                brace_count += line.count('{') - line.count('}')
                if brace_count == 0:
                    namespace_end_line = i
                    print(f"Found namespace end at line {i + 1}")
                    break

        if namespace_line is None:
            print(f"Could not find namespace line in {file_path}")
            return False

        indent = get_indentation(lines[namespace_line])
        lines[namespace_line] = ' ' * indent + f"namespace {namespace_name};"

        if namespace_end_line is not None:
            while namespace_end_line > namespace_line and not lines[namespace_end_line].strip():
                namespace_end_line -= 1
            if lines[namespace_end_line].strip() == '}':
                lines.pop(namespace_end_line)

        with codecs.open(file_path, 'w', encoding='utf-8-sig') as f:
            f.write('\n'.join(lines))

        print(f"Successfully converted {file_path}")
        return True

    except Exception as e:
        print(f"Error processing {file_path}: {str(e)}")
        return False

def process_directory(directory):
    count = 0
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                if convert_to_file_scoped(file_path):
                    count += 1
    print(f"Converted {count} files")

if __name__ == '__main__':
    process_directory('.')
