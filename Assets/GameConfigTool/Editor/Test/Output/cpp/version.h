#ifndef CONFIG_VERSIONS
#define CONFIG_VERSIONS

#include "versions.pb.h"
#include "dungeon.pb.h"
#include "reftest.pb.h"
#include "reftestsimple.pb.h"
#include "test.pb.h"

using namespace ::google::protobuf;
namespace Config {

class Version {
    public:
        inline static differentversions Check(const versions& clientVersions);
}  // class version


/// inline
inline static differentversions version::Check(const versions& clientVersions) {
    differentversions result;
    if (dungeon::GetMD5() != clientVersions.dungeon().md5()) {
        versiontime* diff = result.add_list();
        diff->name = "dungeon";
    }
    if (reftest::GetMD5() != clientVersions.reftest().md5()) {
        versiontime* diff = result.add_list();
        diff->name = "reftest";
    }
    if (reftestsimple::GetMD5() != clientVersions.reftestsimple().md5()) {
        versiontime* diff = result.add_list();
        diff->name = "reftestsimple";
    }
    if (test::GetMD5() != clientVersions.test().md5()) {
        versiontime* diff = result.add_list();
        diff->name = "test";
    }
    return result;
}

}  // namespace Config
#endif // CONFIG_VERSIONS
