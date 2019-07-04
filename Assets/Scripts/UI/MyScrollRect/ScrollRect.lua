--使用此组件，参考SkillRewardView

local ScrollRect        = class("ScrollRect")
local Camera            = require("world.Camera")

function ScrollRect:ctor(row,column,itemData)
    self.itemData       = itemData
    self.dataCount      = table.nums(itemData)
    self.allItems       = {}
    self.itemCounts     = 0
    self.itemInfoList   = {}
    self.allItemsOriPos = {}
    self.itemSizeY      = 0

    --生成item参数
    self.row            = row
    self.column         = column
    self.maxCount       = row * column
    self.maxRow         = 1

    --上界
    self.upLimit        = 0
    self.bottmLimit     = 0

    --curRow存真实的滑动了多少行，upRow大于row = 1
    self.curRow         = 1
    self.upRow          = 1
    self.curTopItem     = nil
    self.curBtmIptItem  = nil
    self.transferIndex  = 0

    self.upperLeft      = Vector2(0,0)
    self.xOffset        = 0
    self.spaceX         = 0
    self.spaceY         = 0

    -- self.UICamera       = Camera:GetInstance().UICamera
    -- self.Canvas         = Camera:GetInstance().Canvas

    self.resetDataEvent = nil
    self.initGridEvent  = nil
    self.refreshDataEvent = nil

    self.obliqueOffsetX = 0
    self.obliqueOriPosX = 0
    self.obliqueOriPosY = 0

    self.Grid           = nil
    self.hideNullItem   = true
end

function ScrollRect:init(asset,Grid,hideItem)
    self:InitGrid(asset,Grid,hideItem)
    self:InitValues()
end

function ScrollRect:InitGrid(asset,thisGrid,hideItem)
    if self.initGridEvent == nil then
        CS.UnityEngine.Debug.LogError("ScrollRect组件必须设置InitGridEvent!")
        return
    end
    self.Grid = thisGrid
    local anchor = asset:GetComponent(typeof(RectTransform))
    anchor.anchorMax = Vector2(0,1)
    anchor.anchorMin = Vector2(0,1)
    local index = 1
    self.hideNullItem = hideItem == nil and true or hideItem
    --多一行备用
    for i = 1,self.row + 1 do
        for j = 1,self.column do
            local obj = Object.Instantiate(asset,self.Grid).transform
            obj.name = tostring(index)
            local rectTransform = obj:GetComponent(typeof(RectTransform))
            self.itemSizeY = rectTransform.rect.yMax
            rectTransform.anchoredPosition = Vector2(self.upperLeft.x  + (j - 1) * self.spaceX + (i - 1) * self.xOffset,self.upperLeft.y - (i - 1) * self.spaceY)

            self.initGridEvent(obj,rectTransform)

            table.insert(self.allItems,rectTransform)        
            table.insert(self.allItemsOriPos,rectTransform.anchoredPosition)        
            index = index + 1      
        end
    end   


    return self.allItems
end

function ScrollRect:InitValues()
    self.itemCounts     = (self.row + 1) * self.column
    self.curTopItem     = self.allItems[1]
    local endItem       = self.allItems[#self.allItems]
    self.obliqueOriPosY = endItem.position.y
    self.obliqueOriPosX = endItem.anchoredPosition.x
    self.obliqueOffsetX = (self.curTopItem.anchoredPosition.x - endItem.anchoredPosition.x) /
                          (self.curTopItem.position.y - endItem.position.y)             
    self:CalculateLimit()
    self:CalculateItems()
    self:AdjustHeight()
    self:InitData()
end


function ScrollRect:Reset(data)
    
    if self.resetDataEvent == nil then
        CS.UnityEngine.Debug.LogError("当前没有ResetEvent！请绑定后再执行")
        return
    end
    
    local realDatas     = data or self.itemData 
    local dataCount     = data == nil and self.dataCount or table.nums(realDatas)

    self.curRow         = 1
    self.upRow          = 1
    self.transferIndex  = 0
    self.curTopItem     = self.allItems[1]
    self.curBtmIptItem  = nil
    self.Grid.anchoredPosition = Vector2(self.Grid.anchoredPosition.x,0)

    local forCount      = dataCount > self.itemCounts and self.itemCounts or dataCount
    
    if dataCount < self.dataCount then
        self:AdjustHeight()
    end

    for i = 1,forCount do
        local rectTransform = self.allItems[i]
        if rectTransform.gameObject.activeSelf == false then
            rectTransform.gameObject:SetActive(true)
        end
        rectTransform.anchoredPosition = self.allItemsOriPos[i]
        if i <= dataCount then
            self.resetDataEvent(rectTransform,realDatas[i])
        end  
    end
    if self.hideNullItem == true then
        for i = forCount + 1,self.itemCounts do
            self.allItems[i].gameObject:SetActive(false)
        end
    end
end

--计算上下边界
function ScrollRect:CalculateLimit()
    local upStartItem            = self.allItems[#self.allItems]
    local upStartOriPos          = upStartItem.anchoredPosition

    upStartItem.anchoredPosition = Vector2(upStartItem.anchoredPosition.x,upStartItem.rect.yMax)
    self.upLimit                 = upStartItem.position.y
    upStartItem.anchoredPosition = Vector2(upStartItem.anchoredPosition.x,self.upperLeft.y - (self.row - 1 ) * self.spaceY)
    self.bottmLimit              = upStartItem.position.y
    
    upStartItem.anchoredPosition = upStartOriPos

end

--竖划
function ScrollRect:VerticalUpdate()

    if self.curRow < self.maxRow and self.curTopItem.position.y >= self.upLimit then
        self.transferIndex = self.curTopItem.name

        for i=1,self.column do
            local item =  self.allItems[(self.upRow - 1) * self.column + i]
            item.anchoredPosition = Vector2(item.anchoredPosition.x, self.upperLeft.y - (self.row + self.curRow ) * self.spaceY)
        end
        self.curRow = self.curRow + 1 
        self.upRow  = (self.upRow + 1) % (self.row + 2)
        self.upRow  = self.upRow == 0 and 1 or self.upRow

        self.curTopItem = self.itemInfoList[self.upRow]["Top"]

        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]
        --print(self.upRow,self.itemInfoList[self.upRow]["Top"],self.itemInfoList[self.upRow]["Bottom"],self.itemInfoList[self.upRow]["BottomInsepect"])

        self:RefreshData(true)
    end

    if self.curRow > 1 and self.curBtmIptItem.position.y < self.bottmLimit  then
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

        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]
        self:RefreshData(false)
    end
