local s, r = setmetatable, rawget
local a = {Id = 1, Type = 2, Name = 3 }
local m = { __index = function(t, k) return r(t, a[k]) end}
return {
    [1] = {
        [1] = s( { [1] = 1, [3] = "星象副本", [2] = 1 }, m ),
    },
}
