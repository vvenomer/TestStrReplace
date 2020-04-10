#include <iostream>
#include <string>
#include <io.h>
#include <fcntl.h>
#include <sstream>
#include <fstream>
#include <codecvt>
#include <vector>
#include <chrono> 
#include "TestCpp.h"
using namespace std;
using namespace chrono;

vector<wstring> split(wistringstream& s, wchar_t delimiter)
{
    vector<wstring> tokens;
    wstring token;
    while (getline(s, token, delimiter))
    {
        tokens.push_back(token);
    }
    return tokens;
}
vector<wstring> split(const wstring& s, wchar_t delimiter)
{
    wistringstream tokenStream(s);
    return split(tokenStream, delimiter);
}

wstring ReplaceString(wstring subject, const wstring& search, const wstring& replace) {
    size_t pos = 0;
    wstring toChange(subject);
    while ((pos = toChange.find(search, pos)) != wstring::npos) {
        toChange.replace(pos, search.length(), replace);
        pos += replace.length();
    }
    return toChange;
}

std::wstring readFile(const char* filename)
{
    wifstream wif(filename, ios_base::binary);
    wif.imbue(locale(wif.getloc(), new codecvt_utf8<wchar_t>));
    wstringstream wss;
    wss << wif.rdbuf();
    return wss.str();
}

int main()
{
	_setmode(_fileno(stdout), _O_U8TEXT);

    wstring encodedData = readFile("C:/Users/pblajer/source/repos/JSiZ/GenerateData/data.txt");
    wstring subject;
    wistringstream tokenStream(encodedData);
    //get first line (subject)
    getline(tokenStream, subject, L'\n');
    //get rest of lines (replace 1 and replace 2)
    vector<wstring> seperatedLines = split(tokenStream, L'\n');
    vector<vector<wstring>> seperatedData;
    //split replaces
    for (auto& line : seperatedLines)
    {
        seperatedData.push_back(split(line, L','));
    }
    //test
    int i = 0;
    auto start = high_resolution_clock::now();
    for (auto& data : seperatedData)
    {
        wstring result = ReplaceString(subject, data[0],data[1]);
        if (result != subject)
            i++;
    }
    auto stop = high_resolution_clock::now();
    auto duration = duration_cast<milliseconds>(stop - start);
    wcout << "times replaced: " << i << endl;
    wcout << "time: " << duration.count() << " [milliseconds]"<< endl;
    
}