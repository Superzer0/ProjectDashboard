'use strict';
app.controller('homeController', ['$scope', 'authService', function ($scope, authService) {

    $scope.isAuth = function () {
        return authService.isAuthenticated();
    }

}]);