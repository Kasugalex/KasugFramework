local LoopScrollRect                = require("ui.LoopScrollRect")
local SkillRewardScrollComponent    = class("SkillRewardScrollComponent",LoopScrollRect)
function SkillRewardScrollComponent:ctor(row,column,itemData)
    self.super.ctor(self,row,column,itemData)
    self.Position           = {}--{-6,-103,-297,-394,-491,-588,-729}
    --当前选择的技能
    self.selectItem         = nil

    self.updatePosition     = false

    self.yOffset            = 5 

    self.oriHeight          = nil
    self.selHeight          = nil
end

function SkillRewardScrollComponent:InitValues()
    self.super.InitValues(self)
    self.scrollComponent.obliqueUpdateEvent = function(rt,speed) self:ObliqueUpdateEvent(rt,speed)  end
end

function SkillRewardScrollComponent:ObliqueUpdateEvent(rt,speed)
    local targetPos = rt.anchoredPosition.y + speed.y
    local moveTo = rt.anchoredPosition + Vector2(0,speed.y)
    local realX = (moveTo.y - self.obliqueOriPosY) * self.obliqueOffsetX + self.obliqueOriPosX 
    rt.anchoredPosition = Vector2(realX,moveTo.y)
end


--[[第一版，不太满意
function SkillRewardScrollComponent:InitGrid(asset,Grid)
    self.super.InitGrid(self,asset,Grid)
    for i=1,table.nums(self.allItems) do
        if i ~= 3 then
            table.insert(self.Position,self.allItems[i].anchoredPosition.y)
        end
    end

    table.insert( self.Position,self.Position[#self.Position] - self.spaceY)
end

function SkillRewardScrollComponent:InitValues()
    self.super.InitValues(self)
    
    local height   = self.allItems[1].sizeDelta.y
    self.oriHeight = height
    self.selHeight = height * 2 + self.yOffset
    self:ScrollStopUpdatePosition()
    self:SetBeginDragEvent(function() self:BeginDrag() end)
    self:SetDragEvent(function() self:Drag() end)
    self:SetEndDragEvent(function() self:EndDrag() end)
    self:SetScrollStopEvent(function() self:ScrollStopUpdatePosition() end)

end

function SkillRewardScrollComponent:ObliqueUpdate()
    if  self.updatePosition == false then
        return
    end

    self.super.ObliqueUpdate(self)
end

function SkillRewardScrollComponent:Reset(data)
    self.super.Reset(self,data)
    if self.selectItem ~= nil then
        self.selectItem.sizeDelta = Vector2(self.selectItem.sizeDelta.x, self.oriHeight)
    end
    self:ScrollStopUpdatePosition()

end

function SkillRewardScrollComponent:BeginDrag()

end

function SkillRewardScrollComponent:Drag()
    self.updatePosition = true
    if  self.selectItem ~= nil then
        self.selectItem.sizeDelta = Vector2(self.selectItem.sizeDelta.x, self.oriHeight)
        local startResetIndex = self.selectIndex + 2
        for i = startResetIndex,self.row + 1 do
            local itemIndex = tonumber(self.Grid:GetChild(i - 1).name)
            self.allItems[itemIndex].anchoredPosition = Vector2(self.allItems[itemIndex].anchoredPosition.x,self.selectItem.anchoredPosition.y - self.spaceY * (i - startResetIndex + 1))
        end
        self.selectItem = nil
    end
end

function SkillRewardScrollComponent:EndDrag()
    if  self.scrollComponent.Velocity.y == 0 and self.selectItem == nil then
        self:ScrollStopUpdatePosition()
    end
end

--滑动停止判断选择
function SkillRewardScrollComponent:ScrollStopUpdatePosition()
    self.updatePosition = false
    local childCount    = self.Grid.childCount
    local firstChild    = self.allItems[tonumber(self.Grid:GetChild(0).name)]
    local height        = firstChild.sizeDelta.y 
    
    --这个selectIndex传给GetChild，从0开始
    self.selectIndex    = firstChild.anchoredPosition.y < height / 2 and 1 or 2
    self.selectItem     = self.allItems[tonumber(self.Grid:GetChild(self.selectIndex).name)]
    self.selectItem.sizeDelta  = Vector2(self.selectItem.rect.width,self.selHeight)
    local child         = nil
    if  self.selectIndex == 2 then
        child =  self.allItems[tonumber(self.Grid:GetChild(0).name)]
        child.anchoredPosition = Vector2(child.anchoredPosition.x,self.Position[#self.Position])  
        for i = 1,childCount - 1 do
            child =  self.allItems[tonumber(self.Grid:GetChild(i).name)]
            child.anchoredPosition = Vector2(child.anchoredPosition.x,self.Position[i])  
        end
    else
        for i = 1,childCount do
            local child =  self.allItems[tonumber(self.Grid:GetChild(i - 1).name)]
            child.anchoredPosition = Vector2(child.anchoredPosition.x,self.Position[i])  
        end
    end

    self:UpdateObliquePosition()
end 
--]]
return SkillRewardScrollComponent