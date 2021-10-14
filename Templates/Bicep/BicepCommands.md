# Bicep commands with Bicep CLI


### Bicep to Json
```
bicep build ./bicepTemplate.bicep
```
### JSON to Bicep
```
bicep decompile ./template.json
``` 
### Deploy
```
- az login
 ### Check which is current active subscription
- az account show --output table
### Set current active subscription if needed
- az account set --subscription <name or id>
### --What-if
- az deployment group create -g servicedata-logic-qa -f ./azuredeploy.bicep -p @azuredeploy-test.parameters.json --what-if
###Deploy
- az deployment group create -g servicedata-logic-qa -f ./azuredeploy.bicep -p @azuredeploy-test.parameters.json
```

### List deployment at a rg 
```
- az deployment group list --resource-group
```

### Create output
- output appServiceAppName string = appServiceAppName
Obs: Important dont output connectionstring, secret stuff like that

### Query for output paramters 
```
az deployment group show -g <resource-group-name> -n <deployment-name> --query properties.outputs
```

### If statement
```
- resource storageAccountDemo 'Microsoft.Storage/storageAccounts@2021-01-01' = if (deployStorageAccount) {
- resource storage 'Microsoft.Storage/storageAccounts@2021-01-01' = if (deployStorage =='prodiction')
- environmentName == 'Production' ? auditStorageAccount.properties.primaryEndpoints.blob : ''
```

### For Loop
```
param storageAccountNames array = [
  'saauditus'
  'saauditeurope'
  'saauditapac'
]

resource storageAccountResources 'Microsoft.Storage/storageAccounts@2021-01-01' = [for storageAccountName in storageAccountNames: {
  name: storageAccountName
}]
```

### For Loop based on range
```
When you use the range() function, you specify its start value and the number of values you want to create. For example, if you want to create storage accounts with the names sa0, sa1, and sa2, you would use the function range(0,3).

resource storageAccountResources 'Microsoft.Storage/storageAccounts@2021-01-01' = [for i in range(1,4): {
  name: 'sa${i}'
}]

```

### For loop and if statement
```
resource sqlServers 'Microsoft.Sql/servers@2020-11-01-preview' = [for sqlServer in sqlServerDetails: if (sqlServer.environmentName == 'Production') {
  name: sqlServer.name
}]
```
