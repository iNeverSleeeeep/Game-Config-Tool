local s, r = setmetatable, rawget
local a = { Type = 1, Id = 2, Name = 3 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
return {
    [1] = {
        [1] = s( { [1] = 1, [2] = 1, [3] = "星象副本" }, m ),
    },
}
