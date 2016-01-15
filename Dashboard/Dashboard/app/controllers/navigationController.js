'use strict';
app.controller('navigationController', ['$scope', '$location', 'authService', 'notificationService',
    function ($scope, $location, authService, notificationService) {

        $scope.notifications = [];
        notificationService.init($scope.notifications);

        $scope.logOut = function () {
            authService.logOut().then(function (response) {
                notificationService.addNotification("Sign out", "Logged out successfully", "");
                $location.path('/home');
            }, function (err) {
                notificationService.addNotification("Sign out", "Error while logging out", "error");
            });
        }

        $scope.authentication = authService.authentication;
    }]);


(function () {
    'use strict';
    angular
        .module('angularAdmin')
        .controller('adminMenuController', indexUserController);

    indexUserController.$inject = ['$scope'];

    function indexUserController($scope) {
        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }
    }
})();