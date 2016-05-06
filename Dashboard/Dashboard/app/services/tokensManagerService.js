'use strict';
app.factory('tokensManagerService', ['$http', function ($http) {

    var tokenManagerServiceFactory = {};

    var _getRefreshTokens = function () {

        return $http.get('api/refreshtokens').then(function (results) {
            return results;
        });
    };

    var _deleteRefreshTokens = function (tokenid) {

        return $http.delete('api/refreshtokens/?tokenid=' + tokenid).then(function (results) {
            return results;
        });
    };

    var _createApp = function(appModel) {
        return $http.post('api/refreshtokens/createapp', appModel);
    };

    var _getClients = function () {
        return $http.get('api/refreshtokens/apps');
    };

    tokenManagerServiceFactory.deleteRefreshTokens = _deleteRefreshTokens;
    tokenManagerServiceFactory.getRefreshTokens = _getRefreshTokens;
    tokenManagerServiceFactory.createApp = _createApp;
    tokenManagerServiceFactory.getClients = _getClients;

    return tokenManagerServiceFactory;
}]);