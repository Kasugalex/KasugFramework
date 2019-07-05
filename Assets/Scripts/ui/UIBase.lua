local UIBase = class("UIBase")
local UIButton = require("ui.UIButton")

local UIClick       = {}
local UIDrag        = {}
local UIPress       = {}
local UIPressTime   = {}

--四个象限里存 UIBaseBound
local firstQuadrantUI = {}
local secondQuadrantUI = {}
local thirdQuadrantUI = {}
local fourthQuadrantUI = {}

local Camera = require("world.Camera")

function UIBase:OnShow(data)    

    self.rectTransform.anchorMin = Vector2.zero
    self.rectTransform.anchorMax = Vector2.one
    self.rectTransform.anchoredPosition3D = Vector3(0,0,-self.rectTransform.parent.anchoredPosition3D.z)
    self.rectTransform.offsetMin = Vector2.zero
    self.rectTransform.offsetMax = Vector2.zero
end

function UIBase.RemoveAllButtonListeners(self)
    dump(self)
end

-- function UIBase:OnDestroy()
--     dump(self, "=====")
--      self:RemoveAllButtonListeners()
-- end

     --[[
    --派发事件必须
    Component(self)
        :addComponent("components.behavior.EventProtocol")
        :exportMethods()

    local transform             = self.gameObject:GetComponent(typeof(CS.UnityEngine.RectTransform));
    transform.anchorMin         = Vector2.zero;
    transform.anchorMax         = Vector2.one;
    transform.anchoredPosition  = Vector2.zero;
    transform.sizeDelta         = Vector2.zero;

    local uiButtons             = self.gameObject:GetComponentsInChildren(typeof(UIButton),true)
    local middleX               = CS.UnityEngine.Screen.width * 0.5
    local middleY               = CS.UnityEngine.Screen.height * 0.5

    local uiCamera              = Camera:GetInstance().UICamera;

    --计算四个角点
    for _,v in ipairs(uiButtons) do
        local rectTransform = v.rectTransform
        local pos               = uiCamera:WorldToScreenPoint(rectTransform.position)
        local rect              = rectTransform.rect
        local size              = {rect.xMin,rect.xMax,rect.yMin,rect.yMax}
        local bound             = {}
        local image             = v.sourceImage

        local left              = pos.x + size[1]
        local right             = pos.x + size[2]
        local bottom            = pos.y + size[3]
        local up                = pos.y + size[4]

        local bottomLeft        = Vector2(left, bottom)
        local upperLeft         = Vector2(left, up)
        local upperRight        = Vector2(right, up)
        local bottomRight       = Vector2(right, bottom)

        bound[1][image]         = image

        bound[2][1]             = bottomLeft
        bound[2][2]             = upperLeft
        bound[2][3]             = upperRight
        bound[2][4]             = bottomRight

        JudgeQuandrant(bound)
    end
end

--判断象限
function JudgeQuandrant(bound,middleX,middleY)
    for k,p in ipairs(bound) do 
        if p.x >= middleX & p.y >= middleY then
            table.insert(firstQuadrantUI,bound)
        elseif p.x >= middleX & p.y < middleY then
            table.insert(fourthQuadrantUI,bound)            
        elseif p.x < middleX & p.y >= middleY then            
            table.insert(secondQuadrantUI,bound)
        elseif p.x < middleX & p.y < middleY then            
            table.insert(thirdQuadrantUI,bound)
        end
    end
end--]]
function SendProto(protoName,args,successEvent)
    utils:StartCoroutine(function() 
        local future, data = network:SendAndWait("ETModel."..protoName, args)
        if future:isRejected() then         
            Debug.LogError("ETModel."..protoName.." failed");
            return;
        end
        successEvent()
    end)
end

return UIBase

