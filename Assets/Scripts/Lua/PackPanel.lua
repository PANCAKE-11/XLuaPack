PackPanel={}

function PackPanel:Init()

    self.gameObject=GameObject.Find("PackPanel")
    if(self.gameObject==nil)then
        local  go=ResourcesManager.LoadGameObject("Prefabs/PackPanel")
        self.gameObject=GameObject.Instantiate(go,Canvas.transform)
    end
    self.gameObject:SetActive(false)
    self.slots= {}
    self.slotsTrans=self.gameObject.transform:Find('slots')
    self.itemNum= self.slotsTrans.childCount
    for i = 1, self.itemNum do 
        self.slots[i]=self.slotsTrans:GetChild(i-1)
    end
    self.itemPrefab =ResourcesManager.LoadGameObject("Prefabs/item")
    self.save=self:Load()
    self.tipTrans=self.gameObject.transform:Find('tip')
    --self.tipTrans.parent=self.gameObject.transform.parent
    self.tipTrans.gameObject:SetActive(false)
    self:Refresh()
end 



function PackPanel:AddItem(name ,num)
  --已有物品
    for k,v in ipairs(self.save)do
        if(v["num"]>0 and  v['name']==name  )then
            v['num']= v['num']+num
            self:Refresh()
            return
        end
    end
    --没有同名的
    for k,v in ipairs(self.save)do
        if( v ["num"]==0)then
            v ['name']=name
            v ['num']=num
            break
        end
    end
    self:Refresh()
   
    return
end
function PackPanel:Refresh()
    for k ,v in ipairs(self.slots) do
        if(v.childCount>0) then
            Object.Destroy(v:GetChild (0).gameObject);
        end
    end
    for i,v in ipairs(self.save)do
        local item=self.save[i]
        if(item['num']>0)then
            self:IniItem(item['name'], item['num'],i)
        end
    end

end
function PackPanel:IniItem(itemName, itemNum, index)
    itemName=itemName or ""
    itemNum= itemNum or 0
    local v=self.slots[index]
    local item=  GameObject.Instantiate(self.itemPrefab,v).transform
    local img= item:GetComponentImage()
    local txt= item:GetTextComponentInChildren()
    img.sprite= ResourcesManager.LoadSprite("Sprites/items/" .. itemName)
    txt.text=string.format("%02d",itemNum)
    self:RegisterDragEvent(item)


end
function PackPanel:Save()
    ResourcesManager.SavePlayerPack(rapidjson.encode(self.save))
end
function PackPanel:Load()
    local s=   ResourcesManager.LoadPlayerPack()
    local save
    if s==nil then
        save={}
        for i = 1,  self.itemNum do
            self.save[i]={}
            self.save[i]['name']='null'
            self.save[i]['num']=0
        end
    else
        save=rapidjson.decode(s)
    end
    
   return save
end
--拖拽
function PackPanel:RegisterDragEvent(trans)
    local dragable=trans:GetDragableComponentInChildren()
    dragable:onBeginDragEvent('+',function(eventData,transform) self:OnBeginDrag(eventData,transform) end)
    dragable:onDragEvent('+',function(eventData,transform) self:OnDrag(eventData,transform) end)
    dragable:onEndDragEvent('+',function(eventData,transform) self:OnEndDrag(eventData,transform) end)
    dragable:onPointerEnterEvent('+',function(eventData,transform) self:OnPointerEnter(eventData,transform) end)
    dragable:onPointerExitEvent('+',function(eventData,transform) self:OnPointerExit(eventData,transform) end)
end

function PackPanel:OnBeginDrag(eventData, transform)
    self.lastSlotIndex=self:GetSlotIndex(transform.parent) 
    transform:GetComponentImage().raycastTarget=false
    transform.parent=self.slotsTrans
end
function PackPanel:OnDrag(eventData, transform)
    transform.position=Vector3(transform.position.x+eventData.delta.x,transform.position.y+eventData.delta.y,transform.position.z)
   
end
function PackPanel:OnEndDrag(eventData, transform)
    local nextSlotIndex=self.lastSlotIndex
    if(EventSystem.current:IsPointerOverGameObject())then
        local raycastResults = CS.System.Collections.Generic["List`1[UnityEngine.EventSystems.RaycastResult]"]()
        EventSystem.current:RaycastAll(eventData, raycastResults);

        if(raycastResults[0].gameObject.name=='slot')then
            nextSlotIndex=self:GetSlotIndex(raycastResults[0].gameObject.transform)

        elseif (raycastResults[0].gameObject.name=='Item(Clone)') then

            local  slot=raycastResults[0].gameObject.transform.parent
            nextSlotIndex=self:GetSlotIndex(slot)

        end
        local lastSave=self.save[self.lastSlotIndex]
        local nextSave=self.save[nextSlotIndex]

        local temp={}
        temp['name']=lastSave['name']
        temp['num']=lastSave['num']

        lastSave['name']=nextSave['name']
        lastSave['num']=nextSave['num']

        nextSave['name']= temp['name']
        nextSave['num']= temp['num']
      
    else --在背包外
        local lastSave=self.save[self.lastSlotIndex]
        
        lastSave['num']=0
    end
 
    Object.Destroy(transform.gameObject)
    self:Refresh()
end
function PackPanel:OnPointerEnter(eventData,transform)
    self.tipTrans.gameObject:SetActive(true)
    local rect=  self.tipTrans.rect
    local index=self:GetSlotIndex(transform.parent)
    self.tipTrans.position  = Vector3(eventData.position.x,eventData.position.y, self.tipTrans.position.z)
    self.tipTrans:GetTextComponent().text=self.save[index]["name"]
end
function PackPanel:OnPointerExit(eventData)
    self.tipTrans.gameObject:SetActive(false)
end
function PackPanel:GetSlotIndex(slot)
    for i=1,self.itemNum do
        if self.slots[i] == slot then
            return i
        end
    end
    return nil
end




function PackPanel :ShowMe()
    self.gameObject:SetActive(true)
end
function PackPanel :HideMe()
    self.gameObject:SetActive(false)
end
return PackPanel