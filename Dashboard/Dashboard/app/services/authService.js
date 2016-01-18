'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'dashboardRoles',
    function ($http, $q, localStorageService, roles) {
        var authorizationDataKey = 'authorizationData';
        var userProfileDataKey = 'userProfileData';
        var authServiceFactory = {};

        var authData = {
            isAuth: false,
            userName: ""
        };

        var dashboardRoles = angular.copy(roles);

        var userProfileData = null;

        var login = function (loginData) {
            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;
            var deferred = $q.defer();

            $http.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                .success(function (response) {

                    localStorageService.set(authorizationDataKey,
                        {
                            token: response.access_token,
                            userName: loginData.userName, refreshToken: ""
                        });

                    authData.isAuth = true;
                    authData.userName = loginData.userName;
                    getUserProfile(true);
                    deferred.resolve(response);

                }).error(function (err, status) {
                    deferred.reject(err);
                });

            return deferred.promise;
        };

        var logOut = function (logoutCookie) {

            var deffered = $q.defer();

            localStorageService.remove(authorizationDataKey);
            authData.isAuth = false;
            authData.userName = "";

            if (logoutCookie) {
                $http.post("/api/account/logoff", {})
                    .success(function (response) {
                        deffered.resolve(response);
                    }).error(function (err, status) {
                        deffered.reject(err);
                    });
            } else {
                deffered.resolve();
            }

            return deffered.promise;
        };

        var saveRegistration = function (registrationData) {
            return $http.post('/api/account/register', registrationData)
                .then(function (response) {
                    return response;
                });
        };

        var changePass = function (oldPass, newPass) {
            return $http.post("/api/account/changePass", { currentPassword: oldPass, newPassword: newPass });
        }

        var fillAuthData = function () {
            var storedData = localStorageService.get(authorizationDataKey);
            if (storedData) {
                authData.isAuth = true;
                authData.userName = storedData.userName;
            }
        };

        var refreshUserProfile = function () {
            var deffered = $q.defer();

            $http.get("/api/account/user-info")
                .success(function (response) {
                    userProfileData = response;
                    localStorageService.set(userProfileDataKey, response);
                    deffered.resolve(response);
                }).error(function (err, status) {
                    deffered.reject(err);
                });

            return deffered.promise;
        }

        var getUserProfile = function (force) {

            if (!userProfileData || force) { // check if cached
                userProfileData = localStorageService.get(userProfileDataKey);
                if (!userProfileData || force) { // check in local storage
                    //force refresh
                    $q.all([refreshUserProfile()]).then(function () {
                        return userProfileData;
                    });
                }
            }
            return userProfileData;
        }

        var getAllUsers = function () {
            return $http.get('/api/account/allusers');
        }

        var deleteUser = function (userId) {
            return $http.delete('/api/account/delete/' + userId);
        }

        var isAuthenticated = function () {
            return authData && authData.isAuth;
        }

        var currentUserHasRole = function (role) {
            if (!isAuthenticated()) return false;

            var userProfile = getUserProfile(); //refresh userProfile
            return userProfile.roles.indexOf(role) >= 0;
        }

        var changeRoles = function (userId, rolesToRemove, rolesToAdd) {
            return $http.post('/api/account/change-roles/' + userId, { rolesToRemove: rolesToRemove, rolesToAdd: rolesToAdd });
        }

        var getAdminPartyState = function () {
            return $http.get('/api/account/admin-party/state');
        }

        var setAdminPartyState = function (allowed) {
            return $http.post('/api/account/admin-party/change', allowed);
        }

        authServiceFactory.saveRegistration = saveRegistration;
        authServiceFactory.login = login;
        authServiceFactory.logOut = logOut;
        authServiceFactory.fillAuthData = fillAuthData;
        authServiceFactory.authentication = authData;
        authServiceFactory.changePass = changePass;
        authServiceFactory.refreshUserProfile = refreshUserProfile;
        authServiceFactory.getUserProfile = getUserProfile;
        authServiceFactory.getAllUsers = getAllUsers;
        authServiceFactory.deleteUser = deleteUser;
        authServiceFactory.isAuthenticated = isAuthenticated;
        authServiceFactory.dashboardRoles = angular.copy(dashboardRoles);
        authServiceFactory.currentUserHasRole = currentUserHasRole;
        authServiceFactory.changeRoles = changeRoles;
        authServiceFactory.getAdminPartyState = getAdminPartyState;
        authServiceFactory.setAdminPartyState = setAdminPartyState;

        return authServiceFactory;
    }]);