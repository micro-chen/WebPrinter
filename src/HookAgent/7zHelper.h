#pragma once
#pragma comment(lib, "C:\\SpiderPackageFiles\\7zLib\\LZMA.lib")
#include "LzmaLib.h"

//LZMA —πÀı
bool Compress(const char*scrfilename, const char*desfilename);

//LZMA Ω‚—π
bool Uncompress(const char*scrfilename, const char*desfilename);
