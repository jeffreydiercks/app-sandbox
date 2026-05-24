@description('Location for all resources.')
param location string = resourceGroup().location

@description('Base name used to derive resource names.')
param appName string = 'appsandbox'

@description('Environment suffix (e.g. dev, prod).')
param environment string = 'dev'

var planName = 'plan-${appName}-${environment}'
var siteName = 'app-${appName}-apphub-${environment}'

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
    }
  }
}

output appHubUrl string = 'https://${appHubSite.properties.defaultHostName}'
output appHubName string = appHubSite.name
output planName string = appServicePlan.name
