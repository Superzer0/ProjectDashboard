'use strict';
app.controller('loginController', ['$scope', '$location', 'authService',
    function ($scope, $location, authService) {

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
                $location.path('/user');
            },
             function (err) {
                 $scope.isLogging = false;
                 $scope.message = err.error_description;
                 $scope.formClass = "shake animated";
             });
        };
    }]);