end

--横滑
function ScrollRect:HorizontalUpdate()
    -- body
end

--斜滑
function ScrollRect:ObliqueUpdate()
    if self.curRow < self.maxRow and self.curTopItem.position.y >= self.upLimit then
        self.transferIndex = self.curTopItem.name

        for i=1,self.column do
            local item            = self.allItems[(self.upRow - 1) * self.column + i]
            local finalY          = self.upperLeft.y - (self.row + self.curRow ) * self.spaceY
            item.anchoredPosition = Vector2(self.obliqueOriPosX, finalY)
        end
        self.curRow = self.curRow + 1 
        self.upRow  = (self.upRow + 1) % (self.row + 2)
        self.upRow  = self.upRow == 0 and 1 or self.upRow

        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]

        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]
     
        self:RefreshData(true)
    end

    if self.curRow > 1 and self.curBtmIptItem.position.y < self.bottmLimit  then
        local curBottomItem = self.itemInfoList[self.upRow]["Bottom"]
        self.transferIndex  = curBottomItem.name
        local index = tonumber(curBottomItem.name)
        for i=1,self.column do
            local item      = self.allItems[index - 1 + i]
            local finalY    = self.upperLeft.y - (self.curRow - 2.0) * self.spaceY
            local realX     = (finalY - self.obliqueOriPosY) * self.obliqueOffsetX + self.obliqueOriPosX
            item.anchoredPosition = Vector2(realX, finalY)
        end

        self.curRow         = self.curRow - 1
        self.upRow          = (self.upRow - 1) % (self.row + 2)
        self.upRow          = self.upRow == 0 and self.row + 1 or self.upRow

        self.curTopItem     = self.itemInfoList[self.upRow]["Top"]
        self.curBtmIptItem  = self.itemInfoList[self.upRow]["BottomInsepect"]
        self:RefreshData(false)
    end

    for i=1,self.itemCounts do
        local realX = (self.allItems[i].position.y - self.obliqueOriPosY) * self.obliqueOffsetX + self.obliqueOriPosX
        self.allItems[i].anchoredPosition = Vector2(realX,self.allItems[i].anchoredPosition.y)
    end

end

--计算curRow对应的TopItem,BottomItem和bottomInsepectItem
function ScrollRect:CalculateItems()
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

        self.itemInfoList[i]        = { ["Top"]             = self.allItems[upIndex] ,
                                        ["Bottom"]          = self.allItems[bottomStartIndex],
                                        ["BottomInsepect"]  = self.allItems[bottomInspectStartIdx]}
    end
end

function ScrollRect:AdjustHeight()
    if self.dataCount <= self.maxCount then
        return
    end
    local intenger, remainder = math.modf(self.dataCount / self.column)

    if remainder >= 1 / self.column then
        intenger = intenger + 1 
    end
    self.maxRow = intenger - self.row
    local rect = self.Grid.rect
    self.Grid.sizeDelta = Vector2(self.Grid.sizeDelta.x, -self.upperLeft.y + (intenger - 1) * self.spaceY + self.itemSizeY)
end

function  ScrollRect:InitData()
    local dataCount     = self.dataCount
    local maxCount      = self.maxCount + self.column
    local isSmallData   = dataCount <= maxCount
    local forCount      = isSmallData == false and maxCount or dataCount

    for index = 1, forCount do
        -- self.allText[index].text = tostring(self.itemData[index].count)
        self.refreshDataEvent(self.allItems[index],self.itemData[index])
    end

    if self.hideNullItem == true then
        local allItemIndex  = self.maxCount + self.column * 1
        if dataCount < allItemIndex then
            for index=dataCount + 1,self.maxCount + self.column do
                self.allItems[index].gameObject:SetActive(false)
            end
        end
    end
end

function ScrollRect:RefreshData(slideDown)
    local dataNums      = self.dataCount
    local dataIndex     = 0
    if slideDown == true then
        dataIndex = (self.curRow - 1 + self.row) * self.column
    else
        dataIndex =  (self.curRow - 1) * self.column
    end

    --刷新数据
    for i=1,self.column do
        local itemIndex     = tonumber(self.transferIndex) - 1 + i
        local dataIndextmp  = dataIndex + i
        if dataIndextmp > dataNums then
            if self.hideNullItem == true then
                self.allItems[itemIndex].gameObject:SetActive(false)
            end
        else
            if self.allItems[itemIndex].gameObject.activeSelf == false then
                self.allItems[itemIndex].gameObject:SetActive(true)
            end
            -- self.allText[itemIndex].text = tostring(self.itemData[dataIndextmp].count)
            if self.refreshDataEvent ~= nil then
                self.refreshDataEvent(self.allItems[itemIndex],self.itemData[dataIndextmp])
            end

        end
    end
end

return ScrollRect