local ScrollRect        = require("ui.ScrollRect")
local LoopScrollRect    = class("LoopScrollRect", ScrollRect)
local Camera            = require("world.Camera")

function LoopScrollRect:ctor(row,column,itemData)
    LoopScrollRect.super.ctor(self,row,column,itemData)
    self.obliqueUpdateEvent = nil
    self.scrollComponent    = nil
    self.view               = nil
    self.currentDataIndex   = 0
end

function LoopScrollRect:init(asset,Grid)
    self:InitGrid(asset,Grid)
    self:InitValues()
end

function LoopScrollRect:InitGrid(asset,Grid)
    if self.initGridEvent == nil then
        CS.UnityEngine.Debug.LogError("ScrollRect组件必须设置InitGridEvent!")
        return
    end
    local anchor        = asset:GetComponent(typeof(RectTransform))
    anchor.anchorMax    = Vector2(0,1)
    anchor.anchorMin    = Vector2(0,1)
    local index         = 1
    self.Grid = Grid

    --多一行备用
    for i = 1,self.row + 1 do
        for j = 1,self.column do
            local obj = Object.Instantiate(asset,self.Grid).transform
            obj.name = tostring(index)
            local rectTransform = obj:GetComponent(typeof(RectTransform))
            self.itemSizeY = rectTransform.rect.yMax
            rectTransform.anchoredPosition = Vector2(self.upperLeft.x + (j - 1) * self.spaceX + (i - 1) * self.xOffset,self.upperLeft.y - (i - 1) * self.spaceY)
            self.initGridEvent(self.view,obj,rectTransform)
            table.insert(self.allItems,rectTransform)        
            table.insert(self.allItemsOriPos,rectTransform.anchoredPosition)        
            index = index + 1      
        end
    end   

end

function LoopScrollRect:InitValues()
    self.itemCounts         = (self.row + 1) * self.column
    self.curTopItem         = self.allItems[1]
    local endItem           = self.allItems[#self.allItems]
    self.obliqueOriPosY     = endItem.anchoredPosition.y
    self.obliqueOriPosX     = endItem.anchoredPosition.x
    self.obliqueOffsetX     = (self.upperLeft.x - self.xOffset - self.spaceX- endItem.anchoredPosition.x) /
                              (self.upLimit - endItem.anchoredPosition.y)       

    self.scrollComponent    = self.Grid.gameObject:GetComponent(typeof(MyScrollRect))
    self.scrollComponent:GetChildren()
    self:CalculateLimit()
    self:CalculateItems()
    self:InitData()
end

function LoopScrollRect:Reset(data)
    
    if self.resetDataEvent == nil then
        CS.UnityEngine.Debug.LogError("当前没有ResetEvent！请绑定后再执行")
        return
    end
    self.itemData       = data or self.itemData
    local realDatas     = self.itemData
    local dataCount     = table.nums(realDatas)
    self.curRow         = 1
    self.upRow          = 1
    self.transferIndex  = 0
    self.curTopItem     = self.allItems[1]
    self.curBtmIptItem  = self.itemInfoList[1]["BottomInsepect"]

    self.currentDataIndex   = 0
    --local log           = {}
    for i = 1,self.itemCounts do
        local rectTransform = self.allItems[i]
        rectTransform.anchoredPosition = self.allItemsOriPos[i]
        rectTransform:SetAsLastSibling()
        local dataIndex = i  % dataCount
        dataIndex = dataIndex == 0 and dataCount or dataIndex
        --table.insert( log,dataIndex)
        self.resetDataEvent(self.view,rectTransform,realDatas[dataIndex])
    end
end

--计算上下边界
function LoopScrollRect:CalculateLimit()
    local upStartItem            = self.allItems[#self.allItems]
    local upStartOriPos          = upStartItem.anchoredPosition

    upStartItem.anchoredPosition = Vector2(upStartItem.anchoredPosition.x,upStartItem.sizeDelta.y)
    self.upLimit                 = upStartItem.anchoredPosition.y
    upStartItem.anchoredPosition = Vector2(upStartItem.anchoredPosition.x,self.upperLeft.y - (self.row - 1) * self.spaceY)
    self.bottmLimit              = upStartItem.anchoredPosition.y
    
    upStartItem.anchoredPosition = upStartOriPos
    --print(self.upLimit,self.bottmLimit)
end

--竖划
function LoopScrollRect:VerticalUpdate()
    if  self.curTopItem.anchoredPosition.y >= self.upLimit then
        self.transferIndex = self.curTopItem.name
        local upperItem = self.itemInfoList[self.upRow]["BottomInsepect"]
        for i=1,self.column do
            local item =  self.allItems[(self.upRow - 1) * self.column + i]
            item.anchoredPosition = Vector2(item.anchoredPosition.x, upperItem.anchoredPosition.y - self.spaceY)
        end
        self.curRow = self.curRow + 1 
        self.upRow  = (self.upRow + 1) % (self.row + 2)
        self.upRow  = self.upRow == 0 and 1 or self.upRow
        self.curTopItem:SetAsLastSibling()

        self.curTopItem = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]
        --print(self.upRow,self.itemInfoList[self.upRow]["Top"],self.itemInfoList[self.upRow]["Bottom"],self.itemInfoList[self.upRow]["BottomInsepect"])

        self:RefreshData(true)
    end

    if  self.curBtmIptItem.anchoredPosition.y < self.bottmLimit  then
        local curBottomItem = self.itemInfoList[self.upRow]["Bottom"]
        self.transferIndex  = curBottomItem.name
        local index = tonumber(curBottomItem.name)
        for i=1,self.column do
            local item = self.allItems[index - 1 + i]
            item.anchoredPosition = Vector2(item.anchoredPosition.x, self.upperLeft.y - (self.curRow - 2) * self.spaceY)
        end

        self.curRow         = self.curRow - 1
        self.upRow          = (self.upRow - 1) % (self.row + 2)
        self.upRow          = self.upRow == 0 and self.row + 1 or self.upRow
        curBottomItem:SetAsFirstSibling()
        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]

        self:RefreshData(false)
    end
