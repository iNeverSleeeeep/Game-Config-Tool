#ifndef CONFIG_DUNGEON
#define CONFIG_DUNGEON

#include <map>
#include <iostream>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include "dungeon.pb.h"

using namespace ::google::protobuf;

namespace Config {

class DungeonConfig {
    public:
        inline static std::string GetMD5();
        inline static std::string GetTimestamp();
        inline static bool Has(EDungeonType type, uint32 id);
        inline static dungeon& Get(EDungeonType type, uint32 id);
        inline static dungeon& Get(const dungeonref& key);
        inline static void Load(const std::string& path);
        inline static void AddDynamic(dungeonlist& datalist);
        inline static void AddDynamic(dungeon& data);
        inline static void ClearDynamic();

    private:
        static std::map<EDungeonType, std::map<uint32, dungeon> > DataTable;
        static std::map<EDungeonType, std::map<uint32, dungeon> > DynamicDataTable;
        static std::string MD5;

        inline static void Add(dungeon& data, std::map<EDungeonType, std::map<uint32, dungeon> >& table);
        inline static void Remove(EDungeonType type, uint32 id, std::map<EDungeonType, std::map<uint32, dungeon> >& table);
}  // class DungeonConfig


/// inline
inline static std::string DungeonConfig::GetMD5() {
    return MD5;
}
inline static bool DungeonConfig::Has(EDungeonType type, uint32 id) {
    std::map<EDungeonType, std::map<uint32, dungeon> >::iterator itType = DataTable.find(type);
    if (itType != DataTable.end()) {
        std::map<uint32, dungeon>::iterator itId = itType->second.find(id);
        if (itId != itType->second.end()) {
            return true
        }
    }

    std::map<EDungeonType, std::map<uint32, dungeon> >::iterator itType = DynamicDataTable.find(type);
    if (itType != DynamicDataTable.end()) {
        std::map<uint32, dungeon>::iterator itId = itType->second.find(id);
        if (itId != itType->second.end()) {
            return true;
        }
    }
    return false;
}
inline static const dungeon& DungeonConfig::Get(EDungeonType type, uint32 id) {
    std::map<EDungeonType, std::map<uint32, dungeon> >::iterator itType = DataTable.find(type);
    if (itType != DataTable.end()) {
        std::map<uint32, dungeon>::iterator itId = itType->second.find(id);
        if (itId != itType->second.end()) {
            return itId->second;
        }
    }

    std::map<EDungeonType, std::map<uint32, dungeon> >::iterator itType = DynamicDataTable.find(type);
    if (itType != DynamicDataTable.end()) {
        std::map<uint32, dungeon>::iterator itId = itType->second.find(id);
        if (itId != itType->second.end()) {
            return itId->second;
        }
    }
    return dungeon::default_instance();
}
inline static const dungeon& DungeonConfig::Get(const dungeonref& key) {
    return Get(key.type(), key.id());
}
inline static void DungeonConfig::Load(const std::string& path) {
    int fd = open(path.c_str(), O_RDONLY);
    if (fd < 0) {
        return;
    }
    io::FileInputStream* fs = new io::FileInputStream(fd);
    fs.SetCloseOnDelete(true);
    dungeonlist datalist;
    TextFormat::Parse(fs, &datalist);
    delete fs; fs = NULL;
    
    DataTable.clear();
    for (int i = 0; i < datalist.dungeons_size(); ++i)
        Add(datalist.mutable_dungeons[i], DataTable);

    MD5 = datalist.md5();
}
inline static void DungeonConfig::AddDynamic(dungeonlist& datalist) {
    for (int i = 0; i < datalist.dungeons_size(); ++i)
        AddDynamic(datalist.mutable_dungeons[i]);
}
inline static void DungeonConfig::AddDynamic(dungeon& data) {
    Add(data, DynamicDataTable);
}
inline static void DungeonConfig::ClearDynamic() {
    DynamicDataTable.clear();
}
inline static void DungeonConfig::Add(dungeon& data, std::map<EDungeonType, std::map<uint32, dungeon> >& table) {
    table[data.type()][data.id()] = data;
}
inline static void DungeonConfig::Remove(EDungeonType type, uint32 id, std::map<EDungeonType, std::map<uint32, dungeon> >& table) {
    std::map<EDungeonType, std::map<uint32, dungeon> >::iterator itType = table.find(type);
    if (itType != table.end()) {
        std::map<uint32, dungeon>::iterator itId = itType->second.find(id);
        if (itId != itType->second.end()) {
            itType->second.erase(itId);
        }
    }
}

}  // namespace Config
#endif // CONFIG_DUNGEON
