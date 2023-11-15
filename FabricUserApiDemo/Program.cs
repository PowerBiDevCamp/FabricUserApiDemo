using FabricUserApiDemo.Services;

string WorkspaceName = "Customer Tenant 01";


// Demo 01
CustomerTenantBuilder.CreateCustomerTenant(WorkspaceName);

// Demo 02
// CustomerTenantBuilder.CreateCustomerTenantWithUsers(WorkspaceName);

// Demo 03
// CustomerTenantBuilder.CreateCustomerTenantWithImportedSalesModel(WorkspaceName);

// Demo 04
// CustomerTenantBuilder.CreateCustomerTenantWithDirectLakeSalesModel(WorkspaceName);

// Demo 05
// FabricUserApi.ExportItemDefinitionsFromWorkspace(WorkspaceName);