end

--横滑
function LoopScrollRect:HorizontalUpdate()

end

--斜滑
function LoopScrollRect:ObliqueUpdate()
    if  self.curTopItem.anchoredPosition.y > self.upLimit then
        self.transferIndex = self.curTopItem.name
        local upperItem = self.itemInfoList[self.upRow]["Bottom"]
        local index = tonumber(self.curTopItem.name)
        for i=index,index + self.column do
            local item            = self.allItems[index]
            local finalY          = upperItem.anchoredPosition.y - self.spaceY
            item.anchoredPosition = Vector2(self.obliqueOriPosX, finalY)
        end

        self:RefreshData(false)

        self.curRow = self.curRow + 1 
        self.upRow  = (self.upRow + 1) % (self.row + 2)
        self.upRow  = self.upRow == 0 and 1 or self.upRow
        self.curTopItem:SetAsLastSibling()
        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]   

    end

    if  self.curBtmIptItem.anchoredPosition.y < self.bottmLimit  then
        local curBottomItem = self.itemInfoList[self.upRow]["Bottom"]
        self.transferIndex  = curBottomItem.name
        local upperItem     = self.curTopItem
        local index         = tonumber(curBottomItem.name)
        for i=index,index   + self.column do
            local item      = self.allItems[index]
            local finalY    = upperItem.anchoredPosition.y + self.spaceY
            local realX     = (finalY - self.obliqueOriPosY) * self.obliqueOffsetX + self.obliqueOriPosX
            item.anchoredPosition = Vector2(realX, finalY)
        end

        self:RefreshData(true)

        self.curRow         = self.curRow - 1
        self.upRow          = (self.upRow - 1) % (self.row + 2)
        self.upRow          = self.upRow == 0 and self.row + 1 or self.upRow
        curBottomItem:SetAsFirstSibling()
        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]

    end

    self:UpdateObliquePosition()

    if self.obliqueUpdateEvent then self.obliqueUpdateEvent(self.allItems,self.upRow) end
end

function LoopScrollRect:UpdateObliquePosition()
    for i=1,self.itemCounts do
        local y = self.allItems[i].anchoredPosition.y
        local realX = (y - self.obliqueOriPosY) * self.obliqueOffsetX + self.obliqueOriPosX
        self.allItems[i].anchoredPosition = Vector2(realX,y)
    end
end

