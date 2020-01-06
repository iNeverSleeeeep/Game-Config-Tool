local s, r = setmetatable, rawget
local a = {Id = 1, Type = 2, Name = 3 }
local m = { __index = function(t, k) return r(t, a[k]) end}
local default = {
    [2] = {
        [1] = s( { [1] = 1, [3] = "材料副本", [2] = 2 }, m ),
    },
}
return s(default, { __index = function(t, k)
    if k == 1 then return require("config.dungeon0")[k] end
end})
