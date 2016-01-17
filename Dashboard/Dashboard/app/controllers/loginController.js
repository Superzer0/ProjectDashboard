'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', 'notificationService',
    function ($scope, $location, authService, notificationService) {

        $scope.loginData = {
            userName: "",
            password: "",
            useRefreshTokens: false
        };

        $scope.isLogging = false;
        $scope.message = "";

        $scope.login = function () {
            $scope.isLogging = true;
            $scope.formClass = "";
            authService.login($scope.loginData).then(function (response) {
                notificationService.removeAll();
                $location.path('/user/index');
            },
             function (err) {
                 $scope.isLogging = false;
                 $scope.message = err.error_description;
                 $scope.formClass = "shake animated";
             });
        };
    }]);
