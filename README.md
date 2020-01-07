# Game-Config-Tool
基于Unity和NPOI的excel配置档导出工具

### 特性
- xlsx格式配置档。
- 多线程导出，导表速度极快。
- 导出lua table为客户端配置档。
- 使用protobuf格式为服务器配置档，目前只支持c++服务器，生成了加载代码。
- 支持分excel功能，例如*skill.xlsx* *skill(1-100).xlsx* *skill(101-200).xlsx* 会被导出到同一个配置档中。
- 支持lua table分片延迟加载功能，并且这种分片对用户透明，例如一个较大的*skill.lua* 可以根据配置拆分为*skill.lua* *skill0.lua* *skill1.lua* ...
- 支持各个excel使用通用的数据结构，同样的数据结构方便代码公用。
- 支持excel间引用功能，并且在导出时进行关联检查，例如drop.xlsx(掉落表)中可以对于item.xlsx(道具表)进行引用，如果道具id不存在则不能正确导出。
- 支持keyword功能，可以使用中文来指定特殊含义的数字，避免配置档中填写过多的magic number。
- 对导出lua table进行了优化，使用的内存为通常的1/3。
- 支持全部protobuf格式，支持数组导出，客户端支持json格式的导出。
- 理论上支持多平台（目前使用的protoc.exe只能在windows平台使用）。
- 导出了每个配置档的md5文件，方便版本校验。
- 只有导出的文件有差异时，才会真正进行写文件。
- 支持Unity 2017.4
- 导出时不需要关闭excel

### 未来计划
- 配置档填错时更好的错误提示
- 更好的lua table压缩版本，占用更少的内存（通过键的重排序）
- 在Unity中的可视化编辑界面
- 增加Unity资源类型引用，可以在导出配置档时直接进行资源检查。
- 跟流程相关的配置，增加更多的可视化编辑工具，例如副本配置，技能配置等。
- 更易扩展的字段检查工具

### 使用方法
- 编辑GCTSettings.asset，配置好Excel路径，导出路径，protoc路径等
- 参考Test目录下的Excel和Include目录，制作你自己的配置档
- Unity->配置档工具->导出全部excel，导出配置档

### 用到的第三方代码
- 使用npoi进行excel加载，为了兼容.net3.5，对源码进行了一些简单修改。
- 使用tolua.dll作为lua虚拟机，为了将字符串序列化成protobuf二进制文件。
