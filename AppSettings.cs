using System;

public class AppSettings
{
    #region Serviços UUID
    //Service UUID
    public static string GetServiceUuid { get; } = "1c930003-d459-11e7-9296-b8e856369374";

    public static string ServiceUUID { get; } = "1c930002-d459-11e7-9296-b8e856369374";

    #endregion


    #region Characteristic Guids

    public static string ChBattery { get; } = "1c930038-d459-11e7-9296-b8e856369374";

    public static string ChNotifyVib { get; } = "1c930020-d459-11e7-9296-b8e856369374";
    #endregion

}
