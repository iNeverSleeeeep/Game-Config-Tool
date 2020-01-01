local s, r = setmetatable, rawget
local a = {Id = 1, Excel = 2, Excel2 = 3, Excel3 = 4, Simpleexcel = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end}
return {
    [1] = s( { [2] = { ID = 1, Subid = 1 }, [3] = { ID = 1, Subid = 1 }, [4] = { t = { ID = 1, Subid = 1 }, t2 = 1 }, [1] = 1, [5] = 1 }, m ),
    [2] = s( { [2] = { ID = 1, Subid = 2 }, [3] = { ID = 1, Subid = 2 }, [4] = { t = { ID = 2, Subid = 1 }, t2 = 2 }, [1] = 2, [5] = 2 }, m ),
}
