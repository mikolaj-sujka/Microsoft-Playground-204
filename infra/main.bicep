targetScope = 'resourceGroup'

param location string = resourceGroup().location
param appName string = 'microsoft-playground'
param appBaseUrl string
param appConfigName string = ''
param alertEmail string = ''
param failedRequestsThreshold int = 5
param availabilityFailedLocationCount int = 2

var normalizedAppName = toLower(replace(appName, ' ', '-'))
var appInsightsName = 'appi-${normalizedAppName}'
var workspaceName = 'log-${normalizedAppName}'
var availabilityTestName = 'wt-${normalizedAppName}-health'
var actionGroupName = 'ag-${normalizedAppName}'

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
    IngestionMode: 'LogAnalytics'
    RetentionInDays: 30
    SamplingPercentage: 100
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource availabilityTest 'Microsoft.Insights/webtests@2022-06-15' = {
  name: availabilityTestName
  location: location
  kind: 'standard'
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    Name: availabilityTestName
    Description: 'GET /health availability test'
    Enabled: true
    Frequency: 300
    Kind: 'standard'
    Locations: [
      {
        Id: 'emea-nl-ams-azr'
      }
      {
        Id: 'us-tx-sn1-azr'
      }
      {
        Id: 'apac-hk-hkn-azr'
      }
    ]
    Request: {
      FollowRedirects: true
      HttpVerb: 'GET'
      ParseDependentRequests: false
      RequestUrl: '${appBaseUrl}/health'
    }
    RetryEnabled: true
    SyntheticMonitorId: availabilityTestName
    Timeout: 30
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      SSLCheck: true
    }
  }
}

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'global'
  properties: {
    enabled: true
    groupShortName: 'msplay'
    emailReceivers: empty(alertEmail) ? [] : [
      {
        name: 'main-email'
        emailAddress: alertEmail
        useCommonAlertSchema: true
      }
    ]
  }
}

var alertActions = [
  {
    actionGroupId: actionGroup.id
  }
]

resource availabilityAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'ma-${normalizedAppName}-availability'
  location: 'global'
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
    'hidden-link:${availabilityTest.id}': 'Resource'
  }
  properties: {
    description: 'Alert when /health availability test fails from multiple locations.'
    severity: 1
    enabled: true
    scopes: [
      availabilityTest.id
      appInsights.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
      webTestId: availabilityTest.id
      componentId: appInsights.id
      failedLocationCount: availabilityFailedLocationCount
    }
    actions: alertActions
  }
}

resource failedRequestsAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'ma-${normalizedAppName}-failed-requests'
  location: 'global'
  properties: {
    description: 'Alert when failed requests are above the configured threshold in 5 minutes.'
    severity: 2
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'FailedRequests'
          metricNamespace: 'Microsoft.Insights/components'
          metricName: 'requests/failed'
          operator: 'GreaterThan'
          threshold: failedRequestsThreshold
          timeAggregation: 'Count'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
    }
    actions: alertActions
  }
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = if (!empty(appConfigName)) {
  name: appConfigName
}

output applicationInsightsConnectionString string = appInsights.properties.ConnectionString
output applicationInsightsName string = appInsights.name
output appConfigEndpoint string = empty(appConfigName) ? '' : appConfig!.properties.endpoint
