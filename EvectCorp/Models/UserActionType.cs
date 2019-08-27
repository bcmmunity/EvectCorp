namespace EvectCorp.Models
{
    public enum Actions
    {
        None,
        
        WaitingForEventCode,
        WaitingForValidationCode,
        AllEventsChangePage,
        
        DeleteOrNot,
        
        Profile, 
        
        FirstQuestion,
        
        SecondQuestion,
        
        ThirdQuestion,
        
        AddingParentTag,
        
        ChoosingTags,
        
        Networking,

        #region private info
        WaitingForName,
        WainingForEmail,
        #endregion
        
        #region AdminActions
        AdminMode,
        GetInformationAboutTheEvent,
        AddNewInformationAboutEvent,
        EditInformationAboutEvent,
        CreateNotification,

        #endregion

    }
}