namespace Common.Enums;

public enum OrigoErrorCodes
{
    CustomerReferenceFieldMissing = 10000,
    CustomerSettingsError = 10001,
    InvalidOperatorId = 10002,
    InvalidPhoneNumber = 10003,
    InvalidSimCard = 10004,
    //Asset
    AssetCategoryNotFound = 20000,
    AssetInvalidData = 20001,
    InvalidImei = 20002,
    InvalidMacAddress = 20003,
    ExpiredError = 20004,
    ExpiresSoonError = 20005,
    ReturnDeviceError = 20006,
    BuyoutDeviceError = 20007,
    InvalidOperationForInactiveState = 20008,
    InvalidLifecycleType = 20009,
    //
    ResourceNotFound = 20010,
}