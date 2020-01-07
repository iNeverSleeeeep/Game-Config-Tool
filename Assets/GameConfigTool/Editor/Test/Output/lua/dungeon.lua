local s, r = setmetatable, rawget
local a = { Type = 1, Id = 2, Name = 3 }
local m = { __index = function(t, k) return r(t, a[k]) end, __newindex = function() end }
local d = {
    [2] = {
        [1] = s( { [1] = 2, [2] = 1, [3] = "材料副本" }, m ),
    },
}
return s(d, { __index = function(t, k)
    if k == 1 then return require("config.dungeon0")[k] end
end, __newindex = function() end })
