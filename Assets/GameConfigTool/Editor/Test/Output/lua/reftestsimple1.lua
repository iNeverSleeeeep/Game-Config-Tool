local s, r = setmetatable, rawget
local a = {Id = 1, Dungeon1 = 2, Dungeon2 = 3, Client = 4, Commontypekeyword = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end}
return {
    [4] = s( { Id = 4 }, m ),
    [5] = s( { Id = 5 }, m ),
}
