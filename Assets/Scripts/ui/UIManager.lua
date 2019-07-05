local UIManager  = {}
local OpenUIList = {}
local OpenUIId   = {}
local currentTopUI

local UIType = require("ui.UIType")

local spriteRootPath = "Assets/Sprites/"
local prefabRootPath = ""


function UIManager:OpenUI(typeName,data)
    local uiPath = UIType[typeName]    
    if OpenUIList[typeName] ~= nil then
        CS.UnityEngine.Debug.LogError("不能重复打开UI")
        return
    end
    assert(uiPath , "typename:["..typeName.."] error")
    local id = utils:OpenUI(uiPath, "ui.uiScripts."..typeName,data)
    OpenUIId[typeName]   = id
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
        local id = utils:OpenUI(uiPath, "ui.uiScripts."..typeName,data)
        OpenUIList[typeName] = typeName
        OpenUIId[typeName]   = id
    end)

end

function UIManager:SendProto(protoName,args,successEvent,failEvent)
    utils:StartCoroutine(function() 
        local future, data = network:SendAndWait("ETModel."..protoName, args)
        if future:isRejected() then         
            if failEvent and typeof(failEvent) == "function" then failEvent() end
            CS.UnityEngine.Debug.LogError("ETModel."..protoName.." failed")
            return
        end
        if successEvent  then successEvent(data) end
    end)
end


function UIManager:GetUIWithTypeName(typeName)

    assert(OpenUIList[typeName] , "["..typeName.."]不存在")

    return utils:GetUILuaTabel(OpenUIId[typeName])
end

function UIManager:CloseUI(uiSelf)
    uiSelf.formLogic:Close()
    assert(OpenUIList[uiSelf.__cname],"class name:["..uiSelf.__cname.."] error")
    OpenUIList[uiSelf.__cname]  = nil
    OpenUIId[uiSelf.__cname]    = nil
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