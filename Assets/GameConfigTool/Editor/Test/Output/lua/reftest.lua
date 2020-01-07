local s, r = setmetatable, rawget
local a = { Id = 1, Excel = 2, Simpleexcel = 3, Excel2 = 4, Excel3 = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
return {
    [1] = s( { [1] = 1, [2] = { ID = 1, Subid = 1 }, [3] = 1, [4] = { ID = 1, Subid = 1 }, [5] = { t = { ID = 1, Subid = 1 }, t2 = 1 } }, m ),
    [2] = s( { [1] = 2, [2] = { ID = 1, Subid = 2 }, [3] = 2, [4] = { ID = 1, Subid = 2 }, [5] = { t = { ID = 2, Subid = 1 }, t2 = 2 } }, m ),
}
