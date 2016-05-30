'use strict';
angular.module('dashboardAuth')
    .factory('dashboardAuthMiddlewareInterceptor', ['$q', '$injector', '$location',
    function ($q, $injector, $location) {
        var authInterceptorServiceFactory = {};
        
        authInterceptorServiceFactory.request = function (config) {

            //  do not intercept calls for token
            if (config.url === '/token') return config;

            var authService = $injector.get('authService');
            config.headers = config.headers || {};

            var authorizationHeader = authService.getAuthorizationHeader();

            if (authService.isAuthenticated()) {
                if (authorizationHeader) {
                    config.headers.Authorization = authorizationHeader;
                }
            } else {
                if (authorizationHeader) {
                    var deffered = $q.defer();
                    authService.loginWithRefreshToken()
                        .then(function (authorizationData) {
                            config.headers.Authorization = authService.getAuthorizationHeader(authorizationData);
                            return deffered.resolve(config);
                        },
                        function () {
                            return deffered.resolve(config);
                        });

                    return deffered.promise;
                }
            }

            return config;
        }

        authInterceptorServiceFactory.responseError = function (rejection) {

            if (rejection.status === 401 ||
                     (rejection.status === 400 && rejection.config.url === '/token') // refresh token failed or has been removed
                ) {
                var authService = $injector.get('authService');
                authService.logOut(false);
                $location.path('/login');
            }

            if (rejection.status === 403) {
                //notificationService.addError("Unauthorized", "You do not have permission to access this resource");
                $location.path('/home');
            }

            return $q.reject(rejection);
        }

        return authInterceptorServiceFactory;

    }]);