(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexUserController', indexUserController);

    indexUserController.$inject = ['$scope'];
    indexUserController.$inject = ['authService'];
    indexUserController.$inject = ['notificationService'];

    function indexUserController($scope, authService, notificationService) {
        var isWorking = false;

        $scope.message = "";

        $scope.changePassData = {
            oldPass: "",
            newPass: "",
            confirmNewPass: ""
        };

        $scope.passNotMatch = function () {
            return $scope.changePassData.newPass !== $scope.changePassData.confirmNewPass;
        }

        $scope.submitNotPermitted = function () {
            return $scope.changePassForm.$invalid || $scope.passNotMatch() || $scope.isWorking;
        }

        $scope.changePassword = function () {
            isWorking = true;

            if ($scope.submitNotPermitted()) return;

            authService.changePass($scope.changePassData.oldPass, $scope.changePassData.newPass).success(function () {
                notificationService.addNotification("password change", "password has been changed successfuly");
                isWorking = false;
            }).error(function () {
                notificationService.addNotification("password change", "error while changing password");
                isWorking = false;
            });
        }

        activate();

        function activate() { }
    }
})();
