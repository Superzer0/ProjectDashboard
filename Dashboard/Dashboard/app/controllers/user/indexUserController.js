(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexUserController', ['$scope', 'authService', 'notificationService', 'utilsService', 'userProfile',
            indexUserController]);

    function indexUserController($scope, authService, notificationService, utilsService, userProfile) {
        var isWorking = false;

        $scope.userProfile = userProfile;

        $scope.changePasswordResult = null;

        $scope.changePassData = {
            oldPass: "",
            newPass: "",
            confirmNewPass: ""
        };

        

        $scope.passNotMatch = function () {
            return $scope.changePassData.newPass !== $scope.changePassData.confirmNewPass;
        }

        $scope.submitNotPermitted = function () {
            return $scope.changePassForm.$invalid || $scope.passNotMatch() || isWorking;
        }

        $scope.changePassword = function () {
            if ($scope.submitNotPermitted()) return;

            $scope.changePasswordResult = null;
            isWorking = true;

            authService.changePass($scope.changePassData.oldPass, $scope.changePassData.newPass).success(function () {
                notificationService.addNotification("password change", "password has been changed successfuly");
                isWorking = false;
                $scope.changePassData = {};
                $scope.changePassForm.$setPristine();
            }).error(function (err) {
                $scope.changePasswordResult = {
                    errors: utilsService.parseModelStateErrors(err),
                    message: "Correct errors listed below: "
                };

                notificationService.addNotification("password change", "error while changing password", "error");

                isWorking = false;
            });
        }

        $scope.getUserData = function () {
            $scope.userProfile = authService.getUserProfile();
        }

        $scope.refreshUserProfile = function () {
            $scope.userProfile = authService.getUserProfile(true);
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

       

        activate();

        function activate() {
            //$scope.getUserData();
        }
    }
})();
