local UIManager = {}
local OpenUIList = {}
local currentTopUI

local UIType = require("ui.UIType")


function UIManager:OpenUI(typeName)
    local uiPath = UIType[typeName]    
    if OpenUIList[typeName] ~= nil then
        CS.UnityEngine.Debug.LogError("不能重复打开UI")
        return
    end
    assert(uiPath , "typename error")
    utils:OpenUI(uiPath, "ui.uiScripts."..typeName)
    OpenUIList[typeName] = typeName
end

function UIManager:OpenUIWithProto(typeName,protoName,args)
    local uiPath = UIType[typeName]    
    assert(uiPath , "typename error")
    if OpenUIList[typeName] ~= nil then
        CS.UnityEngine.Debug.LogError("不能重复打开UI")
        return
    end
    utils:StartCoroutine(function() 
        local future, data = network:SendAndWait("ETModel."..protoName, args)
        if future:isRejected() then         
            CS.UnityEngine.Debug.LogError("ETModel."..protoName.." failed")
            return
        end
        utils:OpenUI(uiPath, "ui.uiScripts."..typeName,data)
        OpenUIList[typeName] = typeName
    end)

end

function UIManager:CloseUI(uiSelf)
    uiSelf.formLogic:Close()
    OpenUIList[uiSelf.class.__cname] = nil
end


function UIManager:AddClickEvent(gameObject,clickEvent)
    gameObject:AddUIListener(EventTriggerType.PointerClick, function(d)
        clickEvent(d)
    end)
end

function UIManager:AddBeginDragEvent(gameObject,beginDragEvent)
    gameObject:AddUIListener(EventTriggerType.BeginDrag, function(d)
        beginDragEvent(d)
    end)
end

function UIManager:AddDragEvent(gameObject,dragEvent)
    gameObject:AddUIListener(EventTriggerType.Drag, function(d)
        dragEvent(d)
    end)
end

function UIManager:AddEndDragEvent(gameObject,endDragEvent)
    gameObject:AddUIListener(EventTriggerType.EndDrag, function(d)
        endDragEvent(d)
    end)
end


return UIManager