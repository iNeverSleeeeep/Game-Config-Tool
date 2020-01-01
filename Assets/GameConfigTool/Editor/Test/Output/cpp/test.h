#ifndef CONFIG_TEST
#define CONFIG_TEST

#include <map>
#include <iostream>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include "test.pb.h"

using namespace ::google::protobuf;

namespace Config {

class TestConfig {
    public:
        inline static std::string GetMD5();
        inline static std::string GetTimestamp();
        inline static bool Has(uint32 id, uint32 subid);
        inline static test& Get(uint32 id, uint32 subid);
        inline static test& Get(const testref& key);
        inline static void Load(const std::string& path);
        inline static void AddDynamic(testlist& datalist);
        inline static void AddDynamic(test& data);
        inline static void ClearDynamic();

    private:
        static std::map<uint32, std::map<uint32, test> > DataTable;
        static std::map<uint32, std::map<uint32, test> > DynamicDataTable;
        static std::string MD5;

        inline static void Add(test& data, std::map<uint32, std::map<uint32, test> >& table);
        inline static void Remove(uint32 id, uint32 subid, std::map<uint32, std::map<uint32, test> >& table);
}  // class TestConfig


/// inline
inline static std::string TestConfig::GetMD5() {
    return MD5;
}
inline static bool TestConfig::Has(uint32 id, uint32 subid) {
    std::map<uint32, std::map<uint32, test> >::iterator itID = DataTable.find(id);
    if (itID != DataTable.end()) {
        std::map<uint32, test>::iterator itSubid = itID->second.find(subid);
        if (itSubid != itID->second.end()) {
            return true
        }
    }

    std::map<uint32, std::map<uint32, test> >::iterator itID = DynamicDataTable.find(id);
    if (itID != DynamicDataTable.end()) {
        std::map<uint32, test>::iterator itSubid = itID->second.find(subid);
        if (itSubid != itID->second.end()) {
            return true;
        }
    }
    return false;
}
inline static const test& TestConfig::Get(uint32 id, uint32 subid) {
    std::map<uint32, std::map<uint32, test> >::iterator itID = DataTable.find(id);
    if (itID != DataTable.end()) {
        std::map<uint32, test>::iterator itSubid = itID->second.find(subid);
        if (itSubid != itID->second.end()) {
            return itSubid->second;
        }
    }

    std::map<uint32, std::map<uint32, test> >::iterator itID = DynamicDataTable.find(id);
    if (itID != DynamicDataTable.end()) {
        std::map<uint32, test>::iterator itSubid = itID->second.find(subid);
        if (itSubid != itID->second.end()) {
            return itSubid->second;
        }
    }
    return test::default_instance();
}
inline static const test& TestConfig::Get(const testref& key) {
    return Get(key.id(), key.subid());
}
inline static void TestConfig::Load(const std::string& path) {
    int fd = open(path.c_str(), O_RDONLY);
    if (fd < 0) {
        return;
    }
    io::FileInputStream* fs = new io::FileInputStream(fd);
    fs.SetCloseOnDelete(true);
    testlist datalist;
    TextFormat::Parse(fs, &datalist);
    delete fs; fs = NULL;
    
    DataTable.clear();
    for (int i = 0; i < datalist.tests_size(); ++i)
        Add(datalist.mutable_tests[i], DataTable);

    MD5 = datalist.md5();
}
inline static void TestConfig::AddDynamic(testlist& datalist) {
    for (int i = 0; i < datalist.tests_size(); ++i)
        AddDynamic(datalist.mutable_tests[i]);
}
inline static void TestConfig::AddDynamic(test& data) {
    Add(data, DynamicDataTable);
}
inline static void TestConfig::ClearDynamic() {
    DynamicDataTable.clear();
}
inline static void TestConfig::Add(test& data, std::map<uint32, std::map<uint32, test> >& table) {
    table[data.id()][data.subid()] = data;
}
inline static void TestConfig::Remove(uint32 id, uint32 subid, std::map<uint32, std::map<uint32, test> >& table) {
    std::map<uint32, std::map<uint32, test> >::iterator itID = table.find(id);
    if (itID != table.end()) {
        std::map<uint32, test>::iterator itSubid = itID->second.find(subid);
        if (itSubid != itID->second.end()) {
            itID->second.erase(itSubid);
        }
    }
}

}  // namespace Config
#endif // CONFIG_TEST
