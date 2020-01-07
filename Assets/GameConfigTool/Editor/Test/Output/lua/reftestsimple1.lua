local s, r = setmetatable, rawget
local a = { Id = 1, Client = 2, Commontypekeyword = 3, Dungeon1 = 4, Dungeon2 = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
return {
    [4] = s( { Id = 4 }, m ),
    [5] = s( { Id = 5 }, m ),
}
