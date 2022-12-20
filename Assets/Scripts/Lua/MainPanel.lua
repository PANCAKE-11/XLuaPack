  MainPanel={}

function MainPanel:Init()
    self.gameObject=GameObject.Find("MainPanel")
    if(self.gameObject==nil)then
        local  go=ResourcesManager.LoadGameObject("Prefabs/MainPanel")
        self.gameObject=GameObject.Instantiate(go,Canvas.transform)
        
    end
    self.gameObject:SetActive(false)
    self.btn_add=(self.gameObject.transform:Find('btn_add')):GetComponentButton()
    self.btn_pack=(self.gameObject.transform:Find('btn_pack')):GetComponentButton()
    self.btn_add.onClick:AddListener(function() self:ClickAddBtn() end )
    self.btn_pack.onClick:AddListener(function() self:ClickPackBtn() end )
    
end


function MainPanel:ClickAddBtn()
    PackPanel:AddItem('拉面',1)
end
function MainPanel:ClickPackBtn()
    if not self.packOpened then
        PackPanel:ShowMe()
        self.packOpened=true
    else
        PackPanel:HideMe()
        self.packOpened=false
    end
    
end


function MainPanel :ShowMe()
    self.gameObject:SetActive(true)
end
function MainPanel :HideMe()
    self.gameObject:SetActive(false)
end