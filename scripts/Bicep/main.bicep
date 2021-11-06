@description('bot name')
param botName string

@description('the registerd application secret')
@secure()
param msAppPassword string

var uniqueSuffix = toLower(substring(uniqueString(resourceGroup().id, 'Microsoft.BotService/bots', botName), 0, 6))
var keyVaultName = 'bot-secrets-${toLower(substring(uniqueString(resourceGroup().id, resourceGroup().location, 'Microsoft.BotService/bots/keyVaults'), 0, 6))}'
var appPasswordSecret = 'bot-${replace(botName, '_', '-')}-pwd-${uniqueSuffix}'
var identityResourceID = resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', botName)
var appPasswordSecretId = resourceId('Microsoft.KeyVault/vaults/secrets', keyVaultName, appPasswordSecret)

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
  }

  resource secret 'secrets' = {
    name: appPasswordSecret
    properties: {
      value: msAppPassword
    }
    dependsOn:[
      keyVault
    ]
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: botName
  location: resourceGroup().location
  sku: {
    tier: 'Shared'
    name: 'D1'
  }
}

resource appInsight 'Microsoft.Insights/components@2020-02-02' = {
  name: botName
  location: resourceGroup().location
  kind: botName

  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    IngestionMode: 'ApplicationInsights'
    Request_Source: 'rest'

  }
}

resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  name: botName
  location: resourceGroup().location

  dependsOn: [
    appInsight
    appServicePlan
  ]

  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', botName)
    siteConfig: {
      appSettings: [
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'default'
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: reference('microsoft.insights/components/${botName}').ConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference('microsoft.insights/components/${botName}').InstrumentationKey
        }
      ]
      netFrameworkVersion: 'v5.0'
    }

  }
}

resource assignMSI 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: botName
  location: resourceGroup().location
  
}

resource bot 'Microsoft.BotService/botServices@2021-05-01-preview' = {
  name: botName
  sku:{
    name: 'F0'
  }
  dependsOn: [
    assignMSI
    webApp
  ]
  properties: {
    displayName: botName
    msaAppId: reference(identityResourceID).clientId
    msaAppTenantId: reference(identityResourceID).tenantId
    msaAppMSIResourceId: identityResourceID
    appPasswordHint: appPasswordSecretId
    endpoint: 'https://${botName}.azurewebsites.net/api/test'
  }
}
