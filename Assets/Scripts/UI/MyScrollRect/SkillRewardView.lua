local UIBase            = require('ui.UIBase')
local SkillRewardView   = class("SkillRewardView",UIBase)
local LoopScrollRect    = require("ui.uiScripts.SkillRewardScrollComponent")
local upperLeft         = Vector2(230,-6)
local xOffset           = -15
local spaceY            = 97
local itemScript        = "ui.uiScripts.SkillInfo"


function SkillRewardView:Awake()
    self.isInited       = false

    --所有技能
    self.currentMySkills   = {}

    --当前所有技能的behaviour
    self.newSkillBehaviour = {}
    self. mySkillBehaviour  = {}

    --英雄对应的技能
    self. heroSkills        = {}


    --选择的技能
    self.currentNewSkill   = nil
    self.currentMySkill    = nil
end

function SkillRewardView:OnShow(data)
    self.super.OnShow(self)
    --dump(data)

    self.newSkillsData          = data.newSkills 
    self.mySkillData            = data.heroSkill1

    self.heroSkills["Hero1Skill"]    = data.heroSkill1
    self.heroSkills["Hero2Skill"]    = data.heroSkill2
    self.heroSkills["Hero3Skill"]    = data.heroSkill3
    self.currentMySkills             = self.heroSkills["Hero1Skill"]

    --默认选择的技能都为第一个
    self.currentNewSkill        = self.newSkillsData[1]
    self.currentMySkill         = self.currentMySkills[1]

    if self.isInited == true then
        self.newSkillScroll:Reset(self.newSkillsData)
        self.mySkillScroll:Reset(self.heroSkills["Hero1Skill"])
    end
end 

function SkillRewardView:Start()
    -- utils:StartCoroutine(function()
    --     coroutine.yield(utils:WaitForRemove(self));
    --     self.super.RemoveAllButtonListeners(self);
    --     print("ButtonListener移除完毕")
    -- end)

    self.Close.onClick:AddListener(function() UIManager:CloseUI(self) end)
    self.Hero1.onClick:AddListener(function() self:Hero1Click() end)
    self.Hero2.onClick:AddListener(function() self:Hero2Click() end)
    self.Hero3.onClick:AddListener(function() self:Hero3Click() end)
    self.Confirm.onClick:AddListener(function() self:ConfirmReplace() end)

    self:InitGrid()
end

function SkillRewardView:OnUpdate()
    if self.isInited == true then  
        self.newSkillScroll:ObliqueUpdate()
        self.mySkillScroll:ObliqueUpdate()
    end
end

function SkillRewardView:OnHide()

end

function SkillRewardView:InitGrid()
    self.newSkillScroll             = LoopScrollRect.new(6, 1, self.newSkillsData)
    self.newSkillScroll.view        = self
    self.newSkillScroll.xOffset     = xOffset
    self.newSkillScroll.spaceY      = spaceY
    self.newSkillScroll.upperLeft   = upperLeft

    self.mySkillScroll              = LoopScrollRect.new(6, 1, self.mySkillData)
    self.mySkillScroll.view        = self
    self.mySkillScroll.xOffset      = xOffset
    self.mySkillScroll.spaceY       = spaceY
    self.mySkillScroll.upperLeft    = upperLeft

    utils:StartCoroutine(function()
        local f, asset = coroutine.yield(utils:WaitAndLoadAsset("Assets/GameMain/UI/UIItems/SkillPrefab.prefab"))
        if f:isResolved() then
            self.newSkillScroll.initGridEvent       = SkillRewardView.InitNewSkillGridEvent
            self.newSkillScroll.refreshDataEvent    = SkillRewardView.RefreshNewSkillDataEvent
            self.newSkillScroll.resetDataEvent      = SkillRewardView.ResetNewSkillDataEvent
            --self.newSkillScroll.obliqueUpdateEvent  = SkillRewardView.UpdateEvent
    
            self.mySkillScroll.initGridEvent        = SkillRewardView.InitMySkillGridEvent
            self.mySkillScroll.refreshDataEvent     = SkillRewardView.RefreshMySkillDataEvent
            self.mySkillScroll.resetDataEvent       = SkillRewardView.ResetMySkillDataEvent
            --self.mySkillScroll.obliqueUpdateEvent   = SkillRewardView.UpdateEvent

            self.newSkillScroll:init(asset,self.NewSkillGrid)
            self.mySkillScroll:init(asset,self.MySkillGrid)
            self.isInited = true
        else
            Debug.LogError("Cannot load ItemPrefab");
        end
    end)
    
end

function SkillRewardView:Hero1Click()
    self.mySkillScroll:Reset(self.heroSkills["Hero1Skill"])
    self.currentMySkills = self.heroSkills["Hero1Skill"]
    self:SetDefaultData()
end

function SkillRewardView:Hero2Click()
    self.mySkillScroll:Reset(self.heroSkills["Hero2Skill"])
    self.currentMySkills = self.heroSkills["Hero2Skill"]
    self:SetDefaultData()
end

function SkillRewardView:Hero3Click()
    self.mySkillScroll:Reset(self.heroSkills["Hero3Skill"])
    self.currentMySkills = self.heroSkills["Hero3Skill"]
    self:SetDefaultData()
end

function SkillRewardView:SetDefaultData()
    self.currentMySkill = self.currentMySkills[1]
    self.currentNewSkill= self.newSkillsData[1]

    --刷新Grid

end 

function SkillRewardView:ConfirmReplace()
    local UOD = {}
    UOD.oldSkill = self.currentMySkill
    UOD.newSkill = self.currentNewSkill
    UIManager:OpenUI("ReplaceSkillView",UOD)
end

-----------------------------------------------------------------
--mySkill 相关
function SkillRewardView.InitMySkillGridEvent(self,obj,rectTransform)
    local behaviour = obj.gameObject:AddLuaBehaviour(itemScript)
    table.insert(self.mySkillBehaviour, behaviour)
    behaviour.LuaTable.isNewSkill = false
end

function SkillRewardView.RefreshMySkillDataEvent(self,item,data)
    SkillRewardView.SetSkillData(self.mySkillBehaviour,item,data)
end

function SkillRewardView.ResetMySkillDataEvent(self,rectTransform,data)
    SkillRewardView.SetSkillData(self.mySkillBehaviour,rectTransform,data)
end 

-----------------------------------------------------------------
--newSkill 相关
function SkillRewardView.InitNewSkillGridEvent(self,obj,rectTransform)
    local behaviour = obj.gameObject:AddLuaBehaviour(itemScript)
    table.insert(self.newSkillBehaviour, behaviour)
    behaviour.LuaTable.isNewSkill = true
end

function SkillRewardView.RefreshNewSkillDataEvent(self,item,data)
    SkillRewardView.SetSkillData(self.newSkillBehaviour,item,data)
end

function SkillRewardView.ResetNewSkillDataEvent(self,rectTransform,data)
    SkillRewardView.SetSkillData(self.newSkillBehaviour,rectTransform,data)
end 

-----------------------------------------------------------------
function SkillRewardView.SetSkillData(skillBehaviours,item,data)

    if data == nil then return end

    local behaviour         = skillBehaviours[tonumber(item.name)]
    local table             = behaviour.LuaTable

    table["itemName"]       = data.name
    table["itemId"]         = data.id
    table["itemIntro"]      = data.itemIntro
    --table["itemIcon"]      = data.itemIcon

    table.Text.text = data.name
end

return SkillRewardView