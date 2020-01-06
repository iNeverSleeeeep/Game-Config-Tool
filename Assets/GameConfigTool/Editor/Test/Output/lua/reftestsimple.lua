local s, r = setmetatable, rawget
local a = {Id = 1, Dungeon1 = 2, Dungeon2 = 3, Client = 4, Commontypekeyword = 5 }
local m = { __index = function(t, k) return r(t, a[k]) end}
local default = {
}
return s(default, { __index = function(t, k)
    if k >= 1 and k < 3 then return require("config.reftestsimple0")[k] end
    if k >= 4 and k < 100 then return require("config.reftestsimple1")[k] end
end})
