#ifndef CONFIG_REFTESTSIMPLE
#define CONFIG_REFTESTSIMPLE

#include <map>
#include <iostream>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include "reftestsimple.pb.h"

using namespace ::google::protobuf;

namespace Config {

class ReftestsimpleConfig {
    public:
        inline static std::string GetMD5();
        inline static std::string GetTimestamp();
        inline static bool Has(uint32 id);
        inline static reftestsimple& Get(uint32 id);
        inline static void Load(const std::string& path);
        inline static void AddDynamic(reftestsimplelist& datalist);
        inline static void AddDynamic(reftestsimple& data);
        inline static void ClearDynamic();

    private:
        static std::map<uint32, reftestsimple> DataTable;
        static std::map<uint32, reftestsimple> DynamicDataTable;
        static std::string MD5;

        inline static void Add(reftestsimple& data, std::map<uint32, reftestsimple>& table);
        inline static void Remove(uint32 id, std::map<uint32, reftestsimple>& table);
}  // class ReftestsimpleConfig


/// inline
inline static std::string ReftestsimpleConfig::GetMD5() {
    return MD5;
}
inline static bool ReftestsimpleConfig::Has(uint32 id) {
    std::map<uint32, reftestsimple>::iterator itId = DataTable.find(id);
    if (itId != DataTable.end()) {
        return true
    }

    std::map<uint32, reftestsimple>::iterator itId = DynamicDataTable.find(id);
    if (itId != DynamicDataTable.end()) {
        return true;
    }
    return false;
}
inline static const reftestsimple& ReftestsimpleConfig::Get(uint32 id) {
    std::map<uint32, reftestsimple>::iterator itId = DataTable.find(id);
    if (itId != DataTable.end()) {
        return itId->second;
    }

    std::map<uint32, reftestsimple>::iterator itId = DynamicDataTable.find(id);
    if (itId != DynamicDataTable.end()) {
        return itId->second;
    }
    return reftestsimple::default_instance();
}
inline static void ReftestsimpleConfig::Load(const std::string& path) {
    int fd = open(path.c_str(), O_RDONLY);
    if (fd < 0) {
        return;
    }
    io::FileInputStream* fs = new io::FileInputStream(fd);
    fs.SetCloseOnDelete(true);
    reftestsimplelist datalist;
    TextFormat::Parse(fs, &datalist);
    delete fs; fs = NULL;
    
    DataTable.clear();
    for (int i = 0; i < datalist.reftestsimples_size(); ++i)
        Add(datalist.mutable_reftestsimples[i], DataTable);

    MD5 = datalist.md5();
}
inline static void ReftestsimpleConfig::AddDynamic(reftestsimplelist& datalist) {
    for (int i = 0; i < datalist.reftestsimples_size(); ++i)
        AddDynamic(datalist.mutable_reftestsimples[i]);
}
inline static void ReftestsimpleConfig::AddDynamic(reftestsimple& data) {
    Add(data, DynamicDataTable);
}
inline static void ReftestsimpleConfig::ClearDynamic() {
    DynamicDataTable.clear();
}
inline static void ReftestsimpleConfig::Add(reftestsimple& data, std::map<uint32, reftestsimple>& table) {
    table[data.id()] = data;
}
inline static void ReftestsimpleConfig::Remove(uint32 id, std::map<uint32, reftestsimple>& table) {
    std::map<uint32, reftestsimple>::iterator itId = table.find(id);
    if (itId != table.end()) {
        table.erase(itId);
    }
}

}  // namespace Config
#endif // CONFIG_REFTESTSIMPLE
