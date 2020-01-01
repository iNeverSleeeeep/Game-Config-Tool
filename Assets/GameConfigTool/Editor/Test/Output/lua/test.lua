local s, r = setmetatable, rawget
local a = {ID = 1, Mask = 2, Maskarray = 3, Keyword = 4, Keywordarray = 5, Name = 6, Subid = 7, Stringarray = 8, Array = 9, Commontype = 10, Commontypearray = 11 }
local m = { __index = function(t, k) return r(t, a[k]) end}
local default = {
    [4] = {
        [1] = s( { [9] = { }, [11] = { }, [1] = 4, [5] = { }, [3] = { }, [8] = { }, [7] = 1 }, m ),
    },
}
return s(default, { __index = function(t, k)
    if k == 1 then return require("luaConfig.test0")[k] end
    if k == 2 then return require("luaConfig.test1")[k] end
end})
