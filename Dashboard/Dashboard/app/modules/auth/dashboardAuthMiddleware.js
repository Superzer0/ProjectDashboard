"use strict";
angular.module('dashboardAuth', ['ngRoute', 'LocalStorageModule'])
    .factory('authService', ['$http', '$q', 'localStorageService', 'dashboardRoles', 'clientId',
    function ($http, $q, localStorageService, roles, clientId) {
        var authorizationDataKey = 'authorizationData';
        var userProfileDataKey = 'userProfileData';
        var authServiceFactory = {};
        authServiceFactory.dashboardRoles = angular.copy(roles);
        var userProfileData = null;

        var onUserProfileChanged = function () { }

        var authData = {
            isAuth: false,
            userName: "",
            tokenExipreTime: new Date().getTime() - 1
        };

        //TODO: this sevice should be splited into smaller ones

        //------------ User profile methods
        authServiceFactory.getAuthorizationHeader = function (authorizationData) {
            authorizationData = authorizationData || localStorageService.get(authorizationDataKey);

            if ((authorizationData && authorizationData.token)) {
                return 'Bearer ' + authorizationData.token;
            }

            return null;
        };

        authServiceFactory.isAuthenticated = function () {
            return authData && authData.isAuth && authData.tokenExipreTime >= Date.now();
        }

        authServiceFactory.getCurrentUserAuthorizationData = function () {
            return localStorageService.get(authorizationDataKey);
        }

        authServiceFactory.getCurrentUserProfile = function (force) {

            if (!userProfileData || force) { // check if cached
                userProfileData = localStorageService.get(userProfileDataKey);
                if (!userProfileData || force) { // check in local storage
                    //force refresh
                    var deffered = $q.defer();

                    $http.get("/api/account/user-info")
                        .success(function (response) {
                            userProfileData = response;
                            localStorageService.set(userProfileDataKey, response);
                            deffered.resolve(userProfileData);
                        }).error(function (err, status) {
                            deffered.reject(err);
                        });

                    return deffered.promise;
                }
            }

            return $q.resolve(userProfileData);
        }

        authServiceFactory.fillAuthData = function () {
            var authorizationData = authServiceFactory.getCurrentUserAuthorizationData();
            if (authorizationData) {
                authData.isAuth = true;
                authData.userName = authorizationData.userName;
                authData.tokenExipreTime = Date.now() + (authorizationData.expiresIn * 1000);
            }
        };

        //--- SignInManagerMethods

        authServiceFactory.loginWithRefreshToken = function () {

            var storedData = localStorageService.get(authorizationDataKey);
            if (storedData && storedData.refreshToken) {
                var singInRequestData = {
                    "grant_type": "refresh_token",
                    "client_id": clientId,
                    "refresh_token": storedData.refreshToken
                };

                return login(singInRequestData);
            }

            return $q.reject();
        }
        authServiceFactory.login = function (loginData) {

            var singInRequestData = {
                "grant_type": "password",
                "username": loginData.userName,
                "password": loginData.password,
                "client_id": clientId
            };

            return login(singInRequestData);
        };

        authServiceFactory.logOut = function (logoutCookie) {

            var deffered = $q.defer();

            if (logoutCookie) {
                $http.post("/api/account/logoff", {})
                    .success(function (response) {
                        deffered.resolve(response);
                    }).error(function (err, status) {
                        deffered.reject(err);
                    }).finally(function () {
                        clearUserProfile();
                    });
            } else {
                clearUserProfile();
                deffered.resolve();
            }

            return deffered.promise;
        };

        authServiceFactory.onUserProfileChanged = function (callback) {
            onUserProfileChanged = callback;
        };

        authServiceFactory.saveRegistration = function (registrationData) {
            return $http.post('/api/account/register', registrationData);
        };

        authServiceFactory.changePass = function (oldPass, newPass) {
            return $http.post("/api/account/changePass", { currentPassword: oldPass, newPassword: newPass });
        }

        // -- Users Service methods

        authServiceFactory.getAllUsers = function () {
            return $http.get('/api/account/allusers');
        }

        authServiceFactory.deleteUser = function (userId) {
            return $http.delete('/api/account/delete/' + userId);
        }

        authServiceFactory.changeRoles = function (userId, rolesToRemove, rolesToAdd) {
            return $http.post('/api/account/change-roles/' + userId, { rolesToRemove: rolesToRemove, rolesToAdd: rolesToAdd })
                .then(function (response) {
                    return authServiceFactory.getCurrentUserProfile()
                        .then(function (userProfile) {
                            if (userId === userProfile.id) {
                                return authServiceFactory.loginWithRefreshToken().finally(function () {
                                    $q.resolve(response);
                                });
                            }
                            return $q.resolve(response);
                        });

                }, function (err) {
                    return $q.reject(err);
                });
        }

        authServiceFactory.getAdminPartyState = function () {
            return $http.get('/api/account/admin-party/state');
        }

        authServiceFactory.setAdminPartyState = function (allowed) {
            return $http.post('/api/account/admin-party/change', allowed);
        }

        function clearUserProfile() {
            localStorageService.remove(authorizationDataKey);
            localStorageService.remove(userProfileDataKey);
            authData.isAuth = false;
            authData.userName = "";
            onUserProfileChanged();
        }

        function login(requestLoginData) {
            var deferred = $q.defer();
            $http({
                method: "POST",
                url: '/token',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                transformRequest: function (obj) {
                    var str = [];
                    for (var p in obj)
                        if (obj.hasOwnProperty(p))
                            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));

                    return str.join("&");
                },
                data: requestLoginData
            }).success(function (response) {

                var authorizationData = {
                    token: response.access_token,
                    userName: response.userName,
                    refreshToken: response.refresh_token,
                    expiresIn: response.expires_in,
                    useRefreshTokens: !(response.refresh_token == 'undefined' || response.refresh_token.length === 0)
                };

                localStorageService.set(authorizationDataKey, authorizationData);

                authData.isAuth = true;
                authData.userName = response.userName;
                authData.tokenExipreTime = Date.now() + (authorizationData.expiresIn * 1000);

                authServiceFactory.getCurrentUserProfile(true)
                    .then(function (userProfile) {
                        onUserProfileChanged(userProfile);
                    });

                deferred.resolve(authorizationData);

            }).error(function (err, status) {
                deferred.reject(err);
            });

            return deferred.promise;
        }

        return authServiceFactory;
    }]);
