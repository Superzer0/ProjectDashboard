'use strict';
app.controller('navigationController', ['$scope', '$location', 'authService', 'notificationService',
    function ($scope, $location, authService, notificationService) {

        $scope.notifications = [];
        notificationService.init($scope.notifications);

        $scope.logOut = function () {
            authService.logOut(true).then(function (response) {
                notificationService.addNotification("Sign out", "Logged out successfully", "");
                $location.path('/home');
            }, function (err) {
                notificationService.addNotification("Sign out", "Error while logging out", "error");
            });
        }

        $scope.isAuth = function () {
            return authService.isAuthenticated();
        }

        $scope.hasUserRole = function () {
            return hasRole(authService.dashboardRoles.user);
        }

        $scope.hasAdminRole = function () {
            return hasRole(authService.dashboardRoles.admin);
        }

        $scope.hasPluginsRole = function () {
            return hasRole(authService.dashboardRoles.plugins);
        }

        function hasRole(role) {
            return authService.currentUserHasRole(role);
        }

        (function () {
            authService.fillAuthData();
            if ($scope.isAuth()) {
                $scope.userProfile = authService.getUserProfile(true);
            }
        })();
    }]);
