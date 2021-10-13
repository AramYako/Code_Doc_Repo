# Functions
Functions we can use in bicep 

### Resource group functions
```
- resourceGroup().location
- resourceGroup().id
```

### Unique names
```
- param solutionName string = 'toyhr${uniqueString(resourceGroup().id)}'
uniqueString will create unique but always same value if a template is deployed to same resource group and subscriptionkey
```