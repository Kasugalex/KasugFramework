local ScrollRect = class("ScrollRect")

function ScrollRect:ctor(row,column,itemData)
    self.itemData       = itemData
    self.allText        = {}
    self.allItems       = {}
    self.itemInfoList   = {}

    --生成item参数
    self.row            = row
    self.column         = column
    self.maxCount       = row * column
    self.maxRow         = 1
    self.itemSizeY      = 0

    --上界
    self.upLimit        = 2.61
    self.bottmLimit     = 0
    self.curRow         = 1
    self.curTopItem     = nil
    self.curBottomItem  = nil

    --第一个实例化item距离左上角的距离
    self.upperLeft      = Vector2(199,-78)
    self.spaceX         = 120
    self.spaceY         = 130

    self.upRow          = 1
    self.bottomRow      = 1
end


function ScrollRect:InitGrid(asset,Grid)
    local anchor = asset:GetComponent(typeof(CS.UnityEngine.RectTransform))
    anchor.anchorMax = Vector2(0,1)
    anchor.anchorMin = Vector2(0,1)
    self.itemSizeY = anchor.rect.yMax + anchor:Find("Text"):GetComponent(typeof(CS.UnityEngine.RectTransform)).rect.yMax * 2
    local index = 1

    --多一行备用
    for i = 1,self.row + 1 do
        for j = 1,self.column do
            local obj = Object.Instantiate(asset,Grid).transform
            obj.name = tostring(index)
            local rectTransform = obj:GetComponent(typeof(CS.UnityEngine.RectTransform))
            rectTransform.anchoredPosition = Vector2(self.upperLeft.x  + (j - 1) * self.spaceX,self.upperLeft.y - (i - 1) * self.spaceY)
            
            table.insert(self.allText,rectTransform:GetComponentInChildren(typeof(CS.UnityEngine.UI.Text)))        
            table.insert(self.allItems,rectTransform)        
            index = index + 1      
        end
    end   

    self.curTopItem = self.allItems[1]
    self:CalculateItems()
    self:AdjustHeight(Grid)
    self:InitData()
end

function ScrollRect:Update()
    if self.curRow < self.maxRow and self.curTopItem.position.y >= self.upLimit then

        for i=1,self.column do
            local item =  self.allItems[(self.upRow - 1) * self.column + i]
            item.anchoredPosition = Vector2(item.anchoredPosition.x, self.upperLeft.y - (self.row + self.curRow ) * self.spaceY)
        end
        self.curRow = self.curRow + 1 
        self.upRow  = (self.upRow + 1) % (self.row + 2)
        self.upRow = self.upRow == 0 and 1 or self.upRow
        print(self.upRow)
        --self.curTopItem = self.itemInfoList[self.upRow]["Top"]
        print(self.itemInfoList[self.upRow]["Top"])
        print(self.itemInfoList[self.upRow]["Bottom"])
        print(self.itemInfoList[self.upRow]["BottomInsepect"])

        self.curTopItem = self.allItems[(self.upRow - 1) * self.column + 1]
        self:RefreshData()
    end

    -- if self.curRow > 1 and self.curBottomItem.position.y <= self.bottmLimit  then
    --     local downRow = (self.curRow + 4) % 6
    --     downRow = downRow == 0 and 1 or downRow + 1
    --     downRow = 5 - downRow
    --     for i=1,self.column do
    --         local item = self.allItems[(upRow - 2) * 4 + i]
    --         item.anchoredPosition = Vector2(item.anchoredPosition.x, self.upperLeft.y - (self.curRow - 2) * self.spaceY)
    --     end

    --     upRow = (upRow - 1) % 6
    --     upRow = upRow == 0 and 1 or upRow
    --     self:RefreshData()
    -- end
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

        self.itemInfoList[i] = {["Top"] = self.allItems[upIndex] ,["Bottom"] = self.allItems[bottomStartIndex],["BottomInsepect"] = self.allItems[bottomInspectStartIdx]}
    end
end


function ScrollRect:AdjustHeight(Grid)
    local dataCount = table.nums(self.itemData)
    if dataCount <= self.maxCount then
        return
    end
    local intenger, remainder = math.modf(dataCount / self.column)

    if remainder >= 1 / self.column then
        intenger = intenger + 1 
    end
    self.maxRow = intenger - self.row
    local rect = Grid.rect
    Grid.sizeDelta = Vector2(Grid.sizeDelta.x, -self.upperLeft.y + (intenger - 1) * self.spaceY + self.itemSizeY)

end

function  ScrollRect:InitData()
    local allItemsCount = table.nums(self.allText) 
    local dataCount     = table.nums(self.itemData)
    local isSmallData   = dataCount < self.maxCount
    local forCount      = isSmallData == false and self.maxCount or dataCount
    for index = 1, forCount do
        self.allText[index].text = tostring(self.itemData[index].count)
    end

    if isSmallData == true then
        for index=dataCount + 1,self.maxCount + self.column do
            self.allItems[index].gameObject:SetActive(false)
        end
    end
end

function ScrollRect:RefreshData()

end

return ScrollRect