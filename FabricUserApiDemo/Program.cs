using FabricUserApiDemo.Services;

string WorkspaceName = "Acme Corp";

// Demo 01
CustomerTenantBuilder.CreateCustomerTenant(WorkspaceName);

// Demo 02
// CustomerTenantBuilder.CreateCustomerTenantWithUsers(WorkspaceName);

// Demo 03
// CustomerTenantBuilder.CreateCustomerTenantWithImportedSalesModel(WorkspaceName);

// Demo 04
// CustomerTenantBuilder.CreateCustomerTenantWithDirectLakeSalesModel(WorkspaceName);

// Demo 05
// CustomerTenantBuilder.CreateCustomerTenantWithSparkJobDefinition(WorkspaceName);

// Demo 06
//FabricUserApi.ExportItemDefinitionsFromWorkspace(WorkspaceName);