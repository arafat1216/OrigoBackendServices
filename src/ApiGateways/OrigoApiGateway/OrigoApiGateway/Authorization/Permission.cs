namespace OrigoApiGateway.Authorization
{
    public enum Permission
    {
        //User permissions
        CanCreateCustomer,
        CanReadCustomer,
        CanUpdateCustomer,
        CanDeleteCustomer,
        CanCreateAsset,
        CanReadAsset,
        CanUpdateAsset,
        CanDeleteAsset,
        CanCreateUser,
        //Product permissions
        BasicHardwareRepair,
        EmployeeAccess,
        DepartmentStructure,
        OnAndOffboarding,
        BuyoutAsset,
        AssetManagement,
        SubscriptionManagement,
        AssetBookValue,
        InternalAssetReturn
    }
}