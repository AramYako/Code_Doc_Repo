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
### List deployment at a rg 
```