--计算curRow对应的TopItem,BottomItem和bottomInsepectItem
function LoopScrollRect:CalculateItems()
    local allRow = self.row + 2
    for i=1,self.row + 1 do
        local bottomRow             = (i + self.row) % allRow
        bottomRow                   = bottomRow == 0 and 1 or (bottomRow + 1) % allRow
        bottomRow                   = bottomRow % allRow == 0 and allRow - 1 or bottomRow % allRow
        local bottomStartIndex      = (bottomRow - 1) * self.column + 1
        local bottomInspectRow      = bottomRow - 1
        bottomInspectRow            = bottomInspectRow == 0 and allRow - 1 or bottomInspectRow
        bottomInspectRow            = bottomInspectRow == 0 and 1 or bottomInspectRow
        local bottomInspectStartIdx = (bottomInspectRow - 1) * self.column + 1
        local upIndex               = (i - 1) * self.column + 1

        self.itemInfoList[i]        = { ["Top"]             = self.allItems[upIndex],
                                        ["Bottom"]          = self.allItems[bottomStartIndex],
                                        ["BottomInsepect"]  = self.allItems[bottomInspectStartIdx]}
    end
    self.curBtmIptItem              = self.itemInfoList[1]["BottomInsepect"]

end

function  LoopScrollRect:InitData()
    local itemCount = self.row + 1
    local smallData = self.dataCount < itemCount
    local forCount = smallData == false and itemCount or self.dataCount
    for index = 1, itemCount do
        local realIndex = index > self.dataCount and index % self.dataCount or index
        realIndex = realIndex == 0 and self.dataCount or realIndex
        self.refreshDataEvent(self.view,self.allItems[index],self.itemData[realIndex])
    end
end

function LoopScrollRect:RefreshData(slideDown)
--[[    local dataNums      = self.dataCount
    local dataIndex     = self.currentDataIndex
    local curRow        = self.curRow


    if dataIndex == 0 then
        if slideDown == true then
            dataIndex = dataNums - self.column
        else
            dataIndex = self.row + 2
        end
    else
        if slideDown == true then
            if self.curRow == 0 then
                dataIndex = dataNums - self.column
            else
                dataIndex = dataIndex - 1
                dataIndex = dataIndex % dataNums
                dataIndex = dataIndex == 0 and dataNums - self.column or dataIndex
            end
        else
            if self.curRow == 1 then
                dataIndex = self.row + 1
            else
                dataIndex = dataIndex + 1
                dataIndex = dataIndex % dataNums
                dataIndex = dataIndex == 0 and 1 or dataIndex
            end
        end
    end

    self.currentDataIndex = dataIndex
    print(curRow.."---->".. self.currentDataIndex + 1)
]]--
    local dataNums      = self.dataCount
    local curRow        = self.curRow
    local dataIndex     = curRow % dataNums
    
    local transferItem  = nil

    if slideDown == true then
        transferItem    = self.curTopItem:GetComponent(typeof(CS.LuaBehaviour)).LuaTable.dataIndex
        dataIndex = transferItem - self.column
        dataIndex = dataIndex % dataNums
        dataIndex = dataIndex == 0 and dataNums - self.column or dataIndex - 1
        --print("下滑---->"..transferItem.."---->"..dataIndex)
    else
        transferItem    = self.curBtmIptItem:GetComponent(typeof(CS.LuaBehaviour)).LuaTable.dataIndex
        dataIndex = transferItem + self.column
        dataIndex = dataIndex % dataNums
        --print("上滑---->"..transferItem.."---->"..dataIndex)
    end

    --刷新数据
    for i=1,self.column do
        local itemIndex     = tonumber(self.transferIndex) - 1 + i
        local dataIndextmp  = dataIndex + i
        if self.refreshDataEvent ~= nil then
            self.refreshDataEvent(self.view,self.allItems[itemIndex],self.itemData[dataIndextmp])
        end
    end
end

function LoopScrollRect:SetBeginDragEvent(beginDragEvent)

    self.scrollComponent.beginDragEvent = beginDragEvent
end

function LoopScrollRect:SetDragEvent(dragEvent)

    self.scrollComponent.dragEvent = dragEvent
end


function LoopScrollRect:SetEndDragEvent(endDragEvent)

    self.scrollComponent.endDragEvent = endDragEvent
end

function LoopScrollRect:SetScrollStopEvent(stopEvent)

    self.scrollComponent.scrollStopEvent = stopEvent
end

--滑动时得到正确的数据索引
function LoopScrollRect:GetFinalIndex(idx)
    local finalIdx = idx
    if finalIdx >  self.dataCount then
        while finalIdx > self.dataCount do
            finalIdx = finalIdx - self.dataCount
        end
    elseif finalIdx < 1 then
        while finalIdx < 1 do
            finalIdx = finalIdx + self.dataCount
        end 
    end
    return finalIdx

end

return LoopScrollRect