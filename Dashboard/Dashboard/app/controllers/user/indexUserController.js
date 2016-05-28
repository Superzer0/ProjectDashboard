(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexUserController', ['$scope', 'authService', 'notificationService',
            'utilsService', 'userProfile', 'dashboardRolesNames', 'instancePluginsService',
            indexUserController]);

    function indexUserController($scope, authService, notificationService, utilsService, userProfile, dashboardRolesNames, instancePluginsService) {
        var isWorking = false;

        $scope.userProfile = userProfile;

        $scope.changePasswordResult = null;

        $scope.changePassData = {
            oldPass: "",
            newPass: "",
            confirmNewPass: ""
        };

        $scope.activePlugins = null;

        $scope.getRoleName = function (role) {
            return dashboardRolesNames[role];
        }

        $scope.passNotMatch = function () {
            return $scope.changePassData.newPass !== $scope.changePassData.confirmNewPass;
        }

        $scope.submitNotPermitted = function () {
            return $scope.changePassForm.$invalid || $scope.passNotMatch() || isWorking;
        }

        $scope.loadActiveUserPlugins = function () {
            instancePluginsService.loadActiveUserPlugins()
                .success(function (data) {
                    if (!angular.equals([], data)) {
                        $scope.activePlugins = data;
                    }

                }).error(function (err) {
                    notificationService.addError('active plugins', 'error while loading active plugins list');
                    console.log(err);
                });
        }

        $scope.changePassword = function () {
            if ($scope.submitNotPermitted()) return;

            $scope.changePasswordResult = null;
            isWorking = true;

            authService.changePass($scope.changePassData.oldPass, $scope.changePassData.newPass).success(function () {
                notificationService.addSuccess("password change", "password has been changed successfuly");
                isWorking = false;
                $scope.changePassData = {};
                $scope.changePassForm.$setPristine();
            }).error(function (err) {
                $scope.changePasswordResult = {
                    errors: utilsService.parseModelStateErrors(err),
                    message: "Correct errors listed below: "
                };

                notificationService.addError("password change", "error while changing password");

                isWorking = false;
            });
        }

        $scope.refreshUserProfile = function () {
            authService.getCurrentUserProfile(true)
                .then(function (userProfile) {
                    $scope.userProfile = userProfile;
                });
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.loadActiveUserPlugins();
    }
})();
