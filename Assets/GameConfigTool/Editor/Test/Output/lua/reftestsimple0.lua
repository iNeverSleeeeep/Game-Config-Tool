local s, r = setmetatable, rawget
local a = { Id = 1, Client = 2, Commontypekeyword = 3, Dungeon1 = 4, Dungeon2 = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
return {
    [1] = s( { Id = 1, Client = 3, Commontypekeyword = { word = 1 }, Dungeon1 = { Type = 1, Id = 1 }, Dungeon2 = { Type = 2, Id = 1 } }, m ),
    [2] = s( { Id = 2, Client = 7, Commontypekeyword = { word = 1 }, Dungeon1 = { Type = 1, Id = 1 }, Dungeon2 = { Type = 2, Id = 1 } }, m ),
}
