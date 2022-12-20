print('lua 开始……')

GameObject = CS.UnityEngine.GameObject
ResourcesManager=CS.ResourcesManager
Canvas=GameObject.Find("Canvas")
Quaternion=CS.UnityEngine.Quaternion
Vector3=CS.UnityEngine.Vector3
rapidjson = require('rapidjson')
Time=CS.UnityEngine.Time
Input=CS.UnityEngine.Input
PackPanel=require('PackPanel')
MainPane=require('MainPanel')
Player=require('Player')
require('UtilTools')
Camera=CS. UnityEngine.Camera
MAINCAMERA=Camera.main
Object=CS.UnityEngine.Object
EventSystem=CS. UnityEngine.EventSystems.EventSystem
UpdateFunc={}
function awake()
    PackPanel:Init()
    MainPanel:Init()
    Player:Init()
    MainPanel:ShowMe()
end 

function update()
    for k,v in pairs(UpdateFunc) do
        v()

    end
end
function RegisterUpdate(func)
    table.insert(UpdateFunc,func)
end

