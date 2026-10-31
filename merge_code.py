import os

# فایل‌ها در این فایل متنی ذخیره خواهند شد
OUTPUT_FILE = 'all_code.txt'

# پوشه‌هایی که نیازی به کدهایشان نداریم
IGNORE_DIRS = ['obj','bin','venv', 'pycache', '.git', 'migrations', 'node_modules', 'images', 'fonts']

# پسوندهایی که می‌خواهیم استخراج کنیم
ALLOWED_EXTENSIONS = ['.py', '.js', '.html','cs',]

with open(OUTPUT_FILE, 'w', encoding='utf-8') as outfile:
    for root, dirs, files in os.walk('.'):
        # حذف پوشه‌های اضافی از جستجو
        dirs[:] = [d for d in dirs if d not in IGNORE_DIRS]
        
        for file in files:
            if any(file.endswith(ext) for ext in ALLOWED_EXTENSIONS):
                # فایل خود این اسکریپت را نادیده بگیر
                if file == 'merge_code.py':
                    continue
                    
                filepath = os.path.join(root, file)
                outfile.write(f"\n\n{'='*60}\n")
                outfile.write(f"📄 FILE PATH: {filepath}\n")
                outfile.write(f"{'='*60}\n\n")
                
                try:
                    with open(filepath, 'r', encoding='utf-8') as infile:
                        outfile.write(infile.read())
                except Exception as e:
                    outfile.write(f"خطا در خواندن فایل: {e}\n")

print(f"✅ انجام شد! تمام کدها در فایل {OUTPUT_FILE} ذخیره شدند.")