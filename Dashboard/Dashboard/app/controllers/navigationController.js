'use strict';
app.controller('navigationController', ['$scope', '$location', 'authService', 'notificationService',
    function ($scope, $location, authService, notificationService) {

        $scope.notifications = [];
        notificationService.init($scope.notifications);

        $scope.logOut = function () {
            authService.logOut(true)
                .then(function (response) {
                    $location.path('/home');
                }, function (err) {
                    notificationService.addWarning("Sign out", "Error while logging out");
                });
        }

        $scope.isAuth = function () {
            return authService.isAuthenticated();
        }

        function hasRole(userProfile, role) {
            return userProfile.roles.indexOf(role) >= 0;
        }

        authService.onUserProfileChanged(function (userProfile) {
            if (userProfile) {
                $scope.userProfileGetter = userProfile;
                $scope.hasUserRole = hasRole(userProfile, authService.dashboardRoles.user);
                $scope.hasAdminRole = hasRole(userProfile, authService.dashboardRoles.admin);
                $scope.hasPluginsRole = hasRole(userProfile, authService.dashboardRoles.plugins);
            } else {
                $scope.userProfileGetter = null;
                $scope.hasUserRole = false;
                $scope.hasAdminRole = false;
                $scope.hasPluginsRole = false;
            }
        });
    }]);
