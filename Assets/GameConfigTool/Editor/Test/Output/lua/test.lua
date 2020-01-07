local s, r = setmetatable, rawget
local a = { ID = 1, Subid = 2, Name = 3, Array = 4, Stringarray = 5, Keyword = 6, Keywordarray = 7, Mask = 8, Maskarray = 9, Commontype = 10, Commontypearray = 11 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
local d = {
    [4] = {
        [1] = s( { [1] = 4, [2] = 1, [3] = "", [4] = { 0, 0, 0 }, [5] = { "", "", "" }, [6] = 0, [7] = { 0, 0, 0 }, [8] = 0, [9] = { 0, 0, 0 }, [10] = { name = "", id = 0, keyword = 0, namearray = { "", "" }, inner = { id = 0 }, innerarray = { { id = 0 }, { id = 0 } }, innershittype = { types = { { id = 0 } } } }, [11] = { { name = "", id = 0, keyword = 0, namearray = { "", "", "" }, inner = { id = 0 }, innerarray = { { id = 0 } }, innershittype = { types = { { id = 0 } } } }, { name = "", id = 0, keyword = 0, namearray = { "" }, inner = { id = 0 }, innerarray = { { id = 0 } }, innershittype = { types = { { id = 0 } } } } } }, m ),
    },
}
return s(d, { __index = function(t, k)
    if k == 1 then return require("config.test0")[k] end
    if k == 2 then return require("config.test1")[k] end
end, __newindex = function() end })
