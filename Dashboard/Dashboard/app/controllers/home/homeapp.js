var app = angular.module('homeApp',
        ['dashboardAuth', 'ngRoute', 'LocalStorageModule', 'ngAnimate', 'chart.js']);

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('dashboardAuthMiddlewareInterceptor');
}]);

app.controller('baseController', ['$scope','aplicationParams','$log',
    function ($scope, aplicationParams, $log) {

        $scope.init = function ($attrs) {
            if (!$attrs) {
                $log.warn("$attrs is missing. Initial params not resolved.");
                return;
            }

            var appId = $attrs.appId;
            var initialParams = aplicationParams[appId];
            if (initialParams) {
                $scope.initialParams = initialParams;
            } else {
                $log.warn("Initial params missing. Did you forget to set data-app-id attr on your controller?");
            }
        };
    }]);
