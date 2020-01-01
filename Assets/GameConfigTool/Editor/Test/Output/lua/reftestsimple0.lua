local s, r = setmetatable, rawget
local a = {Id = 1, Dungeon1 = 2, Dungeon2 = 3, Client = 4, Commontypekeyword = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end}
return {
    [1] = s( { Client = 3, Commontypekeyword = { word = 1 }, Dungeon1 = { Type = 1, Id = 1 }, Dungeon2 = { Type = 2, Id = 1 }, Id = 1 }, m ),
    [2] = s( { Client = 7, Commontypekeyword = { word = 1 }, Dungeon1 = { Type = 1, Id = 1 }, Dungeon2 = { Type = 2, Id = 1 }, Id = 2 }, m ),
}
