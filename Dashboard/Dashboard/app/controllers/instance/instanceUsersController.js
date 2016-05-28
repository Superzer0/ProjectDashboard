(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('instanceUsersController', ['$scope', 'authService', 'notificationService', instanceUsersController]);

    function instanceUsersController($scope, authService, notificationService) {

        $scope.userList = [];
        $scope.isWorking = false;
        $scope.adminPartyState = false;

        var initRoleManagerValues = {
            isAdmin: false,
            isPluginsManager: false,
            userName: "",
            userId: ""
        };

        $scope.manageRoleModel = angular.copy(initRoleManagerValues);

        var showRolesModal = function () {
            $('#manage-roles-modal').modal('show');
        }

        var dismissRolesModal = function () {
            $('#manage-roles-modal').modal('hide');
        }

        $scope.refreshUserList = function () {
            authService.getAllUsers()
                .success(function (response) {
                    $scope.userList = response;
                }).error(function (err) {
                    notificationService.addSuccess("users list", "failed to update users list", "error");
                });
        }

        $scope.deleteUser = function (userId, userName) {
            if (!confirm("Are you sure to delete " + userName + "?")) return;

            $scope.isWorking = true;
            authService.deleteUser(userId)
                .success(function (response) {
                    notificationService.addSuccess("delete user", "user " + userName + " has been deleted");
                    $scope.refreshUserList();
                    $scope.isWorking = false;
                }).error(function (err) {
                    notificationService.addError("deleted user", "user " + userName + " could not be deleted");
                    $scope.isWorking = false;
                });
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.manageRoles = function (userId, userName, roles) {

            $scope.manageRoleModel.isAdmin = roles.indexOf(authService.dashboardRoles.admin) >= 0;
            $scope.manageRoleModel.isPluginsManager = roles.indexOf(authService.dashboardRoles.plugins) >= 0;

            $scope.manageRoleModel.userId = userId;
            $scope.manageRoleModel.userName = userName;

            showRolesModal();
        }

        $scope.updateUserRoles = function () {

            var rolesToRemove = [];
            var rolesToAdd = [];

            if ($scope.manageRoleModel.isAdmin) {
                rolesToAdd.push(authService.dashboardRoles.admin);
            } else {
                rolesToRemove.push(authService.dashboardRoles.admin);
            }

            if ($scope.manageRoleModel.isPluginsManager) {
                rolesToAdd.push(authService.dashboardRoles.plugins);
            } else {
                rolesToRemove.push(authService.dashboardRoles.plugins);
            }

            $scope.isWorking = true;
            authService.changeRoles($scope.manageRoleModel.userId, rolesToRemove, rolesToAdd)
                .then(function (response) {
                    notificationService.addSuccess("roles", "user might need to re-login to see the effects");
                    dismissRolesModal();
                    $scope.manageRoleModel = angular.copy(initRoleManagerValues);
                    $scope.refreshUserList();
                    $scope.isWorking = false;
                }, function (err) {
                    $scope.isWorking = false;
                    console.log(err);
                    dismissRolesModal();
                    notificationService.addError("roles", "an error occured, try again later.");
                });
        }

        $scope.getAdminPartyState = function () {
            authService.getAdminPartyState()
                .success(function (response) {
                    $scope.adminPartyState = response;
                }).error(function () {

                });
        }

        $scope.setAdminPartyState = function () {
            authService.setAdminPartyState($scope.adminPartyState)
               .success(function () {
                   if ($scope.adminPartyState) {
                       notificationService.addSuccess("roles", "admin party on");
                   } else {
                       notificationService.addSuccess("roles", "admin party off");
                   }
               }).error(function () {
                   notificationService.addError("roles", "error occured while setting admin party property");
                   $scope.adminPartyState = !$scope.adminPartyState;
               });
        }

        $scope.refreshUserList();
        $scope.getAdminPartyState();
    }
})();
