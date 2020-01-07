local s, r = setmetatable, rawget
local a = { Id = 1, Client = 2, Commontypekeyword = 3, Dungeon1 = 4, Dungeon2 = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
local d = {
}
return s(d, { __index = function(t, k)
    if k >= 1 and k < 3 then return require("config.reftestsimple0")[k] end
    if k >= 4 and k < 100 then return require("config.reftestsimple1")[k] end
end, __newindex = function() end })
