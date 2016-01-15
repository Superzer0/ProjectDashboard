'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService',
    function ($http, $q, localStorageService) {

        var clientId = 'dummy';
        var authServiceFactory = {};

        var authentication = {
            isAuth: false,
            userName: "",
            useRefreshTokens: false
        };

        var externalAuthData = {
            provider: "",
            userName: "",
            externalAccessToken: ""
        };

        var saveRegistration = function (registration) {

            logOut();

            return $http.post('/api/account/register', registration).then(function (response) {
                return response;
            });

        };

        var login = function (loginData) {

            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

            if (loginData.useRefreshTokens) {
                data = data + "&client_id=" + clientId;
            }

            var deferred = $q.defer();

            $http.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                if (loginData.useRefreshTokens) {
                    localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
                }
                else {
                    localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
                }
                authentication.isAuth = true;
                authentication.userName = loginData.userName;
                authentication.useRefreshTokens = loginData.useRefreshTokens;

                deferred.resolve(response);

            }).error(function (err, status) {
                logOut();
                deferred.reject(err);
            });

            return deferred.promise;

        };

        var logOut = function () {

            var deffered = $q.defer();

            $http.post("/api/account/logoff", {}).success(function (response) {

                localStorageService.remove('authorizationData');
                authentication.isAuth = false;
                authentication.userName = "";
                authentication.useRefreshTokens = false;
                deffered.resolve(response);

            }).error(function (err, status) {
                deffered.reject(err);
            });

            return deffered.promise;
        };

        var changePass = function (oldPass, newPass) {

            return $http.post("/api/account/changePass", { oldPassword: oldPass, newPassword: newPass });
        }

        var fillAuthData = function () {

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                authentication.isAuth = true;
                authentication.userName = authData.userName;
                authentication.useRefreshTokens = authData.useRefreshTokens;
            }

        };

        var refreshToken = function () {
            var deferred = $q.defer();

            var authData = localStorageService.get('authorizationData');

            if (authData) {

                if (authData.useRefreshTokens) {

                    var data = "grant_type=refresh_token&refresh_token=" +
                        authData.refreshToken + "&client_id=" + clientId;

                    localStorageService.remove('authorizationData');

                    $http.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                        localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true });

                        deferred.resolve(response);

                    }).error(function (err, status) {
                        logOut();
                        deferred.reject(err);
                    });
                }
            }

            return deferred.promise;
        };

        var obtainAccessToken = function (externalData) {

            var deferred = $q.defer();

            $http.get('/api/account/ObtainLocalAccessToken',
                { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } })
                .success(function (response) {

                    localStorageService.set('authorizationData',
                        { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

                    authentication.isAuth = true;
                    authentication.userName = response.userName;
                    authentication.useRefreshTokens = false;

                    deferred.resolve(response);

                }).error(function (err, status) {
                    logOut();
                    deferred.reject(err);
                });

            return deferred.promise;

        };

        authServiceFactory.saveRegistration = saveRegistration;
        authServiceFactory.login = login;
        authServiceFactory.logOut = logOut;
        authServiceFactory.fillAuthData = fillAuthData;
        authServiceFactory.authentication = authentication;
        authServiceFactory.refreshToken = refreshToken;
        authServiceFactory.changePass = changePass;
        authServiceFactory.obtainAccessToken = obtainAccessToken;
        authServiceFactory.externalAuthData = externalAuthData;

        return authServiceFactory;
    }]);