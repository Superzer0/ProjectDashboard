(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('brokerInstanceController', ['$scope', 'notificationService', '$http', brokerInstanceController]);

    function brokerInstanceController($scope, notificationService, $http) {
        $scope.title = 'brokerInstanceController';
        $scope.brokerInfoLoading = true;
        $scope.broker = undefined;

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.brokerInfoLoading = true;
        $scope.broker = undefined;

        $scope.refreshBrokerInfo = function () {
            $http.get('api/instance/broker-status')
                .then(function (response) {
                    $scope.broker = response.data;
                    $scope.brokerInfoLoading = false;
                }, function (err) {
                    notificationService.addError('broker info', 'could not fetch broker information');
                    $scope.broker = undefined;
                    $scope.brokerInfoLoading = false;
                });
        };

        $scope.refreshBrokerInfo();
    }
})();
