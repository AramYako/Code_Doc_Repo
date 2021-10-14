
@minLength(3)
@maxLength(24)
@description('Provide a name for the storage account. Use only lower case letters. The name must be unique across Azure.')
param storageAccountName string
@minLength(3)
@maxLength(24)
param resourceGroupLocation string = resourceGroup().location
@minLength(3)
@maxLength(24)
@description('Provide a name for the function app.')
param appName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the grid topic.')
param eventGridTopicName string
@description('Function app name')
param functionAppName string = appName
@minLength(3)
@maxLength(24)
@description('Provide a name for the application insight.')
param appInsightsName string = appName
@minLength(3)
@maxLength(24)
@description('Provide a name for the hosting plan.')
param hostingPlanName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the resource group name where the hosting plan to be hosted.')
param hostingPlanResourceGroup string
@minLength(3)
@maxLength(24)
@allowed([
  'Test'
  'Qa'
  'Prod'
])
param environment string
@minLength(3)
@maxLength(24)
@description('Provide a name for the keyvault.')
param keyVaultName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the cosmos account.')
param cosmosAccountName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the container.')
param cosmosContainerName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the cosmos database plan.')
param cosmosDatabaseName string
@description('Provide throughput value for cosmos.')
param provisionedThroughput int
@minLength(3)
@maxLength(24)
@description('Provide a name for a cosmos container.')
param ServiceDataChangeFeedCollectionName string
@minLength(3)
@maxLength(24)
@description('Provide a name for the blob container.')
param ArchiveBlobContainerName string
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccount.listKeys().keys[0].value}'
@minLength(3)
@maxLength(24)
@description('Provide a name for the cosmos consistency level.')
param cosmosConsistencyLevel string = 'Eventual'
@minLength(3)
@maxLength(24)
@description('Provide a name for the throughput policy.')
@allowed([
  'Autoscale'
  'Manual'
])
param throughputPolicy string = 'Autoscale'
@allowed([
  'Standard_RAGRS'
])
@minLength(3)
@maxLength(24)
param storageSKU string = 'Standard_RAGRS'
param deployStorageAccount bool = true
var throughput_Policy = {
  Manual: {
    Throughput: provisionedThroughput
  }
  Autoscale: {
    autoscaleSettings: {
      maxThroughput: provisionedThroughput
    }
  }
}

//Module
module appService 'appServiceModule.bicep' = {
  name: 'appService'
  params: {
    location: resourceGroup().location
    appServiceAppName: appName
    environmentType: environment
  }
}

//Module have output, and we can take this value 
output appServiceAppHostName string = appService.outputs.appServiceAppHostName


//If statement
resource storageAccountDemo 'Microsoft.Storage/storageAccounts@2021-01-01' = if (deployStorageAccount) {
  name: 'teddybearstorage'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku:{
     name: 'Standard_LRS'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: resourceGroupLocation
  sku: {
    name: storageSKU
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: false
    encryption: {
      services: {
        file: {
          enabled: true
        }
        blob: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  name: '${storageAccountName}/default'
  dependsOn: [
    storageAccount
  ]
  properties: {
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  kind: 'web'
  location: resourceGroupLocation
  properties: {
    Application_Type: 'web'
  }
}

resource eventGridTopic 'Microsoft.EventGrid/topics@2020-06-01' = {
  name: eventGridTopicName
  location: resourceGroupLocation
}

resource azurefunction 'Microsoft.Web/sites@2021-01-15' = {
  name: functionAppName
  location: resourceGroupLocation
  kind: 'functionapp'
  dependsOn: [
    appInsights
    storageAccount
    eventGridTopic
  ]
  properties: {
    serverFarmId: resourceId(hostingPlanResourceGroup, 'Microsoft.Web/serverfarms', hostingPlanName)
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageConnectionString
        }
        {
          name: 'AzureWebJobsDashboard'
          value: storageConnectionString
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'Environment'
          value: environment
        }
        {
          name: 'azureKeyVault'
          value: keyVaultName
        }
        {
          name: 'ServiceDataDatabase'
          value: cosmosDatabaseName
        }
        {
          name: 'ServiceDataCollection'
          value: cosmosContainerName
        }
        {
          name: 'ServiceDataChangeFeedCollectionName'
          value: ServiceDataChangeFeedCollectionName
        }
        {
          name: 'ArchiveBlobContainer'
          value: ArchiveBlobContainerName
        }
      ]
    }
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' = {
  name: cosmosAccountName
  location: resourceGroupLocation
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: cosmosConsistencyLevel
    }
    locations: [
      {
        locationName: resourceGroupLocation
      }
    ]
    databaseAccountOfferType: 'Standard'
  }
}

resource cosmoSqlDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  name: '${cosmosAccountName}/${cosmosDatabaseName}'
  properties: {
    resource: {
      id: cosmosDatabaseName
    }
  }
  dependsOn: [
    cosmosAccount
  ]
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-04-15' = {
  name: '${cosmosAccountName}/${cosmosDatabaseName}/${cosmosContainerName}'
  properties: {
    resource: {
      id: cosmosContainerName
      partitionKey: {
        paths: [
          '/vin'
        ]
      }
    }
    options: throughput_Policy[throughputPolicy]
  }
  dependsOn: [
    cosmoSqlDatabase
  ]
}

resource keyValutStorage 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: resourceGroupLocation
  properties: {
    tenantId: subscription().tenantId
    enabledForTemplateDeployment: true
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: azurefunction.identity.principalId
        permissions: {
          secrets: [
            'list'
            'get'
          ]
        }
      }
    ]
    sku: {
      family: 'A'
      name: 'standard'
    }
    createMode: 'recover'
  }
  dependsOn: [
    azurefunction
  ]
}

resource keyValutSecretsStorageAccount 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${keyVaultName}/StorageConnectionString'
  properties: {
    value: storageConnectionString
    attributes: {
      enabled: true
    }
  }
  dependsOn: [
    keyValutStorage
    storageAccount
  ]
}

resource keyValutSecretsCosmosConnection 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${keyVaultName}/cosmosDbKey'
  properties: {
    value: cosmosAccount.listKeys().primaryMasterKey
  }
  dependsOn: [
    keyValutStorage
    cosmosAccount
  ]
}

resource keyValutSecretsEventGridConnection 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${keyVaultName}/ServiceDataEventGridTopicKey'
  properties: {
    value: eventGridTopic.listKeys().primaryMasterKey
  }
  dependsOn: [
    keyValutStorage
    eventGridTopic
  ]
}
