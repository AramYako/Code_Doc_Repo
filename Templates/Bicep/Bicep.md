

[![](https://shell.azure.com/images/launchcloudshell.png "Launch Azure Cloud Shell")](https://shell.azure.com)

# Deployment of an ARM tempplate

### Arm template can be deployed to 
```
Resource group
Subscription
Management Group
Tenant
```

### ARM Template deployment tools
```
Azure CLI 
Azure Powershell
```

### Process 
```
- Powershell/Azure CLI invoked
- Call Bicep CLI 
- Convert Bicep to ARM json
- Call Azure Resource manager
```

### Good extensions in VS Code 
```
- Bicep
```
### param types
```
string
int
bool
object
array
```

### string interpolation
```
- param storageAccountName string = 'toylaunch${uniqueString(resourceGrou ().id)}'

```


## Installation Instructions

### Bicep with Azure CLI
```
- az bicep install
- az bicep upgrade
- az bicep version
```

### Bicep with Powershell
```
- choco install bicep 
- choco upgrade bicep
```

