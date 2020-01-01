#ifndef CONFIG_REFTEST
#define CONFIG_REFTEST

#include <map>
#include <iostream>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include "reftest.pb.h"

using namespace ::google::protobuf;

namespace Config {

class ReftestConfig {
    public:
        inline static std::string GetMD5();
        inline static std::string GetTimestamp();
        inline static bool Has(uint32 id);
        inline static reftest& Get(uint32 id);
        inline static void Load(const std::string& path);
        inline static void AddDynamic(reftestlist& datalist);
        inline static void AddDynamic(reftest& data);
        inline static void ClearDynamic();

    private:
        static std::map<uint32, reftest> DataTable;
        static std::map<uint32, reftest> DynamicDataTable;
        static std::string MD5;

        inline static void Add(reftest& data, std::map<uint32, reftest>& table);
        inline static void Remove(uint32 id, std::map<uint32, reftest>& table);
}  // class ReftestConfig


/// inline
inline static std::string ReftestConfig::GetMD5() {
    return MD5;
}
inline static bool ReftestConfig::Has(uint32 id) {
    std::map<uint32, reftest>::iterator itId = DataTable.find(id);
    if (itId != DataTable.end()) {
        return true
    }

    std::map<uint32, reftest>::iterator itId = DynamicDataTable.find(id);
    if (itId != DynamicDataTable.end()) {
        return true;
    }
    return false;
}
inline static const reftest& ReftestConfig::Get(uint32 id) {
    std::map<uint32, reftest>::iterator itId = DataTable.find(id);
    if (itId != DataTable.end()) {
        return itId->second;
    }

    std::map<uint32, reftest>::iterator itId = DynamicDataTable.find(id);
    if (itId != DynamicDataTable.end()) {
        return itId->second;
    }
    return reftest::default_instance();
}
inline static void ReftestConfig::Load(const std::string& path) {
    int fd = open(path.c_str(), O_RDONLY);
    if (fd < 0) {
        return;
    }
    io::FileInputStream* fs = new io::FileInputStream(fd);
    fs.SetCloseOnDelete(true);
    reftestlist datalist;
    TextFormat::Parse(fs, &datalist);
    delete fs; fs = NULL;
    
    DataTable.clear();
    for (int i = 0; i < datalist.reftests_size(); ++i)
        Add(datalist.mutable_reftests[i], DataTable);

    MD5 = datalist.md5();
}
inline static void ReftestConfig::AddDynamic(reftestlist& datalist) {
    for (int i = 0; i < datalist.reftests_size(); ++i)
        AddDynamic(datalist.mutable_reftests[i]);
}
inline static void ReftestConfig::AddDynamic(reftest& data) {
    Add(data, DynamicDataTable);
}
inline static void ReftestConfig::ClearDynamic() {
    DynamicDataTable.clear();
}
inline static void ReftestConfig::Add(reftest& data, std::map<uint32, reftest>& table) {
    table[data.id()] = data;
}
inline static void ReftestConfig::Remove(uint32 id, std::map<uint32, reftest>& table) {
    std::map<uint32, reftest>::iterator itId = table.find(id);
    if (itId != table.end()) {
        table.erase(itId);
    }
}

}  // namespace Config
#endif // CONFIG_REFTEST
