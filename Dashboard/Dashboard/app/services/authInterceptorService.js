﻿'use strict';
app.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService', 'notificationService',
    function ($q, $injector, $location, localStorageService, notificationService) {
        var authorizationDataKey = 'authorizationData';
        var authInterceptorServiceFactory = {};

        var request = function (config) {

            config.headers = config.headers || {};

            var authData = localStorageService.get(authorizationDataKey);
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }

            return config;
        }

        var responseError = function (rejection) {
            if (rejection.status === 401) {
                var authService = $injector.get('authService');
                var authData = localStorageService.get(authorizationDataKey);

                if (authData) {
                    if (authData.useRefreshTokens) {
                        $location.path('/refresh');
                        return $q.reject(rejection);
                    }
                }

                authService.logOut();
                $location.path('/login');
            }

            if (rejection.status === 403) {
                notificationService.addNotification("Unauthorized", "You do not have permission to access this resource");
                $location.path('/home');
            }

            return $q.reject(rejection);
        }

        authInterceptorServiceFactory.request = request;
        authInterceptorServiceFactory.responseError = responseError;

        return authInterceptorServiceFactory;
    }]);