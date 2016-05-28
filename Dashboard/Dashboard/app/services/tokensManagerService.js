'use strict';

app.factory('tokensManagerService', ['$http', function ($http) {

    var tokenManagerServiceFactory = {};

    tokenManagerServiceFactory.getRefreshTokens = function () {
        return $http.get('api/manageApps/refreshTokens');
    };

    tokenManagerServiceFactory.deleteRefreshTokens = function (tokenid) {
        return $http.delete('api/manageApps/refreshTokens/' + tokenid);
    };

    tokenManagerServiceFactory.createApp = function (appModel) {
        return $http.post('api/manageApps/app/create', appModel);
    };

    tokenManagerServiceFactory.getClients = function () {
        return $http.get('api/manageApps/apps');
    };

    tokenManagerServiceFactory.deleteClient = function (appId) {
        return $http.delete('api/manageApps/apps/' + appId);
    };

    tokenManagerServiceFactory.changeClientStatus = function (appId, status) {
        return $http.post('api/manageApps/apps/status/' + appId + '/' + status);
    };

    tokenManagerServiceFactory.regenerateClientSecret = function (appId) {
        return $http.post('api/manageApps/apps/secret/regenerate/' + appId);
    };

    return tokenManagerServiceFactory;
}]);