local UIBase            = require("ui.UIBase")
local BagView           = class("BagView",UIBase)
local ScrollRect        = require("ui.uiScripts.ScrollRect")

function BagView:Awake()

    self.isInited       = false
    
    --服务器返回的所有物品信息
    self.itemData       = {}
end

function BagView:OnShow(data)
    self.super.OnShow(self)
    self.itemData = data.Items
end

function BagView:Start()
    self.Close.onClick:AddListener(function() UIManager:CloseUI(self) end)

    self:InitGrid()
end


function BagView:OnUpdate()
    if self.isInited == true then  
        self.scrollRect:Update()
    end
end

function BagView:InitGrid()

    self.scrollRect = ScrollRect.new(4, 4, self.itemData)

    utils:StartCoroutine(function()
        local f, asset = coroutine.yield(utils:WaitAndLoadAsset("Assets/GameMain/UI/UIItems/ItemPrefab.prefab"))
        if f:isResolved() then
            self.scrollRect:InitGrid(asset,self.Grid)
            self.isInited = true
        else
            Debug.LogError("Cannot load ItemPrefab");
        end
    end)
    
end


function BagView:GetCurUTDItem()

end

function BagView:GetCurDTUItem()

end



return BagView