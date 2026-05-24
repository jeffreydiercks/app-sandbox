@description('Location for all resources.')
param location string = resourceGroup().location

@description('Base name used to derive resource names.')
param appName string = 'appsandbox'

@description('Environment suffix (e.g. dev, prod).')
param environment string = 'dev'

var planName = 'plan-${appName}-${environment}'
var siteName = 'app-${appName}-apphub-${environment}'
var myVersesSiteName = 'app-${appName}-myverses-${environment}'
var cosmosAccountName = 'cosmos-${appName}-${environment}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  properties: {
    reserved: false
  }
}

resource appHubSite 'Microsoft.Web/sites@2023-12-01' = {
  name: siteName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v10.0'
      alwaysOn: false
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      appSettings: [
        { name: 'AzureAd__TenantId', value: 'consumers' }
        { name: 'AzureAd__ClientId', value: 'c2e2687b-af23-4e02-94c4-ec2b997a129a' }
      ]
    }
  }
}

resource myVersesSite 'Microsoft.Web/sites@2023-12-01' = {
  name: myVersesSiteName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v10.0'
      alwaysOn: false
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      appSettings: [
        { name: 'AzureAd__TenantId', value: 'consumers' }
        { name: 'AzureAd__ClientId', value: 'c2e2687b-af23-4e02-94c4-ec2b997a129a' }
        { name: 'ConnectionStrings__cosmos', value: cosmosAccount.properties.documentEndpoint }
      ]
    }
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableFreeTier: true
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  parent: cosmosAccount
  name: 'myverses'
  properties: {
    resource: {
      id: 'myverses'
    }
  }
}

output appHubUrl string = 'https://${appHubSite.properties.defaultHostName}'
output appHubName string = appHubSite.name
output myVersesUrl string = 'https://${myVersesSite.properties.defaultHostName}'
output myVersesName string = myVersesSite.name
output cosmosAccountName string = cosmosAccount.name
output planName string = appServicePlan.name
